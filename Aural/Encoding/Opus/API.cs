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
            if (PlatformDetails.CpuArchitecture == CpuArchitecture.x86)
            {
                LibraryLoader.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libs", "32bit", "opus.dll"));
            }
            else if (PlatformDetails.CpuArchitecture == CpuArchitecture.x64)
            {
                LibraryLoader.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libs", "64bit", "opus.dll"));
            }
        }

        internal static IntPtr opus_encoder_create(int sampleRate, int channelCount, int application, out IntPtr error)
        {
            return APIBinding.opus_encoder_create(sampleRate, channelCount, application, out error);
        }

        internal static void opus_encoder_destroy(IntPtr encoder)
        {
            APIBinding.opus_encoder_destroy(encoder);
        }

        internal static int opus_encode(IntPtr encoder, byte[] pcm, int frameSize, IntPtr data, int maxDataBytes)
        {
            return APIBinding.opus_encode(encoder, pcm, frameSize, data, maxDataBytes);
        }

        internal static IntPtr opus_decoder_create(int sampleRate, int channelCount, out IntPtr error)
        {
            return APIBinding.opus_decoder_create(sampleRate, channelCount, out error);
        }

        internal static void opus_decoder_destroy(IntPtr decoder)
        {
            APIBinding.opus_decoder_destroy(decoder);
        }

        internal static int opus_decode(IntPtr decoder, byte[] data, int len, IntPtr pcm, int frameSize, int decodeFec)
        {
            return APIBinding.opus_decode(decoder, data, len, pcm, frameSize, decodeFec);
        }

        internal static int opus_encoder_ctl(IntPtr encoder, Ctl request, int value)
        {
            return APIBinding.opus_encoder_ctl(encoder, request, value);
        }

        internal static int opus_encoder_ctl(IntPtr encoder, Ctl request, out int value)
        {
            return APIBinding.opus_encoder_ctl(encoder, request, out value);
        }
    }

    /// <summary>
    /// Wraps the Opus API.
    /// </summary>
    internal class APIBinding
    {
        [DllImport("opus.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr opus_encoder_create(int sampleRate, int channelCount, int application, out IntPtr error);

        [DllImport("opus.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void opus_encoder_destroy(IntPtr encoder);

        [DllImport("opus.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int opus_encode(IntPtr encoder, byte[] pcm, int frameSize, IntPtr data, int maxDataBytes);

        [DllImport("opus.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr opus_decoder_create(int sampleRate, int channelCount, out IntPtr error);

        [DllImport("opus.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void opus_decoder_destroy(IntPtr decoder);

        [DllImport("opus.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int opus_decode(IntPtr decoder, byte[] data, int len, IntPtr pcm, int frameSize, int decodeFec);

        [DllImport("opus.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int opus_encoder_ctl(IntPtr encoder, Ctl request, int value);

        [DllImport("opus.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int opus_encoder_ctl(IntPtr encoder, Ctl request, out int value);
    }
}