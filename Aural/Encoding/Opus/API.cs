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
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FragLabs.Aural.Encoding.Opus
{
    /// <summary>
    /// Wraps the Opus API.
    /// </summary>
    internal class API
    {
        static API()
        {
            var image = IntPtr.Zero;
            if (PlatformDetails.IsMac)
            {
                image = LibraryLoader.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libs", "32bit", "libopus.dylib"));
            }
            else if (PlatformDetails.IsWindows)
            {
                if (PlatformDetails.CpuArchitecture == CpuArchitecture.x86)
                {
                    image = LibraryLoader.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libs", "32bit", "opus.dll"));
                }
                else if (PlatformDetails.CpuArchitecture == CpuArchitecture.x64)
                {
                    image = LibraryLoader.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libs", "64bit", "opus.dll"));
                }
            }
            else
            {
                image = LibraryLoader.Load("libopus.so.0");
				if (image == null)
				{
					image = LibraryLoader.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libs", "opus.so"));
				}
            }

            if (image != IntPtr.Zero)
            {
                var type = typeof(API);
                foreach (var member in type.GetFields(BindingFlags.Static | BindingFlags.NonPublic))
                {
                    var methodName = member.Name;
                    if (methodName == "opus_encoder_ctl_out") methodName = "opus_encoder_ctl";
                    var fieldType = member.FieldType;
                    var ptr = LibraryLoader.ResolveSymbol(image, methodName);
                    if (ptr == IntPtr.Zero)
                        throw new Exception(string.Format("Could not resolve symbol \"{0}\"", methodName));
                    member.SetValue(null, Marshal.GetDelegateForFunctionPointer(ptr, fieldType));
                }
            }
        }

        internal delegate IntPtr opus_encoder_create_delegate(int sampleRate, int channelCount, int application, out IntPtr error);
        internal static opus_encoder_create_delegate opus_encoder_create;

        internal delegate void opus_encoder_destroy_delegate(IntPtr encoder);
        internal static opus_encoder_destroy_delegate opus_encoder_destroy;

        internal delegate int opus_encode_delegate(IntPtr encoder, IntPtr pcm, int frameSize, IntPtr data, int maxDataBytes);
        internal static opus_encode_delegate opus_encode;

        internal delegate IntPtr opus_decoder_create_delegate(int sampleRate, int channelCount, out IntPtr error);
        internal static opus_decoder_create_delegate opus_decoder_create;

        internal delegate void opus_decoder_destroy_delegate(IntPtr decoder);
        internal static opus_decoder_destroy_delegate opus_decoder_destroy;

        internal delegate int opus_decode_delegate(IntPtr decoder, IntPtr data, int len, IntPtr pcm, int frameSize, int decodeFec);
        internal static opus_decode_delegate opus_decode;

        internal delegate int opus_encoder_ctl_delegate(IntPtr encoder, Ctl request, int value);
        internal static opus_encoder_ctl_delegate opus_encoder_ctl;

        internal delegate int opus_encoder_ctl_out_delegate(IntPtr encoder, Ctl request, out int value);
        internal static opus_encoder_ctl_out_delegate opus_encoder_ctl_out;
    }
}