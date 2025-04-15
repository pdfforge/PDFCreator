.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: Information for Ghostscript Developers

.. include:: header.rst

.. _Develop.html:
.. _Ghostscript Developers:


Information for Ghostscript Developers
======================================


Introduction
------------------------------------
This document provides a wealth of information about Ghostscript's internals, primarily for developers actively working on Ghostscript.  It is primarily **descriptive**, documenting the way things are; the companion :ref:`C style guide<C-style.html>` is primarily **prescriptive**, documenting what developers should do when writing new code.


Architecture
------------------------------------



Design Goals
~~~~~~~~~~~~~~~
Ghostscript has the following high-level design goals (not listed in order
of importance):

Functionality
""""""""""""""""

   - Ability to interpret the current PostScript and PDF languages, as defined (and occasionally, in the case of conflict, as implemented) by Adobe.

   - Ability to convert PostScript to and from PDF, comparable to Adobe products.

   - Ability to produce output for a wide range of resolutions (from TV-resolution displays to imagesetters) and color models (black and white, multilevel gray, bilevel or multi-level RGB and CMYK, 6- or 8-color inkjet printers, spot color).

Performance
""""""""""""""""

   - Ability to render PostScript and PDF with commercial-quality performance (memory usage, speed, and output quality) on all platforms.

   - Specifically, ability to render PostScript effectively in embedded environments with constrained RAM, including the ability to put the code and supporting data in ROM.

Licensing
""""""""""""""""

   - Licensing that supports both the Open Source / Free software communities and a commercial licensing business.

   - Freedom from licensing restrictions or fees imposed by third parties.

Other
""""""""""""""""

   - Easy source portability to any platform (CPU, operating system, and development tools) that has an ANSI C compiler.

   - Support for writing new interpreters and new drivers with no change to any existing code; specifically, ability to support PCL 5e, PCL 5c, and PCL XL interpreters, and the ever-changing roster of inkjet printers.


These goals often conflict: part of Ghostscript's claim to quality is that the conflicts have been resolved well.



Design principles
------------------------

Part of what has kept Ghostscript healthy through many years of major code revisions and functional expansion is consistent and conscientious adherence to a set of design principles. We hope the following list captures the most important ones.

Non-preemption
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Ghostscript is designed to be used as a component. As such, it must share its environment with other components. Therefore, it must not require ownership of, or make decisions about, inherently shared resources. Specifically, it must not assume that it can "own" either the locus of control or the management of the address space.

Not owning control means that whenever Ghostscript passes control to its caller, it must do so in a way that doesn't constrain what the caller can do next. The caller must be able to call any other piece of software, wait for an external event, execute another task, etc., without having to worry about Ghostscript being in an unknown state. While this is easy to arrange in a multi-threaded environment (by running Ghostscript in a separate thread), multi-threading APIs are not well standardized at this time (December 2000), and may not be implemented efficiently, or at all, on some platforms. Therefore, Ghostscript must choose between only two options for interacting with its caller: to return, preserving its own state in data structures, or to call back through a caller-supplied procedure. Calling back constrains the client program unacceptably: the callback procedure only has the options of either returning, or aborting Ghostscript. In particular, if it wants (for whatever reason) to multi-task Ghostscript with another program, it cannot do so in general, especially if the other program also uses callback rather than suspension. Therefore, Ghostscript tries extremely hard to return, rather than calling back, for all caller interaction. In particular:

- For callers that want to pass input to Ghostscript piece by piece, Ghostscript returns with an ``gs_error_NeedInput`` code rather than using a callback. This allows the caller complete flexibility in its control structure for managing the source of input. (It might, for example, be generating the input dynamically).

- In the future, the same arrangement should be used for input from ``stdin`` and output to ``stdout`` and ``stderr``.

- Likewise, scheduling of Ghostscript's own threads (contexts), currently done with a callback, should be done with suspension. The Display Ghostscript project (GNU DGS) is working on this.

The one area where suspension is not feasible with Ghostscript's current architecture is device output. Device drivers are called from deep within the graphics library. (If Ghostscript were being redesigned from scratch, we might try to do this with suspension as well, or at least optional suspension.)


Not owning management of the address space means that even though Ghostscript supports garbage collection for its own data, it must not do any of the things that garbage collection schemes for C often require: it must not replace 'malloc' and 'free', must not require its clients to use its own allocator, must not rely on manipulating the read/write status of memory pages, must not require special compiler or run-time support (e.g., APIs for scanning the C stack), must not depend on the availability of multi-threading, and must not take possession of one of a limited number of timer interrupts. However, in order not to constrain its own code unduly, it must also not require using special macros or calls to enter or leave procedures or assign pointers, and must not constrain the variety of C data structures any more than absolutely necessary. It achieves all of these goals, at the expense of some complexity, some performance cost (mostly for garbage collection), and some extra manual work required for each structure type allocated by its allocator. The details appear in the `Memory management`_ section below.

Multi-instantiability
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

From many years of experience with the benefits of object-oriented design, we have learned that when the word "the" appears in a software design -- "the" process scheduler, "the" memory manager, "the" output device, "the" interpreter, "the" stack -- it often flags an area in which the software will have difficulty adapting to future needs. For this reason, Ghostscript attempts to make every internal structure capable of existing in multiple instances. For example, Ghostscript's memory manager is not a one-of-a-kind entity with global state and procedures: it is (or rather they are, since Ghostscript has multiple memory managers, some of which have multiple instances) objects with their own state and (virtual) procedures. Ghostscript's PostScript interpreter has no writable non-local data (necessary, but not sufficient, to allow multiple instances), and in the future will be extended to be completely reentrant and instantiable. The device driver API is designed to make this easy for drivers as well. The graphics library is currently not completely reentrant or instantiable: we hope this will occur in the future.

Late configuration binding
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Ghostscript is designed to make configuration choices as late as possible, subject to simplicity and performance considerations. The major binding times for such choices are compilation, linking, startup, and dynamic.

- Compilation binds only CPU and compiler characteristics (including data type size, presence of floating point hardware, and data alignment), and whether the code will be used for production, debugging, or profiling.
- Linking binds the choice of what features and device drivers will be included in the executable. (Work is underway to make the choice of drivers dynamic).
- Startup binds essentially nothing. Almost every option and parameter that can appear on the command line can also be changed dynamically.
- The selection of output device, all parameters associated with the device, the selection of debugging printout and self-checking (in debugging configurations), the macro-allocation of memory, and almost all other operational parameters are dynamic.


In addition, a number of major implementation decisions are made dynamically depending on the availability of resources. For example, Ghostscript chooses between banded and non-banded rendering depending on memory availability.


Large-scale structure
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

At the largest design scale, Ghostscript consists of 4 layers. Layer N is allowed to use the facilities of all layers M <= N.

#. The bottom layer is called the substrate. It includes facilities like memory management, streams, fixed-point arithmetic, and low-level interfaces to the operating system. The substrate is written in C, with a little C++ and/or assembler code for some platforms.

#. The layer above the substrate is the graphics layer. It consists of two separate sub-parts. The graphics layer is written in C.

   - The graphics library manages graphics state information for, and decomposes and renders 2-D images described using, a graphics model that is approximately the union of those of PostScript, PDF, and PCL 5e/5c/XL.

   - The device drivers are called by the graphics library to produce actual output. The graphics library, and all higher layers, call device driver procedures only through virtual functions.

#. The principal clients of the graphics layer are language interpreters. Ghostscript as distributed includes the PostScript interpreter; there are also interpreters for PCL 5e, PCL 5c, and PCL XL, which are not currently freely redistributable and are not included in the standard Ghostscript package. The PostScript interpreter is written partly in C and partly in PostScript.

#. The PDF interpreter is actually a client of the PostScript interpreter: it is written entirely in PostScript.

The most important interface in Ghostscript is the API between the graphics library and the device drivers: new printers (and, to a lesser extent, window systems, displays, plotters, film recorders, and graphics file formats) come on the scene frequently, and it must be possible to produce output for them with a minimum of effort and distruption. This API is the only one that is extensively documented (see :ref:`Drivers<Drivers.html>`) and kept stringently backward-compatible through successive releases.


Object-oriented constructs
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Ghostscript makes heavy use of object-oriented constructs, including analogues of classes, instances, subclassing, and class-associated procedures. Since Ghostscript is written in C, not C++, implementing these constructs requires following coding conventions. The :ref:`"Objects"<CStyle_Objects>` section of the :ref:`C style guide<C-style.html>` explains these.

The memory manager API provides run-time type information about each class, but this information does not include anything about subclassing. See under :ref:`Structure descriptors<Develop_Structure_Descriptors>` below.






File roadmap
----------------------


This section of the document provides a roadmap to all of the Ghostscript source files.

Substrate
~~~~~~~~~~~~~~

Runtime Context
""""""""""""""""""""

The ``libctx`` provides pointers to memory, ``stdio``, and various other runtime portablility services:
   base/gslibctx.h, base/gslibctx.c

Memory manager
""""""""""""""""""""

See `Memory Management`_

Streams
""""""""""""""""""""

Framework, file and string streams:
   base/gsdsrc.c, base/gsdsrc.h, base/scommon.h, base/strmio.c, base/strmio.h, base/sfxboth.c, base/sfxfd.c, base/sfxstdio.c, base/sfxcommon.c, base/stream.h, base/stream.c, base/strimpl.h.


Standard filters:

   CCITTFax:
      base/scf.h, base/scfd.c, base/scfdgen.c, base/scfdtab.c, base/scfe.c, base/scfetab.c, base/scfparam.c, base/scfx.h.

   DCT (JPEG):
      base/gsjconf.h, base/gsjmorec.h, base/sdcparam.c, base/sdcparam.h, base/sdct.h, base/sdctc.c, base/sdctd.c, base/sdcte.c, base/sddparam.c, base/sdeparam.c, base/sjpeg.h, base/sjpegc.c, base/sjpegd.c, base/sjpege.c.

   JBIG2:
      base/sjbig2.h, base/sjbig2.c

   JPX (JPEG 2000):
      base/sjpx_openjpeg.h, base/sjpx_openjpeg.c

   Other compression/decompression:
      base/slzwc.c, base/slzwd.c, base/slzwe.c, base/slzwx.h, base/srld.c, base/srle.c, base/srlx.h.

   Other:
      base/sa85d.c, base/sa85d.h, base/sa85x.h, psi/sfilter1.c, base/sfilter2.c, base/sstring.c, base/sstring.h.



Non-standard filters used to implement standard filters:
   base/seexec.c, base/sfilter.h, base/shc.c, base/shc.h, base/spdiff.c, base/spdiffx.h, base/spngp.c, base/spngpx.h, base/szlibc.c, base/szlibd.c, base/szlibe.c, base/szlibx.h, base/szlibxx.h.

Non-standard filters:
   base/sbcp.c, base/sbcp.h, base/sbtx.h, base/smd5.c, base/smd5.h, base/saes.c, base/saes.h, base/sarc4.c, base/sarc4.h,

Internal filters:
   base/siinterp.c, base/siinterp.h, base/siscale.c, base/siscale.h, base/sidscale.c, base/sidscale.h, base/sisparam.h.

Higher-level stream support:
   base/spprint.c, base/spprint.h, base/spsdf.c, base/spsdf.h, base/srdline.h.


Platform-specific code
""""""""""""""""""""""""""""""""""""""""

See `Cross-platform APIs`_.

Miscellaneous
""""""""""""""""""""""""""""""""""""""""

Library top level:
   base/gsinit.c, base/gslib.h.

Configuration-related:
   base/gconf.c, base/gconf.h, base/gscdef.c, base/gscdefs.h, base/gsromfs0.c.

Arithmetic:
   base/gxarith.h, base/gxdda.h, base/gxfarith.h, base/gxfixed.h, base/gxfrac.h.

Operating system interface:
   base/gserrors.h, base/gsexit.h, base/gxstdio.h, base/gxsync.c, base/gxsync.h.

Other:
   base/gsargs.c, base/gsargs.h, base/gserrors.h, base/gsnotify.c, base/gsnotify.h, base/gsrect.h, base/gstypes.h, base/gsuid.h, base/gsutil.h, base/gsutil.c, base/gx.h, base/gsmd5.c, base/gsmd5.h, base/aes.c, base/aes.h.


Graphics library
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


.. _Graphics library_Support:

Support
""""""""""

Bitmap processing:
   base/gsbitcom.c, base/gsbitmap.h, base/gsbitops.c, base/gsbitops.h, base/gsbittab.c, base/gsbittab.h, base/gsflip.c, base/gsflip.h, base/gxbitmap.h, base/gxbitops.h, base/gxsample.c, base/gxsample.h. base/gxsamplp.h.

Functions:
   base/gsfunc.c, base/gsfunc.h, base/gsfunc0.c, base/gsfunc0.h, base/gsfunc3.c, base/gsfunc3.h, base/gsfunc4.c, base/gsfunc4.h, base/gxfunc.h.

Parameter lists:
   base/gscparam.c, base/gsparam.c, base/gsparam.h, base/gsparam2.c (not used), base/gsparams.c, base/gsparams.h, base/gsparamx.c, base/gsparamx.h.

I/O-related:
   base/gdevpipe.c, base/gsfname.c, base/gsfname.h, base/gsio.h, base/gsiodev.c, base/gsiodevs.c, base/gsiodisk.c, base/gsiorom.c. base/gsiorom.h. base/gxiodev.h.


Paths
""""""""""

