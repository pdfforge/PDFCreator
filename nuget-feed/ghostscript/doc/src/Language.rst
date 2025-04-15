.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: Ghostscript and the PostScript Language


.. include:: header.rst

.. _Language.html:
.. _Language:


PostScript Language
===========================================



Ghostscript's capabilities in relation to PostScript
---------------------------------------------------------

The Ghostscript interpreter, except as noted below, is intended to execute properly any source program written in the (LanguageLevel 3) PostScript language as defined in the PostScript Language Reference, Third Edition (ISBN 0-201-37922-8) published by Addison-Wesley in mid-1999. However, the interpreter is configurable in ways that can restrict it to various subsets of this language. Specifically, the base interpreter accepts the Level 1 subset of the PostScript language, as defined in the first edition of the PostScript Language Reference Manual (ISBN 0-201-10174-2) Addison-Wesley 1985, plus the file system, version 25.0 language, and miscellaneous additions listed in sections A.1.6, A.1.7, and A.1.8 of the Second Edition respectively, including allowing a string operand for the "``status``" operator. The base interpreter may be configured (see the documentation on :ref:`building Ghostscript<Make.html>` for how to configure it) by adding any combination of the following:

- The ability to process PostScript Type 1 fonts. This facility is normally included in the interpreter.

- The CMYK color extensions listed in section A.1.4 of the Second Edition (including ``colorimage``). These facilities are available only if the ``color``, ``dps``, or ``level2`` feature was selected when Ghostscript was built.

- The Display PostScript extensions listed in section A.1.3 of the Second Edition, but excluding the operators listed in section A.1.2. These facilities are available only if the dps feature or the ``level2`` feature was selected when Ghostscript was built.

- The composite font extensions listed in section A.1.5 of the Second Edition, and the ability to handle Type 0 fonts. These facilities are available only if the compfont feature or the ``level2`` feature was selected when Ghostscript was built.

- The ability to load TrueType fonts and to handle PostScript Type 42 (encapsulated TrueType) fonts. These facilities are available only if the ``ttfont`` feature was selected when Ghostscript was built.

- The PostScript Level 2 "filter" facilities except the ``DCTEncode`` and ``DCTDecode`` filters. These facilities are available only if the ``filter``, ``dps``, or ``level2`` feature was selected when Ghostscript was built.

- The PostScript Level 2 ``DCTEncode`` and ``DCTDecode`` filters. These facilities are available only if the ``dct`` or ``level2`` feature was selected when Ghostscript was built.

- All the other PostScript Level 2 operators and facilities listed in section A.1.1 of the Second Edition and not listed in any of the other A.1.n sections. These facilities are available only if the ``level2`` feature was selected when Ghostscript was built.

- All PostScript LanguageLevel 3 operators and facilities listed in the Third Edition, except as noted below. These facilities are available only if the ``psl3`` feature was selected when Ghostscript was built.

- The ability to recognize DOS EPSF files and process only the PostScript part, ignoring bitmap previews or other information. This facility is available only if the ``epsf`` feature was selected when Ghostscript was built.


Ghostscript currently does not implement the following PostScript LanguageLevel 3 facilities:

- Settable ``ProcessColorModel`` for page devices, except for a very few special devices.

- ``IODevices`` other than ``%stdin``, ``%stdout``, ``%stderr``, ``%lineedit``, ``%statementedit``, ``%os%``, and (if configured) ``%pipe%`` and ``%disk0%`` through ``%disk0%``.


Ghostscript can also interpret files in the Portable Document Format (PDF) 1.7 format defined in the PDF Reference Version 1.7, distributed by Adobe Systems Incorporated, except as noted below. This facility can be disabled by deselecting the pdf feature when Ghostscript is built.

Ghostscript currently implements the majority of non-interactive features defined in the PDF reference.

Ghostscript also includes a number of :ref:`additional operators<Additional Operators>` defined below that are not in the PostScript language defined by Adobe.


Implementation limits
---------------------------

The implementation limits show here correspond to those in Tables B.1 and B.2 of the Second and Third Editions, which describe the quantities fully. Where Ghostscript's limits are different from those of Adobe's implementations (as shown in the Third Edition), Adobe's limits are also shown.


Architectural limits
~~~~~~~~~~~~~~~~~~~~

*Architectural limits (corresponds to Adobe table B.1)*



.. list-table::
   :widths: 25 25 25 25
   :header-rows: 1

   * - Quantity
     - Limit
     - Type
     - Adobe
   * - integer
     - 32-bit
     - twos complement integer
     -
   * - real
     - single-precision
     - IEEE float
     -
   * - array
     - 16777216
     - elements
     - 65535
   * - dictionary
     - 16777215
     - elements
     - 65535
   * - string
     - 16777216
     - characters
     - 65535
   * - name
     - 16383
     - characters
     - 127
   * - filename
     - 128 :ref:`*<AdobeTableNote1>`
     - characters
     -
   * - ``save`` level
     - none
     - (capacity of memory)
     - 15
   * - ``gsave`` level
     - none
     - (capacity of memory)
     - 13

.. _AdobeTableNote1:

\* The limit on the length of a file name is 128 characters if the name starts with a ``%...% IODevice`` designation, or 124 characters if it does not.


Typical memory limits in LanguageLevel 1
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

*Memory limits (corresponds to Adobe table B.2)*


.. list-table::
   :widths: 25 25 25 25
   :header-rows: 1

   * - Quantity
     - Limit
     - Type
     - Adobe
   * - ``userdict``
     - 200
     -
     -
   * - ``FontDirectory``
     - 100
     -
     -
   * - operand stack
     - 800
     -
     - 500
   * - dictionary stack
     - 20
     -
     -
   * - execution stack
     - 250
     -
     -
   * - interpreter level
     - none
     - (capacity of memory)
     - 10
   * - path
     - none
     - (capacity of memory)
     - 1500
   * - dash
     - 11
     -
     -
   * - VM
     - none
     - (capacity of memory)
     - 240000
   * - file
     - none
     - (determined by operating system)
     - 6
   * - image
     - 65535
     - values (samples × components)

       for 1-, 2-, 4-, or 8-bit samples
     - 3300
   * -
     - 32767
     - values for 12-bit samples
     - 3300


Other differences in VM consumption
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

In 32-bit builds packed array elements occupy either 2 bytes or 12 bytes. The average element size is probably about 7 bytes. Names occupy 16 bytes plus the space for the string.

In 64-bit builds packed array elements occupy either 2 bytes or 16 bytes. The average element size is probably about 9 bytes. Names occupy 24 bytes plus the space for the string.

The garbage collector doesn't reclaim portions of arrays obtained with getinterval, rather it collects entire arrays.



.. _Additional Operators:


Additional operators in Ghostscript
-------------------------------------

Graphics and text operators
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


.. _Language_Transparency:

