// 
// Author: John Carruthers (johnc@frag-labs.com)
// 
// Copyright (C) 2014 John Carruthers
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
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FragLabs.Aural.IO.RaspberryPi
{
    /// <summary>
    /// RaspberryPi audio API.
    /// </summary>
    internal class API
    {
        /// <summary>
        /// Gets a value indicating if RPi audio is supported on the current platform.
        /// </summary>
        public static bool IsSupported { get; private set; }

        static API()
        {
            IsSupported = false;
            if (PlatformDetails.IsMac || PlatformDetails.IsWindows)
                return;

            try
            {
                var image = LibraryLoader.Load("librpiaudio.so.1");
                if (image.Equals(IntPtr.Zero))
                {
                    image =
                        LibraryLoader.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libs", "librpiaudio.so"));
                }

                if (image != IntPtr.Zero)
                {
                    var type = typeof (API);
                    foreach (var member in type.GetFields(BindingFlags.Static | BindingFlags.NonPublic))
                    {
                        var methodName = member.Name;
                        var fieldType = member.FieldType;
                        if (fieldType == typeof (bool)) continue;
                        var ptr = LibraryLoader.ResolveSymbol(image, methodName);
                        if (ptr == IntPtr.Zero)
                            throw new Exception(string.Format("Could not resolve symbol \"{0}\"", methodName));
                        member.SetValue(null, Marshal.GetDelegateForFunctionPointer(ptr, fieldType));
                    }
                    IsSupported = true;
                }
            }
            catch (Exception ex)
            {
                //  ignore - not a supported platform.
                Console.WriteLine(ex);
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr createDeviceDelegate(uint samplerate, uint channelCount, uint bitDepth, uint buffercount, uint bufferSize);
        internal static createDeviceDelegate rpi_audio_create;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void destroyDeviceDelegate(IntPtr device);
        internal static destroyDeviceDelegate rpi_audio_destroy;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void setDestDelegate(IntPtr device, uint dest);
        internal static setDestDelegate rpi_audio_set_dest;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr getBufferDelegate(IntPtr device);
        internal static getBufferDelegate rpi_audio_get_buffer;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr playDelegate(IntPtr device);
        internal static playDelegate rpi_audio_play;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr pauseDelegate(IntPtr device);
        internal static pauseDelegate rpi_audio_pause;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr stopDelegate(IntPtr device);
        internal static stopDelegate rpi_audio_stop;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void queueBufferDelegate(IntPtr device, IntPtr buffer, int byteLength);
        internal static queueBufferDelegate rpi_audio_queue_buffer;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate uint getStateDelegate(IntPtr device);
        internal static getStateDelegate rpi_audio_get_state;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate uint getUnplayedSamplesDelegate(IntPtr device);
        internal static getUnplayedSamplesDelegate rpi_audio_unplayed_samples;
    }

    internal enum OutputDest
    {
        /// <summary>
        /// HDMI output.
        /// </summary>
        HDMI = 0,
        /// <summary>
        /// 3.5mm audio jack.
        /// </summary>
        AudioJack = 1
    }

    internal enum OmxState
    {
        Invalid = 0,
        Loaded = 1,
        Idle = 2,
        Executing = 3,
        Pause = 4,
        WaitForResources = 5
    }
}