Coordinate transformation:
   base/gscoord.c, base/gscoord.h, base/gsmatrix.c, base/gsmatrix.h, base/gxcoord.h, base/gxmatrix.h.

Path building:
   base/gsdps1.c, base/gspath.c, base/gspath.h, base/gspath1.c, base/gspath2.h, base/gxpath.c, base/gxpath.h, base/gxpath2.c, base/gxpcopy.c, base/gxpdash.c, base/gxpflat.c, base/gzpath.h.

Path rendering:
   base/gdevddrw.c, base/gdevddrw.h, base/gxdtfill.h, base/gsdps1.c, base/gspaint.c, base/gspaint.h, base/gspenum.h, base/gxfill.c, base/gxfill.h, base/gxfillsl.h, base/gxfilltr.h, base/gxfillts.h, base/gximask.c, base/gximask.h, base/gxfdrop.c, base/gxfdrop.h, base/gxpaint.c, base/gxpaint.h, base/gxstroke.c, base/gzspotan.c, base/gzspotan.h.

Clipping:
   See under `Clipping`_ below.

Text
""""""""""

Fonts, generic:
   base/gsfont.c, base/gsfont.h, devices/gxfcopy.c, devices/gxfcopy.h, base/gxfont.h.

Fonts, specific FontTypes:
   base/gsfcid.c, base/gsfcid.c, base/gsfcmap.c, base/gsfcmap1.c, base/gsfcmap.h, base/gsfont0.c, base/gsfont0c.c, base/gxcid.h, base/gxfcid.h, base/gxfcmap.h, base/gxfcmap1.h, base/gxfont0.h, base/gxfont0c.h, base/gxfont1.h, base/gxfont42.h, base/gxftype.h, base/gxttf.h.

Character rendering + font cache, generic:
   base/gsccode.h, base/gschar.c, base/gschar.h, base/gscpm.h, base/gsgdata.c, base/gsgdata.h, base/gsgcache.c, base/gsgcache.h, base/gstext.c, base/gstext.h, base/gxbcache.c, base/gxbcache.h, base/gxccache.c, base/gxccman.c, base/gxchar.c, base/gxchar.h, base/gxfcache.h, base/gxtext.h.

Character rendering, specific FontTypes:
   base/gschar0.c, base/gscrypt1.c, base/gscrypt1.h, base/gstype1.c, base/gstype1.h, base/gstype2.c, base/gstype42.c, base/gxchrout.c, base/gxchrout.h, base/gxhintn.h, base/gxhintn.c, base/gxhintn1.c, base/gxtype1.c, base/gxtype1.h.


Images
""""""""""

Buffered API (mostly for PostScript interpreter):
   base/gsimage.c, base/gsimage.h.

Generic support:
   base/gsiparam.h, base/gxiclass.h, base/gximage.c, base/gximage.h, base/gxiparam.h.

Type 1 and 4 images:

   Setup:
      base/gsiparm4.h, base/gximage1.c, base/gximage4.c.

   Rendering:
      base/gxi12bit.c, base/gxi16bit.c, base/gxicolor.c, base/gxidata.c, base/gxifast.c, base/gximono.c, base/gxino12b.c, base/gxino16b.c, base/gxipixel.c, base/gxiscale.c.

Type 2 images (Display PostScript):
   base/gsiparm2.h, base/gximage2.c.

Type 3 images:
   base/gsipar3x.h, base/gsiparm3.h, base/gximag3x.c, base/gximag3x.h, base/gximage3.c, base/gximage3.h.

Other:
   base/gsimpath.c, base/simscale.c, base/simscale.h.

Paint
""""""""""

Ghostscript uses 4 internal representations of color. We list them here in the order in which they occur in the rendering pipeline.

#. Clients of the graphics library normally specify colors using the client color structure (``gs_client_color``, defined in psi/gs.color.h), consisting of one or more numeric values and/or a pointer to a Pattern instance. This corresponds directly to the values that would be passed to the PostScript setcolor operator: one or more (floating-point) numeric components and/or a Pattern. Client colors are interpreted relative to a color space (``gs_color_space``, defined in base/gscspace.h and base/gxcspace.h, with specific color spaces defined in other files). Client colors do not explicitly reference the color space in which they are are interpreted: setcolor uses the color space in the graphics state, while images and shadings explicitly specify the color space to be used.

#. For ordinary non-Pattern colors, the first step in color rendering reduces a client color to a concrete color -- a set of values in a color space that corresponds to the device's color model (except for possible conversions between DeviceGray, DeviceRGB, and DeviceCMYK), together with an identification of the associated color space. (The confusion here between color spaces and color models will have to be cleaned up when we implement native Separation/DeviceN colors.) Concrete colors are like the numeric values in a client color, except that they are represented by arrays of ``frac`` values (defined in base/gxfrac.h) rather than floats. The procedure for this step is the virtual ``concretize_color`` and ``concrete_space`` procedures in the (original) color space. This step reduces Indexed colors, CIEBased colors, and Separation and DeviceN colors that use the alternate space.

#. The final step requires mapping a concrete color to the device's color model, done by procedures in base/gxcmap.c. These procedures combine the following three conceptual sub-steps:

   - A possible mapping between Device color spaces, possibly involving black generation and undercolor removal. The non-trivial cases are implemented in base/gxdcconv.c.

   - Application of the transfer function(s) (done in-line).

   - Halftoning if necessary: see below.

   The result is called (inappropriately) a device color (``gx_device_color``, defined in psi/gs.color.h and base/gxdcolor.h). For ordinary non-Pattern colors, a device color is either a pure color, or a halftone. The device and color model associated with a device color are implicit. The procedure for this step is the virtual ``remap_concrete_color`` procedure in the color space.

#. The pure colors that underlie a device color are opaque pixel values defined by the device (misnamed ``gx_color_index``, defined in base/gscindex.h). The device with which they are associated is implicit. Although the format and interpretation of a pixel value are known only to the device, the device's color model and color representation capabilities are public, defined by a ``gx_color_info`` structure stored in the device (defined in base/gxdevcli.h). Virtual procedures of the device driver map between pixel values and RGB or CMYK. (This area is untidy and will need to be cleaned up when we implement native Separation/DeviceN colors).


Steps 2 and 3 are normally combined into a single step for efficiency, as the ``remap_color`` virtual procedure in a color space.

Using a device color to actually paint pixels requires a further step called color loading, implemented by the load virtual procedure in the device color. This does nothing for pure colors, but loads the caches for halftones and Patterns.

All of the above steps -- concretizing, mapping to a device color, and color loading -- are done as late as possible, normally not until the color is actually needed for painting.

All painting operations (fill, stroke, imagemask/show) eventually call a virtual procedure in the device color, either ``fill_rectangle`` or ``fill_mask`` to actually paint pixels. For rectangle fills, pure colors call the device's fill_rectangle procedure; halftones and tiled Patterns call the device's ``strip_tile_rectangle``; shaded Patterns, and painting operations that involve a RasterOp, do something more complicated.

Color specification:
   base/gsdcolor.h, base/gscolor.c, base/gscolor.h, base/gscolor1.c, base/gscolor1.h, base/gscolor2.c, base/gscolor2.h, base/gscolor3.c, base/gscolor3.h, base/gshsb.c, base/gshsb.h, base/gxcolor2.h, base/gxcvalue.h.

Color spaces:
   base/gscdevn.c, base/gscdevn.h, base/gscie.c, base/gscie.h, base/gscpixel.c, base/gscpixel.h, base/gscscie.c, base/gscsepr.c, base/gscsepr.h, base/gscspace.c, base/gscspace.h, base/gscssub.c, base/gscssub.h, base/gxcdevn.h, base/gxcie.h, base/gxcspace.h.

Color mapping:
   base/gsciemap.c, base/gscindex.h, base/gscrd.c, base/gscrd.h, base/gscrdp.c, base/gscrdp.h, base/gscsel.h, base/gxcindex.h, base/gxcmap.c, base/gxcmap.h, base/gxctable.c, base/gxctable.h, base/gxdcconv.c, base/gxdcconv.h, base/gxdcolor.c, base/gxdcolor.h, base/gxdevndi.c, base/gxdevndi.h, base/gxdither.h, base/gxfmap.h, base/gxlum.h, base/gxtmap.h.


   ICC profiles are in some ways a special case of color mapping, but are not standard in PostScript.

   base/gsicc.c, base/gsicc.h,

   The following files provide a callback mechanism to allow a client program to specify a special case alternate tint transforms for Separation and DeviceN color spaces. Among other uses this can be used to provide special handling for PANTONE colors.

   base/gsnamecl.c, base/gsnamecl.h, base/gsncdummy.c, base/gsncdummy.h, psi/zncdummy.c


Ghostscript represents halftones internally by "whitening orders" -- essentially, arrays of arrays of bit coordinates within a halftone cell, specifying which bits are inverted to get from halftone level K to level K+1. The code does support all of the PostScript halftone types, but they are all ultimately reduced to whitening orders.

Threshold arrays, the more conventional representation of halftones, can be mapped to whitening orders straightforwardly; however, whitening orders can represent non-monotonic halftones (halftones where the bits turned on for level K+1 don't necessarily include all the bits turned on for level K), while threshold arrays cannot. On the other hand, threshold arrays allow rapid conversion of images (using a threshold comparison for each pixel) with no additional space, while whitening orders do not: they require storing the rendered halftone cell for each possible level as a bitmap.

Ghostscript uses two distinct types of rendered halftones -- that is, the bitmap(s) that represent a particular level.

- Binary halftones. The rendered halftone is a single bit plane; each bit selects one of two pure colors. These are fast but limited: they are used for monochrome output devices, or for color devices in those cases where only two distinct colors are involved in a halftone (e.g., a pure cyan shade on a CMYK device). The device color for a binary halftone stores a pointer to the halftone bitmap, and the two pure colors.

- Multi-plane halftones. Internally, each plane is rendered individually. Since there isn't enough room to store all 2^N pure colors, multi-plane halftones only store the scaled values for the individual components; the halftone renderer maps these to the pure colors on the fly, then combines the planes to assemble an N-bit index into the list of colors for each pixel, and stores the color into the fully rendered halftone.


The halftone level for rendering a color is computed in base/gxdevndi.c; the actual halftone mask or tile is computed either in base/gxcht.c (for multi-plane halftones), or in base/gxht.c and base/gxhtbit.c (for binary halftones).

Halftoning:
   base/gsht.c, base/gsht.h, base/gsht1.c, base/gsht1.h, base/gshtscr.c, base/gshtx.c, base/gshtx.h, base/gxcht.c, base/gxdht.h, base/gxdhtres.h, base/gxht.c, base/gxht.h, base/gxhtbit.c, base/gxhttile.h, base/gxhttype.h, base/gzht.h.


Pattern colors (tiled patterns and shadings) each use a slightly different approach from solid colors.

The device color for a tiled (PatternType 1) pattern contains a pointer to a pattern instance, plus (for uncolored patterns) the device color to be masked. The pattern instance includes a procedure that actually paints the pattern if the pattern is not in the cache. For the PostScript interpreter, this procedure returns an ``gs_error_RemapColor`` exception code: this eventually causes the interpreter to run the pattern's PaintProc, loading the rendering into the cache, and then re-execute the original drawing operator.

Patterns:
   base/gs.color.c, base/gs.color.h, base/gsptype1.c, base/gsptype1.h, base/gxp1fill.c, base/gxp1impl.h, base/gxpcache.h, base/gxpcmap.c, base/gxpcolor.h.


The device color for a shading (PatternType 2) pattern also contains a pointer to a pattern instance. Shadings are not cached: painting with a shading runs the shading algorithm every time.

Shading:
   base/gsptype2.c, base/gsptype2.h, base/gsshade.c, base/gsshade.h, base/gxshade.c, base/gxshade.h, base/gxshade1.c, base/gxshade4.c, base/gxshade4.h, base/gxshade6.c, base/gscicach.h, base/gscicach.c.


In addition to the PostScript graphics model, Ghostscript supports RasterOp, a weak form of alpha channel, and eventually the full PDF 1.4 transparency model. The implemention of these facilities is quite slipshod and scattered: only RasterOp is really implemented fully. There is a general compositing architecture, but it is hardly used at all, and in particular is not used for RasterOp. It is used for implementation of the general support for overprint and overprint mode.

Compositing architecture:
   base/gscompt.h, base/gxcomp.h.

RasterOp:
   base/gdevdrop.c, base/gdevrops.c, base/gsrop.c, base/gsrop.h, base/gsropt.h, base/gsroptab.c, base/gxdevrop.h.

Alpha channel and compositing:
   base/gsalpha.c, base/gsalpha.h, base/gsdpnext.h, base/gxalpha.h.

Advanced transparency:
   base/gstparam.h, base/gstrans.c, base/gstrans.h, base/gxblend.c, base/gxblend.h, base/gdevp14.c, base/gdevp14.h.

Overprint and Overprint mode:
   base/gsovrc.c, base/gsovrc.h, base/gxoprect.c, base/gxoprect.h. There is support for both overprint and overprint mode. There is a general compositor based implementation of these features for all devices. In addition, the memory devices implement a higher speed set of special fill routines to improve performance for printer based devices.


Clipping
""""""""""

