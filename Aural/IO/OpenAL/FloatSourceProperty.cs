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
    internal enum FloatSourceProperty
    {
        AL_PITCH = 0x1003,
        AL_POSITION = 0x1004,
        AL_DIRECTION = 0x1005,
        AL_VELOCITY = 0x1006,
        AL_GAIN = 0x100A,
        AL_MIN_GAIN = 0x100D,
        AL_MAX_GAIN = 0x100E,
        AL_ORIENTATION = 0x100F,
        AL_MAX_DISTANCE = 0x1023,
        AL_ROLLOFF_FACTOR = 0x1021,
        AL_CONE_OUTER_GAIN = 0x1022,
        AL_CONE_INNER_ANGLE = 0x1001,
        AL_CONE_OUTER_ANGLE = 0x1002,
        AL_REFERENCE_DISTANCE = 0x1020
    }
}