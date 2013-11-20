# Aural

Aural is a multipurpose audio library built mainly for my own personal use but others might find helpful.
It's goal is to provide bindings to Xiph.Org audio codecs, a binding for OpenAL and some very basic audio
processing tools.

## About

Currently included in Aural is 32-bit and 64-bit support for Opus Codec and OpenAL on Windows.

To use the library reference Aural.dll and add opus.dll and openal.dll to your project (maintaining the directory structure) and copy the libraries to the output directory when compiling.

## License

Aural is licensed under the MIT license.

## Acknowledgements

Parts of the included code are provided by 3rd parties:

* OpenAL binding is based on and partially copied from Eric Maupin's [Gablarski](https://github.com/ermau/gablarski) project.
* [Opus codec](http://www.opus-codec.org/) library is developed by the Xiph.Org Foundation.
* [OpenAL Soft](http://kcat.strangesoft.net/openal.html) is developed by Chris Robinson (kcat(a)strangesoft.net).