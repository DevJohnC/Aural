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
using System.Runtime.InteropServices;

namespace FragLabs.Aural.IO.OpenAL
{
    internal class API
    {
        static API()
        {
            if (PlatformDetails.CpuArchitecture == CpuArchitecture.x86)
            {
                LibraryLoader.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libs", "32bit", "openal.dll"));
            }
            else if (PlatformDetails.CpuArchitecture == CpuArchitecture.x64)
            {
                LibraryLoader.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libs", "64bit", "openal.dll"));
            }
        }

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr alGetString(int name);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr alcGetString([In] IntPtr device, int name);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern sbyte alcIsExtensionPresent([In] IntPtr device, string extensionName);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern sbyte alIsExtensionPresent(string extensionName);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alcCaptureStart(IntPtr device);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alcCaptureStop(IntPtr device);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alcCaptureSamples(IntPtr device, IntPtr buffer, int numSamples);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr alcCaptureOpenDevice(string deviceName, uint frequency, AudioFormat format, int bufferSize);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alcCaptureCloseDevice(IntPtr device);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alcGetIntegerv(IntPtr device, ALCEnum param, int size, out int data);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr alcOpenDevice(string deviceName);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr alcCloseDevice(IntPtr handle);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal extern static IntPtr alcCreateContext(IntPtr device, IntPtr attrlist);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal extern static void alcMakeContextCurrent(IntPtr context);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal extern static IntPtr alcGetContextsDevice(IntPtr context);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal extern static IntPtr alcGetCurrentContext();

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal extern static void alcDestroyContext(IntPtr context);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alGetSourcei(uint sourceID, IntSourceProperty property, out int value);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alSourcePlay(uint sourceID);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alSourcePause(uint sourceID);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alSourceStop(uint sourceID);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alSourceQueueBuffers(uint sourceID, int number, uint[] bufferIDs);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alSourceUnqueueBuffers(uint sourceID, int buffers, uint[] buffersDequeued);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alGenSources(int count, uint[] sources);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alDeleteSources(int count, uint[] sources);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alGetSourcef(uint sourceID, FloatSourceProperty property, out float value);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alGetSource3f(uint sourceID, FloatSourceProperty property, out float val1, out float val2, out float val3);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alSourcef(uint sourceID, FloatSourceProperty property, float value);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alSource3f(uint sourceID, FloatSourceProperty property, float val1, float val2, float val3);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alGetBufferi(uint bufferID, ALEnum property, out int value);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alGenBuffers(int count, uint[] bufferIDs);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alBufferData(uint bufferID, AudioFormat format, byte[] data, int byteSize, uint frequency);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alDeleteBuffers(int numBuffers, uint[] bufferIDs);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alListenerf(FloatSourceProperty param, float val);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alListenerfv(FloatSourceProperty param, float[] val);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alListener3f(FloatSourceProperty param, float val1, float val2, float val3);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alGetListener3f(FloatSourceProperty param, out float val1, out float val2, out float val3);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alGetListenerf(FloatSourceProperty param, out float val);

        [DllImport("openal.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alGetListenerfv(FloatSourceProperty param, float[] val);
    }
}