.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: PostScript Files Distributed with Ghostscript

.. include:: header.rst

.. _Psfiles.html:


PostScript Files Distributed with Ghostscript
================================================




Generally used system files
--------------------------------

``gs_*_e.ps``
~~~~~~~~~~~~~~~~~~~~~
   These files define the Encodings known to Ghostscript. All of them except ``gs_std_e.ps`` and ``gs_il1_e.ps`` are loaded only if referred to. However some are additionally built into ``gscencs.c``.

PostScript Encodings
""""""""""""""""""""""""

These files are found in the ``lib`` subdirectory of the Ghostscript source distribution.

   ``gs_ce_e.ps``

These files are found in the ``Resource/Init`` subdirectory of the Ghostscript source distribution.

   ``gs_il1_e.ps``, ``gs_std_e.ps``, ``gs_sym_e.ps``

PDF Encodings
""""""""""""""""""""""""

These files are found in the ``Resource/Init`` subdirectory of the Ghostscript source distribution.

   ``gs_mex_e.ps``, ``gs_mro_e.ps``, ``gs_pdf_e.ps``, ``gs_wan_e.ps``

Non-standard Encodings
""""""""""""""""""""""""

These files are found in the ``Resource/Init`` subdirectory of the Ghostscript source distribution.

   ``gs_dbt_e.ps``

These files are found in the ``lib`` subdirectory of the Ghostscript source distribution.

   ``gs_il2_e.ps``, ``gs_ksb_e.ps``, ``gs_lgo_e.ps``, ``gs_lgx_e.ps``, ``gs_wl1_e.ps``, ``gs_wl2_e.ps``, ``gs_wl5_e.ps``


Pseudo-encodings
""""""""""""""""""""""""

These files are found in the ``Resource/Init`` subdirectory of the Ghostscript source distribution.
   ``gs_mgl_e.ps``

These files are found in the ``lib`` subdirectory of the Ghostscript source distribution.

   ``gs_lgo_e.ps``, ``gs_lgx_e.ps``


Other files
~~~~~~~~~~~~~~~~~~~~~

These files are found in the ``Resource/Init`` subdirectory of the Ghostscript source distribution.

``gs_btokn.ps``
   ``gs_init.ps`` reads this in if the btoken feature is included in the configuration. It provides support for binary tokens.

``gs_cff.ps``
   Load CFF (compressed) fonts.

``gs_fntem.ps``
   Code for emulating PostScript fonts with non-PostScript font technologies.

``gs_cidtt.ps``
   Code for emulating ``CID`` fonts with TrueType fonts.

``gs_cidcm.ps``
   Code for recognizing font names of the form ``CIDFont-CMap`` (or ``CIDFont--CMap``) and creating the font automatically.

``gs_ciddc.ps``
   Defines Decoding and ``CIDDecoding`` resource categories and related procsets. Used for for emulating PostScript fonts with non-PostScript font technologies.

``gs_cidfm.ps``
   Provides resource mapping for ``CIDFont`` category.

``gs_cidfn.ps``
   ``ProcSet`` for implementing ``CIDFont`` and ``CIDMap`` resources.

``gs_cmap.ps``
   ``ProcSet`` for implementing ``CMap`` resources.

``gs_cspace.ps``
   PostScript portion of the basic color space handling; see the extensive comment at the head of the file for information.

``gs_dscp.ps``
   Code to compensate for badly written PostScript files by setting ``Orientation`` according to the DSC comments.

``gs_epsf.ps``
   Allow the interpreter to recognize DOS EPSF file headers, and skip to the PostScript section of the file.

``gs_fapi.ps``
   :ref:`Font API<Fonts FAPI>` support.

``gs_fonts.ps``
   ``gs_init.ps`` reads this in. It initializes Ghostscript's font machinery and provides some utility procedures that work with fonts.

``gs_frsd.ps``
   Support for the PostScript LanguageLevel 3 ``ReusableStreamDecode`` filter.

``gs_img.ps``
   Implementation of the traditional (non-dictionary) form of the image and imagemask operators, and the colorimage operator (including the Next alphaimage facility).

``gs_init.ps``
   Ghostscript reads this automatically when it starts up. It contains definitions of many standard procedures and initialization for a wide variety of things.

``gs_lev2.ps``
   ``gs_init.ps`` reads this in if the Ghostscript interpreter includes Level 2 PostScript functions. It contains definitions of procedures and miscellaneous initialization for the Level 2 functions.

``gs_ll3.ps``
   Initialize PostScript LanguageLevel 3 functions.

``gs_resmp.ps``
   A ``procset`` for redefining resource categories with a resource map.

``gs_res.ps``
   ``gs_init.ps`` reads this in if the Level 2 resource machinery is included. Currently, this is the case for all Level 2 configurations.

``gs_setpd.ps``
   Implementation of the ``setpagedevice`` operator.

``gs_statd.ps``
   ``gs_init.ps`` reads this in. It creates a dummy ``statusdict`` and some other environmental odds and ends for the benefit of PostScript files that really want to be printed on a LaserWriter.

``gs_trap.ps``
   Stub support for the PostScript LanguageLevel 3 "In-RIP trapping" feature.

