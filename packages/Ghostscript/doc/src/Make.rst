.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: How to Build Ghostscript from Source Code

.. include:: header.rst

.. _Make.html:

Building from Source
=========================================





General overview
------------------------

This document describes how to build a Ghostscript executable from source code. There are four major steps to building Ghostscript:

#. Acquire the compressed archive files of source code for Ghostscript.

#. Unpack the archive files into the Ghostscript directory.

#. Configure the build to match your system and desired configuration options.

#. Invoke "make" to build the software.


The remainder of this document describes each of these steps in detail. Note that some of this process is platform-dependent. After building Ghostscript you must then install it; for that, see the :ref:`installation instructions<Install.html>`.

Long term users of Ghostscript may notice the instructions for a number of older systems have been removed from this document. There is no value judgment implied in this, but recognition that the build system has changed considerably in recent years, and several of these legacy systems are no longer easily available to the development team. We will always consider contributions to continue support for legacy systems.



Built libraries
-------------------

The following Ghostscript libraries will be built for these respective platforms:

.. list-table::
   :header-rows: 1

   * - Platform
     - Ghostscript library files
   * - Windows 32-bit
     - ``gpdldll32.dll`` ``gsdll32.dll``
   * - Windows 64-bit
     - ``gpdldll64.dll`` ``gsdll64.dll``
   * - MacOS
     - ``libgpdl.dylib`` ``libgs.dylib``
   * - Linux / OpenBSD
     - ``libgpdl.so`` ``libgs.so``


.. note::

      The actual filenames on MacOS will be appended with the version of Ghostscript with associated symlinks.



How to acquire the source code
------------------------------------------------

Building Ghostscript requires the Ghostscript source code itself, and in some cases the source code for the third-party libraries that Ghostscript uses.

Official releases can be found under the AGPL license at:

https://ghostscript.com/download/

Ghostscript source code is packaged in gzip-compressed tar archives (``*.tar.gz``), e.g.:

``ghostscript-#.##.tar.gz``

("#.##" are version numbers.)

Software to decompress and extract both formats is available for almost every platform for which Ghostscript is available -- including Unix, Linux, MS Windows, and so on -- but it's up to you to locate that software. See the section on :ref:`unpacking the source code<Unpack>`.


.. note::

   Unlike earlier versions, Ghostscript packages are now one, complete archive, including font files and third party library dependency sources.


.. _Acquiring:


How to acquire the development source code
------------------------------------------------------------------------------------------------

The Ghostscript team use git_ for version control.

If you require a snapshot of the development code, the easiest way to get it is to visit the web interface to our git repository: `ghostpdl.git`_ and click the "snapshot" link next to the specific commit in which you are interested. After a short delay, that will download a complete source tree for the given commit in a gzipped tar archive.

If you require access to several commits, or wish to regularly access the latest development code, you are better to clone the entire git repository, using:

.. code-block:: bash

   git clone git://git.ghostscript.com/ghostpdl.git

which will create a local, read-only repository.

Both the "snapshot" and the git clone methods download the Ghostscript sources as part of the GhostPDL source tree, which includes the PCL/PXL and XPS interpreters also built on top of the Ghostscript graphics library.

The configure script discussed later in the document is created as part of the Ghostscript release process, and as the source tree retrieved from git is "pre-release" code, it does not include a pre-made configure script. See :ref:`autogen.sh<autogen_sh>`.


.. _Unpack:

How to unpack the source code
------------------------------------------------------------------------------------------------

Unfortunately, there are no generally accepted standards for how to package source code into archives, so the instructions for unpacking Ghostscript are longer than they should be. We begin with a brief explanation of how to extract the two kinds of archive files.



.. _TarFiles:

How to unpack compressed tar files generally
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Tar (``.tar``) files are the *de facto* standard for archiving files on Unix (every Unix-like system has the ``tar`` program), and programs to extract their contents are also widely available for MS Windows, and most other environments. To economize on space and downloading time, Ghostscript's tar files are compressed with ``GNU gzip``, which adds the suffix "``.gz``" to the file name, giving "``.tar.gz``".

To unpack a compressed tar file ``MyArchive.tar.gz`` you must both decompress it and extract the contents. You can do this in two steps, one to decompress the file and another to unpack it:


.. code-block:: bash

   gzip -d MyArchive.tar.gz
   tar -xf MyArchive.tar

or in a pipeline:

.. code-block:: bash

   gzip -d -c MyArchive.tar.gz | tar -xf -

or, if you have a program like GNU tar that can handle compressed tar files, with a single command:


.. code-block:: bash

   tar -zxf MyArchive.tar.gz


The ``tar`` program automatically preserves directory structure in extracting files. The Ghostscript source archive puts all files under a directory ``ghostscript-#.##``, so using tar to unpack a compressed archive should always properly create that directory, which we will call the "ghostscript directory".

Some other programs – under MS Windows, for instance – can also unpack compressed tar files, but they may not automatically preserve directory structure nor even extract files into the current directory. If you use one of these, you must:

- set the program's options to "Use folder names" (or the equivalent).

, and:

- check that it is extracting files into the right place.

As both ``tar`` and ``gzip`` formats are now well supported by several applications on MS Windows, we only supply the ``tar.gz`` archive.


WinZip_, `7-zip`_ & `Info-ZIP`_ are respectively a commercial and two free applications which can decompress and extract ``.tar.gz`` archives on MS Windows.



How to unpack Ghostscript itself
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

At this point you have :ref:`acquired the source code<Acquiring>` and are ready to unpack it according to the :ref:`preceding guidelines<TarFiles>`.

2-step:

.. code-block:: bash

   gzip -d ghostscript-#.##.tar.gz
   tar -xf ghostscript-#.##.tar


