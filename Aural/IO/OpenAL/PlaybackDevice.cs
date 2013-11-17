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
using System.Collections.Generic;

namespace FragLabs.Aural.IO.OpenAL
{
    /// <summary>
    /// Static OpenAL playback device methods.
    /// </summary>
    internal class PlaybackDevice
    {
        private static readonly Dictionary<string, IntPtr> OpenDevices = new Dictionary<string, IntPtr>();
        private static readonly Dictionary<string, IntPtr> OpenContexts = new Dictionary<string, IntPtr>();

        /// <summary>
        /// Gets a device, ensuring it's open if not already open.
        /// </summary>
        /// <returns></returns>
        internal static IntPtr GetDevice(string deviceName)
        {
            lock (typeof (PlaybackDevice))
            {
                if (!OpenDevices.ContainsKey(deviceName))
                    OpenDevices[deviceName] = API.alcOpenDevice(deviceName);
                return OpenDevices[deviceName];
            }
        }

        /// <summary>
        /// Gets a context for a device, creating one if needed.
        /// </summary>
        /// <returns></returns>
        internal static IntPtr GetContext(string deviceName)
        {
            var device = GetDevice(deviceName);
            lock (typeof(PlaybackDevice))
            {
                if (!OpenContexts.ContainsKey(deviceName))
                    OpenContexts[deviceName] = API.alcCreateContext(device, IntPtr.Zero);
                return OpenContexts[deviceName];
            }
        }
    }
}