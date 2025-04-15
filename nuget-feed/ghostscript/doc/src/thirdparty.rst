.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: Third Party Libraries Used by Ghostscript and GhostPDL


.. include:: header.rst

.. _thirdparty.html:


Third Party Libraries
=========================================================


Third Party Libraries used by Ghostscript and GhostPDL
---------------------------------------------------------

The table below details the third party libraries that Ghostscript and/or GhostPDL include, the versions QA tested and shipped with our releases, the relevant license, and the "upstream" URL.


.. list-table::
   :header-rows: 1
   :widths: 20 10 25 25 20

   * - Library Name
     - Version
     - Function
     - License
     - URL
   * - CUPS

       (AGPL Release Only)

     - 2.8.0
     - CUPS raster format output
     - GPL Version 3
     - http://www.cups.org/
   * - eXpat
     - 2.5.0
     - XML parsing for XPS interpreter
     - MIT/eXpat License
     - http://expat.sourceforge.net/
   * - FreeType
     - 2.13.0
     - Font scaling and rendering

       for Ghostscript
     - FreeType License

       (BSD-style license

       with a credit clause)

     - http://www.freetype.org/
   * - jbig2dec
     - 0.19
     - JBIG2 decoding for the

       PDF interpreter
     - Licensed with Ghostscript/GhostPDL

       (copyright owned by Artifex)

     - http://www.ghostscript.com/
   * - libjpeg
     - 9e with patches
     - JPEG/DCT decoding/encoding
     - "Free" Can be used in

       commercial applications

       without royalty,

       with acknowledgement.
     - http://www.ijg.org/
   * - LittleCMS2 mt

       (lcms2mt - thread

       safe fork of lcms2)

     - 2.10mt
     - ICC profile based color conversion

       and management

     - MIT LICENSE
     - http://www.ghostscript.com/
   * - libpng
     - 1.6.39
     - PNG image encoding/decoding
     - libpng license - classified as

       "a permissive free software license"
     - http://www.libpng.org/
   * - OpenJPEG
     - 2.5.0
     - JPEG2000 image decoding for the

       PDF interpreter

     - BSD-style
     - http://www.openjpeg.org/
   * - zlib
     - 1.2.13
     - (De)Flate compression
     - zlib license - classified as

       "a permissive free software license"
     - http://www.zlib.net/
   * - libtiff
     - 4.5.0
     - TIFF image encoding/decoding
     - BSD-style
     - http://www.remotesensing.org/libtiff


The following are optional.



.. list-table::
   :header-rows: 1
   :widths: 20 10 25 25 20

   * - Library Name
     - Version
     - Function
     - License
     - URL
   * - tesseract
     - 4.1.0
       with patches
     - Optical Character Recognition (OCR) library
     - Apache License 2.0
     - https://github.com/tesseract-ocr/tesseract
   * - leptonica
     - 1.80.0 with patches
     - Image processing toolkit - support

       library for tesseract OCR

     - Leptonica License

       "a permissive free software license"

     - http://www.leptonica.org/




The following are no-cost, open source licensed, but not GPL compatible.



.. list-table::
   :header-rows: 1
   :widths: 20 10 25 25 20

   * - Library Name
     - Version
     - Function
     - License
     - URL
   * - jpegxr (JPEG XR reference software)
     - 1.8
     - HDPhoto/JPEG-XR image

       decoding for the XPS interpreter
     - ITU/ISO/IEC "Free" License

       Reference implementation
     - http://www.itu.int/rec/T-REC-T.835




The following is optional & relevant to Artifex Software commercial releases only.


.. list-table::
   :header-rows: 1
   :widths: 20 10 25 25 20

   * - Library Name
     - Version
     - Function
     - License
     - URL
   * - Monotype UFST
     - 5.x/6.x with patches
     - Access, scale and render

       Monotype MicroType fonts
     - Commercial
     - https://www.monotype.com/


















.. include:: footer.rst