Pipe:

.. code-block:: bash

   gzip -d -c ghostscript-#.##.tar.gz | tar -xf -

GNU tar:

.. code-block:: bash

   tar -zxf ghostscript-#.##.tar.gz


All the Ghostscript source files are now in subdirectories of the ``ghostscript-#.##`` directory.



Ghostscript Core Source subdirectories
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


   .. list-table::
      :widths: 50 50
      :header-rows: 1

      * - Subdirectory
        - Contents
      * - ``arch/``
        - Pre-defined architecture header files
      * - ``base/``
        - Graphics library C source code and ``makefiles``
      * - ``contrib/``
        - Community contributed/supported output devices
      * - ``devices/``
        - The output devices supported by the Ghostscript team
      * - ``psi/``
        - PS interpreter C source code and makefiles
      * - ``Resource/``
        - Postscript initialization, resource and font files
      * - ``lib/``
        - PostScript utilities and scripts used with Ghostscript
      * - ``doc/``
        - Documentation
      * - ``man/``
        - Unix man pages
      * - ``examples/``
        - Sample PostScript files
      * - ``iccprofiles/``
        - Default set of ICC profiles
      * - ``windows/``
        - Visual Studio for Windows specific project and solution files
      * - ``toolbin/``
        - Useful (non-Postscript) tools, mostly for developer use only



Optionally, if you downloaded the GhostPDL archive, you may also have:

Additional GhostPDL source subdirectories
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


   .. list-table::
      :widths: 50 50
      :header-rows: 1

      * - Subdirectory
        - Contents
      * - ``pcl/``
        - PCL/PXL interpreter C source code, makefiles, fonts etc.
      * - ``xps/``
        - XPS interpreter C source code and makefiles


Supporting third party libraries will also be in their own sub-directories (e.g. jpeg, freetype and so on).




How to check for post-release bug fixes
---------------------------------------------

Bug information and fixes are tracked on `Ghostscript Bugzilla`_.



.. _Make_MakeFilesOverview:

How to prepare the ``makefiles``
---------------------------------------------

The Ghostscript ``makefiles`` are very large and complex in order to deal with the diverse requirements of all the different systems where they may be used.

Ghostscript has an automatic configuration script. If you're on unix or a system that supports unix shell scripts, this is the easiest option to use. Simply type:


.. code-block:: bash

   ./configure

from the top level of the Ghostscript source directory. It should configure itself based on what's available on your system, warn you of any missing dependencies, and generate a ``Makefile``. At this point you can skip to the section :ref:`invoking make<invokingMake>` below. Also, many common configuration options (like install location) can be set through options to the ``configure`` script.

Type ``./configure --help`` for a complete listing. Note that the configuration option is only available with the unix ``.tar`` distributions of the source.

.. _autogen_sh:


.. note::

   If you're building Ghostscript from development source out of a repository instead of from a released source package, you should run ``./autogen.sh`` instead of ``./configure``. This script takes all the same options that ``configure`` does.

If your system doesn't support the ``configure`` script or you don't wish to use it, you can use the traditional Ghostscript ``makefile`` system, editing the options by hand to match your system as described below. Fortunately, the only ``makefiles`` you're likely to want to change are relatively small ones containing platform-specific information.


.. _platformSpecificMakefiles:

Platform-specific makefiles
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The table below lists a number of platform independent ``makefiles`` in each of the core Ghostscript source directories.

.. list-table::
      :widths: 50 50
      :header-rows: 1

      * - Makefile
        - Used for
      * - ``Makefile.in``
        - Template ``makefile`` for the ``autoconf`` build.
      * - ``psi/msvc.mak``
        - MS Windows with Microsoft Visual Studio 2003 and later.
      * - ``base/unix-gcc.mak``
        - Unix with ``gcc``.
      * - ``base/unixansi.mak``
        - Unix with ANSI C compilers other than ``gcc``.


Since these files can change from one Ghostscript version to another, sometimes substantially, and since they all include documentation for the various options, here we don't duplicate most of that documentation: we recommend strongly that you review the entire ``makefile`` specific for your operating system and compiler before building Ghostscript.


.. _PreparingMakefiles:

Changes for your environment
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Assuming you have opted not to use the ``configure`` script or the default Microsoft Visual Studio bulid, you must edit the platform-specific ``makefile`` to change any of these:

- The name of the ``makefile`` itself (``MAKEFILE`` macro).

- The locations to install Ghostscript files (prefix etc.).

- The default search paths for the initialization and font files (``GS_LIB_DEFAULT`` macro).

- The debugging options (``DEBUG`` and ``TDEBUG`` macros).

- Which optional features to include (``FEATURE_DEVS``).

- Which device drivers to include (``DEVICE_DEVS`` and ``DEVICE_DEVS{1--20}`` macros).

- Default resolution parameters for some printer drivers (``devs.mak`` or ``contrib.mak``, whichever defines the driver).



In general these will be set to commonly sensible values already, but may not be ideal for your specific case.



The :ref:`platform-specific makefiles<platformSpecificMakefiles>` include comments describing all these except the ``DEVICE_DEVS`` options. These are described in ``devs.mak`` and ``contrib.mak``, even though the file that must be edited to select them is the :ref:`platform-specific makefile<platformSpecificMakefiles>`.

Some platform-specific options are described in the sections for individual platforms. See the "Options" section near the beginning of the relevant ``makefile`` for more information.





Selecting features and devices
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

You may build Ghostscript with any of a variety of features and with any subset of the available device drivers. The complete list of features is in a comment at the beginning of ``gs.mak``, and the complete list of drivers in comments at the beginning of ``devs.mak`` and ``contrib.mak``. To find what devices a platform-specific ``makefile`` selects to include in the executable, look in it for all lines of the form:


