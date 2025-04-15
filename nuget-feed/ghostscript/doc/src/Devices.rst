.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: Details of Ghostscript Output Devices


.. include:: header.rst

.. _Devices.html:


Details of Ghostscript Output Devices
=============================================


For other information, see the :ref:`Ghostscript overview<Ghostscript Introduction>`. You may also be interested in :ref:`how to build Ghostscript<Make.html>` and :ref:`install<Install.html>` it, as well as the description of the :ref:`driver interface<Drivers.html>`.

Documentation for some older, superceded devices has been moved to :ref:`unsupported devices<UnsupportedDevices.html>`. In general such devices are deprecated and will be removed in future versions of Ghostscript. In general all older printer drivers can be replaced by the :title:`ijs` interface and one of the available 3rd party raster driver collections. We recommend moving to the :title:`ijs` device for all such printing.


Documentation for device subclassing can be found on the :ref:`Device Subclassing<DeviceSubclassing.html>` page.


.. _Devices_Notes on measurements:

Notes on measurements
----------------------------------------------

Several different important kinds of measures appear throughout this document: :ref:`inches<Inches>`, :ref:`centimeters and millimeters<Centimeters and millimeters>`, :ref:`points<Points>`, :ref:`dots per inch<Dots per inch>` and :ref:`bits per pixel<Bits per pixel>`.



