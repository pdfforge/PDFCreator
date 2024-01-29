.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: Using Ghostscript


.. include:: header.rst

.. _Use.html:


Using
===================================


This document describes how to use the command line Ghostscript client. Ghostscript is also used as a general engine inside other applications (for viewing files for example). Please refer to the documentation for those applications for using Ghostscript in other contexts.


Invoking Ghostscript
------------------------------------------


The command line to invoke Ghostscript is essentially the same on all systems, although the name of the executable program itself may differ among systems. For instance, to invoke Ghostscript on unix-like systems type:

.. code-block:: bash

   gs [options] {filename 1} ... [options] {filename N} ...

Here are some basic examples. The details of how these work are described below.

To view a file
~~~~~~~~~~~~~~~~~~

.. code-block:: bash

   gs -dSAFER -dBATCH document.pdf


.. note::

      You'll be prompted to press return between pages.

To convert a figure to an image file
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: bash

   gs -dSAFER -dBATCH -dNOPAUSE -sDEVICE=png16m -dGraphicsAlphaBits=4 \
      -sOutputFile=tiger.png tiger.eps


To render the same image at 300 dpi
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: bash

   gs -dSAFER -dBATCH -dNOPAUSE -sDEVICE=png16m -r300 \
                   -sOutputFile=tiger_300.png tiger.eps


To render a figure in grayscale
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: bash


   gs -dSAFER -dBATCH -dNOPAUSE -sDEVICE=pnggray -sOutputFile=figure.png figure.pdf


To rasterize a whole document
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: bash

   gs -dSAFER -dBATCH -dNOPAUSE -sDEVICE=pgmraw -r150 \
                   -dTextAlphaBits=4 -sOutputFile='paper-%00d.pgm' paper.ps


Convert a PostScript document to PDF
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: bash

   ps2pdf file.ps


The output is saved as ``file.pdf``.



.. note::

   There are other utility scripts besides ``ps2pdf``, including ``pdf2ps``, ``ps2epsi``, ``pdf2dsc``, ``ps2ascii``, ``ps2ps`` and ``ps2ps2``. These just call Ghostscript with the appropriate (if complicated) set of options. You can use the 'ps2' set with eps files.

   Ghostscript is capable of interpreting PostScript, encapsulated PostScript (EPS), DOS EPS (EPSF), and Adobe Portable Document Format (PDF). The interpreter reads and executes the files in sequence, using the method described under "`File searching`_" to find them.

   The interpreter runs in interactive mode by default. After processing the files given on the command line (if any) it reads further lines of PostScript language commands from the primary input stream, normally the keyboard, interpreting each line separately. To quit the interpreter, type "``quit``". The ``-dBATCH -dNOPAUSE`` options in the examples above disable the interactive prompting. The interpreter also quits gracefully if it encounters end-of-file or *control-C*.

   The interpreter recognizes many options. An option may appear anywhere in the command line, and applies to all files named after it on the line. Many of them include "=" followed by a parameter. The most important are described in detail here. Please see the reference sections on `Command line options`_ and `Devices <Devices.html>`_ for a more complete listing.



Help at the command line: gs -h
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

You can get a brief help message by invoking Ghostscript with the ``-h`` or ``-?`` switch, like this:


.. code-block:: bash

   gs -h
   gs -?


The message shows for that version of the Ghostscript executable:

- the version and release information.
- the general format of the command line.
- a few of the most useful options.
- the formats it can interpret.
- the available output devices.
- the search path.
- the bug report address.


On other systems the executable may have a different name:



.. list-table::
   :widths: 50 50
   :header-rows: 1

   * - System
     - Invocation Name
   * - Unix
     - gs
   * - VMS
     - gs
   * - MS Windows 95 and later
     - gswin32.exe

       gswin32c.exe

       gswin64.exe

       gswin64c.exe

   * - OS/2
     - gsos2


On Windows, the two digit number indicates the word length of the system for which the binary was built (so ``gswin32.exe`` is for x86 Windows systems, whilst ``gswin64.exe`` is for x86_64 Windows systems). And the "c" suffix indicates a Windows console based binary (note that the "display device" window will still appear).




Selecting an output device
------------------------------------------


Ghostscript has a notion of 'output devices' which handle saving or displaying the results in a particular format. Ghostscript comes with a diverse variety of such devices supporting vector and raster file output, screen display, driving various printers and communicating with other applications.

The command line option ``'-sDEVICE=device'`` selects which output device Ghostscript should use. If this option isn't given the default device (usually a display device) is used. Ghostscript's built-in help message (``gs -h``) lists the available output devices. For complete description of the devices distributed with Ghostscript and their options, please see the `Devices <Devices.html>`_  section of the documentation.

Note that this switch must precede the name of the first input file, and only its first use has any effect. For example, for printer output in a configuration that includes an Epson printer driver, instead of just ``'gs myfile.ps'`` you might use:


.. code-block:: bash

   gs -sDEVICE=epson myfile.ps





The output device can also be set through the ``GS_DEVICE`` environment variable.

Once you invoke Ghostscript you can also find out what devices are available by typing ``'devicenames =='`` at the interactive prompt. You can set the output device and process a file from the interactive prompt as well:

.. code-block:: bash

   (epson) selectdevice
   (myfile.ps) run

All output then goes to the Epson printer instead of the display until you do something to change devices. You can switch devices at any time by using the ``selectdevice`` procedure, for instance like one of these:

.. code-block:: bash

   (x11alpha) selectdevice
   (epson) selectdevice



Output resolution
~~~~~~~~~~~~~~~~~~~

Some printers can print at several different resolutions, letting you balance resolution against printing speed. To select the resolution on such a printer, use the ``-r`` switch:


.. code-block:: bash

   gs -sDEVICE=printer -rXRESxYRES


where ``XRES`` and ``YRES`` are the requested number of dots (or pixels) per inch. Where the two resolutions are same, as is the common case, you can simply use ``-rres``.

The ``-r`` option is also useful for controlling the density of pixels when rasterizing to an image file. It is used this way in the examples at the beginning of this document.


Output to files
~~~~~~~~~~~~~~~~

Ghostscript also allows you to control where it sends its output. With a display device this isn't necessary as the device handles presenting the output on screen internally. Some specialized printer drivers operate this way as well, but most devices are general and need to be directed to a particular file or printer.

To send the output to a file, use the ``-sOutputFile=`` switch or the :ref:`-o switch <O switch>` (below). For instance, to direct all output into the file ``ABC.xyz``, use:

.. code-block:: bash

   gs -sOutputFile=ABC.xyz


When printing on MS Windows systems, output normally goes directly to the printer, ``PRN``. On Unix and VMS systems it normally goes to a temporary file which is sent to the printer in a separate step. When using Ghostscript as a file rasterizer (converting PostScript or PDF to a raster image format) you will of course want to specify an appropriately named file for the output.

Ghostscript also accepts the special filename '``-``' which indicates the output should be written to standard output (the command shell).

Be aware that filenames beginning with the character % have a special meaning in PostScript. If you need to specify a file name that actually begins with %, you must prepend the ``%os%`` filedevice explicitly. For example to output to a file named ``%abc``, you need to specify:


.. code-block:: bash

   gs -sOutputFile=%os%%abc


Please see `Ghostscript and the PostScript Language <Language.html>`_ and the PostScript Language Reference Manual for more details on ``%`` and filedevices.


.. note::

   On MS Windows systems, the ``%`` character also has a special meaning for the command processor (shell), so you will have to double it, e.g.:

   .. code-block:: bash

      gs -sOutputFile=%%os%%%%abc


Note, some devices (e.g. :title:`pdfwrite`, :title:`ps2write`) only write the output file upon exit, but changing the OutputFile device parameter will cause these devices to emit the pages received up to that point and then open the new file name given by OutputFile.

For example, in order to create two PDF files from a single invocation of Ghostscript the following can be used:

.. code-block:: bash

   gs -sDEVICE=pdfwrite -o tiger.pdf examples/tiger.eps -c "<< /OutputFile (colorcir.pdf) >> setpagedevice" -f examples/colorcir.ps



.. _Use_OnePagePerFile:


One page per file
""""""""""""""""""""


Specifying a single output file works fine for printing and rasterizing figures, but sometimes you want images of each page of a multi-page document. You can tell Ghostscript to put each page of output in a series of similarly named files. To do this place a template ``'%d'`` in the filename which Ghostscript will replace with the page number.

Note: Since the % character is used to precede the page number format specification, in order to represent a file name that contains a %, double % characters must be used. For example for the file ``my%foo`` the OutputFile string needs to be ``my%%foo``.

The format can in fact be more involved than a simple ``'%d'``. The format specifier is of a form similar to the C ``printf`` format. The general form supported is:

.. code-block:: bash

      %[flags][width][.precision][l]type

       where:  flags    is one of:  #+-
               type     is one of:  diuoxX


For more information, please refer to documentation on the C printf format specifications. Some examples are:

.. code-block:: bash

   -sOutputFile=ABC-%d.png
      produces 'ABC-1.png', ... , 'ABC-10.png', ..
   -sOutputFile=ABC-%03d.pgm
      produces 'ABC-001.pgm', ... , 'ABC-010.pgm', ...
   -sOutputFile=ABC_p%04d.tiff
      produces 'ABC_p0001.tiff', ... , 'ABC_p0510.tiff', ... , 'ABC_p5238.tiff'


Note, however that the one page per file feature may not supported by all devices. Also, since some devices write output files when opened, there may be an extra blank page written (:title:`pdfwrite`, :title:`ps2write`, :title:`eps2write`, :title:`pxlmono`, :title:`pxlcolor`).

As noted above, when using MS Windows console (command.com or cmd.exe), you will have to double the ``%`` character since the ``%`` is used by that shell to prefix variables for substitution, e.g.,


.. code-block:: bash

   gswin32c -sOutputFile=ABC%%03d.xyz



.. _O switch:

-o option
""""""""""""""""""

As a convenient shorthand you can use the ``-o`` option followed by the output file specification as discussed above. The ``-o`` option also sets the :ref:`-dBATCH and -dNOPAUSE options<Interaction related parameters>`. This is intended to be a quick way to invoke Ghostscript to convert one or more input files.

For instance, to convert somefile.ps to JPEG image files, one per page, use:


.. code-block:: bash

   gs -sDEVICE=jpeg -o out-%d.jpg somefile.ps

is equivalent to:

.. code-block:: bash

   gs -sDEVICE=jpeg -sOutputFile=out-%d.jpg -dBATCH -dNOPAUSE somefile.ps


.. _Use_PaperSize:


Choosing paper size
~~~~~~~~~~~~~~~~~~~~~~~


Ghostscript is distributed configured to use U.S. letter paper as its default page size. There are two ways to select other paper sizes from the command line:

If the desired paper size is listed in the section on paper sizes known to Ghostscript below, you can select it as the default paper size for a single invocation of Ghostscript by using the ``-sPAPERSIZE=`` switch, for instance:

.. code-block:: bash

   -sPAPERSIZE=a4
   -sPAPERSIZE=legal

Otherwise you can set the page size using the pair of switches:

.. code-block:: bash

   -dDEVICEWIDTHPOINTS=w -dDEVICEHEIGHTPOINTS=h


Where ``w`` be the desired paper width and ``h`` be the desired paper height in points (units of 1/72 of an inch).


Individual documents can (and often do) specify a paper size, which takes precedence over the default size. To force a specific paper size and ignore the paper size specified in the document, select a paper size as just described, and also include the :ref:`-dFIXEDMEDIA switch<FIXEDMEDIA>` on the command line.

