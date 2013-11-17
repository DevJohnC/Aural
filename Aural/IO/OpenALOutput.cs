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
using FragLabs.Aural.IO.OpenAL;

namespace FragLabs.Aural.IO
{
    /// <summary>
    /// OpenAL audio output.
    /// </summary>
    public class OpenALOutput : IAudioOutput
    {
        public uint OutputSampleRate { get; private set; }
        public int BitDepth { get; private set; }
        public int ChannelCount { get; private set; }

        /// <summary>
        /// Size of each sample in bytes.
        /// </summary>
        private readonly int _sampleSize;

        /// <summary>
        /// OpenAL playback source.
        /// </summary>
        private uint _source;

        private readonly AudioFormat _format;

        /// <summary>
        /// Available buffers.
        /// </summary>
        private readonly Queue<uint> _availableBuffers = new Queue<uint>();

        /// <summary>
        /// OpenAL context.
        /// </summary>
        private IntPtr _context;

        public OpenALOutput(string deviceName, int outputSampleRate, int bitDepth, int channelCount)
        {
            OutputSampleRate = (uint)outputSampleRate;
            BitDepth = bitDepth;
            ChannelCount = channelCount;
            if (deviceName == null) throw new ArgumentNullException("deviceName");
            if (bitDepth != 8 && bitDepth != 16) throw new ArgumentOutOfRangeException("bitDepth", "Only 8 or 16 bitdepths are supported.");
            if (channelCount != 1 && channelCount != 2) throw new ArgumentOutOfRangeException("channelCount", "Only 1 or 2 channels are supported.");

            _format = AudioFormat.Unknown;
            if (bitDepth == 8 && channelCount == 1)
                _format = AudioFormat.Mono8Bit;
            if (bitDepth == 8 && channelCount == 2)
                _format = AudioFormat.Stereo8Bit;
            if (bitDepth == 16 && channelCount == 1)
                _format = AudioFormat.Mono16Bit;
            if (bitDepth == 16 && channelCount == 2)
                _format = AudioFormat.Stereo16Bit;

            _sampleSize = FormatHelper.SampleSize(bitDepth, channelCount);
            _context = PlaybackDevice.GetContext(deviceName);
            //  todo: error checking
            CreateSource();
            CreateBuffers(6);
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
                foreach(var buffer in buffers)
                    _availableBuffers.Enqueue(buffer);
            }
        }

        public unsafe void Write(byte[] pcmBuffer, int offset, int sampleCount)
        {
            var pcmLength = _sampleSize*sampleCount;
            if (pcmLength > pcmBuffer.Length - offset)
                throw new Exception("Sample count is too large to be read from pcm buffer.");
            if (_availableBuffers.Count == 0)
                CreateBuffers(1);
            lock (typeof (PlaybackDevice))
            {
                API.alcMakeContextCurrent(_context);
                var bufferId = _availableBuffers.Dequeue();
                fixed (byte* bPcm = pcmBuffer)
                {
                    var pcmPtr = IntPtr.Add(new IntPtr(bPcm), offset);
                    API.alBufferData(bufferId, _format, pcmPtr, pcmLength, OutputSampleRate);
                    API.alSourceQueueBuffers(_source, 1, new[] {bufferId});
                }
            }
            CleanupPlayedBuffers();
        }

        private void CleanupPlayedBuffers()
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
                foreach (var buffer in removedBuffers)
                {
                    _availableBuffers.Enqueue(buffer);
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            
        }
    }
}