``gs_ttf.ps``
   Support code for direct use of TrueType fonts.

``gs_typ32.ps``
   Initialization file for Type 32 fonts.

``gs_typ42.ps``
   Support code for Type 42 fonts (TrueType font in a PostScript "wrapper").

``gs_type1.ps``
   ``gs_init.ps`` reads this in if the Ghostscript interpreter includes Type 1 font capability (which it normally does).




Configuration files
------------------------

These files are found in the ``Resource/Init`` subdirectory of the Ghostscript source distribution. Users are allowed to modify them to configure Ghostscript.

``Fontmap``
   Font mapping table.

``cidfmap``
   CID font mapping table. Allows substitution of a CID font for another CID font or a TrueType font for a CID font.

``FAPIconfig``
   A configuration file for Font API client.

``FAPIfontmap``
   Font mapping table for Font-API-handled fonts.

``FAPIcidfmap``
   Font mapping table for Font-API-handled CID fonts.



More obscure system files
------------------------------------------------

Unless otherwise stated, these files are found in the ``Resource/Init`` subdirectory of the Ghostscript source distribution.

``gs_agl.ps``
   Contains the mapping from Adobe glyph names to Unicode values, used to support TrueType fonts and disk-based Type 1 fonts.

``gs_cet.ps``
   Sets a number of alternate defaults to make Ghostscript behave more like Adobe CPSI. Useful for running the CET conformance test suite.

``gs_diskn.ps``
   This file implements the ``%disk IODevice`` (``diskn.dev`` feature). See the :ref:`language documentation<Language.html>` for information on the use of the ``%disk#`` devices. These PostScript modifications primarily perform the searching of all ``Searchable`` file systems in a defined ``SearchOrder`` when a file reference does not contain an explicit ``%device%`` specifier (such as ``%os%`` or ``%disk0%``). This is required to emulate undocumented behaviour of Adobe PostScript printers that have a disk and was experimentally determined.

``gs_kanji.ps``
   This file provides support for the Wadalab free Kanji font. It is not included automatically in any configuration. This file is stored in the ``lib`` subdirectory.

``gs_pdfwr.ps``
   This file contains some patches for providing information to the :title:`pdfwrite` driver. It is included only if the :title:`pdfwrite` driver is included.

``ht_ccsto.ps``
   A default stochastic CMYK halftone. This file is in the public domain. This file is stored in the ``lib`` subdirectory.

``stcolor.ps``
   Configure the (Epson) :title:`stcolor` driver. This file is stored in the ``lib`` subdirectory.


PDF-specific system files
------------------------------------------------

These files are found in the ``Resource/Init`` subdirectory of the Ghostscript source distribution.

``pdf_base.ps``
   Utilities for interpreting PDF objects and streams.

``pdf_draw.ps``
   The interpreter for drawing-related PDF operations.

``pdf_font.ps``
   Code for handling fonts in PDF files.

``pdf_main.ps``
   Document- and page-level control for interpreting PDF files.

``pdf_ops.ps``
   Definitions for most of the PDF operators.

``pdf_rbld.ps``
   Contains procedures for bebuilding damaged PDF files.

``pdf_sec.ps``
   PDF security (encryption) code.

``gs_icc.ps``
   Support for ICC color profiles. These are not a standard PostScript feature, but are used in the PDF interpreter, as ICC profiles may be embedded in PDF files.


These files are found in the lib subdirectory of the Ghostscript source distribution. These files are templates and should not be used without modification.

``PDFX_def.ps``
   This is a sample prefix file for creating a PDF/X-3 document with the :title:`pdfwrite` device.

``PDFA_def.ps``
   This is a sample prefix file for creating a PDF/A document with the :title:`pdfwrite` device.



Display PostScript-specific system files
------------------------------------------------

These files are found in the ``Resource/Init`` subdirectory of the Ghostscript source distribution.

``gs_dpnxt.ps``
   NeXT Display PostScript extensions.

``gs_dps.ps``, ``gs_dps1.ps``, ``gs_dps2.ps``
   ``gs_init.ps`` reads these in if the dps feature is included in the configuration. They provide support for various Display PostScript and Level 2 features.



Art and examples
------------------------------------------------

These files are found in the ``examples`` subdirectory of the Ghostscript source distribution.

``alphabet.ps``
   Prints a sample alphabet at several different sizes.

``annots.pdf``
   A sample file with a wide variety of PDF "annotations".

``colorcir.ps``
   A set of nested ellipses made up of colored bars.

``doretree.ps``
   A 3-D image produced by a modeling program. This file is in the public domain.

``escher.ps``
   A colored version of a hexagonally symmetric Escher drawing of interlocking butterflies. Can be printed on monochrome devices, with somewhat less dramatic results.

``golfer.eps``
   A gray-scale picture of a stylishly dressed woman swinging a golf club.

``grayalph.ps``
   Grayscaled text test pattern.

``ridt91.eps``
   The RIDT '91 logo. Note that since this is an EPS file, you will have to add ``-c showpage`` at the end of the command line to print it or convert it to a raster file.

