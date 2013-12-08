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
namespace FragLabs.Aural.Processing.Filters
{
    /// <summary>
    /// Audio filter.
    /// </summary>
    public interface IAudioFilter
    {
        /// <summary>
        /// Gets the expected input audio format.
        /// </summary>
        AudioFormat InputAudioFormat { get; }

        /// <summary>
        /// Gets the output audio format.
        /// </summary>
        AudioFormat OutputAudioFormat { get; }

        /// <summary>
        /// Gets a value indicating if the filter supports the byte typed API.
        /// </summary>
        bool SupportsByte { get; }

        /// <summary>
        /// Gets a value indicating if the filter supports the double typed API.
        /// </summary>
        bool SupportsDouble { get; }

        int Process(byte[] inputPcmSamples, int inputOffset, byte[] outputPcmSamples, int outputOffset, int inputSampleCount);

        int Process(double[] inputSamples, int inputOffset, double[] outputSamples, int outputOffset,
                    int inputSampleCount);
    }
}