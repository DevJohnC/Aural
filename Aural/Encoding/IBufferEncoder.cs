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

namespace FragLabs.Aural.Encoding
{
    /// <summary>
    /// Encoder that writes encoded audio to a byte array.
    /// </summary>
    public interface IBufferEncoder : IDisposable
    {
        /// <summary>
        /// Encode samples.
        /// </summary>
        /// <param name="srcPcmSamples">PCM samples to be encoded.</param>
        /// <param name="srcOffset">The zero-based byte offset in srcPcmSamples at which to begin reading PCM samples.</param>
        /// <param name="dstOutputBuffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values starting at offset replaced with encoded audio data.</param>
        /// <param name="dstOffset">The zero-based byte offset in dstOutputBuffer at which to begin writing encoded audio.</param>
        /// <param name="sampleCount">The number of samples, per channel, to encode.</param>
        /// <returns>The total number of bytes written to dstOutputBuffer.</returns>
        int Encode(byte[] srcPcmSamples, int srcOffset, byte[] dstOutputBuffer, int dstOffset, int sampleCount);
    }
}