The Ghostscript graphics library implements clipping by inserting a clipping device in the device pipeline. The clipping device modifies all drawing operations to confine them to the clipping region.

The library supports three different kinds of clipping:

- Region/path clipping
   This corresponds to the PostScript concept of a clipping path. The clipping region is specified either by a list of rectangles (subject to the constraints documented in base/gxcpath.h), or by a path that is converted to such a list of rectangles.

- Stationary mask clipping
   This corresponds to the mask operand of a PostScript ImageType 3 image. The clipping region is specified by a bitmap and an (X,Y) offset in the coordinate space.

- Tiled mask clipping
   This corresponds to the region painted by a PostScript Pattern, for the case where the Pattern does not completely cover its bounding box but the combined transformation matrix has no skew or non-orthogonal rotation (i.e., XStep and YStep map respectively to (X,0) and (0,Y) or vice versa). The clipping region is specified by a bitmap and an (X,Y) offset in the coordinate space, and is replicated indefinitely in both X and Y.


Note that simply scan-converting a clipping path in the usual way does not produce a succession of rectangles that can simply be stored as the list for region-based clipping: in general, the rectangles do not satisfy the constraint for rectangle lists specified in base/gxcpath.h, since they may overlap in X, Y, or both. A non-trivial "clipping list accumulator" device is needed to produce a rectangle list that does satisfy the constraint.

Clipping support:
   base/gxclip.c, base/gxclip.h.

Region/path clipping:
   base/gxcpath.c, base/gxcpath.h, base/gzcpath.h.

Clipping list accumulator:
   base/gxacpath.c, base/gzacpath.h.

Mask clipping support:
   base/gxmclip.c, base/gxmclip.h.

Stationary mask clipping:
   base/gxclipm.c, base/gxclipm.h.

Tiled mask clipping:
   base/gxclip2.c, base/gxclip2.h.


Other graphics
""""""""""""""""""""

Miscellaneous graphics state:
   base/gsclipsr.c, base/gsclipsr.h, base/gsdps.c, base/gsdps.h, base/gsdps1.c, base/gsistate.c, base/gsline.c, base/gsline.h, base/gslparam.h, base/gsstate.c, base/gsstate.h, base/gstrap.c, base/gstrap.h, base/gxclipsr.h, base/gxistate.h, base/gxline.h, base/gxstate.h, base/gzline.h, base/gzstate.h.


Font API support
""""""""""""""""""""

UFST bridge:
   base/gxfapiu.c, base/gxfapiu.h.


Driver support
""""""""""""""""""""

Generic driver support:
   base/gdevdcrd.c, base/gdevdcrd.h, base/gdevdsha.c, base/gdevemap.c, base/gsdevice.c, base/gsdevice.h, base/gsdparam.c, base/gsxfont.h, base/gxdevbuf.h, base/gxdevcli.h, base/gxdevice.h, base/gxrplane.h, base/gxxfont.h.

Accessing rendered bits:
   base/gdevdbit.c, base/gdevdgbr.c, base/gxbitfmt.h, base/gxgetbit.h.

"Printer" driver support:
   devices/gdevmeds.c, devices/gdevmeds.h, base/gdevppla.c, base/gdevppla.h, base/gdevprn.c, base/gdevprn.h, base/gdevprna.c, base/gdevprna.h, base/gxband.h, base/gxpageq.c, base/gxpageq.h.

High-level device support:
   base/gdevvec.c, base/gdevvec.h, base/gxhldevc.c, base/gxhldevc.h.

Banding:
   base/gxclbits.c, base/gxcldev.h, base/gxclfile.c, base/gxclimag.c, base/gxclio.h, base/gxclist.c, base/gxclist.h, base/gxcllzw.c, base/gxclmem.c, base/gxclmem.h, base/gxclpage.c, base/gxclpage.h, base/gxclpath.c, base/gxclpath.h, base/gxclrast.c, base/gxclread.c, base/gxclrect.c, base/gxclthrd.c, base/gxclthrd.h, base/gxclutil.c, base/gxclzlib.c, base/gxdhtserial.c, base/gxdhtserial.h, base/gsserial.c, base/gsserial.h.


Visual Trace
""""""""""""""""""""

Visual Trace support :
   base/vdtrace.h, base/vdtrace.c.


See :ref:`Visual Trace instructions<Lib_VisualTrace>` for extensive documentation.


Device drivers
~~~~~~~~~~~~~~~~~~~~

See :ref:`Drivers<Drivers.html>` for extensive documentation on the interface between the core code and drivers.

The driver API includes high-level (path / image / text), mid-level (polygon), and low-level (rectangle / raster) operations. Most devices implement only the low-level operations, and let generic code break down the high-level operations. However, some devices produce high-level output, and therefore must implement the high-level operations.

Internal devices
""""""""""""""""""""

There are a number of "devices" that serve internal purposes. Some of these are meant to be real rendering targets; others are intended for use in device pipelines. The rendering targets are:

Memory devices, depth-independent:
   base/gdevmem.c, base/gdevmem.h, base/gdevmpla.c, base/gdevmpla.h, base/gdevmrop.h, base/gsdevmem.c, base/gxdevmem.h.

Memory devices, specific depths:
   base/gdevm1.c, base/gdevm2.c, base/gdevm4.c, base/gdevm8.c, base/gdevm16.c, base/gdevm24.c, base/gdevm32.c, base/gdevm40.c, base/gdevm48.c, base/gdevm56.c, base/gdevm64.c, base/gdevmr1.c, base/gdevmr2n.c, base/gdevmr8n.c.

Alpha-related devices:
   base/gdevabuf.c.

Other devices:
   base/gdevdflt.c, base/gdevhit.c, base/gdevmrun.c, base/gdevmrun.h, base/gdevplnx.c, base/gdevplnx.h.


The forwarding devices meant for use in pipelines are:

The bounding box device:
   base/gdevbbox.h, base/gdevbbox.c.

Clipping devices:
   See under Clipping_ above.

Other devices:
   base/gdevnfwd.c.


PostScript and PDF writers
"""""""""""""""""""""""""""""""""""""

Because PostScript and PDF have the same graphics model, lexical syntax, and stack-based execution model, the drivers that produce PostScript and PDF output share a significant amount of support code. In the future, the PostScript output driver should be replaced with a slightly modified version of the PDF driver, since the latter is far more sophisticated (in particular, it has extensive facilities for image compression and for handling text and fonts).

The PDF code for handling text and fonts is complex and fragile. A major rewrite in June 2002 was intended to make it more robust and somewhat easier to understand, but also increased its size by about 40%, contrary to the expectation that it would shrink. Currently both sets of code are in the code base, with compatible APIs, selected by a line in ``devices/devs.mak``.


Shared support
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
Writing fonts:
   devices/vector/gdevpsf.h, devices/vector/gdevpsf1.c, devices/vector/gdevpsf2.c, devices/vector/gdevpsfm.c, devices/vector/gdevpsft.c, devices/vector/gdevpsfu.c, devices/vector/gdevpsfx.c, base/gscedata.c, base/gscedata.h, base/gscencs.c, base/gscencs.h.

Other:
   devices/vector/gdevpsdf.h, devices/vector/gdevpsdi.c, devices/vector/gdevpsdp.c, devices/vector/gdevpsds.c, devices/vector/gdevpsds.h, devices/vector/gdevpsdu.c.

Encapsulated PostScript output driver (epswrite):
   devices/vector/gdevpsu.c, devices/vector/gdevpsu.h.

PDF output driver (pdfwrite)
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

Substrate:
   devices/vector/gdevpdfo.c, devices/vector/gdevpdfo.h, devices/vector/gdevpdfr.c, devices/vector/gdevpdfu.c.

Old text and fonts:
   devices/vector/gdevpdfe.c, devices/vector/gdevpdft.c.

New text and fonts:
   devices/vector/gdevpdt.c, devices/vector/gdevpdt.h, devices/vector/gdevpdtb.c, devices/vector/gdevpdtb.h, devices/vector/gdevpdtc.c, devices/vector/gdevpdtd.c, devices/vector/gdevpdtd.h, devices/vector/gdevpdte.c, devices/vector/gdevpdtf.c, devices/vector/gdevpdtf.h, devices/vector/gdevpdti.c, devices/vector/gdevpdti.h, devices/vector/gdevpdts.c, devices/vector/gdevpdts.h, devices/vector/gdevpdtt.c, devices/vector/gdevpdtt.h, devices/vector/gdevpdtv.c, devices/vector/gdevpdtv.h, devices/vector/gdevpdtw.c, devices/vector/gdevpdtw.h, devices/vector/gdevpdtx.h. base/ConvertUTF.h, base/ConvertUTF.c,

Graphics:
   devices/vector/gdevpdfc.c, devices/vector/gdevpdfc.h, devices/vector/gdevpdfd.c, devices/vector/gdevpdfg.c, devices/vector/gdevpdfg.h, devices/vector/gdevpdfk.c, devices/vector/gdevpdft.c. devices/vector/gdevpdfv.c.

Images:
   devices/vector/gdevpdfb.c, devices/vector/gdevpdfi.c, devices/vector/gdevpdfj.c.

Other:
   devices/vector/gdevpdf.c, devices/vector/gdevpdfm.c, devices/vector/gdevpdfp.c, devices/vector/gdevpdfx.h. devices/vector/gdevpdfb.h.

Other high-level devices
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

PCL XL output device (pxlmono, pxlcolor):
   devices/vector/gdevpx.c, base/gdevpxat.h, base/gdevpxen.h, base/gdevpxop.h, devices/gdevpxut.c, devices/gdevpxut.h.

Text extraction:
   devices/vector/gdevtxtw.c.

Other:
   devices/gdevtrac.c.

Other maintained drivers
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

The standard Ghostscript distribution includes a collection of drivers, mostly written by Aladdin Enterprises, that are "maintained" in the same sense as the Ghostscript core code.

Display drivers:
   devices/gdev8bcm.c, devices/gdev8bcm.h, devices/gdevevga.c, devices/gdevl256.c, base/gdevpccm.c, base/gdevpccm.h, devices/gdevpcfb.c, devices/gdevpcfb.h, devices/gdevs3ga.c, devices/gdevsco.c, devices/gdevsvga.c, devices/gdevsvga.h, devices/gdevvglb.c.

Window system drivers
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

X Windows:
   devices/gdevx.c, devices/gdevx.h, devices/gdevxalt.c, devices/gdevxcmp.c, devices/gdevxcmp.h, devices/gdevxini.c, devices/gdevxres.c.

Microsoft Windows:
   devices/gdevmswn.c, devices/gdevmswn.h, devices/gdevmsxf.c, devices/gdevwddb.c, devices/gdevwdib.c.

OS/2 Presentation Manager:
   devices/gdevpm.h, base/gspmdrv.c, base/gspmdrv.h.



Raster file output drivers
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

Fax and TIFF:
   devices/gdevfax.c, devices/gdevfax.h, devices/gdevtfax.c, devices/gdevtfax.h, devices/gdevtifs.c, devices/gdevtifs.h, devices/gdevtfnx.c. devices/gdevtsep.c.

Example DeviceN devices:
   base/gdevdevn.c, base/gdevdevn.h, devices/gdevxcf.c, devices/gdevpsd.c, devices/gdevperm.c.

Other raster file formats:
   devices/gdevbit.c, devices/gdevbmp.c, devices/gdevbmp.h, devices/gdevbmpa.c, devices/gdevbmpc.c, devices/gdevjpeg.c, devices/gdevmiff.c, devices/gdevp2up.c, devices/gdevpcx.c, devices/gdevpbm.c, devices/gdevpng.c, devices/gdevpsim.c.


Printer drivers
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

Operating system printer services:
   devices/gdevos2p.c, devices/gdevwpr2.c, devices/gdevwprn.c.

H-P monochrome printers:
   devices/gdevdljm.c, devices/gdevdljm.h, devices/gdevdjet.c, devices/gdevlj56.c.

Other printers:
   devices/gdevatx.c.


Contributed drivers
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

This list is likely to be incomplete and inaccurate: see devices/contrib.mak and contrib/contrib.mak.

Display and window system drivers:
   devices/gdev3b1.c, devices/gdevherc.c, devices/gdevpe.c, devices/gdevsnfb.c, devices/gdevsun.c.

Raster file output drivers:
   devices/gdevcfax.c, devices/gdevcif.c, devices/gdevdfax.c, devices/gdevifno.c, devices/gdevmgr.c, devices/gdevmgr.h, devices/gdevsgi.c, devices/gdevsgi.h, devices/gdevsunr.c, devices/gdevjbig2.c, devices/gdevjpx.c.

