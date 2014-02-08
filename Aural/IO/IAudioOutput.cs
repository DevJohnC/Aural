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
    /// Audio output.
    /// </summary>
    public interface IAudioOutput : IDisposable
    {
        /// <summary>
        /// Gets a value indicating if the audio output is currently playing.
        /// </summary>
        bool IsPlaying { get; }
        /// <summary>
        /// Gets a value indicating if the audio output is currently stopped.
        /// </summary>
        bool IsStopped { get; }
        /// <summary>
        /// Gets a value indicating if the audio output is currently paused.
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        /// Gets the number of frames queued for playback. May not be precise.
        /// </summary>
        int QueuedSampleCount { get; }

        /// <summary>
        /// Begins audio playback.
        /// </summary>
        void Play();
        /// <summary>
        /// Stops audio playback.
        /// </summary>
        void Stop();
        /// <summary>
        /// Pauses audio playback.
        /// </summary>
        void Pause();

        /// <summary>
        /// Queues PCM samples to be played.
        /// </summary>
        /// <param name="pcmBuffer"></param>
        /// <param name="offset"></param>
        /// <param name="sampleCount"></param>
        void Write(byte[] pcmBuffer, int offset, int sampleCount);
    }
}