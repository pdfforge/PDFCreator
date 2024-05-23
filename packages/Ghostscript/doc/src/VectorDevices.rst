.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: High Level Devices


.. include:: header.rst

.. _VectorDevices.html:


High Level Devices
===================================


High level devices are Ghostscript output devices which do not render to a raster, in general they produce 'vector' as opposed to bitmap output. Such devices currently include: :title:`pdfwrite`, :title:`ps2write`, :title:`eps2write`, :title:`txtwrite`, :title:`xpswrite`, :title:`pxlmono`, :title:`pxlcolor` and :title:`docxwrite`.

Although these devices produce output which is not a raster, they still work in the same general fashion as all Ghostscript devices. The input (PostScript, PDF, XPS, PCL or PXL) is handled by an appropriate interpreter, the interpreter processes the input and produces from it a sequence of drawing 'primitives' which are handed to the device. The device decides whether to handle the primitive itself, or call upon the graphics library to render the primitive to the final raster.

Primitives are quite low level graphics operations; as an example consider the PDF sequence ``'0 0 100 100 re f'``. This constructs a rectangle with the bottom left corner at 0,0 which is 100 units wide by 100 units high, and fills it with the current color. A lower level implementation using only primitives would first move the current point to 0,0, then construct a line to 0,100, then a line to 100,100, a line to 100, 0 and finally a line back to 0,0. It would then fill the result.

Obviously that's a simple example but it serves to demonstrate the point.

Now the raster devices all call the graphics library to process primitives (though they may choose to take some action first) and render the result to a bitmap. The high level devices instead reassemble the primitives back into high level page description and write the result to a file. This means that the output, while it should be visually the same as the input (because it makes the same marks), is not the same as the original input, even if the output Page Description Language is the same as the input one was (eg PDF to PDF).

Why is this important? Firstly because the description of the page won't be the same, if your workflow relies upon (for example) finding rectangles in the description then it might not work after it has been processed by a high level device, as the rectangles may all have turned into lengthy path descriptions.

In addition, any part of the original input which does not actually make marks on the page (such as hyperlinks, bookmarks, comments etc) will normally not be present in the output, even if the output is the same format. In general the PDF interpreter and the PDF output device (:title:`pdfwrite`) try to preserve the non-marking information from the input, but some kinds of content are not carried across, in particular comments are not preserved.

We often hear from users that they are 'splitting' PDF files, or 'modifying' them, or converting them to PDF/A, and it's important to realize that this is not what's happening. Instead, a new PDF file is being created, which should look the same as the original, but the actual insides of the PDF file are not the same as the original. This may not be a problem, but if it's important to keep the original contents, then you need to use a different tool (we'd suggest MuPDF_, also available from Artifex). Of course, if the intention is to produce a modified PDF file (for example, reducing the resolution of images, or changing the colour space), then clearly you cannot keep the original contents unchanged, and :title:`pdfwrite` performs these tasks well.



PCL-XL (PXL)
--------------

The :title:`pxlmono` and :title:`pxlcolor` devices output HP PCL-XL, a graphic language understood by many recent laser printers.

Options
~~~~~~~~~~~~


``-dCompressMode=1 | 2 | 3 (default is 1)``
   Set the compression algorithm used for bitmap graphics. RLE=1, JPEG=2, DeltaRow=3. When JPEG=2 is on, it is applied only to full-color images; indexed-color graphics and masks continues to be compressed with RLE.



Text output
--------------

The :title:`txtwrite` device will output the text contained in the original document as Unicode.

Options
~~~~~~~~~~~~

``-dTextFormat=0 | 1 | 2 | 3 | 4 (default is 3)``
   Format 0 is intended for use by developers and outputs XML-escaped Unicode along with information regarding the format of the text (position, font name, point size, etc). The XML output is the same format as the MuPDF output, but no additional processing is performed on the content, so no block detection.

   Format 1 uses the same XML output format, but attempts similar processing to MuPDF, and will output blocks of text. Note the algorithm used is not the same as the MuPDF code, and so the results will not be identical.

   Format 2 outputs Unicode (UCS2) text (with a Byte Order Mark) which approximates the layout of the text in the original document.

   Format 3 is the same as format 2, but the text is encoded in UTF-8.

   Format 4 is internal format similar to Format 0 but with extra information.


DOCX output
--------------

The :title:`docxwrite` device creates a DOCX file suitable for use with applications such as Word or LibreOffice, containing the text in the original document.

Rotated text is placed into textboxes. Heuristics are used to group glyphs into words, lines and paragraphs; for some types of formatting, these heuristics may not be able to recover all of the original document structure.

This device currently has no special configuration parameters.


XPS file output
----------------------------

The :title:`xpswrite` device writes its output according to the Microsoft XML Paper Specification. This specification was later amended to the Open XML Paper specification, submitted to ECMA International and adopted as ECMA-388.

This device currently has no special configuration parameters.


The family of PDF and PostScript output devices
---------------------------------------------------



Common controls and features
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The PDF and PostScript (including Encapsulated PostScript, or EPS) devices have much of their code in common, and so many of the controlling parameters are also common amongst the devices. The pdfwrite, ps2write and eps2write devices create PDF or PostScript files whose visual appearance should match, as closely as possible, the appearance of the original input (PS, PDF, XPS, PCL, PXL). There are a number of caveats as mentioned in the overview above. In addition to the general comments there are some additional points that bear mentioning.

PCL has a graphics model which differs significantly from the PostScript or PDF one, in particular it has a form of transparency called ``RasterOps``, some aspects of which cannot be represented in PDF at a high level (or at all, in PostScript). The :title:`pdfwrite` device makes no attempt to handle this, and the resulting PDF file will not match the original input. The only way to deal with these types of file is to render the whole page to a bitmap and then 'wrap' the bitmap as a PDF file. Currently we do not do this either, but it is possible that a future enhancement may do so.

If the input contains PDF-compatible transparency, but the :title:`ps2write` device is selected, or the :title:`pdfwrite` device is selected, but has been told to limit the PDF feature set to a version less than 1.4, the transparency cannot be preserved. In this case the entire page is rendered to a bitmap and that bitmap is 'wrapped up' in appropriate PDF or PostScript content. The output should be visually the same as the input, but since it has been rendered it will not scale up or down well, unlike the original, vector, content of the input.

The options in the command line may include any switches that may be used with the language interpreter appropriate for the input (see :ref:`here<Use_Command line options>` for a complete list). In addition the following options are common to all the :title:`pdfwrite` family of devices, and should work when specified on the command line with any of the language interpreters.

``-rresolution``
   Sets the resolution for pattern fills, for fonts that must be converted to bitmaps and any other rendering required (eg rendering transparent pages for output to PDF versions < 14). The default internal resolution for :title:`pdfwrite` is 720dpi.

``-dUNROLLFORMS``
   When converting from PostScript, :title:`pdfwrite` (and :title:`ps2write`) preserve the use of Form resources as Form XObjects in the output. Some badly written PostScript can cause this to produce incorrect output (the Quality Logic CET tests for example). By setting this flag, forms will be unrolled and stored in the output each time they are used, which avoids the problems. Note that the output file will of course be larger this way. We do not attempt to preserve Form XObjects from PDF files, unless they are associated with transparency groups.


.. _VectorDevices_NoOutputFonts:

``-dNoOutputFonts``
   Ordinarily the :title:`pdfwrite` device family goes to considerable lengths to preserve fonts from the input as fonts in the output. However in some highly specific cases it can be useful to have the text emitted as linework/bitmaps instead. Setting this switch will prevent these devices from emitting any fonts, all text will be stored as vectors (or bitmaps in the case of bitmapped fonts) in the page content stream. Note that this will produce larger output which will process more slowly, render differently and particularly at lower resolution produce less consistent text rendering. Use with caution.

  **PostScript device parameters**

  These controls can only be set from PostScript as they require a PostScript array to contain the names of the fonts.


  ``AlwaysOutline``
    Similar to the :ref:`-dNoOutputFonts<VectorDevices_NoOutputFonts>` control, this can be used to control whether text is converted into linework or preserved as a font, based on the name. Text using a font which is listed in the ``AlwaysOutline`` array will always be converted into linework and the font will not be embedded in the output. Subset fonts have a prefix of the form 'ABCDE+' (Acrobat does not display the prefix in the Fonts dialog) and the code will remove this from fonts used in the document before matching so you should not specify that part of the font name.

    Example usage:

    .. code-block:: c

      gs -sDEVICE=pdfwrite -o out.pdf -c "<< /AlwaysOutline [/Calibri (Comic Sans) cvn] >> setdistillerparams" -f input.pdf
    
    .. note::
    
      CIDFonts are composed with the CMap name to produce the final font name, and it is not possible to reliably remove the CMap name in the font name matching, so you must specify the composed name.

    .. code-block:: c
    
      gs -sDEVICE=pdfwrite -o out.pdf -c "<< /AlwaysOutline [/Calibri-Identity-H (Comic Sans-Identity-H) cvn] >> setdistillerparams" -f input.pdf


  ``NeverOutline``
    Associated with the :ref:`-dNoOutputFonts<VectorDevices_NoOutputFonts>` control, this can be used to control whether text is converted into linework or preserved as a font, based on the name. When :ref:`-dNoOutputFonts<VectorDevices_NoOutputFonts>` is set to `true`, text using a font which is listed in the ``NeverOutline`` array will not be converted into linework but will continue to use a font, which will be embedded in the output, subject to the embedding rules.

    Example usage:

    .. code-block:: c

      gs -sDEVICE=pdfwrite -o out.pdf -dNoOutputFonts -c"<< /NeverOutline [/Calibri (Conic Sans) cvn] >> setdistillerparams" -f input.pdf


    .. note::

      When using ``NeverOutline``, if :ref:`-dNoOutputFonts<VectorDevices_NoOutputFonts>` is not set to `true` then this control will have no effect.



