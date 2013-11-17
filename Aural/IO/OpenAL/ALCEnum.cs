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
namespace FragLabs.Aural.IO.OpenAL
{
    internal enum ALCEnum
    {
        ALC_MAJOR_VERSION = 0x1000,
        ALC_MINOR_VERSION = 0x1001,
        ALC_ATTRIBUTES_SIZE = 0x1002,
        ALC_ALL_ATTRIBUTES = 0x1003,
        ALC_CAPTURE_SAMPLES = 0x312,
        ALC_FREQUENCY = 0x1007,
        ALC_REFRESH = 0x1008,
        ALC_SYNC = 0x1009,
        ALC_MONO_SOURCES = 0x1010,
        ALC_STEREO_SOURCES = 0x1011,
    }
}