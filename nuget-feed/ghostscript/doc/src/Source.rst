.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: Guide to Ghostscript Source Code


.. include:: header.rst

.. _Source.html:


Guide to Ghostscript Source Code
===================================


Conceptual overview
------------------------

The Ghostscript source code is divided conceptually as follows:

**PostScript interpreter**

.. list-table::
   :header-rows: 0
   :widths: 50 50

   * - PostScript operators
     - ``z*.h`` and ``z*.c``
   * - Other interpreter code
     - ``i*.h`` and ``i*.c``
   * - PostScript code
     - ``gs_*.ps``


**PDF interpreter**


.. list-table::
   :header-rows: 0
   :widths: 50 50

   * - PostScript code
     - ``pdf_*.ps``



**Graphics library**

.. list-table::
   :header-rows: 0
   :widths: 50 50

   * - Main library code
     - ``g*.h`` and ``g*.c``
   * - Streams
     - ``s*.h`` and ``s*.c``
   * - `Device drivers`_
     - ``gdev*.h`` and ``gdev*.c``
   * - `Platform-specific code`_
     - ``gp*.h`` and ``gp*.c``



PostScript interpreter
------------------------

``gs.c`` is the main program for the interactive language interpreter; ``gserver.c`` is an alternative main program that is a rudimentary server. If you configure Ghostscript as a server rather than an interactive program, you will use ``gserver.c`` instead of ``gs.c``.

Files named ``z*.c`` are Ghostscript operator files. The names of the files generally follow the section headings of the operator summary in section 6.2 (Second Edition) or 8.2 (Third Edition) of the PostScript Language Reference Manual. Each operator ``XXX`` is implemented by a procedure named ``zXXX``, for example, ``zfill`` and ``zarray``.

Files named ``i*.c``, and ``*.h`` other than ``g*.h``, are the rest of the interpreter. See the ``makefile`` for a little more information on how the files are divided functionally.

The main loop of the PostScript interpreter is the interp procedure in ``interp.c``. When the interpreter is reading from an input file, it calls the token scanner in ``iscan*.c``.

``idebug.c`` contains a lot of debugger-callable routines useful for printing PostScript objects when debugging.


PDF interpreter
--------------------

The PDF interpreter is written entirely in ``C`` meaning it's faster than the old interpreter, uses less memory, is more robust and more secure because it can't execute PostScript. Furthermore it is easier to maintain than previous versions.



Graphics library
------------------

Files beginning with ``gs``, ``gx``, or ``gz`` (both ``.c`` and ``.h``), other than ``gs.c`` and ``gserver.c``, are the Ghostscript library. Files beginning with ``gdev`` are device drivers or related code, also part of the library. Other files beginning with ``g`` are library files that don't fall neatly into either the kernel or the driver category.

Files named ``s*.c`` and ``s*.h`` are a flexible stream package, including the Level 2 PostScript "filters" supported by Ghostscript. See ``stream.h``, ``scommon.h``, and ``strimpl.h`` for all the details.


Device drivers
~~~~~~~~~~~~~~~~~~

The interface between the graphics library and device drivers is the only really well documented one in all of Ghostscript: see the :ref:`documentation on drivers<Drivers.html>`.

In addition to many real device and file format drivers listed in ``devs.mak`` and ``contrib.mak``, a number of drivers are used for internal purposes. You can search ``lib.mak`` for files named ``gdev*.c`` to find almost all of them.

Drivers are divided into "printer" drivers, which support banding, and non-printer drivers, which don't. The decision whether banding is required is made (by default on the basis of how much memory is available) in the procedure ``gdev_prn_alloc`` in ``gdevprn.c``: it implements this decision by filling the virtual procedure table for the printer device in one of two different ways.

A good simple "printer" (bandable) driver to read is ``gdevmiff.c``: it's less than 100 lines, of which much is boilerplate. There are no simple non-printer drivers that actually drive devices: probably the simplest non-printer driver for reading is ``gdevm8.c``, which implements 8-bit-deep devices that only store the bits in memory.