The default set of paper sizes will be included in the ``currentpagedevice`` in the ``InputAttributes`` dictionary with each paper size as one of the entries. The last entry in the dictionary (which has numeric keys) is a non-standard (Ghostscript extension) type of PageSize where the array has four elements rather than the standard two elements. This four element array represents a page size range where the first two elements are the lower bound of the range and the second two are the upper bound. By default these are [0, 0] for the lower bound and [16#fffff, 16#fffff] for the upper bound.

The range type of PageSize is intended to allow flexible page size sepcification for non-printer file formats such as JPEG, PNG, TIFF, EPS, ...

For actual printers, either the entire ``InputAttributes`` dictionary should be replaced or the range type entry should not be included. To simplify using the default page sizes in the ``InputAttributes`` dictionary, the command line option ``-dNORANGEPAGESIZE`` can be used. Using this option will result in automatic rotation of the document page if the requested page size matches one of the default page sizes.

When the :ref:`-dFIXEDMEDIA switch<FIXEDMEDIA>` is given on the command line, the ``InputAttributes`` dictionary will only be populated with the single page size. This allows the ``-dPSFitPage`` option to fit the page size requested in a PostScript file to be rotated, scaled and centered for the best fit on the specified page.



Changing the installed default paper size
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


You can change the installed default paper size on an installed version of Ghostscript, by editing the initialization file ``gs_init.ps``. This file is usually in the ``Resource/Init`` directory somewhere in the search path. See the section on :ref:`finding files<Use_How Ghostscript finds files>` for details.

Find the line:

.. code-block:: bash

   % /DEFAULTPAPERSIZE (a4) def

Then to make A4 the default paper size, uncomment the line to change this to:

.. code-block:: bash

   /DEFAULTPAPERSIZE (a4) def

For a4 you can substitute any :ref:`paper size Ghostscript knows<Known Paper Sizes>`.

This supecedes the previous method of uncommenting the line ``% (a4) ...``.

Sometimes the initialization files are compiled into Ghostscript and cannot be changed.

On Windows and some Linux builds, the default paper size will be selected to be a4 or letter depending on the locale.




.. _Pipes:

Interacting with pipes
-----------------------------



As noted above, input files are normally specified on the command line. However, one can also "pipe" input into Ghostscript from another program by using the special file name '``-``' which is interpreted as standard input. Examples:


.. code-block:: bash

   {some program producing ps} | gs [options] -
   zcat paper.ps.gz | gs -


When Ghostscript finishes reading from the pipe, it quits rather than going into interactive mode. Because of this, options and files after the '``-``' in the command line will be ignored.

On Unix and MS Windows systems you can send output to a pipe in the same way. For example, to pipe the output to ``lpr``, use the command:


.. code-block:: bash

   gs -q -sOutputFile=- | lpr

In this case you must also use the ``-q`` switch to prevent Ghostscript from writing messages to standard output which become mixed with the intended output stream.

Also, using the ``-sstdout=%stderr`` option is useful, particularly with input from PostScript files that may print to stdout.

Similar results can be obtained with the ``%stdout`` and ``%pipe%`` filedevices. The example above would become:


.. code-block:: bash

   gs -sOutputFile=%stdout -q | lpr

or:

.. code-block:: bash

   gs -sOutputFile=%pipe%lpr


(again, doubling the ``%`` character on MS Windows systems.)

In the last case, ``-q`` isn't necessary since Ghostscript handles the pipe itself and messages sent to stdout will be printed as normal.



Using Ghostscript with PDF files
----------------------------------------------------------


Ghostscript is normally built to interpret both PostScript and PDF files, examining each file to determine automatically whether its contents are PDF or PostScript. All the normal switches and procedures for interpreting PostScript files also apply to PDF files, with a few exceptions. In addition, the ``pdf2ps`` utility uses Ghostscript to convert PDF to (Level 2) PostScript.


Switches for PDF files
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Here are some command line options specific to PDF:


``-dPDFINFO``
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

Starting with release 9.56.0 this new switch will work with the PDF interpreter (GhostPDF) and with the PDF interpreter integrated into Ghostscript. When this switch is set the interpreter will emit information regarding the file, similar to that produced by the old pdf_info.ps program in the 'lib' folder.
The format is not entirely the same, and the search for fonts and spot colours is 'deeper' than the old program; pdf_info.ps stops at the page level whereas the PDFINFO switch will descend into objects such as Forms, Images, type 3 fonts and Patterns. In addition different instances of fonts with the same name are now enumerated.

Unlike the pdf_info.ps program there is no need to add the input file to the list of permitted files for reading (using --permit-file-read).

``-dPDFFitPage``
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

Rather than selecting a PageSize given by the PDF MediaBox, BleedBox (see -``dUseBleedBox``), TrimBox (see ``-dUseTrimBox``), ArtBox (see ``-dUseArtBox``), or CropBox (see ``-dUseCropBox``), the PDF file will be scaled to fit the current device page size (usually the default page size).
This is useful for creating fixed size images of PDF files that may have a variety of page sizes, for example thumbnail images.

This option is also set by the ``-dFitPage`` option.


``-dPrinted`` & ``-dPrinted=false``
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

Determines whether the file should be displayed or printed using the "screen" or "printer" options for annotations and images. With ``-dPrinted``, the output will use the file's "print" options; with ``-dPrinted=false``, the output will use the file's "screen" options. If neither of these is specified, the output will use the screen options for any output device that doesn't have an ``OutputFile`` parameter, and the printer options for devices that do have this parameter.

``-dUseBleedBox``
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

Sets the page size to the BleedBox rather than the MediaBox. defines the region to which the contents of the page should be clipped when output in a production environment. This may include any extra bleed area needed to accommodate the physical limitations of cutting, folding, and trimming equipment. The actual printed page may include printing marks that fall outside the bleed box.

``-dUseTrimBox``
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

Sets the page size to the TrimBox rather than the MediaBox. The trim box defines the intended dimensions of the finished page after trimming. Some files have a TrimBox that is smaller than the MediaBox and may include white space, registration or cutting marks outside the CropBox. Using this option simulates appearance of the finished printed page.

``-dUseArtBox``
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

Sets the page size to the ArtBox rather than the MediaBox. The art box defines the extent of the page's meaningful content (including potential white space) as intended by the page's creator. The art box is likely to be the smallest box. It can be useful when one wants to crop the page as much as possible without losing the content.

``-dUseCropBox``
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

Sets the page size to the CropBox rather than the MediaBox. Unlike the other "page boundary" boxes, CropBox does not have a defined meaning, it simply provides a rectangle to which the page contents will be clipped (cropped). By convention, it is often, but not exclusively, used to aid the positioning of content on the (usually larger, in these cases) media.

``-sPDFPassword=password``
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

Sets the user or owner password to be used in decoding encrypted PDF files. For files created with encryption method 4 or earlier, the password is an arbitrary string of bytes; with encryption method 5 or later, it should be text in either UTF-8 or your locale's character set (Ghostscript tries both).


``-dShowAnnots=false``
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

Don't enumerate annotations associated with the page ``Annots`` key. Annotations are shown by default.

In addition, finer control is available by defining an array ``/ShowAnnotTypes``. Annotation types listed in this array will be drawn, whilst those not listed will not be drawn.

To use this feature: ``-c "/ShowAnnotTypes [....] def" -f <input file>``

Where the array can contain one or more of the following names: ``/Stamp``, ``/Squiggly``, ``/Underline``, ``/Link``, ``/Text``, ``/Highlight``, ``/Ink``, ``/FreeText``, ``/StrikeOut`` and ``/stamp_dict``.

For example, adding the follow to the command line: ``-c "/ShowAnnotTypes [/Text /UnderLine] def" -f <input file>`` would draw only annotations with the subtypes "Text" and "UnderLine".

``-dShowAcroForm=false``
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

Don't show annotations from the Interactive Form Dictionary (AcroForm dictionary). By default, AcroForm processing is now enabled because Adobe Acrobat does this. This option is provided to restore the previous behavior which corresponded to older Acrobat.

``-dNoUserUnit``
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

Ignore ``UserUnit`` parameter. This may be useful for backward compatibility with old versions of Ghostscript and Adobe Acrobat, or for processing files with large values of ``UserUnit`` that otherwise exceed implementation limits.

``-dRENDERTTNOTDEF``
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

If a glyph is not present in a font the normal behaviour is to use the /.notdef glyph instead. On TrueType fonts, this is often a hollow square. Under some conditions Acrobat does not do this, instead leaving a gap equivalent to the width of the missing glyph, or the width of the /.notdef glyph if no /Widths array is present. Ghostscript now attempts to mimic this undocumented feature using a user parameter ``RenderTTNotdef``. The PDF interpreter sets this user parameter to the value of ``RENDERTTNOTDEF`` in systemdict, when rendering PDF files. To restore rendering of /.notdef glyphs from TrueType fonts in PDF files, set this parameter to true.


These command line options are no longer specific to PDF, but have some specific differences with PDF files:


``-dFirstPage=pagenumber``
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""
Begin on the designated page of the document. Pages of all documents in PDF collections are numbered sequentionally.


``-dLastPage=pagenumber``
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""
Stop after the designated page of the document. Pages of all documents in PDF collections are numbered sequentionally.


.. note::

   The PDF and XPS interpreters allow the use of a ``-dLastPage`` less than ``-dFirstPage``. In this case the pages will be processed backwards from LastPage to FirstPage.



``-sPageList=pageranges``
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

Page ranges are separated by a comma '`,`'. Each range of pages can consist of:

- (a) a single page number.
- (b) a range with a starting page number, followed by a dash '`-`' followed by an ending page number.
- (c) a range with a starting page number, followed by a dash '`-`' which ends at the last page.
- (d) the keyword "even" or "odd", which optionally can be followed by a colon ':' and a page range. If there is no page range then all even or odd pages are processed in forward order.
- (e) a range with an initial dash '`-`' followed by and ending page number which starts at the last page and ends at the specified page (PDF and XPS only).

For example:


.. code-block:: bash

   -sPageList=1,3,5 indicates that pages 1, 3 and 5 should be processed.
   -sPageList=5-10 indicates that pages 5, 6, 7, 8, 9 and 10 should be processed.
   -sPageList=1,5-10,12- indicates that pages 1, 5, 6, 7, 8, 9, 10 and 12 onwards should be processed.
   -sPageList=odd:3-7,9-,-1,8 processes pages 3, 5, 7, 9, 10, 11, ..., last, last, last-1, ..., 1, 8


.. note::

   Use of ``PageList`` overrides ``FirstPage`` and/or ``LastPage``, if you set these as well as ``PageList`` they will be ignored.


Be aware that using the ``%d`` syntax for ``-sOutputFile=...`` does not reflect the page number in the original document. If you chose (for example) to process even pages by using ``-sPageList=even``, then the output of ``-sOutputFile=out%d.png`` would still be ``out1.png``, ``out2.png``, ``out3.png`` etc.

For PostScript or PCL input files, the list of pages must be given in increasing order, you cannot process pages out of order or repeat pages and this will generate an error. PCL and PostScript require that all the pages must be interpreted, however since only the requested pages are rendered, this can still lead to savings in time.

The PDF and XPS interpreters handle this in a slightly different way. Because these file types provide for random access to individual pages in the document these inerpreters only need to process the required pages, and can do so in any order.

Because the PostScript and PCL interpreters cannot determine when a document terminates, sending multple files as input on the command line does not reset the ``PageList`` between each document, each page in the second and subsequent documents is treated as following on directly from the last page in the first document. The PDF interpreter, however, does not work this way. Since it knows about individual PDF files the ``PageList`` is applied to each PDF file separately. So if you were to set ``-sPageList=1,2`` and then send two PDF files, the result would be pages 1 and 2 from the first file, and then pages 1 and 2 from the second file. The PostScript interpreter, by contrast, would only render pages 1 and 2 from the first file. This means you must exercise caution when using this switch, and probably should not use it at all when processing a mixture of PostScript and PDF files on the same command line.



Problems interpreting a PDF file
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


Occasionally you may try to read or print a 'PDF' file that Ghostscript doesn't recognize as PDF, even though the same file can be opened and interpreted by an Adobe Acrobat viewer. In many cases, this is because of incorrectly generated PDF. Acrobat tends to be very forgiving of invalid PDF files. Ghostscript tends to expect files to conform to the standard. For example, even though valid PDF files must begin with %PDF, Acrobat will scan the first 1000 bytes or so for this string, and ignore any preceding garbage.

In the past, Ghostscript's policy has been to simply fail with an error message when confronted with these files. This policy has, no doubt, encouraged PDF generators to be more careful. However, we now recognize that this behavior is not very friendly for people who just want to use Ghostscript to view or print PDF files. Our new policy is to try to render broken PDF's, and also to print a warning, so that Ghostscript is still useful as a sanity-check for invalid files.



PDF files from standard input
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The PDF language, unlike the PostScript language, inherently requires random access to the file. If you provide PDF to standard input using the special filename :ref:`'-'<Pipes>`, Ghostscript will copy it to a temporary file before interpreting the PDF.





Using Ghostscript with EPS files
----------------------------------------------------------

Encapsulated PostScript (EPS) files are intended to be incorporated in other PostScript documents and may not display or print on their own. An EPS file must conform to the Document Structuring Conventions, must include a ``%%BoundingBox`` line to indicate the rectangle in which it will draw, must not use PostScript commands which will interfere with the document importing the EPS, and can have either zero pages or one page. Ghostscript has support for handling EPS files, but requires that the ``%%BoundingBox`` be in the header, not the trailer. To customize EPS handling, see :ref:`EPS parameters<EPS parameters>`.

For the official description of the EPS file format, please refer to the Adobe documentation.


Using Ghostscript with overprinting and spot colors
----------------------------------------------------------

In general with PostScript and PDF interpreters, the handling of overprinting and spot colors depends upon the process color model of the :ref:`output device<Selecting an output device>`. Devices that produce gray or RGB output have an additive process color model. Devices which produce CMYK output have a subtractive process color model. Devices may, or may not, have support for spot colors.

.. note::

   The differences in appearance of files with overprinting and spot colors caused by the differences in the color model of the output device are part of the PostScript and PDF specifications. They are not due to a limitation in the implementation of Ghostscript or its output devices.


With devices which use a subtractive process color model, both PostScript and PDF allow the drawing of objects using colorants (inks) for one or more planes without affecting the data for the remaining colorants. Thus the inks for one object may overprint the inks for another object. In some cases this produces a transparency like effect. (The effects of overprinting should not be confused with the PDF 1.4 blending operations which are supported for all output devices.) Overprinting is not allowed for devices with an additive process color model. With files that use overprinting, the appearance of the resulting image can differ between devices which produce RGB output versus devices which produce CMYK output. Ghostscript automatically overprints (if needed) when the output device uses a subtractive process color model. For example, if the file is using overprinting, differences can be seen in the appearance of the output from the :ref:`tiff24nc and tiff32nc devices<Devices_TIFF>` which use an RGB and a CMYK process color models.

Most of the Ghostscript :ref:`output devices<Devices.html>` do not have file formats which support spot colors. Instead spot colors are converted using the tint transform function contained within the color space definition.. However there are several devices which have support for spot colors. The PSD format (Adobe Photoshop) produced by the :ref:`psdcmyk device<Devices_PSD>` contains both the raster data plus an equivalent CMYK color for each spot color. This allows Photoshop to simulate the appearance of the spot colors. The :ref:`display device (MS Windows, OS/2, gtk+)<Devices_Display_Device>` can be used with different color models: Gray, RGB, CMYK only, or CMYK plus spot colors (separation). The display device, when using its CMYK plus spot color (separation) mode, also uses an equivalent CMYK color to simulate the appearance of the spot color. The :ref:`tiffsep device<Devices_TIFF>` creates output files for each separation (CMYK and any spot colors present). It also creates a composite CMYK file using an equivalent CMYK color to simulate the appearance of spot colors. The :ref:`xcfcmyk device<Devices_XCF>` creates output files with spot colors placed in separate alpha channels. (The XCF file format does not currently directly support spot colors.)

Overprinting with spot colors is not allowed if the tint transform function is being used to convert spot colors. Thus if spot colors are used with overprinting, then the appearance of the result can differ between output devices. One result would be obtained with a CMYK only device and another would be obtained with a CMYK plus spot color device. In a worst case situation where a file has overprinting with both process (CMYK) and spot colors, it is possible to get three different appearances for the same input file using the :ref:`tiff24nc<Devices_TIFF>` (RGB), :ref:`tiff32nc<Devices_TIFF>` (CMYK), and :ref:`tiffsep<Devices_TIFF>` (CMYK plus spot colors) devices.


.. note::
   In Adobe Acrobat, viewing of the effects of overprinting is enabled by the 'Overprint Preview' item in the 'Advanced' menu. This feature is not available in the free Acrobat Reader. The free Acrobat Reader also uses the tint transform functions to convert spot colors to the appropriate alternate color space.



.. _Use_How Ghostscript finds files:

How Ghostscript finds files
----------------------------------------------------------


When looking for initialization files (``gs_*.ps``, ``pdf_*.ps``), font files, the ``Fontmap`` file, files named on the command line, and resource files, Ghostscript first tests whether the file name specifies an absolute path.



Testing a file name for an absolute path
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. list-table::
   :widths: 50 50
   :header-rows: 1

   * - System
     - Does the name ...
   * - Unix
     - Begin with ``/`` ?
   * - MS Windows
     - Have ``:`` as its second character, or begin with ``/``, ``\``, or ``//servername/share/`` ?
   * - VMS
     - Contain a node, device, or root specification?


If the test succeeds, Ghostscript tries to open the file using the name given. Otherwise it tries directories in this order:

#. The current directory if enabled by the :ref:`-P switch<P switch>`.
#. The directories specified by :ref:`-I switches<I switch>` in the command line, if any.
#. The directories specified by the ``GS_LIB`` environment variable, if any.
#. If built with ``COMPILE_INITS=1`` (currently the default build) the files in the ``%rom%Resource/`` and ``%rom%iccprofiles/`` directories are built into the executable.
#. The directories specified by the ``GS_LIB_DEFAULT`` macro (if any) in the makefile when this executable was built.


``GS_LIB_DEFAULT``, ``GS_LIB``, and the ``-I`` parameter may specify either a single directory or a list of directories separated by a character appropriate for the operating system ("``:``" on Unix systems, "``,``" on VMS systems, and "``;``" on MS Windows systems). By default, Ghostscript no longer searches the current directory first but provides ``-P`` switch for a degree of backward compatibility.

Note that Ghostscript does not use this file searching algorithm for the ``run`` or ``file`` operators: for these operators, it simply opens the file with the name given. To run a file using the searching algorithm, use ``runlibfile`` instead of ``run``.



.. _PS resources:

Finding PostScript Level 2 resources
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


Adobe specifies that resources are installed in a single directory. Ghostscript instead maintains a list of resource directories, and uses an extended method for finding resource files.

The search for a resource file depends on whether the value of the system parameter ``GenericResourceDir`` specifies an absolute path. The user may set it as explained in :ref:`resource related parameters<resource related parameters>`.

If the user doesn't set the system parameter ``GenericResourceDir``, or use the ``-sGenericResourceDir=`` command line option, Ghostscript creates a default value for it by looking on the directory paths explained in `How Ghostscript finds files`_, excluding the current directory. The first path with Resource in it is used, including any prefix up to the path separator character following the string Resource. For example, when ``COMPILE_INITS=1`` (the current default build), if the first path is ``%rom%Resource/Init/``, then the ``GenericResourceDir`` systemparam will be set to ``%rom%Resource/`` by default.

If the value of the system parameter ``GenericResourceDir`` is an absolute path (the default), Ghostscript assumes a single resource directory. It concatenates:


#. The value of the system parameter ``GenericResourceDir``.
#. The name of the resource category (for instance, ``CMap``).
#. The name of the resource instance (for instance, ``Identity-H``).

If the value of the system parameter GenericResourceDir is not an absolute path, Ghostscript assumes multiple resource directories. In this case it concatenates:

#. A directory listed in the section `How Ghostscript finds files`_, except the current directory.
#. The value of the system parameter ``GenericResourceDir``.
#. The name of the resource category (for instance, ``CMap``).
#. The name of the resource instance (for instance, ``Identity-H``).


Due to possible variety of the part 1, the first successful combination is used. For example, if the value of the system parameter ``GenericResourceDir`` is the string ``../Resource/`` (or its equivalent in the file path syntax of the underlying platform), Ghostscript searches for ``../Resource/CMap/Identity-H`` from all directories listed in `How Ghostscript finds files`_. So in this example, if the user on a Windows platform specifies the command line option ``-I.;../gs/lib;c:/gs8.50/lib``, Ghostscript searches for ``../gs/Resource/CMap/Identity-H`` and then for ``c:/gs8.50/Resource/CMap/Identity-H``.

To get a proper platform dependent syntax Ghostscript inserts the value of the system parameter ``GenericResourcePathSep`` (initially "``/``" on Unix and Windows, "``:``" on MacOS, "``.``" or "``]``" on OpenVMS). The string ``../Resource`` is replaced with a platform dependent equivalent.

In the case of multiple resource directories, the default ``ResourceFileName`` procedure retrieves either a path to the first avaliable resource, or if the resource is not available it returns a path starting with ``GenericResourceDir``. Consequently Postscript installers of Postscript resources will overwrite an existing resource or add a new one to the first resource directory.



To look up fonts, after exhausting the search method described in the :ref:`next section<Font lookup>`, it concatenates together:

#. the value of the system parameter FontResourceDir (initially ``/Resource/Font/``).
#. the name of the resource font (for instance, *Times-Roman*).

.. note::

   Even although the system parameters are named "somethingDir", they are not just plain directory names: they have "``/``" on the end, so that they can be concatenated with the category name or font name.


.. _Use_Font lookup:

Font lookup
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Ghostscript has a slightly different way to find the file containing a font with a given name. This rule uses not only the search path defined by ``-I``, ``GS_LIB``, and ``GS_LIB_DEFAULT`` as :ref:`described above<How Ghostscript finds files>`, but also the directory that is the value of the ``FontResourceDir`` system parameter, and an additional list of directories that is the value of the ``GS_FONTPATH`` environment variable (or the value provided with the ``-sFONTPATH= switch``, if present).


At startup time, Ghostscript reads in the ``Fontmap`` files in every directory on the search path (or in the list provided with the ``-sFONTMAP=`` switch, if present): these files are catalogs of fonts and the files that contain them. (See the :ref:`documentation of fonts<Fonts.html>` for details.) Then, when Ghostscript needs to find a font that isn't already loaded into memory, it goes through a series of steps.

#. First, it looks up the font name in the combined Fontmaps. If there is an entry for the desired font name, and the file named in the entry can be found in some directory on the general search path (defined by ``-I``, ``GS_LIB``, and ``GS_LIB_DEFAULT``), and the file is loaded successfully, and loading it defines a font of the desired name, that is the end of the process.

#. If this process fails at any step, Ghostscript looks for a file whose name is the concatenation of the value of the ``FontResourceDir`` system parameter and the font name, with no extension. If such a file exists, can be loaded, and defines a font of the desired name, that again is the end. The value of ``FontResourceDir`` is normally the string ``/Resource/Font/``, but it can be changed with the ``setsystemparams`` operator: see the PostScript Language Reference Manual for details.

#. If that fails, Ghostscript then looks for a file on the general search path whose name is the desired font name, with no extension. If such a file exists, can be loaded, and defines a font of the desired name, that again is the end.

#. If that too fails, Ghostscript looks at the ``GS_FONTPATH`` environment variable (or the value provided with the ``-sFONTPATH=`` switch, if present), which is also a list of directories. It goes to the first directory on the list, and it's descendants, looking for all files that appear to contain PostScript fonts (also Truetype fonts); it then adds all those files and fonts to the combined Fontmaps, and starts over.

#. If scanning the first ``FONTPATH`` directory doesn't produce a file that provides the desired font, it adds the next directory on the ``FONTPATH`` list, and so on until either the font is defined successfully or the list is exhausted.

#. Finally, if all else fails, it will try to find a substitute for the font from among the standard 35 fonts.

.. note::
   :ref:`CID fonts<CID Fonts>` (e.g. Chinese, Japanese and Korean) are found using a different method.




Differences between search path and font path
""""""""""""""""""""""""""""""""""""""""""""""""""""

.. list-table::
   :header-rows: 1
   :widths: 50 50

   * - Search path
     - Font path

   * - ``-I switch``
     - ``-sFONTPATH= switch``

   * - ``GS_LIB`` and ``GS_LIB_DEFAULT`` environment variables
     - ``GS_FONTPATH`` environment variable

   * - Consulted first
     - Consulted only if search path and ``FontResourceDir`` don't provide the file.

   * - Font-name-to-file-name mapping given in Fontmap files;

       aliases are possible, and there need not be any relation

       between the font name in the Fontmap and the ``FontName`` in the file.

     - Font-name-to-file-name mapping is implicit – the ``FontName`` in the file is used.

       Aliases are not possible.

   * - Only fonts and files named in Fontmap are used.
     - Every Type 1 font file in each directory is available;

       if TrueType fonts are supported (the ``ttfont.dev`` feature was included

       when the executable was built), they are also available.



If you are using one of the following types of computer, you may wish to set the environment variable ``GS_FONTPATH`` to the value indicated so that Ghostscript will automatically acquire all the installed Type 1 (and, if supported, TrueType) fonts (but see below for notes on systems marked with "*"):


Suggested GS_FONTPATH for different systems
""""""""""""""""""""""""""""""""""""""""""""""""""""

.. list-table::
   :header-rows: 1
   :widths: 50 50


   * - System type
     -  GS_FONTPATH

   * - Digital Unix
     - /usr/lib/X11/fonts/Type1Adobe

   * - Ultrix
     - /usr/lib/DPS/outline/decwin

   * - HP-UX 9
     - /usr/lib/X11/fonts/type1.st/typefaces

   * - IBM AIX
     - /usr/lpp/DPS/fonts/outlines

       /usr/lpp/X11/lib/X11/fonts/Type1

       /usr/lpp/X11/lib/X11/fonts/Type1/DPS

   * - NeXT
     - /NextLibrary/Fonts/outline

   * - SGI IRIX :ref:`*<GS_FONTPATH footnote 1>`
     - /usr/lib/DPS/outline/base

       /usr/lib/X11/fonts/Type1

   * - SunOS 4.x (NeWSprint only)
     - newsprint_2.5/SUNWsteNP/reloc/$BASEDIR/NeWSprint/

       small_openwin/lib/fonts

   * - SunOS 4.x :ref:`**<GS_FONTPATH footnote 2>`
     - /usr/openwin/lib/X11/fonts/Type1/outline

   * - Solaris 2.x :ref:`**<GS_FONTPATH footnote 2>`
     - /usr/openwin/lib/X11/fonts/Type1/outline

   * - VMS
     - SYS$COMMON:[SYSFONT.XDPS.OUTLINE]



.. _GS_FONTPATH footnote 1:

:ref:`**<GS_FONTPATH footnote 1>` On SGI IRIX systems, you must use ``Fontmap.SGI`` in place of ``Fontmap`` or ``Fontmap.GS``, because otherwise the entries in Fontmap will take precedence over the fonts in the FONTPATH directories.

.. _GS_FONTPATH footnote 2:

:ref:`**<GS_FONTPATH footnote 2>` On Solaris systems simply setting ``GS_FONTPATH`` or using ``-sFONTPATH=`` may not work, because for some reason some versions of Ghostscript can't seem to find any of the Type1 fonts in ``/usr/openwin/lib/X11/fonts/Type1/outline``. (It says: "15 files, 15 scanned, 0 new fonts". We think this problem has been fixed in Ghostscript version 6.0, but we aren't sure because we've never been able to reproduce it.) See ``Fontmap.Sol`` instead. Also, on Solaris 2.x it's probably not worth your while to add Sun's fonts to your font path and Fontmap. The fonts Sun distributes on Solaris 2.x in the directories: ``/usr/openwin/lib/X11/fonts/Type1`` & ``/usr/openwin/lib/X11/fonts/Type1/outline`` are already represented among the ones distributed as part of Ghostscript; and on some test files, Sun's fonts have been shown to cause incorrect displays with Ghostscript.


These paths may not be exactly right for your installation; if the indicated directory doesn't contain files whose names are familiar font names like Courier and Helvetica, you may wish to ask your system administrator where to find these fonts.

Adobe Acrobat comes with a set of fourteen Type 1 fonts, on Unix typically in a directory called ``/Acrobat3/Fonts``. There is no particular reason to use these instead of the corresponding fonts in the Ghostscript distribution (which are of just as good quality), except to save about a megabyte of disk space, but the installation documentation explains how to do it on Unix.



CID fonts
~~~~~~~~~~~~~~

CID fonts are PostScript resources containing a large number of glyphs (e.g. glyphs for Far East languages, Chinese, Japanese and Korean). Please refer to the PostScript Language Reference, third edition, for details.

CID font resources are a different kind of PostScript resource from fonts. In particular, they cannot be used as regular fonts. CID font resources must first be combined with a CMap resource, which defines specific codes for glyphs, before it can be used as a font. This allows the reuse of a collection of glyphs with different encodings.

The simplest method to request a font composed of a CID font resource and a CMap resource in a PostScript document is:


.. code-block:: bash

   /CIDFont-CMap findfont


where ``CIDFont`` is a name of any CID font resource, and ``CMap`` is a name of a CMap resource designed for the same character collection. The interpreter will compose the font automatically from the specified CID font and CMap resources. Another method is possible using the ``composefont`` operator.

CID fonts must be placed in the ``/Resource/CIDFont/`` directory. They are not found using `Font lookup`_ on the search path or font path.






CID font substitution
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


Automatic CIDFont Substitution
""""""""""""""""""""""""""""""""""

In general, it is highly recommended that CIDFonts used in the creation of PDF jobs should be embedded or available to Ghostscript as CIDFont resources, this ensures that the character set, and typeface style are as intended by the author.

In cases where the original CIDFont is not available, the next best option is to provide Ghostscript with a mapping to a suitable alternative CIDFont - see below for details on how this is achieved. However, Ghostscript does provide the ability to use a "fall back" CIDFont substitute. As shipped, this uses the DroidSansFallback.ttf font. This font contains a large number of glyphs covering several languages, but it is not comprehensive. There is, therefore, a chance that glyphs may be wrong, or missing in the output when this fallback is used.

Internally, the font is referenced as CIDFont resource called ``CIDFallBack``, thus a different fallback from ``DroidSansFallback.ttf`` can be specified adding a mapping to your cidfmap file (see below for details) to map the name "CIDFallBack" as you prefer. For ``CIDFallBack`` the mapping must be a TrueType font or TrueType collection, it cannot be a Postscript CIDFont file.

As with any font containing large numbers of glyphs, ``DroidSansFallback.ttf`` is quite large (~3.5Mb at the of writing). If this is space you cannot afford in your use of Ghostscript, you can simply delete the file from: ``Resource/CIDFSubst/DroidSansFallback.ttf``. The build system will cope with the file being removed, and the initialization code will avoid adding the internal fall back mapping if the file is missing.

If ``DroidSansFallback.ttf`` is removed, and no other ``CIDFallBack`` mapping is supplied, the final "fall back" is to use a "dumb" bullet CIDFont, called ``ArtifexBullet``. As the name suggests, this will result in all the glyphs from a missing CIDFont being replaced with a simple bullet point.

This type of generic fall back CIDFont substitution can be very useful for viewing and proofing jobs, but may not be appropriate for a "production" workflow, where it is expected that only the original font should be used. For this situation, you can supply Ghostscript with the command line option: ``-dPDFNOCIDFALLBACK``. By combining ``-dPDFNOCIDFALLBACK`` with ``-dPDFSTOPONERROR`` a production workflow can force a PDF with missing CIDFonts to error, and avoid realising a CIDFont was missing only after printing.

The directory in which the fallback TrueType font or collection can be specified by the command line parameter ``-sCIDFSubstPath="path/to/TTF"``, or with the environment variable ``CIDFSUBSTPATH``. The file name of the substitute TrueType font can be specified using the command line parameter ``-sCIDFSubstFont="TTF file name"`` or the environment variable ``CIDFSUBSTFONT``.



Explicit CIDFont Substitution
""""""""""""""""""""""""""""""""""

Substitution of CID font resources is controlled, by default, by the Ghostscript configuration file ``Resource/Init/cidfmap``, which defines a CID font resource map.

The file forms a table of records, each of which should use one of three formats, explained below. Users may modify ``Resource/Init/cidfmap`` to configure Ghostscript for a specific need. Note that the default Ghostscript build includes such configuration and resource files in a rom file system built into the executable. So, to ensure your changes have an effect, you should do one of the following: rebuild the executable; use the "``-I``" command line option to add the directory containing your modified file to Ghostscript's search path; or, finally, build Ghostscript to use disk based resources.


Format 1
""""""""""""""""""""""""""""""""""

To substitute a CID font resource with another CID font resource, add a record like this:


.. code-block:: bash

   /Substituted /Original ;


where ``Substituted`` is a name of CID font resource being used by a document, and ``Original`` is a name of an available CID font resource. Please pay attention that both them must be designed for same character collection. In other words, you cannot substitute a Japanese CID font resource with a Korean CID font resource, etc. CMap resource names must not appear in ``lib/cidfmap``. The trailing semicolon and the space before it are both required.


Format 2
""""""""""""""""""""""""""""""""""

To substitute (emulate) a CID font resource with a TrueType font file, add a record like this:

.. code-block:: bash

   /Substituted << keys&values >> ;

Where ``keys&values`` are explained in the table below.


.. list-table::
   :header-rows: 1
   :widths: 15 15 70


   * - Key
     - Type
     - Description

   * - ``/Path``
     - string
     - A path to a TrueType font file.

       This must be an absolute path. If using -dSAFER, the directory containing the font file must be on one of the permitted paths.

   * - ``/FileType``
     - name
     - Must be ``/TrueType``.

   * - ``/SubfontID``
     - integer
     - (optional) Index of the font in font collection, such as TTC.

       This is ignored if Path doesn't specify a collection. The first font in a collection is 0. Default value is 0.

   * - ``/CSI``
     - array of 2 or 3 elements
     - (required) Information for building ``CIDSystemInfo``.

       If the array consists of 2 elements, the first element is a string,

       which specifies ``Ordering``; the second element is a number, which specifies ``Supplement``.

       |

       If the array consists of 3 elements, the first element is a string,

       which specifies ``Registry``; the second element is a string,

       which specifies ``Ordering``; the third element is a number,

       which specifies ``Supplement``.


Currently only CIDFontType 2 can be emulated with a TrueType font. The TrueType font must contain enough characters to cover an Adobe character collection, which is specified in ``Ordering`` and used in documents.


Format 3
""""""""""""""

To point Ghostscript at a specific CIDFont file outside it's "normal" resource search path :



.. code-block:: bash

   /CIDName (path/to/cid/font/file) ;


where ``CIDName`` is a name of CID font resource being used by a document, and ``path/to/cid/font/file`` is the path to the Postscript CIDFont file, including the file name. NOTE: the CIDFont file, when executed by the Postscript interpreter, must result in a CIDFont resource being defined whose CIDFontName matches the "CIDName" key for the current record. I.E. an entry with the key /PingHei-Bold must reference a file which creates a CIDFont resource called "PingHei-Bold". To substitute a file based CIDFont for a differently named CIDFont, use formats 1 and 3 in combination (the order of the entries is not important).

The trailing semicolon and the space before it are both required.



Examples
""""""""""

Format 1

.. code-block:: bash

   /Ryumin-Medium /ShinGo-Bold ;
   /Ryumin-Light /MS-Mincho ;
   Format 2:
   /Batang << /FileType /TrueType /Path (C:/WINDOWS/fonts/batang.ttc) /SubfontID 0 /CSI [(Korea1) 3] >> ;
   /Gulim << /FileType /TrueType /Path (C:/WINDOWS/fonts/gulim.ttc) /SubfontID 0 /CSI [(Korea1) 3] >> ;
   /Dotum << /FileType /TrueType /Path (C:/WINDOWS/fonts/gulim.ttc) /SubfontID 2 /CSI [(Korea1) 3] >> ;

Format 1 & 2

.. code-block:: bash

   /SimSun << /FileType /TrueType /Path (C:/WINDOWS/fonts/simsun.ttc) /SubfontID 0 /CSI [(GB1) 2] >> ;
   /SimHei << /FileType /TrueType /Path (C:/WINDOWS/fonts/simhei.ttf) /SubfontID 0 /CSI [(GB1) 2] >> ;
   /STSong-Light /SimSun ;
   /STHeiti-Regular /SimHei ;
   Format 3:
   /PMingLiU (/usr/local/share/font/cidfont/PMingLiU.cid) ;

Format 1 & 3

.. code-block:: bash

   /Ryumin-Light /PMingLiU ;
   /PMingLiU (/usr/local/share/font/cidfont/PMingLiU.cid) ;



The win32 installer of recent version of Ghostscript has a checkbox for "Use Windows TrueType fonts for Chinese, Japanese and Korean" to optionally update ``lib/cidfmap`` with the common CJK fonts provided by Microsoft products. The script can also be run separately (e.g. against a network drive with windows CJK fonts):

.. code-block:: bash

   gswin32c -q -dBATCH -sFONTDIR=c:/windows/fonts -sCIDFMAP=lib/cidfmap lib/mkcidfm.ps

Note that the font file path uses Postscript syntax. Because of this, backslashes in the paths must be represented as a double backslash.

This can complicate substitutions for fonts with non-Roman names. For example, if a PDF file asks for a font with the name ``/#82l#82r#83S#83V#83b#83N``. This cannot be used directly in a cidfmap file because the #xx notation in names is a PDF-only encoding. Instead, try something like:

.. code-block:: bash

   <82C68272835383568362834E>cvn << /Path (C:/WINDOWS/Fonts/msmincho.ttc) /FileType /TrueType /SubfontID 0 /CSI [(Japan1) 3] >> ;


Where ``<82C68272835383568362834E>`` is the same byte sequence converted to a hex string. This lets you specify a name using any sequence of bytes through the encodings available for Postscript strings.

Note that loading truetype fonts directly from ``/Resources/CIDFont`` is no longer supported. There is no reliable way to generate a character ordering for truetype fonts. The 7.0x versions of Ghostscript supported this by assuming a Japanese character ordering. This is replaced in the 8.0x and later releases with the more general ``cidfmap`` mechanism.

The PDF specification requires CID font files to be embedded, however some documents omit them. As a workaround the PDF interpreter applies an additional substitution method when a requested CID font resource is not embedded and it is not available. It takes values of the keys ``Registry`` and ``Ordering`` from the ``CIDFontSystem`` dictionary, and concatenates them with a dash inserted. For example, if a PDF CID font resource specifies:


.. code-block:: bash

   /CIDSystemInfo << /Registry (Adobe) /Ordering (CNS1) /Supplement 1 >>


the generated subsitituite name is ``Adobe-CNS1``. The latter may look some confusing for a font name, but we keep it for compatibility with older Ghostscript versions, which do so due to a historical reason. Add a proper record to ``lib/cidfmap`` to provide it.

Please note that when a PDF font resource specifies:

.. code-block:: bash

   /Registry (Adobe) /Ordering (Identity)


there is no way to determine the language properly. If the CID font file is not embedded, the ``Adobe-Identity`` record depends on the document and a correct record isn't possible when a document refers to multiple Far East languages. In the latter case add individual records for specific CID font names used in the document.

Consequently, if you want to handle any PDF document with non-embedded CID fonts (which isn't a correct PDF), you need to create a suitable ``lib/cidfmap`` by hand, possibly a specific one for each document.




Using Unicode True Type fonts
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Ghostscript can make use of Truetype fonts with a Unicode character set. To do so, you should generate a (NOTE: non-standard!) Postscript or PDF job where the relevant text is encoded as UTF-16. Ghostscript may be used for converting such jobs to other formats (Postscript, PDF, PXL etc). The resulting output will be compliant with the spec (unlike the input).

To render an UTF-16 encoded text, one must do the following:

- Provide a True Type font with Unicode Encoding. It must have a ``cmap`` table with ``platformID`` equals to 3 (Windows), and ``SpecificID`` eqials to 1 (Unicode).
- Describe the font in ``Resource/Init/cidfmap`` with special values for the ``CSI`` key : ``[(Artifex) (Unicode) 0]``.
- In the PS or PDF job combine the font with one of CMap ``Identity-UTF16-H`` (for the horizontal writing mode) or ``Identity-UTF16-V`` (for the vertical writing mode). Those CMaps are distributed with Ghostscript in ``Resource/CMap``.


Please note that ``/Registry (Adobe) /Ordering (Identity)`` won't properly work for Unicode documents, especially for the searchability feature (see `CID font substitution`_).


Temporary files
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


Where Ghostscript puts temporary files
"""""""""""""""""""""""""""""""""""""""""""


.. list-table::
   :header-rows: 1

   * - Platform
     - Filename
     - Location
   * - MS Windows and OpenVMS
     - _temp_XX.XXX
     - Current directory
   * - OS/2
     - gsXXXXXX
     - Current directory
   * - Unix
     - gs_XXXXX
     - /tmp


You can change in which directory Ghostscript creates temporary files by setting the ``TMPDIR`` or ``TEMP`` environment variable to the name of the directory you want used. Ghostscript currently doesn't do a very good job of deleting temporary files if it exits because of an error; you may have to delete them manually from time to time.




Notes on specific platforms
----------------------------------

Word size (32 or 64 bits)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The original PostScript language specification, while not stating a specific word size, defines 'typical' limits which make it clear that it was intended to run as a 32-bit environment. Ghostscript was originally coded that way, and the heritage remains within the code base.

Because the Ghostscript PDF interpreter is currently written in PostScript, it proved necessary to add support for 64-bit integers so that we could process PDF files which exceed 2GB in size. This is the only real purpose in adding support for large integers, however since that time, we have made some efforts to allow for the use of 64-bit words; in particular the use of integers, but also lifting the 64K limit on strings and arrays, among other areas.

However this is, obviously, dependent on the operating system and compiler support available. Not all builds of Ghostscript will support 64-bit integers, though some 32-bit builds (eg Windows) will.

Even when the build supports 64-bit words, you should be aware that there are areas of Ghostscript which do not support 64-bit values. Sometimes these are dependent on the build and other times they are inherent in the architecture of Ghostscript (the graphics library does not support 64-bit co-ordinates in device space for example, and most likely never will).


.. note ::

  The extended support for 64-bit word size can be disabled by executing 'true .setcpsimode', This is important for checking the output of the Quality Logic test suite (and possibly other test suites) as the tests make assumptions about the sizes of integers (amongst other things). You can run ``/ghostpdl/Resource/Init/gs_cet.ps`` to change Ghostscript's behaviour so that it matches the observed behaviour of Adobe CPSI interpreters.


Unix
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The Ghostscript distribution includes some Unix shell scripts to use with Ghostscript in different environments. These are all user-contributed code, so if you have questions, please contact the user identified in the file, not Artifex Software.

``pv.sh``
""""""""""""""""""""
   Preview a specified page of a ``dvi`` file in an X window


``sysvlp.sh``
""""""""""""""""""""
   System V 3.2 lp interface for parallel printer


``pj-gs.sh``
""""""""""""""""""""
   Printing on an H-P PaintJet under HP-UX


``unix-lpr.sh``
""""""""""""""""""""
   Queue filter for ``lpr`` under Unix; its documentation is intended for system administrators


``lprsetup.sh``
""""""""""""""""""""
   Setup for ``unix-lpr.sh``



VMS
~~~~~~~~~

To be able to specify switches and file names when invoking the interpreter, define ``gs`` as a foreign command:


.. code-block:: bash

   $ gs == "$disk:[directory]gs.exe"


where the "disk" and "directory" specify where the Ghostscript executable is located. For instance:

.. code-block:: bash

   $ gs == "$dua1:[ghostscript]gs.exe"


On VMS systems, the last character of each "directory" name indicates what sort of entity the "directory" refers to. If the "directory" name ends with a colon "``:``", it is taken to refer to a logical device, for instance:

.. code-block:: bash

   $ define ghostscript_device dua1:[ghostscript_510]

.. code-block:: bash

   $ define gs_lib ghostscript_device:

If the "directory" name ends with a closing square bracket "``]``", it is taken to refer to a real directory, for instance

.. code-block:: bash

   $ define gs_lib dua1:[ghostscript]

Defining the logical ``GS_LIB``:


.. code-block:: bash

   $ define gs_lib disk:[directory]

allows Ghostscript to find its initialization files in the Ghostscript directory even if that's not where the executable resides.


Although VMS DCL itself converts unquoted parameters to upper case, C programs such as Ghostscript receive their parameters through the C runtime library, which forces all unquoted command-line parameters to lower case. That is, with the command:

.. code-block:: bash

   $ gs -Isys$login:


Ghostscript sees the switch as ``-isys$login``, which doesn't work. To preserve the case of switches, quote them like this:

.. code-block:: bash

   $ gs "-Isys$login:"


If you write printer output to a file with ``-sOutputFile=`` and then want to print the file later, use ``"PRINT/PASSALL"``.

PDF files (or PostScript files that use the ``setfileposition`` operator) must be "stream LF" type files to work properly on VMS systems. (Note: This definitely matters if Ghostscript was compiled with ``DEC C``; we are not sure of the situation if you use ``gcc``.) Because of this, if you transfer files by FTP, you probably need to do one of these two things after the transfer:


- If the FTP transfer was in text (ASCII) mode:

.. code-block:: bash

   $ convert/fdl=streamlf.fdl input-file output-file


where the contents of the file ``STREAMLF.FDL`` are:


.. blockquote


FILE

            ORGANIZATION            sequential

    RECORD

            BLOCK_SPAN              yes

            CARRIAGE_CONTROL        carriage_return

            FORMAT                  stream_lf



- If the FTP transfer was in binary mode:

.. code-block:: bash

   $ set file/attribute=(rfm:stmlf)



Using X Windows on VMS
"""""""""""""""""""""""""""

If you are using on an X Windows display, you can set it up with the node name and network transport, for instance:

.. code-block:: bash

   $ set display/create/node="doof.city.com"/transport=tcpip

and then run Ghostscript by typing ``gs`` at the command line.




MS Windows
~~~~~~~~~~~~~~

The name of the Ghostscript command line executable on MS Windows is ``gswin32c/gswin64c`` so use this instead of the plain '``gs``' in the quickstart examples.

To run the batch files in the Ghostscript lib directory, you must add ``gs\bin`` and ``gs\lib`` to the ``PATH``, where ``gs`` is the top-level Ghostscript directory.

When passing options to Ghostscript through a batch file wrapper such as ``ps2pdf.bat`` you need to substitute '#' for '=' as the separator between options and their arguments. For example:


.. code-block:: bash

   ps2pdf -sPAPERSIZE#a4 file.ps file.pdf


Ghostscript treats '#' the same internally, and the '=' is mangled by the command shell.

There is also an older version for MS Windows called just ``gswin32`` that provides its own window for the interactive postscript prompt. The executable ``gswin32c/gswin64c`` is usually the better option since it uses the native command prompt window.

For printer devices, the default output is the default printer. This can be modified as follows:


.. code-block:: bash

   -sOutputFile="%printer%printer name"


Output to the named printer. If your printer is named "HP DeskJet 500" then you would use ``-sOutputFile="%printer%HP DeskJet 500"``.


MS-DOS
~~~~~~~~~~~~~~


.. note::

   Ghostscript is no longer supported on MS-DOS.

Invoking Ghostscript from the command prompt in Windows is supported by the Windows executable described above.




X Windows
~~~~~~~~~~~~~~

Ghostscript looks for the following resources under the program name ``ghostscript`` and class name ``Ghostscript``; the ones marked "**" are calculated from display metrics:

X Windows resources
"""""""""""""""""""""


.. list-table::
   :header-rows: 1

   * - Name
     - Class
     - Default
   * - background
     - Background
     - white
   * - foreground
     - Foreground
     - black
   * - borderColor
     - BorderColor
     - black
   * - borderWidth
     - BorderWidth
     - 1
   * - geometry
     - Geometry
     - NULL
   * - xResolution
     - Resolution
     - :sub:`**`
   * - yResolution
     - Resolution
     - :sub:`**`
   * - useExternalFonts
     - UseExternalFonts
     - true
   * - useScalableFonts
     - UseScalableFonts
     - true
   * - logExternalFonts
     - LogExternalFonts
     - false
   * - externalFontTolerance
     - ExternalFontTolerance
     - 10.0
   * - palette
     - Palette
     - Color
   * - maxGrayRamp
     - MaxGrayRamp
     - 128
   * - maxRGBRamp
     - MaxRGBRamp
     - 5
   * - maxDynamicColors
     - MaxDynamicColors
     - 256
   * - useBackingPixmap
     - UseBackingPixmap
     - true
   * - useXPutImage
     - UseXPutImage
     - true
   * - useXSetTile
     - UseXSetTile
     - true


X resources
"""""""""""""""

To set X resources, put them in a file (such as ``~/.Xdefaults`` on Unix) in a form like this:

.. list-table::

   * - Ghostscript*geometry:
     - 595x842-0+0
   * - Ghostscript*xResolution:
     - 72
   * - Ghostscript*yResolution:
     - 72


Then merge these resources into the X server's resource database:


.. code-block:: bash

   xrdb -merge ~/.Xdefaults


- Ghostscript doesn't look at the default system background and foreground colors; if you want to change the background or foreground color, you must set them explicitly for Ghostscript. This is a deliberate choice, so that PostScript documents will display correctly by default -- with white as white and black as black -- even if text windows use other colors.
- The ``geometry`` resource affects only window placement.
- Resolution is expressed in pixels per inch (1 inch = 25.4mm).
- The font tolerance gives the largest acceptable difference in height of the screen font, expressed as a percentage of the height of the desired font.
- The ``palette`` resource can be used to restrict Ghostscript to using a grayscale or monochrome palette.

``maxRGBRamp`` and ``maxGrayRamp`` control the maximum number of colors that Ghostscript allocates ahead of time for the dither cube (ramp). Ghostscript never preallocates more than half the cells in a colormap. ``maxDynamicColors`` controls the maximum number of colors that Ghostscript will allocate dynamically in the colormap.

Working around bugs in X servers
""""""""""""""""""""""""""""""""""""

The "``use...``" resources exist primarily to work around bugs in X servers.

- Old versions of DEC's X server (DECwindows) have bugs that require setting ``useXPutImage`` or ``useXSetTile`` to false.
- Some servers do not implement backing pixmaps properly, or do not have enough memory for them. If you get strange behavior or "out of memory" messages, try setting ``useBackingPixmap`` to ``false``.
- Some servers do not implement tiling properly. This appears as broad bands of color where dither patterns should appear. If this happens, try setting ``useXSetTile`` to ``false``.
- Some servers do not implement bitmap or pixmap displaying properly. This may appear as white or black rectangles where characters should appear; or characters may appear in "inverse video" (for instance, white on a black rectangle rather than black on white). If this happens, try setting ``useXPutImage`` to ``false``.


X device parameters
"""""""""""""""""""""""""""""
   In addition to the device parameters recognized by all devices, Ghostscript's X driver provides parameters to adjust its performance. Users will rarely need to modify these. Note that these are parameters to be set with the ``-d`` switch in the command line (e.g., ``-dMaxBitmap=10000000``), not resources to be defined in the ``~/.Xdefaults`` file.

``AlwaysUpdate <boolean>``
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   If ``true``, the driver updates the screen after each primitive drawing operation; if ``false`` (the default), the driver uses an intelligent buffered updating algorithm.

``MaxBitmap <integer>``
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   If the amount of memory required to hold the pixmap for the window is no more than the value of ``MaxBitmap``, the driver will draw to a pixmap in Ghostscript's address space (called a "client-side pixmap") and will copy it to the screen from time to time; if the amount of memory required for the pixmap exceeds the value of ``MaxBitmap``, the driver will draw to a server pixmap. Using a client-side pixmap usually provides better performance -- for bitmap images, possibly much better performance -- but since it may require quite a lot of RAM (e.g., about 2.2 Mb for a 24-bit 1024x768 window), the default value of ``MaxBitmap`` is 0.


``MaxTempPixmap, MaxTempImage <integer>``
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   These control various aspects of the driver's buffering behavior. For details, please consult the source file ``gdevx.h``.


SCO Unix
"""""""""""""

Because of bugs in the SCO Unix kernel, Ghostscript will not work if you select direct screen output and also allow it to write messages on the console. If you are using direct screen output, redirect Ghostscript's terminal output to a file.




.. _Use_Command line options:


Command line options
-----------------------

Unless otherwise noted, these switches can be used on all platforms.


General switches
~~~~~~~~~~~~~~~~~~~

.. _Use_Input Control:

Input control
""""""""""""""""""

**@filename**
^^^^^^^^^^^^^^^^^
   Causes Ghostscript to read filename and treat its contents the same as the command line. (This was intended primarily for getting around DOS's 128-character limit on the length of a command line.) Switches or file names in the file may be separated by any amount of white space (space, tab, line break); there is no limit on the size of the file.

**--** *filename arg1 ...*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
**-+** *filename arg1 ...*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Takes the next argument as a file name as usual, but takes all remaining arguments (even if they have the syntactic form of switches) and defines the name ``ARGUMENTS`` in userdict (not systemdict) as an array of those strings, before running the file. When Ghostscript finishes executing the file, it exits back to the shell.

**-@** *filename arg1 ...*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Does the same thing as -- and -+, but expands @filename arguments.

**-**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
**-_**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   These are not really switches: they tell Ghostscript to read from standard input, which is coming from a file or a pipe, with or without buffering. On some systems, Ghostscript may read the input one character at a time, which is useful for programs such as ghostview that generate input for Ghostscript dynamically and watch for some response, but can slow processing. If performance is significantly slower than with a named file, try '-_' which always reads the input in blocks. However, '-' is equivalent on most systems.

**-c** *token ...*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
**-c** *string ...*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Interprets arguments as PostScript code up to the next argument that begins with "-" followed by a non-digit, or with "@". For example, if the file ``quit.ps`` contains just the word "``quit``", then ``-c quit`` on the command line is equivalent to ``quit.ps`` there. Each argument must be valid PostScript, either individual tokens as defined by the token operator, or a string containing valid PostScript.

   Because Ghostscript must initialize the PostScript environment before executing the commands specified by this option it should be specified after other setup options. Specifically this option 'bind's all operations and sets the systemdict to readonly.

**-f**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Interprets following non-switch arguments as file names to be executed using the normal run command. Since this is the default behavior, ``-f`` is useful only for terminating the list of tokens for the ``-c`` switch.

**-f** *filename*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Execute the given file, even if its name begins with a "-" or "@".


File searching
""""""""""""""""""""""""""""""""""

Note that by "library files" here we mean all the files identified using the search rule under "`How Ghostscript finds files`_" above: Ghostscript's own initialization files, fonts, and files named on the command line.

.. _I switch:

**-I** *directories*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Adds the designated list of directories at the head of the search path for library files.

.. _P switch:

**-P**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Makes Ghostscript look first in the current directory for library files.

**-P-**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Makes Ghostscript not look first in the current directory for library files (unless, of course, the first explicitly supplied directory is "."). This is now the default.


.. _Use_Setting Parameters:

Setting parameters
"""""""""""""""""""""""""""""""""""

**-D** *name*, **-d** *name*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

   Define a name in systemdict with value=true.

**-D** *name=token*, **-d** *name=token*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

   Define a name in systemdict with the given value. The value must be a valid PostScript token (as defined by the ``token`` operator). If the token is a non-literal name, it must be true, false, or null. It is recommeded that this is used only for simple values -- use ``-c`` (above) for complex values such as procedures, arrays or dictionaries.

   Note that these values are defined before other names in systemdict, so any name that that conflicts with one usually in systemdict will be replaced by the normal definition during the interpreter initialization.

**-S** *name=string*, **-s** *name=string*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

   Define a name in systemdict with a given string as value. This is different from ``-d``. For example, ``-dXYZ=35`` on the command line is equivalent to the program fragment:

.. code-block:: bash

   /XYZ 35 def

whereas ``-sXYZ=35`` is equivalent to:

.. code-block:: bash

   /XYZ (35) def



**-p** *name=string*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Define a name in systemdict with the parsed version of the given string as value. The string takes a parameter definition in (something very close to) postscript format. This allows more complex structures to be passed in than is possible with ``-d`` or ``-s``. For example:

.. code-block:: bash

   -pFoo="<< /Bar[1 2 3]/Baz 0.1 /Whizz (string) /Bang <0123> >>"

This means that ``-p`` can do the job of both ``-d`` and ``-s``. For example:

.. code-block:: bash

   -dDownScaleFactor=3

can be equivalently performed by:

.. code-block:: bash

   -pDownScaleFactor=3

and:

.. code-block:: bash

   -sPAPERSIZE=letter

can be equivalently performed by:

.. code-block:: bash

   -pPAPERSIZE="(letter)"


.. note ::

  There are some 'special' values that should be set using ``-s``, not ``-p``, such as ``DEVICE`` and ``DefaultGrayProfile``. Broadly, only use ``-p`` if you cannot set what you want using ``-s`` or ``-d``.

Also, internally, after setting an parameter with ``-p`` we perform an ``initgraphics`` operation. This is required to allow changes in parameters such as ``HWResolution`` to take effect. This means that attempting to use ``-p`` other than at the start of a page is liable to give unexpected results.


**-u** *name*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Un-define a name, cancelling ``-d`` or ``-s``.

   Note that the initialization file gs_init.ps makes systemdict read-only, so the values of names defined with ``-D``, ``-d``, ``-S``, and ``-s`` cannot be changed -- although, of course, they can be superseded by definitions in userdict or other dictionaries. However, device parameters set this way (PageSize, Margins, etc.) are not read-only, and can be changed by code in PostScript files.

**-g** *number1 x number2*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Equivalent to ``-dDEVICEWIDTH=number1`` and ``-dDEVICEHEIGHT=number2``, specifying the device width and height in pixels for the benefit of devices such as X11 windows and VESA displays that require (or allow) you to specify width and height. Note that this causes documents of other sizes to be clipped, not scaled: see ``-dFIXEDMEDIA`` below.

**-r** *number* (same as *-r number x number*)
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
**-r** *number1 x number2*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Equivalent to ``-dDEVICEXRESOLUTION=number1`` and ``-dDEVICEYRESOLUTION=number2``, specifying the device horizontal and vertical resolution in pixels per inch for the benefit of devices such as printers that support multiple X and Y resolutions.



Suppress messages
""""""""""""""""""""""""""""""""""""""

**-q**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Quiet startup: suppress normal startup messages, and also do the equivalent of :ref:`-dQUIET<DQUIET>`.


Parameter switches (-d and -s)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
   As noted above, ``-d`` and ``-s`` define initial values for PostScript names. Some of these names are parameters that control the interpreter or the graphics engine. You can also use ``-d`` or ``-s`` to define a value for any device parameter of the initial device (the one defined with ``-sDEVICE=``, or the default device if this switch is not used). For example, since the :title:`ppmraw` device has a numeric ``GrayValues`` parameter that controls the number of bits per component, ``-sDEVICE=ppmraw -dGrayValues=16`` will make this the default device and set the number of bits per component to 4 (log2(16)).


Rendering parameters
"""""""""""""""""""""""""""""""

**-dCOLORSCREEN**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
**-dCOLORSCREEN=0**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
**-dCOLORSCREEN=false**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   On high-resolution devices (at least 150 dpi resolution, or ``-dDITHERPPI`` specified), ``-dCOLORSCREEN`` forces the use of separate halftone screens with different angles for CMYK or RGB if halftones are needed (this produces the best-quality output); ``-dCOLORSCREEN=0`` uses separate screens with the same frequency and angle; ``-dCOLORSCREEN=false`` forces the use of a single binary screen. The default if ``COLORSCREEN`` is not specified is to use separate screens with different angles if the device has fewer than 5 bits per color, and a single binary screen (which is never actually used under normal circumstances) on all other devices.


**-dDITHERPPI=** *lpi*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Forces all devices to be considered high-resolution, and forces use of a halftone screen or screens with *lpi* lines per inch, disregarding the actual device resolution. Reasonable values for lpi are N/5 to N/20, where N is the resolution in dots per inch.

**-dInterpolateControl=** *control_value*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   This allows control of the image interpolation.

   By default ``InterpolateControl`` is 1 and the image rendering for images that have ``/Interpolate true`` are interpolated to the full device resolution. Otherwise, images are rendered using the nearest neighbour scaling (Bresenham's line algorithm through the image, plotting the closest texture coord at each pixel). When downscaling this results in some source pixels not appearing at all in the destination. When upscaling, each source pixels will cover at least one destination pixel.

   When the *control_value* is 0 no interpolation is performed, whether or not the file has images with ``/Interpolate true``.

   When the *control_value* is greater than 1 interpolation is performed for images with ``/Interpolate true`` as long as the image scaling factor on either axis is larger than the *control_value*. Also, the interpolation only produces images that have (device resolution / *control_value*) maximum resolution rather than full device resolution. This allows for a performance vs. quality tradeoff since the number of pixels produced by the interpolation will be a fraction of the interpolated pixels at full device resolution. Every source pixel will contribute partially to the destination pixels.

   When the ``InterpolateControl`` *control_value* is less than 0 interpolation is forced as if all images have ``/Interpolate true``, and the interpolation is controlled by the absolute value of the *control_value* as described above. Thus, ``-dInterpolateControl=-1`` forces all images to be interpolated at full device resolution.

   Computationally, image interpolation is much more demanding than without interpolation (lots of floating point muliplies and adds for every output pixel vs simple integer additions, subtractions, and shifts).

   In all but special cases image interpolation uses a Mitchell filter function to scale the contributions for each output pixel. When upscaling, every output pixel ends up being the weighted sum of 16 input pixels, When downscaling more source pixels will contribute to the interpolated pixels. Every source pixel has some effect on the output pixels.


**-dDOINTERPOLATE**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   This option still works, but is deprecated, and is the equivalent of ``-dInterpolateControl=-1``.


**-dNOINTERPOLATE**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   This option still works, but is deprecated and is the equivalent of ``-dInterpolateControl=0``.

   Turns off image interpolation, improving performance on interpolated images at the expense of image quality. ``-dNOINTERPOLATE`` overrides ``-dDOINTERPOLATE``.


**-dTextAlphaBits=** *n*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
**-dGraphicsAlphaBits=** *n*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   These options control the use of subsample antialiasing. Their use is highly recommended for producing high quality rasterizations. The subsampling box size n should be 4 for optimum output, but smaller values can be used for faster rendering. Antialiasing is enabled separately for text and graphics content. Allowed values are 1, 2 or 4.


.. note ::
   Because of the way antialiasing blends the edges of shapes into the background when they are drawn some files that rely on joining separate filled polygons together to cover an area may not render as expected with ``GraphicsAlphaBits`` at 2 or 4. If you encounter strange lines within solid areas, try rendering that file again with ``-dGraphicsAlphaBits=1``.

   Further note: because this feature relies upon rendering the input it is incompatible, and will generate an error on attempted use, with any of the vector output devices.



**-dAlignToPixels=** *n*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Chooses glyph alignent to integral pixel boundaries (if set to the value 1) or to subpixels (value 0). Subpixels are a smaller raster grid which is used internally for text antialiasing. The number of subpixels in a pixel usually is ``2^TextAlphaBits``, but this may be automatically reduced for big characters to save space in character cache.

   The parameter has no effect if ``-dTextAlphaBits=1``. Default value is 0.

   Setting ``-dAlignToPixels=0`` can improve rendering of poorly hinted fonts, but may impair the appearance of well-hinted fonts.

**-dGridFitTT=** *n*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   This specifies the initial value for the implementation specific user parameter :ref:`GridFitTT<Language_GridFitTT>`. It controls grid fitting of True Type fonts (Sometimes referred to as "hinting", but strictly speaking the latter is a feature of Type 1 fonts). Setting this to 2 enables automatic grid fitting for True Type glyphs. The value 0 disables grid fitting. The default value is 2. For more information see the description of the user parameter :ref:`GridFitTT<Language_GridFitTT>`.


**-dUseCIEColor**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Set ``UseCIEColor`` in the page device dictionary, remapping device-dependent color values through a Postscript defined CIE color space. Document ``DeviceGray``, ``DeviceRGB`` and ``DeviceCMYK`` source colors will be substituted respectively by Postscript CIEA, CIEABC and CIEDEFG color spaces. See the document :ref:`Ghostscript Color Management<GhostscriptColorManagement.html>` for details on how this option will interact with Ghostscript's ICC-based color workflow. If accurate colors are desired, it is recommended that an ICC workflow be used.

**-dNOCIE**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Substitutes ``DeviceGray`` for CIEBasedA, DeviceRGB for CIEBasedABC and CIEBasedDEF spaces and ``DeviceCMYK`` for CIEBasedDEFG color spaces. Useful only on very slow systems where color accuracy is less important.


**-dNOSUBSTDEVICECOLORS**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   This switch prevents the substitution of the ``ColorSpace`` resources (``DefaultGray``, ``DefaultRGB``, and ``DefaultCMYK``) for the ``DeviceGray``, ``DeviceRGB``, and ``DeviceCMYK`` color spaces. This switch is primarily useful for PDF creation using the :title:`pdfwrite` device when retaining the color spaces from the original document is important.

**-dNOPSICC**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Disables the automatic loading and use of an input color space that is contained in a PostScript file as DSC comments starting with the %%BeginICCProfile: comment. ICC profiles are sometimes embedded by applications to convey the exact input color space allowing better color fidelity. Since the embedded ICC profiles often use multidimensional RenderTables, color conversion may be slower than using the Default color conversion invoked when the ``-dUseCIEColor`` option is specified, therefore the ``-dNOPSICC`` option may result in improved performance at slightly reduced color fidelity.


**-dNOTRANSPARENCY**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Turns off PDF 1.4 transparency, resulting in faster (but possibly incorrect) rendering of pages containing PDF 1.4 transparency and blending.

**-dALLOWPSTRANSPARENCY**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Enables the use of the Ghostscript custom transparency operators (:ref:`Transparency<Language_Transparency>`) from Postscript input. Normally, these operators are not accessible from Postscript jobs, being primarily intended to be called by the PDF interpreter. Using ``-dALLOWPSTRANSPARENCY`` leaves them available. It is important that these operators are used correctly, especially the order in which they are called, otherwise unintended, even undefined behavior may result.

**-dNO_TN5044**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Turns off the TN 5044 psuedo operators. These psuedo operators are not a part of the official Postscript specification. However they are defined in Technical Note #5044 Color Separation Conventions for PostScript Language Programs. These psuedo operators are required for some files from QuarkXPress. However some files from Corel 9 and Illustrator 88 do not operate properly if these operators are present.

**-dDOPS**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Enables processing of Subtype /PS streams in PDF files and the DoPS operator. DoPS has in fact been deprecated for some time. Also the "PS" operator that was removed from the 1.3 2nd edition specification is also disabled by default, and enabled by ``-dDOPS``. Use of this option is NOT recommended in security-conscious applications, as it increases the scope for malicious code. ``-dDOPS`` has no effect on processing of PostScript source files. Note: in releases 7.30 and earlier, processing of DoPS was always enabled.

**-dBlackText**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Forces text to be drawn with black. This occurs for text fill and text stroke operations. PDF output created with this setting will be updated to be drawn with gray values of 0. Type 3 fonts, which are sometimes used for graphics, are not affected by this parameter. Note, works only for fills with gray, rgb, and cmyk. Pattern, separation, and deviceN fills will not be affected.


**-dBlackVector**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Forces vector stroke and fills to be drawn with black. PDF output created with this setting will be updated to be drawn with gray values of 0. Note, works only for fills with gray, rgb, and cmyk. Pattern, separation, and deviceN fills will not be affected.


**-dBlackThresholdL=** *float*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Sets the threshold for the luminance value (L*) at which that value and above will be mapped to white when using the ``BlackText`` and ``BlackVector`` option. Default is 90. Pure white has a value of 100. Pure black has a value of 0. This means that if you set ``BlackThresholdL=101``, all colors will be mapped to black. If you set ``BlackThresholdL=75``, colors that are below an L* value of 75 will be mapped to black. Colors at or above an L* of 75 will be mapped to white, depending upon the setting of ``BlackThresholdC`` (see below).


**-dBlackThresholdC=** *float*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   For colors that are at or above the value set by ``BlackThresholdL`` (or the default setting of 90), map colors to white that are within a distance of ``BlackThresholdC`` from the CIELAB neutral axis in terms of the L1 norm on the a* and b* value. All others are mapped to black. This has the effect of forcing colors with high luminance and high chrominance to black (e.g. pure yellow) while those with a lower luminance and less chrominance to white (e.g. a light gray). Default value is 3. You can visualize the region that is mapped to white as a cuboid that is centered on the CIELAB neutral axis with one end tied to the L*=100 value. The cuboid cross sections across the neutral axis are squares whose size is set by ``BlackThresholdC``. The cuboid length is set by ``BlackThresholdL`` and is effectively ``100-BlackThresholdL``.



Page parameters
"""""""""""""""""""""""""""""""


**-dFirstPage=** *pagenumber*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Begin on the designated page of the document. Pages of all documents in PDF collections are numbered sequentionally.


**-dLastPage=** *pagenumber*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Stop after the designated page of the document. Pages of all documents in PDF collections are numbered sequentionally.

**-sPageList=** *pagenumber*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   There are three possible values for this; even, odd or a list of pages to be processed. A list can include single pages or ranges of pages. Ranges of pages use the minus sign '-', individual pages and ranges of pages are separated by commas ','. A trailing minus '-' means process all remaining pages. For example:

   - ``-sPageList=1,3,5`` indicates that pages 1, 3 and 5 should be processed.
   - ``-sPageList=5-10`` indicates that pages 5, 6, 7, 8, 9 and 10 should be processed.
   - ``-sPageList=1, 5-10, 12`` indicates that pages 1, 5, 6, 7, 8, 9, 10 and 12 onwards should be processed.


The PDF interpreter and the other language interpreters handle these in slightly different ways. Because PDF files enable random access to pages in the document the PDF inerpreter only interprets and renders the required pages. PCL and PostScript cannot be handled in ths way, and so all the pages must be interpreted. However only the requested pages are rendered, which can still lead to savings in time. Be aware that using the '``%d``' syntax for ``OutputFile`` does not reflect the page number in the original document. If you chose (for example) to process even pages by using ``-sPageList=even``, then the output of ``-sOutputFile=out%d.png`` would still be ``out0.png``, ``out1.png``, ``out2.png`` etc.

Because the PostScript and PCL interpreters cannot determine when a document terminates, sending multple files as input on the command line does not reset the ``PageList`` between each document, each page in the second and subsequent documents is treated as following on directly from the last page in the first document. The PDF interpreter, however, does not work this way. Since it knows about individual PDF files the ``PageList`` is applied to each PDF file separately. So if you were to set ``-sPageList=1,2`` and then send two PDF files, the result would be pages 1 and 2 from the first file, and then pages 1 and 2 from the second file. The PostScript interpreter, by contrast, would only render pages 1 and 2 from the first file. This means you must exercise caution when using this switch, and probably should not use it at all when processing a mixture of PostScript and PDF files on the same command line.

.. _FIXEDMEDIA:

**-dFIXEDMEDIA**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Causes the media size to be fixed after initialization, forcing pages of other sizes or orientations to be clipped. This may be useful when printing documents on a printer that can handle their requested paper size but whose default is some other size. Note that ``-g`` automatically sets ``-dFIXEDMEDIA``, but ``-sPAPERSIZE=`` does not.

**-dFIXEDRESOLUTION**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Causes the media resolution to be fixed similarly. ``-r`` automatically sets ``-dFIXEDRESOLUTION``.

**-dPSFitPage**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   The page size from the PostScript file ``setpagedevice`` operator, or one of the older ``statusdict`` page size operators (such as ``letter`` or ``a4``) will be rotated, scaled and centered on the "best fit" page size from those availiable in the ``InputAttributes`` list. The ``-dPSFitPage`` is most easily used to fit pages when used with the ``-dFIXEDMEDIA`` option.

   This option is also set by the ``-dFitPage`` option.

**-dReopenPerPage**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Normally, when using a single fixed filename (e.g. `output.tif`) the Ghostscript device will not release the file handle when a page is complete, but instead will retain it for the next page. The file handle is only released when the device is closed. However, if ``-dReopenPerPage=true``, then the file handle will be released at the end of the page and a new file (with the same name) will be created for the next page, thereby creating a new file handle.

**-dORIENT1=true**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
**-dORIENT1=false**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Defines the meaning of the 0 and 1 orientation values for the ``setpage[params]`` compatibility operators. The default value of ``ORIENT1`` is true (set in ``gs_init.ps``), which is the correct value for most files that use ``setpage[params]`` at all, namely, files produced by badly designed applications that "know" that the output will be printed on certain roll-media printers: these applications use 0 to mean landscape and 1 to mean portrait. ``-dORIENT1=false`` declares that 0 means portrait and 1 means landscape, which is the convention used by a smaller number of files produced by properly written applications.

**-dDEVICEWIDTHPOINTS=** *w*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
**-dDEVICEHEIGHTPOINTS=** *h*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Sets the initial page width to w or initial page height to h respectively, specified in 1/72" units.

**-sDEFAULTPAPERSIZE=** *a4*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   This value will be used to replace the device default papersize ONLY if the default papersize for the device is 'letter' or 'a4' serving to insulate users of A4 or 8.5x11 from particular device defaults (the collection of contributed drivers in Ghostscript vary as to the default size).

**-dFitPage**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   This is a "convenience" operator that sets the various options to perform page fitting for specific file types.
   This option sets the ``-dEPSFitPage``, ``-dPDFFitPage``, and the ``-dFitPage`` options.


**-sNupControl=** *Nup_option_string*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   This option specifies the N-up nesting to be performed. The pages are scaled and arranged on the current ``PageSize`` "master" page according the the option.

   The only option strings are as follows:

   ``-sNupControl=number1xnumber2``
      Will fit *number1* nested pages across the master page, and *number2* down the master page, from the upper left, then to the right to fill the row, moving down to the leftmost place on the next row until the nest is complete.
      A partially filled nest will be output when the ``-sNupControl=`` *string* is changed, when Ghostscript exits, or when the page size changes.

   Pages are scaled to fit the requested number horizontally and vertically, maintaining the aspect ratio. If the scaling selected for fitting the nested pages leaves space horizontally on the master page, the blank area will be added to the left and right of the entire row of nested pages. If the fit results in vertical space, the blank area will be added above and below all of the rows.

   ``-sNupControl=``
      An empty string will turn off nesting. If there are any nested pages on the master page, the partially filled master page will be output.
      Printer devices typically reallocate their memory whenever the transparency use of a page changes (from one page having transparency, to the next page not having transparency, or vice versa). This would cause problems with Nup, possibly leading to lost or corrupt pages in the output. To avoid this, the Nup device changes the parameters of the page to always set the ``PageUsesTransparency`` flag. While this should be entirely transparent for the user and not cause extra transparency blending operations during the standard rendering processes for most devices, it may cause devices to use the clist rather than PageMode.


Font-related parameters
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

**-dLOCALFONTS**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Causes Type 1 fonts to be loaded into the current VM -- normally local VM -- instead of always being loaded into global VM. Useful only for compatibility with Adobe printers for loading some obsolete fonts.

.. _UseDNoFontMap:

**-dNOFONTMAP**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Suppresses the normal loading of the ``Fontmap`` file. This may be useful in environments without a file system.

**-dNOFONTPATH**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Suppresses consultation of ``GS_FONTPATH``. This may be useful for debugging.

**-dNOPLATFONTS**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Disables the use of fonts supplied by the underlying platform (X Windows or Microsoft Windows). This may be needed if the platform fonts look undesirably different from the scalable fonts.

**-dNONATIVEFONTMAP**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Disables the use of font map and corresponding fonts supplied by the underlying platform. This may be needed to ensure consistent rendering on the platforms with different fonts, for instance, during regression testing.

**-sFONTMAP=** *filename1;filename2;...*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Specifies alternate name or names for the ``Fontmap`` file. Note that the names are separated by ":" on Unix systems, by ";" on MS Windows systems, and by "," on VMS systems, just as for search paths.

**-sFONTPATH=** *dir1;dir2;...*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Specifies a list of directories that will be scanned when looking for fonts not found on the search path, overriding the environment variable ``GS_FONTPATH``.

   By implication, any paths specified by ``FONTPATH`` or ``GS_FONTPATH`` are automatically added to the permit file read list (see ":ref:`-dSAFER<dSAFER>`").

**-sSUBSTFONT=** *fontname*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Causes the given font to be substituted for all unknown fonts, instead of using the normal intelligent substitution algorithm. Also, in this case, the font returned by findfont is the actual font named fontname, not a copy of the font with its ``FontName`` changed to the requested one.


.. note ::

    THIS OPTION SHOULD NOT BE USED WITH HIGH LEVEL (VECTOR) DEVICES, such as :title:`pdfwrite`, because it prevents such devices from providing the original font names in the output document. The font specified (fontname) will be embedded instead, limiting all future users of the document to the same approximate rendering.



.. _resource related parameters:


Resource-related parameters
""""""""""""""""""""""""""""""

**-sGenericResourceDir=** *path*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Specifies a path to resource files. The value is platform dependent. It must end with a directory separator.
   A note for Windows users, Artifex recommends the use of the forward slash delimiter due to the special interpretation of ``\"`` by the Microsoft C startup code. See `Parsing C Command-Line Arguments`_ for more information.

   Adobe specifies ``GenericResourceDir`` to be an absolute path to a single resource directory. Ghostscript instead maintains multiple resource directories and uses an extended method for finding resources, which is explained in :ref:`"Finding PostScript Level 2 resources"<PS resources>`.

   Due to the extended search method, Ghostscript uses ``GenericResourceDir`` only as a default directory for resources being not installed. Therefore ``GenericResourceDir`` may be considered as a place where new resources to be installed. The default implementation of the function ``ResourceFileName`` uses ``GenericResourceDir`` when it is an absolute path, or when the resource file is absent.

   The extended search method does not call ``ResourceFileName``.

   Default value is (``./Resource/``) for Unix, and an equivalent one on other platforms.

**-sFontResourceDir=** *path*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Specifies a path where font files are installed. It's meaning is similar to ``GenericResourceDir``.

   Default value is (``./Font/``) for Unix, and an equivalent one on other platforms.



.. _Interaction related parameters:

Interaction-related parameters
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""


**-dBATCH**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Causes Ghostscript to exit after processing all files named on the command line, rather than going into an interactive loop reading PostScript commands. Equivalent to putting ``-c`` quit at the end of the command line.

**-dNOPAGEPROMPT**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Disables only the prompt, but not the pause, at the end of each page. This may be useful on PC displays that get confused if a program attempts to write text to the console while the display is in a graphics mode.

**-dNOPAUSE**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Disables the prompt and pause at the end of each page. Normally one should use this (along with ``-dBATCH``) when producing output on a printer or to a file; it also may be desirable for applications where another program is "driving" Ghostscript.

**-dNOPROMPT**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Disables the prompt printed by Ghostscript when it expects interactive input, as well as the end-of-page prompt (``-dNOPAGEPROMPT``). This allows piping input directly into Ghostscript, as long as the data doesn't refer to ``currentfile``.


.. _DQUIET:

**-dQUIET**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Suppresses routine information comments on standard output. This is currently necessary when redirecting device output to standard output.

**-dSHORTERRORS**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Makes certain error and information messages more Adobe-compatible.

**-sstdout=** *filename*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Redirect PostScript ``%stdout`` to a file or ``stderr``, to avoid it being mixed with device ``stdout``. To redirect ``stdout`` to ``stderr`` use ``-sstdout=%stderr``. To cancel redirection of stdout use ``-sstdout=%stdout`` or ``-sstdout=-``.

.. note::

   This redirects PostScript output to ``%stdout`` but does not change the destination FILE of device output as with ``-sOutputFile=-`` or even ``-sOutputFile=%stdout`` since devices write directly using the stdout ``FILE *`` pointer with C function calls such as ``fwrite`` or ``fputs``.

**-dTTYPAUSE**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Causes Ghostscript to read a character from ``/dev/tty``, rather than standard input, at the end of each page. This may be useful if input is coming from a pipe.

.. note::

   ``-dTTYPAUSE`` overrides ``-dNOPAUSE``.



Device and output selection parameters
""""""""""""""""""""""""""""""""""""""""""""""

**-dNODISPLAY**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Initializes Ghostscript with a null device (a device that discards the output image) rather than the default device or the device selected with -sDEVICE=. This is usually useful only when running PostScript code whose purpose is to compute something rather than to produce an output image.

**-sDEVICE=** *device*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Selects an alternate initial :ref:`output device<Selecting an output device>`.

**-sOutputFile=** *filename*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Selects an alternate output file (or pipe) for the initial output device, as described above.

**-d.IgnoreNumCopies=** *true*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Some devices implement support for "printing" multiple copies of the input document and some do not, usually based on whether it makes sense for a particular output format. This switch instructs all devices to ignore a request to print multiple copies, giving more consistent behaviour.


Deferred Page Rendering
""""""""""""""""""""""""""""""""""""""""""""""

Raster printers and image formats that can use the "command list" (clist) to store a representation of the page prior to rendering can use the ``--saved-pages=`` *string* on the command line for deferred rendering of pages.

Pages that are saved instead of printed are retained until the list of saved pages is emptied by the flush command of the ``saved-pages=`` command string.

Pages can be printed in reverse or normal order, or selected pages, including all even or all odd, and multiple collated copies can be produced. Since pages are saved until the flush command, pages can be printed multiple times, in any order.

Refer to the :ref:`Using Saved Pages<SavedPages.html>` document for details.




EPS parameters
""""""""""""""""""""""""""""""""""""""""""""""

**-dEPSCrop**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Crop an EPS file to the bounding box. This is useful when converting an EPS file to a bitmap.


**-dEPSFitPage**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Resize an EPS file to fit the page. This is useful for shrinking or enlarging an EPS file to fit the paper size when printing.
   This option is also set by the ``-dFitPage`` option.

**-dNOEPS**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Prevent special processing of EPS files. This is useful when EPS files have incorrect Document Structuring Convention comments.




ICC color parameters
""""""""""""""""""""""""""""""""""""""""""""""

For details about the ICC controls see the document `GS9 Color Management`_.


**-sDefaultGrayProfile=** *filename*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Set the ICC profile that will be associated with undefined device gray color spaces. If this is not set, the profile file name "default_gray.icc" will be used as the default.

**-sDefaultRGBProfile=** *filename*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Set the ICC profile that will be associated with undefined device RGB color spaces. If this is not set, the profile file name "default_rgb.icc" will be used as the default.

**-sDefaultCMYKProfile=** *filename*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Set the ICC profile that will be associated with undefined device CMYK color spaces. If this is not set, the profile file name "default_cmyk.icc" will be used as the default.

**-sDeviceNProfile=** *filename*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Associate a :title:`devicen` color space contained in a PS or PDF document with an ICC profile. Note that neither PS nor PDF provide in-document ICC profile definitions for DeviceN color spaces. With this interface it is possible to provide this definition. The colorants tag order in the ICC profile defines the lay-down order of the inks associated with the profile. A windows-based tool for creating these source profiles is contained in ``./toolbin/color/icc_creator``.

**-sOutputICCProfile=** *filename*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Set the ICC profile that will be associated with the output device. Care should be taken to ensure that the number of colorants associated with the device is the same as the profile. If this is not set, an appropriate profile (i.e. one with the proper number of colorants) will be selected from those in the directory specified by ``ICCProfilesDir`` (see below). Note that if the output device is CMYK + spot colorants, a CMYK profile can be used to provide color management for the CMYK colorants only. In this case, spot colors will pass through unprocessed assuming the device supports those colorants. It is also possible for these devices to specify NCLR ICC profiles for output.


**-sICCOutputColors=** *"Cyan, Magenta, Yellow, Black, Orange, Violet"*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   For the psdcmyk and tiffsep separation devices, the device ICC profile can be an NCLR profile, which means something that includes non-traditional inks like Orange, Violet, etc. In this case, the list of the color names in the order that they exist in the profile must be provided with this command line option. Note that if a color name that is specified for the profile occurs also within the document (e.g. "Orange" above), then these color names will be associated with the same separation. Additional names beyond those of the ICC profile component count can be included. In this case, those components will be installed into the tiffsep or psdcmyk device list of colors, following the ICC profile colors. The number of spot colors (those that go beyond the standard CMYK colors) allowed by tiffsep or psdcmyk can be set using ``-dMaxSpots=#``. The default value for this is currently set to 10 (``GS_SOFT_MAX_SPOTS``). As an example consider the case where we wish to use a 6CLR ICC profile that includes Orange and Violet, but need the device to include a specialty color component such as Varnish, which does not appear in the document and is not handled by the 6CLR ICC profile.

   In addition, we desire to allow one more spot color of the document to come through to our device. For this case using ``-sICCOutputColors="Cyan, Magenta, Yellow, Black, Orange, Violet, Varnish" -dMaxSpots=4 -sOutputICCProfile=My_6CLR_Profile.icc`` would provide the desired outcome. Note that it is up to the device or through the use of ``-sNamedProfile`` (see below) to involve the setting of any values in the Varnish channel. However, if an All color value is encountered in the document, the Varnish component will have its value set as will the Orange and Violet values (Likewise if a spot color named Varnish is encountered in the document the Varnish component will be used for the values). The All value is typically used for placing registration targets on separations. Finally, note that if an NCLR ICC profile is specified and ``ICCOutputColors`` is not used, then a set of default names will be used for the extra colorants (non-CMYK) in the profile. These names are given as ``ICC_COLOR_N`` for the Nth non-CMYK channel.

**-sProofProfile=** *filename*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Enable the specificiation of a proofing profile that will make the color management system link multiple profiles together to emulate the device defined by the proofing profile. See the document `GS9 Color Management`_ for details about this option.

**-sDeviceLinkProfile=** *filename*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Define a device link profile. This profile is used following the output device profile. Care should be taken to ensure that the output device process color model is the same as the output color space for the device link profile. In addition, the color space of the ``OutputICCProfile`` should match the input color space of the device link profile. For example, the following would be a valid specification ``-sDEVICE=tiff32nc -sOutputICCProfile=srgb.icc -sDeviceLinkProfile=linkRGBtoCMYK.icc``. In this case, the output device's color model is CMYK (tiff32nc) and the colors are mapped through sRGB and through a devicelink profile that maps sRGB to CMYK values. See the document `GS9 Color Management`_ for details about this option.


**-sNamedProfile=** *filename*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Define a structure that is to be used by the color management module (CMM) to provide color management of named colors. While the ICC does define a named color format, this structure can in practice be much more general. Many developers wish to use their own proprietary-based format for spot color management. This command option is for developer use when an implementation for named color management is designed for the function gsicc_transform_named_color located in gsicccache.c . An example implementation is currently contained in the code for the handling of both Separation and DeviceN colors. For the general user this command option should really not be used.


**-sBlendColorProfile=** *filename*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   With the PDF transparency imaging model, a color space can be specified within which the color blending operations are to take place. Some files lack this specification, in which case the blending occurs in the output device's native color space. This dependency of blending color space on the device color model can be avoided by using the above command to force a specific color space in which to perform the blending.

**-dColorAccuracy=** *0/1/2*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Set the level of accuracy that should be used. A setting of 0 will result in less accurate color rendering compared to a setting of 2. However, the creation of a transformation will be faster at a setting of 0 compared to a setting of 2. Default setting is 2.

**-dRenderIntent=** *0/1/2/3*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Set the rendering intent that should be used with the profile specified above by ``-sOutputICCProfile``. The options 0, 1, 2, and 3 correspond to the ICC intents of Perceptual, Colorimetric, Saturation, and Absolute Colorimetric.

**-dBlackPtComp=** *0/1*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Specify if black point compensation should be used with the profile specified above by ``-sOutputICCProfile``.

**-dKPreserve=** *0/1/2*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Specify if black preservation should be used when mapping from CMYK to CMYK. When using littleCMS as the CMM, the code 0 corresponds to no preservation, 1 corresponds to the ``PRESERVE_K_ONLY`` approach described in the littleCMS documentation and 2 corresponds to the ``PRESERVE_K_PLANE`` approach. This is only valid when using littleCMS for color management.

**-sVectorICCProfile=** *filename*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Set the ICC profile that will be associated with the output device for vector-based graphics (e.g. Fill, Stroke operations). Care should be taken to ensure that the number of colorants associated with the device is the same as the profile. This can be used to obtain more saturated colors for graphics.

**-dVectorIntent=** *0/1/2/3*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Set the rendering intent that should be used with vector-based graphic objects. The options are the same as specified for ``-dRenderIntent``.

**-dVectorBlackPt=** *0/1*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Specify if black point compensation should be used for vector-based graphic objects.

**-dVectorKPreserve=** *0/1/2*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Specify if black preservation should be used when mapping from CMYK to CMYK for vector-based graphic objects. The options are the same as specified for ``-dKPreserve``.

**-sImageICCProfile=** *filename*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Set the ICC profile that will be associated with the output device for images. Care should be taken to ensure that the number of colorants associated with the device is the same as the profile. This can be used to obtain perceptually pleasing images.

**-dImageIntent=** *0/1/2/3*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Set the rendering intent that should be used for images.

**-dImageBlackPt=** *0/1*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Specify if black point compensation should be used with images.


**-dImageKPreserve=** *0/1/2*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Specify if black preservation should be used when mapping from CMYK to CMYK for image objects. The options are the same as specified for ``-dKPreserve``.

**-sTextICCProfile=** *filename*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Set the ICC profile that will be associated with the output device for text. Care should be taken to ensure that the number of colorants associated with the device is the same as the profile. This can be used ensure K only text.

**-dTextIntent=** *0/1/2/3*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Set the rendering intent that should be used text objects. The options are the same as specified for ``-dRenderIntent``.

**-dTextBlackPt=** *0/1*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Specify if black point compensation should be used with text objects.

**-dTextKPreserve=** *0/1/2*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Specify if black preservation should be used when mapping from CMYK to CMYK for text objects. The options are the same as specified for ``-dKPreserve``.

**-dOverrideICC**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Override any ICC profiles contained in the source document with the profiles specified by ``sDefaultGrayProfile``, ``sDefaultRGBProfile``, ``sDefaultCMYKProfile``. Note that if no profiles are specified for the default ``Device`` color spaces, then the system default profiles will be used. For detailed override control in the specification of source colors see ``-sSourceObjectICC``.

**-sSourceObjectICC=** *filename*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   This option provides an extreme level of override control to specify the source color spaces and rendering intents to use with vector-based graphics, images and text for both RGB and CMYK source objects. The specification is made through a file that contains on a line a key name to specify the object type (e.g. ``Image_CMYK``) followed by an ICC profile file name, a rendering intent number (0 for perceptual, 1 for colorimetric, 2 for saturation, 3 for absolute colorimetric) and information for black point compensation, black preservation, and source ICC override. It is also possible to turn off color management for certain object types, use device link profiles for object types and do custom color replacements. An example file is given in ``./gs/toolbin/color/src_color/objsrc_profiles_example.txt``. Profiles to demonstrate this method of specification are also included in this folder. Note that if objects are colorimetrically specified through this mechanism other operations like ``-dImageIntent``, ``-dOverrideICC``, have no affect. Also see below the interaction with the ``-dDeviceGrayToK`` option. See further details in the document `GS9 Color Management`_.



**-dDeviceGrayToK=** *true/false*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   By default, Ghostscript will map ``DeviceGray`` color spaces to pure K when the output device is CMYK based. This may not always be desired. In particular, it may be desired to map from the gray ICC profile specified by ``-sDefaultGrayProfile`` to the output device profile. To achieve this, one should specify ``-dDeviceGrayToK=false``. Note that this option may not have any effect in cases where ``SourceObjectICC`` settings are made for gray objects. In particular, if the gray objects in ``SourceObjectICC`` are set to None, which implies that ICC color management is not to be applied to these objects, then they are treated as ``DeviceGray`` and always mapped to K values in a CMYK target device, regardless of the settings of ``-dDeviceGrayToK`` (i.e. there is no color management). If instead, the gray objects in ``SourceObjectICC`` are set to a specific ICC profile, then they are no longer ``DeviceGray`` but are ICC colors. They will be color managed, regardless of the setting of ``-dDeviceGrayToK``.

**-dUseFastColor=** *true/false*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   This is used to avoid the use of ICC profiles for source colors. This includes those that are defined by ``DeviceGray``, ``DeviceRGB`` and ``DeviceCMYK`` definitions as well as ICC-based color spaces in the source document. With UseFastColor set to true, the traditional Postscript 255 minus operations are used to convert between RGB and CMYK with black generation and undercolor removal mappings.

**-dSimulateOverprint=** *true/false*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   This option has been replaced by ``-dOverprint=``

**-dOverprint=** */enable | /disable | /simulate*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   This option provides control of overprinting. The default setting is */enable* which allows devices such as CMYK that can support overprint to leave planes unchanged under control of PostScript and PDF overprint settings.

   The */disable* setting ignores all overprint (and overprint mode) from the input.

   If */simulate* is set, then pages with overprint (or overprint mode) set for CMYK or Separation colors will be internally maintained and output to RGB or Gray devices.

.. note::

   Not all spot color overprint cases can be accurately simulated with a CMYK only device. For example, a case where you have a spot color overprinted with CMYK colors will be indistiguishable from a case where you have spot color equivalent CMYK colorants overprinted with CMYK colors, even though they may need to show significantly different overprint simulations. To obtain a full overprint simulation, use the */simulate* setting or the psdcmyk or tiffsep device, where the spot colors are kept in their own individual planes.


**-dUsePDFX3Profile=** *int*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   This option enables rendering with an output intent defined in the PDF source file. If this option is included in the command line, source device color values (e.g ``DeviceCMYK``, ``DeviceRGB``, or ``DeviceGray``) that match the color model of the output intent will be interpreted to be in the output intent color space. In addition, if the output device color model matches the output intent color model, then the destination ICC profile will be the output intent ICC profile. If there is a mismatch between the device color model and the output intent, the output intent profile will be used as a proofing profile, since that is the intended rendering. Note that a PDF document can have multiple rendering intents per the PDF specification. As such, with the option ``-dUsePDFX3Profile`` the first output intent encountered will be used. It is possible to specify a particular output intent where int is an integer (a value of 0 is the same as not specifying a number). Probing of the output intents for a particular file is possible using extractICCprofiles.ps in ``./gs/toolbin``. Finally, note that the ICC profile member entry is an option in the output intent dictionary. In these cases, the output intent specifies a registry and a standard profile (e.g. Fogra39). Ghostscript will not make use of these output intents. Instead, if desired, these standard profiles should be used with the commands specified above (e.g. ``-sOutputICCProfile``).

**-sUseOutputIntent=** *string*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Like ``-dUsePDFX3Profile`` above, this option enables rendering with an output intent defined in the PDF source file.
   This option behaves the same way as the ``-dUsePDFX3Profile``, but the selection criteria are different. Because its possible (as of PDF 2.0) for each page to have a different array, its not sufficient just to supply an array index, as the same profile might potentially be at different indices in each array.

   Instead this option takes a string, which is first compared against the ``OutputConditionIdentifier`` in each ``OutputIntent`` in the array. If the ``OutputConditionIdentifier`` is not a standard identifier then it should be ``Custom`` and the ``UseOutputIntent`` string will be matched against the value of the ``Info`` key instead. If the ``OutputConditionIdentifier`` or ``Info`` matches the value of ``UseOuttpuIntent``, then that ``OutputIntent`` is selected if the ``OutputIntent`` contains a ``DestOutputProfile`` key.


**-sICCProfilesDir=** *path*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Set a directory in which to search for the above profiles. The directory path must end with a file system delimiter.
   If the user doesn't use the ``-sICCProfilesDir=`` command line option, Ghostscript creates a default value for it by looking on the directory paths explained in `How Ghostscript finds files`_. If the current directory is the first path a test is made for the ``iccprofiles`` directory. Next, the remaining paths with the string Resource in it are tested. The prefix up to the path separator character preceding the string Resource, concatenated with the string ``iccprofiles`` is used and if this exists, then this path will be used for ``ICCProfilesDir``.

   Note that if the build is performed with ``COMPILE_INITS=1``, then the profiles contained in ``gs/iccprofiles`` will be placed in the ROM file system. If a directory is specified on the command line using ``-sICCProfilesDir=``, that directory is searched before the ``iccprofiles/`` directory of the ROM file system is searched.

.. note ::

   A note for Windows users, Artifex recommends the use of the forward slash delimiter due to the special interpretation of ``\"`` by the Microsoft C startup code. See `Parsing C Command-Line Arguments`_ for more information.



Other parameters
"""""""""""""""""""""""

**-dFILTERIMAGE**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   If set, this will ignore all images in the input (in this context image means a bitmap), these will therefore not be rendered.

**-dFILTERTEXT**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   If set, this will ignore all text in the input (just because it looks like text doesn't mean it is, it might be an image), text will therefore not be rendered.

**-dFILTERVECTOR**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   If set, this will ignore anything which is neither text nor an image.

**-dDELAYBIND**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Causes bind to remember all its invocations, but not actually execute them until the :ref:`.bindnow<Language_BindNow>` procedure is called. Useful only for certain specialized packages like pstotext that redefine operators. See the documentation for :ref:`.bindnow<Language_BindNow>` for more information on using this feature.

**-dDOPDFMARKS**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Causes pdfmark to be called for bookmarks, annotations, links and cropbox when processing PDF files. Normally, ``pdfmark`` is only called for these types for PostScript files or when the output device requests it (e.g. :title:`pdfwrite` device).


**-dJOBSERVER**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Define ``\004 (^D)`` to start a new encapsulated job used for compatibility with Adobe PS Interpreters that ordinarily run under a job server. The ``-dNOOUTERSAVE`` switch is ignored if ``-dJOBSERVER`` is specified since job servers always execute the input PostScript under a save level, although the ``exitserver`` operator can be used to escape from the encapsulated job and execute as if the ``-dNOOUTERSAVE`` was specified.
   This also requires that the input be from stdin, otherwise an error will result (Error: ``/invalidrestore in --restore--``).

   Example usage is:

.. code-block :: bash

       gs ... -dJOBSERVER - < inputfile.ps

   Or:

.. code-block :: bash

       cat inputfile.ps | gs ... -dJOBSERVER -

.. note::

   The ``^D`` does not result in an end-of-file action on stdin as it may on some PostScript printers that rely on TBCP (Tagged Binary Communication Protocol) to cause an out-of-band ``^D`` to signal EOF in a stream input data. This means that direct file actions on ``stdin`` such as ``flushfile`` and ``closefile`` will affect processing of data beyond the ``^D`` in the stream.

**-dNOCACHE**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Disables character caching. Useful only for debugging.

**-dNOGC**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Suppresses the initial automatic enabling of the garbage collector in Level 2 systems. (The ``vmreclaim`` operator is not disabled.) Useful only for debugging.

**-dNOOUTERSAVE**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Suppresses the initial save that is used for compatibility with Adobe PS Interpreters that ordinarily run under a job server. If a job server is going to be used to set up the outermost save level, then ``-dNOOUTERSAVE`` should be used so that the restore between jobs will restore global VM as expected.

**-dNOSAFER**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Equivalent to ``-dDELAYSAFER``.

   This flag disables ``SAFER`` mode until the ``.setsafe`` procedure is run. This is intended for clients or scripts that cannot operate in ``SAFER`` mode. If Ghostscript is started with ``-dNOSAFER`` or ``-dDELAYSAFER``, PostScript programs are allowed to read, write, rename or delete any files in the system that are not protected by operating system permissions.

   This mode should be used with caution, and ``.setsafe`` should be run prior to running any PostScript file with unknown contents.




.. _Use Safer:
.. _dSAFER:


**-dSAFER**
^^^^^^^^^^^^

   .. important ::

      Ghostscript now (as of 9.50) defaults to ``SAFER`` being active.

   Enables access controls on files. Access controls fall into three categories, files from which Ghostscript is permitted to read, ones to which it is permitted to write, and ones over which it has "control" (i.e. delete/rename). These access controls apply to all files accessed via Ghostscript's internal interface to the C library file handling. Whilst we have taken considerable pains to ensure that all the code we maintain (as well as the so called "contrib" devices, that are devices included in our release packages, but not strictly maintained by the Ghostscript development team) uses this interface, we have no control over thirdparty code.


   This is an entirely new implementation of ``SAFER`` for Ghostscript versions 9.50 and later. Earlier versions relied on storing the file permission lists in Postscript VM (Virtual Memory), and only applied file access permissions to the Postscript file related operators. It relied on restricting the function of setpagedevice to avoid the device code from being manipulated into opening arbitrary files. The application of the file permissions was done within the internal context of the Postscript interpreter, and some other aspects of the Postscript restrictions were applied in the Postscript environment. With so many of the feature's capabilities relying on the Postscript context and environment, by using other (Ghostscript specific) features maliciously, the restrictions could be overridden.

   Whilst the path storage and application of the permissions is implemented entirely in C, it is still possible for Postscript to add and remove paths from the permissions lists (see .addcontrolpath) until such time as the access controls are enabled (see :ref:`.activatepathcontrol<Language_ActivateControlPath>`), any call to :ref:`.addcontrolpath<Language_AddControlPath>` after :ref:`.activatepathcontrol<Language_ActivateControlPath>` will result in a ``Fatal`` error causing the interpreter to immediately exit.

   An incompatibility exists between the pre-9.50 and 9.50 and later ``SAFER``. By removing storage and application entirely from the Postscript language environment and internal context, ``SAFER`` is no longer affected by Postscript save/restore operations. Previously, it was possible to do the equivalent of:


   .. code-block :: bash

      save
      .setsafe
      Postscript ops
      restore

   In that sequence, the Postscript ops would run with ``SAFER`` protection but after the restore, ``SAFER`` would no longer be in force. This is no longer the case. After the call to ``.setsafe`` the file controls are in force until the interpreter exits. As the 9.50 and later implementation no longer restricts the operation of ``setpagedevice``, and because this capability is extremely rarely used, we feel the improvement in security warrants the small reduction in flexibility.

   Path matching is very simple: it is case sensitive, and we do not implement full featured "globbing" or regular expression matching (such complexity would significantly and negatively impact performance). Further, the string parameter(s) passed to the ``--permit-file-*`` option must exactly match the string(s) used to reference the file(s): for example, you cannot use an absolute path to grant permission, and then a relative path to reference the file (or vice versa) - the path match will fail. Similarly, you cannot grant permission through one symlink, and then reference a file directly, or through an alternative symlink - again, the matching will fail.


   The following cases are handled:

   .. code-block :: bash

      "/path/to/file"

   Permits access only to the file: "/path/to/file"

   .. code-block :: bash

      "/path/to/directory/"

   Permits access to any file in, and only in, the directory: "/path/to/directory"

   .. code-block :: bash

      "/path/to/directory/*"

   Permits access to any file in the directory: "/path/to/directory" and any child of that directory.


   .. important :: Note for Windows Users

      The file/path pattern matching is case sensitive, even on Windows. This is a change in behaviour compared to the old code which, on Windows, was case insensitive. This is in recognition of changes in Windows behaviour, in that it now supports (although does not enforce) case sensitivity.


   Four command line parameters permit explicit control of the paths included in the access control lists:

   .. code-block :: bash

      --permit-file-read=pathlist

   Adds a path, or list of paths, to the "permit read" list. A list of paths is a series of paths separated by the appropriate path list separator for your platform (for example, on Unix-like systems it is ":" and on MS Windows it is ";").

   .. code-block :: bash

      --permit-file-write=pathlist

   Adds a path, or list of paths, to the "permit write" list. A list of paths is a series of paths separated by the appropriate path list separator for your platform (for example, on Unix-like systems it is ":" and on MS Windows it is ";").

   .. code-block :: bash

      --permit-file-control=pathlist

   Adds a path, or list of paths, to the "permit control" list. A list of paths is a series of paths separated by the appropriate path list separator for your platform (for example, on Unix-like systems it is ":" and on MS Windows it is ";").

   .. code-block :: bash

      --permit-file-all=pathlist

   Adds a path, or list of paths, to the all the above lists. A list of paths is a series of paths separated by the appropriate path list separator for your platform (for example, on Unix-like systems it is ":" and on MS Windows it is ";").

   '``*``' may be used as a wildcard in the above paths to mean "any character other than the directory separator. Do not use two or more ``*``'s without intervening characters.

   Finally, paths supplied on the command line (such as those in ``-I``, ``-sFONTPATH`` parameters) are added to the permitted reading list. Similarly, paths read during initialisation from ``Fontmap``, ``cidfmap``, and the platform specific font file enumeration (e.g. ``fontconfig`` on Unix systems) are automatically added to the permit read lists.


**-dPreBandThreshold=** *true/false*
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   If the target device is a halftone device, then images that are normally stored in the command list during banded output will be halftoned during the command list writing phase, if the resulting image will result in a smaller command list. The decision to halftone depends upon the output and source resolution as well as the output and source color space.

**-dWRITESYSTEMDICT**
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
   Leaves systemdict writable. This is necessary when running special utility programs such as ``font2c`` and ``pcharstr``, which must bypass normal PostScript access protection.


.. _Use_Improving Performance:

Improving performance
----------------------------------------------

Ghostscript attempts to find an optimum balance between speed and memory consumption, but there are some cases in which you may get a very large speedup by telling Ghostscript to use more memory.

Please note that this discussion relates to devices which produce a bitmap format as the output. These parameters have no effect on the vector devices, such as :title:`pdfwrite`.

- For raster printers and image format (jpeg*, tiff*, png* ...) devices, performance can be 'tuned' by adjusting some of the parameters related to banding (clist) options (refer to: :ref:`Banding Parameters<Language_BandingParams>`).

- All devices may use a display list ("clist") and use banding when rendering PDF 1.4 transparency. This prevents allocation of excessively large amounts of memory for the transparency buffer stack. The ``-dMaxBitmap=`` option is used to control when to use the display list, and the other banding parameters mentioned above control the band size.

   In general, page buffer mode is faster than banded/clist mode (a full page buffer is used when ``-dMaxBitmap=#`` is large enough for the entire raster image) since there is no need to write, then interpret the clist data.

   On a multi-core system where multiple threads can be dispatched to individual processors/cores, banding mode may provide higher performance since ``-dNumRenderingThreads=#`` can be used to take advantage of more than one CPU core when rendering the clist. The number of threads should generally be set to the number of available processor cores for best throughput.

   In general, larger ``-dBufferSpace=#`` values provide slightly higher performance since the per-band overhead is reduced.

- If you are using X Windows, setting the ``-dMaxBitmap=`` parameter described in `X device parameters`_ may dramatically improve performance on files that have a lot of bitmap images.

- With some PDF files, or if you are using Chinese, Japanese, or other fonts with very large character sets, adding the following sequence of switches before the first file name may dramatically improve performance at the cost of an additional memory. For example, to allow use of 30Mb of extra RAM use: ``-c 30000000 setvmthreshold -f``.

   This can also be useful in processing large documents when using a high-level (vector) output device (like :title:`pdfwrite`) that maintains significant internal state.

- For pattern tiles that are very large, Ghostscript uses an internal display list (memory based clist), but this can slow things down. The current default threshold is 8Mb -- pattern tiles larger than this will be cached as clist rather than bitmap tiles. The parameter ``-dMaxPatternBitmap=#`` can be used to adjust this threshold, smaller to reduce memory requirements and larger to avoid performance impacts due to clist based pattern handling.

   For example, ``-dMaxPatternBitmap=200000`` will use clist based patterns for pattern tiles larger than 200,000 bytes.



Summary of environment variables
----------------------------------------------

GS, GSC (MS Windows only)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~
   Specify the names of the Ghostscript executables. ``GS`` brings up a new typein window and possibly a graphics window; GSC uses the DOS console. If these are not set, ``GS`` defaults to ``gswin32``, and ``GSC`` defaults to ``gswin32c``.

GS_DEVICE
~~~~~~~~~~~~~~
   Defines the default output device. This overrides the compiled-in default, but is overridden by any command line setting.

GS_FONTPATH
~~~~~~~~~~~~~~
   Specifies a list of directories to scan for fonts if a font requested can't be found anywhere on the search path.

GS_LIB
~~~~~~~~~~
   Provides a search path for initialization files and fonts.

GS_OPTIONS
~~~~~~~~~~~~~
   Defines a list of command-line arguments to be processed before the ones actually specified on the command line. For example, setting ``GS_DEVICE`` to ``XYZ`` is equivalent to setting ``GS_OPTIONS`` to ``-sDEVICE=XYZ``. The contents of ``GS_OPTIONS`` are not limited to switches; they may include actual file names or even "@file" arguments.

TEMP, TMPDIR
~~~~~~~~~~~~~~~~
   Defines a directory name for temporary files. If both ``TEMP`` and ``TMPDIR`` are defined, ``TMPDIR`` takes precedence.


.. _Use_Debugging:


Debugging
----------------------------------------------

The information here describing is probably interesting only to developers.

Debug switches
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

There are several debugging switches that are detected by the interpreter. These switches are available whether or not Ghostscript was built with the ``DEBUG`` macro defined to the compiler (refer to building a debugging configuration).

Previous to 8.10, there was a single ``DEBUG`` flag, enabled with ``-dDEBUG`` on the command line. Now there are several debugging flags to allow more selective debugging information to be printed containing only what is needed to investigate particular areas. For backward compatibilty, the ``-dDEBUG`` option will set all of the subset switches.


.. list-table::
   :widths: 50 50

   * - -dCCFONTDEBUG
     - Compiled Fonts
   * - -dCFFDEBUG
     - CFF Fonts
   * - -dCMAPDEBUG
     - CMAP
   * - -dDOCIEDEBUG
     - CIE color
   * - -dEPSDEBUG
     - EPS handling
   * - -dFAPIDEBUG
     - Font API
   * - -dINITDEBUG
     - Initialization
   * - -dPDFDEBUG
     - PDF Interpreter
   * - -dPDFWRDEBUG
     - PDF Writer
   * - -dSETPDDEBUG
     - setpagedevice
   * - -dSTRESDEBUG
     - Static Resources
   * - -dTTFDEBUG
     - TTF Fonts
   * - -dVGIFDEBUG
     - ViewGIF
   * - -dVJPGDEBUG
     - ViewJPEG

The PDF interpreter normally tries to repair, or ignore, all problems encountered in PDF files. Setting ``-dPDFSTOPONERROR`` instead causes the interpreter to signal an error and stop processing the PDF file, instead of printing a warning.

The ``-dPDFSTOPONWARNING`` switch behaves the same, but will stop if a condition which would normally merit a warning (instead of an error) is encountered. Note that setting ``-dPDFSTOPONWARNING`` also sets ``-dPDFSTOPONERROR``.

The ``-Z`` and ``-T`` switches apply only if the interpreter was :ref:`built for a debugging configuration<Make_Debugging>`. In the table below, the first column is a debugging switch, the second is an equivalent switch (if any) and the third is its usage.



.. list-table::
   :widths: 20 20 60
   :header-rows: 1

   * - Switch
     - Equivalent switch
     - Usage
   * - ``0``
     -
     - garbage collector, minimal detail
   * - ``1``
     -
     - type 1 and type 42 font interpreter
   * - ``2``
     -
     - curve subdivider/rasterizer
   * -
     - ``3``
     - curve subdivider/rasterizer, detail
   * - ``4``
     -
     - garbage collector (strings)
   * -
     - ``5``
     - garbage collector (strings, detail)
   * - ``6``
     -
     - garbage collector (clumps, roots)
   * -
     - ``7``
     - garbage collector (objects)
   * -
     - ``8``
     - garbage collector (refs)
   * -
     - ``9``
     - garbage collector (pointers)
   * - ``a``
     -
     - allocator (large blocks only)
   * -
     - ``A``
     - allocator (all calls)
   * - ``b``
     -
     - bitmap image processor
   * -
     - ``B``
     - bitmap images, detail
   * - ``c``
     -
     - color/halftone mapper
   * - ``d``
     -
     - dictionary put/undef
   * -
     - ``D``
     - dictionary lookups
   * - ``e``
     -
     - external (OS-related) calls
   * - ``f``
     -
     - fill algorithm (summary)
   * -
     - ``F``
     - fill algorithm (detail)
   * - ``g``
     -
     - gsave/grestore[all]
   * - ``h``
     -
     - halftone renderer
   * -
     - ``H``
     - halftones, every pixel
   * - ``i``
     -
     - interpreter, just names
   * -
     - ``I``
     - interpreter, everything
   * - ``j``
     -
     - (Japanese) composite fonts
   * - ``k``
     -
     - character cache and xfonts
   * -
     - ``K``
     - character cache, every access
   * - ``l``
     -
     - command lists, bands
   * -
     - ``L``
     - command lists, everything
   * - ``m``
     -
     - makefont and font cache
   * - ``n``
     -
     - name lookup (new names only)
   * - ``o``
     -
     - outliner (stroke)
   * -
     - ``O``
     - stroke detail
   * - ``p``
     -
     - band list paths
   * -
     - ``P``
     - all paths
   * - ``q``
     -
     - clipping
   * - ``r``
     -
     - arc renderer
   * - ``s``
     -
     - streams
   * -
     - ``S``
     - scanner
   * - ``t``
     -
     - tiling algorithm
   * - ``u``
     -
     - undo saver (for save/restore), finalization
   * -
     - ``U``
     - undo saver, more detail
   * - ``v``
     -
     - compositors: alpha/transparency/overprint/rop
   * -
     - ``V``
     - compositors: alpha/transparency/overprint/rop, more detail
   * - ``w``
     -
     - compression encoder/decoder
   * - ``x``
     -
     - transformations
   * - ``y``
     -
     - Type 1 hints
   * -
     - ``Y``
     - Type 1 hints, every access
   * - ``z``
     -
     - trapezoid fill
   * - ``#``
     -
     - operator error returns
   * - ``%``
     -
     - externally processed comments
   * - ``*``
     -
     - image and RasterOp parameters
   * - ``:``
     -
     - command list and allocator/time summary
   * - ``~``
     -
     - math functions and Functions
   * - ``'``
     -
     - contexts, create/destroy
   * -
     - ``"``
     - contexts, every operation
   * - ``^``
     -
     - reference counting
   * - ``_``
     -
     - high-level (vector) output
   * - ``!``
     -
     - Postscript operator names (this option is available only when Ghostscript is compiled with a predefined macro DEBUG_TRACE_PS_OPERATORS)
   * - ``|``
     -
     - (reserved for experimental code)


The following switch affects what is printed, but does not select specific items for printing:


.. list-table::
   :widths: 20 20 60
   :header-rows: 1

   * - Switch
     - Equivalent switch
     - Usage
   * - ``/``
     -
     - include file name and line number on all trace output


These switches select debugging options other than what should be printed:

.. We need to add a special piece of raw HTML to print a dingle backtick character in RST

.. |bt| raw:: html

    <code class="code docutils literal notranslate">`</code>


.. list-table::
   :widths: 20 20 60
   :header-rows: 1

   * - Switch
     - Equivalent switch
     - Usage
   * - ``$``
     -
     - set unused parts of object references to identifiable garbage values
   * - ``+``
     -
     - use minimum-size stack blocks
   * - ``,``
     -
     - don't use path-based banding
   * - |bt|
     -
     - don't use high-level banded images
   * - ``?``
     -
     - validate pointers before, during and after garbage collection, also before and after save and restore; also make other allocator validity checks
   * - ``@``
     -
     - fill newly allocated, garbage-collected, and freed storage with a marker (a1, c1, and f1 respectively)
   * - ``f``
     -
     - the filling algorithm with characters
   * -
     - ``F``
     - the filling algorithm with non-character paths
   * - ``h``
     -
     - the Type 1 hinter
   * - ``s``
     -
     - the shading algorithm
   * -
     - ``S``
     - the stroking algorithm


Switches used in debugging
"""""""""""""""""""""""""""


.. list-table::
   :widths: 50 50
   :header-rows: 1

   * - Switch
     - Description
   * - ``-Bsize``
     - Run all subsequent files named on the command line (except for ``-F``) through the ``run_string`` interface, using a buffer of size bytes.
   * - ``-B-``
     - Turn off ``-B``: run subsequent files (except for ``-F``) directly in the normal way.
   * - ``-Ffile``
     - Execute the file with ``-B1`` temporarily in effect
   * - ``-Kn``
     - Limit the total amount of memory that the interpreter can have allocated at any one time to nK bytes. n is a positive decimal integer.
   * - ``-Mn``
     - Force the interpreter's allocator to acquire additional memory in units of nK bytes, rather than the default 20K.

       n is a positive decimal integer, on 16-bit systems no greater than 63.
   * - ``-Nn``
     - Allocate space for nK names, rather than the default (normally 64K).

       n may be greater than 64 only if ``EXTEND_NAMES`` was defined (in ``inameidx.h``) when the interpreter was compiled.
   * - ``-Zxxx``

       ``-Z-xxx``
     - Turn debugging printout on (off). Each of the xxx characters selects an option.

       Case is significant: "a" and "A" have different meanings.
   * - ``-Txxx``

       ``-T-xxx``
     - Turn Visual Trace on (off). Each of the xxx characters selects an option.

       Case is significant: "f" and "F" have different meanings.



In addition, calling Ghostscript with ``--debug`` will list all the currently defined (non visual trace) debugging flags, both in their short form (as listed above for use with ``-Z``) and in a long form, which can be used as in: ``--debug=tiling,alloc``. All the short form flags for ``-Z`` have an equivalent long form. Future flags may be added with a long form only (due to all the short form flags being used already).




Visual Trace
~~~~~~~~~~~~~~~~~~

Visual Trace allows to view internal Ghostscript data in a graphical form while execution of C code. Special :ref:`instructions<Lib_VisualTrace>` to be inserted into C code for generating the output. Client application rasterizes it into a window.

Currently the rasterization is implemented for Windows only, in clients ``gswin32.exe`` and ``gswin32c.exe``. They open Visual Trace window when graphical debug output appears, ``-T`` :ref:`switch<Debug switches>` is set, and Ghostscript was :ref:`built<Make_Debugging>` with ``DEBUG`` option.

There are two important incompletenesses of the implementation :

#. The graphical output uses a hardcoded scale. An advanced client would provide a scale option via user interface.

#. Breaks are not implemented in the client. If you need a step-by-step view, you should use an interactive C debugger to delay execution at breakpoints.


.. _Known Paper Sizes:


Appendix: Paper sizes known to Ghostscript
----------------------------------------------

The paper sizes known to Ghostscript are defined at the beginning of the initialization file ``gs_statd.ps``; see the comments there for more details about the definitions. The table here lists them by name and size. ``gs_statd.ps`` defines their sizes exactly in points, and the dimensions in inches (at 72 points per inch) and centimeters shown in the table are derived from those, rounded to the nearest 0.1 unit. A guide to international paper sizes can be found at `papersizes.org`_.


U.S. standard
~~~~~~~~~~~~~~~~

.. list-table::
   :widths: 20 20 20 20 20
   :header-rows: 2

   * - Name
     - Inches
     - mm
     - Points
     - Notes
   * -
     - W x H
     - W x H
     - W x H
     -
   * - 11 x 17
     - 11.0 x 17.0
     - 279 x 432
     - 792 x 1224
     - 11×17in portrait
   * - ledger
     - 17.0 x 11.0
     - 432 x 279
     - 1224 x 792
     - 11×17in landscape
   * - legal
     - 8.5 x 14.0
     - 216 x 356
     - 612 x 1008
     -
   * - letter
     - 8.5 x 11.0
     - 216 x 279
     - 612 x 792
     -
   * - letter small
     - 8.5 x 11.0
     - 216 x 279
     - 612 x 792
     -
   * - archA
     - 9.0 x 12.0
     - 229 x 305
     - 648 x 864
     -
   * - archB
     - 12.0 x 18.0
     - 305 x 457
     - 864 x 1296
     -
   * - archC
     - 18.0 x 24.0
     - 457 x 610
     - 1296 x 1728
     -
   * - archD
     - 24.0 x 36.0
     - 610 x 914
     - 1728 x 2592
     -
   * - archE
     - 36.0 x 48.0
     - 914 x 1219
     - 2592 x 3456
     -

ISO standard
~~~~~~~~~~~~~~~~


.. list-table::
   :widths: 20 20 20 20 20
   :header-rows: 2

   * - Name
     - Inches
     - mm
     - Points
     - Notes
   * -
     - W x H
     - W x H
     - W x H
     -
   * - a0
     - 33.1   x  46.8
     - 841    x  1189
     - 2384    x 3370
     -
   * - a1
     - 23.4   x  33.1
     - 594   x   841
     - 1684    x 2384
     -
   * - a2
     - 16.5  x   23.4
     - 420    x  594
     - 1191   x  1684
     -
   * - a3
     - 11.7  x   16.5
     - 297   x   420
     - 842    x  1191
     -
   * - a4
     - 8.3   x   11.7
     - 210   x   297
     - 595    x  842
     -
   * - a4small
     - 8.3   x   11.7
     - 210   x   297
     - 595    x  842
     -
   * - a5
     - 5.8   x  8.3
     - 148   x   210
     - 420   x   595
     -
   * - a6
     - 4.1    x  5.8
     - 105   x   148
     - 297    x  420
     -
   * - a7
     - 2.9   x   4.1
     - 74  x  105
     - 210   x   297
     -
   * - a8
     - 2.1  x    2.9
     - 52  x  74
     - 148   x   210
     -
   * - a9
     - 1.5   x   2.1
     - 37  x  52
     - 105   x   148
     -
   * - a10
     - 1.0   x   1.5
     - 26  x  37
     - 73  x  105
     -
   * - isob0
     - 39.4  x   55.7
     - 1000  x   1414
     - 2835   x  4008
     -
   * - isob1
     - 27.8   x  39.4
     - 707    x  1000
     - 2004   x  2835
     -
   * - isob2
     - 19.7   x  27.8
     - 500    x  707
     - 1417   x  2004
     -
   * - isob3
     - 13.9  x   19.7
     - 353    x  500
     - 1001   x  1417
     -
   * - isob4
     - 9.8   x   13.9
     - 250   x   353
     - 709   x   1001
     -
   * - isob5
     - 6.9   x   9.8
     - 176   x   250
     - 499   x   709
     -
   * - isob6
     - 4.9   x   6.9
     - 125   x   176
     - 354   x   499
     -
   * - c0
     - 36.1   x  51.1
     - 917   x   1297
     - 2599  x   3677
     -
   * - c1
     - 25.5  x   36.1
     - 648   x   917
     - 1837  x   2599
     -
   * - c2
     - 18.0  x   25.5
     - 458   x   648
     - 1298  x   1837
     -
   * - c3
     - 12.8  x   18.0
     - 324   x   458
     - 918   x   1298
     -
   * - c4
     - 9.0   x   12.8
     - 229   x   324
     - 649   x   918
     -
   * - c5
     - 6.4   x   9.0
     - 162   x   229
     - 459   x   649
     -
   * - c6
     - 4.5    x  6.4
     - 114   x   162
     - 323   x   459
     -



JIS standard
~~~~~~~~~~~~~~~~

.. list-table::
   :widths: 20 20 20
   :header-rows: 2

   * - Name
     - mm
     - Notes
   * -
     - W x H
     -
   * - jisb0
     - 1030   x  1456
     -
   * - jisb1
     - 728    x  1030
     -
   * - jisb2
     - 515   x   728
     -
   * - jisb
     - 364   x   515
     -
   * - jisb4
     - 257   x   364
     -
   * - jisb5
     - 182   x   257
     -
   * - jisb6
     - 128   x   182
     -



ISO/JIS switchable
~~~~~~~~~~~~~~~~~~~~

.. list-table::
   :widths: 20
   :header-rows: 1

   * - Name
   * - b0
   * - b1
   * - b2
   * - b3
   * - b4
   * - b5

.. note::

   Initially the B paper sizes are the ISO sizes, e.g., **b0** is the same as **isob0**. Running the file ``lib/jispaper.ps`` makes the B paper sizes be the JIS sizes, e.g., **b0** becomes the same as **jisb0**.



Other
~~~~~~~~~~~

.. list-table::
   :widths: 20 20 20 20 20
   :header-rows: 2

   * - Name
     - Inches
     - mm
     - Points
     - Notes
   * -
     - W x H
     - W x H
     - W x H
     -
   * - flsa
     - 8.5   x   13.0
     - 216   x   330
     - 612   x   936
     - U.S. foolscap
   * - flse
     - 8.5   x   13.0
     - 216   x   330
     - 612   x   936
     - European foolscap
   * - halfletter
     - 5.5   x   8.5
     - 140   x   216
     - 396   x   612
     -
   * - hagaki
     - 3.9   x   5.8
     - 100   x   148
     - 283   x   420
     - Japanese postcard


.. External links



.. _GS9 Color Management: https://ghostscript.com/doc/current/GS9_Color_Management.pdf
.. _Parsing C Command-Line Arguments: http://msdn.microsoft.com/en-us/library/a1y7w461.aspx
.. _papersizes.org: https://www.papersizes.org/



.. include:: footer.rst

