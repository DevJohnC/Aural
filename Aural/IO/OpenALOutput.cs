//  
//  Author: John Carruthers (johnc@frag-labs.com)
//  
//  Copyright (C) 2013 John Carruthers
//  
//  Permission is hereby granted, free of charge, to any person obtaining
//  a copy of this software and associated documentation files (the
//  "Software"), to deal in the Software without restriction, including
//  without limitation the rights to use, copy, modify, merge, publish,
//  distribute, sublicense, and/or sell copies of the Software, and to
//  permit persons to whom the Software is furnished to do so, subject to
//  the following conditions:
//   
//  The above copyright notice and this permission notice shall be
//  included in all copies or substantial portions of the Software.
//   
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//  WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//  

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FragLabs.Aural.IO.OpenAL;
using ALAudioFormat = FragLabs.Aural.IO.OpenAL.AudioFormat;

namespace FragLabs.Aural.IO
{
    /// <summary>
    /// OpenAL audio output.
    /// </summary>
    public class OpenALOutput : IAudioOutput
    {
        public static OpenALOutput[] GetOutputDevices()
        {
            var strings = new string[0];
            if (GetIsExtensionPresent("ALC_ENUMERATE_ALL_EXT"))
            {
                strings =
                    ReadStringsFromMemory(API.alcGetString(IntPtr.Zero,
                                                            (int)ALCStrings.ALC_ALL_DEVICES_SPECIFIER));
            }
            else if (GetIsExtensionPresent("ALC_ENUMERATION_EXT"))
            {
                strings =
                    ReadStringsFromMemory(API.alcGetString(IntPtr.Zero, (int)ALCStrings.ALC_DEVICE_SPECIFIER));
            }
            var ret = new OpenALOutput[strings.Length];
            for (var i = 0; i < strings.Length; i++)
            {
                ret[i] = new OpenALOutput(strings[i]);
            }
            return ret;
        }

        private static string[] ReadStringsFromMemory(IntPtr location)
        {
            if (location == IntPtr.Zero)
                return new string[0];

            var strings = new List<string>();

            var lastNull = false;
            var i = -1;
            byte c;
            while (!((c = Marshal.ReadByte(location, ++i)) == '\0' && lastNull))
            {
                if (c == '\0')
                {
                    lastNull = true;

                    strings.Add(Marshal.PtrToStringAnsi(location, i));
                    location = new IntPtr((long)location + i + 1);
                    i = -1;
                }
                else
                    lastNull = false;
            }

            return strings.ToArray();
        }

        private static bool GetIsExtensionPresent(string extension)
        {
            sbyte result;
            if (extension.StartsWith("ALC"))
            {
                result = API.alcIsExtensionPresent(IntPtr.Zero, extension);
            }
            else
            {
                result = API.alIsExtensionPresent(extension);
                //  todo: check for errors here
            }

            return (result == 1);
        }

        public uint OutputSampleRate { get; private set; }
        public int BitDepth { get; private set; }
        public int ChannelCount { get; private set; }

        /// <summary>
        /// Size of each sample in bytes.
        /// </summary>
        private int _sampleSize;

        /// <summary>
        /// OpenAL playback source.
        /// </summary>
        private uint _source;

        private ALAudioFormat _format;

        /// <summary>
        /// Available buffers.
        /// </summary>
        private readonly Queue<uint> _availableBuffers = new Queue<uint>();

        /// <summary>
        /// Buffers queued for playback on the source.
        /// </summary>
        private readonly List<uint> _queuedBuffers = new List<uint>();

        private readonly Dictionary<uint,int> _bufferSampleCounts = new Dictionary<uint, int>(); 

        /// <summary>
        /// OpenAL context.
        /// </summary>
        private IntPtr _context;

        /// <summary>
        /// Token issued by PlaybackDevice.
        /// </summary>
        private int _token;

        private int _queuedSampleCount;

        internal OpenALOutput(string deviceName)
        {
            if (deviceName == null) throw new ArgumentNullException("deviceName");
            Name = deviceName;
        }

        ~OpenALOutput()
        {
            Dispose();
        }

        private void CreateSource()
        {
            lock (typeof (PlaybackDevice))
            {
                API.alcMakeContextCurrent(_context);
                var sources = new uint[1];
                API.alGenSources(1, sources);
                _source = sources[0];
            }
        }

        private void CreateBuffers(int bufferCount)
        {
            lock (typeof (PlaybackDevice))
            {
                API.alcMakeContextCurrent(_context);
                var buffers = new uint[bufferCount];
                API.alGenBuffers(bufferCount, buffers);
                foreach (var buffer in buffers)
                    _availableBuffers.Enqueue(buffer);
            }
        }

        public string Name { get; private set; }

        /// <summary>
        /// Gets the number of frames queued for playback.
        /// </summary>
        public int QueuedSampleCount
        {
            get { CleanupPlayedBuffers(); return _queuedSampleCount; }
            private set { _queuedSampleCount = value; }
        }

        public AudioFormat Format { get; private set; }

