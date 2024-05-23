.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: How to Install Ghostscript


.. include:: header.rst

.. _Install.html:
.. _HowToInstallGhostscript:


Installing
===================================


Downloading
----------------------

See `Ghostscript releases`_ if you need to download a Ghostscript release.


Overview of how to install Ghostscript
----------------------------------------

You must have four things to run Ghostscript:

#. The Ghostscript executable file; on some operating systems, more than one file is required. These are entirely platform-specific. See below for details.

#. Initialization files that Ghostscript reads in when it starts up; these are the same on all platforms.

#. Check the following:

   - ``gs_*.ps`` unless Ghostscript was compiled using the "compiled initialization files" option. See the documentation of :ref:`PostScript files distributed with Ghostscript<PsFiles.html>`.

   - ``pdf_*.ps`` if Ghostscript was compiled with the ability to interpret Adobe Portable Document Format (PDF) files, that is, ``pdf.dev`` was included in ``FEATURE_DEVS`` when Ghostscript was built.

   - ``Fontmap`` and ``Fontmap.GS`` (or the appropriate ``Fontmap.xxx`` for your platform), unless you plan always to invoke Ghostscript with the :ref:`-dNOFONTMAP switch<UseDNoFontMap>`.

#. Fonts, for rendering text. These are platform-independent, but if you already have fonts of the right kind on your platform, you may be able to use those. See below for details. Also see the :ref:`documentation on fonts<Fonts.html>`.


The :ref:`usage documentation<Use.html>` describes the search algorithms used to find initialization files and font files. The per-platform descriptions that follow tell you where to install these files.



Installing Ghostscript on Unix
----------------------------------------

Ghostscript uses the common ``configure``, ``build`` and ``install`` method common to many modern software packages. In general the following with suffice to build Ghostscript:


.. code-block:: bash

   ./configure
   make


and then it may be installed in the default location with:

.. code-block:: bash

   make install


This last command may need to be performed with super user privileges.

You can set the installation directory by adding ``--prefix=path`` to the configure invocation in the first step. The default prefix is ``/usr/local``, which is to say the ``gs`` executable is installed as ``/usr/local/bin/gs``.

A list of similar configuration options is available via ``./configure --help``.

For more detailed information on building Ghostscript see :ref:`how to build Ghostscript on Unix<MakeHowToBuildForUnix>` in the documentation on building Ghostscript, especially regarding information on using the older hand edited ``makefile`` approach. Whatever configuration method you use, execute ``make install`` to install the executable and all the required and ancillary files after the build is complete.



Fonts
~~~~~~~~~~~~~~~~~~~

The ``makefile`` installs all the files except fonts under the directory defined in the ``makefile`` as prefix. Fonts need to be installed separately. The fonts should be installed in ``{prefix}/share/ghostscript/fonts``. (That is, ``/usr/local/share/ghostscript/fonts/`` if you used the default configuration above.)

If you have Adobe Acrobat installed, you can use the Acrobat fonts in place of the ones distributed with with Ghostscript by adding the Acrobat fonts directory to ``GS_FONTPATH`` and removing these fonts from ``Fontmap.GS``:
   ``Courier``, ``Courier-Bold``, ``Courier-BoldOblique``, ``Courier-Oblique``, ``Helvetica``, ``Helvetica-Bold``, ``Helvetica-BoldOblique``, ``Helvetica-Oblique``, ``Symbol``, ``Times-Bold``, ``Times-BoldItalic``, ``Times-Italic``, ``Times-Roman``, ``ZapfDingbats``



Similarly, you can have Ghostscript use other fonts on your system by adding entries to the ``fontmap`` or adding the directories to the ``GS_FONTMAP`` environment variable. See the :ref:`usage documentation<Use.html>` for more information.

For example, many linux distributions place fonts under ``/usr/share/fonts``.


Ghostscript as a shared object
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

If you've built Ghostscript as a shared object, instead of ``make install``, you must use ``make soinstall``. See :ref:`how to build Ghostscript as a shared object<Make_SharedObject>` for more details.


Additional notes on Linux
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

For Linux, you may be able to install or upgrade Ghostscript from precompiled RPM_ files using:


.. code-block:: bash

   rpm -U ghostscript-N.NN-1.i386.rpm
   rpm -U ghostscript-fonts-N.NN-1.noarch.rpm


However, please note that we do not create RPMs for Ghostscript, and we take no responsibility for RPMs created by others.



Installing Ghostscript on MS Windows
----------------------------------------


We usually distribute Ghostscript releases for Windows as a binary installer, for the convenience of most users.

Windows 3.1 (16-bit)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The last version to run on 16-bit Windows 3.1 was Ghostscript 4.03.

Windows 95, 98, Me
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The last version to be available as a binary for Windows 95/98/Me was 8.60. Although building from source with Visual Studio 2003 should produce a working binary for those versions.

Windows NT4, 2000, XP, 2003 or Vista (32-bit)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The installer is normally named ``gs###w32.exe``, where ``###`` is the release number (e.g., 871 for Ghostscript 8.71, 910 for Ghostscript 9.10).

Windows XP x64 edition, 2003 or Vista (64-bit)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The x64 installer is normally named ``gs###w64.exe`` This is for 64-bit Windows operating systems based on the x64 instruction set. Do not use this on 64-bit processors running 32-bit Windows.

Installing
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

To install Ghostscript on Windows, you should run the installer executable.

The installer is NSIS-based and supports a few standard NSIS options: ``/NCRC`` disables the ``CRC`` check, ``/D`` sets the default installation directory (It must be the last parameter used in the command line and must not contain any quotes, even if the path contains spaces. Only absolute paths are supported).