.. code-block:: bash

   FEATURE_DEVS={list of features}
   DEVICE_DEVS*={list of devices}


For example, if the ``makefile`` has:


.. code-block:: bash

   FEATURE_DEVS=$(PSD)level2.dev


indicating that only the PostScript Level 2 facilities should be included, you might make it:



.. code-block:: bash

   FEATURE_DEVS=$(PSD)level2.dev $(PSD)pdf.dev


to add the ability to interpret PDF files. (In fact, ``FEATURE_DEVS`` in the current Unix ``makefiles`` already includes ``$(PSD)pdf.dev``.).

It is extremely important that ``FEATURE_DEVS`` is set correctly. Currently, the default builds will include a complete feature set, and as such most of those building Ghostscript will have no need to change it. Only those working in heavily resource constrained environment will want to experiment, and it is vital that the implications of such changes be understood, otherwise Ghostscript may behave in unexpected or apparently incorrect ways, or may even fail to build.


The Unix ``makefile`` also defines:


.. code-block:: bash

   DEVICE_DEVS=$(DD)x11.dev


indicating that the X Windows driver should be included, but since platform-specific ``makefiles`` as distributed normally include many of the possible features and drivers, you will probably rather remove from the ``makefile`` the features and drivers you don't want. It does no harm to include unneeded features and devices, but the resulting executable will be larger than needed.

You may edit the ``FEATURE_DEVS`` line to select or omit any of the features listed near the beginning of ``gs.mak``, and the ``DEVICE_DEVS*`` lines to select or omit any of the device drivers listed near the beginning of ``devs.mak`` and ``contrib.mak``. ``GS_DEV_DEFAULT`` is a string containing whitespace separate device names, and give the devices Ghostscript should attempt to use (and the order) if no device is specified on the command line; see the usage documentation for how to select an output device at run time using the ``-sDEVICE=`` switch. If you can't fit all the devices on a single line, you may add lines defining:

.. code-block:: bash

   DEVICE_DEVS1=$(DD){dev11}.dev ... $(DD){dev1n}.dev
   DEVICE_DEVS2=$(DD){dev21}.dev ... $(DD){dev2n}.dev


etc., up to ``DEVICE_DEVS15``. Don't use continuation lines -- on some platforms they don't work.


.. note::

   If you want to include a driver named ``xxx``, you must put ``$(DD)xxx.dev`` in ``DEVICE_DEVS*``. Similarly, if you want to include a feature related to the PostScript or PDF language interpreters (PostScript level 1 .. 3, or other language features such as the ability to read EPSF files or TrueType font files), you must represent it as ``$(PSD)xxx.dev``.


