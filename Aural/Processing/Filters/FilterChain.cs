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

using System.Collections.Generic;

namespace FragLabs.Aural.Processing.Filters
{
    /// <summary>
    /// A simple filter chain.
    /// </summary>
    public class FilterChain : IAudioFilter
    {
        private readonly List<IAudioFilter> _filters = new List<IAudioFilter>();

        public FilterChain()
        {
            SupportsByte = false;
            SupportsDouble = true;
        }

        /// <summary>
        /// Adds an audio filter to the chain.
        /// </summary>
        /// <param name="filter"></param>
        public void Add(IAudioFilter filter)
        {
            lock(_filters)
                _filters.Add(filter);
            if (_filters.Count == 1)
                InputAudioFormat = filter.InputAudioFormat;
            OutputAudioFormat = filter.OutputAudioFormat;
        }

        /// <summary>
        /// Gets the expected input audio format.
        /// </summary>
        public AudioFormat InputAudioFormat { get; private set; }

        /// <summary>
        /// Gets the output audio format.
        /// </summary>
        public AudioFormat OutputAudioFormat { get; private set; }

        /// <summary>
        /// Gets a value indicating if the filter supports the byte typed API.
        /// </summary>
        public bool SupportsByte { get; private set; }

        /// <summary>
        /// Gets a value indicating if the filter supports the double typed API.
        /// </summary>
        public bool SupportsDouble { get; private set; }

        public int Process(byte[] inputPcmSamples, int inputOffset, byte[] outputPcmSamples, int outputOffset, int inputSampleCount)
        {
            throw new System.NotImplementedException();
        }

        public int Process(double[] inputSamples, int inputOffset, double[] outputSamples, int outputOffset, int inputSampleCount)
        {
            lock (_filters)
            {
                foreach (var filter in _filters)
                {
                    inputSampleCount = filter.Process(inputSamples, inputOffset, outputSamples, outputOffset, inputSampleCount);
                }
            }
            return inputSampleCount;
        }
    }
}