Inches
~~~~~~~~~~~~~~~~~~~~~~~~~~~~
   1 inch equals 2.54 centimeters. The inch measure is sometimes represented by in or a quotation mark (") to the right of a measure, like 8.5in or 8.5". U.S. "letter" paper is exactly 8.5in×11in, approximately 21.6cm×27.9cm. (See in the usage documentation all the :ref:`paper sizes predefined in Ghostscript<Known Paper Sizes>`.)



Centimeters and millimeters
~~~~~~~~~~~~~~~~~~~~~~~~~~~~
   ISO standard paper sizes such as A4 and A3 are commonly represented in the SI units of centimeters and millimeters. Centimeters are abbreviated cm, millimeters mm. ISO A4 paper is quite close to 210×297 millimeters (approximately 8.3×11.7 inches).



Points
~~~~~~~~~~~~~~~~~~~~~~~~~~~~
   Points are a measure traditionally used in the printing trade and now in PostScript, which specifies exactly 72 points per inch (approximately 28.35 per centimeter). The :ref:`paper sizes known to Ghostscript<Known Paper Sizes>` are defined in the initialization file ``gs_statd.ps`` in terms of points.



Dots per inch
~~~~~~~~~~~~~~~~~~~~~~~~~~~~
   Dots per inch or "dpi" is the common measure of printing resolution in the US.



Bits per pixel
~~~~~~~~~~~~~~~~~~~~~~~~~~~~
   Commonly abbreviated "bpp" this is the number of digital bits used to represent the color of each pixel. This is also referred to as 'bit depth' or 'pixel depth'.




Image file formats
----------------------------------------------

Ghostscript supports output to a variety of image file formats and is widely used for rasterizing postscript and pdf files. A collection of such formats ('output devices' in Ghostscript terminology) are described in this section.

Here are some commonly useful driver options that apply to all raster drivers. Options specific to particular file formats are described in their respective sections below.

.. code-block:: bash

   -sOutputFile=filename

This is a general option telling Ghostscript what to name the output. It can either be a single filename 'tiger.png' or a template 'figure-%03d.jpg' where the %03d is replaced by the page number.

.. code-block:: bash

   -rres
   -rxresxyres

This option sets the resolution of the output file in dots per inch. The default value if you don't specify this options is usually 72 dpi.

.. code-block:: bash

   -dTextAlphaBits=n
   -dGraphicsAlphaBits=n

These options control the use of subsample antialiasing. Their use is highly recommended for producing high quality rasterizations of the input files. The size of the subsampling box n should be 4 for optimum output, but smaller values can be used for faster rendering. Antialiasing is enabled separately for text and graphics content.

Because this feature relies upon rendering the input it is incompatible, and will generate an error on attempted use, with any of the vector output devices.


It is also conventional to call Ghostscript with the ``-dSAFER -dBATCH -dNOPAUSE`` trio of options when rasterizing to a file. These suppress interactive prompts and enable some security checks on the file to be run. Please see the :ref:`Using Ghostscript<Use.html>` section for further details.



.. _Devices_PNG:

PNG file format
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

PNG (pronounced 'ping') stands for Portable Network Graphics, and is the recommended format for high-quality images. It supports full quality color and transparency, offers excellent lossless compression of the image data, and is widely supported. Please see the `PNG website`_ for a complete description of the format.

Ghostscript provides a variety of devices for PNG output varying by bit depth. For normal use we recommend :title:`png16m` for 24-bit RGB color, or :title:`pnggray` for grayscale. The :title:`png256`, :title:`png16` and :title:`pngmono` devices respectively provide 8-bit color, 4-bit color and black-and-white for special needs. The :title:`pngmonod` device is also a black-and-white device, but the output is formed from an internal 8 bit grayscale rendering which is then error diffused and converted down to 1bpp.

The :title:`png16malpha` and :title:`pngalpha` devices are 32-bit RGBA color with transparency indicating pixel coverage. The background is transparent unless it has been explicitly filled. PDF 1.4 transparent files do not give a transparent background with this device. The devices differ, in that the :title:`pngalpha` device enables Text and graphics anti-aliasing by default. We now recommend that people use the :title:`png16malpha` device in preference, and achieve any required antialiasing via the ``DownScaleFactor`` parameter, as this gives better results in many cases.

Options
""""""""""

The :title:`pngmonod`, :title:`png16m`, :title:`pnggray`, :title:`png16malpha` and :title:`pngalpha` devices all respond to the following:


.. code-block:: bash

   -dDownScaleFactor=integer

This causes the internal rendering to be scaled down by the given (integer <= 8) factor before being output. For example, the following will produce a 200dpi output png from a 600dpi internal rendering:

.. code-block:: bash

   gs -sDEVICE=png16m -r600 -dDownScaleFactor=3 -o tiger.png\
      examples/tiger.eps


The :title:`pngmonod` device responds to the following option:

.. code-block:: bash

   -dMinFeatureSize=state (0 to 4; default = 1)

This option allows a minimum feature size to be set; if any output pixel appears on its own, or as part of a group of pixels smaller than ``MinFeatureSize`` x ``MinFeatureSize``, it will be expanded to ensure that it does. This is useful for output devices that are high resolution, but that have trouble rendering isolated pixels.

While this parameter will accept values from 0 to 4, not all are fully implemented. 0 and 1 cause no change to the output (as expected). 2 works as specified. Values of 3 and 4 are accepted for compatibility, but behave as for 2.

The :title:`png16malpha` and :title:`pngalpha` devices respond to the following option:

.. code-block:: bash

   -dBackgroundColor=16#RRGGBB (RGB color, default white = 16#ffffff)

For the :title:`png16malpha` and :title:`pngalpha` devices only, set the suggested background color in the PNG bKGD chunk. When a program reading a PNG file does not support alpha transparency, the PNG library converts the image using either a background color if supplied by the program or the bKGD chunk. One common web browser has this problem, so when using ``<body bgcolor="CCCC00">`` on a web page you would need to use ``-dBackgroundColor=16#CCCC00`` when creating alpha transparent PNG images for use on the page.


Examples
""""""""""

Examples of how to use Ghostscript to convert postscript to PNG image files:


.. code-block:: bash

   gs -dSAFER -dBATCH -dNOPAUSE -sDEVICE=png16m -dGraphicsAlphaBits=4 \
      -sOutputFile=tiger.png examples/tiger.png
   gs -dSAFER -dBATCH -dNOPAUSE -r150 -sDEVICE=pnggray -dTextAlphaBits=4 \
      -sOutputFile=doc-%02d.png doc.pdf

In commercial builds, the :title:`png16m` device will accept a ``-dDeskew`` option to automatically detect/correct skew when generating output bitmaps.



.. _Devices_JPG:

JPEG file format (JFIF)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Ghostscript includes output drivers that can produce jpeg files from postscript or pdf images. These are the :title:`jpeg` and :title:`jpeggray` devices.

Technically these produce Independent JPEG Group JFIF (JPEG File Interchange Format) files, the common sort found on the web.

Please note that JPEG is a compression method specifically intended for continuous-tone images such as photographs, not for graphics, and it is therefore quite unsuitable for the vast majority of page images produced with PostScript. For anything other than pages containing simple images the lossy compression of the jpeg format will result in poor quality output regardless of the input. To learn more about the distinction, consult a reference about uses and abuses of JPEG, such as the `JPEG FAQ`_.


Options
""""""""""""

The JPEG devices support several special parameters to control the JPEG "quality setting" (DCT quantization level).

.. code-block:: bash

   -dJPEGQ=N (integer from 0 to 100, default 75)

Set the quality level N according to the widely used IJG quality scale, which balances the extent of compression against the fidelity of the image when reconstituted. Lower values drop more information from the image to achieve higher compression, and therefore have lower quality when reconstituted.

.. code-block:: bash

   -dQFactor=M (float from 0.0 to 1.0)


Adobe's QFactor quality scale, which you may use in place of ``JPEGQ`` above. The QFactor scale is used by PostScript's ``DCTEncode`` filter but is nearly unheard-of elsewhere.

At this writing the default JPEG quality level of 75 is equivalent to ``-dQFactor=0.5``, but the JPEG default might change in the future. There is currently no support for any additional JPEG compression options, such as the other DCTEncode filter parameters.




Examples
""""""""""

You can use the JPEG output drivers -- :title:`jpeg` to produce color JPEG files and :title:`jpeggray` for grayscale JPEGs -- the same as other file-format drivers: by specifying the device name and an output file name, for example

.. code-block:: bash

   gs -sDEVICE=jpeg -sOutputFile=foo.jpg foo.ps



.. _Devices_PNM:

PNM
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The PNM (portable network map) family of formats are very simple uncompressed image formats commonly used on unix-like systems. They are particularly useful for testing or as input to an external conversion utility.

A wide variety of data formats and depths is supported. Devices include :title:`pbm pbmraw pgm pgmraw pgnm pgnmraw pnm pnmraw ppm ppmraw pkm pkmraw pksm pksmraw`.



.. _Devices_TIFF:

TIFF file formats
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

TIFF is a loose collection of formats, now largely superceded by PNG except in applications where backward compatibility or special compression is required. The TIFF file format is described in the TIFF 6.0 Specification published by Adobe Systems Incorporated.


.. note ::

   Due to the structure of the TIFF format, writing TIFF output requires that the target file be seekable. Writing to stdout, pipes or other similar stream is not supported. Attempting to do so will generate an error.

There are two unrelated sets of TIFF drivers. There are five color TIFF drivers that produce uncompressed output:



:title:`tiffgray`
   Produces 8-bit gray output.

:title:`tiff12nc`
   Produces 12-bit RGB output (4 bits per component).

:title:`tiff24nc`
   Produces 24-bit RGB output (8 bits per component).

:title:`tiff48nc`
   Produces 48-bit RGB output (16 bits per component).

:title:`tiff32nc`
   Produces 32-bit CMYK output (8 bits per component).

:title:`tiff64nc`
   Produces 64-bit CMYK output (16 bits per component).

:title:`tiffsep`
   The :title:`tiffsep` device creates multiple output files: a single 32 bit composite CMYK file and multiple :title:`tiffgray` files, one for each separation (unless ``-dNoSeparationFiles`` is specified). If separation files are being produced and more than one page is being generated, the output file specification must include a format specifier (e.g ``-o outfile-%d.tif``) so that each page can have a uniquely named set of separation files.

   The default compression is ``lzw`` but this may be overridden by the ``-sCompression=`` option.

   The file specified via the ``OutputFile`` command line parameter will contain CMYK data. This data is based upon the CMYK data within the file plus an equivalent CMYK color for each spot color. The equivalent CMYK color for each spot color is determined using the alternate tint transform function specified in the ``Separation`` and :title:`devicen` color spaces. Since this file is created based upon having color planes for each colorant, the file will correctly represent the appearance of overprinting with spot colors.

   File names for the separations for the CMYK colorants are created by appending '.Cyan.tif', '.Magenta.tif' '.Yellow.tif' or '.Black.tif' to the end of the file name specified via the ``OutputFile`` parameter. File names for the spot color separation files are created by appending the Spot color name in '(' and ').tif' to the filename.

   Note that, while the name of the ink is case-sensitive, the filename may not be (depending on the Operating System), so if a spot name matches one of the process ink names, it will have the spot number included as part of the name (e.g. ``YELLOW0``).

   If desired the file names for the spot color separation files can be created by appending '.sn.tif' (where n is the spot color number, see below) to the end of the file name specified via the ``OutputFile`` parameter. This change is a compile time edit. To obtain this type of output the function ``create_separation_file_name`` in ``gdevtsep.c`` should be called with a true value for its ``use_sep_name`` parameter.

   The :title:`tiffsep` device will automatically recognize spot colors. In this case their order is determined by when they are found in the input file. The names of spot colors may be specified via the ``SeparationColorNames`` device parameters.

   Internally each spot color is assigned a spot color number. These numbers start with 0 for the first spot color. The spot color numbers are assigned in the same order as the names are printed to stderr (see below). This order also matches the ordering in the ``SeparationColorNames`` list, if this parameter is specified. The spot color numbers are not affected by the ``SeparationOrder`` parameter.

   If only a subset of the colorants for a file is desired, then the separations to be output can be selected via the ``SeparationOrder`` device parameter. When colorants are selected via the ``SeparationOrder`` parameter, the composite CMYK output contains the equivalent CMYK data only for the selected colorants.

   .. note::
      The composite CMYK output, because it uses the tint transformed colour equivalents for any spot colours (see Postscript Language Reference "Separation Color Spaces" and "DeviceN Color Spaces"), may not produce an accurate preview, if the job uses overprinting.

   The :title:`tiffsep` device can also print the names of any spot colors detected within a document to stderr, (stderr is also used for the output from the :title:`bbox` device). For each spot color, the name of the color is printed preceded by '%%SeparationName: '. This provides a simple mechanism for users and external applications to be informed about the names of spot colors within a document. This is controlled by the ``PrintSpotCMYK`` switch, which defaults to ``false`` (don't print).

   Generally Ghostscript will support a maximum of 64 process and spot colors. The :title:`tiffsep` device the :title:`psdcmyk` device and the :title:`psdcmyk16` devices maintain rendered data in a planar form with a maximum of 64 planes set by the definition of ``GS_CLIENT_COLOR_MAX_COMPONENTS`` in the code. That is there can be up to 64 colorants accurately handled with overprint on a single page. If more than 64 colorants are encountered, those beyond 64 will be mapped to CMYK using the alternate tint transform.

   When rendering a PDF document, Ghostscript can deteremine prior to rendering how many colorants occur on a particular page. With Postscript, this is not possible in general. To optimize for this, when rendering Postscript, it is possible to specify at run-time the number of spot colorants you wish to have the device capable of handling using the ``-dMaxSpots=N`` command option, where N is the number of spot colorants that you wish to be able to handle and must be no more than the 64 minus the number of process colors. For example, 60 or less for a CMYK device such as tiffsep. If you specify more than is needed, the document will render more slowly. The ideal case is to use the same number as the maximum number of spot colorants that occur on a single page of the document. If more spot colorants are encountered than is specified by ``-dMaxSpots``, then a warning will be printed indicating that some spot colorants will be mapped to CMYK using the alternate tint transform.

   The tiffsep device accepts a ``-dBitsPerComponent=`` option, which may be set to 8 (the default) or 1. In 1bpp mode, the device renders each component internally in 8 bits, but then converts down to 1bpp with error diffusion before output as described below in the tiffscaled device. No composite file is produced in 1bpp mode, only individual separations.

   The device also accepts the ``-dDownScaleFactor= -dTrapX= -dTrapy=`` and ``-sPostRenderProfile=`` parameters as described below in the :title:`tiffscaled` device, and ``-dMinFeatureSize=`` in 1bpp mode.

   When ``-dDownScaleFactor=`` is used in 8 bit mode with the tiffsep (and :title:`psdcmyk`/:title:`psdrgb`/:title:`psdcmyk16`/:title:`psdrgb16`) device(s) 2 additional "special" ratios are available, 32 and 34. 32 provides a 3:2 downscale (so from 300 to 200 dpi, say). 34 produces a 3:4 upscale (so from 300 to 400 dpi, say).

   In commercial builds, with 8 bit per component output, the ``-dDeskew`` option can be used to automatically detect/correct skew when generating output bitmaps.

   The :title:`tiffscaled` and :title:`tiffscaled4` devices can optionally use Even Toned Screening, rather than simple Floyd Steinberg error diffusion. This patented technique gives better quality at the expense of some speed. While the code used has many quality tuning options, none of these are currently exposed. Any device author interested in trying these options should contact Artifex for more information. Currently ETS can be enabled using ``-dDownScaleETS=1``.

:title:`tiffsep1`
   The :title:`tiffsep1` device creates multiple output files, one for each component or separation color. The device creates multiple :title:`tiffg4` files (the compression can be set using ``-sCompression=`` described below). The 1 bit per component output is halftoned using the current screening set by 'setcolorscreen' or 'sethalftone' which allows for ordered dither or stochastic threshold array dither to be used. This is faster than error diffusion.

   The file specified via the ``OutputFile`` command line parameter will not be created (it is opened, but deleted prior to finishing each page).

   File names for the separations for the CMYK colorants are created by appending '(Cyan).tif', '(Magenta).tif' '(Yellow).tif' or '(Black).tif' to the to the end of the file name specified via the ``OutputFile`` parameter. File names for the spot color separation files are created by appending the ``Spot`` color name in '(' and ').tif' to the filename. If the file name specified via the ``OutputFile`` parameter ends with the suffix '.tif', then the suffix is removed prior to adding the component name in '(' and ').tif'.

   The :title:`tiffsep1` device can also print the names of any spot colors detected within a document to stderr, (stderr is also used for the output from the :title:`bbox` device). For each spot color, the name of the color is printed preceded by '%%SeparationName: '. This provides a simple mechanism for users and external applications to be informed about the names of spot colors within a document. This is controlled by the ``PrintSpotCMYK`` switch, which defaults to ``false`` (don't print).

:title:`tiffscaled`
   The :title:`tiffscaled` device renders internally at the specified resolution to an 8 bit greyscale image. This is then scaled down by an integer scale factor (set by ``-dDownScaleFactor=`` described below) and then error diffused to give 1bpp output. The compression can be set using ``-sCompression=`` as described below.

:title:`tiffscaled4`
   The :title:`tiffscaled4` device renders internally at the specified resolution to an 8 bit cmyk image. This is then scaled down by an integer scale factor (set by ``-dDownScaleFactor=`` described below) and then error diffused to give 4bpp cmyk output. The compression can be set using ``-sCompression=`` as described below.

:title:`tiffscaled8`
   The :title:`tiffscaled8` device renders internally at the specified resolution to an 8 bit greyscale image. This is then scaled down by an integer scale factor (set by ``-dDownScaleFactor=`` described below). The compression can be set using ``-sCompression=`` as described below.

:title:`tiffscaled24`
   The :title:`tiffscaled24` device renders internally at the specified resolution to a 24 bit rgb image. This is then scaled down by an integer scale factor (set by ``-dDownScaleFactor=`` described below). The compression can be set using ``-sCompression=`` as described below.

   In commercial builds, the ``-dDeskew`` option can be used to automatically detect/correct skew when generating output bitmaps.

:title:`tiffscaled32`
   The :title:`tiffscaled32` device renders internally at the specified resolution to a 32 bit cmyk image. This is then scaled down by an integer scale factor (set by ``-dDownScaleFactor=`` described below). The compression can be set using ``-sCompression=`` as described below.

   In commercial builds, the ``-dDeskew`` option can be used to automatically detect/correct skew when generating output bitmaps.


The remaining TIFF drivers all produce black-and-white output with different compression modes:

:title:`tiffcrle`
   G3 fax encoding with no EOLs.

:title:`tiffg3`
   G3 fax encoding with EOLs.

:title:`tiffg32d`
   2-D G3 fax encoding.

:title:`tiffg4`
   G4 fax encoding.

:title:`tifflzw`
   LZW-compatible (tag = 5) compression.

:title:`tiffpack`
   PackBits (tag = 32773) compression.

See the ``AdjustWidth`` option documentation below for important information about these devices.


Options
""""""""""""""""""""""""


All TIFF drivers support creation of files that are comprised of more than a single strip. Multi-strip files reduce the memory requirement on the reader, since readers need only store and process one strip at a time. The ``MaxStripSize`` parameter controls the strip size:


.. code-block:: bash

   -dMaxStripSize=N

Where **N** is a non-negative integer; default = 8192, 0 on Fax devices. Set the maximum (uncompressed) size of a strip.


The TIFF 6.0 specification, Section 7, page 27, recommends that the size of each strip be about 8 Kbytes.

If the value of the ``MaxStripSize`` parameter is smaller than a single image row, then no error will be generated, and the TIFF file will be generated correctly using one row per strip. Note that smaller strip sizes increase the size of the file by increasing the size of the ``StripOffsets`` and ``StripByteCounts`` tables, and by reducing the effectiveness of the compression which must start over for each strip.

If the value of ``MaxStripSize`` is 0, then the entire image will be a single strip.

Since v. 8.51 the logical order of bits within a byte, ``FillOrder``, tag = 266 is controlled by a parameter:


.. code-block:: bash

   -dFillOrder=1 | 2 (default = 1)

If this option set to 2 then pixels are arranged within a byte such that pixels with lower column values are stored in the lower-order bits of the byte; otherwise pixels are arranged in reverse order.


Earlier versions of Ghostscript always generated TIFF files with ``FillOrder = 2``. According to the TIFF 6.0 specification, Section 8, page 32, support of ``FillOrder = 2`` is not required in a Baseline TIFF compliant reader

The writing of ``BigTIFF`` format output files is controlled with the ``-dUseBigTIFF`` parameter.

Unfortunately, due the unpredictable size of compressed output, we cannot automate the selection of ``BigTIFF``, using it only when the output file grows large enough to warrant it.

.. code-block:: bash

   -dUseBigTIFF(=false/true) (boolean, default: false)


Forces use (or not) of ``BigTIFF`` format in output from TIFF devices.

The writing of the DateTime TAG can be controlled using the ``-dTIFFDateTime`` parameter.

.. code-block:: bash

   -dTIFFDateTime(=true/false) (boolean, default: true)

Write or otherwise the ``DateTime TAG`` to the TIFF output file. Thus to disable writing the ``TAG``, use: ``-dTIFFDateTime=false``.

The compression scheme that is used for the image data can be set for all tiff devices with:

.. code-block:: bash

   -sCompression=none | crle | g3 | g4 | lzw | pack

Change the compression scheme of the tiff device. ``crle``, ``g3``, and ``g4`` may only be used with 1 bit devices (including :title:`tiffsep1`).



For the :title:`tiffsep` device, it changes the compression scheme of the separation files and composite cmyk file (which is lzw by default). It defaults to ``g4`` for the :title:`tiffsep1` device.

The black-and-white TIFF devices also provide the following parameters:


.. code-block:: bash

   -dAdjustWidth=state (0, 1, or value; default = 1)

If this option is 1 then if the requested page width is in the range of either 1680..1736 or 2000..2056 columns, set the page width to A4 (1728 columns) or B4 (2048 columns) respectively. If this option is set to a value >1 then the width is unconditionally adjusted to this value.

This behavior is the default for all the fax based devices (i.e. all the black and white devices except :title:`tifflzw`, :title:`tiffpack` and :title:`tiffscaled`). Pass ``-dAdjustWidth=0`` to force this behaviour off.

When using this option with :title:`tiffscaled` it is the downsampled size that triggers the adjustment.

.. code-block:: bash

   -dMinFeatureSize=state (0 to 4; default = 1)

This option allows a minimum feature size to be set; if any output pixel appears on its own, or as part of a group of pixels smaller than ``MinFeatureSize`` x ``MinFeatureSize``, it will be expanded to ensure that it does. This is useful for output devices that are high resolution, but that have trouble rendering isolated pixels.

While this parameter will accept values from 0 to 4, not all are fully implemented. 0 and 1 cause no change to the output (as expected). 2 works as specified. 3 and 4 currently expand pixels correctly horizontally, but only expand vertically to the 2 pixel size.

The mechanism by which ``MinFeatureSize`` is implemented for :title:`tiffscaled` is different, in that it is done as part of the error diffusion. Values of 0 to 2 work as expected, but values 3 and 4 (while accepted for compatibility) will behave as for 2.


The :title:`tiffscaled`, :title:`tiffscaled4`, :title:`tiffscaled8`, :title:`tiffscaled24` and :title:`tiffscaled32` TIFF drivers also provide the following two parameters:


.. code-block:: bash

   -dDownScaleFactor=factor (integer <= 8; default = 1)

If this option set then the page is downscaled by the given factor on both axes before error diffusion takes place. For example rendering with -r600 and then specifying ``-dDownScaleFactor=3`` will produce a 200dpi image.

.. code-block:: bash

   -sPostRenderProfile=path (path to an ICC profile)

If this option set then the page will be color transformed using that profile after downscaling.

This is useful when the file uses overprint to separately paint to some subset of the C, M, Y, and K colorants, but the final CMYK is to be color corrected for printing or display.


The :title:`tiffsep` TIFF device also provide this parameter:

.. code-block:: bash

   -dPrintSpotCMYK=boolean defaults to false.

When set to true the device will print (to stdout) the name of each ink used on the page, and the CMYK values which are equivalent to 100% of that ink. The values are 16-bits ranging from 0 to 32760.


The :title:`tiffsep` device (along with the :title:`tiffscaled32` and :title:`psdcmyk` devices) can perform rudimentary automatic bitmap 'trapping' on the final rendered bitmap. This code is disabled by default; see the :ref:`note<TrappingPatentsNote>` below as to why.

Trapping is a process whereby the output is adjusted to minimise the visual impact of offsets between each printed plane. Typically this involves slightly extending abutting regions that are rendered in different inks. The intent of this is to avoid the unsightly gaps that might be otherwise be revealed in the final printout if the different color plates do not exactly line up.

This trapping is controlled by 3 device parameters. Firstly the maximum X and Y offsets are specified using ``-dTrapX=N`` and ``-dTrapY=N`` (where N is a figure in pixels, before the downscaler is applied).

The final control is to inform the trapping process in what order inks should be processed, from darkest to lightest. For a typical CMYK device this order would be [ 3 1 0 2 ] (K darker than M darker than C darker than Y). This is the default. In the case where CMYK + spots are used, the code defaults to assuming that the spots are lighter than the standard colours and are sent darkest first (thus [ 3 1 0 2 4 5 6 ... ]).

To override these defaults, the ``TrapOrder`` parameter can be used, for example:

.. code-block:: bash

   gs -sDEVICE=psdcmyk -dTrapX=2 -dTrapY=2 -o out.psd -c "<< /TrapOrder [ 4 5 3 1 0 2 ] >> setpagedevice" -f examples\tiger.eps


.. _TrappingPatentsNote:

.. note::

   **Trapping patents**. Trapping is an technology area encumbered by many patents. We believe that the last of these has now lapsed, and so have enabled the code by default.


.. _Devices_FAX:

FAX
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Ghostscript supports a variety of fax encodings, both encapsulated in TIFF (see above) and as raw files. The later case is described here.

The fax devices are :title:`faxg3`, :title:`faxg32d` and :title:`faxg4`.

The fax devices support the ``MinFeatureSize`` parameter as defined in the TIFF device section.

It appears from this commit: ``0abc209b8460396cdece8fc824c053a2662c4cbf`` that some (many ?) fax readers cannot cope with multiple strip TIFF files. The commit noted above works around this by assuming no fax output will exceed 1MB. Unfrotunately it also altered all the TIFF devices' default strip size which we now think was inadvisable. The fax devices now use a ``MaxStripSize`` of 0 so that the file only contains a single strip. This can still be overridden by specifying ``MaxStripSize`` on the command line.


.. _Devices_BMP:

BMP
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

BMP is a simple uncompressed image format commonly used on MS Windows.

It is supported by the :title:`bmpmono bmpgray bmpsep1 bmpsep8 bmp16 bmp256 bmp16m bmp32b` series of devices.


.. _Devices_PCX:

PCX
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

PCX is an image format sometimes used on MS Windows. It has some support for image compression and alternate color spaces, and so can be a useful way to output CMYK.

It is supported by the :title:`pcxmono pcxgray pcx16 pcx256 pcx24b pcxcmyk` series of devices.



.. _Devices_PSD:

PSD
~~~~~~~~~~~~~~~~~~~~

PSD is the image format used by Adobe Photoshop.

It is supported by the :title:`psdcmyk psdrgb psdcmyk16 psdrgb16` series of devices.

Of special interest with the :title:`psdcmyk` and :title:`psdcmyk16` devices is that they support spot colors. See the comments under the :title:`tiffsep` and :title:`tiffsep1` device about the maximum number of spot colors supported by Ghostscript.

The :title:`psdcmyk16` and :title:`psdrgb16` devices are essentially the same as the :title:`psdcmyk` and :title:`psdrgb` devices except they provide 16 bit output.

The :title:`psdcmykog` device produces PSD files with 6 components: Cyan, Magenta, Yellow, blacK, Orange, and Green. This device does not support the ``-dDownScaleFactor=`` option (see below), instead it always scales down by a factor of two.

These devices support the same ``-dDownScaleFactor=`` ratios as :title:`tiffsep`. The :title:`psdcmyk` device supports the same trapping options as :title:`tiffsep` (but see :ref:`this note<TrappingPatentsNote>`).


.. note::

   The PSD format is a single image per file format, so you must use the "%d" format for the ``OutputFile`` (or "-o") file name parameter (see :ref:`One page per file<Use_OnePagePerFile>` for details). An attempt to output multiple pages to a single PSD file (i.e. without the "%d" format) will result in an ``ioerror`` Postscript error.

In commercial builds, for the :title:`psdcmyk` and :title:`psdrgb` devices, the ``-dDeskew`` option can be used to automatically detect/correct skew when generating output bitmaps.


.. _Devices_PDF:

PDF
~~~~~~~~~~~~~~~~~~~~

These devices render input to a bitmap (or in the case of PCLm multiple bitmaps) then wraps the bitmap(s) up as the content of a PDF file. For PCLm there are some additional rules regarding headers, extra content and the order in which the content is written in the PDF file.

The aim is to support the PCLm mobile printing standard, and to permit production of PDF files from input where the graphics model differs significantly from PDF (eg PCL and RasterOPs).

There are five devices named :title:`pdfimage8`, :title:`pdfimage24`, :title:`pdfimage32`, :title:`pclm` and :title:`pclm8`. These produce valid PDF files with a colour depth of 8 (Gray), 24 (RGB) or 32 (CMYK), the :title:`pclm` device only supports 24-bit RGB and the :title:`pclm8` device only supports 8-bit gray. These are all implemented as 'downscale' devices, which means they can implement page level anti-aliasing using the ``-dDownScaleFactor`` switch.


.. code-block:: bash

   -dDownScaleFactor=integer

This causes the internal rendering to be scaled down by the given (integer <= 8) factor before being output. For example, the following will produce a PDF containing a 200dpi output from a 600dpi internal rendering:

.. code-block:: bash

   gs -sDEVICE=pdfimage8 -r600 -dDownScaleFactor=3 -o tiger.pdf\
      examples/tiger.eps



In commercial builds, the ``-dDeskew`` option can be used to automatically detect/correct skew when generating the output file.

The type of compression used for the image data can also be selected using the ``-sCompression`` switch. Valid compression types are ``None``, ``LZW``, ``Flate``, :title:`jpeg` and ``RLE``.

.. note::

   ``LZW`` is not supported on :title:`pclm` (not valid) and ``None`` is only supported on :title:`pclm` for debugging purposes.

For JPEG compression the devices support both the JPEGQ and QFactor switches as documented for the JPEG file format device.

In addition, the PCLm device supports some other parameters. Firstly, the ``-dStripHeight`` switch to set the vertical height of the strips of image content, as required by the specification.

Secondly, the standard postscript ``-dDuplex`` and ``-dTumbleswitches`` are supported, in that if both are set to true, every verso page (i.e. all even pages) will be rotated by 180 degrees.

As an extension to this, a ``-dTumble2`` parameter is also supported that will add an additional X-axis flip for every verso page. Thus ``-dDuplex=true -dTumble=false -dTumble2=true`` will result in verso pages being flipped horizontally, and ``-dDuplex=true -dTumble=true -dTumble2=true`` will result in verso pages being flipped vertically.



.. note::

   In addition to raster image files, Ghostscript supports output in a number of 'high-level' formats. These allow Ghostscript to preserve (as much as possible) the drawing elements of the input file maintaining flexibility, resolution independence, and editability.



.. _Devices_OCR:

Optical Character Recognition (OCR) devices
--------------------------------------------------------------------------------------------

OCR text output
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

These devices render internally in 8 bit greyscale, and then feed the resultant image into an OCR engine. Currently, we are using the Tesseract engine. Not only is this both free and open source, it gives very good results, and supports a huge number of languages/scripts.

The Tesseract engine relies on files to encapsulate each language and/or script. These "traineddata" files are available in different forms, including fast and best variants. Alternatively, people can train their own data using the standard Tesseract tools.

These files are looked for from a variety of places.

#. Files will be searched for in the directory given by the environment variable ``TESSDATA_PREFIX``.

#. Then they will be searched for within the ROM filing system. Any files placed in "tessdata" will be included within the ROM filing system in the binary for any standard (``COMPILE_INITS=1``) build.

#. Then files will be searched for in the configured 'tessdata' path. On Unix, this can be specified at the configure stage using '--with-tessdata=<path>' (where <path> is a list of directories to search, separated by ':' (on Unix) or ';' (on Windows)).

#. Finally, we resort to searching the current directory.

Please note, this pattern of directory searching differs from the original release of the OCR devices.

By default, the OCR process defaults to looking for English text, using "eng.traineddata". This can be changed by using the ``-sOCRLanguage=`` switch:

.. code-block:: bash

   -sOCRLanguage=language

This sets the trained data sets to use within the Tesseract OCR engine.

For example, the following will use English and Arabic:

.. code-block:: bash

   gs -sDEVICE=ocr -r200 -sOCRLanguage="eng+ara" -o out.txt\
      zlib/zlib.3.pdf


The first device is named :title:`ocr`. It extracts data as unicode codepoints and outputs them to the device as a stream of UTF-8 bytes.

The second device is named :title:`hocr`. This extracts the data in `hOCR`_ format.

These devices are implemented as downscaling devices, so the standard parameters can be used to control this process. It may seem strange to use downscaling on an image that is not actually going to be output, but there are actually good reasons for this. Firstly, the higher the resolution, the slower the OCR process. Secondly, the way the Tesseract OCR engine works means that anti-aliased images perform broadly as well as the super-sampled image from which it came.


PDF image output (with OCR text)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

These devices do the same render to bitmap and wrap as a PDF process as the ``PDFimage`` devices above, but with the addition of an OCR step at the end. The OCR'd text is overlaid "invisibly" over the images, so searching and cut/paste should still work.

The OCR engine being used is Tesseract. For information on this including how to control what language data is used, see the OCR devices section above.

There are three devices named :title:`pdfocr8`, :title:`pdfocr24` and :title:`pdfocr32`. These produce valid PDF files with a colour depth of 8 (Gray), 24 (RGB) or 32 (CMYK).

These devices accept all the same flags as the PDFimage devices described above.



Vector PDF output (with OCR Unicode CMaps)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The :title:`pdfwrite` device has been augmented to use the OCR engine to analyse text (not images!) in the input stream, and derive Unicode code points for it. That information can then be used to create ``ToUnicode CMaps`` which are attached to the ``Font`` (or ``CIDFont``) objects embedded in the PDF file.

Fonts which have ``ToUnicode CMaps`` can be reliably (limited by the accuracy of the ``CMap``) used in search and copy/paste functions, as well as text extraction from PDF files. Note that OCR is not a 100% perfect process; it is possible that some text might be misidentified.

OCR is a slow operation! In addition it can (for Latin text at least) sometimes be preferable not to add ``ToUnicode`` information which may be incorrect, but instead to use the existing font ``Encoding``. For English text this may give better results.

For these reasons the OCR functionality of :title:`pdfwrite` can be controlled by using a new parameter ``-sUseOCR``. This has three possible values:

.. code-block:: bash

   -sUseOCR=string

**string** values as follows:

**Never**
   Default - don't use OCR at all even if support is built-in.

**AsNeeded**
   If there is no existing ToUnicode information, use OCR.

**Always**
   Ignore any existing information and always use OCR.


.. _Devices_HighLevel:


High level devices
---------------------------------------------------------------------------------


Please refer to :ref:`High Level Devices<VectorDevices.html>` for documentation on the device options for these devices.



PDF writer
~~~~~~~~~~~~~~~~~~~~

The :title:`pdfwrite` device outputs PDF.

PS2 writer
~~~~~~~~~~~~~~~~~~~~

The :title:`ps2write` device outputs postscript language level 2. It is recommnded that this device is used for PostScript output. There is no longer any support for creating PostScript level 1 output.


EPS writer
~~~~~~~~~~~~~~~~~~~~

The :title:`eps2write` device outputs encapsulated postscript.

PXL
~~~~~~~~~~~~~~~~~~~~

The :title:`pxlmono` and :title:`pxlcolor` devices output HP PCL-XL, a graphic language understood by many recent laser printers.

Text output
~~~~~~~~~~~~~~~~~~~~

The :title:`txtwrite` device will output the text contained in the original document as Unicode.


.. _Devices_Display_Device:
.. _Devices_Display_Devices:

Display devices
---------------------------

Ghostscript is often used for screen display of postscript and pdf documents. In many cases, a client or 'viewer' application calls the Ghostscript engine to do the rasterization and handles the display of the resulting image itself, but it is also possible to invoke Ghostscript directly and select an output device which directly handles displaying the image on screen.

This section describes the various display-oriented devices that are available in Ghostscript.


X Window System
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Perhaps the most common use of of a display device is with the X Window System on unix-like systems. It is the default device on the command line client on such systems, and is used more creatively by the gv client application.

The available devices are:

:title:`x11`
   This is the default device, handling display on X11R6.

:title:`x11alpha`
   This is the :title:`x11` device, but with antialiasing. It is equivalent to invoking the x11 device with the options ``-dGraphicsAlphaBits=4 -dTextAlphaBits=4 -dMaxBitmap=50000000``.

:title:`x11cmyk`
   This device rasterizes the image in the CMYK color space, then flattens it to RGB for display. It's intended for testing only.

:title:`x11mono`
   This is a strict black-and-white device for 1-bit monochrome displays.

:title:`x11gray2`
   This is a device for 2 bpp (4-level) monochrome displays.

:title:`x11gray4`
   This is a device for 4 bpp (16-level) monochrome displays.

On Mac OS X as of 10.6, the X server (XQuartz) only supports color depth 15 and 24. Depth 15 isn't well-tested, and it may be desirable, for serious use, to switch to depth 24 with:

.. code-block:: bash

   defaults write org.x.X11 depth 24



Display device (MS Windows, OS/2, gtk+)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The display device is used by the MS Windows, OS/2 and the gtk+ versions of Ghostscript.


Options
"""""""""""""""

The display device has several user settable options.


``-dDisplayFormat=N (integer bit-field)``

   Some common values are ``16#30804`` for Windows RGB, ``16#804`` for gtk+ RGB, ``16#20101`` for Windows monochrome, ``16#102`` for gtk+ monochrome, ``16#20802`` grayscale, ``16#20808`` for CMYK, ``16#a0800`` for separations.

   The bit fields are:

      - native (1), gray (2), RGB (4), CMYK (8), or separation (80000) color spaces.
      - unused first byte (40) or last byte (80).
      - 1 (100), 4 (400), or 8 (800) bits/component.
      - bigendian (00000 = RGB) or littleendian (10000 = BGR) order.
      - top first (20000) or bottom first (00000) raster.
      - 16 bits/pixel with 555 (00000) or 565 (40000) bitfields.
      - For more details, see the Ghostscript Interpreter API.

``-dDisplayResolution=DPI``

   Set the initial resolution resolution for the display device. This is used by the Windows clients to set the display device resolution to the Windows display logical resolution. This can be overriden by the command line option ``-rDPI``.



When using the separation color space, the following options may be set using ``setpagedevice``, as described in the PostScript Language Reference:

``SeparationColorNames``
   An array giving the names of the spot colors.

``SeparationOrder``
   An array giving the names and order of the colorants to be output.



IJS - Inkjet and other raster devices
---------------------------------------------------------------------------------

IJS is a relatively new initiative to improve the quality and ease of use of inkjet printing with Ghostscript. Using IJS, you can add new drivers, or upgrade existing ones, without recompiling Ghostscript. All driver authors are encouraged to adapt their drivers for IJS, and if there is an IJS driver available for your printer, it should be your first choice.

Please see the IJS web page for more information about IJS, including a listing of IJS-compatible drivers.

A typical command line for IJS is:

.. code-block:: bash

   gs -dSAFER -sDEVICE=ijs -sIjsServer=hpijs -sDeviceManufacturer=HEWLETT-PACKARD -sDeviceModel='DESKJET 990' -dIjsUseOutputFD -sOutputFile=/dev/usb/lp1 -dNOPAUSE -- examples/tiger.eps


Individual IJS command line parameters are as follows:


``-sIjsServer={path}``
   Sets the pathname for the IJS server (ie printer driver). Ghostscript will spawn a new process for this driver, and communicate with it using the IJS protocol. The pathname need not be absolute, as the PATH environment variable is searched, but it's probably a good idea for robustness and security. Note also that if ``-dSAFER`` is not specified, it's possible for PostScript code to set this parameter, so it can cause arbitrary code to be executed. See the section on Security for more information.

``-sDeviceManufacturer={name} -sDeviceModel={name}``
   These parameters select the device according to IEEE-1284 standard device ID strings. In general, consult the documentation for the driver to find the appropriate settings. Note that, if the value contains a space, you'll want to quote the value in your shell, as in the example above.

``-sIjsParams={params}``
   This parameter allows you to set arbitrary IJS parameters on the IJS driver. The format is a comma-separated list of key=value pairs. If it is necessary to send a value containing a comma or backslash, it can be escaped with a backslash. Thus, ``-sIjsParams=Foo=bar,Baz=a\,b`` sets the parameter Foo to "bar", and Baz to "a,b".

``-dIjsUseOutputFD``
   This flag indicates that Ghostscript should open the output file and pass a file descriptor to the server. If not set, Ghostscript simply passes the filename set in OutputFile to the server. In most cases, this flag won't matter, but if you have a driver which works only with OutputFD (such as hpijs 1.0.2), or if you're using the ``-sOutputFile="|cmd"`` syntax, you'll need to set it.

``-dBitsPerSample=N``
   This parameter controls the number of bits per sample. The default value of 8 should be appropriate for most work. For monochrome images, use ``-dBitsPerSample=1``.


Generic Ghostscript options that are particularly relevant for IJS are summarized below:

``-rnumber -rnumber1xnumber2``
   Sets the resolution, in dpi. If the resolution is not specified, Ghostscript queries the IJS server to determine the preferred resolution. When the resolution is specified, it overrides the value (if any) preferred by the IJS server.

``-dDuplex -dTumble``
   These flags enable duplex (two-sided) printing. ``Tumble`` controls the orientation. When ``Tumble`` is false, the pages are oriented suitably at the left or right. When ``Tumble`` is true, the pages are oriented suitably for binding at the top or bottom.

``-sProcessColorModel={name}``
   Use this flag to select the process color model. Suitable values include ``DeviceGray``, ``DeviceRGB``, and ``DeviceCMYK``.


Building IJS
~~~~~~~~~~~~~~~~~~~~~

IJS is included by default on Unix gcc builds, and also in autoconf'ed builds. Others may need some ``makefile`` tweaking. Firstly, make sure the IJS device is selected:

``DEVICE_DEVS2=$(DD)ijs.dev``
   Next, make sure that the path and execution type are set in the top level makefile. The values for Unix are as follows:

``IJSSRCDIR=ijs IJSEXECTYPE=unix``
   At present, "unix" and "win" are the only supported values for ``IJSEXECTYPE``. If neither sounds appropriate for your system, it's possible that more porting work is needed.

Lastly, make sure that ``ijs.mak`` is included in the top level ``makefile``. It should be present right after the include of ``icclib.mak``.

IJS is not inherently platform-specific. We're very much interested in taking patches from people who have ported it to non-mainstream platforms. And once it's built, you won't have to recompile Ghostscript to support new drivers!



Rinkj - Resplendent inkjet driver
---------------------------------------------------

The Rinkj driver is an experimental new driver, capable of driving some Epson printers at a very high level of quality. It is not currently recommended for the faint of heart.

You will need to add the following line to your ``makefile``:

``DEVICE_DEVS2=$(DD)rinkj.dev``
   Most of the configuration parameters, including resolution, choice of printer model, and linearization curves, are in a separate setup file. In addition, we rely heavily on an ICC profile for mapping document colors to actual device colors.

A typical command line invocation is:

.. code-block:: bash

   gs -r1440x720 -sDEVICE=rinkj -sOutputFile=/dev/usb/lp0 -sSetupFile=lib/rinkj-2200-setup -sProfileOut=2200-cmyk.icm -dNOPAUSE -dBATCH file.ps

Individual Rinkj command line parameters are as follows:

``-sSetupFile={path}``
   Specifies the path for the setup file.

``-sProfileOut={path}``
   Specifies the path for the output ICC profile. This profile should be a link profile, mapping the ``ProcessColorModel`` (``DeviceCMYK`` by default) to the device color space.

For 6- and 7-color devices, the target color space for the output profile is currently a 4-component space. The conversion from this into the 6- or 7-color space (the "ink split") is done by lookup tables in the setup file.


Setup files are in a simple "Key: value" text format. Relevant keys are:

``Manufacturer:{name} Model:{name}``
   The manufacturer and model of the individual device, using the same syntax as IEEE printer identification strings. Currently, the only supported manufacturer string is "EPSON", and the only supported model strings are "Stylus Photo 2200" and "Stylus Photo 7600".

``Resolution:{x-dpi}x{y-dpi}``
   The resolution in dpi. Usually, this should match the Ghostscript resolution set with the ``-r`` switch. Otherwise, the page image will be scaled.

``Dither:{int}``
   Selects among variant dither options. Currently, the choices are 1 for one-bit dither, and 2, for a 2-bit variable dot dither.

``Aspect:{int}``
   Controls the aspect ratio for highlight dot placement. Valid values are 1, 2, and 4. For best results, choose a value near the x resolution divided by the y resolution. For example, if resolution is 1440x720, aspect should be 2.

``Microdot:{int}``
   Chooses a microdot size. On EPSON devices, this value is passed directly through to the "ESC ( e" command. See EPSON documentation for further details (see, I told you this wasn't for the faint of heart).

``Unidirectional:{int}``
   Enables (1) or disables (0) unidirectional printing, which is slower but possibly higher quality.

``AddLut:{plane}``
   Adds a linearization look-up table. The plane is one of "CcMmYKk". The lookup table data follows. The line immediately following AddLut is the number of data points. Then, for each data point is a line consisting of two space-separated floats - the output value and the input value. If more than one LUT is specified for a single plane, they are applied in sequence.

A typical setup file is supplied in ``lib/rinkj-2200-setup``. It is configured for the 2200, but can be adapted to the 7600 just by changing the "Model" line.

A known issue with this driver is poor support for margins and page size. In some cases, this will cause an additional page to be ejected at the end of a job. You may be able to work around this by supplying a cut-down value for ``-dDEVICEHEIGHTPOINTS``, for example 755 for an 8.5x11 inch page on the EPSON 2200.



HP Deskjet official drivers
-----------------------------------

HP provides official drivers for many of their Deskjet printer models. In order to use these drivers, you will need the HP Inkjet Server as well as Ghostscript, available from `HP Linux Imaging and Printing`_. This version of Ghostscript includes the patch from version 0.97 of the ``hpijs`` software. If you are installing ``hpijs`` from an RPM, you will only need the hpijs RPM, not the Ghostscript-hpijs one, as the code needed to work with ``hpijs`` is already included.

Note that newer version of the ``hpijs`` drivers support the IJS protocol. If you can, you should consider using the :title:`ijs` driver instead. Among other things, the ``hpijs`` Ghostscript driver is Unix-only, and is untested on older Unix platforms.

As of the 0.97 version, ``hpijs`` supports the following printer models:

**e-Series**: e-20

**DeskJet 350C Series**: 350C

**DeskJet 600C Series**: 600C, 660C, 670/672C, 670TV, 680/682C

**DeskJet 600C Series Photo**: 610/612C, 640/648C, 690/692/693/694/695/697C

**DeskJet 630C Series**: 630/632C

**DeskJet 800C Series**: 810/812C, 830/832C, 840/842/843C, 880/882C, 895C

**DeskJet 900C Series, PhotoSmart**: 930/932C, 950/952C, 970C, PhotoSmart 1000/1100

**DeskJet 990C, PhotoSmart**: 960C, 980C, 990C, PhotoSmart 1215/1218


You will need to add the following line to your ``makefile``:

.. code-block:: bash

   DEVICE_DEVS2=$(DD)DJ630.dev $(DD)DJ6xx.dev $(DD)DJ6xxP.dev $(DD)DJ8xx.dev $(DD)DJ9xx.dev $(DD)DJ9xxVIP.dev $(DD)AP21xx.dev


Please see `HP Linux Imaging and Printing`_ for more information about this driver. Thanks to the folks at HP, especially David Suffield for making this driver available and working to integrate it with Ghostscript.


Gimp-Print driver collection
-----------------------------------

The Gimp-Print project provides a large collection of printer drivers with an IJS interface. Please see `Gimp print`_ for details.




MS Windows printers
-----------------------------------

This section was written by Russell Lang, the author of Ghostscript's MS Windows-specific printer driver, and updated by Pierre Arnaud.

The :title:`mswinpr2` device uses MS Windows printer drivers, and thus should work with any printer with device-independent bitmap (DIB) raster capabilities. The printer resolution cannot be selected directly using PostScript commands from Ghostscript: use the printer setup in the Control Panel instead. It is however possible to specify a maximum resolution for the printed document (see below).

If no Windows printer name is specified in ``-sOutputFile``, Ghostscript prompts for a Windows printer using the standard Print Setup dialog box. You must set the orientation to Portrait and the page size to that expected by Ghostscript; otherwise the image will be clipped. Ghostscript sets the physical device size to that of the Windows printer driver, but it does not update the PostScript clipping path.

If a Windows printer name is specified in ``-sOutputFile`` using the format "``%printer%printer_name``", for instance:

.. code-block:: bash

   gs ... -sOutputFile="%printer%Apple LaserWriter II NT"


Then Ghostscript attempts to open the Windows printer without prompting (except, of course, if the printer is connected to FILE:). Ghostscript attempts to set the Windows printer page size and orientation to match that expected by Ghostscript, but doesn't always succeed. It uses this algorithm:

#. If the requested page size matches one of the Windows standard page sizes +/- 2mm, request that standard size.
#. Otherwise if the requested page size matches one of the Windows standard page sizes in landscape mode, ask for that standard size in landscape.
#. Otherwise ask for the page size by specifying only its dimensions.
#. Merge the requests above with the defaults. If the printer driver ignores the requested paper size, no error is generated: it will print on the wrong paper size.
#. Open the Windows printer with the merged orientation and size.


The Ghostscript physical device size is updated to match the Windows printer physical device.



Supported command-line parameters
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The :title:`mswinpr2` device supports a limited number of command-line parameters (e.g. it does not support setting the printer resolution). The recognized parameters are the following:


``-sDEVICE=mswinpr2``
   Selects the MS Windows printer device. If Ghostscript was not compiled with this device as the default output device, you have to specify it on the command line.

``-dNoCancel``
   Hides the progress dialog, which shows the percent of the document page already processed and also provides a cancel button. This option is useful if GS is intended to print pages in the background, without any user intervention.

``-sOutputFile="%printer%printer_name"``
   Specifies which printer should be used. The ``printer_name`` should be typed exactly as it appears in the Printers control panel, including spaces.

Supported options (device properties)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Several extra options exist which cannot be set through the command-line, but only by executing the appropriate PostScript setup code. These options can be set through the inclusion of a setup file on the command-line:

.. code-block:: bash

   gs ... setup.ps ...

The ``setup.ps`` file is responsible for the device selection, therefore you should not specify the ``-sDEVICE=mswinpr2`` option on the command-line if you are using such a setup file. Here is an example of such a setup file:

.. code-block:: postscript

   <<
     /NoCancel      true                       % don't show the cancel dialog
     /BitsPerPixel  4                          % force 4 bits/pixel
     /UserSettings
       <<
         /DocumentName  (Ghostscript document) % name for the Windows spooler
         /MaxResolution 360                    % maximum document resolution
       >>
     /OutputDevice /mswinpr2                   % select the Windows device driver
   >> setpagedevice
  setdevice


This example disables the progress dialog (same as the ``-dNoCancel`` option), forces a 4 bits/pixel output resolution and specifies additional user settings, such as the document name (which will be displayed by the Windows spooler for the queued document) and the maximum resolution (here 360 dpi). It then finds and selects an instance of the MS Windows device printer and activates it. This will show the standard printer dialog, since no ``/OutputFile`` property was specified.

The following options are available:


``/NoCancel boolean``
   Disables (hides) the progress dialog when set to true or show the progress dialog if not set or set to false.

``/OutputFile string``
   Specifies which printer should be used. The string should be of the form ``%printer%printer_name``, where the ``printer_name`` should be typed exactly as it appears in the Printers control panel, including spaces.

``/QueryUser integer``
   Shows the standard printer dialog (1 or any other value), shows the printer setup dialog (2) or selects the default Windows printer without any user interaction (3).

``/BitsPerPixel integer``
   Sets the device depth to the specified bits per pixel. Currently supported values are 1 (monochrome), 4 (CMYK with screening handled by Ghostscript) and 24 (True Color, dithering handled by the Windows printer driver; this option can produce huge print jobs).

``/UserSettings dict``
   Sets additional options, defined in a dictionary. The following properties can be set:

   ``/DocumentName string``
      Defines the user friendly document name which will be displayed by the Windows spooler.

   ``/DocumentRange [n1 n2]``
      Defines the range of pages contained in the document. This information can be used by the printer dialog, in conjunction with the following property.

   ``/SelectedRange [n1 n2]``
      Defines the selected range of pages. This information will be displayed in the printer dialog and will be updated after the user interaction. A PostScript program could check these values and print only the selected page range.

   ``/MaxResolution dpi``
      Specifies the maximum tolerated output resolution. If the selected printer has a higher resolution than dpi, then Ghostscript will render the document with a submultiple of the printer resolution. For example, if ``MaxResolution`` is set to 360 and the output printer supports up to 1200 dpi, then Ghostscript renders the document with an internal resolution of 1200/4=300 dpi. This can be very useful to reduce the memory requirements when printing in True Color on some high resolution ink-jet color printers.


These properties can be queried through the `currentpagedevice` operator. The following PostScript code snippet shows how to do it for some of the properties:

.. code-block:: postscript

   currentpagedevice /BitsPerPixel get ==  % displays the selected depth

   currentpagedevice /UserSettings get     % get the additional options..
   /us exch def                            % ..and assign them to a variable

   us /DocumentName get ==     % displays the document name
   us /SelectedRange get ==    % displays the selected page range

   % other misc. information (don't rely on them)

   us /Color get ==            % 1 => monochrome output, 2 => color output
   us /PrintCopies get ==      % displays the number of copies requested


There are a few undocumented parameters stored in the ``UserSettings`` dictionary. You should not rely on them. Their use is still experimental and they could be removed in a future version.

Duplex printing
~~~~~~~~~~~~~~~~~~~~~

If the Windows printer supports the duplex printing feature, then it will also be available through the :title:`mswinpr2` device. You can query for this support through the ``/Duplex`` property of the ``currentpagedevice``. If it returns ``null``, then the feature is not supported by the selected printer. Otherwise, true means that the printer is currently set up to print on both faces of the paper and false that it is not, but that it can.

The following example shows how to print on both faces of the paper (using the long side of the paper as the reference):

.. code-block:: postscript

   << /Duplex true /Tumble false >> setpagedevice



Sun SPARCprinter
----------------------------

This section was contributed by Martin Schulte.

With a SPARCprinter you always buy software that enables you to do PostScript printing on it. A page image is composed on the host, which sends a bitmap to the SPARCprinter through a special SBUS video interface. So the need for a Ghostscript interface to the SPARCprinter seems low, but on the other hand, Sun's software prints some PostScript drawings incorrectly: some pages contain a thin vertical line of rubbish, and on some Mathematica drawings the text at the axes isn't rotated. Ghostscript, however, gives the correct results. Moreover, replacing proprietary software should never be a bad idea.

The problem is that there has yet been no effort to make the SPARCPrinter driver behave like a BSD output filter. I made my tests using the script shown here.

Installation
~~~~~~~~~~~~~~~~~~~~~

Add ``sparc.dev`` to ``DEVICE_DEVS`` and compile Ghostscript as described in the documentation on how to build Ghostscript. Afterwards you can use the following script as an example for printing after modifying it with the right pathnames -- including for ``{GSPATH}`` the full ``pathname`` of the Ghostscript executable:

.. code-block:: postscript

   outcmd1='/vol/local/lib/troff2/psxlate -r'
   outcmd2='{GSPATH} -sDEVICE=sparc -sOUTPUTFILE=/dev/lpvi0 -'

   if [ $# -eq 0 ]
   then
     $outcmd1 | $outcmd2
   else
     cat $* | $outcmd1 | $outcmd2
   fi



Problems
~~~~~~~~~~~~~~~~~~~~~

Since ``/dev/lpi`` can be opened only for exclusive use, if another job has it open (``engine_ctl_sparc`` or another Ghostscript are the most likely candidates), Ghostscript stops with "Error: /invalidfileaccess in --.outputpage--"

In case of common printer problems like being out of paper, a warning describing the reason is printed to stdout. The driver tries access again each five seconds. Due to a problem with the device driver (in the kernel) the reason for printer failure isn't always reported correctly to the program. This is the case, for instance, if you open the top cover (error E5 on the printer's display). Look at the display on the printer itself if a "Printer problem with unknown reason" is reported. Fatal errors cause the print job to be terminated.


.. note::

   There is some confusion whether the resolution setting should be the integers 300 and 400, or the symbolic constants DPI300 and DPI400 (defined in ``lpviio.h``). Ghostscript releases have had it both ways. It is currently the latter. However, INOUE Namihiko reports (in bug #215256) that the former works better for him. If anyone has a definitive answer, please let us know.




Apple dot matrix printer
------------------------------

This section was contributed by `Mark Wedel`_.

The Apple Dot Matrix Printer (DMP) was a parallel predecessor to the Imagewriter printer. As far as I know, Imagewriter commands are a superset of the Dot Matrix printer's, so the driver should generate output that can be printed on Imagewriters.

To print images, the driver sets the printer for unidirectional printing and 15 characters per inch (cpi), or 120dpi. It sets the line feed to 1/9 inch. When finished, it sets the printer to bidirectional printing, 1/8-inch line feeds, and 12 cpi. There appears to be no way to reset the printer to initial values.

This code does not set for 8-bit characters (which is required). It also assumes that carriage return-newline is needed, and not just carriage return. These are all switch settings on the DMP, and I have configured them for 8-bit data and carriage return exclusively. Ensure that the Unix printer daemon handles 8-bit (binary) data properly; in my ``SunOS 4.1.1 printcap`` file the string "``ms=pass8,-opost``" works fine for this.

Finally, you can search ``devdemp.c`` for "Init" and "Reset" to find the strings that initialize the printer and reset things when finished, and change them to meet your needs.




Special and Test devices
------------------------------

The devices in this section are intended primarily for testing. They may be interesting as code examples, as well.


Raw 'bit' devices
~~~~~~~~~~~~~~~~~~~~~~~~~

There are a collection of 'bit' devices that don't do any special formatting but output 'raw' binary data for the page images. These are used for benchmarking but can also be useful when you want to directly access the raster data.

The raw devices are :title:`bit bitrgb bitcmyk`.


Bounding box output
~~~~~~~~~~~~~~~~~~~~~~~~~

There is a special :title:`bbox` "device" that just prints the bounding box of each page. You select it in the usual way:

.. code-block:: bash

   gs -dSAFER -dNOPAUSE -dBATCH -sDEVICE=bbox

It prints the output in a format like this:

.. code-block:: bash

   %%BoundingBox: 14 37 570 719
   %%HiResBoundingBox: 14.3ep08066 37.547999 569.495061 718.319158

Currently, it always prints the bounding box on stderr; eventually, it should also recognize ``-sOutputFile=``.

By default, white objects don't contribute to the bounding box because many files fill the whole page with white before drawing other objects. This can be changed by:

.. code-block:: bash

   << /WhiteIsOpaque true >> setpagedevice

Note that this device, like other devices, has a resolution and a (maximum) page size. As for other devices, the product (resolution x page size) is limited to approximately 500K pixels. By default, the resolution is 4000 DPI and the maximum page size is approximately 125", or approximately 9000 default (1/72") user coordinate units. If you need to measure larger pages than this, you must reset both the resolution and the page size in pixels, e.g.,

.. code-block:: bash

   gs -dNOPAUSE -dBATCH -sDEVICE=bbox -r100 -g500000x500000


.. note::

   The box returned by the :title:`bbox` device is just sufficient to contain the pixels which would be rendered by Ghostscript at the selected resolution. The language rendering rules can mean this differs by up to two pixels from the 'theoretical' area covered, and the precise area covered by curves and line joins is also, to some extent, related to the resolution. Finally the actual pixel position needs to be converted back to PostScript points, and that can be affected by mathematical precision, which can cause rounding errors. As a result the bounding box returned may differ very slightly from that which was expected.


Ink coverage output
~~~~~~~~~~~~~~~~~~~~~~~~~


There are two special :title:`inkcov` devices that print the ink coverage of each page; the :title:`inkcov` device and the :title:`ink_cov` device. They are selected like this:


.. code-block:: bash

   gs -dSAFER -dNOPAUSE -dBATCH -o- -sDEVICE=inkcov Y.pdf
   gs -dSAFER -dNOPAUSE -dBATCH -o- -sDEVICE=ink_cov Y.pdf


These commands also work as expected:

.. code-block:: bash

   gs -o X_inkcov.txt -sDEVICE=inkcov Y.pdf
   gs -o X_inkcov_page%03d.txt -sDEVICE=inkcov Y.pdf


The devices print their output in a format like this:

.. code-block:: postscript

   Page 1
    0.10022  0.09563  0.10071  0.06259 CMYK OK
   Page 2
    0.06108  0.05000  0.05834  0.04727 CMYK OK

The difference between the two devices is that the :title:`inkcov` device considers each rendered pixel and whether it marks the C, M, Y or K channels. So the percentages are a measure of how many device pixels contain that ink. The :title:`ink_cov` device gives the more traditional use of ink coverage, it also considers the amount of each colourant at each rendered pixel, so the percentages in this case are what percentage of the ink is used on the page.

As an example, If we take a page which is covered by a pure 100% cyan fill both devices would give the same result 1.00 0.00 0.00 0.00; each pixel is marked by the cyan ink and each pixel contains 100% cyan. If however we use a 50% cyan fill the inkcov device will still give 1.00 0.00 0.00 0.00 as 100% of the pixels contain cyan. The ink_cov device, however, would give a result of 0.50 0.00 0.00 0.00.


Permutation (DeviceN color model)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

With no additional parameters, the device named "permute" looks to Ghostscript like a standard CMYK contone device, and outputs a PPM file, using a simple CMYK->RGB transform. This should be the baseline for regression testing.

With the addition of ``-dPermute=1``, the internal behavior changes somewhat, but in most cases the resulting rendered file should be the same. In this mode, the color model becomes "DeviceN" rather than "DeviceCMYK", the number of components goes to six, and the color model is considered to be the (yellow, cyan, cyan, magenta, 0, black) tuple. This is what's rendered into the memory buffer. Finally, on conversion to RGB for output, the colors are permuted back.

As such, this code should check that all imaging code paths are 64-bit clean. Additionally, it should find incorrect code that assumes that the color model is one of ``DeviceGray``, ``DeviceRGB``, or ``DeviceCMYK``.

Currently, the code has the limitation of 8-bit continuous tone rendering only. An enhancement to do halftones is planned as well. Note, however, that when testing permuted halftones for consistency, it is important to permute the planes of the default halftone accordingly, and that any file which sets halftones explicitly will fail a consistency check.

spotcmyk (DeviceN color model)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The :title:`spotcmyk` device was created for debugging and testing of the :title:`devicen` extensions to Ghostscript that were released in version 8.0. There are also another device (:title:`devicen`) in the same source file. It were created for testing however it are not actually useful except as example code.


The :title:`spotcmyk` device was also designed to provide example code for a device which supports spot colors. Spot colors need to be specified prior to opening the first page. This can be done via adding the following to the command line: ``-c "<< /SeparationColorNames [ /Name1 /Name2 ] >> setpagedevice" -f``.

The :title:`spotcmyk` device produces a binary data file (similar to the :title:`bitcmyk` device) for the CMYK data. This data file has the name specified by the "OutputFile" parameter. The device also produces a binary data file (similar to the :title:`bitmono` device) for each spot color plane. These data files have the name specified by the "OutputFile" parameter with "sn" appended to the end (where "n" is the spot color number 0 to 12)".

After the :title:`spotcmyk` device produces the binary data files, the files are read and PCX format versions of these files are created with ".pcx" appended to the binary source file name.

If the :title:`spotcmyk` is being used with three spot colors and the "OutputFile" parameter is ``xxx`` then the following files would be created by the device:

.. list-table::
   :widths: 50 50
   :header-rows: 0

   * - ``xxx``
     - binary CMYK data
   * - ``xxxs0``
     - binary data for first spot color
   * - ``xxxs1``
     - binary data for second spot color
   * - ``xxxs2``
     - binary data for third spot color
   * - ``xxx.pcx``
     - CMYK data in PCX format
   * - ``xxxs0.pcx``
     - first spot color in PCX format
   * - ``xxxs1.pcx``
     - second spot color in PCX format
   * - ``xxxs2.pcx``
     - third spot color in PCX format


The :title:`spotcmyk` device has the creation of the binary data files separated from the creation of the PCX files since the source file is intended as example code and many people may not be interested in the PCX format. The PCX format was chosen because it was simple to implement from pre-existing code and viewers are available. The PCX format does have the dis-advantage that most of those viewers are on Windows.


.. _Devices_XCF:

XCF (DeviceN color model)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The XCF file format is the native image format for the GIMP program. This format is currently supported by two devices: :title:`xcfrgb` and :title:`xcfcmyk`.

We have been warned by the people supporting the GIMP program that they reserve the right to change the XCF format at anytime and thus these devices may become invalid. They are being included in the documentation because we have received some questions about what these devices do.

The XCF devices were created for testing of the :title:`devicen` extensions to Ghostscript which were released in version 8.0.

The :title:`xcfrgb` device uses a ``DeviceRGB`` process color model and creates a normal XCF file.

The :title:`xcfcmyk` device was created as a means of viewing spot colors for those users that do not have access to either Photoshop (see the PSD devices) or a PCX viewer (see the :title:`spotcmyk` device).

The :title:`xcfcmyk` device starts by using a ``DeviceCMYK`` process color model. The ``DeviceCMYK`` process color model allows the :title:`xcfcmyk` device to also support spot colors. Spot colors need to be specified prior to opening the first page. This can be done via adding the following to the command line:

.. code-block:: bash

   -c "<< /SeparationColorNames [ /Name1 /Name2 ] >> setpagedevice" -f

After a page is complete, the :title:`xcfcmyk` converts the CMYK image data into RGB for storing in the XCF output file. The XCF format does not currently support CMYK data directly. The spot color planes are converted into alpha channel planes. This is done because the XCF format does not currently support spot colors.











.. External links

.. _PNG website: http://www.libpng.org/pub/png/pngintro.html
.. _JPEG FAQ: http://www.faqs.org/faqs/jpeg-faq/
.. _hOCR: https://en.wikipedia.org/wiki/HOCR
.. _HP Linux Imaging and Printing: https://developers.hp.com/hp-linux-imaging-and-printing/
.. _Gimp print: gimp-print.sourceforge.net
.. _Mark Wedel: master@cats.ucsc.edu


.. include:: footer.rst