Transparency
"""""""""""""""

.. note::

   The following paragraphs describe non-standard operators for accessing the PDF 1.4 and later transparent imaging model through Postscript. If used incorrectly, they can have unexpected side effects and result in undefined behavior. As a result, these operators are disabled when :ref:`SAFER<dSAFER>` is in force (as it is by default from version 9.50 onwards). To utilise these operators you will either have to disable ``SAFER`` (``-dNOSAFER``) or use the command line parameter ``-dALLOWPSTRANSPARENCY``. The latter will make the custom operators available, but leave the file access controls active.

Ghostscript provides a set of operators for implementing the transparency and compositing facilities of PDF 1.4. These are defined only if the ``transpar`` option was selected when Ghostscript was built. We do not attempt to explain the underlying graphics model here: for details, see Adobe Technical Note #5407, "Transparency in PDF". Previously (in 9.52 and earlier), Ghostscript's model maintained separate alpha and mask values for opacity and shape. This model has been changed (as of 9.53) and instead Ghostscript maintains separate float values for stroke and fill alpha values with a boolean that indicates if these should be interpreted as shape or alpha values to be more in line with the PDF specification.

What follows is a subset of all the custom operators related to transparency, but covers the most useful, most common requirements.

Graphics state operators
^^^^^^^^^^^^^^^^^^^^^^^^^^^

Pushing the compositor device must be done before any other marking operations are made on the current page, and must be done per page. Popping the compositor should be done after the last marking operation of the page, and before the call to ``showpage``. Any marking operations made after the compositor is popped will bypass the transparent imaging model, and may produce unexpected output.


``<depth> .pushpdf14devicefilter -``
   Installs the transparency compositor device into the graphics state. At present the ``depth`` parameter should always be zero (Subject To Change.)

``- .popdf14devicefilter -``
   Removes (or, more accuracately, disables) the transparency compositor in graphics state.

``<modename> .setblendmode -``
   Sets the blending mode in the graphics state. If the mode name is not recognized, causes a ``rangecheck`` error. The initial value of the blending mode is ``/Compatible``.

``- .currentblendmode <modename>``
   Returns the graphics state blend mode on the stack.

``[Deprecated as of 9.53] <0..1> .setopacityalpha -``
   Sets the opacity alpha value in the graphics state. The initial opacity alpha value is 1. Note, it is strongly suggested that this method not be used as it currently may give inconsistent results when mixed with methods that set stroke and fill alpha values.

``[Deprecated as of 9.53] - .currentopacityalpha <0..1>``
   Returns the graphics state opacity alpha on the stack. Note, it is strongly suggested that this method not be used as it currently may give inconsistent results when mixed with methods that set stroke and fill alpha values.

``[Deprecated as of 9.53] <0..1> .setshapealpha -``
   Sets the shape alpha value in the graphics state. The initial shape alpha value is 1. Note, it is strongly suggested that this method not be used as it currently may give inconsistent results when mixed with methods that set stroke and fill alpha values.

``[Deprecated as of 9.53] - .currentshapealpha <0..1>``
   Returns the graphics state shape alpha on the stack. Note, it is strongly suggested that this method not be used as it currently may give inconsistent results when mixed with methods that set stroke and fill alpha values.

``<0..1> .setstrokeconstantalpha -``
   Sets the stroke alpha value in the graphics state. The initial stroke alpha value is 1.

``- .currentstrokeconstantalpha <0..1>``
   Returns the graphics state stroke alpha value on the stack.

``<0..1> .setfillconstantalpha -``
   Sets the fill alpha value in the graphics state. The initial fill alpha value is 1.

``- .currentfillconstantalpha <0..1>``
   Returns the graphics state fill alpha value on the stack.

``<bool> .setalphaisshape -``
   If true, the values set by ``setstrokeconstantalpha`` and ``setfillconstantalpha`` are interpreted as shape values. The initial value of the AIS flag is false.

``- .currentalphaisshape <0..1>``
   Returns the graphics state alpha is shape (AIS) on the stack.

``<bool> .settextknockout -``
   Sets the text knockout flag in the graphics state. The initial value of the text knockout flag is true.

``- .currenttextknockout <bool>``
   Returns the graphics state text knockout on the stack.


Rendering stack operators
^^^^^^^^^^^^^^^^^^^^^^^^^^^

The interpreter state is extended to include a (per-context) rendering stack for handling transparency groups and masks (generically, "layers"). Groups accumulate a full value for each pixel (paint plus transparency); masks accumulate only a coverage value. Layers must be properly nested, i.e., the 'end' or 'discard' operator must match the corresponding 'begin' operator.

Beginning and ending groups must nest properly with respect to save and restore: save and restore do not save and restore the layer stack. Currently, layers are not required to nest with respect to gsave and grestore, except that the device that is current in the graphics state when ending a layer must be the same as the device that was current when beginning the layer.


.. warning:: THIS AREA IS SUBJECT TO CHANGE.

``<paramdict> <llx> <lly> <urx> <ury> .begintransparencygroup -``
   Begins a new transparency group. The ``ll/ur`` coordinates are the bounding box of the group in the current user coordinate system. ``paramdict`` has the following keys:

      ``/Isolated``
         (optional) Boolean; default value = false.

      ``/Knockout``
         (optional) Boolean; default value = false.

``- .endtransparencygroup -``
   Ends the current transparency group, compositing the group being ended onto the group that now becomes current.

``<cs_set?> <paramdict> <llx> <lly> <urx> <ury> .begintransparencymaskgroup -``
   Begins a new transparency mask, which is represented as a group. The ``ll/ur`` coordinates are the bounding box of the mask in the current user coordinate system. ``paramdict`` has the following keys:

      ``/Subtype``
         (required) Name, either ``/Alpha`` or ``/Luminosity``.

      ``/Background``
         (optional) Array of number.

      ``/TransferFunction``
         (optional) Function object (produced by applying ``.buildfunction`` to a Function dictionary).

   The ``cs_set`` parameter is a boolean indicating whether the color space for the mask group is the current color space in the graphics state, or whether mask group color space should be inherited from the previous group in the transparency group stack. In general, for the most consistent results, it is recommended that this be set to true, and the intended color space set in the graphics state prior to the ``.begintransparencymaskgroup`` call.

``<mask#> .endtransparencymask -``
   Ends the current transparency mask group, compositing the mask group being ended and setting it as the current soft mask in the graphics state.
   The ``mask#`` parameter indicates whether the mask should be treated as as opacity mask (``0``) or shape (``1``).


New ImageType
^^^^^^^^^^^^^^^^^^^^^^^^^^^

The transparency extension defines a new ImageType 103, similar to ImageType 3 with the following differences:

- The required ``MaskDict`` is replaced by two optional dictionaries, ``OpacityMaskDict`` and ``ShapeMaskDict``. If present, these dictionaries must have a ``BitsPerComponent`` entry, whose value may be greater than 1. Note that in contrast to ImageType 3, where any non-zero chunky mask value is equivalent to 1, ImageType 103 simply takes the low-order bits of chunky mask values.

- A ``Matte`` entry may be present in one or both mask dictionaries, indicating premultiplication of the data values. If both ``MaskDicts`` have a ``Matte`` entry and the values of the two ``Matte`` entries are different, a ``rangecheck`` error occurs.

- ``InterleaveType`` appears in the ``MaskDicts``, not the ``DataDict``, because each mask has its own ``InterleaveType``. ``InterleaveType`` 2 (interlaced scan lines) is not supported.



Other graphics state operators
"""""""""""""""""""""""""""""""

``<int> .setoverprintmode -``
   Sets the overprint mode in the graphics state. Legal values are 0 or 1. Per the PDF 1.3 specification, if the overprint mode is 1, then when the current color space is ``DeviceCMYK``, color components whose value is 0 do not write into the target, rather than writing a 0 value. THIS BEHAVIOR IS NOT IMPLEMENTED YET. The initial value of the overprint mode is 0.

``- .currentoverprintmode <int>``
   Returns the current overprint mode.

Character operators
"""""""""""""""""""""""""""""""

``<font> <charcode> %Type1BuildChar -``
   This is not a new operator: rather, it is a name known specially to the interpreter. Whenever the interpreter needs to render a character (during a ``...show``, ``stringwidth``, or ``charpath``), it looks up the name ``BuildChar`` in the font dictionary to find a procedure to run. If it does not find this name, and if the ``FontType`` is 1, the interpreter instead uses the value (looked up on the dictionary stack in the usual way) of the name ``%Type1BuildChar``.

   The standard definition of ``%Type1BuildChar`` is in the initialization file ``gs_type1.ps``. Users should not need to redefine ``%Type1BuildChar``, except perhaps for tracing or debugging.

``<font> <charname> %Type1BuildGlyph -``
   Provides the Type 1 implementation of ``BuildGlyph``.


Other operators
~~~~~~~~~~~~~~~~~~~~~

Mathematical operators
""""""""""""""""""""""""""

``<number> arccos <number>``
   Computes the arc cosine of a number between -1 and 1.

``<number> arcsin <number>``
   Computes the arc sine of a number between -1 and 1.

Dictionary operators
""""""""""""""""""""""""""

``mark <key1> <value1> <key2> <value2> ... .dicttomark <dict>``
   Creates and returns a dictionary with the given keys and values. This is the same as the PostScript Level 2 >> operator, but is available even in Level 1 configurations.

``<dict> <key> .knownget <value> true``, ``<dict> <key> .knownget false``
   Combines ``known`` and ``get`` in the obvious way.


File operators
""""""""""""""""""""""""""

``<file> .fileposition <integer> true``
   Returns the position of file. Unlike the standard ``fileposition`` operator, which causes an error if the file is not positionable, ``.fileposition`` works on all files, including filters: for non-positionable files, it returns the total number of bytes read or written since the file was opened.

``<string> findlibfile <foundstring> <file> true``, ``<string> findlibfile <string> false``
   Opens the file of the given name for reading, searching through directories :ref:`as described in the usage documentation<Use_How Ghostscript finds files>`. If the search fails, ``findlibfile`` simply pushes false on the stack and returns, rather than causing an error.

.. _Tempfile:

``<prefix_string|null> <access_string> .tempfile <string> <file>``
   Creates and opens a temporary file like the file operator, also returning the file name. There are three cases for the ``<prefix_string|null>`` operand:

   - ``null``: create the file in the same directory and with the same name conventions as other temporary files created by the Ghostscript implementation on this platform. E.g., the temporary file might be named ``/tmp/gs_a1234``.

   - A string that contains only alphanumeric characters, underline, and dash: create the file in the standard temporary directory, but use the ``<prefix_string>`` as the first part of the file name. E.g., if ``<prefix_string>`` is ``xx``, the temporary file might be named ``/tmp/xxa1234``.

   - A string that is the beginning of an absolute file name: use the ``<prefix_string>`` as the first part of the file name. E.g., if ``<prefix_string>`` is ``/my/tmpdir/zz``, the temporary file might be named ``/my/tmpdir/zza1234``.

    When running in ``SAFER`` mode, the absolute path must be one of the strings on the permit file writing list (see :ref:`-dSAFER<dSAFER>`) .


Ghostscript also supports the following ``IODevice`` in addition to a subset of those defined in the Adobe documentation:

- ``%pipe%command``, which opens a pipe on the given command. This is supported only on operating systems that provide popen (primarily Unix systems, and not all of those).

- ``%disk#%``, which emulates the ``%disk0`` through ``%disk9`` devices on some Adobe PostScript printers. This pseudo device provides a flat filenaming system with a user definable location for the files (``/Root``). These devices will only be present if the ``diskn.dev`` feature is specified during the build.

   This feature is intended to allow compatibility with font downloaders that expect to store fonts on the ``%disk`` device of the printer.

   Use of the ``%disk#%`` devices requires that the location of files be given by the user setting the ``/Root`` device parameter. The syntax for setting the ``/Root`` parameter is:

      .. code-block:: postscript

           mark /Root (directory_specification) (%disk#) .putdevparams

   For example, to store the files of the ``%disk0`` device on the directory ``/tmp/disk0``, use:

      .. code-block:: postscript

           mark /Root (/tmp/disk0/) (%disk0) .putdevparams

   The files will be stored in the specified directory with arbitrary names. A mapping file is used to store the association between the file names given for the file operations on the ``%diskn#`` device and the file that resides in the ``/Root`` directory.


Miscellaneous operators
""""""""""""""""""""""""""

``<array> bind <array>``

   Depending on the command line parameters ``bind`` is redefined as:


   .. list-table::
      :widths: 50 50
      :header-rows: 1

      * - Flag
        - Definition
      * - ``DELAYBIND``
        - Returns the argument, stores the argument for later use by ``.bindnow``



``<array> .bind <array>``
   Performs standard bind operation as defined in PLRM regardless of the ``DELAYBIND`` flag.


.. _Language_BindNow:

``- .bindnow -``
   Applies ``bind`` operator to all saved procedures after binding has been deferred through ``-dDELAYBIND``. Note that idiom recognition has no effect for the deferred binding because the value returned from ``bind`` is discarded.

   Since v. 8.12 ``.bindnow`` undefines itself and restores standard definition of ``bind`` operator. In earlier versions after calling ``.bindnow``, the postscript ``bind`` operator needs to be rebound to the internal implementation ``.bind``, as in this fragment from the ``ps2ascii`` script:

   .. code-block:: postscript

      DELAYBIND {
        .bindnow
        /bind /.bind load def
      } if


   This is necessary for correct behavior with later code that uses the ``bind`` operator.


``<string> getenv <string> true``, ``<string> getenv false``
   Looks up a name in the shell environment. If the name is found, returns the corresponding value and true; if the name is not found, returns false.

``<string> <boolean> .setdebug -``
   Sets or clears any subset of the debugging flags included in ``<string>`` based on the value of ``<boolean>``. These correspond to the debug flags set by ``-Z`` on the command line and enable debug and tracing output from various internal modules.

   .. note::

      Most tracing output is only produced if the Ghostscript interpreter was built with the ``DEBUG`` preprocessor symbol defined.

   The ``zsetdebug()`` C function, which implements this operator, is a useful breakpoint for debuggers. Inserting '``() true .setdebug``' in the interpreted code will trigger a breakpoint at that location without side effects. The current flag state is available in C as the ``gs_debug[]`` array, indexed by character value. The ``zsetdebug`` function will be entered, and ``gs_debug[]`` updated, whether or not Ghostscript is built with the ``DEBUG`` preprocessor symbol defined, so this is useful even with release builds.


``- .setsafe -``
   If Ghostscript is started with ``-dNOSAFER`` or ``-dDELAYSAFER``, this operator can be used to enter ``SAFER`` mode (see :ref:`-dSAFER<dSAFER>`)

   The following is deprecated, see :ref:`-dSAFER<dSAFER>`.

   Since ``SAFER`` mode is implemented with userparameters and device parameters, it is possible to use ``save`` and ``restore`` before and after ``.setsafe`` to return to ``NOSAFER`` mode, but note that such a save object is accessible to any procedures or file run in ``SAFER`` mode. A malicious file with an unbalanced restore could potentially restore back to a point where ``SAFER`` was not in operation.

   .. note::

      This uses ``setpagedevice`` to change ``.LockSafetyParams``, so the page will be erased as a side effect of this operator.



``- .locksafe -``
   The following is deprecated, see :ref:`-dSAFER<dSAFER>`.

   This operator sets the current device's ``.LockSafetyParams`` and the ``LockFilePermissions`` user parameter true as well as adding the paths on ``LIBPATH`` and ``FONTPATH`` and the paths given by the system params ``/GenericResourceDir`` and ``/FontResourceDir`` to the current ``PermitFileReading`` list of paths.

   If Ghostscript is started with ``-dNOSAFER`` or ``-dDELAYSAFER``, this operator can be used to enter ``SAFER`` mode with the current set of ``PermitFile...`` user parameters in effect. Since ``.setsafe`` sets the ``PermitFile...`` user parameters to empty arrays, a script or job server that needs to enable certain paths for file Reading, Writing and/or Control can use this operator to perform the locking needed to enter ``SAFER`` mode.

   For example, to enable reading everywhere, but disallow writing and file control (deleting and renaming files), the following can be used:

   .. code-block:: postscript

       { << /PermitFileReading [ (*) ]
            /PermitFileWriting [ ]
            /PermitFileControl [ ]
         >> setuserparams
         .locksafe
       } stopped pop


   In the above example, use of stopped will allow the use of this sequence on older versions of Ghostscript where ``.locksafe`` was not an operator.


   .. note::

      This uses ``setpagedevice`` to change ``.LockSafetyParams``, so the page will be erased as a side effect of this operator.

   See also :ref:`.LockSafetyParams<Language_LockSafetyParams>` and `User Parameters`_.


.. _Language_AddControlPath:

``<name> <string> .addcontrolpath``

   Adds a single path to the file access control lists.

   The ``<name>`` parameter can be one of:

   - ``/PermitFileReading``

   - ``/PermitFileWriting``

   - ``/PermitFileControl``

   Whilst the string parameter is the path to be added to the requested list.

   .. note ::

      Any attempt to call this operator after ``.activatepathcontrol`` has been called will result in a ``Fatal`` error, and the interpreter will immediately exit.


.. _Language_ActivateControlPath:

``.activatepathcontrol``
   Activates file access controls. Once activated, these access controls remain in place until the interpreter shuts down.

``.currentpathcontrolstate``
   Returns true on the operand stack if file access control has been activated, false if not.

``<dict> .genordered <dict> (default: /OutputType /Type3).``, ``<dict> .genordered <string> (/OutputType /ThreshString).``, ``<dict> .genordered <array> (/OutputType /TOSArray).``
   This operator creates an ordered dither screening pattern with the parameters from the dictionary, returning (by default) a PostScript HalftoneType 3 (threshold array based) dictionary suitable for use with ``sethalftone`` or as a component Halftone of a ``HalftoneType 5`` Halftone dictionary. The ``/OutputType`` parameter can also select other than Halftone Type 3 as the return paramter, ``<dict>`` has the following keys (all are optional):

      ``/Frequency``
         Integer; default value = 75

      ``/Angle``
         Integer; default value = 0

      ``/HResolution``
         Real or Integer; default value is device X resolution.

      ``/VResolution``
         Real or Integer; default value is device Y resolution.

      ``/DotShape``
         Integer; default value = 0 (CIRCLE). Other shapes available are:

         1=REDBOOK, 2=INVERTED, 3=RHOMBOID, 4=LINE_X, 5=LINE_Y, 6=DIAMOND1, 7=DIAMOND2, 8=ROUNDSPOT

      ``/SuperCellSize``
         Integer; default value = 1 -- actual cell size determined by Frequency, Angle, H/V Resolution.

         A larger value will allow more levels to be attained.

      ``/Levels``
         Integer; default value = 1 -- actual number of gray levels is determined by Frequency and H/V Resolution.

         SuperCellSize may need to be specified large enough to achieve the requested number of gray levels.

      ``/OutputType``
         Name; default value = /Type3 (HalftoneType 3 dictionary).

      ``/ThreshString``
         First two bytes are width (high byte first), next two bytes are height, followed by the threshold array bytes (same as ``/Thresholds`` of the Type3 dictionary).

      ``/TOSArray``
         First element is the width, next is the height, followed by pairs X, then Y, of the turn-on-sequence of the threshold array. This information can be used to construct a threshold array with a transfer function "pickled into" the threshold array, which is useful if the turn-on-sequence has more than 256 pairs. Refer to ``toolbin``/``halftone``/``thresh_remap`` for more information.

``.shellarguments``
   This operator is used to access the ``ARGUMENTS`` command line option.

   Relies on Ghostscript being called with the "``--``" command line option - see :ref:`Input Control<Use_Input Control>`.

   See examples in ``lib`` for more information.



Device operators
""""""""""""""""""""""""""

``<matrix> <width> <height> <palette> makeimagedevice <device>``
   Makes a new device that accumulates an image in memory. ``matrix`` is the initial transformation matrix: it must be orthogonal (that is, [a 0 0 b x y] or [0 a b 0 x y]). palette is a string of 2^N or 3 × 2^N elements, specifying how the 2^N possible pixel values will be interpreted. Each element is interpreted as a gray value, or as RGB values, multiplied by 255. For example, if you want a monochrome image for which 0=white and 1=black, the palette should be <ff 00>; if you want a 3-bit deep image with just the primary colors and their complements (ignoring the fact that 3-bit images are not supported), the palette might be ``<000000 0000ff 00ff00 00ffff ff0000 ff00ff ffff00 ffffff>``. At present, the palette must contain exactly 2, 4, 16, or 256 entries, and must contain an entry for black and an entry for white; if it contains any entries that aren't black, white, or gray, it must contain at least the six primary colors (red, green, blue, and their complements cyan, magenta, and yellow); aside from this, its contents are arbitrary.
   This operator is only available when running Ghostscript with NOSAFER.

   Alternatively, ``palette`` can be 16, 24, 32, or null (equivalent to 24). These are interpreted as:


   .. list-table::
      :widths: 50 50
      :header-rows: 1

      * - Palette
        - Bits allocated per color
      * - 16
        - 5 red, 6 green, 5 blue
      * - 24
        - 8 red, 8 green, 8 blue
      * - 32
        - 8C, 8M, 8Y, 8K


   .. note::

      One can also make an image device (with the same palette as an existing image device) by copying a device using the copydevice operator.


``<device> <index> <string> copyscanlines <substring>``
   Copies one or more scan lines from an image device into a string, starting at a given scan line in the image. The data is in the same format as for the image operator. It is an error if the device is not an image device or if the string is too small to hold at least one complete scan line. Always copies an integral number of scan lines.

``<device> setdevice -``
   Sets the current device to the specified device. Also resets the transformation and clipping path to the initial values for the device. Signals an ``invalidaccess`` error if the device is a prototype or if :ref:`.LockSafetyParams<Language_LockSafetyParams>` is true for the current device.

   Some device properties may need to be set before ``setdevice`` is called. For example, the :title:`pdfwrite` device will try to open its output file, causing an ``undefinedfilename`` error if ``OutputFile`` hasn't been set to a valid filename. In such cases use the level 2 operator instead: ``<< /OutputDevice /pdfwrite /OutputFile (MyPDF.pdf) >> setpagedevice``.

``- currentdevice <device>``
   Gets the current device from the graphics state.


Filters
--------


Standard filters
~~~~~~~~~~~~~~~~~~~~~~~

In its usual configuration, Ghostscript supports all the standard PostScript LanguageLevel 3 filters, both encoding and decoding, except that it does not currently support:

- the ``EarlyChange`` key in the ``LZWEncode`` filter.

Ghostscript also supports additional keys in the optional dictionary operands for some filters. For the ``LZWDecode`` filter:

``InitialCodeLength <integer> (default 8)``
   An integer between 2 and 11 specifying the initial number of data bits per code. Note that the actual initial code length is 1 greater than this, to allow for the reset and end-of-data code values.

``FirstBitLowOrder <boolean> (default false)``
   If true, codes appear with their low-order bit first.

``BlockData <boolean> (default false)``
   If true, the data is broken into blocks in the manner specified for the GIF file format.


For the ``CCITTFaxEncode`` and ``CCITTFaxDecode`` filters:

``DecodedByteAlign <integer> (default 1)``
   An integer N with the value 1, 2, 4, 8, or 16, specifying that decoded data scan lines are always a multiple of N bytes. The encoding filter skips data in each scan line from Columns to the next multiple of N bytes; the decoding filter pads each scan line to a multiple of N bytes.


Non-standard filters
~~~~~~~~~~~~~~~~~~~~~~~

In addition to the standard PostScript LanguageLevel 3 filters, Ghostscript supports the following non-standard filters. Many of these filters are used internally to implement standard filters or facilities; they are almost certain to remain, in their present form or a backward-compatible one, in future Ghostscript releases.

``<target> /BCPEncode filter <file>``, ``<source> /BCPDecode filter <file>``
   Create filters that implement the Adobe Binary Communications Protocol. See Adobe documentation for details.

``<target> <seed_integer> /eexecEncode filter <file>``
   Creates a filter for encrypting data into the encrypted format described in the Adobe Type 1 Font Format documentation. The ``seed_integer`` must be 55665 for the ``eexec`` section of a font, or 4330 for a ``CharString``. Note that for the ``eexec`` section of a font, this filter produces binary output and does not include the initial 4 (or lenIV) garbage bytes.

``<source> <seed_integer> /eexecDecode filter <file>``, ``<source> <dict> /eexecDecode filter <file>``
   Creates a filter for decrypting data encrypted as described in the Adobe Type 1 Font Format documentation. The ``seed_integer`` must be 55665 or 4330 as described just above. PDF interpreters don't skip space characters after operator ``eexec``. Use ``keep_spaces = true`` for decoding embedded PDF fonts. Recognized dictionary keys are:

   .. code-block:: postscript

      seed <16-bit integer> (required)
      lenIV <non-negative integer> (default=4)
      eexec <bool> (default=false)
      keep_spaces <bool> (default=false)

``<target> /MD5Encode filter <file>``
   Creates a filter that produces the 16-byte MD5 digest of the input. Note that no output is produced until the filter is closed.

``<source> <hex_boolean> /PFBDecode filter <file>``
   Creates a filter that decodes data in ``.PFB`` format, the usual semi-binary representation for Type 1 font files on IBM PC and compatible systems. If ``hex_boolean`` is true, binary packets are converted to hex; if false, binary packets are not converted.



``<target> <dict> /PixelDifferenceEncode filter <file>``, ``<source> <dict> /PixelDifferenceDecode filter <file>``
   Implements the Predictor=2 pixel-differencing option of the LZW filters. Recognized keys are:

   .. code-block:: postscript

      Colors <integer> (1 to 4, default=1)
      BitsPerComponent <integer> (1, 2, 4, or 8, default=8)
      Columns <integer> (>= 0, required)

   See the Adobe PDF Reference Manual for details.

``<target> <dict> /PNGPredictorEncode filter <file>``, ``<source> <dict> /PNGPredictorDecode filter <file>``
   Implements the "filter" algorithms of the Portable Network Graphics (PNG) graphics format. Recognized keys are:


   .. list-table::
      :widths: 40 40 20
      :header-rows: 1

      * - Key
        - Range
        - Default
      * - ``Colors <integer>``
        - 1 to 16
        - 16
      * - ``BitsPerComponent <integer>``
        - 1, 2, 4, 8, or 16
        - 8
      * - ``Columns <integer>``
        - >= 0
        - 1
      * - ``Predictor <integer>``
        - 10 to 15
        - 15

   The ``Predictor`` is the PNG algorithm number + 10 for the ``Encoding`` filter; the ``Decoding`` filter ignores ``Predictor``. 15 means the encoder attempts to optimize the choice of algorithm. For more details see the `PNG specification`_.



``<target> /TBCPEncode filter <file>``, ``<source> /TBCPDecode filter <file>``
   Create filters that implement the Adobe Tagged Binary Communications Protocol. See Adobe documentation for details.

``<target> /zlibEncode filter <file>``, ``<source> /zlibDecode filter <file>``
   Creates filters that use the data compression method variously known as 'zlib' (the name of a popular library that implements it), 'Deflate' (as in `RFC 1951`_, which is a detailed specification for the method), 'gzip' (the name of a popular compression application that uses it), or 'Flate' (Adobe's name). Note that the PostScript ``Flate`` filters are actually a combination of this filter with an optional predictor filter.



Unstable filters
~~~~~~~~~~~~~~~~~~~~~~~

Some versions of Ghostscript may also support other non-standard filters for experimental purposes. The current version includes the following such filters, which are not documented further. No code should assume that these filters will exist in compatible form, or at all, in future versions.

``<target/source> <string> ByteTranslateEncode/Decode filter <file>``
   ``string`` must be a string of exactly 256 bytes. Creates a filter that converts each input byte ``b`` to ``string[b]``. Note that the ``Encode`` and ``Decode`` filters operate identically: the client must provide a string for the ``Decode`` filter that is the inverse mapping of the ``string`` for the ``Encode`` filter.

``<target/source> <dict> BoundedHuffmanEncode/Decode filter <file>``
   These filters encode and decode data using Huffman codes. Since these filters aren't used anywhere, we don't document them further, except to note the recognized dictionary keys, which must be set identically for encoding and decoding:

   .. code-block:: postscript

      FirstBitLowOrder <bool> (default=false)
      MaxCodeLength <int> (default=16)
      EndOfData <bool> (default=true)
      EncodeZeroRuns <int> (default=256)
      Tables <int_array>


``<target/source> <dict> BWBlockSortEncode/Decode filter <file>``
   This filter implements the Burroughs-Wheeler block sorting compression method, which we've heard is also used in the popular ``bzip2`` compression application. The only recognized dictionary key is:

   .. code-block:: postscript

      BlockSize <integer> (default=16384)




.. _Language_DeviceParameters:


Device parameters
---------------------


Ghostscript supports the concept of device parameters for all devices, not just page devices. Here are the currently defined parameters for all devices:


.. _Language_LockSafetyParams:

``.LockSafetyParams <boolean>``
   This parameter allows for improved system security by preventing PostScript programs from being able to change potentially dangerous device parameters such as ``OutputFile``. This parameter cannot be set false if it is already true.

   If this parameter is true for the current device, attempt to set a new device that has ``.LockSafetyParams`` false will signal an ``invalidaccess`` error.


``BitsPerPixel <integer> (usually read-only)``
   Number of bits per pixel.

``.HWMargins [<four floats>]``
   Size of non-imageable regions around the edges of the page, in points (units of 1/72in; see the :ref:`notes on measurements<Devices_Notes on measurements>` in the documentation on :ref:`devices<Devices.html>`).


``HWSize [<integer> <integer>]``
   X and Y size in pixels.

``%MediaSource <integer>``
   The input tray key as determined by ``setpagedevice``. PostScript language programs don't set this parameter directly; they can request a particular tray through the ``MediaPosition`` ``setpagedevice`` parameter, but the ``setpagedevice`` logic need not necessarily honor the request. Devices which support switchable trays should implement ``%MediaSource`` in their ``put_params`` device procedure, but (unlike most other such parameters) need not implement corresponding reading logic in ``get_params``.

``%MediaDestination <integer>``
   The output tray key as determined by ``setpagedevice``. Handling by devices should be parallel to ``%MediaSource``.

``.IgnoreNumCopies <boolean>``
   Some page description languages support a ``NumCopies`` parameter. This parameter instructs the device to ignore this, producing only one copy of the document on output. Note that some devices ignore ``NumCopies`` regardless because of limitation of the output format or the implementation.

``Name <string> (read-only)``
   The device name. Currently the same as ``OutputDevice``.

``Colors, GrayValues, RedValues, GreenValues, BlueValues, ColorValues (usually read-only)``
   As for the ``deviceinfo`` operator of Display PostScript. ``Red``, ``Green``, ``Blue``, and ``ColorValues`` are only defined if ``Colors > 1``.

``TextAlphaBits, GraphicsAlphaBits (usually read-only)``
   The number of bits of anti-aliasing information for text or graphics respectively. Legal values are 1 (no anti-aliasing, the default for most devices), 2, or 4.

   Because this feature relies upon rendering the input it is incompatible, and will generate an error on attempted use, with any of the vector output devices.


Ghostscript also supports the following read-only parameter that is not a true device parameter:

``.EmbedFontObjects <integer>``
   If non-zero, indicates that the device may embed font objects (as opposed to bitmaps for individual characters) in the output. The purpose of this parameter is to disable third-party font renderers for such devices. (This is zero for almost all devices.)

In addition, the following are defined per Adobe's documentation for the ``setpagedevice`` operator:

.. code-block:: postscript

   Duplex (if supported)
   HWResolution
   ImagingBBox
   Margins
   LeadingEdge
   MediaPosition
   NumCopies (for printers only)
   Orientation (if supported)
   OutputDevice
   PageOffset (write-only)
   PageSize
   ProcessColorModel (usually read-only)

Some devices may only allow certain values for ``HWResolution`` and ``PageSize``. The null device ignores attempts to set ``PageSize``; its size is always ``[0 0]``.

It should be noted that calling ``setpagedevice`` with one of the above keys may reset the effects of any ``pdfmark`` commands up to that point. In particular this is true of ``HWResolution``, a behavior that differs from Adobe Distiller.

For raster printers and image format (jpeg*, tiff*, png* ...) devices these page device parameters are also defined:


``MaxBitmap <integer>``
   Maximum space for a full page raster image (bitmap) in memory.
   This value includes the space for padding raster lines and for an array of pointers for each raster line, thus the ``MaxBitmap`` value to allow a given ``PageSize`` of a specific number of bits per pixel to be rendered in a full page buffer may be somewhat larger than the bitmap size alone.

``BandListStorage <file|memory>``
   The default is determined by the make file macro ``BAND_LIST_STORAGE``. Since memory is always included, specifying ``-sBandListStorage=memory`` when the default is file will use memory based storage for the band list of the page. This is primarily intended for testing, but if the disk I/O is slow, band list storage in memory may be faster.

``BufferSpace <integer>``
   Size of the buffer space for band lists, if the full page raster image (bitmap) is larger than ``MaxBitmap`` (see above.)

   The buffer space is used to collect display list (clist) commands for the bands and then to consolidate those commands when writing the clist to the selected ``BAND_LIST_STORAGE`` device (memory or file) set when Ghostscript is compiled.

   If ``MaxBitmap`` (above) forces banding mode, and if ``BufferSpace`` is large enough, the display list (``clist``) will consist of a single band.

   The ``BufferSpace`` will determine the size of the 'consolidation' buffer (above) even if the MaxBitmap value is low enough to force banding/clist mode.


``BGPrint <boolean>``
   With many printer devices, when the display list (``clist``) banding mode is being used, the page rendering and output can be performed in a background thread. The default value, false, causes the rendering and printing to be done in the same thread as the parser. When ``-dBGPrint=true``, the page output will be overlapped with parsing and writing the ``clist`` for the next page.

   If the device does not support background printing, rendering and printing will be performed as if ``-dBGPrint=false``.

   .. note::

      The background printing thread will allocate a band buffer (size determined by the ``BufferSpace`` or ``BandBufferSpace`` values) in addition to the band buffer in the 'main' parsing thread.

   If ``NumRenderingThreads`` is ``> 0``, then the background printing thread will use the specified number of rendering threads as children of the background printing thread. The background printing thread will perform any processing of the raster data delivered by the rendering threads. Note that ``BGPrint`` is disabled for vector devices such as :title:`pdfwrite` and ``NumRenderingThreads`` has no effect on these devices either.

``GrayDetection <boolean>``
   When true, and when the display list (``clist``) banding mode is being used, during writing of the ``clist``, the color processing logic collects information about the colors used before the device color profile is applied. This allows special devices that examine ``dev->icc_struct->pageneutralcolor`` with the information that all colors on the page are near neutral, i.e. monochrome, and converting the rendered raster to gray may be used to reduce the use of color toners/inks.

   Since the determination of whether or not the page uses colors is determined before the conversion to device colors, this information is independent of the device output profile. The determination has a small delta (``DEV_NEUTRAL`` and ``AB_NEUTRAL`` in ``base/gscms.h``) to allow colors close to neutral to be detected as neutral. Changing this value requires rebuilding.

   Among the devices distributed with the source, currently only the ``pnmcmyk`` device supports this parameter and will produce either a ``P7 PAM CMYK`` output or a ``P5 PGM Gray`` output depending on the use of color on the page.

   Also, the ``pageneutralcolor`` status can be interrogated as a device parameter of the same name. Using PostScript there are several methods:

      ``currentpagedevice /pageneutralcolor get``

      ``mark currentdevice getdeviceprops .dicttomark /pageneutralcolor get``

      ``/pageneutralcolor /GetDeviceParam .special_op { exch pop }{ //false } ifelse``

   Note that the ``pageneutralcolor`` state is reset to false after the page is output, so this parameter is only valid immediately before ``showpage`` is executed, although the ``setpagedevice`` ``EndPage`` procedure can be used to check the state just prior to the actual output of the page that resets ``pagenuetralcolor``. For example:

      .. code-block:: postscript

         << /EndPage {
            exch pop 2 ne dup {
              currentpagedevice /pageneutralcolor get (pageneutralcolor: ) print = flush
            } if
         }
         >> setpagedevice

   .. note::

      Since ``-dGrayDetection=true`` requires extra checking during writing of the ``clist``, this option should only be used for devices that support the optimization of pages to monochrome, otherwise performance may be degraded for no benefit.

      Since ``GrayDetection=true`` is only effective when in ``clist`` (banding) mode, it is recommended to also force banding. For example: ``-dGrayDetection=true -dMaxBitmap=0``.


``NumRenderingThreads <integer>``
   When the display list (``clist``) banding mode is being used, bands can be rendered in separate threads. The default value, 0, causes the rendering of bands to be done in the same thread as the parser and device driver. ``NumRenderingThreads`` of 1 or higher results in bands rendering in the specified number of 'background' threads.

   The number of threads should generally be set to the number of available processor cores for best throughput.

   Note that each thread will allocate a band buffer (size determined by the ``BufferSpace`` or ``BandBufferSpace`` values) in addition to the band buffer in the 'main' thread.

   Additionally note that this parameter has no effect with devices which do not generally render to a bitmap output, such as the vector devices (e.g. :title:`pdfwrite`) and has no effect when rendering, but not using a ``clist``. See :ref:`Improving performance<Use_Improving Performance>`.



``OutputFile <string>``
   An empty string means "send to printer directly", otherwise specifies the file name for output; ``%d`` is replaced by the page number for page-oriented output devices; on Unix systems ``%pipe%`` *command* writes to a pipe. (``|`` *command* also writes to a pipe, but is now deprecated). Also see the ``-o`` parameter.

   Attempts to set this parameter if ``.LockSafetyParams`` is true will signal an ``invalidaccess`` error.

``OpenOutputFile <boolean>``
   If true, open the device's output file when the device is opened, rather than waiting until the first page is ready to print.

``PageCount <integer> (read-only)``
   Counts the number of pages printed on the device.

.. _Language_BandingParams:

The following parameters are for use only by very specialized applications that separate band construction from band rasterization. Improper use may cause unpredictable errors. In particular, if you only want to allocate more memory for banding, to increase band size and improve performance, use the ``BufferSpace`` parameter, not ``BandBufferSpace``.



``BandHeight <integer>``
   The height of bands when banding. 0 means use the largest band height that will fit within the ``BandBufferSpace`` (or ``BufferSpace``, if ``BandBufferSpace`` is not specified). If ``BandHeight`` is larger than the number of lines that will fit in the buffer, opening the device will fail. If the value is -1, the ``BandHeight`` will automatically be set to the page height (1 band for the entire page). This is primarily for developers debugging ``clist`` issues.

``BandWidth <integer>``
   The width of bands in the rasterizing pass, in pixels. 0 means use the actual page width. A ``BandWidth`` value smaller than the width of the page will be ignored, and the actual page width will be used instead.

``BandBufferSpace <integer>``
   The size of the band buffer in the rasterizing pass, in bytes. 0 means use the same buffer size as for the interpretation pass.


Ghostscript supports the following parameter for ``setpagedevice`` and ``currentpagedevice`` that is not a device parameter per se:

``ViewerPreProcess <procedure>``
   Specifies a procedure to be applied to the page device dictionary before any other processing is done. The procedure may not alter the dictionary, but it may return a modified copy. This "hook" is provided for use by viewing programs such as ``GSview``.




User parameters
---------------------



Ghostscript supports the following non-standard user parameters:

``ProcessDSCComment <procedure|null>``
   If not null, this procedure is called whenever the scanner detects a DSC comment (comment beginning with ``%%`` or ``%!``). There are two operands, the file and the comment (minus any terminating ``EOL``), which the procedure must consume.

``ProcessComment <procedure|null>``
   If not null, this procedure is called whenever the scanner detects a comment (or, if ``ProcessDSCComment`` is also not null, a comment other than a ``DSC`` comment). The operands are the same as for ``ProcessDSCComment``.

``LockFilePermissions <boolean>``
   If true, this parameter and the three ``PermitFile...`` parameters cannot be changed. Attempts to change any of the values when ``LockFilePermissions`` is true will signal ``invalidaccess``. Also, when this value is true, the file operator will give ``invalidaccess`` when attempting to open files (processes) using the ``%pipe`` device.

   Also when ``LockFilePermissions`` is true, strings cannot reference the parent directory (platform specific). For example (``../../xyz``) is illegal on unix, Windows and Macintosh, and (``[.#.#.XYZ]``) is illegal on VMS.

   This parameter is set true by the ``.setsafe`` and ``.locksafe`` operators.


``PermitFileReading <array of strings>``, ``PermitFileWriting <array of strings>``, ``PermitFileControl <array of strings>``
   These parameters specify paths where file reading, writing and the 'control' operations are permitted, respectively. File control operations are ``deletefile`` and ``renamefile``. For ``renamefile``, the filename for the current filename must match one of the paths on the ``PermitFileControl`` list, and the new filename must be on both the ``PermitFileControl`` and the ``PermitFileWriting`` lists of paths.

   The strings can contain wildcard characters as for the ``filenameforall`` operator and unless specifying a single file, will end with a ``*`` for directories (folders) to allow access to all files and sub-directories in that directory.

   .. note::

      The strings are used for ``stringmatch`` operations similar to ``filenameforall``, thus on MS Windows platforms, use the '``/``' character to separate directories and filenames or use '``\\\\``' to have the string contain '``\\``' which will match a single '``\``' in the target filename (use of '``/``' is strongly recommended).

   The :ref:`SAFER<dSAFER>` mode and the ``.setsafe`` operator set all three lists to empty arrays, thus the only files that can be read are the ``%stdin`` device and on ``LIBPATH`` or ``FONTPATH`` or the ``Resource`` paths specified by the ``/FontResourceDir`` or ``/GenericResourceDir`` system params. Files cannot be opened for writing anywhere and cannot be deleted or renamed except for files created with the :ref:`.tempfile<Tempfile>` operator).


``AlignToPixels <integer>``
   Control sub-pixel positioning of character glyphs (where applicable). A value of 1 specifies alignment of text characters to pixels boundaries. A value of 0 to subpixels where the division factor is set by the device parameter ``TextAlphaBits``. If the latter is 1, the same rendering results regardless of the value of ``AlignToPixels``. The initial value defaults to 1, but this may be overridden by the command line argument ``-dAlignToPixels``.


.. _Language_GridFitTT:

``GridFitTT <integer>``
   Control the use of True Type grid fitting. Ghostscript, by default, uses Freetype for rendering Truetype (and most other) glyphs (but other scaler/renderer libraries can be used), thus has access to a complete Truetype bytecode interpreter.

   This parameter controls the hinting of Truetype glyphs.

   - A value of 0 disables grid fitting for all True Type fonts (not generally recommended).

   - A value of 1 enables the grid fitting using the native Truetype hinting bytecode program(s). Fonts or glyphs with faulty bytecode program(s) will be rendered unhinted.

   - A value 2 is scaler/renderer dependent (generally, if no alternative hinting engine is available this will be equivalent to 1). With the Freetype (our default) this enables Freetype's built-in autohinter.

   - With Freetype, a value of 3 is effectively equivalent to 1.

   This parameter defaults to 1, but this may be overridden on the command line with ``-dGridFitTT=n``.



Miscellaneous additions
---------------------------

Extended semantics of 'run'
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The operator run can take either a string or a file as its argument. In the latter case, it just runs the file, closing it at the end, and trapping errors just as for the string case.

Decoding resources
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

``Decoding`` is a Ghostscript-specific resource category. It contains various resources for emulating PostScript fonts with other font technologies. Instances of the ``Decoding`` category are tables which map PostScript glyph names to character codes used with TrueType, Intellifont, Microtype and other font formats.


Currently Ghostscript is capable of PostScript font emulation in 2 ways :

1. Through :ref:`FAPI<Fonts FAPI>` plugins.
2. With TrueType font files, using the native font renderer, by specifying TrueType font names or files in ``Resource/Init/Fontmap.GS``.


``Decoding`` resources are not currently used by the native font renderer.

An instance of the ``Decoding`` resource category is a dictionary. The dictionary keys are PostScript glyph names and the values are either character codes, or arrays of character codes. Arrays are used when a single name may be mapped to various character codes - in this case Ghostscript tries all alternatives until a success. The name of the resource instance should reflect the character set for which it maps. For example, ``/Unicode`` ``/Decoding`` resource maps to Unicode UTF-16.

The rules for using ``Decoding`` resources in particular cases are specified in the configuration file ``Resource/Init/xlatmap``. See the file itself for more information.

The file format for ``Decoding`` resource files is generic PostScript. Users may want to define custom ``Decoding`` resources. The ``ParseDecoding`` ``procset`` defined in ``Resource/Init/gs_ciddc.ps`` allows representation of the table in a comfortable form.



CIDDecoding resources
~~~~~~~~~~~~~~~~~~~~~~~

``CIDDecoding`` resources are similar to ``Decoding`` resources, except they map ``Character Identifiers (CIDs)`` rather than glyph names. Another difference is that the native Ghostscript font renderer uses ``CIDDecoding`` resources while emulate ``CID`` fonts with TrueType or OpenType fonts.

An instance of the ``CIDDecoding`` resource category is a dictionary of arrays. Keys in the dictionary are integers, which correspond to high order byte of a ``CID``. Values are 256-element arrays, and their indices correspond to the low order byte of a ``CID``. Each elemet of an array is either null, or character code (integer), or an array of character codes (integers). The zero code represents mapping to the default character.

The dictionary includes the additional key ``CIDCount``. Its value is the maximal ``CID`` defined, plus one.

The Ghostscript library is capable of generating some ``CIDDecoding`` instances automatically, using the appropriate ``CMap`` (character map) resources. This covers most of practical cases if the neccessary ``CMap`` resources are provided. See the table ``.CMapChooser`` in ``Resource/Init/gs_ciddc.ps`` for the names of automatically generated resources and associated ``CMaps``. They allow to mapping CNS1, GB1, Japan1, Japan2 and Korea1 CID sets to TrueType character sets known as Unicode (exactly UTF-16), Big5, GB1213, ShiftJIS, Johab and Wansung.

The file format for ``CIDDecoding`` resource file is generic PostScript. Users may want to define custom resources to ``CIDDecoding`` resource category.


GlyphNames2Unicode
~~~~~~~~~~~~~~~~~~~~~

``GlyphNames2Unicode`` is an undocumented dictionary which Adobe PostScript printer driver uses to communicate with Adobe Distiller. In this dictionary the keys are glyph names, the values are Unicode UTF-16 codes for them. The dictionaly is stored in the ``FontInfo`` dictionary under the key ``GlyphNames2Unicode``. Ghostscript recognises it and uses to generate ``ToUnicode CMaps`` with :title:`pdfwrite`.

Multiple Resource directories
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Since 8.10 release Ghostscript maintains multiple resource directories.

Ghostscript does not distinguish ``lib`` and ``Resource`` directories. There is no file name conflicts because ``lib`` does not contain subdirectories, but ``Resource`` always store files in subdirectories.

The search method with multiple resource directories appears not fully conforming to PLRM. We cannot unconditionally call ``ResourceFileName`` while executing ``findresource`` or ``resourcestatus``, ``resourceforall``, because per PLRM it always returns a single path. Therefore Ghostscript implements an extended search method in ``findresource``, ``resourcestatus`` and ``resourceforall``, which first calls ``ResourceFileName`` and checks whether the returned path points to an existing file. If yes, the file is used, otherwise Ghostscript searches all directories specified in ``LIB_PATH``. With a single resource directory it appears conforming to PLRM and equivalent to Adobe implementations.

``ResourceFileName`` may be used for obtaining a path where a resource file to be installed. In this case Ghostscript to be invoked with ``-sGenericResourceDir=path``, specifying an absolute path. The default value for ``GenericResourceDir`` is a relative path. Therefore a default invocation with a PostScript installer will install resource files into ``/gs/Resource``.


Scripting the PDF interpreter
-------------------------------

PostScript functions
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

We have not previously documented the internals of the Ghostscript PDF interpreter, but we have, on occasion, provided solutions that rely upon scripting the interpreter from PostScript. This was possible because the interpreter was written in PostScript.

From release 9.55.0 Ghostscript comes supplied with two PDF interpreters, the original written in PostScript and a brand-new interpreter written in C. While the new interpreter can be run as part of the ``GhostPDL`` family it has also been integrated into Ghostscript, and can be run from the PostScript environment in a similar fashion to the old interpreter. We plan to deprecate, and eventually remove, the old interpreter and carry on with the new one.

Because we have supplied solutions in the past based on the old interpreter, we have had to implement the same capabilities in the integration of the new interpreter. Since this has meant discovering which internal portions were being used, working out how those function, and duplicating them anew, it seemed a good time to document these officially, so that in future the functionality would be available to all.

The following functions existed in the original PDF interpreter and have been replicated for the new interpreter. It should be possible to use these for the forseeable future.


``<file> runpdf -``
   Called from the modified PostScript run operator (which copies ``stdin`` to a temp file if required). Checks for PDF collections, processes all requested pages.

``<file> runpdfbegin -``
   This must be called before performing any further operations. Its exact action depends on which interpreter is being used, but it essentially sets up the environment to process the file as a PDF.

``<int> pdfgetpage <pagedict> | <null>``
   ``int`` is a number from 1 to N indicating the desired page number from the PDF file. Returns the a dictionary containing various informational key/value pairs. If this fails, returns a null object.

``- pdfshowpage_init -``
   In the PostScript PDF interpreter this simply adds 1 to the ``/DSCPageCount`` value in a dictionary. It has no effect in the new PDF interpreter but is maintained for backwards compatibility.

``<pagedict> pdfshowpage_setpage <pagedict>``
   Takes a dictionary as returned from ``pdfgetpage``, extracts various parameters from it, and sets the media size for the page, taking into account the boxes, and requested ``Box``, ``Rotate`` value and ``PDFFitPage``.

``<pagedict> pdfshowpage_finish -``
   Takes a dictionary as returned from ``pdfgetpage``, renders the page content executes ``showpage`` to transfer the rendered content to the device.

``- runpdfend -``
   Terminates the PDF processing, executes restore and various cleanup activities.

``<file> pdfopen <dict>``
   Open a PDF file and read the header, trailer and cross-reference.

``<dict> pdfclose -``
   Terminates processing the original PDF file object. The dictionary parameter should be the one returned from ``pdfopen``.

``<pagedict> pdfshowpage -``
   Takes a dictionary returned from ``pdfgetpage`` and calls the ``pdfshowpage_init``, ``pdfshowpage_setpage``, ``pdfshowpage_finish`` trio to start the page, set up the media and render the page.

``<int> <int> dopdfpages -``
   The integers are the first and last pages to be run from the file. Runs a loop from the fist integer to the last.

   .. note::

      If the current dictionary contains a ``PDFPageList`` array the two values on the stack are ignored and we use the range triples from that array (even/odd, start, end) to determine the pages to process. Page numbers for start and end are ``1..lastpage`` and even/odd is 1 for odd, 2 for even, otherwise 0. Uses ``pdfshowpage`` to actually render the page.

``- runpdfpagerange <int> <int>``
   Processes the PostScript ``/FirstPage``, ``/LastPage`` and ``/PageList`` parameters. These are used together to build an internal array of page numbers to run, which is used by ``dopdfpages`` to actually process the pages if ``PageList`` is present, and a ``FirstPage`` and ``LastPage`` value.

   Despite the name this function does not actually 'run' any pages at all.

   Normal operation simply calls ``runpdf`` with an opened-for-read PostScript file object. The table below shows the normal calling sequence:


   .. list-table::
      :widths: 25 25 25 25
      :header-rows: 1

      * - Function
        - Calls
        - Calls
        - Calls
      * - runpdf
        - runpdfbegin
        - pdfopen
        -
      * -
        - process_trailer_attrs
        -
        -
      * -
        - runpdfpagerange
        -
        -
      * -
        - dopdfpages
        - pdfgetpage
        -
      * -
        -
        - pdfshowpage
        - pdfshowpage_init
      * -
        -
        -
        - pdfshowpage_setpage
      * -
        -
        -
        - pdfshowpage_finish
      * -
        - runpdfend
        - pdfclose
        -

   It is important to get the number of spots and the presence of transparency correct when rendering. Failure to do so will lead to odd output, and potentially crahses. This can be important in situations such as N-up ordering.

   As an example, if we have 2 A4 pages and want to render them side-by-side on A3 media, we might set up the media size to A3, draw the first page contents, translate the origin, draw the second page contents and then render the final content. If the first PDF page did not contain transparency, but the second did, it would be necessary to set ``/PageHasTransparency`` before drawing the first PDF page.


PostScript operators interfacing to the PDF interpreter
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The PostScript functions documented above must somehow interface with the actual PDF interpreter, and this is done using a small number of custom PostScript operators. These operators do not exist in standard PostScript; they are specific to the Ghostscript implementation. These operators are documented here for the benefit of any developers wishing to use them directly.



``dict .PDFInit <PDFContext>``
   Initialises an instance of the PDF interpreter. ``dict`` is an optional dictionary that contains any interpreter-level switches, such as ``PDFDEBUG``, this is used to set the initial state of the PDF interpreter. The return value is a ``PDFContext`` object which is an opaque object to be used with the other PDF operators.

``filename PDFContext .PDFFile -``
   Opens a named file and associates it with the instance of the PDF interpreter. Filename is a string containing a fully qualified path to the PDF file to open, this file must have been made accesible by setting ``--permit-file-read``.

``file PDFContext .PDFStream -``
   Takes an already open (disk-based) file and associates it with the instance of the PDF interpreter.

``PDFcontext .PDFClose -``
   If the context contains an open PDF file which was opened via the ``.PDFfile`` operator, this closes the file. Files associated with the context by the ``.PDFStream`` operator are unaffected. Regardless of the source it then shuts down the PDF interpreter and frees the associated memory.

``PDFContext .PDFInfo dict``
   ``PDFContext`` is a ``PDFContext`` object returned from a previous call to ``.PDFInit``. The returned dictionary contains various key/value pairs with useful file level information:

   .. code-block:: postscript

      /NumPages int
      /Creator string
      /Producer string
      /IsEncrypted boolean


``PDFContext .PDFMetadata -``
   ``PDFContext`` is a ``PDFContext`` object returned from a previous call to ``.PDFInit``. For the benefit of high level devices, this is a replacement for 'process_trailer_attrs' which is a seriously misnamed function now. This function needs to write any required output intents, load and send ``Outlines`` to the device, copy the Author, Creator, Title, Subject and Keywords from the Info dict to the output device, copy Optional Content Properties (``OCProperties``) to the output device. If an AcroForm is present send all its fields and link widget annotations to fields, and finally copy the ``PageLabels``. If we add support for anything else, it will be here too.

``PDFContext int .PDFPageInfo -``
   The integer argument is the page number to retrieve information for. This value starts from zero for the first page. Returns a dictionary with the following key/value pairs:

   .. code-block:: postscript

      /UsesTransparency true|false
      /NumSpots integer containing the number of spot inks on this page
      /MediaBox [llx lly urx ury]
      /HasAnnots true|false

   May also contain (if they are present in the Page dictionary):

   .. code-block:: postscript

      /ArtBox [llx lly urx ury]
      /CropBox [llx lly urx ury]
      /BleedBox [llx lly urx ury]
      /TrimBox [llx lly urx ury]
      /UserUnit int
      /Rotate number

``PDFcontext int .PDFPageInfoExt -``
   As per ``.PDFPageInfo`` above but returns 'Extended' information. This consists of two additional arrays in the returned dictionary:

   .. code-block:: postscript

      /Spots array of names, may be empty
      /Fonts array of dictionaries, one dictionary per font used on the page.


   Each font dictionary contains:

   .. code-block:: postscript

      /BaseFont string containing the name of the font.
      /Subtype string containing the type of the font, as per the PDF Reference.
      /ObjectNum if present, the object number of the font in the file (fonts may be defined inline and have no object number).
      /Embedded boolean indicating if the font's FontDescriptor includes a FontFile and is therefore embedded.

   Type 0 fonts also contain:

   .. code-block:: postscript

      /Descendants an array containing a single font dictionary, contents as above.


``PDFContext int .PDFDrawPage -``
   ``PDFContext`` is a ``PDFContext`` object returned from a previous call to ``.PDFInit``. The integer argument is the page number to be processed. Interprets the page content stream(s) of the specified page using the current graphics state.

``PDFContext int .PDFDrawAnnots -``
   ``PDFContext`` is a ``PDFContext`` object returned from a previous call to ``.PDFInit``. The integer argument is the page number to be processed.

   Renders the Annotations (if any) of the specified page using the current graphics state For correct results, the graphics state when this operator is run should be the same as when PDFDrawPage is executed.

   .. note::

      The ``PDFContext`` object created by ``PDFInit`` must (clearly) have a PDF file associated with it before you can usefully use it. Attempting to use a ``PDFContext`` with any of the processing operators (e.g. ``.PDFDrawPage``) before using either ``.PDFStream`` of ``.PDFFile`` to associate a file with the context will result in an error.




.. External Links

.. _PNG specification: http://www.w3.org/TR/WD-png-960128.html
.. _RFC 1951: http://www.ietf.org/rfc/rfc1951.txt

.. include:: footer.rst