Printer drivers:
   lib/bj8.rpd, lib/cbjc600.ppd, lib/cbjc800.ppd, devices/gdev3852.c, devices/gdev4081.c, devices/gdev4693.c, devices/gdev8510.c, devices/gdevadmp.c, devices/gdevbj10.c, devices/gdevbjc.h, devices/gdevbjcl.c, devices/gdevbjcl.h, devices/gdevccr.c, devices/gdevcdj.c, devices/gdevclj.c, devices/gdevcljc.c, devices/gdevcslw.c, devices/gdevdjtc.c, devices/gdevdm24.c, devices/gdevepsc.c, devices/gdevepsn.c, devices/gdevescp.c, devices/gdevhl7x.c, devices/gdevijs.c, devices/gdevimgn.c, devices/gdevl31s.c, devices/gdevlbp8.c, devices/gdevlp8k.c, devices/gdevlxm.c, devices/gdevn533.c, devices/gdevo182.c, devices/gdevokii.c, devices/gdevpcl.c, devices/gdevpcl.h, devices/gdevphex.c, devices/gdevpjet.c, devices/gdevsj48.c, devices/gdevsppr.c, devices/gdevstc.c, devices/gdevstc.h, devices/gdevstc1.c, devices/gdevstc2.c, devices/gdevstc3.c, devices/gdevstc4.c, devices/gdevtknk.c, devices/gdevupd.c.

The special rinkj high-quality inkjet driver:
   devices/gdevrinkj.c, base/gsequivc.c, base/gsequivc.h, devices/rinkj/evenbetter-rll.c, devices/rinkj/evenbetter-rll.h, devices/rinkj/rinkj-byte-stream.c, devices/rinkj/rinkj-byte-stream.h, devices/rinkj/rinkj-config.c, devices/rinkj/rinkj-config.h, devices/rinkj/rinkj-device.c, devices/rinkj/rinkj-device.h, devices/rinkj/rinkj-dither.c, devices/rinkj/rinkj-dither.h, devices/rinkj/rinkj-epson870.c, devices/rinkj/rinkj-epson870.h, devices/rinkj/rinkj-screen-eb.c, devices/rinkj/rinkj-screen-eb.h, lib/rinkj-2200-setup.


PostScript interpreter
~~~~~~~~~~~~~~~~~~~~~~~~~~

The PostScript interpreter is conceptually simple: in fact, an interpreter that could execute "3 4 add =" and print "7" was running 3 weeks after the first line of Ghostscript code was written. However, a number of considerations make the code large and complex.

The interpreter is designed to run in environments with very limited memory. The main consequence of this is that it cannot allocate its stacks (dictionary, execution, operand) as ordinary arrays, since the user-specified stack size limit may be very large. Instead, it allocates them as a linked list of blocks. See below for more details.

The interpreter must never cause a C runtime error that it cannot trap. Unfortunately, C implementations almost never provide the ability to trap stack overflow. In order to put a fixed bound on the C stack size, the interpreter never implements PostScript recursion by C recursion. This means that any C code that logically needs to call the interpreter must instead push a continuation (including all necessary state information) on the PostScript execution stack, followed by the PostScript object to be executed, and then return to the interpreter. (See psi/estack.h for more details about continuations.) Unfortunately, since PostScript Level 2 introduces streams whose data source can be a PostScript procedure, any code that reads or writes stream data must be prepared to suspend itself, storing all necessary state in a continuation. There are some places where this is extremely awkward, such as the scanner/parser.

The use of continuations affects many places in the interpreter, and even some places in the graphics library. For example, when processing an image, one may need to call a PostScript procedure as part of mapping a CIE color to a device color. Ghostscript uses a variety of dodges to handle this: for example, in the case of CIE color mapping, all of the PostScript procedures are pre-sampled and the results cached. The Adobe implementation limits this kind of recursion to a fixed number of levels (5?): this would be another acceptable approach, but at this point it would require far more code restructuring than it would be worth.

A significant amount of the PostScript language implementation is in fact written in PostScript. Writing in PostScript leverages the C code for multi-threading, garbage collection, error handling, continuations for streams, etc., etc.; also, we have found PostScript in general more concise and easier to debug than C, mostly because of memory management issues. So given the choice, we tended to implement a feature in PostScript if it worked primarily with PostScript data structures, wasn't heavily used (example: font loading), or if it interacted with the stream or other callback machinery (examples: ReusableFileDecode streams, resourceforall). Often we would add non-standard PostScript operators for functions that had to run faster or that did more C-like things, such as the media matching algorithm for setpagedevice.

Main program
"""""""""""""""""""""""

The main program of the interpreter is normally invoked from the command line, but it has an API as well. In fact, it has two APIs: one that recognizes the existence of multiple "interpreter instances" (although it currently provides a default instance, which almost all clients use), and a completely different one designed for Windows DLLs. These should be unified as soon as possible, since there are two steadily growing incompatible bodies of client code.

Files:
   psi/gs.c, psi/gserver.c, psi/iinit.c, psi/iinit.h, psi/imain.c, psi/imain.h, psi/imainarg.c, psi/imainarg.h, psi/iminst.h, psi/main.h.


Data structures
"""""""""""""""""""""""

The main data structures visible to the PostScript programmers are arrays, contexts, dictionaries, names, and stacks.

Arrays have no unusual properties. See under Refs below for more information about how array elements are stored.

Contexts are used to hold the interpreter state even in configurations that don't include the Display PostScript multiple context extension. Context switching is implemented by a complex cooperation of C and PostScript code.

Dictionaries have two special properties worth noting:

They use an optimized storage representation if all the keys are names, which is almost always the case.

They interact with a caching scheme used to accelerate name lookup in the interpreter.

Names are allocated in blocks. The characters and hash chains are stored separately from the lookup cache information, so that in the future, most of the former can be compiled into the executable and shared or put in ROM. (This is not actually done yet.)

Contexts:
   psi/icontext.c, psi/icontext.h, psi/icstate.h.

Dictionaries:
   psi/iddict.h, psi/idict.h, psi/idict.c, psi/idictdef.h, psi/idicttpl.h.

Names:
   psi/iname.c, psi/iname.h, psi/inamedef.h, psi/inameidx.h, psi/inames.h, psi/inamestr.h.


Stacks
"""""""""""""""""""""""

As mentioned above, each stack is allocated as a linked list of blocks. However, for reasonable performance, operators must normally be able to access their operands and produce their results using indexing rather than an access procedure. This is implemented by ensuring that all the operands of an operator are in the topmost block of the stack, using guard entries that cause an internal error if the condition isn't met. See psi/iostack.h for more details.

Generic stacks:
   psi/isdata.h, psi/istack.c, psi/istack.h, psi/istkparm.h.

Specific stacks:
   Dictionary stack:
      psi/dstack.h, psi/iddstack.h, psi/idsdata.h, psi/idstack.c, psi/idstack.h.

   Execution stack:
      psi/estack.h, psi/iesdata.h, psi/iestack.h.

   Operand stack:
      psi/iosdata.h, psi/iostack.h, psi/ostack.h.


Interpreter loop
"""""""""""""""""""""""


Files:
   psi/interp.c, psi/interp.h.


Scanning/parsing
"""""""""""""""""""""""
PostScript parsing consists essentially of token scanning, and is simple in principle. The scanner is complex because it must be able to suspend its operation at any time (i.e., between any two input characters) to allow an interpreter callout, if its input is coming from a procedure-based stream and the procedure must be called to provide more input data.

Main scanner:
   psi/iscan.c, psi/iscan.h, psi/iscannum.c, psi/iscannum.h, base/scanchar.h, base/scantab.c.

Binary tokens:
   psi/btoken.h, psi/ibnum.c, psi/ibnum.h, psi/inobtokn.c, psi/iscanbin.c, psi/iscanbin.h.

DSC parsing:
   psi/dscparse.c, psi/dscparse.h.


Standard operators
"""""""""""""""""""""""

Non-output-related:
   Filters:
      psi/ifilter.h, psi/ifilter2.h, psi/ifrpred.h, psi/ifwpred.h, psi/istream.h, psi/zfbcp.c, psi/zfdctd.c, psi/zfdcte.c, psi/zfdecode.c, psi/zfilter.c, psi/zfilter2.c, psi/zfjbig2.c, psi/zfjpx.c, psi/zfmd5.c, psi/zfarc4.c, psi/zfproc.c, psi/zfrsd.c, psi/zfzlib.c.

   File and stream I/O:
      psi/files.h, psi/itoken.h, psi/zbseq.c, psi/zdscpars.c, psi/zfile.h, psi/zfile.c, psi/zfile1.c, psi/zfileio.c, psi/ztoken.c.

   Data structures:
      psi/zarray.c, psi/zdict.c, psi/zgeneric.c, psi/zpacked.c, psi/zstring.c.

   Functions:
      psi/ifunc.h, psi/zfunc.c, psi/zfunc0.c, psi/zfunc3.c, psi/zfunc4.c,

   Other:
      psi/ivmem2.h, psi/zalg.c, psi/zarith.c, psi/zcontext.c, psi/zcontrol.c, psi/zmath.c, psi/zmatrix.c, psi/zmisc.c, psi/zmisc1.c, psi/zmisc2.c, psi/zmisc3.c, psi/zrelbit.c, psi/zstack.c, psi/ztype.c, psi/zusparam.c, psi/zvmem.c, psi/zvmem2.c.



Output-related:
   Device management:
      psi/zdevcal.c, psi/zdevice.c, psi/zdevice2.c, psi/ziodev.c, psi/ziodev2.c, psi/ziodevs.c, psi/zmedia2.c,

   Fonts and text:
      psi/bfont.h, psi/ichar.h, psi/ichar1.h, psi/icharout.h, psi/icid.h, psi/ifcid.h, psi/ifont.h, psi/ifont1.h, psi/ifont2.h, psi/ifont42.h, psi/zbfont.c, psi/zcfont.c, psi/zchar.c, psi/zchar1.c, psi/zchar2.c, psi/zchar32.c, psi/zchar42.c, psi/zchar42.h, psi/zcharout.c, psi/zcharx.c, psi/zcid.c, psi/zfcid.c, psi/zfcid0.c, psi/zfcid1.c, psi/zfcmap.c, psi/zfont.c, psi/zfont0.c, psi/zfont1.c, psi/zfont2.c, psi/zfont32.c, psi/zfont42.c, psi/zfontenum.c.

   A bridge to the True Type bytecode interpreter:
      base/gxttfb.c, base/gxttfb.h, base/ttfoutl.h, base/ttfmain.c, base/ttfmemd.c, base/ttfmemd.h, base/ttfinp.c, base/ttfinp.h.

   A reduced True Type bytecode interpreter:
      (this is based in part on the work of the Freetype Team and incorporates some code from the FreeType 1 project)
      base/ttfsfnt.h, base/ttcalc.c, base/ttcalc.h, base/ttcommon.h, base/ttconf.h, base/ttconfig.h, base/ttinterp.c, base/ttinterp.h, base/ttload.c, base/ttload.h, base/ttmisc.h, base/ttobjs.c, base/ttobjs.h, base/tttables.h, base/tttype.h, base/tttypes.h.

   Color, pattern, and halftone:
      psi/icie.h, psi/icolor.h, psi/icremap.h, psi/icsmap.h, psi/iht.h, psi/ipcolor.h, psi/zcie.c, psi/zcolor.c, psi/zcolor1.c, psi/zcolor2.c, psi/zcolor3.c, psi/zcrd.c, psi/zcsindex.c, psi/zcspixel.c, psi/zcssepr.c, psi/zicc.c, psi/zht.c, psi/zht1.c, psi/zht2.h, psi/zht2.c, psi/zpcolor.c, psi/zshade.c, psi/ztrans.c.

   Images:
      psi/iimage.h, psi/zimage.c, psi/zimage3.c, psi/zfimscale.c.

   Other graphics:
      psi/igstate.h, psi/zdpnext.c, psi/zdps.c, psi/zdps1.c, psi/zgstate.c, psi/zpaint.c, psi/zpath.c, psi/zpath1.c, psi/ztrap.c, psi/zupath.c.


Operator support:
   psi/oparc.h, psi/opcheck.h, psi/opdef.h, psi/oper.h, psi/opextern.h.


Non-standard operators
""""""""""""""""""""""""""""""""""""""""""""""

The interpreter includes many non-standard operators. Most of these provide some part of the function of a standard operator, so that the standard operator itself can be implemented in PostScript: these are not of interest to users, and their function is usually obvious from the way they are used. However, some non-standard operators provide access to additional, non-standard facilities that users might want to know about, such as transparency, RasterOp, and in-memory rendering. These are documented at :ref:`Additional Operators<Additional Operators>`.

We don't document the complete set of non-standard operators here, because the set changes frequently. However, all non-standard operators are supposed to have names that begin with '.', so you can find them all by executing the following (Unix) command:

``grep '{".[.]' psi/[zi]*.c``

In addition to individual non-standard operators implemented in the same files as standard ones, there are several independent optional packages of non-standard operators. As with other non-standard operators, the names of all the operators in these packages begin with '.'. We list those packages here.

psi/zdouble.c
   Provides "double" floating point arithmetic, using 8-byte strings to hold values. Developed under a contract; probably used only by the group that funded the development.

psi/zfsample.c,
   Provides a special operator to sample a given function and create a new type 0 function.

psi/zsysvm.c
   Provides operators for allocating objects in specific VM spaces, disregarding the current VM mode.


Interpreter support
""""""""""""""""""""""""""""""""""""""""""""""