``snowflak.ps``
   A rectangular grid of intricate colored snowflakes. (May render very slowly.)

``text_graph_image_cmyk_rgb.pdf``
   A simple PDF containing text and graphics in both RGB and CMYK spaces.

``text_graphic_image.pdf``
   A simple PDF containing text and graphics in RGB space.

``tiger.eps``
   A dramatic colored picture of a tiger's head.

``transparency_example.ps``
   A simple example of transparency.

``vasarely.ps``
   Colored rectangles and ellipses inspired by Victor Vasarely's experiments with tilting circles and squares.

``waterfal.ps``
   Prints text in a variety of different sizes, to help evaluate the quality of text rendering.



Utilities
----------------------------

For more information on these utility programs, see the comments at the start of each file . The ones marked (``*``) have batch files or shell scripts of the same name (like ``bdftops`` and ``bdftops.bat``) to invoke them conveniently.

These files are found in the ``lib`` subdirectory of the Ghostscript source distribution.

``align.ps``
   A test page for determining the proper margin and offset parameters for your printer.

``caption.ps``
   A file for putting a caption in a box at the bottom of each page, useful for trade show demos.

``cat.ps``
   Appends one file to another. Primarily used to overcome the 'copy' limitation of Windows command shell for ``ps2epsi``.

``cid2code.ps``
   A utility for creating maps from CIDs to Unicode, useful when substituting a TrueType font for an Adobe font.

``docie.ps``
   An emulation of the CIE color mapping algorithms.

``font2pcl.ps``
   A utility to write a font as a PCL bitmap font.

``gslp.ps``
   A utility for doing "line printing" of plain text files.

``gsnup.ps``
   A file that you can concatenate in front of (very well-behaved) PostScript files to do N-up printing. It is deliberately simple and naive: for more generality, use ``psnup`` (which, however, requires DSC comments).

``jispaper.ps``
   A file that makes the b0 through b6 procedures refer to JIS B paper sizes rather than ISO B.

``landscap.ps``
   A file that you can put in front of your own files to get them rendered in landscape mode.

``mkcidfm.ps``
   A utility for creating a CID font mapping table cidfmap from fonts found in a specified directory.

``pdf2dsc.ps``
   A utility to read a PDF file and produce a DSC "index" file.

``pf2afm.ps``
   A utility for producing AFM files from PFA, PFB, and optionally PFM files.

``pfbtopfa.ps``
   A utility to convert PFB (binary) font files to PFA (text) format.

``prfont.ps``
   A utility to print a font catalog.

``printafm.ps``
   A utility to print an AFM file on standard output.

``ps2ai.ps``
   A utility for converting an arbitrary PostScript file into a form compatible with Adobe Illustrator. NOTE: ``ps2ai`` doesn't work properly with Adobe's Helvetica-Oblique font, and other fonts whose original ``FontMatrix`` involves skewing or rotation.

``ps2epsi.ps``
   A utility for converting an arbitrary PostScript file into EPSI form.

``rollconv.ps``
   A utility for converting files produced by Macromedia's Rollup program to a Type 0 form directly usable by Ghostscript.

``stocht.ps``
   A file that installs the ``StochasticDefault`` halftone as the default, which may improve output quality on inkjet printers. See the file for more information.

``viewcmyk.ps``
   A utility for displaying CMYK files.

``viewgif.ps``
   A utility for displaying GIF files.

``viewjpeg.ps``
   A utility for displaying JPEG files.

``viewmiff.ps``
   A utility for displaying MIFF files.

``viewpbm.ps``
   A utility for displaying PBM/PGM/PPM files.

``viewpcx.ps``
   A utility for displaying PCX files.

``viewrgb.ps``
   A utility for displaying files created by ``-sDEVICE=bitrgb``.

``viewraw.ps``
   An extended utility for displaying files created by ``-sDEVICE=bitrgb``.


Development tools
----------------------------

These files are found in the ``lib`` subdirectory of the Ghostscript source distribution.

``acctest.ps``
   A utility that checks whether the interpreter enforces access restrictions.

``image-qa.ps``
   A comprehensive test of the image display operators.

``ppath.ps``
   A couple of utilities for printing out the current path, for debugging.

``pphs.ps``
   A utility to print the Primary Hint Stream of a linearized PDF file.

``traceimg.ps``
   Trace the data supplied to the image operator.

``traceop.ps``
   A utility for tracing uses of any procedure or operator for debugging.

``uninfo.ps``
   Some utilities for printing out PostScript data structures.

``viewps2a.ps``
   A utility for displaying the output of ``ps2ascii.ps``.

``winmaps.ps``
   A utility for creating mappings between the Adobe encodings and the Microsoft Windows character sets.

``zeroline.ps``
   A utility for testing how interpreters handle zero-width lines.



Odds and ends
----------------------------

These files are found in the ``lib`` subdirectory of the Ghostscript source distribution.

``jobseparator.ps``
   Convenience file containing a job separator sequence for use when passing files with ``-dJOBSERVER``.

``lines.``
   A test program for line joins and caps.

``stcinfo.ps``
   Print and show parameters of the (Epson) :title:`stcolor` driver.




.. include:: footer.rst