Platform-specific code
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

There are very few platform dependencies in Ghostscript. Ghostscript deals with them in three ways:

Files named ``*_.h`` substitute for the corresponding ``<*.h>`` file by adding conditionals that provide a uniform set of system interfaces on all platforms.

The file ``arch.h`` contains a set of mechanically-discovered platform properties like byte order, size of int, etc. These properties, not the names of specific platforms, are used to select between different algorithms or parameters at compile time.

Files named ``gp*.h`` define interfaces that are intended to be implemented differently on each platform, but whose specification is common to all platforms.

The platform-specific implementations of the ``gp*.h`` interfaces have names of the form ``gp_{platform}.c``, specifically (this list may be out of date):


**Platform-specific interfaces**


.. list-table::
   :header-rows: 1
   :widths: 50 50

   * - Routine
     - Platform
   * - ``gp_dosfb.c``
     - DOS
   * - ``gp_dosfs.c``
     - DOS and MS Windows
   * - ``gp_itbc.c``
     - DOS, Borland compilers
   * - ``gp_iwatc.c``
     - DOS, Watcom or Microsoft compiler
   * - ``gp_msdos.c``
     - DOS and MS Windows
   * - ``gp_ntfs.c``
     - MS Windows NT
   * - ``gp_os2.c``
     - OS/2
   * - ``gp_os9.c``
     - OS-9
   * - ``gp_unifs.c``
     - Unix, OS-9, and QNX
   * - ``gp_unix.c``
     - Unix and QNX
   * - ``gp_vms.c``
     - VMS
   * - ``gp_win32.c``
     - MS Windows NT

If you are going to extend Ghostscript to new machines or operating systems, check the ``*_.h`` files for ``ifdef`` on things other than ``DEBUG``. You should probably plan to make a new ``makefile`` and a ``new gp_XXX.c`` file.




Makefiles
---------------------------------------

This section is only for advanced developers who need to integrate Ghostscript into a larger program at build time.

.. note ::

   THIS SECTION IS INCOMPLETE. IT WILL BE IMPROVED IN A LATER REVISION.


The Ghostscript ``makefiles`` are meant to be organized according to the following two principles:

#. All the parameters that vary from platform to platform appear in the top-level ``makefile`` for a given platform. ("Platform" = OS + compiler + choice of interpreter vs. library).

#. All the rules and definitions that can meaningfully be shared among more than 1 platform appear in a ``makefile`` that is "included" by a ``makefile`` (normally the top-level ``makefile``) for those platforms.

Thus, for example:

- Rules and definitions shared by all builds are in ``gs.mak``.

- Rules and definitions specific to the library (on all platforms) are in ``lib.mak``. In principle this could be merged with ``gs.mak``, but we wanted to leave open the possibility that ``gs.mak`` might be useful with hypothetical interpreter-only products.

- Stuff specific to interpreters (on all platforms) is in ``int.mak``.
- Stuff specific to all Unix platforms should be in a single ``unix.mak`` file, but because make sometimes cares about the order of definitions, and because some of it is shared with DV/X, it got split between ``unix-aux.mak``, ``unix-end.mak``, ``unixhead.mak``, ``unixinst.mak``, and ``unixlink.mak``.

For MS-DOS and MS Windows builds, there should be:


- A ``makefile`` for all MS OS (DOS or Windows) builds, for all compilers and products.

   Perhaps a ``makefile`` for all MS-DOS builds, for all compilers and products, although since Watcom is the only such compiler we're likely to support this may be overkill.

- A ``makefile`` for all MS Windows builds, for all compilers and products.

- A ``makefile`` for all Watcom builds (DOS or Windows), for all products.

- A top-level ``makefile`` for the Watcom DOS interpreter product.

- A top-level ``makefile`` for the Watcom Windows interpreter product.

- A top-level ``makefile`` for the Watcom DOS library "product".

- A top-level ``makefile`` for the Watcom Windows library "product".

- A ``makefile`` for all Borland builds (DOS or Windows), for all products.


and so on.

















.. include:: footer.rst