Memory management (refs, GC, save/restore) -- see `Postscript Interpreter Extensions`_.

Font API :
   psi/ifapi.h, psi/zfapi.c, base/fapiufst.c, base/fapi_ft.c, base/wrfont.h, base/wrfont.c, base/write_t1.h, base/write_t1.c, base/write_t2.h, base/write_t2.c,

Miscellaneous support:
   psi/ierrors.h, base/gserrors.h, psi/ghost.h, psi/iconf.c, psi/iconf.h, psi/idparam.c, psi/idparam.h, psi/ilevel.h, psi/inouparm.c, psi/iparam.c, psi/iparam.h, psi/iparray.h, psi/iutil.c, psi/iutil.h, psi/iutil2.c, psi/iutil2.h, psi/iplugin.c, psi/iplugin.h.


PostScript code
""""""""""""""""""""""""""""""""""""""""""""""

Initialization and language support:
   All configurations:
      Resource/Init/gs_init.ps, Resource/Init/gs_statd.ps.

      Level 2:
         Resource/Init/gs_btokn.ps, Resource/Init/gs_dps1.ps, Resource/Init/gs_dps2.ps, Resource/Init/gs_lev2.ps, Resource/Init/gs_res.ps, Resource/Init/gs_resmp.ps, Resource/Init/gs_setpd.ps.

      LanguageLevel 3:
         Resource/Init/gs_frsd.ps, Resource/Init/gs_ll3.ps, Resource/Init/gs_trap.ps.

      Display PostScript:
         Resource/Init/gs_dpnxt.ps, Resource/Init/gs_dps.ps.

      Emulation of other interpreters:
         Resource/Init/gs_cet.ps (Adobe CPSI).


Color Spaces and support:
   Color Space Loading:
      Resource/Init/gs_cspace.ps,

   ICC color profiles:
      Resource/Init/gs_icc.ps.


Font loading and support:
   Font name mapping:
      Resource/Init/Fontmap, lib/Fontmap.ATB, lib/Fontmap.ATM, Resource/Init/Fontmap.GS, lib/Fontmap.OS2, lib/Fontmap.OSF, lib/Fontmap.SGI, lib/Fontmap.Sol, lib/Fontmap.Ult, lib/Fontmap.VMS, lib/Fontmap.URW-136.T1, lib/Fontmap.URW-136.TT, Resource/Init/cidfmap, Resource/Init/FAPIcidfmap, Resource/Init/FAPIfontmap, Resource/Init/FCOfontmap-PCLPS2.

   Generic:
      Resource/Init/gs_fonts.ps, Resource/Init/gs_fntem.ps.

   Type 1 and CFF:
      Resource/Init/gs_cff.ps, Resource/Init/gs_diskf.ps, Resource/Init/gs_type1.ps.

   TrueType:
      Resource/Init/gs_ttf.ps, Resource/Init/gs_typ42.ps.

   CID-keyed:
      Resource/Init/gs_cidcm.ps, Resource/Init/gs_cidfn.ps, Resource/Init/gs_cmap.ps, Resource/Init/gs_ciddc.ps, Resource/Init/gs_cidfm.ps, Resource/Init/gs_cidtt.ps.

   Font API:
      Resource/Init/gs_fapi.ps, Resource/Init/FAPIconfig, lib/FAPIconfig-FCO, Resource/Init/xlatmap. Resource/Init/FCOfontmap-PCLPS2. lib/FCOfontmap-PCLPS3. lib/FCOfontmap-PS3.

   Other:
      lib/gs_kanji.ps, lib/gs_pfile.ps, Resource/Init/gs_typ32.ps.


Encodings:
   Adobe-specified:
      lib/gs_ce_e.ps, Resource/Init/gs_dbt_e.ps, Resource/Init/gs_il1_e.ps, Resource/Init/gs_mex_e.ps, Resource/Init/gs_mro_e.ps, Resource/Init/gs_pdf_e.ps, Resource/Init/gs_std_e.ps, Resource/Init/gs_sym_e.ps, Resource/Init/gs_wan_e.ps.

   Additional:
      lib/gs_il2_e.ps, lib/gs_ksb_e.ps, lib/gs_wl1_e.ps, lib/gs_wl2_e.ps, lib/gs_wl5_e.ps.

   Pseudo-encodings for internal use:
      lib/gs_lgo_e.ps, lib/gs_lgx_e.ps, Resource/Init/gs_mgl_e.ps.


Miscellaneous:
   Image support:
      Resource/Init/gs_img.ps,

   Emulation of %disk IODevice:
      Resource/Init/gs_diskn.ps,

   Other support:
      Resource/Init/gs_agl.ps, Resource/Init/gs_dscp.ps, Resource/Init/gs_epsf.ps, Resource/Init/gs_pdfwr.ps, lib/gs_rdlin.ps.

   X Windows icon bitmaps:
      lib/gs_l.xbm, lib/gs_l.xpm, lib/gs_l_m.xbm, lib/gs_m.xbm, lib/gs_m.xpm, lib/gs_m_m.xbm, lib/gs_s.xbm, lib/gs_s.xpm, lib/gs_s_m.xbm, lib/gs_t.xbm, lib/gs_t.xpm, lib/gs_t_m.xbm.

   PDF/X-3 definition file sample:
      lib/PDFX_def.ps


PDF interpreter
~~~~~~~~~~~~~~~~~~~~

Ghostscript's PDF interpreter is written entirely in PostScript, because its data structures are the same as PostScript's, and it is much more convenient to manipulate PostScript-like data structures in PostScript than in C. There is definitely a performance cost, but apparently not a substantial one: we considered moving the main interpreter loop (read a token using slightly different syntax than PostScript, push it on the stack if literal, look it up in a special dictionary for execution if not) into C, but we did some profiling and discovered that this wasn't accounting for enough of the time to be worthwhile.

Until recently, there was essentially no C code specifically for the purpose of supporting PDF interpretation. The one major exception is the PDF 1.4 transparency features, which we (but not Adobe) have made available to PostScript code.

In addition to patching the run operator to detect PDF files, the interpreter provides some procedures in Resource/Init/pdf_main.ps that are meant to be called from applications such as previewers.

Files:
   Resource/Init/pdf_base.ps, Resource/Init/pdf_draw.ps, Resource/Init/pdf_font.ps, Resource/Init/pdf_main.ps, Resource/Init/pdf_rbld.ps, Resource/Init/pdf_ops.ps, Resource/Init/pdf_sec.ps.


PostScript Printer Description
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

A PostScript Printer Description tells a generic PostScript printer driver how to generate PostScript for a particular printer. Ghostscript includes a PPD file for generating PostScript intended to be converted to PDF. A Windows INF file for installing the PPD on Windows 2000 and XP is included.

Files:
   lib/ghostpdf.ppd, lib/ghostpdf.inf, lib/ghostpdf.cat, lib/ghostpdf.README.

Build process
~~~~~~~~~~~~~~~~~~~~

Makefile structure
""""""""""""""""""""""""""

Ghostscript's makefiles embody a number of design decisions and assumptions that may not be obvious from a casual reading of the code.

- All files are stored in subdirectories. The names of all subdirectories used in the build process are defined in the top-level makefiles for the various platforms: there are no "hard wired" directory names in any makefile rule. Subdirectory names in the makefiles are relative to the directory that is current at build time: normally this directory is the parent of the various subdirectories, and holds only a makefile, which in turn simply references the real top-level makefile in the source subdirectory.

- All compiler and linker switches are likewise defined by macros, again preferably in the top-level platform makefile.

- There is an absolute distinction between "source-like" subdirectories, which are read-only during the build process, and "object-like" subdirectories, all of whose contents are generated by the build process and which can be emptied (``rm *``) at any time with no bad effects. The source subdirectories are defined by macros named ``xxxSRCDIR``.

- Object subdirectories may distinguish further between those that hold the results of the build process that are needed at run time (i.e., that should be included in a run-time distribution), defined by BINDIR, and those that are not needed at run time, defined by xxxGENDIR and xxxOBJDIR. (The distinction between these is historical and probably no longer relevant).

- There may be multiple object subdirectories for different build configurations. On Unix, the obj and bin directories are used for normal production builds, the debugobj directory for debugging builds, and the pgobj directory for profiling builds; other platforms may use different conventions. The Unix makefiles support targets named debug and pg for debugging and profiling builds respectively; other platforms generally do not.

- Makefiles will be maintained by hand. This requires editing the following makefile elements whenever the relevant source files changes:

   - Every header (.h) file must have a corresponding macro definition in a makefile. If abc.h #includes def.h and xyz.h, the definition must have the form:
      ``xyz_h=$(xxxSRCD)xyz.h $(def_h) $(xyz_h)``

      where ``xxxSRCD`` is the macro defining the relevant source directory (including a trailing '``/``'). Note that the '``.``' in the file name has been replaced by an underscore. Note also that the definition must follow all definitions it references, since some make programs expand macros in definitions at the time of definition rather than at the time of use.

   - Every .c file must have a corresponding rule in a makefile. If abc.c #includes def.h and lmn.h, the rule must have approximately the form:

      ``$(xxxOBJD)abc.$(OBJ) : $(xxxSRCD)abc.c $(def_h) $(lmn_h)``
      ``$(xxCC) $(xxO_)abc.$(OBJ) $(C_) $(xxxSRCD)abc.c``

      where ``xxxSRCD`` is as before; ``xxxOBJD`` is the relevant object directory; ``xxCC`` defines the name of the C compiler plus the relevant compilation switches; and ``xxO_`` and ``C_`` are macros used to bridge syntactic differences between different make programs.

The requirement to keep makefiles up to date by hand has been controversial. Two alternatives are generally proposed.

- Programs like ``makedeps``, which generate build rules automatically from the #include lists in C files. We have found such programs useless: they "wire in" specific, concrete directory names, not only for our own code but even for the system header files; they have to be run manually whenever code files are added, renamed, or deleted, or whenever the list of #includes in any file changes; and they cannot deal with different source files requiring different compilation switches.

- MSVC-style "project" files. These are equally useless: they are not portable across different vendors' tools -- in fact, there may not even be a usable import/export facility to convert their data to or from text form -- and they cannot combine configuration-independent data with configuration-specific data.


We have seriously considered writing our own build program in Tcl or Python that would eliminate these problems, or perhaps porting the tools developed by Digital's Vesta research project (if we can get access to them); however, either of these approaches would create potential portability problems of its own, not to mention difficulties in integrating with others' build systems.

For more information about makefiles:

- For a detailed list of makefiles, and a discussion of makefile issues related to portability, see the Makefiles_ section below.
- For more detailed information about editing configuration information in makefiles, see :ref:`Makefiles Overview<Make_MakeFilesOverview>`.
- For a complete example of adding a new driver to a makefile, see :ref:`Drivers<Drivers.html>`.
- For a few more notes on makefile coding conventions, see :ref:`C-Style Makefiles<CStyle_Makefiles>`.



.dev files
""""""""""""""""""""""""""

On top of the general conventions just described, Ghostscript's makefiles add a further layer of structure in order to support an open-ended set of fine-grained, flexible configuration options. Selecting an option (usually called a "module") for inclusion in the build may affect the build in many ways:

- Almost always, it requires linking in some compiled code files.
- It may require running some additional initialization procedures at startup.
- It may require reading in some additional PostScript files at startup. For example, a Level 2 PostScript build requires support for PostScript resources and for ``setpagedevice``, which are implemented in PostScript code.
- It may require adding entries to a variety of internal tables that catalogue such things as drivers, IODevices, Function types, etc.
- It may require that other particular modules be included. For example, the "PostScript Level 2" module requires the modules for various filters, color spaces, etc.
- It may require removing some other (default) module from the build. For example, the core (Level 1) PostScript build has a "stub" for binary tokens, which are a Level 2 feature but are referenced by the core scanner: a Level 2 build must remove the stub. For more information about this, look for the string ``-replace`` in the makefiles and in ``base/genconf.c``.


Each module is defined in the makefiles by rules that create a file named ``xxx.dev``. The dependencies of the rule include all the files that make up the module (compiled code files, PostScript files, etc.); the body of the rule creates the .dev file by writing the description of the module into it. A program called genconf, described in the next section, merges all the relevant .dev files together. For examples of .dev rules, see any of the Ghostscript makefiles.

Ultimately, a person must specify the root set of modules to include in a build (which of course may require other modules, recursively). Ghostscript's makefiles do this with a set of macros called ``FEATURE_DEVS`` and ``DEVICE_DEVSn``, defined in each top-level makefile, but nothing in the module machinery depends on this.

Generators
""""""""""""""""""""""""""

Ghostscript's build procedure is somewhat unusual in that it compiles and then executes some support programs during the build process. These programs then generate data or source code that is used later on in the build.

The most important and complex of the generator programs is genconf. genconf merges all the .dev files that make up the build, and creates three or more output files used in later stages:

- ``gconfig.h``, consisting mainly of macro calls, one call per "resource" making up the build, other than "resources" listed in the other output files.
- ``gconfigf.h``, produced only for PostScript builds with compiled-in fonts, consisting of one macro call per font.
- A linker control file whose name varies from one platform to another, containing the list of compiled code files to be linked.
- If necessary, another linker control file, also varying between platforms, that contains other information for the linker such as the list of system libraries to be searched. (On Unix platforms, the two linker control functions are combined in a single file).


Source generators:
   base/genarch.c
      Creates a header file containing a variety of information about the hardware and compiler that isn't provided in any standard system header file. Always used.

   base/genconf.c (also generates non-source)
      Constructs header files and linker control files from the collection of options and modules that make up the build. See above. Always used.

   base/genht.c
      Converts a PostScript halftone (in a particular constrained format) to a C data structure that can be compiled into an executable. Only used if any such halftones are included in the build.

   base/mkromfs.c
      Takes a set of directories, and creates a compressed filesystem image that can be compiled into the executable as static data and accessed through the %rom% iodevice prefix. This is used to implement the ``COMPILE_INITS=1`` feature (a compressed init fileset is more efficient than the current 'gsinit.c' produced by 'geninit.c'). This IODevice is more versatile since other files can be encapsulated such as fonts, helper PostScript files and Resources. The list of files is defined in part in psi/psromfs.mak.


Other generators:
   base/echogs.c
      Implements a variety of shell-like functions to get around quirks, limitations, and omissions in the shells on various platforms. Always used.

   base/genconf.c (also generates source)
      See above.

   base/gendev.c (not used)
      Was intended as a replacement for genconf, but was never completed.


Support
""""""""""""""""""""""""""

There are a number of programs, scripts, and configuration files that exist only for the sake of the build process.

Files for PC environments:
   base/gswin.icx, base/gswin16.icx, base/bcc32.cfg, base/cp.bat, base/cp.cmd, psi/dw32c.def, psi/dwmain.rc, psi/dwmain32.def, psi/dwsetup.def, psi/dwsetup_x86.manifest, psi/dwsetup_x64.manifest, psi/dwuninst.def, psi/dwuninst_x86.manifest, psi/dwuninst_x64.manifest, psi/gsdll2.def, psi/gsdll2.rc, psi/gsdll32.def, psi/gsdll32.rc, psi/gsdll32w.lnk, psi/gsos2.def, psi/gsos2.icx, psi/gsos2.rc, base/gspmdrv.def, base/gspmdrv.icx, base/gspmdrv.rc, base/gswin.rc, base/gswin32.rc, base/mv.bat, base/mv.cmd, base/rm.bat, base/rm.cmd,

Files for MacOS:
   lib/Info-macos.plist.

Files for OpenVMS:
   base/append_l.com, base/copy_one.com, base/rm_all.com, base/rm_one.com.

Other files:
   base/bench.c, base/catmake, base/instcopy.


Utilities
~~~~~~~~~~~~~

Ghostscript comes with many utilities for doing things like viewing bitmap files and converting between file formats. Some of these are written in PostScript, some as scripts, and some as scripts that invoke special PostScript code.

Utilities in PostScript
""""""""""""""""""""""""""""""

These are all documented in doc/Psfiles.html, q.v.

Utility scripts
""""""""""""""""""""""""""""""

Many of these scripts come in both Unix and MS-DOS (.bat versions; some also have an OS/2 (.cmd) version. The choice of which versions are provided is historical and irregular. These scripts should all be documented somewhere, but currently, many of them have man pages, a few have their own documentation in the doc directory, and some aren't documented at all.

Script files without PC versions:
   lib/afmdiff.awk, lib/dvipdf, lib/lprsetup.sh, lib/pphs, lib/printafm, lib/unix-lpr.sh, lib/wftopfa.

Script files with PC versions:
   lib/eps2eps, lib/eps2eps.bat, lib/eps2eps.cmd, lib/ps2ps2, lib/ps2ps2.bat, lib/ps2ps2.cmd, lib/font2c, lib/font2c.bat, lib/font2c.cmd, lib/gsbj, lib/gsbj.bat, lib/gsdj, lib/gsdj.bat, lib/gsdj500, lib/gsdj500.bat, lib/gslj, lib/gslj.bat, lib/gslp, lib/gslp.bat, lib/gsnd, lib/gsnd.bat, lib/pdf2dsc, lib/pdf2dsc.bat, lib/pdf2ps, lib/pdf2ps.bat, lib/pdf2ps.cmd, lib/pf2afm, lib/pf2afm.bat, lib/pf2afm.cmd, lib/pfbtopfa, lib/pfbtopfa.bat, lib/ps2ascii, lib/ps2ascii.bat, lib/ps2ascii.cmd, lib/ps2epsi, lib/ps2epsi.bat, lib/ps2epsi.cmd, lib/ps2pdf, lib/ps2pdf.bat, lib/ps2pdf.cmd, lib/ps2pdf12, lib/ps2pdf12.bat, lib/ps2pdf12.cmd, lib/ps2pdf13, lib/ps2pdf13.bat, lib/ps2pdf13.cmd, lib/ps2pdf14, lib/ps2pdf14.bat, lib/ps2pdf14.cmd, lib/ps2pdfwr, lib/ps2pdfxx.bat, lib/ps2ps, lib/ps2ps.bat, lib/ps2ps.cmd.

Script files with only PC versions:
   lib/gsndt.bat, lib/gssetgs.bat, lib/gssetgs32.bat, lib/gssetgs64.bat, lib/gst.bat, lib/gstt.bat, lib/lp386.bat, lib/lp386r2.bat, lib/lpgs.bat, lib/lpr2.bat, lib/pftogsf.bat, lib/wmakebat.bat.



Memory management
---------------------

Memory manager architecture
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

In many environments, the memory manager is a set of library facilities that implicitly manage the entire address space in a homogenous manner. Ghostscript's memory manager architecture has none of these properties:

- Rather than a single library accessed as procedures, Ghostscript includes multiple allocator types, each of which in turn may have multiple instances (allocators). Allocators are 'objects' with a substantial set of virtual functions.
- Rather than managing the entire address space, each allocator manages a storage pool, which it may or may not be able to expand or reduce by calling on a 'parent' allocator.
- Rather than a single genus of untyped storage blocks, Ghostscript's allocators provide two genera -- type-tagged 'objects', and 'strings' -- with substantially different properties.

Objects vs strings
""""""""""""""""""""""

