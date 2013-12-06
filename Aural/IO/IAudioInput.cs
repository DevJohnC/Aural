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

namespace FragLabs.Aural.IO
{
    /// <summary>
    /// Audio input.
    /// </summary>
    public interface IAudioInput : IDisposable
    {
        /// <summary>
        /// Event raised when audio data is received.
        /// </summary>
        event EventHandler<AudioReceivedEventArgs> AudioReceived;

        /// <summary>
        /// Gets the name of the audio input.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets if the audio input is currently being read.
        /// </summary>
        bool IsReading { get; }

        /// <summary>
        /// Starts reading audio samples from the audio input. Will raise AudioReceived when audio samples are read from the input.
        /// </summary>
        /// <param name="sampleCount">The number of samples, per channel, to read before raising the AudioReceived event.</param>
        void StartReading(int sampleCount);

        /// <summary>
        /// Stops reading audio samples from the audio input.
        /// </summary>
        void StopReading();

        /// <summary>
        /// Reads audio samples from the input device into a buffer.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values starting at offset replaced with audio samples.</param>
        /// <param name="offset">The zero-based byte offset in dstOutputBuffer at which to begin writing audio samples.</param>
        /// <param name="sampleCount">The number of samples, per channel, to read.</param>
        /// <returns>The total number of bytes written to buffer.</returns>
        int Read(byte[] buffer, int offset, int sampleCount);
    }
}