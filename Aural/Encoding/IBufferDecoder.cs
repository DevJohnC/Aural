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

namespace FragLabs.Aural.Encoding
{
    /// <summary>
    /// Decoder that writes decoded data to a byte array.
    /// </summary>
    public interface IBufferDecoder : IDisposable
    {
        /// <summary>
        /// Decodes audio samples.
        /// </summary>
        /// <param name="srcEncodedBuffer">Encoded data.</param>
        /// <param name="srcOffset">The zero-based byte offset in srcEncodedBuffer at which to begin reading encoded data.</param>
        /// <param name="srcLength">The number of bytes to read from srcEncodedBuffer.</param>
        /// <param name="dstBuffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values starting at offset replaced with audio samples.</param>
        /// <param name="dstOffset">The zero-based byte offset in dstBuffer at which to begin writing decoded audio samples.</param>
        /// <returns>The number of bytes decoded and written to dstBuffer.</returns>
        int Decode(byte[] srcEncodedBuffer, int srcOffset, int srcLength,
                   byte[] dstBuffer, int dstOffset);
    }
}