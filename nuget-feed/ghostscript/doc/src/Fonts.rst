.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: Fonts and Font Facilities Supplied with Ghostscript

.. include:: header.rst

.. _Fonts.html:


Fonts and Font Facilities Supplied with Ghostscript
======================================================




About Ghostscript fonts
---------------------------

Ghostscript is distributed with two kinds of files related to fonts:

- The fonts themselves in individual files.

- A file "Fontmap" that defines for Ghostscript which file represents which font.

Additionally, the file ``cidfmap`` can be used to create substitutes for ``CIDFonts`` referenced by name in Postscript and PDF jobs. See the section on CID Font Substitution for details.

.. note::

  Care must be exercised since poor or incorrect output may result from inappropriate ``CIDFont`` substitution. We therefore strongly recommend embedding CIDFonts in your Postscript and PDF files if at all possible.

The "base 35" fonts required for Postscript (and "base 14" required for PDF) are Postscript Type 1 font files.

When Ghostscript needs a font, it must have some way to know where to look for it: that's the purpose of the ``Fontmap`` file, which associates the names of fonts such as ``/Times-Roman`` with the names of font files, such as ``n021003l.pfb``. ``Fontmap`` can also create aliases for font names, so that for instance, ``/NimbusNo9L-Regu`` means the same font as ``/Times-Roman``.

Where a mapping in ``Fontmap`` maps a font name to a ``path/file``, the directory containing the font file is automatically added to the ``permit file read`` list. For example:

.. code-block:: postscript

  /Arial (/usr/share/fonts/truetype/msttcorefonts/arial.ttf) ;

will result in the path ``/usr/share/fonts/truetype/msttcorefonts/`` being added to the permit file read list. This is done on the basis that font files are often grouped in common directories, and rather than risk the file permissions lists being swamped with (potentially) hundreds of individual files, it makes sense to add the directories.


.. note::

  ``Fontmap`` is processed (and the paths added to the file permissions list) during initialisation of the Postscript interpreter, so any attempt by a Postscript job to change the font map cannot influence the file permissions list.



Ghostscript's free fonts
------------------------------

35 commercial-quality Type 1 basic PostScript fonts -- Times, Helvetica, Courier, Symbol, etc. -- contributed by `URW++ Design and Development Incorporated`_, of Hamburg, Germany. ``Fontmap`` names them all.



How Ghostscript gets fonts when it runs
---------------------------------------------------

Fonts occupy about 50KB each, so Ghostscript doesn't load them all automatically when it runs. Instead, as part of normal initialization Ghostscript runs a file ``gs_fonts.ps``, which arranges to load fonts on demand using information from the font map. To preload all of the known fonts, invoke the procedure:


.. code-block:: postscript

  loadallfonts

The file ``lib/prfont.ps`` contains code to print a sample page of a font. Load this program by including it in the ``gs`` command line or by invoking:

.. code-block:: bash

  (prfont.ps) run

Then to produce a sampler of a particular font ``XYZ``, invoke:

.. code-block:: postscript

  /XYZ DoFont

For example,

.. code-block:: postscript

  /Times-Roman DoFont

For more information about how Ghostscript loads fonts during execution, see :ref:`Font lookup<Use_Font lookup>`.


.. _Fonts_Add:

Adding your own fonts
---------------------------------------------------

Ghostscript can use any Type 0, 1, 3, 4, or 42 font acceptable to other PostScript language interpreters or to ATM, including MultiMaster fonts. Ghostscript can also use TrueType font files.

To add fonts of your own, you must edit ``Fontmap`` to include at the end an entry for your new font; the format for entries is documented in ``Fontmap`` itself. Since later entries in ``Fontmap`` override earlier entries, a font you add at the end supersedes any corresponding fonts supplied with Ghostscript and defined earlier in the file. To ensure correct output, it is vital that entries for the "base 35" fonts remain intact in the ``Fontmap`` file.

In the PC world, Type 1 fonts are customarily given names ending in ``.PFA`` or ``.PFB``. Ghostscript can use these directly: you just need to make the entry in ``Fontmap``. If you want to use with Ghostscript a commercial Type 1 font (such as fonts obtained in conjunction with Adobe Type Manager), please read carefully the license that accompanies the font to satisfy yourself that you may do so legally; we take no responsibility for any possible violations of such licenses. The same applies to TrueType fonts.

Converting BDF fonts
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. note::

    This is deprecated.

Ghostscript provides a way to construct a (low-quality) Type 1 font from a bitmap font in the BDF format popular in the Unix world. The shell script bdftops (Unix) or the command file ``bdftops.bat`` (DOS) converts a BDF file to a scalable outline using ``bdftops.ps`` . Run the shell command:

