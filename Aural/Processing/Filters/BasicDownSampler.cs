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

namespace FragLabs.Aural.Processing.Filters
{
    /// <summary>
    /// Basic downsampler.
    /// </summary>
    public class BasicDownSampler : IAudioFilter
    {
        private readonly int _downsampleFactor;

        public BasicDownSampler(AudioFormat inputFormat, int downsampleFactor)
        {
            _downsampleFactor = downsampleFactor;
            InputAudioFormat = inputFormat;
            OutputAudioFormat = new AudioFormat
                {
                    BitDepth = InputAudioFormat.BitDepth,
                    Channels = InputAudioFormat.Channels,
                    SampleRate = InputAudioFormat.SampleRate / downsampleFactor
                };
        }

        /// <summary>
        /// Gets or sets the expected input audio format.
        /// </summary>
        public AudioFormat InputAudioFormat { get; private set; }

        /// <summary>
        /// Gets or sets the output audio format.
        /// </summary>
        public AudioFormat OutputAudioFormat { get; private set; }

        /// <summary>
        /// Gets a value indicating if the filter supports the byte typed API.
        /// </summary>
        public bool SupportsByte { get { return true; } }

        /// <summary>
        /// Gets a value indicating if the filter supports the double typed API.
        /// </summary>
        public bool SupportsDouble { get { return true; } }

        public int Process(byte[] inputPcmSamples, int inputOffset, byte[] outputPcmSamples, int outputOffset, int inputSampleCount)
        {
            var sampleSize = FormatHelper.SampleSize(InputAudioFormat.BitDepth, InputAudioFormat.Channels);
            var outputSampleCount = inputSampleCount/_downsampleFactor;
            for (var i = 0; i < outputSampleCount; i++)
            {
                var index = inputOffset + (sampleSize * i * _downsampleFactor);
                double downSampleSum = 0;
                for(var j = 0; j < _downsampleFactor; j++)
                {
                    var sample = BitConverter.ToInt16(inputPcmSamples, index + (j*sampleSize));
                    downSampleSum += sample;
                }
                var downSampled = downSampleSum/_downsampleFactor;
                var pcmSample = BitConverter.GetBytes((short)downSampled);
                Buffer.BlockCopy(pcmSample, 0, outputPcmSamples, outputOffset + (i * sampleSize), sampleSize);
            }
            return outputSampleCount * sampleSize;
        }

        public int Process(double[] inputSamples, int inputOffset, double[] outputSamples, int outputOffset, int inputSampleCount)
        {
            var outputSampleCount = inputSampleCount/_downsampleFactor;
            for (var i = 0; i < outputSampleCount; i++)
            {
                var index = inputOffset + (i*_downsampleFactor);
                double downSampleSum = 0;
                for (var j = 0; j < _downsampleFactor; j++)
                    downSampleSum += inputSamples[index + j];
                outputSamples[outputOffset + i] = downSampleSum / _downsampleFactor;
            }
            return outputSampleCount;
        }
    }
}