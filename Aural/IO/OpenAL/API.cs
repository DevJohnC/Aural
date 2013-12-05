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
using System.Reflection;
using System.Runtime.InteropServices;

namespace FragLabs.Aural.IO.OpenAL
{
    internal class API
    {
        static API()
        {
            var image = IntPtr.Zero;
            if (PlatformDetails.IsMac)
            {
				image = LibraryLoader.Load("/System/Library/Frameworks/OpenAL.framework/OpenAL");
            }
			else if (PlatformDetails.IsWindows)
			{
	            if (PlatformDetails.CpuArchitecture == CpuArchitecture.x86)
	            {
	                image = LibraryLoader.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libs", "32bit", "openal.dll"));
	            }
	            else if (PlatformDetails.CpuArchitecture == CpuArchitecture.x64)
	            {
	                image = LibraryLoader.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libs", "64bit", "openal.dll"));
	            }
			}
            if (image != IntPtr.Zero)
            {
                //alGetString = (alGetStringDelegate)Marshal.GetDelegateForFunctionPointer(LibraryLoader.ResolveSymbol(image, "alGetString"), typeof(alGetStringDelegate));
                var type = typeof (API);
                foreach (var member in type.GetFields(BindingFlags.Static|BindingFlags.NonPublic))
                {
                    var methodName = member.Name;
                    var fieldType = member.FieldType;
                    var ptr = LibraryLoader.ResolveSymbol(image, methodName);
                    if (ptr == IntPtr.Zero)
                        throw new Exception(string.Format("Could not resolve symbol \"{0}\"", methodName));
                    member.SetValue(null, Marshal.GetDelegateForFunctionPointer(ptr, fieldType));
                }
            }
        }

        internal delegate IntPtr alGetStringDelegate(int name);
        internal static alGetStringDelegate alGetString;

        internal delegate IntPtr alcGetStringDelegate([In] IntPtr device, int name);
        internal static alcGetStringDelegate alcGetString;

        internal delegate sbyte alcIsExtensionPresentDelegate([In] IntPtr device, string extensionName);
        internal static alcIsExtensionPresentDelegate alcIsExtensionPresent;

        internal delegate sbyte alIsExtensionPresentDelegate(string extensionName);
        internal static alIsExtensionPresentDelegate alIsExtensionPresent;

        internal delegate  void alcCaptureStartDelegate(IntPtr device);
        internal static alcCaptureStartDelegate alcCaptureStart;

        internal delegate  void alcCaptureStopDelegate(IntPtr device);
        internal static alcCaptureStopDelegate alcCaptureStop;

        internal delegate  void alcCaptureSamplesDelegate(IntPtr device, IntPtr buffer, int numSamples);
        internal static alcCaptureSamplesDelegate alcCaptureSamples;

        internal delegate  IntPtr alcCaptureOpenDeviceDelegate(string deviceName, uint frequency, AudioFormat format, int bufferSize);
        internal static alcCaptureOpenDeviceDelegate alcCaptureOpenDevice;

        internal delegate  void alcCaptureCloseDeviceDelegate(IntPtr device);
        internal static alcCaptureCloseDeviceDelegate alcCaptureCloseDevice;

        internal delegate  void alcGetIntegervDelegate(IntPtr device, ALCEnum param, int size, out int data);
        internal static alcGetIntegervDelegate alcGetIntegerv;

        internal delegate  IntPtr alcOpenDeviceDelegate(string deviceName);
        internal static alcOpenDeviceDelegate alcOpenDevice;

        internal delegate  IntPtr alcCloseDeviceDelegate(IntPtr handle);
        internal static alcCloseDeviceDelegate alcCloseDevice;

        internal delegate IntPtr alcCreateContextDelegate(IntPtr device, IntPtr attrlist);
        internal static alcCreateContextDelegate alcCreateContext;

        internal delegate void alcMakeContextCurrentDelegate(IntPtr context);
        internal static alcMakeContextCurrentDelegate alcMakeContextCurrent;