.. code-block:: bash

  bdftops BDF_filename [AFM_file1_name ...] gsf_filename fontname
          UniqueID [XUID] [encodingname]


The arguments have these meanings:


.. list-table::
   :header-rows: 0

   * - ``BDF_filename``
     - Input bitmap file in BDF format
     -
   * - ``AFM_file1_name``
     - AFM files giving metrics
     - (Optional)
   * - ``gsf_filename``
     - Output file
     -
   * - ``fontname``
     - Name of the font
     -
   * - ``UniqueID``
     - UniqueID (:ref:`as described below<Fonts_UniqueIDs>`)
     -
   * - ``XUID``
     - XUID, in the form n1.n2.n3... (:ref:`as described below<Fonts_UniqueIDs>`)
     - (Optional)
   * - ``encodingname``
     - "StandardEncoding" (the default), "ISOLatin1Encoding", "SymbolEncoding", "DingbatsEncoding"
     - (Optional)

For instance:

.. code-block:: postscript

  bdftops pzdr.bdf ZapfDingbats.afm pzdr.gsf ZapfDingbats 4100000 1000000.1.41

Then make an entry in ``Fontmap`` for the ``.gsf`` file (``pzdr.gsf`` in the example) as :ref:`described above<Fonts_Add>`.



For developers only
---------------------

The rest of this document is very unlikely to be of value to ordinary users.

Contents of fonts
~~~~~~~~~~~~~~~~~~~~

As noted above, Ghostscript accepts fonts in the same formats as PostScript interpreters. Type 0, 1, and 3 fonts are documented in the PostScript Language Reference Manual (Second Edition); detailed documentation for Type 1 fonts appears in a separate Adobe book. Type 2 (compressed format) fonts are documented in separate Adobe publications. Type 4 fonts are not documented anywhere; they are essentially Type 1 fonts with a BuildChar or BuildGlyph procedure. Types 9, 10, and 11 (CIDFontType 0, 1, and 2) and Type 32 (downloaded bitmap) fonts are documented in Adobe supplements. Type 42 (encapsulated TrueType) fonts are documented in an Adobe supplement; the TrueType format is documented in publications available from Apple and Microsoft. Ghostscript does not support Type 14 (Chameleon) fonts, which use a proprietary Adobe format.



.. _Fonts_UniqueIDs:

Font names and unique IDs
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

If you create your own fonts and will use them only within your own organization, you should use ``UniqueID`` values between ``4000000`` and ``4999999``.

If you plan to distribute fonts, ask Adobe to assign you some ``UniqueIDs`` and also an ``XUID`` for your organization. Contact:

::

  Unique ID Coordinator
  Adobe Developers Association
  Adobe Systems, Inc.
  345 Park Avenue
  San Jose, CA 95110-2704
  +1-408-536-9000 telephone (ADA)
  +1-408-536-6883 fax
  fontdev-person@adobe.com