General Windows configuration
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The installer includes files in these subdirectories:

- ``gs#.##\bin``
- ``gs#.##\examples``
- ``gs#.##\lib``
- ``gs#.##\doc``
- ``gs#.##\Resource``
- ``fonts``


The actual executable files for the 32-bit Windows install, in the ``gs#.##\bin`` subdirectory, are:


.. list-table::
      :widths: 20 80
      :header-rows: 0

      * - GSWIN32C.EXE
        - Ghostscript as a 32-bit Windows command line program. This is usually the preferred executable.
      * - GSWIN32.EXE
        - 32-bit Ghostscript using its own window for commands.
      * - GSDLL32.DLL
        - 32-bit dynamic link library containing most of Ghostscript's functionality.


For the 64-bit Windows install, also in the ``gs#.##\bin`` subdirectory, they are:


.. list-table::
      :widths: 20 80
      :header-rows: 0

      * - GSWIN64C.EXE
        - Ghostscript as a 64-bit Windows command line program. This is usually the preferred executable.
      * - GSWIN64.EXE
        - 64-bit Ghostscript using its own window for commands.
      * - GSDLL64.DLL
        - 64-bit dynamic link library containing most of Ghostscript's functionality.


For printer devices, the default output is the default printer. This can be modified as follows:

.. code-block:: bash

   -sOutputFile="%printer%printer name"

If your printer is named "HP DeskJet 500" then you would use ``-sOutputFile="%printer%HP DeskJet 500"``.



If Ghostscript fails to find an environment variable, it looks for a registry value of the same name under the key

.. code-block:: bash

   HKEY_CURRENT_USER\Software\GPL Ghostscript\#.##

or if that fails, under the key:

.. code-block:: bash

   HKEY_LOCAL_MACHINE\SOFTWARE\GPL Ghostscript\#.##

where ``#.##`` is the Ghostscript version number.

Ghostscript will attempt to load the Ghostscript dynamic link library ``GSDLL32.DLL`` in the following order:


- In the same directory as the Ghostscript executable.

- If the environment variable ``GS_DLL`` is defined, Ghostscript tries to load the Ghostscript dynamic link library (``DLL``) with the name given.

- Using the standard Windows library search method: the directory from which the application loaded, the current directory, the Windows system directory, the Windows directory and the directories listed in the ``PATH`` environment variable.


The Ghostscript installer will create registry values for the environment variables ``GS_LIB`` and ``GS_DLL``.


Uninstalling Ghostscript on Windows
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

To uninstall Ghostscript, use the Control Panel, Add/Remove Programs and remove "Ghostscript #.##" and "Ghostscript Fonts". (The entries may be called "GPL Ghostscript" or "AFPL Ghostscript", rather than just "Ghostscript", depending on what version of Ghostscript was installed).

Alternatively, an uninstall shortcut is also available in the Start Menu group.




Installing Ghostscript on OpenVMS
-------------------------------------------

Support for OpenVMS has stagnated (and almost certainly bit-rotted), and as the core development team has no access to an OpenVMS environment, we are unable to bring it up to date. We will consider patches from contributors if any wish to take on the task of getting it working again. Given the very limited appeal of OpenVMS these days, however, we are unlikely to consider patches with invasive code changes.

You need the file ``GS.EXE`` to run Ghostscript on OpenVMS, and installing Ghostscript on an OpenVMS system requires building it first.

The following installation steps assume that the Ghostscript directory is ``DISK1:[DIR.GHOSTSCRIPT]``. Yours will almost certainly be in a different location so adjust the following commands accordingly.

- Download the fonts and unpack them into ``DISK1:[DIR.GHOSTSCRIPT.LIB]``.

- Enable access to the program and support files for all users with:

   .. code-block:: bash

      $ set file/prot=w:re DISK1:[DIR]GHOSTSCRIPT.dir
      $ set file/prot=w:re DISK1:[DIR.GHOSTSCRIPT...]*.*

- Optionally, add the Ghostscript help instructions to your system wide help file:

   .. code-block:: bash

      $ lib/help sys$help:HELPLIB.HLB DISK1:[DIR.GHOSTSCRIPT.DOC]GS-VMS.HLP

- Lastly, add the following lines to the appropriate system wide or user specific login script.

   .. code-block:: bash

      $ define gs_exe DISK1:[DIR.GHOSTSCRIPT.BIN]
      $ define gs_lib DISK1:[DIR.GHOSTSCRIPT.EXE]
      $ gs :== $gs_exe:gs.exe

If you have ``DECWindows/Motif`` installed, you may wish to replace the ``FONTMAP.GS`` file with ``FONTMAP.VMS``. Read the comment at the beginning of the latter file for more information.


Installing Ghostscript on MacOS
------------------------------------

The simplest way to install :title:`Ghostscript` on a Mac would be to use MacPorts_ or Homebrew_.

The installation on MacPorts would be as follows:

- Install MacPorts - https://www.macports.org/install.php
- Goto https://ports.macports.org/port/ghostscript/ & follow the instructions there.
   - If your MacPorts is out of date and cannot find the latest verison of Ghostscript be sure to `update MacPorts`_.
- At the end of the install, run ``gs --version`` in a new Terminal window to validate your installation.





.. External links

.. _RPM: http://www.rpm.org/
.. _Ghostscript releases: https://ghostscript.com/releases
.. _Homebrew: https://formulae.brew.sh/formula/ghostscript
.. _MacPorts: https://ports.macports.org/port/ghostscript/
.. _update MacPorts: https://guide.macports.org/chunked/using.html


.. include:: footer.rst