``-dCompressFonts=boolean``
   Defines whether :title:`pdfwrite` will compress embedded fonts in the output. The default value is true; the false setting is intended only for debugging as it will result in larger output.

``-dCompressStreams=boolean``
   Defines whether :title:`pdfwrite` will compress streams other than those in fonts or pages in the output. The default value is true; the false setting is intended only for debugging as it will result in larger output.




Distiller Parameters
""""""""""""""""""""""

Options may also include ``-dparameter=value`` or ``-sparameter=string`` switches for setting "distiller parameters", Adobe's documented parameters for controlling the conversion of PostScript into PDF. The PostScript ``setdistillerparams`` and ``currentdistillerparams`` operators are also recognized when the input is PostScript, and provide an equivalent way to set these parameters from within a PostScript input file.

Although the name implies that these parameters are for controlling PDF output, in fact the whole family of devices use these same parameters to control the conversion into PostScript and EPS as well.

The :title:`pdfwrite` family of devices recognize all of the Acrobat Distiller 5 parameters defined in the DistillerParameters (version 5) document available from the Adobe web site. Cells in the table below containing '=' mean that the value of the parameter is the same as in the "default" column.




.. list-table::
   :header-rows: 1

   * - Parameter name
     - Notes
     - default
     - screen
     - ebook
     - printer
     - prepress
   * - AlwaysEmbed
     - :ref:`(13)<DistillerParameters_note_13>`
     - [ ]
     - =
     - =
     - =
     - =
   * - AntiAliasColorImages
     - :ref:`(0)<DistillerParameters_note_0>`
     - false
     - =
     - =
     - =
     - =
   * - AntiAliasGrayImages
     - :ref:`(0)<DistillerParameters_note_0>`
     - false
     - =
     - =
     - =
     - =
   * - AntiAliasMonoImages
     - :ref:`(0)<DistillerParameters_note_0>`
     - false
     - =
     - =
     - =
     - =
   * - ASCII85EncodePages
     -
     - false
     - =
     - =
     - =
     - =
   * - AutoFilterColorImages
     - :ref:`(1)<DistillerParameters_note_1>`
     - true
     - =
     - =
     - =
     - =
   * - AutoFilterGrayImages
     - :ref:`(1)<DistillerParameters_note_1>`
     - true
     - =
     - =
     - =
     - =
   * - AutoPositionEPSFiles
     - :ref:`(0)<DistillerParameters_note_0>`
     - true
     - =
     - =
     - =
     - =
   * - AutoRotatePages
     -
     - /PageByPage
     - /PageByPage
     - /All
     - /None
     - /None
   * - Binding
     - :ref:`(0)<DistillerParameters_note_0>`
     - /Left
     - =
     - =
     - =
     - =
   * - CalCMYKProfile
     - :ref:`(0)<DistillerParameters_note_0>`
     - ()
     - =
     - =
     - =
     - =
   * - CalGrayProfile
     - :ref:`(0)<DistillerParameters_note_0>`
     - ()
     - =
     - =
     - =
     - =
   * - CalRGBProfile
     - :ref:`(0)<DistillerParameters_note_0>`
     - ()
     - =
     - =
     - =
     - =
   * - CannotEmbedFontPolicy
     - :ref:`(0)<DistillerParameters_note_0>`
     - /Warning
     - /Warning
     - /Warning
     - /Warning
     - /Error
   * - ColorACSImageDict
     - :ref:`(13)<DistillerParameters_note_13>`
     - :ref:`(7)<DistillerParameters_note_7>`
     - :ref:`(10)<DistillerParameters_note_10>`
     - :ref:`(10)<DistillerParameters_note_10>`
     - :ref:`(8)<DistillerParameters_note_8>`
     - :ref:`(9)<DistillerParameters_note_9>`
   * - ColorConversionStrategy
     - :ref:`(6)<DistillerParameters_note_6>`
     - LeaveColorUnchanged
     - RGB
     - RGB
     - UseDeviceIndependentColor
     - LeaveColorUnchanged
   * - ColorImageDepth
     -
     - -1
     - =
     - =
     - =
     - =
   * - ColorImageDict
     - :ref:`(13)<DistillerParameters_note_13>`
     - :ref:`(7)<DistillerParameters_note_7>`
     - =
     - =
     - =
     - =
   * - ColorImageFilter
     -
     - /DCTEncode
     - =
     - =
     - =
     - =
   * - ColorImageDownsampleThreshold
     -
     - 1.5
     - =
     - =
     - =
     - =
   * - ColorImageDownsampleType
     - :ref:`(3)<DistillerParameters_note_3>`
     - /Subsample
     - /Average
     - /Average
     - /Average
     - /Bicubic
   * - ColorImageResolution
     -
     - 72
     - 72
     - 150
     - 300
     - 300
   * - CompatibilityLevel
     -
     - 1.7
     - 1.5
     - 1.5
     - 1.7
     - 1.7
   * - CompressPages
     - :ref:`(14)<DistillerParameters_note_14>`
     - true
     - =
     - =
     - =
     - =
   * - ConvertCMYKImagesToRGB
     -
     - false
     - =
     - =
     - =
     - =
   * - ConvertImagesToIndexed
     - :ref:`(0)<DistillerParameters_note_0>`
     - false
     - =
     - =
     - =
     - =
   * - CoreDistVersion
     -
     - 4000
     - =
     - =
     - =
     - =
   * - CreateJobTicket
     - :ref:`(0)<DistillerParameters_note_0>`
     - false
     - false
     - false
     - true
     - true
   * - DefaultRenderingIntent
     -
     - /Default
     - =
     - =
     - =
     - =
   * - DetectBlends
     - :ref:`(0)<DistillerParameters_note_0>`
     - true
     - =
     - =
     - =
     - =
   * - DoThumbnails
     - :ref:`(0)<DistillerParameters_note_0>`
     - false
     - false
     - false
     - false
     - true
   * - DownsampleColorImages
     -
     - false
     - true
     - true
     - false
     - false
   * - DownsampleGrayImages
     -
     - false
     - true
     - true
     - false
     - false
   * - DownsampleMonoImages
     -
     - false
     - true
     - true
     - false
     - false
   * - EmbedAllFonts
     -
     - true
     - false
     - true
     - true
     - true
   * - EmitDSCWarnings
     - :ref:`(0)<DistillerParameters_note_0>`
     - false
     - =
     - =
     - =
     - =
   * - EncodeColorImages
     -
     - true
     - =
     - =
     - =
     - =
   * - EncodeGrayImages
     -
     - true
     - =
     - =
     - =
     - =
   * - EncodeMonoImages
     -
     - true
     - =
     - =
     - =
     - =
   * - EndPage
     - :ref:`(0)<DistillerParameters_note_0>`
     - -1
     - =
     - =
     - =
     - =
   * - GrayACSImageDict
     - :ref:`(13)<DistillerParameters_note_13>`
     - :ref:`(7)<DistillerParameters_note_7>`
     - :ref:`(7)<DistillerParameters_note_7>`
     - :ref:`(10)<DistillerParameters_note_10>`
     - :ref:`(8)<DistillerParameters_note_8>`
     - :ref:`(9)<DistillerParameters_note_9>`
   * - GrayImageDepth
     -
     - -1
     - =
     - =
     - =
     - =
   * - GrayImageDict
     - :ref:`(13)<DistillerParameters_note_13>`
     - :ref:`(7)<DistillerParameters_note_7>`
     - =
     - =
     - =
     - =
   * - GrayImageDownsampleThreshold
     -
     - 1.5
     - =
     - =
     - =
     - =
   * - GrayImageDownsampleType
     - :ref:`(3)<DistillerParameters_note_3>`
     - /Subsample
     - /Average
     - /Bicubic
     - /Bicubic
     - /Bicubic
   * - GrayImageFilter
     -
     - /DCTEncode
     - =
     - =
     - =
     - =
   * - GrayImageResolution
     -
     - 72
     - 72
     - 150
     - 300
     - 300
   * - ImageMemory
     - :ref:`(0)<DistillerParameters_note_0>`
     - 524288
     - =
     - =
     - =
     - =
   * - LockDistillerParams
     -
     - false
     - =
     - =
     - =
     - =
   * - LZWEncodePages
     - :ref:`(2)<DistillerParameters_note_2>`
     - false
     - =
     - =
     - =
     - =
   * - MaxSubsetPct
     -
     - 100
     - =
     - =
     - =
     - =
   * - MonoImageDepth
     -
     - -1
     - =
     - =
     - =
     - =
   * - MonoImageDict
     - :ref:`(13)<DistillerParameters_note_13>`
     - <<K -1>>
     - =
     - =
     - =
     - =
   * - MonoImageDownsampleThreshold
     -
     - 1.5
     - =
     - =
     - =
     - =
   * - MonoImageDownsampleType
     -
     - /Subsample
     - /Subsample
     - /Subsample
     - /Subsample
     - /Subsample
   * - MonoImageFilter
     -
     - /CCITTFaxEncode
     - =
     - =
     - =
     - =
   * - MonoImageResolution
     -
     - 300
     - 300
     - 300
     - 1200
     - 1200
   * - NeverEmbed
     - :ref:`(13)<DistillerParameters_note_13>`
     - :ref:`(11)<DistillerParameters_note_11>` :ref:`(12)<DistillerParameters_note_12>`
     - :ref:`(11)<DistillerParameters_note_11>` :ref:`(12)<DistillerParameters_note_12>`
     - :ref:`(11)<DistillerParameters_note_11>` :ref:`(12)<DistillerParameters_note_12>`
     - [] :ref:`(12)<DistillerParameters_note_12>`
     - [] :ref:`(12)<DistillerParameters_note_12>`
   * - OffOptimizations
     -
     - 0
     - =
     - =
     - =
     - =
   * - OPM
     -
     - 1
     - =
     - =
     - =
     - =
   * - Optimize
     - :ref:`(0)<DistillerParameters_note_0>` :ref:`(5)<DistillerParameters_note_5>`
     - false
     - true
     - true
     - true
     - true
   * - ParseDSCComments
     -
     - true
     - =
     - =
     - =
     - =
   * - ParseDSCCommentsForDocInfo
     -
     - true
     - =
     - =
     - =
     - =
   * - PreserveCopyPage
     - :ref:`(0)<DistillerParameters_note_0>`
     - true
     - =
     - =
     - =
     - =
   * - PreserveEPSInfo
     - :ref:`(0)<DistillerParameters_note_0>`
     - true
     - =
     - =
     - =
     - =
   * - PreserveHalftoneInfo
     -
     - false
     - =
     - =
     - =
     - =
   * - PreserveOPIComments
     - :ref:`(0)<DistillerParameters_note_0>`
     - false
     - false
     - false
     - true
     - true
   * - PreserveOverprintSettings
     -
     - false
     - false
     - false
     - true
     - true
   * - sRGBProfile
     - :ref:`(0)<DistillerParameters_note_0>`
     - ()
     - =
     - =
     - =
     - =
   * - StartPage
     - :ref:`(0)<DistillerParameters_note_0>`
     - 1
     - =
     - =
     - =
     - =
   * - SubsetFonts
     -
     - true
     - =
     - =
     - =
     - =
   * - TransferFunctionInfo
     - :ref:`(4)<DistillerParameters_note_4>`
     - /Preserve
     - =
     - =
     - =
     - =
   * - UCRandBGInfo
     -
     - /Remove
     - /Remove
     - /Remove
     - /Preserve
     - /Preserve
   * - UseFlateCompression
     - :ref:`(2)<DistillerParameters_note_2>`
     - true
     - =
     - =
     - =
     - =
   * - UsePrologue
     - :ref:`(0)<DistillerParameters_note_0>`
     - false
     - =
     - =
     - =
     - =
   * - PassThroughJPEGImages
     - :ref:`(15)<DistillerParameters_note_15>`
     - true
     - =
     - =
     - =
     - =
   * - PassThroughJPXImages
     - :ref:`(16)<DistillerParameters_note_16>`
     - true
     - =
     - =
     - =
     - =

.. _DistillerParameters_note_0:

Note 0
^^^^^^^^^
   This parameter can be set and queried, but currently has no effect.

.. _DistillerParameters_note_1:

Note 1
^^^^^^^^^
   ``-dAutoFilterxxxImages=false`` works since Ghostscript version 7.30. Older versions of Ghostscript don't examine the image to decide between JPEG and LZW or Flate compression: they always use Flate compression.

.. _DistillerParameters_note_2:

Note 2
^^^^^^^^^
   Because the LZW compression scheme was covered by patents at the time this device was created, :title:`pdfwrite` does not actually use LZW compression: all requests for LZW compression are ignored. ``UseFlateCompression`` is treated as always on, but the switch ``CompressPages`` can be set to false to turn off page level stream compression. Now that the patent has expired, we could change this should it become worthwhile.

.. _DistillerParameters_note_3:

Note 3
^^^^^^^^^
   The ``xxxDownsampleType`` parameters can also have the value ``/Bicubic`` (a Distiller 4 feature), this will use a Mitchell filter. (older versions of :title:`pdfwrite` simply used Average instead). If a non-integer downsample factor is used the code will clamp to the nearest integer (if the difference is less than 0.1) or will silently switch to the old bicubic filter, NOT the Mitchell filter.

.. _DistillerParameters_note_4:

Note 4
^^^^^^^^^
   The default for transfer functions is to preserve them, this is because transfer functions are a device-dependent feature, a set of transfer functions designed for an RGB device will give incorrect output on a CMYK device for instance. The :title:`pdfwrite` device does now support ``/Preserve``, ``/Apply`` and ``/Remove`` (the previous documentation was incorrect, application of transfer functions was not supported). PDF 2.0 deprecates the use of transfer functions, and so when producing PDF 2.0 compatible output if the ``TransferFunctionInfor`` is set to ``/Preserve`` it will be silently replaced with ``/Apply``. You can instead specifically set ``TransferFunctionInfo`` to ``/Remove`` when producing PDF 2.0 in order to avoid the transfer function being applied.

.. _DistillerParameters_note_5:

Note 5
^^^^^^^^^
   Use the ``-dFastWebView`` command line switch to 'optimize' output.

.. _DistillerParameters_note_6:

Note 6
^^^^^^^^^
   The value ``UseDeviceIndependentColorForImages`` works the same as ``UseDeviceIndependentColor``. The value sRGB actually converts to RGB with the default Ghostscript conversion. The new Ghostscript-specific value Gray converts all colors to ``DeviceGray``. With the introduction of new color conversion code in version 9.11 it is no longer necessary to set ``ProcessColorModel`` when selecting Gray, RGB or CMYK. It is also no longer necessary to set ``UseCIEColor`` for ``UseDeviceIndependentColor`` to work properly, and the use of ``UseCIEColor`` is now strongly discouraged.

.. _DistillerParameters_note_7:

Note 7
^^^^^^^^^
The default image parameter dictionary is:

.. code-block:: bash

   << /QFactor 0.9 /Blend 1 /HSamples [2 1 1 2] /VSamples [2 1 1 2] >>

.. _DistillerParameters_note_8:

Note 8
^^^^^^^^^
The printer ACS image parameter dictionary is:

   .. code-block:: bash

      << /QFactor 0.4 /Blend 1 /ColorTransform 1 /HSamples [1 1 1 1] /VSamples [1 1 1 1] >>

.. _DistillerParameters_note_9:

Note 9
^^^^^^^^^
The prepress ACS image parameter dictionary is:

   .. code-block:: bash

      << /QFactor 0.15 /Blend 1 /ColorTransform 1 /HSamples [1 1 1 1] /VSamples [1 1 1 1] >>

.. _DistillerParameters_note_10:

Note 10
^^^^^^^^^
   The screen and ebook ACS image parameter dictionary is:

   .. code-block:: bash

      << /QFactor 0.76 /Blend 1 /ColorTransform 1 /HSamples [2 1 1 2] /VSamples [2 1 1 2] >>

.. _DistillerParameters_note_11:

Note 11
^^^^^^^^^
   The default, screen, and ebook settings never embed the 14 standard fonts (Courier, Helvetica, and Times families, Symbol, and ZapfDingbats). This behaviour is intentional but can be overridden by:

   .. code-block:: bash

      << /NeverEmbed [ ] >> setdistillerparams

.. _DistillerParameters_note_12:

Note 12
^^^^^^^^^
   ``NeverEmbed`` can include CID font names. If a CID font is substituted in ``lib/cidfmap``, the substitute font name is used when the CID font is embedded, and the original CID font name is used when it is not embedded. ``NeverEmbed`` should always specify the original CID font name.

.. _DistillerParameters_note_13:

Note 13
^^^^^^^^^
   The arrays ``AlwaysEmbed`` and ``NeverEmbed`` and image parameter dictionaries ``ColorACSImageDict``, ``ColorACSImageDict``, ``ColorImageDict``, ``GrayACSImageDict``, ``GrayImageDict``, ``MonoImageDict`` cannot be specified on the command line. To specify these, you must use PostScript, either by including it in the PostScript source or by passing the ``-c`` command-line parameter to Ghostscript as described in Limitations_ below. For example, including the PostScript string in your file ``in.ps``:

   .. code-block:: bash

      <</AlwaysEmbed [/Helvetica /Times-Roman]>> setdistillerparams

   is equivalent to invoking:

   .. code-block:: bash

      gs -dBATCH -dSAFER -DNOPAUSE -q -sDEVICE=pdfwrite -sOutputFile=out.pdf -c '<</AlwaysEmbed [/Helvetica /Times-Roman]>> setdistillerparams' -f in.ps

   or using the extra parameters in a file:

   .. code-block:: bash

      @params.in

   where the file ``params.in`` contains:

   .. code-block:: bash

      -c '<</AlwaysEmbed [/Helvetica /Times-Roman]>> setdistillerparams' -f in.ps


.. _DistillerParameters_note_14:

Note 14
^^^^^^^^^
   The default value of ``CompressPages`` is false for :title:`ps2write` and :title:`eps2write`.

.. _DistillerParameters_note_15:

Note 15
^^^^^^^^^
   When true image data in the source which is encoded using the DCT (JPEG) filter will not be decompressed and then recompressed on output. This prevents the multiplication of JPEG artefacts caused by lossy compression. ``PassThroughJPEGImages`` currently only affects simple JPEG images. It has no effect on JPX (JPEG2000) encoded images (see below) or masked images. In addition this parameter will be ignored if the :title:`pdfwrite` device needs to modify the source data. This can happen if the image is being downsampled, changing colour space or having transfer functions applied. Note that this parameter essentially overrides the ``EncodeColorImages`` and ``EncodeGrayImages`` parameters if they are false, the image will still be written with a ``DCTDecode`` filter. NB this feature currently only works with PostScript or PDF input, it does not work with PCL, PXL or XPS input.

.. _DistillerParameters_note_16:

Note 16
^^^^^^^^^
   When true image data in the source which is encoded using the JPX (JPEG 2000) filter will not be decompressed and then recompressed on output. This prevents the multiplication of JPEG artefacts caused by lossy compression. ``PassThroughJPXImages`` currently only affects simple JPX encoded images. It has no effect on JPEG encoded images (see above) or masked images. In addition this parameter will be ignored if the :title:`pdfwrite` device needs to modify the source data. This can happen if the image is being downsampled, changing colour space or having transfer functions applied. Note that this parameter essentially overrides the ``EncodeColorImages`` and ``EncodeGrayImages`` parameters if they are false, the image will still be written with a ``JPXDecode`` filter. NB this feature currently only works with PostScript or PDF input, it does not work with PCL, PXL or XPS input.



Color Conversion and Management
""""""""""""""""""""""""""""""""""""""""""""

As of the 9.11 pre-release, the color management in the :title:`pdfwrite` family has been substantially altered so that it now uses the same Color Management System as rendering (the default is LCMS2). This considerably improves the color handling in both :title:`pdfwrite` and :title:`ps2write`, particularly in the areas of ``Separation`` and :title:`devicen` color spaces, and ``Indexed`` color spaces with images.

Note that while :title:`pdfwrite` uses the same CMS as the rendering devices, this does not mean that the entire suite of options is available, as described in the ``GS9_Colour Management.pdf`` file. The colour management code has no effect at all unless either ``ColorConversionStrategy`` or ``ConvertCMYKImagesToRGB`` is set, or content has to be rendered to an image (this is rare and usually required only when converting a PDF file with transparency to a version < PDF 1.4).

Options based on object type (image, text, linework) are not used, all objects are converted using the same scheme. ``-dKPreserve`` has no effect because we will not convert CMYK to CMYK. ``-dDeviceGrayToK`` also has no effect; when converting to CMYK ``DeviceGray`` objects are left in ``DeviceGray`` since that can be mapped directly to the K channel.

The ``ColorConversionStrategy`` switch can now be set to ``LeaveColorUnchanged``, ``Gray``, ``RGB``, ``CMYK`` or ``UseDeviceIndependentColor``. Note that, particularly for :title:`ps2write`, ``LeaveColorUnchanged`` may still need to convert colors into a different space (ICCbased colors cannot be represented in PostScript for example). ``ColorConversionStrategy`` can be specified either as; a string by using the ``-s`` switch (``-sColorConversionStrategy=RGB``) or as a name using the ``-d`` switch (``-dColorConversionStrategy=/RGB``).
:title:`ps2write` cannot currently convert into device-independent color spaces, and so ``UseDeviceIndependentColor`` should not be used with :title:`ps2write` (or :title:`eps2write`).

All other color spaces are converted appropriately. ``Separation`` and :title:`devicen` spaces will be preserved if possible (:title:`ps2write` cannot preserve :title:`devicen` or ``Lab``) and if the alternate space is not appropriate a new alternate space will be created, e.g. a ``[/Separation (MyColor) /DeviceRGB {...}]`` when the ``ColorConversionStrategy`` is set to CMYK would be converted to ``[/Separation (MyColor) /DeviceCMYK {...}]`` The new tint transform will be created by sampling the original tint transform, converting the RGB values into CMYK, and then creating a function to linearly interpolate between those values.

The ``PreserveSeparation`` switch now controls whether the :title:`pdfwrite` family of devices will attempt to preserve ``Separation`` spaces. If this is set to false then all ``Separation`` colours will be converted into the current device space specified by ``ProcessColorModel``.



Setting page orientation
""""""""""""""""""""""""""""""""""""""""""""

By default Ghostscript determines viewing page orientation based on the dominant text orientation on the page. Sometimes, when the page has text in several orientations or has no text at all, wrong orientation can be selected.

Acrobat Distiller parameter ``AutoRotatePages`` controls the automatic orientation selection algorithm. On Ghostscript, besides input stream, Distiller parameters can be given as command line arguments. For instance: ``-dAutoRotatePages=/None`` or ``/All`` or ``/PageByPage``.

When there is no text on the page or automatic page rotation is set to ``/None`` an orientation value from ``setpagedevice`` is used. Valid values are: 0 (portrait), 3 (landscape), 2 (upside down), and 1 (seascape). The orientation can be set from the command line as ``-c "<</Orientation 3>> setpagedevice"`` using Ghostscript directly but cannot be set in ``ps2pdf``. See Limitations_ below.

Ghostscript passes the orientation values from DSC comments to the :title:`pdfwrite` driver, and these are compared with the auto-rotate heuristic. If they are different then the DSC value will be used preferentially. If the heuristic is to be preferred over the DSC comments then comment parsing can be disabled by setting ``-dParseDSCComments=false``.


Controls and features specific to PostScript and PDF input
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

``-dPDFSETTINGS=configuration``
   Presets the "distiller parameters" to one of the following predefined settings:

- ``/screen`` selects low-resolution output similar to the Acrobat Distiller (up to version X) "Screen Optimized" setting.

- ``/ebook`` selects medium-resolution output similar to the Acrobat Distiller (up to version X) "eBook" setting.

- ``/printer`` selects output similar to the Acrobat Distiller "Print Optimized" (up to version X) setting.

- ``/prepress`` selects output similar to Acrobat Distiller "Prepress Optimized" (up to version X) setting.

- ``/default`` selects output intended to be useful across a wide variety of uses, possibly at the expense of a larger output file.


.. note ::

   Adobe has recently changed the names of the presets it uses in Adobe Acrobat Distiller, in order to avoid confusion with earlier versions we do not plan to change the names of the ``PDFSETTINGS`` parameters. The precise value for each control is listed in the table above.

Please be aware that the ``/prepress`` setting does not indicate the highest quality conversion. Using any of these presets will involve altering the input, and as such may result in a PDF of poorer quality (compared to the input) than simply using the defaults. The 'best' quality (where best means closest to the original input) is obtained by not setting this parameter at all (or by using /default).

The ``PDFSETTINGS`` presets should only be used if you are sure you understand that the output will be altered in a variety of ways from the input. It is usually better to adjust the controls individually (see the table below) if you have a genuine requirement to produce, for example, a PDF file where the images are reduced in resolution.



Controls and features specific to PCL and PXL input
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Many of the controls used for distiller parameters can be used on the command line with the ``-d`` or ``-s`` switches, and these will work correctly with PCL or PXL input. However, some controls (e.g. ``/NeverEmbed``) do not take simple numeric or string arguments, and these cannot be set from the command line. When the input is PostScript or PDF we can use the ``-c`` and ``-f`` switches to send PostScript through the interpreter to control these parameters, but clearly this is not possible when the interpreter does not understand PostScript. In addition some features are controlled using the PostScript ``pdfmark`` operator and again that clearly is not possible unless we are using a PostScript interpreter to read the input.

To overcome this new, GhostPCL-specific, PJL parameters have been added. These parameters are defined as ``PDFMARK`` and ``SETDISTILLERPARAMS``. In order to reduce confusion when using PostScript and PCL as inputs these PJL parameters take essentially the same PostScript constructs as the corresponding PostScript operators ``pdfmark`` and ``setdistillerparams``. However it is important to realise that these are not processed by a full PostScript interpreter, and there are syntactic rules which must be followed carefully when using these parameters.

You cannot use arbitrary PostScript operators, only boolean, number, name, string, array and dictionary objects are supported (but see ``PUTFILE`` later). All tokens must be separated by white space, so while this ``[/Test(string)]`` is perfectly valid in PostScript, you must instead write it as ``[ /Test (string) ]`` for PJL parsing. All ``PDFMARK`` and ``SETDISTILLERPARAMS`` must be set as ``DEFAULT``, the values must be on a single line, and delimited by ``""``.

``pdfmarks`` sometimes require the insertion of file objects (especially for production of PDF/A files) so we must find some way to handle these. This is done (for the ``pdfmark`` case only) by defining a special (non-standard) pdfmark name ``PUTFILE``, this simply takes the preceding string, and uses it as a fully qualified path to a file. Any further ``pdfmark`` operations can then use the named object holding the file to access it.

The easiest way to use these parameters is to create a 'settings' file, put all the commands in it, and then put it on the command line immediately before the real input file. For example:

.. code-block:: bash

   ./gpcl6 -sDEVICE=pdfwrite -dPDFA=1 -dCompressPages=false -dCompressFonts=false -sOutputFile=./out.pdf ./pdfa.pjl ./input.pcl

Where ``pdfa.pjl`` contains the PJL commands to create a PDF/A-1b file (see example below).

Example creation of a PDF/A output file
""""""""""""""""""""""""""""""""""""""""""""""

