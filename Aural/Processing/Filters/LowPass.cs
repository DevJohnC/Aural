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
    /// Simple low pass filter.
    /// </summary>
    public class LowPass : IAudioFilter
    {
        private int _cutoffFrequency;

        public LowPass(AudioFormat audioFormat, int cutoffFrequency)
        {
            InputAudioFormat = audioFormat;
            OutputAudioFormat = audioFormat;
            CutoffFrequency = cutoffFrequency;
        }

        private double _rc;
        private double _dt;
        private double _alpha;

        public int CutoffFrequency
        {
            get { return _cutoffFrequency; }
            set
            {
                _cutoffFrequency = value;
                _rc = 1.0f/(_cutoffFrequency*2*3.142);
                _dt = 1.0f/InputAudioFormat.SampleRate;
                _alpha = _dt/(_rc + _dt);
            }
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
        public bool SupportsByte { get { return false; } }

        /// <summary>
        /// Gets a value indicating if the filter supports the double typed API.
        /// </summary>
        public bool SupportsDouble { get { return true; } }

        public int Process(byte[] inputPcmSamples, int inputOffset, byte[] outputPcmSamples, int outputOffset, int inputSampleCount)
        {
            throw new NotSupportedException();
        }

        public int Process(double[] inputSamples, int inputOffset, double[] outputSamples, int outputOffset, int inputSampleCount)
        {
            outputSamples[0] = inputSamples[0];
            for (var i = 1; i < inputSampleCount; i++)
            {
                outputSamples[i] = outputSamples[i - 1] + (_alpha * (inputSamples[i] - outputSamples[i - 1]));
            }
            return inputSampleCount;
        }
    }
}