The XUID is a Level 2 PostScript feature that serves the same function as the ``UniqueID``, but is not limited to a single 24-bit integer. The ``bdftops`` program creates ``XUIDs`` of the form "``[-X- 0 -U-]``" where "``-X-``" is the organization ``XUID`` and "``-U-``" is the ``UniqueID``. (Aladdin Enterprises' organization XUID, which appears in a few places in various font-related files distributed with Ghostscript, is 107; do not use this for your own fonts that you distribute.)








.. Note this was originally on the Use.html page

.. _Fonts FAPI:




Running Ghostscript with third-party font renderers
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Font API (FAPI) is a feature which allows to attach third-party font renderers to Ghostscript. This section explains how to run Ghostscript with third-party font renderers, such as UFST.


.. note:: FreeType is now the default font renderer for Ghostscript.


.. note::

   To run Ghostscript with UFST you need a license from Monotype Imaging. Please ignore issues about UFST if you haven't got it.

.. important::

   Third-party font renderers may be incompatible with devices that can embed fonts in their output (such as pdfwrite), because such renderers may store fonts in a form from which Ghostscript cannot get the necessary information for embedding, for example, the Microtype fonts supplied with the UFST. Ghostscript can be configured to disable such renderers when such a device is being used.

As of Ghostscript version 9.0, Ghostscript uses Freetype 2.4.x as the default font scaler/renderer.

With this change, we added a new switch:``-dDisableFAPI=true`` to revert to the older behavior, just in case serious regression happens that cannot be resolved in a timely manner. It is intended that this switch will be removed once the FAPI/Freetype implementation has proven itself robust and reliable in the "real world".

With version 9.18 released we have, for some time, regarded FAPI/Freetype as being the canonical glyph rendering solution for Ghostscript and associated products, and the non-FAPI rendering to be deprecated. As such, the ``-dDisableFAPI=true`` option is also considered deprecated, and should be expected to be removed shortly after the next release.

To run Ghostscript with UFST, you first need to :ref:`build Ghostscript with the UFST bridge<Make_USFTBuild>`. Both bridges may run together.

There are 2 ways to handle fonts with a third-party font renderer (FAPI). First, you can substitute any FAPI-handled font to a resident PostScript font, using special map files ``FAPIfontmap`` and ``FAPIcidfmap``. Second, you can redirect PostScript fonts to FAPI, setting entries in ``FAPIconfig`` file.

Names ``FAPIfontmap``, ``FAPIcidfmap``, ``FAPIconfig`` in this text actually are placeholders, which may be substituted with command line arguments : ``-sFAPIfontmap=name1 -sFAPIcidfmap=name2 -sFAPIconfig=name3``. Ghostscript searches the specified file names as explained in How Ghostscript finds files. Default values for these arguments are equal to argument names. When building Ghostscript with ``COMPILE_INITS=1``, only default values are used.

Font files, which are being handled with FAPI, may reside in any directory in your hard disk. Paths to them to be specified in ``FAPIfontmap`` and with special command line arguments, explained below. The path may be either absolute or relative. Relative ones are being resolved from the path, which is specified in ``FAPIconfig`` file.

The file ``FAPIfontmap`` is actually special PostScript code. It may include records of 2 types : general records and FCO records (see below).

A general record describes a font, which is being rendered with FAPI. They must end with semicolon. Each general record is a pair. The first element of the pair is the font name (the name that PostScript documents use to access the font, which may differ from real name of the font which the font file defines). The second element is a dictionary with entries:



.. list-table::
   :widths: 20 20 60
   :header-rows: 1

   * - Key
     - Type
     - Description
   * - Path
     - string
     - Absolute path to font file, or relative path to font file from the FontPath value, being specified in FAPIconfig.
   * - FontType
     - integer
     - PostScript type for this font. Only 1 and 42 are currently allowed.

       Note that this is unrelated to the real type of the font file - the bridge will perform a format conversion.

   * - FAPI
     - name
     - Name of the renderer to be used with the font. Only /UFST and /FreeType are now allowed.
   * - SubfontId
     - integer
     - (optional) Index of the font in font collection, such as FCO or TTC.

       It is being ignored if Path doesn't specify a collection.

       Note that Free Type can't handle FCO. Default value is 0.

   * - Decoding
     - name
     - (optional) The name of a Decoding resource to be used with the font.

       If specified, lib/xlatmap (see below) doesn't work for this font.


Example of a general FAPI font map record :

.. code-block:: bash

   /FCO1 << /Path (/AFPL/UFST/fontdata/MTFONTS/PCLPS3/MT1/PCLP3__F.fco) /FontType 1 /FAPI /UFST >> ;


FCO records work for UFST only. A group of FCO records start with a line *name* ``ReadFCOfontmap:``, where *name* is a name of a command line argument, which specify a path to an FCO file. The group of FCO records must end with the line ``EndFCOfontmap``. Each record of a group occupy a single line, and contains a number and 1, 2 or 3 names. The number is the font index in the FCO file, the first name is the Postscript font name, the secong is an Encoding resource name, and the third is a decoding resource name.


.. note::

  ``FAPIfontmap`` specifies only instances of Font category. CID fonts to be listed in another map file.

Ghostscript distribution includes sample map files ``gs/lib/FAPIfontmap, gs/lib/FCOfontmap-PCLPS2, gs/lib/FCOfontmap-PCLPS3, gs/lib/FCOfontmap-PS3``, which may be customized by the user. The last 3 ones include an information about UFST FCO files.

The file ``FAPIcidfmap`` defines a mapping table for CIDFont resources. It contains records for each CID font being rendered with FAPI. The format is similar to ``FAPIfontmap``, but dictionaries must contain few different entries:



.. list-table::
   :widths: 20 20 60
   :header-rows: 1

   * - Key
     - Type
     - Description
   * - Path
     - string
     - Absolute path to font file, or relative path to font file from the ``CIDFontPath`` value, being specified in FAPIconfig.
   * - CIDFontType
     - integer
     - PostScript type for this CID font.

       Only 0, 1 and 2 are currently allowed.

       Note that this is unrelated to the real type of the font file - the bridge will perform format conversion.

   * - FAPI
     - name
     -  Name of the renderer to be used with the font.

        Only /UFST and /FreeType are now allowed.

   * - SubfontId
     - integer
     - (optional) Index of the font in font collection, such as FCO or TTC.

       It is being ignored if Path doesn't specify a collection.

       Default value is 0.

   * - CSI
     - array of 2 elements
     - (required) Information for building CIDSystemInfo.

       The first element is a string, which specifies ``Ordering``.

       The second element is a number, which specifies ``Supplement``.



Example of FAPI CID font map record:

.. code-block:: bash

   /HeiseiKakuGo-W5 << /Path (/WIN2000/Fonts/PMINGLIU.TTF) /CIDFontType 0 /FAPI /UFST /CSI [(Japan1) 2] >> ;

The control file FAPIconfig defines 4 entries:



.. list-table::
   :widths: 20 20 60
   :header-rows: 1

   * - Key
     - Type
     - Description

   * - FontPath
     - string
     - Absolute path to a directory, which contains fonts.

       Used to resolve relative paths in ``FAPIfontmap``.

   * - CIDFontPath
     - string
     - Absolute path to a directory, which contains fonts to substitute to CID fonts.

       Used to resolve relative paths in ``FAPIcidfmap``.

       It may be same or different than ``FontPath``.

   * - HookDiskFonts
     - array of integers
     - List of PS font types to be handled with FAPI.

       This controls other fonts that ones listed in ``FAPIfontmap`` and ``FAPIcidfmap`` -

       such ones are PS fonts installed to Ghostscript with ``lib/fontmap`` or

       with ``GS_FONTPATH``, or regular CID font resources.

       Unlisted font types will be rendered with the native Ghostscript font renderer.

       Only allowed values now are 1,9,11,42.

       Note that 9 and 11 correspond to ``CIDFontType`` 0 and 2.

   * - HookEmbeddedFonts
     - array of integers
     - List of PS font types to be handled with FAPI.

       This controls fonts being embedded into a document - either fonts or CID font resources.

       Unlisted font types will be rendered with the native Ghostscript font renderer.

       Only allowed values now are 1,9,11,42.

       Note that 9 and 11 correspond to ``CIDFontType`` 0 and 2.


Ghostscript distribution includes sample config files ``gs/lib/FAPIconfig``, ``gs/lib/FAPIconfig-FCO``. which may be customized by the user. The last ones defines the configuration for handling resident UFST fonts only.

In special cases you may need to customize the file ``lib/xlatmap``. Follow instructions in it.

Some UFST font collections need a path for finding an UFST plugin. If you run UFST with such font collection, you should run Ghostscript with a special command line argument ``-sUFST_PlugIn=path``, where path specifies a disk path to the UFST plugin file, which Monotype Imaging distributes in ``ufst/fontdata/MTFONTS/PCL45/MT3/plug__xi.fco``. If UFST needs it and the command line argument is not specified, Ghostscript prints a warning and searches plugin file in the current directory.

If you want to run UFST with resident UFST fonts only (and allow Ghostscript font renderer to handle fonts, which may be downloaded or embedded into documents), you should run Ghostscript with these command line arguments : ``-sFCOfontfile=path1 -sFCOfontfile2=path2 -sUFST_PlugIn=path3 -sFAPIfontmap=map-name -sFAPIconfig=FAPIconfig-FCO`` where ``path1`` specifies a disk path to the main FCO file, ``path2`` specifies a disk path to the Wingdings FCO file, ``path3`` a disk path the FCO plugin file, ``path1`` is either ``gs/lib/FCOfontmap-PCLPS2``, ``gs/lib/FCOfontmap-PCLPS3``, or ``gs/lib/FCOfontmap-PS3``. ``FAPIcidfmap`` works as usual, but probably you want to leave it empty because FCO doesn't emulate CID fonts.

Some configurations of UFST need a path for finding symbol set files. If you compiled UFST with such configuration, you should run Ghostscript with a special command line argument ``-sUFST_SSdir=path``, where path specifies a disk path to the UFST support directory, which Monotype Imaging distributes in ``ufst/fontdata/SUPPORT``. If UFST needs it and the command line argument is not specified, Ghostscript prints a warning and searches symbol set files in the current directory.

.. note::

   UFST and Free Type cannot handle some Ghostscript fonts because they do not include a PostScript interpreter and therefore have stronger restrictions on font formats than Ghostscript itself does - in particular, Type 3 fonts. If their font types are listed in ``HookDiskFonts`` or in ``HookEmbeddedFonts``, Ghostscript interprets them as PS files, then serializes font data into a RAM buffer and passes it to FAPI as PCLEOs. (see the FAPI-related source code for details).




.. external links

.. _URW++ Design and Development Incorporated: http://www.urwpp.de/

.. include:: footer.rst