As noted above, allocators provide two different storage genera.

Objects:

- Are aligned in storage to satisfy the most stringent alignment requirement imposed by the CPU or compiler;
- Can be referenced only by pointers to their start, not to any internal location, unless special arrangements are made;
- May contain pointers to other objects, or to strings;
- Have an associated structure descriptor that specifies their size (usually) and the location of any pointers contained within them.


Given a pointer to an object, the allocator that allocated it must be able to return the object's size and the pointer to its structure descriptor. (It is up to the client to know what allocator allocated an object.)

Strings:

- Are not aligned in storage;
- Can be referenced by pointers (consisting of a starting address and a length) to any substring, starting anywhere within the string;
- May not contain pointers;
- Do not have a structure descriptor.

The object/string distinction reflects a space/capability tradeoff. The per-object space overhead of the standard type of allocator is typically 12 bytes; this is too much to impose on every string of a few bytes. On the other hand, restricting object pointers to reference the start of the object currently makes object garbage collection and compaction more space-efficient. If we were to redesign the standard allocator, we would probably opt for a different design in which strings were allocated within container objects of a few hundred bytes, and pointers into the middle of all objects were allowed.


.. _Develop_Structure_Descriptors:

Structure descriptors
""""""""""""""""""""""""""""""""""""""""""""

Every object allocated by a Ghostscript allocator has an associated structure descriptor, which the caller provides when allocating the object. The structure descriptor serves several purposes:

- Specifying the size of the object for allocation;
- Providing pointer-enumeration and pointer-relocation procedures for the garbage collector;
- Providing an optional finalization procedure to be called when the object is freed (either explicitly or automatically).

Structure descriptors are read-only, and are normally defined statically using one of the large set of ``gs_private_st_`` or ``gs_public_st_`` macros in base/gsstruct.h.

While the structure descriptor normally specifies the size of the object, one can also allocate an array of bytes or objects, whose size is a multiple of the size in the descriptor. For this reason, every object stores its size as well as a reference to its descriptor.

Because the standard Ghostscript garbage collector is non-conservative and can move objects, every object allocated by a Ghostscript allocator must have an accurate structure descriptor. If you define a new type of object (structure) that will be allocated in storage managed by Ghostscript, you must create an accurate descriptor for it, and use that descriptor to allocate it. The process of creating accurate descriptors for all structures was long and painful, and accounted for many hard-to-diagnose bugs.

By convention, the structure descriptor for structure type ``xxx_t`` is named ``st_xxx`` (this is preferred), or occasionally ``st_xxx_t``.

Note that a structure descriptor is only required for objects allocated by the Ghostscript allocator. A structure type ``xxx_t`` does not require a structure descriptor if instances of that type are used only in the following ways:

- Instances are allocated only on the C stack, e.g., as ``xxx_t``, ``xxx1``, ``xxx2``;, or on the C heap, with malloc or through the Ghostscript "wrapper" defined in base/gsmalloc.h.

- Pointers to instances are not stored in places where the garbage collector will try to trace the pointer.
- Code never attempts to get the structure type or size of an instance through the allocator API.

In general, structures without descriptors are problem-prone, and are deprecated; in new code, they should only be used if the structure is confined to a single .c file and its instances are only allocated on the C stack.

Files:
   base/gsstruct.h, base/gsstype.h.

Garbage collection
""""""""""""""""""""""

The allocator architecture is designed to support compacting garbage collection. Every object must be able to enumerate all the pointers it contains, both for tracing and for relocation. As noted just above, the structure descriptor provides procedures that do this.

Whether or not a particular allocator type actually provides a garbage collector is up to the allocator: garbage collection is invoked through a virtual procedure. In practice, however, there are only two useful garbage collectors for Ghostscript's own allocator:

- The "real" garbage collector associated with the PostScript interpreter, described :ref:`below<Interpreter_GC>`;
- A "non" garbage collector that only merges adjacent free blocks.

As noted above, because the architecture supports compacting garbage collection, a "real" garbage collector cannot be run at arbitrary times, because it cannot reliably find and relocate pointers that are on the C stack. In general, it is only safe to run a "real" garbage collector when control is at the top level of the program, when there are no pointers to garbage collectable objects from the stack (other than designated roots).

Files:
   base/gsgc.h, base/gsnogc.c, base/gsnogc.h.


Movability
""""""""""""""""""""""
As just noted, objects are normally movable by the garbage collector. However, some objects must be immovable, usually because some other piece of software must retain pointers to them. The allocator API includes procedures for allocating both movable (default) and immovable objects. Note, however, that even immovable objects must be traceable (have a structure descriptor), and may be freed, by the garbage collector.



Parent hierarchy
""""""""""""""""""""""

When an allocator needs to add memory to the pool that it manages, it requests the memory from its parent allocator. Every allocator has a pointer to its parent; multiple allocators may share a single parent. The ultimate ancestor of all allocators that can expand their pool dynamically is an allocator that calls malloc, described below. However, especially in embedded environments, an allocator may be limited to a fixed-size pool assigned to it when it is created.

Allocator API
""""""""""""""""""""""

In summary, the allocator API provides the following principal operations:

- Allocate and free movable (default) or immovable objects and strings.
- Return the structure type and size of an object.
- Resize (shrink or grow) movable objects and strings, preserving the contents insofar as possible.
- Report the size of the managed pool, and how much of it is in use.
- Register and unregister root pointers for the garbage collector.
- Free the allocator itself.
- Consolidate adjacent free blocks to reduce fragmentation.

For details, see base/gsmemory.h.

The allocator API also includes one special hook for the PostScript interpreter: the concept of stable allocators. See the section on "save and restore" below for details.

Files:
   base/gsmemraw.h, base/gsmemory.c, base/gsmemory.h, base/gsstruct.h, base/gsstype.h.

Freeing storage
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Ghostscript's memory management architecture provides three different ways to free objects: explicitly, by reference counting, or by garbage collection. They provide different safety / performance / convenience tradeoffs; we believe that all three are necessary.

Objects are always freed as a whole; strings may be freed piecemeal.

An object may have an associated finalization procedure, defined in the structure descriptor. This procedure is called just before the object is freed, independent of which method is being used to free the object. A few types of objects have a virtual finalization procedure as well: the finalization procedure defined in the descriptor simply calls the one in the object.

Explicit freeing
""""""""""""""""""""""

Objects and strings may be freed explicitly, using the ``gs_free_`` virtual procedures in the allocator API. It is up to the client to ensure that all allocated objects are freed at most once, and that there are no dangling pointers.

Explicit freeing is the fastest method, but is the least convenient and least safe. It is most appropriate when storage is freed in the same procedure where it is allocated, or for storage that is known to be referenced by only one pointer.

Reference counting
""""""""""""""""""""""

Objects may be managed by reference counting. When an object is allocated, its reference count may be set to 0 or 1. Subsequently, when the reference count is decremented to 0, the object is freed.

The reference counting machinery provides its own virtual finalization procedure for all reference-counted objects. The machinery calls this procedure when it is about to free the object (but not when the object is freed in any other way, which is probably a design bug). This is in addition to (and called before) any finalization procedure associated with the object type.

