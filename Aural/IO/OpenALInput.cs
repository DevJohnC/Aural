// 
// Author: John Carruthers (johnc@frag-labs.com)
// 
// Copyright (C) 2013 John Carruthers
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//  
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//  
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using FragLabs.Aural.IO.OpenAL;

namespace FragLabs.Aural.IO
{
    /// <summary>
    /// OpenAL audio input.
    /// </summary>
    public class OpenALInput : IAudioInput
    {
        /// <summary>
        /// Gets the available input device names.
        /// </summary>
        /// <returns></returns>
        public static string[] GetInputDevices()
        {
            return ReadStringsFromMemory(API.alcGetString(IntPtr.Zero, (int)ALCStrings.ALC_CAPTURE_DEVICE_SPECIFIER));
        }

        internal static string[] ReadStringsFromMemory(IntPtr location)
        {
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

        /// <summary>
        /// OpenAL device.
        /// </summary>
        private IntPtr _device;

        /// <summary>
        /// Size of each sample in bytes.
        /// </summary>
        private readonly int _sampleSize;

        /// <summary>
        /// Number of samples to read from StartReading.
        /// </summary>
        internal int ReadSampleCount;

        /// <summary>
        /// Buffer to write to from StartReading.
        /// </summary>
        internal byte[] ReadBuffer;

        /// <summary>
        /// Opens a device for input.
        /// </summary>
        /// <param name="deviceName">Name of input device to open.</param>
        /// <param name="readSampleRate">Sample rate in Hz.</param>
        /// <param name="bitDepth">Bitdepth, 8 or 16.</param>
        /// <param name="channelCount">Number of channel, 1 or 2.</param>
        /// <param name="internalBufferSize">Size of internal sample buffer in bytes.</param>
        public OpenALInput(string deviceName, int readSampleRate, int bitDepth, int channelCount, int internalBufferSize)
        {
            if (deviceName == null) throw new ArgumentNullException("deviceName");
            if (bitDepth != 8 && bitDepth != 16) throw new ArgumentOutOfRangeException("bitDepth","Only 8 or 16 bitdepths are supported.");
            if (channelCount != 1 && channelCount != 2) throw new ArgumentOutOfRangeException("channelCount", "Only 1 or 2 channels are supported.");

            Name = deviceName;

            var format = AudioFormat.Unknown;
            if (bitDepth == 8 && channelCount == 1)
                format = AudioFormat.Mono8Bit;
            if (bitDepth == 8 && channelCount == 2)
                format = AudioFormat.Stereo8Bit;
            if (bitDepth == 16 && channelCount == 1)
                format = AudioFormat.Mono16Bit;
            if (bitDepth == 16 && channelCount == 2)
                format = AudioFormat.Stereo16Bit;

            _sampleSize = FormatHelper.SampleSize(bitDepth, channelCount);
            _device = API.alcCaptureOpenDevice(deviceName, (uint)readSampleRate, format, internalBufferSize);
            API.alcCaptureStart(_device);
        }

        ~OpenALInput()
        {
            Dispose();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            StopReading();
            if (_device != IntPtr.Zero)
            {
                API.alcCaptureStop(_device);
                API.alcCaptureCloseDevice(_device);
                _device = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Event raised when audio data is received.
        /// </summary>
        public event EventHandler<AudioReceivedEventArgs> AudioReceived;

        protected virtual void OnAudioReceived(AudioReceivedEventArgs e)
        {
            var handler = AudioReceived;
            if (handler != null) handler(this, e);
        }

        /// <summary>
        /// Gets the name of the audio input.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets if the audio input is currently being read.
        /// </summary>
        public bool IsReading { get; private set; }

        /// <summary>
        /// Starts reading audio samples from the audio input. Will raise AudioReceived when audio samples are read from the input.
        /// </summary>
        /// <param name="sampleCount">The number of samples, per channel, to read before raising the AudioReceived event.</param>
        /// <remarks>sampleCount should be small enough to allow any processing of audio samples without overrunning the internal buffer.</remarks>
        public void StartReading(int sampleCount)
        {
            ReadSampleCount = sampleCount;
            var bufferSize = sampleCount*_sampleSize;
            ReadBuffer = new byte[bufferSize];
            IsReading = true;
            Runner.Add(this);
        }

        /// <summary>
        /// Stops reading audio samples from the audio input.
        /// </summary>
        public void StopReading()
        {
            if (IsReading)
            {
                Runner.Remove(this);
                IsReading = false;
            }
        }

        /// <summary>
        /// Internal read complete call from Runner.
        /// </summary>
        internal void ReadComplete(int byteCount)
        {
            OnAudioReceived(new AudioReceivedEventArgs
                {
                    Buffer = ReadBuffer,
                    ByteCount = byteCount,
                    SampleCount = byteCount/_sampleSize
                });
        }

        /// <summary>
        /// Reads audio samples from the input device into a buffer.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values starting at offset replaced with audio samples.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin writing audio samples.</param>
        /// <param name="sampleCount">The number of samples, per channel, to read.</param>
        /// <returns>The total number of bytes written to buffer.</returns>
        public int Read(byte[] buffer, int offset, int sampleCount)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            var sampleLength = _sampleSize*sampleCount;
            if (offset + sampleLength > buffer.Length)
                throw new Exception("Not enough space in buffer for requested samples");
            var samplesAvailable = GetSamplesAvailable();
            while (samplesAvailable < sampleCount)
            {
                Thread.Sleep(1);
                samplesAvailable = GetSamplesAvailable();
            }
            ReadIntoBuffer(buffer, offset, sampleCount);
            return sampleLength;
        }

        /// <summary>
        /// Gets the number of samples available for reading immediately.
        /// </summary>
        /// <returns></returns>
        public int GetSamplesAvailable()
        {
            if (_device == IntPtr.Zero)
                return 0;

            int samples;
            API.alcGetIntegerv(_device, ALCEnum.ALC_CAPTURE_SAMPLES, 4, out samples);
            //  todo: error checking
            return samples;
        }

        /// <summary>
        /// Copy samples into a buffer.
        /// </summary>
        /// <param name="buffer">Buffer to copy samples into.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin writing audio samples.</param>
        /// <param name="samples">Number of samples to copy.</param>
        unsafe void ReadIntoBuffer(byte[] buffer, int offset, int samples)
        {
            fixed (byte* bbuff = buffer)
            {
                var buffPtr = IntPtr.Add(new IntPtr(bbuff), offset);
                API.alcCaptureSamples(_device, buffPtr, samples);
            }
        }
    }
}