        internal delegate IntPtr alcGetContextsDeviceDelegate(IntPtr context);
        internal static alcGetContextsDeviceDelegate alcGetContextsDevice;

        internal delegate IntPtr alcGetCurrentContextDelegate();
        internal static alcGetCurrentContextDelegate alcGetCurrentContext;

        internal delegate void alcDestroyContextDelegate(IntPtr context);
        internal static alcDestroyContextDelegate alcDestroyContext;

        internal delegate  void alGetSourceiDelegate(uint sourceID, IntSourceProperty property, out int value);
        internal static alGetSourceiDelegate alGetSourcei;

        internal delegate  void alSourcePlayDelegate(uint sourceID);
        internal static alSourcePlayDelegate alSourcePlay;

        internal delegate  void alSourcePauseDelegate(uint sourceID);
        internal static alSourcePauseDelegate alSourcePause;

        internal delegate  void alSourceStopDelegate(uint sourceID);
        internal static alSourceStopDelegate alSourceStop;

        internal delegate  void alSourceQueueBuffersDelegate(uint sourceID, int number, uint[] bufferIDs);
        internal static alSourceQueueBuffersDelegate alSourceQueueBuffers;

        internal delegate  void alSourceUnqueueBuffersDelegate(uint sourceID, int buffers, uint[] buffersDequeued);
        internal static alSourceUnqueueBuffersDelegate alSourceUnqueueBuffers;

        internal delegate  void alGenSourcesDelegate(int count, uint[] sources);
        internal static alGenSourcesDelegate alGenSources;

        internal delegate  void alDeleteSourcesDelegate(int count, uint[] sources);
        internal static alDeleteSourcesDelegate alDeleteSources;

        internal delegate  void alGetSourcefDelegate(uint sourceID, FloatSourceProperty property, out float value);
        internal static alGetSourcefDelegate alGetSourcef;

        internal delegate  void alGetSource3fDelegate(uint sourceID, FloatSourceProperty property, out float val1, out float val2, out float val3);
        internal static alGetSource3fDelegate alGetSource3f;

        internal delegate  void alSourcefDelegate(uint sourceID, FloatSourceProperty property, float value);
        internal static alSourcefDelegate alSourcef;

        internal delegate  void alSource3fDelegate(uint sourceID, FloatSourceProperty property, float val1, float val2, float val3);
        internal static alSource3fDelegate alSource3f;

        internal delegate  void alGetBufferiDelegate(uint bufferID, ALEnum property, out int value);
        internal static alGetBufferiDelegate alGetBufferi;

        internal delegate  void alGenBuffersDelegate(int count, uint[] bufferIDs);
        internal static alGenBuffersDelegate alGenBuffers;

        internal delegate  void alBufferDataDelegate(uint bufferID, AudioFormat format, IntPtr data, int byteSize, uint frequency);
        internal static alBufferDataDelegate alBufferData;

        internal delegate  void alDeleteBuffersDelegate(int numBuffers, uint[] bufferIDs);
        internal static alDeleteBuffersDelegate alDeleteBuffers;

        internal delegate  void alListenerfDelegate(FloatSourceProperty param, float val);
        internal static alListenerfDelegate alListenerf;

        internal delegate  void alListenerfvDelegate(FloatSourceProperty param, float[] val);
        internal static alListenerfvDelegate alListenerfv;

        internal delegate  void alListener3fDelegate(FloatSourceProperty param, float val1, float val2, float val3);
        internal static alListener3fDelegate alListener3f;

        internal delegate  void alGetListener3fDelegate(FloatSourceProperty param, out float val1, out float val2, out float val3);
        internal static alGetListener3fDelegate alGetListener3f;

        internal delegate  void alGetListenerfDelegate(FloatSourceProperty param, out float val);
        internal static alGetListenerfDelegate alGetListenerf;

        internal delegate  void alGetListenerfvDelegate(FloatSourceProperty param, float[] val);
        internal static alGetListenerfvDelegate alGetListenerfv;
    }
}