Precompiled run-time data
""""""""""""""""""""""""""""""""""

Ghostscript normally reads a number of external data files at run time: initialization files containing PostScript code, fonts, and other resources such as halftones. By changing options in the top-level ``makefile`` for the platform, you can cause some of these files to be compiled into the executable: this simplifies installation, improves security, may reduce memory requirements, and may be essential if you are planning on putting Ghostscript into ROM. Compiling these files into the executable also means the executable is (largely) self-contained, meaning initialization files, font files, resource files and ICC profile files are certain to be available and accessible. In general, Ghostscript should initialize more quickly, and files (especially PDF) files making heavy use of the built-in fonts will interpret more quickly.

For those distributing Ghostscript binaries, compiling those files into the executable has another implication, any site-specific customizations (such as font and ``CIDFont`` substitutions) are slightly more complex to implement - see: :ref:`How Ghostscript finds files<Use_How Ghostscript finds files>` for how to influence where Ghostscript searches for files. Furthermore, if the files Ghostscript uses are also required to be accessible by applications other than Ghostscript (the mostly case for this would be font files and ICC profile files), having those files compiled into Ghostscript maybe suboptimal, essentially require two copies of the file data to be distributed (one set built into Ghostscript, and the other as "normal" files accessible outside of Ghostscript.



Compiling the initialization files (``Resource/Init/gs_init.ps``, etc.) into the executable is the default. To disable this, change the 1 to a 0 in the line:


.. code-block:: postscript

   COMPILE_INITS=1


Or, if you use the configure based Unix-style build, you can disable ``COMPILE_INITS`` by adding the option ``--disable-compile-inits`` to the invocation of configure

Files are now compiled into the executable as a ``%rom%`` file system that can be searched, opened, etc. as with the normal (``%os%``) file system. The data is (mostly) compressed. Several of the initialisation files (those in ``Resource/Init``) are also converted to binary Postscript encoding, and "merged" into a single monolithic file - this is done for both size and speed optimization. Files that are often customized for individual installations (such as ``Fontmap`` and ``cidfmap``) are not merged into the single file and thus installation specific versions can be used.

The set of files built into the ``%rom%`` file system is specified in the ``psi/psromfs.mak`` file. By default the set of files built into the rom file system comprises all the resource files Ghostscript requires to run successfully (all the files under ``Resource`` directory, and those under the ``iccprofiles`` directory). Refer to the file ``base/mkromfs.c`` for a description of the parameters that control source and destination pathnames, file enumeration exclusion, compression, etc.

Fonts normally are compiled into the executable using ``mkromfs`` (above) from the ``Resource/Font/`` directory.

Similarly, Halftone resources can be compiled into the executable using ``mkromfs``, but also threshold-array halftones can be compiled into the executable. See the "Compiled halftone" section of ``int.mak`` for a sample ``makefile`` fragment, ``genht.c`` for the syntax of halftone data files, and ``lib/ht_ccsto.ps`` for a sample data file. Note that even though the data files use PostScript syntax, compiled halftones do not require the PostScript interpreter and may be used with the graphics library alone.


Setting up "makefile"
~~~~~~~~~~~~~~~~~~~~~~~~~~~~

After going through the steps just described to :ref:`unpack the sources<Unpack>`, configure the build and make any desired :ref:`changes to the makefiles<PreparingMakefiles>`. As the final step in preparing to build Ghostscript you must usually associate the name "makefile" with the correct ``makefile`` for your environment so the make command can find it. See the section on your particular platform for how to do that if necessary.

On unix systems, ``./configure`` (or if checked out of git, ``./autogen.sh``) should create a ``Makefile`` which works in most scenarios. Manual tampering and editing should rarely be needed nor recommended.




.. _invokingMake:


Invoking "make"
~~~~~~~~~~~~~~~~~~~~~~~~~~~~

``make``
   Builds :title:`Ghostscript` without debugging options.


.. _Make_Debugging:

``make debug``
   Builds :title:`Ghostscript` with debugging options and additional internal error checks. The program will be somewhat larger and slower, but it will behave no differently unless you actually turn on debugging options at execution time with the ``-DDEBUG`` or :ref:`-Z command line switches<Use_Debugging>` described in the usage documentation.

``make pg``
   On Unix platforms, builds with the ``-pg`` compiler switch, creating an executable for time profiling.

``make install``
   After building, installs the Ghostscript executables, support files, and documentation, but does not install fonts. See the :ref:`installation documentation<Install.html>`.

``make (debug)clean``
   Deletes all the files created by the build process (relocatables, executables, and miscellaneous temporary files). If you've built an executable and want to save it, move it first to another place, because "make clean" deletes it.


.. _Make_SharedObject:

``make so``
  On some platforms (Linux, \*BSD, Darwin/Mac OS X, SunOS), it is possible to build :title:`Ghostscript` as a shared object library. There is a corresponding ``make soclean`` for cleaning up.

``make sanitize``
  Builds :title:`Ghostscript` with ``AddressSanitizer``. Output is placed in ``./sanbin``.

``make libgs``
  Builds static library for :title:`Ghostscript`.

``make libgpcl6``
  Builds static library for :title:`GhostPCL`. Requires the full ghostpdl_ source release.

``make libgxps``
  Builds static library for :title:`GhostXPS`. Requires the full ghostpdl_ source release.

``make libgpdl``
  Builds static library for :title:`GhostPDL`. Requires the full ghostpdl_ source release.



.. note::

   - On some platforms aspects of these simple instructions don’t quite work in one way or another. Read the section on your specific platform.

   - If you are attempting to build a statically linked executable, you will probably need to add libraries to the linker options (libraries that are normally pulled-in automatically by the dynamic linker). These can be added at the make command line using the ``EXTRALIBS=`` option. Unfortunately, the set of libraries that may be required varies greatly depending on platform and configuration, so it is not practical to offer a list here.



Cross-compiling
~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Cross-compiling is not fully supported by the configure script (such support is a work-in-progress).

You can either use ``base/unixansi.mak`` or ``unix-gcc.mak`` as the basis for a cross-compile ``makefile``, or use configure to create a basic ``Makefile`` as the basis. And modify to suit.

You can set the compiler to your cross-compiler for configure by doing:


.. code-block:: bash

   ./configure CC=<cross-compiler executable>


and configure will then run its checks (as best it can) with the cross-compiler.

If you do so, you should also give configure the option to set the target architecture endianness: ``--enable-big-endian`` or ``--enable-little-endian``.

It would also be wise to review the settings shown in the output of ``./configure --help`` for any that would be applicable to your target.


The Ghostscript build system uses several interim executables, built and run on the host, as such, even when cross-compiling, a host native compiler is also required. You must edit your ``makefile`` to ensure that is available. Find the line that starts:

``CCAUX=``

and set that to your host compiler.

If you did not use configure or did not set the CC variable for configure, you must also set the:

``CC=``

to your cross-compiler.

The Ghostscript build system uses a utility called ``genarch`` (see ``base/genarch.c`` for details) to interrogate the environment and generate a header file describing the architecture for which Ghostscript is being built. As this is run on the ``host`` it will generate header for the ``host architecture`` rather than that of the target.

For cross compiling, you must create (or modify) a header file (``arch.h``) which accurately describes the ``target architecture``. Then you must edit your ``makefile`` by finding the line:

``TARGET_ARCH_FILE=``

and set it to the path to, and file name of your custom ``arch.h`` file. With that setting, ``genarch`` will still be run, but rather than interrogate the current environment, it will copy the contents of your custom ``arch.h`` to the build.




How to build Ghostscript from source (PC version)
-------------------------------------------------------------

All Ghostscript builds in PC (DOS and MS Windows) environments are 32- or 64-bit: 16-bit builds are not supported. The relevant ``makefiles`` are:


.. list-table::
      :widths: 20 40 40
      :header-rows: 1

      * - Makefile
        - Construction tools
        - For environment
      * - ``msvc.mak``
        - :ref:`Microsoft Visual Studio .NET 2003 (or later)<Microsoft build>`
        - MS Windows 32/64-bit
      * - ``Makefile.in``
        - :ref:`Cygwin/gcc<Cygwin build>`
        - Cygwin (Use Unix configure)


Ghostscript requires at least MS Windows 95 (although we no longer actively test nor support Win95, we have not deliberately done anything to break compatibility with it). We recommend at least MS Windows NT 4.0.

For building, Ghostscript requires at least Visual Studio .NET 2003, and we recommend at least Visual Studio 2019. It can probably be made to work with earlier versions, though at least VS2005 will be required for 64 bit Windows support.


.. note::

   The ``make`` program supplied with Visual Studio (and earlier Visual C++ versions) is actually called ``nmake``. We refer to this program generically as make everywhere else in this document.

You must have ``cmd.exe`` in your path to build Ghostscript (using the Visual Studio command prompt is ideal). After making any changes required to choose features and devices to build into the executable, you can then invoke ``make`` to build the executable.


.. _Microsoft build:
.. _Make Building with Visual Studio:

Microsoft Visual Studio
~~~~~~~~~~~~~~~~~~~~~~~~~~~

Using Microsoft Visual Studio
""""""""""""""""""""""""""""""""

To build the required DLLs, load ``/windows/ghostpdl.sln`` into Visual Studio, and select the required architecture from the drop down - then right click on 'ghostpdl' in the solution explorer and choose "Build".

Further details
""""""""""""""""""

The Ghostscript source distribution ships with ``project`` and ``solution`` files for Visual Studio 2015 and later. These can be found in the ``windows`` directory. The ``project(s)`` are ``nmake projects`` which means that rather than Visual Studio controlling the build directly, it delegates the build process to the ``nmake``.

Beyond lacking support for parallel builds (``nmake`` cannot support parallel builds), there should be little visible difference between a conventional VS project and an ``nmake project`` to the user of the VS graphical interface. The only exception to that is if you have to make changes to build options beyond those available in the defined build configurations. In that case, you need to find the ``Nmake`` tab in the project ``Property Pages`` and modify the appropriate entry: ``Build Command Line``, ``Rebuild All Command Line`` and/or ``Clean Command Line``.

As mentioned above, ``nmake`` does not support parallel builds. If you have downloaded and are building the GhostPDL source archive (which contains Ghostscript, GhostPCL, GhostXPS, and GhostPDL "products"), the ``GhostPDL.sln`` contains individual projects for each product but, as a result of the limitations of ``nmake`` the products cannot be built in parallel, because ``nmake's`` lack of parallel build awareness means it cannot manage the dependencies shared between the products, and may fail as multiple builds attempt to access the same dependencies.

To build all the products in one action, use the ``All`` "pseudo-project". The ``All`` project uses a single nmake invocation to build all the supported products.


.. note::

   Changing the ``Output`` property in the ``Nmake`` properties will not change the name of the executable - to do that requires editing of the ``psi/msvc.mak makefile``, or you can add: ``GS=myname.exe`` to the ``nmake`` command line.


Using the command line
""""""""""""""""""""""""""""""""

Ghostscript can be made using the Windows command prompt or one of the various command line shells made for Windows, as long as the command line syntax is compatible with the Windows ``CMD.exe``. The Visual Studio command prompt is ideal.

In order for the ``makefiles`` to work properly, two items may have to be changed. An attempt is made to select the correct version of Microsoft Visual C++ based on the version of ``nmake``. If this doesn't work it will default to version 6.x. If the auto-detection does not work, and you are not using version 6.x then before building, in ``psi\msvc.mak`` find the line ``#MSVC_VERSION=6`` and change it to ``MSVC_VERSION=4``, ``MSVC_VERSION=5``, ``MSVC_VERSION=7`` or ``MSVC_VERSION=8`` and so on.

In some cases the location of the Microsoft Developer Studio, needs to be changed. The location of Microsoft Developer Studio is defined by the value of ``DEVSTUDIO``. There are several different definitions of ``DEVSTUDIO`` in ``psi\msvc.mak``. There is one for each of the currently supported versions of Microsoft Visual C++ (4, 5, 6, 7, 7.1 and 8).

The normal installation process for Microsoft Visual C++ includes setting the location of the Microsoft Visual C++ executables (``cl.exe``, ``link.exe``, ``nmake.exe``, ``rc.exe``) in your ``PATH`` definition and the ``LIB`` and ``INCLUDE`` environment variables are set to point to the Microsoft Visual C++ directories. If this is true then the value for ``DEVSTUDIO`` can be changed to empty, i.e. ``DEVSTUDIO=``

If ``PATH``, ``LIB``, and ``INCLUDE`` are not correctly set then the value for ``DEVSTUDIO`` needs to be defined. For example, for version 6.0, the default definition for the location for the Microsoft Developer Studio is: ``DEVSTUDIO=C:\Program Files\Microsoft Visual Studio`` If the path to Microsoft Developer Studio on your system differs from the default then change the appropriate definition of ``DEVSTUDIO``. (Remember that there is a separate definition of ``DEVSTUDIO`` for each version of ``MSVC``, so be sure to change the correct definition.)

To run the make program, give the command:


.. code-block:: bash

   nmake -f psi\msvc.mak


Rather than changing ``psi/msvc.mak``, these values can also be specified on the make command line, i.e.


.. code-block:: bash

   nmake -f psi\msvc.mak MSVC_VERSION=6 DEVSTUDIO="C:\Program Files\Microsoft Visual Studio"
   nmake -f psi\msvc.mak MSVC_VERSION=7 DEVSTUDIO="C:\Program Files\Microsoft Visual Studio .NET"


Note that double quotes have been added around the path for ``DEVSTUDIO`` due to the spaces in the path value.

This command line can also be put into a ``batch`` file.

You may get warning messages during compilation about various undefined and/or unsupported switches - this is because the compiler switches are set in the ``makefiles``, and are applied when building with all versions of Visual Studio, but not all options are supported (or required) by all versions of Visual Studio. These warnings are benign and can be ignored.



Microsoft Environment for 64-bit
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Building Ghostscript for 64-bit Windows (AMD64 processor) requires Microsoft Visual Studio .NET 2005 or Microsoft Visual Studio 2008 or later on 64-bit Windows. Cross compiling on 32-bit Windows is possible.

Compiling for 64-bit is similar to the Microsoft Environment instructions above, but with the addition of a ``WIN64`` define.

To make Ghostscript use:


.. code-block:: bash

   nmake -f psi/msvc.mak WIN64=


Making self-extracting installers
""""""""""""""""""""""""""""""""""""""

You can build self-extracting Windows installers based on NSIS (Nullsoft Scriptable Install System). To do so, use the ``nsis makefile`` target as well as any other options, for example:


.. code-block:: bash

   nmake -f psi/msvc.mak WIN64= nsis


will create an ``nsis`` based installer for Ghostscript built for 64 bit Windows systems.


Microsoft Environment for WinRT
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Ghostscript can be built in the form of a ``win32 DLL`` for use within a Windows Runtime application or Windows Runtime component. Building for WinRT requires use of Microsoft Visual Studio 2012. There is a solution file that can be loaded into VS 2012, in the directory ``winrt``.

The WinRT application or component should include ``iapi.h`` from ``gs/psi`` and link with ``gsdll32metro.lib`` from ``gs/debugbin`` or ``gs/releasebin``. Also any app using Ghostscript either directly or via a component should add ``gsdll32metro.dll`` as "content". This inclusion of the dll is necessary so that it will be packaged with the app. If one wishes to be able to run the debugger on Ghostscript then ``gsdll32metro.pdb`` should also be added as content.


.. _Cygwin build:

Cygwin32 gcc
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

It is possible to compile Ghostscript for MS Windows using the Cygwin32 gcc compiler, ``GNU make``, using the "configure" generated ``Makefile``.

Information about this compiler and environment is at the `Cygwin site`_.


MSys/Mingw
""""""""""""""

The configure build can be used to build Ghostscript on ``MSys/Mingw`` systems, but with a caveat. The ``msys-dvlpr`` adds header files into the compiler's header search paths which cause a clash, and the build will fail as a result. If you have the ``msys-dvlpr`` package installed, and until a better solution is available you can work around this by temporarily renaming the ``\mingw\msys\1.0\include`` directory so those headers are no longer found by the compiler.



.. _Make Building with MacOS:

How to build Ghostscript from source (MacOS version)
---------------------------------------------------------

MacOS X
~~~~~~~~~~~~~

The unix source distribution (``.tar.gz``) builds fine on Darwin/MacOS X, albeit without a display device. You can generally just use the ``Makefile`` generated by configure as your top-level ``makefile`` and get a reasonable default build. This will allow you to use Ghostscript from the command line as a BSD-layer tool to rasterize postscript and pdf to image files, and convert between the high-level formats supported by Ghostscript. See the :ref:`instructions for the unix build<MakeHowToBuildForUnix>` below for details of how to customize this build.


.. note::

   If you have MacPorts_ installed, it can "confuse" the configure script because it includes some librares which duplicate the "system" ones. This can cause missing symbol link errors. In order to resolve this, you can do: ``LDFLAGS="-L/usr/lib" ./configure``. That will force the linker to search the default directory first, and thus pick up the system libraries first.

It is also possible to build "universal binaries" for MacOS X, containing i386 and x86_64 binaries in one file, using the ``Makefile`` from ``configure``. This can be achieved by using the following invocation of ``configure``:


.. code-block:: bash

   ./configure CC="gcc -arch i386 -arch x86_64 -arch ppc" CPP="gcc -E"



You can choose the combination of valid architectures (i386/x86_64/ppc) that you require.

The separate options for ``CC`` and ``CPP`` are required because some of the features used by configure to explore the capabilities of the preprocessor are not compatible with having multiple ``-arch`` options.

Building a shared library on MacOS X is the same as for other Unix-like systems, the "configure" step is done normally, and the "so" target is given to the make invocation, thus:

.. code-block:: bash

   make so

The only difference compared to other Unix-like systems is that on OS X the resulting shared library is created with the ".dylib" file name extension, instead of the more usual ".so".


.. _Make Building with Unix:
.. _MakeHowToBuildForUnix:

How to build Ghostscript from source (Unix version)
---------------------------------------------------------

Ghostscript now ships with a build system for unix-like operating systems based on ``GNU Autoconf``. In general the following should work to configure and build Ghostscript:


.. code-block:: bash

   ./configure
   make

or

.. code-block:: bash

   ./configure
   make so

for building Ghostscript as a shared library.

Please report any problems with this method on your system as a bug.

On modern unix systems, ``./configure`` should create a ``Makefile`` which works in most scenarios. Manual tempering and editing should rarely be needed nor recommended.

.. note::

   If you're building Ghostscript from development source out of a repository instead of from a released source package, you should run ``./autogen.sh`` instead of ``./configure``. This script takes all the same options that ``configure`` does.

(deprecated; see Autoconf-based method above) For the convenience of those already familiar with Ghostscript, the old method based on hand-edited makefiles is still possible but no longer supported (and in many cases, simply do not work without substantial expert manual-editing effort). It may also be helpful in getting Ghostscript to build on very old platforms. The rest of this section deals exclusively with that older method and includes numerous pointers regarding legacy systems.

(deprecated; see Autoconf-based method above) Before issuing the make command to build Ghostscript, you have to make some choices, for instance:

- Which compiler to use.

- What features and devices to include.

- Whether to use system libraries for PNG and zlib.

- How to handle issues for your particular platform.

Be sure to check the sections on tool-, OS-, and hardware-specific issues for notes on your particular platform and compiler. In fact, that is the first place to check if you build Ghostscript and it crashes or produces obviously incorrect results.




make tools
~~~~~~~~~~~~~~

You require a make tool which supports separate directories for the derived objects (such as object files, executables and dynamically created header files) and the source files.

In general, GNU make is the recommended choice, and some features (such as the building of the Linux/Unix shared library build ("make so") are only available with GNU make.

Other make implementations are known to work, but are not guaranteed to do so.


GNU make
"""""""""""""

Current versions of ``GNU make`` have no problems building Ghostscript.



OS-specific issues
~~~~~~~~~~~~~~~~~~~~~~~~~~~~

MacOS or Linux / OpenBSD
""""""""""""""""""""""""""""""""""""""""

