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

using System.Collections.Generic;
using System.Threading;

namespace FragLabs.Aural.IO.OpenAL
{
    /// <summary>
    /// OpenAL runner.
    /// </summary>
    internal class Runner
    {
        /// <summary>
        /// List of OpenAL inputs to monitor.
        /// </summary>
        private static readonly List<OpenALInput> _inputs = new List<OpenALInput>();

        /// <summary>
        /// Runner thread.
        /// </summary>
        private static Thread _runnerThread;

        /// <summary>
        /// Runner is running?
        /// </summary>
        private static bool _isRunning;

        /// <summary>
        /// Set to true to signal the runner thread to stop.
        /// </summary>
        private static bool _stopSignal;

        /// <summary>
        /// Adds an OpenAL input to the runner for reading.
        /// </summary>
        /// <param name="input"></param>
        public static void Add(OpenALInput input)
        {
            lock (_inputs)
            {
                _inputs.Add(input);
            }
            EnsureThreadRunning();
        }

        /// <summary>
        /// Removes an OpenAL input from the runner.
        /// </summary>
        /// <param name="input"></param>
        public static void Remove(OpenALInput input, bool allowThreadShutdown = true)
        {
            lock (_inputs)
            {
                _inputs.Remove(input);
                if (allowThreadShutdown && _inputs.Count == 0)
                {
                    StopThread();
                }
            }
        }

        /// <summary>
        /// Ensures the runner thread is running.
        /// </summary>
        private static void EnsureThreadRunning()
        {
            lock (typeof (Runner))
            {
                if (!_isRunning)
                {
                    _runnerThread = new Thread(RunnerThread);
                    _runnerThread.Start();
                }
            }
        }

        /// <summary>
        /// Stops the runner thread if it's already running.
        /// </summary>
        private static void StopThread()
        {
            lock (typeof (Runner))
            {
                if (_isRunning)
                {
                    _stopSignal = true;
                }
            }
        }

        /// <summary>
        /// Runner thread.
        /// </summary>
        private static void RunnerThread()
        {
            _isRunning = true;
            while (!_stopSignal)
            {
                lock (_inputs)
                {
                    foreach (var input in _inputs)
                    {
                        while(input.GetSamplesAvailable() >= input.ReadSampleCount)
                        {
                            var readBytes = input.Read(input.ReadBuffer, 0, input.ReadSampleCount);
                            input.ReadComplete(readBytes);
                        }
                    }
                }
                Thread.Sleep(1);
            }
            _runnerThread = null;
            _isRunning = false;
        }
    }
}