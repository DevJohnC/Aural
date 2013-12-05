﻿// 
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
using System.Runtime.InteropServices;

namespace FragLabs.Aural
{
    /// <summary>
    /// Library loader.
    /// </summary>
    internal class LibraryLoader
    {
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport ("dl", CharSet=CharSet.Ansi)]
        private static extern IntPtr dlopen(string filename, int flags);

        [DllImport("dl", CharSet = CharSet.Ansi)]
        static extern IntPtr dlsym(IntPtr handle, string symbol);

        /// <summary>
        /// Load a library.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        internal static IntPtr Load(string fileName)
        {
            return PlatformDetails.IsWindows ? LoadLibrary(fileName) : dlopen(fileName, 0);
        }

        /// <summary>
        /// Resolves library function pointer.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        internal static IntPtr ResolveSymbol(IntPtr image, string symbol)
        {
            return PlatformDetails.IsWindows ? GetProcAddress(image, symbol) : dlsym(image, symbol);
        }
    }
}