Running the ``autogen.sh`` script from the command line depends on having both ``autoconf`` and ``automake`` installed on your system.

If this software is not already on your system (usually this can be found in the following location: ``usr/local/bin``, but it could be located elsewhere depending on your setup) then it can be installed from your OS's package system.

Alternatively, it can be installed from `GNU Software`_


Or, it can be installed via Brew by running:

.. code-block:: bash

   brew install autoconf automake

Once built, these libraries can be found in your ``ghostpdl/sobin/`` or ``ghostpdl/sodebugbin`` location depending on your build command.



H-P RISC workstations
""""""""""""""""""""""""""

(see Autoconf-based method above)

- HP-UX versions before 11.0 do not support ``POSIX`` threads. Set ``SYNC=nosync`` in the ``makefile`` before building.

- Ghostscript builds on H-P machines with either ``GNU gcc`` or H-P's ANSI-capable ``cc``. The minimal, non-ANSI-capable ``cc`` that shiped with some basic HPUX system does not work. If ``cc`` on your system doesn't accept the ``-Aa`` switch, then you need to get the full ``cc`` or ``gcc``.

- If you use H-P's compiler, be sure you have upgraded to a recent release. Many bizarre symptoms have been reported trying to build Ghostscript with older, buggier compilers, for example:

   - The link step fails with a message about "max" not being defined.

   - The build succeeds, but the resulting executable fails to start up, with an error message like "Initializing... Unrecoverable error: typecheck in .registerencoding".

   - The build succeeds, but the resulting executable produces a black background on the first page of output.

