// 
// Author: John Carruthers (johnc@frag-labs.com)
// 
// Copyright (C) 2014 John Carruthers
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
using FragLabs.Aural.IO.RaspberryPi;

namespace FragLabs.Aural.IO
{
    /// <summary>
    /// 
    /// </summary>
    public class RaspberryPiOutput : IAudioOutput
    {
        private readonly OutputDest _dest;
        /// <summary>
        /// size of each buffer in bytes
        /// </summary>
        private uint _bufferSize;

        private int _samplesPerBuffer;

        public static RaspberryPiOutput[] GetOutputDevices()
        {
            if (!API.IsSupported)
                return new RaspberryPiOutput[0];
            return new []
                {
                    new RaspberryPiOutput(OutputDest.HDMI),
                    new RaspberryPiOutput(OutputDest.AudioJack) 
                };
        }

        private IntPtr _device;

        internal RaspberryPiOutput(OutputDest dest)
        {
            _dest = dest;
            Name = (dest == OutputDest.HDMI) ? "HDMI" : "3.5mm Jack";
        }

        public void Dispose()
        {
            if (IsOpen)
                Close();
        }

        public bool IsPlaying
        {
            get
            {
                if (!IsOpen)
                    return false;
                return (OmxState) API.rpi_audio_get_state(_device) == OmxState.Executing;
            }
        }

        public bool IsStopped
        {
            get
            {
                if (!IsOpen)
                    return false;
                return (OmxState)API.rpi_audio_get_state(_device) == OmxState.Idle;
            }
        }

        public bool IsPaused
        {
            get
            {
                if (!IsOpen)
                    return false;
                return (OmxState)API.rpi_audio_get_state(_device) == OmxState.Pause;
            }
        }

        /// <summary>
        /// Gets a value indicating if the audio output is open.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return _device != IntPtr.Zero;
            }
        }

        /// <summary>
        /// Gets the device name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the number of frames queued for playback. May not be precise.
        /// </summary>
        public int QueuedSampleCount
        {
            get
            {
                if (!IsOpen)
                    throw new Exception("Must open device first.");
                return (int)API.rpi_audio_unplayed_samples(_device);
            }
        }

        /// <summary>
        /// Gets the audio format.
        /// </summary>
        public AudioFormat Format { get; private set; }

        public void Open(AudioFormat format)
        {
            if (IsOpen)
                Close();
            _bufferSize = (uint)(FormatHelper.SampleSize(format.BitDepth, format.Channels) * format.SampleRate) / 10;
            _samplesPerBuffer = (int)_bufferSize/FormatHelper.SampleSize(format.BitDepth, format.Channels);
            Format = format;
            //  create a device that can buffer 1s of audio in 10 discrete buffers.
            _device = API.rpi_audio_create((uint) format.SampleRate, (uint) format.Channels, (uint) format.BitDepth,
                                           10, _bufferSize);
            if (_device == IntPtr.Zero)
            {
                throw new Exception("Failed to open device.");
            }
            var dest = (uint) _dest;
            API.rpi_audio_set_dest(_device, dest);
        }

        public void Close()
        {
            if (IsOpen)
            {
                API.rpi_audio_destroy(_device);
                _device = IntPtr.Zero;
            }
        }

        public void Play()
        {
            if (IsOpen)
                API.rpi_audio_play(_device);
        }

        public void Stop()
        {
            if (IsOpen)
                API.rpi_audio_stop(_device);
        }

        public void Pause()
        {
            if (IsOpen)
                API.rpi_audio_pause(_device);
        }

        public unsafe int Write(byte[] pcmBuffer, int offset, int sampleCount)
        {
            if (!IsOpen)
                throw new Exception("Must open device first.");
            if (!IsPaused || !IsPlaying)
                throw new Exception("Device must be in playing or paused state to write audio samples.");

            var written = 0;
            var sampleSize = FormatHelper.SampleSize(Format.BitDepth, Format.Channels);
            while (written < sampleCount)
            {
                var buffer = API.rpi_audio_get_buffer(_device);
                if (buffer == IntPtr.Zero) return written;

                var pointer = (byte*)buffer.ToPointer();
                var samplesToWrite = sampleCount;
                if (samplesToWrite > _samplesPerBuffer)
                    samplesToWrite = _samplesPerBuffer;

                for (var i = 0; i < samplesToWrite; i++)
                {
                    for (var j = 0; j < sampleSize; j++)
                    {
                        pointer[(i * sampleSize) + j] = pcmBuffer[(i * sampleSize) + j + offset];
                    }
                }
                API.rpi_audio_queue_buffer(_device, buffer, samplesToWrite * sampleSize);

                written += samplesToWrite;
            }
            return written;
        }
    }


}