For readability the line has been bisected, when used for real this must be a single line. The 'ESC' represents a single byte, value ``0x1B``, an escape character in ASCII. The line must end with an ASCII newline ``(\n, 0x0A)`` and this must be the only newline following the ``@PJL``. The line breaks between ``""`` below should be replaced with space characters, the double quote characters (``"``) are required.

.. code-block:: postscript

   ESC%-12345X
   @PJL DEFAULT PDFMARK = "
   [ /_objdef {icc_PDFA} /type /stream /OBJ pdfmark
   [ {icc_PDFA} << /N 3 >> /PUT pdfmark
   [ {icc_PDFA} (/ghostpdl/iccprofiles/default_rgb.icc) /PUTFILE pdfmark
   [ /_objdef {OutputIntent_PDFA} /type /dict /OBJ pdfmark
   [ {OutputIntent_PDFA} << /S /GTS_PDFA1 /Type /OutputIntent /DestOutputProfile {icc_PDFA} /OutputConditionIdentifier (sRGB) >> /PUT pdfmark
   [ {Catalog} << /OutputIntents [{OutputIntent_PDFA}] >> /PUT pdfmark
   [ /Author (Ken) /Creator (also Ken) /Title (PDF/A-1b) /DOCINFO pdfmark
   "


Example using DISTILLERPARAMS to set the quality of JPEG compression
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

.. code-block:: postscript

   ESC%-12345X @PJL DEFAULT SETDISTILLERPARAMS = "<< /ColorImageDict << /QFactor 0.7 /Blend 1 /HSamples [ 2 1 1 2 ] /VSamples [ 2 1 1 2 ] >> >>"




PDF file output - ``pdfwrite``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

``-dMaxInlineImageSize=integer``
   Specifies the maximum size of an inline image, in bytes. For images larger than this size, :title:`pdfwrite` will create an XObject instead of embedding the image into the context stream. The default value is 4000. Note that redundant inline images must be embedded each time they occur in the document, while multiple references can be made to a single XObject image. Therefore it may be advantageous to set a small or zero value if the source document is expected to contain multiple identical images, reducing the size of the generated PDF.

``-dDoNumCopies``
   When present, causes :title:`pdfwrite` to use the ``#copies`` or ``/NumCopies`` entry in the page device dictionary to duplicate each page in the output PDF file as many times as the 'copies' value. This is intended for use by workflow applications like :title:`cups` and should not be used for generating general purpose PDF files. In particular any ``pdfmark`` operations which rely on page numbers, such as ``Link`` or ``Outline`` annotations will not work correctly with this flag.

``-dDetectDuplicateImages``
   Takes a Boolean argument, when set to true (the default) :title:`pdfwrite` will compare all new images with all the images encountered to date (NOT small images which are stored in-line) to see if the new image is a duplicate of an earlier one. If it is a duplicate then instead of writing a new image into the PDF file, the PDF will reuse the reference to the earlier image. This can considerably reduce the size of the output PDF file, but increases the time taken to process the file. This time grows exponentially as more images are added, and on large input files with numerous images can be prohibitively slow. Setting this to false will improve performance at the cost of final file size.

``-dFastWebView``
   Takes a Boolean argument, default is false. When set to true :title:`pdfwrite` will reorder the output PDF file to conform to the Adobe 'linearised' PDF specification. The Acrobat user interface refers to this as 'Optimised for Fast Web Viewing'. Note that this will cause the conversion to PDF to be slightly slower and will usually result in a slightly larger PDF file.
   This option is incompatible with producing an encrypted (password protected) PDF file.

``-dPreserveAnnots=boolean``
   We now attempt to preserve most annotations from input PDF files as annotations in the output PDF file (note, not in output PostScript!). There are a few annotation types which are not preserved, most notably ``Link`` and ``Widget`` annotations. However, should you wish to revert to the old behaviour, or find that the new behaviour leads to problems, you can set this switch to false which will cause all annotations to be inserted into the page content stream, instead of preserved as annotations.

   In addition, finer control is available by defining an array ``/PreserveAnnotTypes``. Annotation types listed in this array will be preserved, whilst those not listed will be drawn according to the setting of ``ShowAnnots`` and ``ShowAnnotTypes``. By using the controls ``PreserveAnnots``, ``PreserveAnnotTypes``, ``ShowAnnots`` and ``ShowAnnotTypes`` it is possible to select by annotation type whether annotations are preserved as annotations, drawn into the page, or simply dropped.

   To use this feature: ``-c "/PreserveAnnotTypes [....] def" -f <input file>``

   Where the array can contain one or more of the following names: ``/Stamp``, ``/Squiggly``, ``/Underline``, ``/Link``, ``/Text``, ``/Highlight``, ``/Ink``, ``/FreeText``, ``/StrikeOut`` and ``/stamp_dict``.

   For example, adding the follow to the command line: ``-c "/PreserveAnnotTypes [/Text /UnderLine] def" -f <input file>`` would preserve only annotations with the subtypes "Text" and "UnderLine".

``-dPreserveMarkedContent=boolean``
   We now attempt to preserve marked content from input PDF files through to the output PDF file (note, not in output PostScript!). This does now include marked content relating to optional content.

   This control also requires the PDF interpreter to pass the marked content to the :title:`pdfwrite` device, this is only done with the new (C-based) PDF interpreter. The old (PostScript-based) interpreter does not support this feature and will not pass marked content to the :title:`pdfwrite` device.

``-dOmitInfoDate=boolean``
   Under some conditions the ``CreationDate`` and ``ModDate`` in the ``/Info`` dictionary are optional and can be omitted. They are required when producing PDF/X output however. This control will allow the user to omit the ``/CreationDate`` and ``/ModDate`` entries in the ``Info`` dictionary (and the corresponding information in the XMP metadata, if present). If you try to set this control when writing PDF/X output, the device will give a warning and ignore this control.

``-dOmitID=boolean``
   Under some conditions the ``/ID`` array trailer dictionary is optional and can be omitted. It is required when producing PDF 2.0, or encrypted PDFs however. This control will allow the user to omit the ``/ID`` entry in the trailer dictionary. If you try to set this control when writing PDF 2.0 or encrypted PDF output, the device will give a warning and ignore this control.

``-dOmitXMP=boolean``
   Under some conditions the XMP ``/Metadata`` entry in the ``Catalog`` dictionary is optional and can be omitted. It is required when producing PDF/A output however. This control will allow the user to omit the ``/Metadata`` entry in the ``Catalog`` dictionary. If you try to set this control when writing PDF/A output, the device will give a warning and ignore this control.

``-dNO_PDFMARK_OUTLINES``
  When the input is a PDF file which has an ``/Outlines`` tree (called "Bookmarks" in Adobe Acrobat) these are normally turned into ``pdfmarks`` and sent to the ``pdfwrite`` device so that they are preserved in the output PDF file. However, if this control is set then the interpreter will ignore the Outlines in the input.


The following options are useful for creating PDF 1.2 files:

``-dPatternImagemask=boolean``
   With CompatibilityLevel < 1.3 it specifies whether the target viewer handles ``ImageMask`` with a pattern color. Some old viewers, such as Ghostscript 3.30 fail with such constructs. Setting this option to false, one can get more compatibility, but the mask interpolation is lost. With CompatibilityLevel ≥ 1.3 this option is ignored. Default value is false.

``-dMaxClipPathSize=integer``
   Specifies the maximum number of elements in the clipping path that the target viewer can handle. This option is used only with ``CompatibilityLevel < 1.3`` and ``PatternImagemask=false``, and only when converting a mask into a clipping path. If the clipping path exceeds the specified size, the masked image and the clipping path is decomposed into smaller images. The value of the option counts straight path segments (curved segments are not used for representing a mask). Default value is 12000.

``-dMaxShadingBitmapSize=integer``
   Specifies the maximum number of bytes allowed for representing a shading as a bitmap. If a shading exceeds this value, the resolution of the output bitmap is reduced to fit into the specified number of bytes. Note that the number of bytes depends on the number of color components in ``ProcessColorModel`` or ``ColorConversionStrategy``, assumes 8 bits per sample, and doesn't consider image compression or downsampling. The image is rendered at the current resolution as specified by ``-r`` or the default of 720 dpi. Default value is 256000. In general larger values will result in higher quality, but the output file size may increase dramatically, particularly with shadings which cover large areas. Shadings should generally only be rendered to images if ``CompatibilityLevel`` is 1.2 or less or if ``ColorCoversionStrategy`` specifies a color space different to that of the shading.

``-dHaveTrueTypes=boolean``
   With ``CompatibilityLevel < 1.3`` it specifies whether the target viewer can handle TrueType fonts. If not, TrueType fonts are converted into raster fonts with resolution specified in ``HWResolution``. Note that large text at higher resolutions results in very large bitmaps which are likely to defeat caching in many printers. As a result the text is emitted as simple images rather than as a (type 3) bitmap font. The PostScript user parameter ``MaxFontItem`` can be used to increase the maximum size of a cache entry which will increase the size/resolution of the text which can be stored in a font. With ``CompatibilityLevel ≥ 1.3`` this option is ignored. Default value is true.

The following options are useful for creating PDF 1.3 files:

``-dHaveTransparency=boolean``
   With ``CompatibilityLevel ≥ 1.4`` it specifies whether the target viewer can handle PDF 1.4 transparency objects. If not, the page is converted into a single plain image with all transparency flattened. Default value is true.

The following option specifies creation of a PDF/X-3 file:

``-dPDFX=boolean``
   Specifies the generated document is to follow the PDF/X-3 standard. When true, a DefaultRGB ColorSpace resource must be defined, and options ``NOSUBSTDEVICECOLORS``, ``NOCIE`` must not be specified. Default value is false.

   The :title:`pdfwrite` device does not currently support PDF/X versions other than 3.


When generating a PDF/X-3 document, Ghostscript performs the following special actions to satisfy the PDF/X-3 standard:

- All fonts are embedded.

- ``DeviceRGB`` color space is substituted with the ``DefaultRGB`` color space, which must be defined in the ``ColorSpace`` category. The easiest way is to provide it in the ``DefaultRGB`` file in the resource directory.

- ``DeviceRGB`` color values are passed unchanged. If a user needs a non trivial color adjustment, a non trivial ``DefaultRGB`` color space must be defined.

- Transfer functions and halftone phases are skipped.

- ``/PS pdfmark`` interprets the DataSource stream or file.

- ``TrimBox`` and ``BleedBox`` entries are generated in page descriptions. Their values can be changed using the ``PDFXTrimBoxToMediaBoxOffset``, ``PDFXSetBleedBoxToMediaBox``, and ``PDFXBleedBoxToTrimBoxOffset`` distiller parameters (see below).


The following switches are used for creating encrypted documents:

``-sOwnerPassword=string``
   Defines that the document be encrypted with the specified owner password.

``-sUserPassword=string``
   Defines the user password for opening the document. If empty, the document can be opened with no password, but the owner password is required to edit it.

``-dPermissions=number``
   Defines the PDF permissions flag field. Negative values are allowed to represent unsigned integers with the highest bit set. See the PDF Reference manual for the meaning of the flag bits.

``-dEncryptionR=number``
   Defines the encryption method revision number - either 2 or 3.

``-dKeyLength=number``
   Defines the length (in bits) of the encryption key. Must be a multiple of 8 in the interval [40, 128]. If the length isn't 40, ``-dEncryptionR`` must be 3.


The following switches are used for generating metadata according to the Adobe XMP specification :


``-sDocumentUUID=string``
   Defines a ``DocumentID`` to be included into the document Metadata. If not specified, Ghostscript generates an UUID automatically. Otherwise the specified string is copied into the document without checking its syntax or consistence.

   Note that Adobe XMP specification requires ``DocumentID`` must be same for all versions of a document. Since Ghostscript does not provide a maintenance of document versions, users are responsible to provide a correct UUID through this parameter.

   Note that Ghostscript has no access to the host node ID due to a minimization of platform dependent modules. Therefore it uses an MD5 hash of the document contents for generating UUIDs.

``-sInstanceUUID=string``
   Defines a instance ID to be included into the document ``Metadata``. If not specified, Ghostscript generates an UUID automatically. Otherwise the specified string is copied into the document without checking its syntax or consistence.

   Note that the Adobe XMP specification requires the instance ID to be unique for all versions of the document. This parameter may be used to disable unique ID generation for debug purposes.

   When none of ``DocumentUUID`` and ``InstanceUUID`` are specified, the generated ``DocumentID`` appears same as instance ID.

``-sDocumentTimeSeq=integer``
   Defines an integer to be used as a deconflictor for generating UUIDs, when several invocations of Ghostscript create several PDF documents within same clock quantum (tick). Mainly reserved for very fast computers and/or multithreading applications, which may appear in future. If both ``DocumentUUID`` and ``InstanceUUID`` are specified, ``DocumentTimeSeq`` is ignored.


``-sDSCEncoding=string``
   Defines the name of a Postscript encoding in which DSC comments in the source document are encoded. If specified, the comments are converted from that encoding into Unicode UTF-8 when writing ``Metadata``. If not specified, the comments are copied to ``Metadata`` with no conversion. Note that Adobe Distiller for Windows uses the default locale's code page for this translation, so it's result may differ from Ghostscript. Adobe Acrobat appears to use ``PDFDocEncoding`` when displaying document's properties, so we recommend this value.

``-sUseOCR=string``
   Controls the use of OCR in :title:`pdfwrite`. If enabled this will use an OCR engine to analyse the glyph bitmaps used to draw text in a PDF file, and the resulting Unicode code points are then used to construct a ``ToUnicode CMap``.

   PDF files containing ``ToUnicode CMaps`` can be searched, use copy/paste and extract the text, subject to the accuracy of the ``ToUnicode CMap``. Since not all PDF files contain these it can be beneficial to create them.

   Note that, for English text, it is possible that the existing standard character encoding (which most PDF consumers will fall back to in the absence of Unicode information) is better than using OCR, as OCR is not a 100% reliable process. OCR processing is also comparatively slow.

   For the reasons above it is useful to be able to exercise some control over the action of :title:`pdfwrite` when OCR processing is available, and the UseOCR parameter provides that control. There are three possible values:

   - Never Default - don't use OCR at all even if support is built-in.

   - AsNeeded If there is no existing ToUnicode information, use OCR.

   - Always Ignore any existing information and always use OCR.

   Our experimentation with the Tesseract OCR engine has shown that the more text we can supply for the engine to look at, the better the result we get. We are, unfortunately, limited to the graphics library operations for text as follows.

   The code works on text 'fragments'; these are the text sequences sent to the text operators of the source language. Generally most input languages will try to send text in its simplest form, eg "Hello", but the requirements of justification, kerning and so on mean that sometimes each character is positioned independently on the page.

   So :title:`pdfwrite` renders all the bitmaps for every character in the text document, when set up to use OCR. Later, if any character in the font does not have a Unicode value already we use the bitmaps to assemble a 'strip' of text which we then send to the OCR engine. If the engine returns a different number of recognised characters than we expected then we ignore that result. We've found that (for English text) constructions such as ". The" tend to ignore the full stop, presumably because the OCR engine thinks that it is simply noise. In contrast "text." does identify the full stop correctly. So by ignoring the failed result we can potentially get a better result later in the document.

   Obviously this is all heuristic and undoubtedly there is more we can do to improve the functionality here, but we need concrete examples to work from.

``-dWriteXRefStm=boolean``
   Controls whether the pdfwrite device will use an XRef stream in the output file instead of a regular xref table. This switch defaults to true, however if the output file is less than PDF 1.5 then XRef streams are not supported, and it will be set to false.
   
   Using an XRef stream instead of a regular xref can reduce the size of the output file, and is required in order to use ObjStms (see below) which can reduce the file size even further. This is currently a new feature and can be disabled if problems arise.

``-dWriteObjStms=boolean``
   Controls whether the pdfwrite device will use ObjStms to store non-stream objects in the output file. This switch defaults to true, however if the output file is less than PDF 1.5, or XRef streams are disabled then ObjStms are not supported and this switch will be set to false. Additionally, the pdfwrite device does not currently support ObjStms when Linearizing (Optimize for Fast Web View) the output file and ObjStms will be disabled if Linearization is activated.
   
   Using ObjStms can significantly reduce the size of soem PDF files, at the cost of somewhat reduced performance. Taking as an exmple the PDF 1.7 Reference Manual; the original file is ~32MB, producing a PDF file from it using pdfwrite without the XRefStm or ObjStms enabled produces a file ~19MB, with both these features the output file is ~13.9MB. This is currently a new feature and can be disabled if problems arise.

----


:title:`pdfwrite` for the new interpreter
"""""""""""""""""""""""""""""""""""""""""""""""

Using the new interpreter (the default from version 10.0.0 of :title:`Ghostscript`) these can be set from the command line using ``-d`` or from :title:`PostScript` using the :ref:`setpagedevice<Language_DeviceParameters>` operator:


``-dModifiesPageSize=boolean``
  When this device parameter is *true* the new :title:`PDF` interpreter will not pass the various Box values (:title:`ArtBox`, :title:`BleedBox`, :title:`CropBox`, :title:`TrimBox`) to the output device. This is used when scripting the interpreter and using a media size other than the :title:`MediaBox` in the :title:`PDF` file. If the media size has changed and we preserve the :title:`Boxes` in the output they would be incorrect.

``-dModifiesPageOrder=boolean``
  When this device parameter is *true* the new :title:`PDF` interpreter will not pass :title:`Outlines` (what :title:`Acrobat` calls :title:`Bookmarks`) and :title:`Dests` (the destination of document-local hyperlinks for example). This is because these rely on specifying the page by number, and if we alter the page order the destination would be incorrect. Again this is intended for use when scripting the interpreter.


.. note::

  The :title:`Nup` device already sets these parameters internally (and they cannot be changed in that device) as it modifies both the media size and the order of pages, the ``pdfwrite`` device now supports both these parameters and they can be altered as required.

PostScript file output
~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The :title:`ps2write` device handles the same set of distiller parameters as are handled by the :title:`pdfwrite` device (and 2 unique extensions, see below).

The option ``-dMaxInlineImageSize=integer`` must not be used with :title:`ps2write` as all images are inline in PostScript.

There are also two additional (not Adobe-standard) Distiller parameters, specific to :title:`ps2write`:

``/PSDocOptions string``
   No default value. If defined, the contents of the string will be emitted in the output PostScript prolog enclosed within ``%%BeginSetup`` and ``%%EndSetup`` comments. This is intended as a means of introducing device-specific document wide setup or configuration options into the output. Default media selection, printer resolution etc might be included here.

``/PSPageOptions array of strings``
   No default value. If defined, the contents of the strings in the array will be emitted in the output PostScript at the start of each page, one string per page, enclosed within ``%%BeginPageSetup`` and ``%%EndPageSetup`` comments. This is intended as a means of introducing device-specific setup or configuration options into the output on a page by page basis. The strings are used from the array sequentially, if there are more pages than strings then we 'wrap round' and start again with the first string. This makes it convenient to do setup for even/odd pages by simply including 2 strings in the array.

.. note::

   Executing ``setpagedevice`` will reset distiller parameters to the default, if you use any of these options via ``setdistillerparams``, and expect to execute ``setpagedevice``, you should set ``/LockDistillerParams`` true. Ordinarily the PDF interpreter executes ``setpagedevice`` for every page in order to set the media size.

.. note::

   The strings contained in ``PSDocOptions``, and the ``PSPageOptions`` array, are written verbatim to the output. No error checking is (or can be) performed on these strings and it is the users responsibility to ensure they contain well formed PostScript which does not cause errors on the target device.


There are also the following ps2write specific options :

``-dProduceDSC=boolean``
   Default value is true. When this value is true the output PostScript file will be constructed in a way which is compatible with the Adobe Document Structuring Convention, and will include a set of comments appropriate for use by document managers. This enables features such as page extraction, N-up printing and so on to be performed. When set to false, the output file will not be DSC-compliant. Older versions of Ghostscript cannot produce DSC-compliant output from ps2write, and the behaviour for these older versions matches the case when ``ProduceDSC`` is false.

``-dCompressEntireFile=boolean``
   When this parameter is true, the ``LZWEncode`` and ``ASCII85Encode`` filters will be applied to the entire output file. In this case ``CompressPages`` should be false to prevent a dual compression. When this parameter is false, these filters will be applied to the initial procset only, if ``CompressPages`` is true. Default value is false.

.. note::

   It is not possible to set ``CompressEntireFile`` when ``ProduceDSC`` is true as a single compressed object cannot conform to the DSC. It is possible to set ``CompressPages`` which will also compress the ``ps2write ProcSet``.


Controlling device specific behaviour
""""""""""""""""""""""""""""""""""""""""""

A few options can be used to influence the behavior of a printer or PostScript interpreter that reads the result of ``ps2ps2``. All of these options are incompatible with DSC-compliant PostScript, in order to use any of them ``ProduceDSC`` must be set to false.

``-dRotatePages=boolean``
   The printer will rotate pages for a better fit with the physical size. Default value : false. Must be false if ``-dSetPageSize=true``.

``-dFitPages=boolean``
   The printer will scale pages down to better fit the physical page size. The rendering quality may be poor due to the scaling, especially for fonts which Ghostscript had converted into bitmaps (see the :title:`ps2write` device parameter ``HaveTrueTypes``; See options about the ``PageSize`` entry of the ``Policies`` dictionary while the conversion step). Default value : false. Must be false if ``-dSetPageSize=true`` or ``-dCenterPages=true``.

``-dCenterPages=boolean``
   The printer will center the page image on the selected media. Compatible with ``-dRotatePages=true``, which may rotate the image on the media if it fits better, and then center it. Default value : false. Must be false if ``-dSetPageSize=true`` or ``-dFitPages=true``.

``-dSetPageSize=boolean``
   The printer will try to set page size from the job. Only use with printers which can handle random ``PageSize``. Defaults to true, must be false if ``-dRotatePages=true``, ``-dCenterPages=true`` or ``-dFitPages=true``.

``-dDoNumCopies=boolean``
   The PostScript emitted by :title:`ps2write` will try to use ``copypage`` to create the number of copies originally requested. Note that this relies on the level 2 semantics for ``copypage`` and will not reliably work on language level 3 devices (such as Ghostscript itself). Defaults to false.

   This flag is not compatible with the ``ProduceDSC`` flag which will take precedence if set.


These correspond to keys in the Postscript userdict of the target printer's virtual memory to control its behavior while executing a job generated with :title:`ps2write`.

These keys can be set when executing using the :title:`ps2write` device, this 'fixes' the resulting behaviour according to which key has been set. If these keys are not defined during conversion, the resulting PostScript will not attempt any form of media selection. In this case the behaviour can then be modified by setting the keys, either by modifying the resulting PostScript or setting the values in some other manner on the target device.

See also the distiller params ``PSDocOptions`` and ``PSPageOptions`` mentioned :ref:`above<Distiller Parameters>`.



Encapsulated PostScript (EPS) file output
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The :title:`eps2write` device is the same as the :title:`ps2write` device, except that it produces Encapsulated PostScript, which is intended to be imported into another document and treated as a 'black box'. There are certain restrictions which EPS files must follow, the primary one being that they must be DSC conformant. This means that you must not set ``-dProduceDSC`` to false.

In addition EPS files may only contain a single page and may not contain device-specific PostScript. You should therefore not use the ``PSDocOptions`` or ``PSPageOptions`` or any of the switches noted in the :title:`ps2write` section above under `Controlling device specific behaviour`_.




Creating a PDF/X-3 document
----------------------------------

To create a PDF/X-3 document from a Postscript or a PDF file, you should :

- Specify the :title:`pdfwrite` device or use the ``ps2pdf`` script.

- Specify the ``-dPDFX`` option. It provides the document conformity and forces ``-dCompatibilityLevel=1.3``.

- Specify ``-sColorConversionStrategy=Gray``, ``-sColorConversionStrategy=CMYK`` or ``-sColorConversionStrategy=UseDeviceIndependentColor`` (RGB is not allowed).

- Specify a PDF/X definition file before running the input document. It provides additional information to be included into the output document. A sample PDF/X definition file may be found in ``gs/lib/PDFX_def.ps``. You will need to modify the content of this file; in particular you must alter the ``/ICCProfile`` so that it points to a valid ICC profile for your ``OutputCondition``. The string '(...)' defining the ICCProfile must be a fully qualified device and path specification appropriate for your Operating System.

- If a registered printing condition is applicable, specify its identifier in the PDF/X definition file. Otherwise provide an ICC profile and specify it in the PDF/X definition file as explained below.

- Provide a ``DefaultRGB`` resource file in the ``ColorSpace`` resource category. Either define it in the PDF/X definition file, or provide a definition of ``gs/Resource/ColorSpace/DefaultRGB``. ``gs/Resource/ColorSpace/DefaultRGB`` is usually distributed with Ghostscript, its content may not necessarily satisfy your needs, see below.


.. note::

   Unless ``-dNOSAFER`` is specified (NOT reccomended!) the ICC profile will be read using the ``SAFER`` file permissions; you must ensure that the profile is in a directory which is readable according to the ``SAFER`` permissions, or that the file itself is specifically made readable. See :ref:`-dSAFER<Use Safer>` for details of how to set file permissions for ``SAFER``.

As mentioned above, the PDF/X definition file provides special information, which the PDF/X-3 standard requires. You can find a sample file in ``gs/lib/PDFX_def.ps``, and edit it according to your needs. The file follows Postscript syntax and uses the operator ``pdfmark`` to pass the special information. To ease customisation the lines likely to need editing in the sample file are marked with the comment ``% Customize``. They are explained below.


``OutputCondition string``
   Defines an ``OutputCondition`` value for the output intent dictionary.

``OutputConditionIdentifier string``
   Defines an ``OutputConditionIdentifier`` value for the output intent dictionary.

``ICCProfile string``
   May be omitted if ``OutputConditionIdentifier`` specifies a registered identifier of characterized printing condition (see `IPA_2003-11_PDFX.pdf`_). Defines a file name of an ICC profile file to be included into the output document. You may specify either an absolute file name, or a relative path from the working directory.

``Title string``
   Defines the document title. Only useful if the source Postscript file doesn't define a title with DSC comments. Otherwise remove entire line from definition file.

``Info string``
   Defines an ``Info`` value for the output intent dictionary.



The Ghostscript distribution does not contain an ICC profile to be used for creating a PDF/X-3 document. Users should either create an appropriate one themselves, or use one from a public domain, or create one with the PDF/X-3 inspector freeware.

The PDF/X-3 standard requires a ``TrimBox`` entry to be written for all page descriptions. This is an array of four offsets that specify how the page is to be trimmed after it has been printed. It is set to the same as ``MediaBox`` by default unless the ``PDFXTrimBoxToMediaBoxOffset`` distiller parameter is present. It accepts offsets to the ``MediaBox`` as an array [left right top bottom], e.g., the PostScript input code ``<< /PDFXTrimBoxToMediaBoxOffset [10 20 30 40] >> setdistillerparams`` specifies that 10 points will be trimmed at the left, 20 points at the right, 30 points at the top, and 40 points at the bottom.

Another page entry is the ``BleedBox``. It gives the area of the page to which actual output items may extend; cut marks, color bars etc. must be positioned in the area between the ``BleedBox`` and the ``MediaBox``. The ``TrimBox`` is always contained within the ``BleedBox``. By default, the ``PDFXSetBleedBoxToMediaBox`` distiller parameter is true, and the ``BleedBox`` is set to the same values as the ``MediaBox``. If it is set to false, the ``PDFXBleedBoxToTrimBoxOffset`` parameter gives offset to the ``TrimBox``. It accepts a four-value array in the same format as the ``PDFXTrimBoxToMediaBoxOffset`` parameter.


Here is a sample command line to invoke Ghostscript for generating a PDF/X-3 document :


.. code-block:: bash

   gs -dPDFX -dBATCH -dNOPAUSE -sColorConversionStrategy=CMYK -sDEVICE=pdfwrite -sOutputFile=out-x3.pdf PDFX_def.ps input.ps


Please also see the ``PDFACompatibilityPolicy`` control described under `Creating a PDF/A document`_. The same control is now used to specify the desired behaviour when an input file cannot be converted 'as is' into a PDF/X file.



Creating a PDF/A document
------------------------------

To create a PDF/A document, please follow the instructions for `Creating a PDF/X-3 document`_, with the following exceptions :

- Specify the :title:`pdfwrite` device or use the ``ps2pdf`` script.

- Specify the ``-dPDFA`` option to specify ``PDF/A-1``, ``-dPDFA=2`` for ``PDF/A-2`` or ``-dPDFA=3`` for ``PDF/A-3``.

- Specify ``-sColorConversionStrategy=RGB``, ``-sColorConversionStrategy=CMYK`` or ``-sColorConversionStrategy=UseDeviceIndependentColor``.

- Specify a PDF/A definition file before running the input document. It provides additional information to be included in the output document. A sample PDF/A definition file may be found in ``gs/lib/PDFA_def.ps``. You will need to modify the content of this file; in particular you must alter the ``/ICCProfile`` so that it points to a valid ICC profile for your ``OutputIntent``. The string '(...)' defining the ICCProfile must be a fully qualified device and path specification appropriate for your Operating System.


There is one additional control for PDF/A output:


``PDFACompatibilityPolicy integer``
   When an operation (e.g. ``pdfmark``) is encountered which cannot be emitted in a PDF/A compliant file, this policy is consulted, there are currently three possible values:

   ``0`` - (default) Include the feature or operation in the output file, the file will not be PDF/A compliant. Because the document ``Catalog`` is emitted before this is encountered, the file will still contain PDF/A metadata but will not be compliant. A warning will be emitted in this case.

   ``1`` - The feature or operation is ignored, the resulting PDF file will be PDF/A compliant. A warning will be emitted for every elided feature.

   ``2`` - Processing of the file is aborted with an error, the exact error may vary depending on the nature of the PDF/A incompatibility.


Here is a sample command line to invoke Ghostscript for generating a PDF/A document:

.. code-block:: bash

   gs -dPDFA=1 -dBATCH -dNOPAUSE -sColorConversionStrategy=RGB -sDEVICE=pdfwrite -sOutputFile=out-a.pdf PDFA_def.ps input.ps



PDF Optimization and Compression
------------------------------------------------------------

The are techniques to compress & optmize **PDF** files which are explained in depth on our `Optimizing PDFs with Ghostscript blog <https://artifex.com/blog/optimizing-pdfs-with-ghostscript>`_.






Ghostscript PDF Printer Description
-----------------------------------------

To assist with creating a PostScript file suitable for conversion to PDF, Ghostscript includes ``ghostpdf.ppd``, a PostScript Printer Description (PPD) file. This allows some `distiller parameters`_ to be set when a PostScript file is generated.

Windows XP or 2000
~~~~~~~~~~~~~~~~~~~~~~~~~

To install a "Ghostscript PDF" printer on Windows XP, select the Windows Control Panel, Printers and Faxes, Add a Printer, Local Printer, Use port FILE: (Print to File), Have Disk..., select the directory containing ghostpdf.ppd and ghostpdf.inf, select "Ghostscript PDF", Replace existing driver (if asked), and answer the remaining questions appropriately. After installing, open the "Ghostscript PDF" properties, select the Device Settings tab, set "Minimum Font Size to Download as Outline" to 0 pixels.

To set distiller parameters, select the "Ghostscript PDF" Printing Preferences, then the Advanced button. The PDF settings are under "Printer Features".



``pdfmark`` extensions
-----------------------------------------

In order to better support the `ZUGFeRD electronic invoice standard`_ and potentially other standards in the future, a new non-standard ``pdfmark`` has been defined for use by pdfwrite. Find out more on our `creating ZUGFeRD documents with Ghostscript blog <https://artifex.com/blog/creating-zugferd-documents-with-ghostscript>`_.

This ``pdfmark`` allows additional ``Metadata`` to be defined which will be inserted into the ``Metadata`` generated by the :title:`pdfwrite` device. This is necessary because the standard requires a PDF/A-3 file be produced, with an extension schema (and some additional XML data) contained within the ``Metadata`` referenced from the ``Catalog`` object.

While it would be possible to use the existing ``Metadata`` ``pdfmark`` to write a completely new set of metadata into the ``Catalog``, creating a conformant set of XML, with all the information synchronised with the ``/Info`` dictionary would be challenging, this ``pdfmark`` allows the :title:`pdfwrite` device to generate all the normal information leaving the user with only the task of specifying the additional data. ``[ /XML (string containing additional XMP data) /Ext_Metadata pdfmark``


Limitations
--------------------

The :title:`pdfwrite` family will sometimes convert input constructs to lower-level ones, even if a higher-level construct is available. For example, if the PostScript file uses ``charpath`` to set a clipping path consisting of text, :title:`pdfwrite` may write the clipping path as a path in the PDF file, rather than as text, even though PDF is able to express clipping with text. This is only a performance issue, and will be improved incrementally over time.

Some applications, such as HIGZ, produce PostScript files that use ridiculously large coordinates. On such files, pdfwrite may cause a limitcheck error. If this occurs, try reducing the default internal resolution of 720 dpi by using the ``-r`` switch, e.g., ``-r300 somefile.ps``.

:title:`pdfwrite` ignores the PDF 1.3 (Acrobat 4.x) ``pdfmarks`` related to document content structure: ``StRoleMap``, ``StClassMap``, ``StPNE``, ``StBookmarkRoot``, ``StPush``, ``StPop``, ``StPopAll``, ``StBMC``, ``StBDC``, ``EMC``, ``StOBJ``, ``StAttr``, ``StStore``, ``StRetrieve``, ``NamespacePush``, ``NamespacePop``, and ``NI``. While this causes some structural information to be omitted from the output file, the displayed and printed output are normally not affected.



.. External links

.. _MuPDF: https://mupdf.com
.. _IPA_2003-11_PDFX.pdf: http://www.color.org/IPA_2003-11_PDFX.pdf
.. _ZUGFeRD electronic invoice standard: http://www.ferd-net.de/front_content.php?idcat=231&changelang=4



.. include:: footer.rst