- It is reported that On HPUX 9.* you need at least compiler patch PHSS_5723 and ``dld.sl`` patch PHSS_5734 to build Ghostscript. (As of late 1997, those patches are long obsolete; the current patches are compiler PHSS_10357 and ``dld.sl`` PHSS_11246. It is unknown whether current Ghostscript releases work with ``compiler/dld.sl`` versions older than these).

- On HPUX 10.*, we don't know what combinations of compiler version and switches work. It is reported that On HPUX 10.20, setting ``CC=c89`` and ``CFLAGS=+O3 $(XCFLAGS)`` works, contradicting the information in the next paragraph, but this may be dependent on the specific compiler version.

- In either HPUX version, you need to set ``CC=cc -Aa`` (or use ``-Ae`` if you prefer), and set ``CFLAGS=-D_HPUX_SOURCE -O $(XCFLAGS)``. Higher levels of optimization than -O may work depending on your compiler revision; some users have reported success with +O3, some have not.

- Some users have reported needing ``-DNOSYSTIME`` and ``-D_POSIX_SOURCE`` in ``CFLAGS``, but recent tests do not show these to be necessary.

- If you use ``gcc``, it's a good idea to have a recent release -- at the very least 2.7.2.1 or later. You may be able to get a working executable with an older ``gcc`` by removing ``-O`` from ``CFLAGS``.


