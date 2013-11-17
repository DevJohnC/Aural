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
namespace FragLabs.Aural
{
    /// <summary>
    /// Helpful audio format methods.
    /// </summary>
    public class FormatHelper
    {
        /// <summary>
        /// Calculates the size of a single sample for all channels in bytes.
        /// </summary>
        /// <param name="bitDepth">Sample bitdepth.</param>
        /// <param name="channelCount">The number of channels in a sample.</param>
        /// <returns>The number of bytes required to store a single sample.</returns>
        public static int SampleSize(int bitDepth, int channelCount)
        {
            return (bitDepth/8)*channelCount;
        }
    }
}