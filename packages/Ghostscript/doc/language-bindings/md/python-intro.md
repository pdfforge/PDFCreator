# Python overview

<div class="banner">
<div class="python-text"></div>
<div class="vendor-logo python-logo"></div>
</div>

## About

The `Python` API is provided by the file [gsapi.py] - this is the binding to the Ghostscript `C` library.

In the [GhostPDL repository] sample `Python` examples can be found in `/demos/python/examples.py`.

## Platform & setup

### Building Ghostscript

Ghostscript should be built as a shared library for your platform.

See [Building Ghostscript].

## Specifying the Ghostscript shared library

Two environmental variables can be used to specify where to find the Ghostscript shared library.

`GSAPI_LIB` sets the exact path of the Ghostscript shared library, otherwise, `GSAPI_LIBDIR` sets the directory containing the Ghostscript shared library.

If neither is defined we will use the OS's default location(s) for shared libraries.

If `GSAPI_LIB` is not defined, the leafname of the shared library is inferred
from the OS type - `libgs.so` on Unix, `libgs.dylib` on MacOS, `gsdll64.dll` on Windows 64.


## API test

The `gsapi.py` file that provides the `Python` bindings can also be used to test the bindings, by running it directly.

Assuming that your Ghostscript library has successfully been created, then from the root of your `ghostpdl` checkout run:


#### Windows

<div class="tag shellCommand"> from ghostpdl</div>

```
// Run gsapi.py as a test script in a cmd.exe window:
set GSAPI_LIBDIR=debugbin&& python ./demos/python/gsapi.py

// Run gsapi.py as a test script in a PowerShell window:
cmd /C "set GSAPI_LIBDIR=debugbin&& python ./demos/python/gsapi.py"
```

#### Linux/OpenBSD/MacOS

<div class="tag shellCommand"> from ghostpdl</div>

```
// Run gsapi.py as a test script:
GSAPI_LIBDIR=sodebugbin ./demos/python/gsapi.py
```


If there are no errors then this will have validated that the Ghostscript library is present & operational.




[Python]: https://www.python.org/
[Brew]: https://brew.sh/
[GNU]: https://www.gnu.org/
[gsapi.py]: python-gsapi
[Building Ghostscript]: index.html#building-ghostscript
[display]: https://ghostscript.com/doc/current/API.htm#display


[GhostPDL repository]: https://github.com/ArtifexSoftware/ghostpdl