IBM AIX
""""""""""""""""""""""""""

We recommend installing ``gcc`` and ``GNU make``, and using the Autoconf-based method.

Other combinations are known to work, but are less well supported.

Recent veresions of Ghostscript can trigger a 'TOC overflow' error with some compilers on AIX. If this occurs, use the linker flag ``-bbigtoc``, which can either be added to your configure options:


.. code-block:: bash

   configure LDFLAGS="-Wl,-bbigtoc"


Or on the make command line:


.. code-block:: bash

   make XLDFLAGS="-Wl,-bbigtoc"



Silicon Graphics
""""""""""""""""""""""""""

(see Autoconf-based method above)

Users have had a lot of problems with the MIPSpro compilers on SGI systems. We recommend using ``gcc``. If you do choose to use the MIPSpro compiler, please read the following carefully.

- To make the optimizer allocate enough table space, set:

      ``CFLAGS="-Olimit 2500"`` (for older compilers)
      ``CFLAGS="-OPT:Olimit=2500"`` (for newer compilers)

- MIPSpro compiler version 3.19 is "older", and 7.1 is "newer"; we aren't sure at what point in between the latter syntax was introduced.

- With the compiler shipped with Irix 5.2, use the ``-ansi`` option.

- The SGI C compiler may produce warnings about "Undefined the ANSI standard library defined macro stdin/stdout/stderr". To suppress these warnings, add ``-woff 608`` to the definition of ``CFLAGS``.

- The SGI C compiler shipped with Irix 6.1 and 6.2 will not compile ``zlib/deflate.c`` properly with optimization. Compile this file separately without ``-O``.