Reference counting is as fast as explicit freeing, but takes more space in the object. It is most appropriate for relatively large objects which are referenced only from a small set of pointers. Note that reference counting cannot free objects that are involved in a pointer cycle (e.g., A -> B -> C -> A).

Files:
   base/gsrefct.h.


(Real) garbage collection
""""""""""""""""""""""""""""""""""""""""""""

Objects and strings may be freed automatically by a garbage collector. See :ref:`below<Interpreter_GC>`.

Special implementations
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

malloc
""""""""""""""""""""""""""""""""""""""""""""

As mentioned :ref:`above<Parent hierarchy>`, the ultimate ancestor of all allocators with an expandable pool is one that calls malloc.

Note that the default gsmalloc.c allocator for malloc/free now uses a mutex so that allocators that use this can be assured of thread safe behavior.

Files:
   base/gsmalloc.h, base/gsmalloc.c.

Locking
""""""""""""""""""""""""""""""""""""""""""""

In a multi-threaded environment, if an allocator must be callable from multiple threads (for example, if it is used to allocate structures in one thread that are passed to, and freed by, another thread), the allocator must provide mutex protection. Ghostscript provides this capability in the form of a wrapper allocator, that simply forwards all calls to a target allocator under protection of a mutex. Using the wrapper technique, any allocator can be made thread-safe.

Files:
   base/gsmemlok.h, base/gsmemlok.c.

Retrying
""""""""""""""""""""""""""""""""""""""""""""

In an embedded environment, job failure due to memory exhaustion is very undesirable. Ghostscript provides a wrapper allocator that, when an allocation attempt fails, calls a client-provided procedure that can attempt to free memory, then ask for the original allocation to be retried. For example, such a procedure can wait for a queue to empty, or can free memory occupied by caches.

Files:
   base/gsmemret.h, base/gsmemret.c.

Chunk
""""""""""""""""""""""""""""""""""""""""""""

When multiple threads are used and there may be frequent memory allocator requests, mutex contention is a problem and can cause severe performance degradation. The chunk memory wrapper can provide each thread with its own instance of an allocator that only makes requests on the underlying (non-GC) alloctor when large blocks are needed. Small object allocations are managed within chunks.

This allocator is intended to be used on top of the basic 'gsmalloc' allocator (malloc/free) which is NOT garbage collected or relocated and which MUST be mutex protected.

Files:
   base/gsmchunk.h, base/gsmchunk.c.

Standard implementation
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The standard Ghostscript allocator gets storage from its parent (normally the malloc allocator) in large blocks called clumps, and then allocates objects up from the low end and strings down from the high end. Large objects or strings are allocated in their own clump.

The standard allocator maintains a set of free-block lists for small object sizes, one list per size (rounded up to the word size), plus a free-block list for large objects (but not for objects so large that they get their own clump: when such an object is freed, its chunk is returned to the parent). The lists are not sorted; adjacent blocks are only merged if needed.

While the standard allocator implements the generic allocator API, and is usable with the library alone, it includes a special hook for the PostScript interpreter to aid in the efficient allocation of PostScript composite objects (arrays and dictionaries). See the section on Refs below for details.

Files:
   base/gsalloc.c, base/gsalloc.h, base/gxalloc.h, base/gxobj.h.


PostScript interpreter extensions
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The PostScript interpreter uses an allocator that extends the graphic library's standard allocator to handle PostScript objects, save and restore, and real garbage collection.

Refs (PostScript "objects")
""""""""""""""""""""""""""""""""""""""""""""

Ghostscript represents what the PLRM calls PostScript "objects" using a structure called a ref, defined in psi/iref.h; packed refs, used for the elements of packed arrays, are defined in psi/ipacked.h. See those files for detailed information.

Files:
   psi/ipacked.h, psi/iref.h.

The PLRM calls for two types of "virtual memory" (VM) space: global and local. Ghostscript adds a third space, system VM, whose lifetime is an entire session -- i.e., it is effectively "permanent". All three spaces are subject to garbage collection. There is a separate allocator instance for each VM space (actually, two instances each for global and local spaces; see below). In a system with multiple contexts and multiple global or local VMs, each global or local VM has its own allocator instance(s).

Refs that represent PostScript composite objects, and therefore include pointers to stored data, include a 2-bit VM space tag to indicate in which VM the object data are stored. In addition to system, global, and local VM, there is a tag for "foreign" VM, which means that the memory is not managed by a Ghostscript allocator at all. Every store into a composite object must check for invalidaccess: the VM space tag values are chosen to help make this check efficient. See psi/ivmspace.h, psi/iref.h, and psi/store.h for details.

Files:
   psi/ivmspace.h.

PostScript composite objects (arrays and dictionaries) are usually small. Using a separate memory manager object for each composite object would waste a lot of space for object headers. Therefore, the interpreter's memory manager packs multiple composite objects (also called "ref-containing objects") into a single memory manager object, similar to the way the memory manager packs multiple objects into a clump (see above). See base/gxalloc.h for details. This memory manager object has a structure descriptor, like all other memory manager objects.

Note that the value.pdict, value.refs, or value.packed member of a ref must point to a PostScript composite object, and therefore can point into the middle of a memory manager object. This requires special handling by the garbage collector (q.v.).

Files:
   psi/ialloc.c, psi/ialloc.h, psi/iastate.h, psi/iastruct.h, psi/ilocate.c, psi/imemory.h, psi/istruct.h.
   save/.forgetsave/restore

In addition to save and restore, Ghostscript provides a .forgetsave operator that makes things as though a given save had never happened. (In data base terminology, save is "begin transaction", restore is "abort transaction", and .forgetsave is "end/commit transaction"). .forgetsave was implemented for a specific commercial customer (who may no longer even be using it): it was a pain to make work, but it's in the code now, and should be maintained. See the extensive comments in psi/isave.c for more information about how these operations work.

Files:
   psi/idosave.h, psi/isave.c, psi/isave.h, psi/isstate.h, psi/store.h.


Stable allocators
""""""""""""""""""""

Even though ``save`` and ``restore`` are concepts from the PostScript interpreter, the generic allocator architecture and API include a feature to support them, called stable allocators. Every allocator has an associated stable allocator, which tags pointers with the same VM space number but which is not subject to save and restore. System VM is intrinsically stable (its associated stable allocator is the same allocator), so there are only 5 allocators in ordinary single-context usage: system VM, stable global VM, ordinary global VM, stable local VM, ordinary local VM.

The reason that we cannot simply allocate all stable objects in system VM is that their refs must still be tagged with the correct VM space number, so that the check against storing pointers from global VM to local VM can be enforced properly.

All PostScript objects are normally allocated with the non-stable allocators. The stable allocators should be used with care, since using them can easily create dangling pointers: if storage allocated with a stable allocator contains any references to PostScript objects, the client is responsible for ensuring that the references don't outlive the referenced objects, normally by ensuring that any such referenced objects are allocated at the outermost save level.

The original reason for wanting stable allocators was the PostScript stacks, which are essentially PostScript arrays but are not subject to save and restore. Some other uses of stable allocators are:

- Several per-context structures for DPS.
- Paths (see ``gstate_path_memory`` in base/gsstate.c.
- Row buffers for images (see ``gs_image_row_memory`` in base/gsimage.c), because the data-reading procedure for an image can invoke save and restore.
- Notification lists for fonts, to handle the sequence allocate .. save .. register .. restore.
- The parameter lists for pdfwrite and epswrite devices (in devices/vector/gdevpsdp.c), because the whole issue of local vs. global VM for ``setpagedevice`` is, in the words of Ed Taft of Adobe, "a mess".
- Many places in the pdfwrite driver, because many of its bookkeeping structures must not be restorable.


For more specific examples, search the sources for references to ``gs_memory_stable``.


.. _Interpreter_GC:

Garbage collection
"""""""""""""""""""""""

The interpreter's garbage collector is a compacting, non-conservative, mark-and-sweep collector.

- It compacts storage because that is the only way to avoid permanent sandbars, which is essential in limited-memory environments.
- It is non-conservative because conservative collectors (which attempt to determine whether arbitrary bit patterns are pointers) cannot compact.
- It uses mark-and-sweep, rather than a more modern copying approach, because it cannot afford the extra memory required for copying.


Because the garbage collector is non-conservative, it cannot be run if there are any pointers to movable storage from the C stack. Thus it cannot be run automatically when the allocator is unable to allocate requested space. Instead, when the allocator has allocated a given amount of storage (the ``vm_threshold`` amount, corresponding to the PostScript ``VMThreshold`` parameter), it sets a flag that the interpreter checks in the main loop. When the interpreter sees that this flag is set, it calls the garbage collector: at that point, there are no problematic pointers from the stack.

Roots for tracing must be registered with the allocator. Most roots are registered during initialization.

"Mark-and-sweep" is a bit of a misnomer. The garbage collector actually has 5 main phases:

- Sweep to clear marks;
- Trace and mark;
- Sweep to compute relocation;
- Sweep to relocate pointers;
- Sweep and compact.


There is some extra complexity to handle collecting local VM only. In this case, all pointers in global VM are treated as roots, and global VM is not compacted.

As noted above, PostScript arrays and strings can have refs that point within them (because of ``getinterval``). Thus the garbage collector must mark each element of an array, and even each byte of a string, individually. Specifically, it marks objects, refs, and strings using 3 different mechanisms:

- Objects have a mark bit in their header: see base/gxobj.h,
- Refs and packed refs have a reserved mark bit: see psi/iref.h and psi/ipacked.h.
- Strings use a separate bit table, with one bit per string byte: see base/gxalloc.h,

Similarly, it records the relocation information for objects, refs, and strings differently:

- Objects record relocation in the object header.
- Refs record relocation in unused fields of refs such as nulls that don't use their value field. Every memory manager object that stores ref-containing objects as described above has an extra, unused ref at the end for this purpose.
- Strings use a separate relocation table.

Files:
   psi/igc.c, psi/igc.h, psi/igcref.c, psi/igcstr.c, psi/igcstr.h, psi/ireclaim.c.


Portability
~~~~~~~~~~~~~~~~~~~~~~~

One of Ghostscript's most important features is its great portability across platforms (CPUs, operating systems, compilers, and build tools). The code supports portability through two mechanisms:

- `Structural mechanisms`_ -- segregating platform-dependent information into files in a particular way.
- `Coding standards`_ -- avoiding relying on byte order, scalar size, and platform-specific compiler or library features.




Structural mechanisms
"""""""""""""""""""""""""

CPU and compiler
^^^^^^^^^^^^^^^^^^^^^^^^^^

Ghostscript attempts to discover characteristics of the CPU and compiler automatically during the build process, by compiling and then executing a program called genarch. genarch generates a file ``obj/arch.h``, which almost all Ghostscript files then include. This works well for things like word size, byte order, and floating point representation, but it can't determine whether or not a compiler supports a particular feature, because if a feature is absent, the compilation may fail.

Files:
   base/genarch.c, obj/arch.h.

Library headers
^^^^^^^^^^^^^^^^^^^^^^^^^^
Despite the supposed standardization of ANSI C, platforms vary considerably in where (and whether) they provide various standard library facilities. Currently, Ghostscript's build process doesn't attempt to sort this out automatically. Instead, for each library header file ``<xxx.h>`` there is a corresponding Ghostscript source file ``base/xxx_.h``, containing a set of compile-time conditionals that attempt to select the correct platform header file, or in some cases substitute Ghostscript's own code for a missing facility. You may need to edit these files when moving to platforms with unusually non-standard libraries.

Files:
   ``base/ctype_.h``, ``base/dirent_.h``, ``base/dos_.h``, ``base/errno_.h``, ``base/fcntl_.h``, ``base/jerror_.h``, ``base/malloc_.h``, ``base/math_.h``, ``base/memory_.h``, ``base/pipe_.h``, ``base/png_.h``, ``base/setjmp_.h``, ``base/stat_.h``, ``base/stdint_.h``, ``base/stdio_.h``, ``base/string_.h``, ``base/time_.h``, ``base/unistd_.h``, ``base/vmsmath.h``, ``base/windows_.h``, ``base/x_.h``

It has been suggested that the GNU configure scripts do the above better, for Unix systems, than Ghostscript's current methods. While this may be true, we have found configure scripts difficult to write, understand, and maintain; and the autoconf tool for generating configure scripts, which we found easy to use, doesn't cover much of the ground that Ghostscript requires.

Cross-platform APIs
^^^^^^^^^^^^^^^^^^^^^^^^^^

For a few library facilities that are available on all platforms but are not well standardized, or that may need to be changed for special environments, Ghostscript defines its own APIs. It is an architectural property of Ghostscript that the implementations of these APIs are the only .c files for which the choice of platform (as opposed to choices of drivers or optional features) determines whether they are compiled and linked into an executable.

API:
   base/gp.h, base/gpcheck.h, base/gpgetenv.h, base/gpmisc.h, base/gpsync.h.


Implementation files shared among multiple platforms:
   base/gp_getnv.c, base/gp_mktmp.c, base/gp_nsync.c, base/gp_paper.c, base/gp_psync.c, base/gp_strdl.c, base/gpmisc.c.


Platform-specific implementation files:
   base/gp_dosfe.c, base/gp_dosfs.c, base/gp_dvx.c, base/gp_msdos.c, base/gp_mshdl.c, base/gp_mslib.c, base/gp_mswin.c, base/gp_mswin.h, base/gp_ntfs.c, base/gp_os2.c, base/gp_os2.h, base/gp_os2fs.c, base/gp_os9.c, base/gp_stdia.c, base/gp_stdin.c, base/gp_unifn.c, base/gp_unifs.c, base/gp_unix.c, base/gp_unix_cache.c, base/gp_upapr.c, base/gp_vms.c, base/gp_wgetv.c, base/gp_win32.c, base/gp_wpapr.c, base/gp_wsync.c, base/gs_dll_call.h.


Makefiles
^^^^^^^^^^^^^^^^^^^^^^^^^^

For information on the structure and conventions used within makefiles, see the `Makefile structure`_ section above.

Ghostscript's makefiles are structured very similarly to the cross-platform library files. The great majority of the makefiles are portable across all platforms and all versions of make. To achieve this, the platform-independent makefiles must obey two constraints beyond those of the POSIX make program:

- No conditionals or includes are allowed. While most make programs now provide some form of conditional execution and some form of inclusion, there is no agreement on the syntax. (Conditionals and includes are allowed in platform-dependent makefiles; in fact, an inclusion facility is required.)
- There must be a space on both sides of the : that separates the target of a rule from its dependencies. This is required for compatibility with the OpenVMS MMS and MMK programs.


The top-level makefile for each platform (where "platform" includes the OS, the compiler, and the flavor of make) contains all the build options, plus includes for the generic makefiles and any platform-dependent makefiles that are shared among multiple platforms.

While most of the top-level makefiles build a PostScript and/or PDF interpreter configuration, there are also a few makefiles that build a test program that only uses the graphics library without any language interpreter. Among other things, this can be helpful in verifying that no accidental dependencies on the interpreter have crept into the library or drivers.

For families of similar platforms, the question arises whether to use multiple top-level makefiles, or whether to use a single top-level makefile that may require minor editing for some (or all) platforms. Ghostscript currently uses the following top-level makefiles for building interpreter configurations:


- POSIX systems (inluding Linux and Unix):
   - GNU Autoconf source script for automatic build configuration.
   - Makefile.in, source for the top-level makefile used in the Autoconf build.
   - base/unix-gcc.mak, for Unix with gcc.
   - base/unixansi.mak, for Unix with an ANSI C compiler other than gcc.
- PC:
   - ghostscript.vcproj, Visual Studio project file used to build Ghostscript.
   - psi/msvc32.mak, for MS Windows with Microsoft Visual C (MSVC).
   - psi/os2.mak, for MS-DOS or OS/2 GCC/EMX environment.
- Macintosh:
   - base/macosx.mak, commandline makefile for MacOS X.
   - base/macos-mcp.mak, dummy makefile to generate an xml project file for Metrowerks Codewarrior.
- Other:
   - base/all-arch.mak, for building on many Unix systems in a networked test environment.
   - base/openvms.mak, for OpenVMS with Digital's CC compiler and the MMS build program.
   - base/openvms.mmk, for OpenVMS with Digital's CC compiler and the MMK build program.

The following top-level makefiles build the library test program:

- base/ugcclib.mak, on Unix with gcc.
- base/msvclib.mak, on MS Windows with MSVC.

The MSVC makefiles may require editing to select between different versions of MSVC, since different versions may have slightly incompatible command line switches or customary installation path names. The Unix makefiles often require editing to deal with differing library path names and/or library names. For details, see the :ref:`Unix section of the documentation<MakeHowToBuildForUnix>` for building Ghostscript.

Library test program:
   base/gslib.c.

Platform-independent makefiles:
   Graphics library and support:
      devices/contrib.mak, devices/devs.mak, base/gs.mak, base/lib.mak, base/version.mak.

   PostScript interpreter and fonts:
      psi/int.mak.

   Third-party libraries:
      base/expat.mak, base/ijs.mak, base/jbig2.mak, base/ldf_jb2.mak, base/lwf_jp2.mak, base/jpeg.mak, base/png.mak, base/zlib.mak.


Shared platform-dependent makefiles:
   Unix:
      base/unix-aux.mak, base/unix-dll.mak, base/unix-end.mak, base/unixhead.mak, base/unixinst.mak, base/unixlink.mak.

   Microsoft Windows and MS-DOS:
      base/msvccmd.mak, base/msvctail.mak, base/pcwin.mak, psi/winint.mak, base/winlib.mak, base/winplat.mak.


Coding standards
"""""""""""""""""""

Coding for portability requires avoiding both explicit dependencies, such as platform-dependent ``#ifdefs``, and implicit dependencies, such as dependencies on byte order or the size of the integral types.

Explicit dependencies
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

The platform-independent .c files never, ever, use ``#ifdef`` or ``#if`` to select code for specific platforms. Instead, we always try to characterize some abstract property that is being tested. For example, rather than checking for macros that are defined on those specific platforms that have 64-bit long values, we define a macro ARCH_SIZEOF_LONG that can then be tested. Such macros are always defined in a .h file, either automatically in arch.h, or explicitly in a ``xxx_.h`` file, as described in earlier sections.

Files:
   base/std.h, base/stdpn.h, base/stdpre.h.

Implicit dependencies
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

The most common source of byte ordering dependencies is casting between types (``T1 *``) and (``T2 *``) where ``T1`` and ``T2`` are numeric types that aren't merely signed/unsigned variants of each other. To avoid this, the only casts allowed in the code are between numeric types, from a pointer type to a long integral type, and between pointer types.

Ghostscript's code assumes the following about the sizes of various types:

char
   8 bits

short
   16 bits

int
   32 or 64 bits

long
   32 or 64 bits

float
   32 bits (may work with 64 bits)

double
   64 bits (may work with 128 bits)

The code does not assume that the char type is signed (or unsigned); except for places where the value is always a literal string, or for interfacing to library procedures, the code uses ``byte`` (a Ghostscript synonym for ``unsigned char``) almost everywhere.

Pointers are signed on some platforms and unsigned on others. In the few places in the memory manager where it's necessary to reliably order-compare (as opposed to equality-compare) pointers that aren't known to point to the same allocated block of memory, the code uses the PTR_relation macros rather than direct comparisons.

See the files listed above for other situations where a macro provides platform-independence or a workaround for bugs in specific compilers or libraries (of which there are a distressing number).

Platform-specific code
""""""""""""""""""""""""""""""""

There are some features that are inherently platform-specific:

- Microsoft Windows requires a lot of special top-level code, and also has an installer and uninstaller.
- OS/2 requires a little special code.
- MacOS also requires special top-level code (now distributed with the standard Ghostscript package).
- All platforms supporting DLLs (currently all three of the above) share some special top-level code.


MS Windows files:
   psi/dpmain.c, psi/dwdll.c, psi/dwdll.h, psi/dwimg.c, psi/dwimg.h, psi/dwmain.c, psi/dwmainc.c, psi/dwnodll.c, psi/dwreg.c, psi/dwreg.h, psi/dwres.h, psi/dwtext.c, psi/dwtext.h, psi/dwtrace.c, psi/dwtrace.h, base/gp_msdll.c, base/gp_mspol.c, base/gp_msprn.c, base/gsdllwin.h.

OS/2 files:
   base/gp_os2pr.c,

Unix files:
   psi/dxmain.c, psi/dxmainc.c.

Macintosh files:
   devices/gdevmac.c, devices/gdevmac.h, devices/gdevmacpictop.h, devices/gdevmacttf.h, base/gp_mac.c, base/gp_mac.h, base/gp_macio.c, base/gp_macpoll.c, base/gsiomacres.c, base/macgenmcpxml.sh, base/macsystypes.h, base/macos_carbon_pre.h, base/macos_carbon_d_pre.h, base/macos_classic_d_pre.h, psi/dmmain.c, psi/dmmain.r.

VMS files:
   base/vms_x_fix.h.

DLL files:
   psi/gsdll.c, base/gsdll.h, devices/gdevdsp.c, devices/gdevdsp.h, devices/gdevdsp2.h, psi/iapi.c, psi/iapi.h, psi/idisp.c, psi/idisp.h.
   The new DLL interface (new as of 7.0) is especially useful with the new display device, so it is included here. Both are due to Russell Lang.

Troubleshooting
-------------------------

The Ghostscript code has many tracing and debugging features that can be enabled at run time using the -Z command line switch, if the executable was compiled with ``DEBUG`` defined. One particularly useful combination is ``-Z@\?``, which fills free memory blocks with a pattern and also turns on run-time memory consistency checking. For more information, see doc/Use.html#Debugging; you can also search for occurrences of ``if_debug`` or ``gs_debug_c`` in the source code. Note that many of these features are in the graphics library and do not require a PostScript interpreter.

The code also contains many run-time procedures whose only purpose is to be called from the debugger to print out various data structures, including all the procedures in psi/idebug.c (for the PostScript interpreter) and the ``debug_dump_`` procedures in base/gsmisc.c.

Files:
   doc/Use.html#Debugging, base/gdebug.h, base/gsmdebug.h, psi/idebug.h, psi/idebug.c.


Profiling
-------------------------


Profiling with Microsoft Developer Studio 6
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The Microsoft profiling tool is included into Microsoft Developer Studio 6 Enterprise Edition only. Standard Edition and Professional Edition do not include it.

Microsoft profiler tool requires the application to be linked with a special linker option. To provide it you need the following change to gs/base/msvccmd.mak:


.. code-block:: bash

   *** SVN-GS\HEAD\gs\src\msvccmd.mak    Tue Jan  9 21:41:07 2007
   --- gs\src\msvccmd.mak    Mon May  7 11:29:35 2007
   ***************
   *** 159,163 ****
     # Note that it must be followed by a space.
     CT=/Od /Fd$(GLOBJDIR) $(NULL) $(CDCC) $(CPCH)
   ! LCT=/DEBUG /INCREMENTAL:YES
     COMPILE_FULL_OPTIMIZED=    # no optimization when debugging
     COMPILE_WITH_FRAMES=    # no optimization when debugging
   --- 159,164 ----
     # Note that it must be followed by a space.
     CT=/Od /Fd$(GLOBJDIR) $(NULL) $(CDCC) $(CPCH)
   ! # LCT=/DEBUG /INCREMENTAL:YES
   ! LCT=/DEBUG /PROFILE
     COMPILE_FULL_OPTIMIZED=    # no optimization when debugging
     COMPILE_WITH_FRAMES=    # no optimization when debugging
   ***************
   *** 167,175 ****
     !if $(DEBUGSYM)==0
     CT=
   ! LCT=
     CMT=/MT
     !else
     CT=/Zi /Fd$(GLOBJDIR) $(NULL)
   ! LCT=/DEBUG
     CMT=/MTd
     !endif
   --- 168,178 ----
     !if $(DEBUGSYM)==0
     CT=
   ! # LCT=
   ! LCT=/PROFILE
     CMT=/MT
     !else
     CT=/Zi /Fd$(GLOBJDIR) $(NULL)
   ! # LCT=/DEBUG
   ! LCT=/DEBUG /PROFILE
     CMT=/MTd
     !endif


.. note ::

   Any of debug and release build may be profiled.

Microsoft Profiler tool can't profile a dynamically loaded DLLs. When building Ghostscript with makefiles you need to specify ``MAKEDLL=0`` to nmake command line.

The Integrated Development Environment of Microsoft Developer Studio 6 cannot profile a makefile-based project. Therefore the profiling tool to be started from command line.

The profiling from command line is a 4 step procedure. The following batch file provides a sample for it :


.. code-block:: bash

   set DEVSTUDIO=G:\Program Files\Microsoft Visual Studio
   set GS_HOME=..\..\gs-hdp
   set GS_COMMAND_LINE=%GS_HOME%\bin\gswin32c.exe -I%GS_HOME%\lib;f:\afpl\fonts -r144 -dBATCH -dNOPAUSE -d/DEBUG attachment.pdf
   set START_FUNCTION=_main
   set Path=%DEVSTUDIO%\Common\MSDev98\Bin;%DEVSTUDIO%\VC98\Bin
   PREP.EXE /OM /SF %START_FUNCTION% /FT   %GS_HOME%\bin\gswin32c.exe
   If ERRORLEVEL 1 echo step 1 fails&exit
   PROFILE  /I %GS_HOME%\bin\gswin32c.pbi  %GS_COMMAND_LINE%
   If ERRORLEVEL 1 echo step 2 fails&exit
   PREP /M %GS_HOME%\bin\gswin32c /OT xxx.pbt
   If ERRORLEVEL 1 echo step 3 fails&exit
   PLIST /ST xxx.pbt >profile.txt
   If ERRORLEVEL 1 echo step 4 fails&exit

This batch file to be adopted to your configuration :

- Edit the path to developer studio in the line 1.
- Edit the Ghostscript home directory in the line 2.
- Edit Ghostscript command line in line 3. Note that profiling without ``/NOPAUSE`` is a bad idea.
- In the line 4 edit the function name to start the profiling from. The sample code specifies ``_main`` as the starting function. Note it is the linker's function name, which starts with underscore.
- Edit the output file name in the line 5.




.. include:: footer.rst