# Introduction

<div class="banner intro">
    <div class="default-text"></div>
    <div class="vendor-logo gs-logo"></div>
    <div class="vendor-logo c-sharp-logo"></div>
    <div class="vendor-logo java-logo"></div>
    <div class="vendor-logo python-logo"></div>
</div>

## About our APIs

The core of Ghostscript is written in `C`, but also supports [language bindings] for the following programming languages:

* `C#`
* `Java`
* `Python`

All of the above languages have equivalent methods as defined in the [C API]. `Java` and `C#` provide additional helper methods to make the use of the API easier for certain applications. These languages also provide example viewers that make use of these methods.

This developer documentation is organized by programming language type and includes API reference and sample code.

## The C API

<div class="vendor-logo c-logo"></div>
<p/>

Ghostscript has been in development for over thirty years and is written in `C`. The API has evolved over time and is continually being developed. The language bindings into Ghostscript will attempt to mirror this evolution and match the current [C API] as much as possible.

## Licensing

Before using Ghostscript, please make sure that you have a valid license to do so. There are two available licenses; make sure you pick the one whose terms you can comply with.

### Open Source license

If your software is open source, you may use Ghostscript under the terms of the GNU Affero General Public License.

This means that all of the source code for your complete app must be released under a compatible open source license!

It also means that you may not use any proprietary closed source libraries or components in your app.

Please read the full text of the AGPL license agreement from the [FSF web site]

If you cannot or do not want to comply with these restrictions, you must acquire a commercial license instead.

<button class="cta orange"><a href="https://artifex.com/licensing/" target="new">FIND OUT MORE</a></button>

### Commercial license

If your project does not meet the requirements of the AGPL, please contact our sales team to discuss a commercial license. Each Artifex commercial license is crafted based on your individual use case.

<button class="cta orange"><a href="https://artifex.com/contact/" target="new">CONTACT US</a></button>


## Building Ghostscript

In order to use Ghostscript language bindings firstly Ghostscript must be built as a shared library for your platform.

The following built libraries are required for these respective platforms:

| Platform | Ghostscript library files |
|---|---|
|Windows 32-bit|`gpdldll32.dll` `gsdll32.dll`|
|Windows 64-bit|`gpdldll64.dll` `gsdll64.dll`|
|MacOS|`libgpdl.dylib` `libgs.dylib`|
|Linux / OpenBSD|`libgpdl.so` `libgs.so`|


> **NOTE**<br>
> The actual filenames on MacOS will be appended with the version of Ghostscript with associated symlinks.

### Building on Windows

To build the required DLLs, load `/windows/ghostpdl.sln` into Visual Studio, and select the required architecture from the drop down - then right click on 'ghostpdl' in the solution explorer and choose "Build".


### Building on MacOS or Linux / OpenBSD

Firstly run the `autogen.sh` script from the command line to create the required configuration files followed by `make so` to build the shared libraries. The scripts also depend on having both `autoconf` and `automake` installed on your system. <sup>[1]</sup>

#### autoconf & automake

If this software is not already on your system (usually this can be found in the following location: `usr/local/bin`, but it could be located elsewhere depending on your setup) then it can be installed from your OS's package system.


Alternatively, it can be installed from [GNU] here:

https://www.gnu.org/software/autoconf/

https://www.gnu.org/software/automake/

Or, it can be installed via [Brew] by running:

```
brew install autoconf automake
```

Once built, these libraries can be found in your `ghostpdl/sobin/` or `ghostpdl/sodebugbin` location depending on your build command.



> **NOTE**<br>
> For full detailed instructions on how to build your Ghostscript library see [here].
>

[Python]: https://www.python.org/
[Brew]: https://brew.sh/
[GNU]: https://www.gnu.org/

[C API]: https://www.ghostscript.com/doc/current/API.htm
[FSF web site]: https://www.gnu.org/licenses/agpl-3.0.html
[here]: https://ghostscript.com/doc/current/Make.htm
[language bindings]: https://en.wikipedia.org/wiki/Language_binding