- With IRIX 6.5.x and the MIPSpro 7.x compilers there have been reports about incorrect output and binaries that cause segmentation faults. Various solutions have been suggested and you may want to try them in this order, until you get a working binary:

   - Compile ``idict.c`` and ``isave.c`` separately without optimization after doing a normal compile; then relink.e.g.:

      .. code-block:: bash

         cc -OPT:Olimit=2500 -I. -I./obj -o ./obj/idict.o -c ./idict.c
         cc -OPT:Olimit=2500 -I. -I./obj -o ./obj/isave.o -c ./isave.c


   - Set ``CFLAGS=`` (no optimization).

   - Use only ``-O2``. Compiler produces incorrect output with ``-O3`` or ``-Ofast=ip32 -show``.

   - Irix 6.5.1m with MIPSpro compiler 7.2.1.1m, Irix 6.5.3m with MIPSpro compiler 7.2.1, and probably other 6.5x / 7.2x combinations require compiling with the ``-o32`` option. Compiling with the (default) ``-n32`` option produces non-working executables. ``-O2`` is OK (possibly except for ``idict.c``), but not ``-O3``.


Oracle/Sun
~~~~~~~~~~~~~~~~
(see Autoconf-based method above)

- The Sun unbundled C compiler (SC1.0) doesn't compile Ghostscript properly with the ``-fast`` option: Ghostscript core-dumps in ``build_gs_font``. With that compiler use ``-g``, or use ``gcc`` instead.

- The Sun version of ``dbx`` often gives up with an error message when trying to load Ghostscript. If this happens, use ``GNU gdb`` instead. (``gdb`` is more reliable than ``dbx`` in other ways as well).

- A bug in some versions of ``zlib`` results in an undefined symbol ``zmemcmp`` when compiling with Sun ``cc``. Use ``gcc`` instead.



Solaris
~~~~~~~~~~~~~~~~

- Solaris 2.2 may require setting ``EXTRALIBS=-lsocket``. Solaris 2.3 and later seem to require ``EXTRALIBS=-lnsl -lsocket -lposix4``.

- For Solaris 2.6 (and possibly some other versions), if you set ``SHARE_LIBPNG=1``, ``SHARE_ZLIB=1``, or ``SHARE_JPEG=1``, you may need to set ``XLDFLAGS=-R /usr/local/xxx/lib:/usr/local/lib`` using the full path names of the relevant directories.

- Solaris 2.n uses ``/usr/openwin/share/include`` for the X11 libraries rather than ``/usr/local/X/include``.

- Solaris 2.n typically has Type 1 fonts in ``/usr/openwin/lib/X11/fonts/Type1/outline``.

- For Solaris 2.n in the ``makefile`` you must change the definition of ``INSTALL`` from ``install -c`` to ``/usr/ucb/install -c``.

- You may need to set ``XLIBDIR`` to the directory that holds the X11 libraries, as for other SVR4 systems. Set ``-DSVR4`` in ``CFLAGS``.

- If you are using the SunPRO C compiler, don't use optimization level ``-xO3``. On ``SPARC`` platforms the compiler hangs; on Intel platforms the generated code is incorrect. With this compiler on Intel, do not use the ``-native`` flag: floating point computations become unacceptably inaccurate. You can use ``-xcg92`` (``SPARC V8``) and ``-dalign`` for better performance.

- One user reported compiling from source on a Linux NFS mounted volume failed. Compiling from a local volume was the workaround.


Other environments
--------------------

Environments lacking multi-threading
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

All environments mentioned here by name have multi-threading capability. However, if your environment doesn't, you can remove all need for multi-threading by setting ``SYNC=nosync`` in the top-level ``makefile``. Note that you will not be able to use any so-called "async" drivers (drivers that overlap interpretation and rasterization) if you do this. No such drivers are in the ``DEVICE_DEVS*`` lists of any ``makefile`` that we distribute.

Plan 9
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Use ``unix-gcc.mak``, editing it to define:

.. code-block:: bash

   CC=cc GCFLAGS=-D_BSD_EXTENSION -DPlan9

You will also probably have to edit many path names.



.. _Make_USFTBuild:


How to build Ghostscript with UFST
-------------------------------------------


.. note::

   This section is only for customers who have a Monotype Imaging UFST license. Other users please skip this section.


Ghostscript sources do not include UFST sources. You need to obtain them separately. The Ghostscript distributed source include only some source modules that provide a bridge to UFST. You will also need an additional, UFST specific makefile: contact Ghostscript support for more information.

If optioned in, the Ghostscript build system will build the UFST as part of the normal bulid process (previously, the UFST was required to be built separately).

To build Ghostscript with UFST, specify additional options for "make":

``UFST_BRIDGE=1``
   Forces the UFST bridge to build.

``UFST_ROOT=path``
   Specifies the path to UFST root directory or folder.

``UFST_CFLAGS=options``
   Specifies C compiler options for UFST library. Refer to UFST manual for information about them.

``UFST_LIB_EXT=extension``
   Sets the file name extension for object libraries. You must use the appropriate one for your platform and linker.


An example for Unix/GCC :


.. code-block:: bash

   UFST_BRIDGE=1 UFST_ROOT=../ufst UFST_CFLAGS=-DGCCx86 UFST_LIB_EXT=.a


Starting with Ghostscript 9.x (Summer 2010), the above options are conveniently inserted in the ``Makefile`` with (this also automatically disable the freetype bridge):

.. code-block:: bash

   ./configure --with-ufst=../ufst

For Windows/MSVC you need only specify ``UFST_ROOT``. ``msvc.mak`` sets the other options automatically.









.. External links:


.. _git: http://git-scm.com/
.. _ghostpdl:
.. _ghostpdl.git: http://git.ghostscript.com/?p=ghostpdl.git;a=summary

.. _WinZip: http://www.winzip.com/
.. _7-zip: http://www.7-zip.org/
.. _Info-ZIP: http://www.info-zip.org/

.. _Ghostscript Bugzilla: http://bugs.ghostscript.com
.. _Cygwin site: http://www.cygwin.com/
.. _MacPorts: http://www.macports.org/

.. _GNU Software: https://www.gnu.org/software/


.. include:: footer.rst