        public void Open(AudioFormat outputFormat)
        {
            if (IsOpen)
                Close();

            OutputSampleRate = (uint)outputFormat.SampleRate;
            BitDepth = outputFormat.BitDepth;
            ChannelCount = outputFormat.Channels;
            if (outputFormat.BitDepth != 8 && outputFormat.BitDepth != 16) throw new ArgumentOutOfRangeException("outputFormat", "Only 8 or 16 bitdepths are supported.");
            if (outputFormat.Channels != 1 && outputFormat.Channels != 2) throw new ArgumentOutOfRangeException("outputFormat", "Only 1 or 2 channels are supported.");

            _format = ALAudioFormat.Unknown;
            if (outputFormat.BitDepth == 8 && outputFormat.Channels == 1)
                _format = ALAudioFormat.Mono8Bit;
            if (outputFormat.BitDepth == 8 && outputFormat.Channels == 2)
                _format = ALAudioFormat.Stereo8Bit;
            if (outputFormat.BitDepth == 16 && outputFormat.Channels == 1)
                _format = ALAudioFormat.Mono16Bit;
            if (outputFormat.BitDepth == 16 && outputFormat.Channels == 2)
                _format = ALAudioFormat.Stereo16Bit;

            _sampleSize = FormatHelper.SampleSize(outputFormat.BitDepth, outputFormat.Channels);
            _context = PlaybackDevice.GetContext(Name);
            _token = PlaybackDevice.GetToken(Name);
            //  todo: error checking
            CreateSource();
            CreateBuffers(6);
        }

        public void Close()
        {
            if (IsOpen)
            {
                if (_source != 0)
                {
                    CleanupPlayedBuffers(true);
                    var buffers = _queuedBuffers.ToArray();
                    var removedBuffers = new uint[buffers.Length];
                    API.alSourceUnqueueBuffers(_source, buffers.Length, removedBuffers);
                    API.alDeleteBuffers(buffers.Length, removedBuffers);
                    lock (typeof(PlaybackDevice))
                    {
                        API.alcMakeContextCurrent(_context);
                        API.alDeleteSources(1, new[] { _source });
                        _source = 0;
                    }
                }
                if (_context != IntPtr.Zero)
                    _context = IntPtr.Zero;
                if (_token != 0)
                {
                    PlaybackDevice.RetireToken(_token);
                    _token = 0;
                }
            }
        }

        /// <summary>
        /// Begins playback.
        /// </summary>
        public void Play()
        {
            lock (typeof (PlaybackDevice))
            {
                API.alcMakeContextCurrent(_context);
                API.alSourcePlay(_source);
            }
        }

        /// <summary>
        /// Pauses playback.
        /// </summary>
        public void Pause()
        {
            lock (typeof(PlaybackDevice))
            {
                API.alcMakeContextCurrent(_context);
                API.alSourcePause(_source);
            }
        }

        public unsafe int Write(byte[] pcmBuffer, int offset, int sampleCount)
        {
            var pcmLength = _sampleSize * sampleCount;
            if (pcmLength > pcmBuffer.Length - offset)
                throw new Exception("Sample count is too large to be read from pcm buffer.");
            if (_availableBuffers.Count == 0)
                CreateBuffers(1);
            lock (typeof(PlaybackDevice))
            {
                API.alcMakeContextCurrent(_context);
                var bufferId = _availableBuffers.Dequeue();
                fixed (byte* bPcm = pcmBuffer)
                {
                    var pcmPtr = IntPtr.Add(new IntPtr(bPcm), offset);
                    API.alBufferData(bufferId, _format, pcmPtr, pcmLength, OutputSampleRate);
                    API.alSourceQueueBuffers(_source, 1, new[] { bufferId });
                }
                _bufferSampleCounts[bufferId] = sampleCount;
                _queuedBuffers.Add(bufferId);
                QueuedSampleCount += sampleCount;
            }
            CleanupPlayedBuffers();
            return sampleCount;
        }

        /// <summary>
        /// Stops playback.
        /// </summary>
        public void Stop()
        {
            lock (typeof(PlaybackDevice))
            {
                API.alcMakeContextCurrent(_context);
                API.alSourceStop(_source);
            }
        }

        private SourceState State
        {
            get
            {
                if (_source == 0)
                    return SourceState.Uninitialized;

                int state;
                API.alGetSourcei(_source, IntSourceProperty.AL_SOURCE_STATE, out state);

                return (SourceState)state;
            }
        }

        /// <summary>
        /// Gets if the device is playing audio.
        /// </summary>
        public bool IsPlaying
        {
            get { return (State == SourceState.Playing); }
        }

        /// <summary>
        /// Gets if the device is paused.
        /// </summary>
        public bool IsPaused
        {
            get { return (State == SourceState.Paused); }
        }

        public bool IsOpen
        {
            get
            {
                return _source != 0;
            }
        }

        /// <summary>
        /// Gets if the device is stopped.
        /// </summary>
        public bool IsStopped
        {
            get { return (State == SourceState.Stopped); }
        }

        private void CleanupPlayedBuffers(bool destroy = false)
        {
            if (_source == 0) return;
            lock (typeof (PlaybackDevice))
            {
                API.alcMakeContextCurrent(_context);
                int buffers;
                API.alGetSourcei(_source, IntSourceProperty.AL_BUFFERS_PROCESSED, out buffers);
                if (buffers < 1) return;

                var removedBuffers = new uint[buffers];
                API.alSourceUnqueueBuffers(_source, buffers, removedBuffers);
                if (!destroy)
                {
                    foreach (var buffer in removedBuffers)
                    {
                        _availableBuffers.Enqueue(buffer);
                    }
                }
                else
                {
                    API.alDeleteBuffers(buffers, removedBuffers);
                }
                foreach (var bufferId in removedBuffers)
                {
                    var sampleCount = 0;
                    if (_bufferSampleCounts.TryGetValue(bufferId, out sampleCount))
                    {
                        _queuedSampleCount -= sampleCount;
                    }
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (IsOpen)
                Close();
        }
    }
}