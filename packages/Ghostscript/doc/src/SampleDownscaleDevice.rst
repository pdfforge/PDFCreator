.. Copyright (C) 2001-2022 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: Sample CMYK 32-bit Device that Supports Post Rendering Processing


.. include:: header.rst

.. _SampleDownscaleDevice.htm:


Sample CMYK 32-bit Device that Supports Post Rendering Processing
=====================================================================




This device is a basic CMYK 32-bit (8-bits per component) device that uses the downscaler features to provide post-rendering operations on the raster data produced by Ghostscript.

It is designed to allow a developer to add code to handle the resulting raster data and perform any desired formatting and transmission to some destination, for example DMA to a printer or sending over a network link.



Features
------------

The ``downscaler`` modules provide for the following optional post-rendering manipulation:

``-dDownScaleFactor=factor`` (small non-negative integer; default = 1)
   If this option set then the page is downscaled by the given factor on both axes. For example rendering with -r600 and then specifying ``-dDownScaleFactor=3`` will produce a 200dpi image.

   2 additional "special" ratios are available, 32 and 34. 32 provides a 3:2 downscale (so from 300 to 200 dpi, if the resolution is 300 dpi as with -r300). 34 produces a 3:4 upscale (so from 300 to 400 dpi, similarly).

``-sPostRenderProfile=path`` (path to an ICC profile)
   If this option set then the page will be color transformed using that profile after downscaling.
   This is useful when the file uses overprint to separately paint to some subset of the C, M, Y, and K colorants, but the final CMYK is to be color corrected for printing or display.


The ``ds32`` device can perform rudimentary automatic bitmap 'trapping' on the final rendered bitmap. This code is disabled by default; see the `Trapping patents`_ below as to why.

Trapping is a process whereby the output is adjusted to minimise the visual impact of offsets between each printed plane. Typically this involves slightly extending abutting regions that are rendered in different inks. The intent of this is to avoid the unsightly gaps that might be otherwise be revealed in the final printout if the different color plates do not exactly line up.

This trapping is controlled by 3 device parameters. Firstly the maximum X and Y offsets are specified using:

``-dTrapX=N``

and:

``-dTrapY=N``

(where N is a figure in pixels, before the downscaler is applied).


The final control is to inform the trapping process in what order inks should be processed, from darkest to lightest. For a typical CMYK device this order would be [ 3 1 0 2 ] (K darker than M darker than C darker than Y). This is the default.

To override these defaults, the ``TrapOrder`` parameter can be used. Since this parameter requires an array, it must be specified using a different method. For example, if Cyan is darker than Magenta:


.. code-block:: bash

   gs -sDEVICE=ds32 -dTrapX=2 -dTrapY=2 -c "<< /TrapOrder [ 3 0 1 2 ] >> setpagedevice" -f examples\colorcir.ps


Trapping patents
~~~~~~~~~~~~~~~~~~~

Trapping is an technology area encumbered by many patents. Until we can convince ourselves that our trapping code is not covered by any of these patents, the functionality is disabled by default.

It can be enabled by building with the ``ENABLE_TRAPPING`` define, but before you do so you should convince yourself that either:

- The trapping code is not covered by any existing patent.
- Any patents that do cover the code are invalid in your jurisdiction.
- That you have appropriate patent licenses for any patents that do apply.

You bear full responsibility for choosing to build with ``ENABLE_TRAPPING``.






Changes to devices/devs.mak
--------------------------------

The make files used to build Ghostscript use the file ``devices/devs.mak`` to describe what is needed for devices that can be built into the executable, such as source code files, header files, libraries and dependencies on other parts of Ghostscript.

The section below can be added to the end of the ``devices/devs.mak`` file:

.. code-block:: bash

   ### -------- Example 32-bit CMYK downscaled device --------------------- ###
   # NB: downscale_ is standard in the lib (LIB1s)
   $(DD)ds32.dev : $(DEVOBJ)gdevds32.$(OBJ) $(GLD)page.dev \
    $(GDEV) $(DEVS_MAK) $(MAKEDIRS)
           $(SETPDEV2) $(DD)ds32 $(DEVOBJ)gdevds32.$(OBJ)
           $(ADDMOD) $(DD)ds32 -include $(GLD)page

   $(DEVOBJ)gdevds32.$(OBJ) : $(DEVSRC)gdevds32.c $(gsicc_cache_h) $(gxdownscale_h) $(AK) \
     $(arch_h) $(gdevprn_h) $(stdio__h)  $(stdint__h) $(DEVS_MAK) $(MAKEDIRS)
           $(DEVCC) $(DEVO_)gdevds32.$(OBJ) $(C_) $(DEVSRC)gdevds32.c



This snippet is also in a commented block at the top of the source code file.




Building Ghostscript with the driver
-----------------------------------------

The make system also needs to be told to include the ``ds32`` device. This can either be added to the top level make file on one of the ``DEVICE_DEVS`` lines, for example, in ``psi/msvc.mak`` for the Windows build:

.. code-block:: bash

   DEVICE_DEVS21=$(DD)ds32.dev $(DD)spotcmyk.dev $(DD)devicen.dev $(DD)bmpsep1.dev $(DD)bmpsep8.dev $(DD)bmp16m.dev $(DD)bmp32b.dev $(DD)psdcmyk.dev $(DD)psdrgb.dev $(DD)cp50.dev $(DD)gprf.dev


Rather than editing a make file, there is a convenient macro that allows the extra device to be added to the build command line, ``DEVICE_DEVS_EXTRA``.

For example, on linux:

.. code-block:: bash

   make DEVICE_DEVS_EXTRA=obj/ds32.dev

or, on Windows:

.. code-block:: bash

   nmake -f psi/msvc32.mak DEVICE_DEVS_EXTRA=obj\ds32.dev


Source code
-------------

The source for this device driver is in: ``doc/gdevds32.c``





.. include:: footer.rst
