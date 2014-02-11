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
using System.IO;

namespace FragLabs.Aural.Encoding
{
    public class WaveDecoder : IFrameWindowDecoder
    {
        private readonly int _bitDepth;
        private readonly int _channelCount;
        private readonly int _sampleSize;

        public WaveDecoder(int bitDepth, int channelCount)
        {
            _bitDepth = bitDepth;
            _channelCount = channelCount;
            _sampleSize = FormatHelper.SampleSize(bitDepth, channelCount);
        }

        public int Decode(byte[] srcEncodedBuffer, int srcOffset, int srcLength, byte[] dstBuffer, int dstOffset)
        {
            //  read as many complete samples as will fit in the buffer
            var maxLen = dstBuffer.Length - dstOffset;
            if (maxLen > srcLength)
                maxLen = srcLength;
            var sampleCount = maxLen/_sampleSize;
            var readCount = sampleCount*_sampleSize;
            Buffer.BlockCopy(srcEncodedBuffer, srcOffset, dstBuffer, dstOffset, readCount);
            return readCount;
        }

        public void Dispose()
        {
        }

        /// <summary>
        /// Creates a WaveDecoder instance by reading the RIFF header on a stream.
        /// </summary>
        /// <param name="wavStream"></param>
        /// <param name="audioFormat"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static WaveDecoder FromStream(Stream wavStream, out AudioFormat audioFormat,
            out TimeSpan duration)
        {
            var riffHeader = new byte[12];
            wavStream.Read(riffHeader, 0, riffHeader.Length);

            var chunkSize = BitConverter.ToInt32(riffHeader, 4);

            var subchunkHeader = new byte[8];
            wavStream.Read(subchunkHeader, 0, subchunkHeader.Length);

            var subchunkSize = BitConverter.ToInt32(subchunkHeader, 4);
            var subchunk = new byte[subchunkSize];
            wavStream.Read(subchunk, 0, subchunk.Length);

            var channels = BitConverter.ToInt16(subchunk, 2);
            var sampleRate = BitConverter.ToInt32(subchunk, 4);
            var byteRate = BitConverter.ToInt32(subchunk, 8);
            var bytesPerFullSample = BitConverter.ToInt16(subchunk, 12);
            var bitDepth = BitConverter.ToInt16(subchunk, 14);

            wavStream.Read(subchunkHeader, 0, subchunkHeader.Length);
            var length = BitConverter.ToInt32(subchunkHeader, 4);
            var sampleCount = length/bytesPerFullSample;
            var fileDuration = sampleCount/sampleRate;

            duration = TimeSpan.FromSeconds(fileDuration);

            audioFormat = new AudioFormat
                {
                    BitDepth = bitDepth,
                    Channels = channels,
                    SampleRate = sampleRate
                };

            return new WaveDecoder(bitDepth, channels);
        }
    }
}