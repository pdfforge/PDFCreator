.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: The Interface between Ghostscript and Device Drivers

.. include:: header.rst

.. _Drivers.html:
.. _Drivers:


The Interface between Ghostscript and Device Drivers
=======================================================





Adding a driver
----------------------

To add a driver to Ghostscript, first pick a name for your device, say "smurf". (Device names must be 1 to 8 characters, begin with a letter, and consist only of letters, digits, and underscores. Case is significant: all current device names are lower case.) Then all you need do is edit ``contrib.mak`` in two places.

#. The list of devices, in the section headed "Catalog". Add "smurf" to the list.

#. The section headed "Device drivers".

Suppose the files containing the "smurf" driver are called "joe" and "fred". Then you should add the following lines:


.. code-block:: postscript

   # ------ The SMURF device ------ #

   smurf_=$(GLOBJ)joe.$(OBJ) $(GLOBJ)fred.$(OBJ)
   $(DD)smurf.dev: $(smurf_)
           $(SETDEV) $(DD)smurf $(smurf_)

   $(GLOBJ)joe.$(OBJ) : $(GLSRC)joe.c
       $(GLCC) $(GLO_)joe.$(OBJ) $(C_) $(GLSRC)joe.c

   $(GLOBJ)fred.$(OBJ) : $(GLSRC)fred.c
       $(GLCC) $(GLO_)fred.$(OBJ) $(C_) $(GLSRC)fred.c

and whatever ``joe.c`` and ``fred.c`` depend on. If the "smurf" driver also needs special libraries, for instance a library named "gorf", then the entry should look like this:

.. code-block:: postscript

   $(DD)smurf.dev : $(smurf_)
         $(SETDEV) $(DD)smurf $(smurf_)
         $(ADDMOD) $(DD)smurf -lib gorf


If, as will usually be the case, your driver is a printer driver (as :ref:`discussed below<Printer drivers>`), the device entry should look like this:


.. code-block:: postscript

    $(DD)smurf.dev : $(smurf_) $(GLD)page.dev
    $(SETPDEV) $(DD)smurf $(smurf_)

or:

.. code-block:: postscript

    $(DD)smurf.dev : $(smurf_) $(GLD)page.dev
    $(SETPDEV) $(DD)smurf $(smurf_)
    $(ADDMOD) $(DD)smurf -lib gorf

.. note::

   The space before the ``:``, and the explicit compilation rules for the ``.c`` files, are required for portability.


Keeping things simple
------------------------

If you want to add a simple device (specifically, a monochrome printer), you probably don't need to read the rest of this document; just use the code in an existing driver as a guide. The Epson and Canon BubbleJet drivers ``gdevepsn.c`` and ``gdevbj10.c`` are good models for dot-matrix printers, which require presenting the data for many scan lines at once; the DeskJet/LaserJet drivers in ``gdevdjet.c`` are good models for laser printers, which take a single scan line at a time but support data compression. For color printers, there are unfortunately no good models: the two major color inkjet printer drivers, ``gdevcdj.c`` and ``gdevstc.c``, are far too complex to read.

On the other hand, if you're writing a driver for some more esoteric device, you probably do need at least some of the information in the rest of this document. It might be a good idea for you to read it in conjunction with one of the existing drivers.

Duplication of code, and sheer volume of code, is a serious maintenance and distribution problem for Ghostscript. If your device is similar to an existing one, try to implement your driver by adding some parameterization to an existing driver rather than by copying code to create an entirely new source module. ``gdevepsn.c`` and ``gdevdjet.c`` are good examples of this approach.


Driver structure
------------------------

A device is represented by a structure divided into three parts:

#. Parameters that are present in all devices but may be different for each device or instance.

#. An ``initialize_device_procs`` procedure.

#. Device-specific parameters that may be different for each instance.

A prototype of the parameter structure (including both generic and device-specific parameters) is defined and initialized at compile time, but is copied and filled in when an instance of the device is created. This structure should be declared as const, but for backward compatibility reasons it is not.

The ``gx_device_common`` macro defines the common structure elements, with the intent that devices define and export a structure along the following lines. Do not fill in the individual generic parameter values in the usual way for C structures: use the macros defined for this purpose in ``gxdevice.h`` or, if applicable, ``gdevprn.h``.


.. code-block:: c

   typedef struct smurf_device_s {
           gx_device_common;
           ... device-specific parameters ...
   } smurf_device;
   smurf_device gs_smurf_device = {
           ... macro for generic parameter values ...,
           initialize_device_procs,
           ... device-specific parameter values if any ...
   };

The device structure instance must have the name ``gs_smurf_device``, where "smurf" is the device name used in ``contrib.mak``. ``gx_device_common`` is a macro consisting only of the element definitions.

The ``initialize_device_procs`` function pointer is called when the device is created. Its sole job is to initialize the entries in the device procedure table. On entry, the device procedure table will be full of NULL pointers. On exit, any NULLs left in the table will be filled in with pointers to the default routines. Therefore, the routine should set any non-default entries itself.

Devices that are (in object-oriented terms) derived from 'base' classes (for instance a new printer device that derives from the ``prn`` device) can call provided helper functions for setting the standard functions for that base class.

For example, if the "smurf" device was a printer device, its ``initialize_device_procs`` procedure might look like:

.. code-block:: c

   static void smurf_initialize_device_procs(gx_device *dev) {
     /* We are derived from a prn device, and can print in the background */
     gdev_prn_initialize_bg(dev);

     /* Override functions for our specifics */
     set_dev_proc(dev, map_color_rgb, smurf_map_color_rgb);
     set_dev_proc(dev, map_rgb_color, smurf_map_rgb_color);
     ...
   }

The initialize procedure function pointer does not live in the in the device procedure table (and as such is statically initialized at compile time). Nonetheless, we refer to this as being a device procedure in the rest of the discussion here.

Note that the ``initialize_device_procs`` function may be called with a pointer to a ``gx_device`` rather than to the derived device class. This happens frequently when one device wants to obtain the prototype of another to copy device procedures around. Initialization of items in the device other than device procs should therefore be reserved for the ``initialize_device`` device procedure.

The use of the initialize procedure is new to Ghostscript 9.55. Previous versions used a statically initialized table of device procedures. We changed to a dynamically initialized system to more easily cope with future changes to the device procedures.

All the device procedures are called with the device as the first argument. Since each device type is actually a different structure type, the device procedures must be declared as taking a ``gx_device *`` as their first argument, and must cast it to ``smurf_device *`` internally. For example, in the code for the "memory" device, the first argument to all routines is called dev, but the routines actually use mdev to refer to elements of the full structure, using the following standard initialization statement at the beginning of each procedure:

.. code-block:: c

   gx_memory_device *const mdev = (gx_device_memory *)dev;

(This is a cheap version of "object-oriented" programming: in C++, for example, the cast would be unnecessary, and in fact the procedure table would be constructed by the compiler.)

Structure definition
~~~~~~~~~~~~~~~~~~~~~~~~~

You should consult the definition of struct ``gx_device_s`` in ``gxdevice.h`` for the complete details of the generic device structure. Some of the most important members of this structure for ordinary drivers are:

.. list-table::
   :widths: 50 50

   * - ``const char *dname;``
     - The device name
   * - ``bool is_open;``
     - True if device has been opened
   * - ``gx_device_color_info color_info;``
     - Color information
   * - ``int width;``
     - Width in pixels
   * - ``int height;``
     - Height in pixels

The name in the structure (``dname``) should be the same as the name in ``contrib.mak``.


For sophisticated developers only
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

If for any reason you need to change the definition of the basic device structure, or to add procedures, you must change the following places:

- This document and the :ref:`news document<News.html>` (if you want to keep the documentation up to date).

- The definition of ``gx_device_common`` and the procedures in ``gxdevcli.h``.

- Possibly, the default forwarding procedures declared in ``gxdevice.h`` and implemented in ``gdevnfwd.c``.

- The device procedure record completion routines in ``gdevdflt.c``.

- Possibly, the default device implementation in ``gdevdflt.c``, ``gdevddrw.c``, and ``gxcmap.c``.

- The bounding box device in ``gdevbbox.c`` (probably just adding NULL procedure entries if the new procedures don't produce output).

- These devices that must have complete (non-defaulted) procedure vectors:

   - The null device in ``gdevnfwd.c``.

   - The command list "device" in ``gxclist.c``. This is not an actual device; it only defines procedures.

   - The "memory" devices in ``gdevmem.h`` and ``gdevm*.c``.

- The clip list accumulation "device" in ``gxacpath.c``.

- The clipping "devices" ``gxclip.c``, ``gxclip2.c``, and ``gxclipm.c``.

- The pattern accumulation "device" in ``gxpcmap.c``.

- The hit detection "device" in ``gdevhit.c``.

- The generic printer device macros in ``gdevprn.h``.

- The generic printer device code in ``gdevprn.c``.

- The ``RasterOp`` source device in ``gdevrops.c``.


You may also have to change the code for ``gx_default_get_params`` or ``gx_default_put_params`` in ``gsdparam.c``.

You should not have to change any of the real devices in the standard Ghostscript distribution (listed in ``devs.mak`` and ``contrib.mak``) or any of your own devices, because all of them are supposed to use the macros in ``gxdevice.h`` or ``gdevprn.h`` to define and initialize their state.


Coordinates and types
-----------------------

Coordinate system
~~~~~~~~~~~~~~~~~~

Since each driver specifies the initial transformation from user coordinates to device coordinates, the driver can use any coordinate system it wants, as long as a device coordinate will fit in an int. (This is only an issue on DOS systems, where ints are only 16 bits. User coordinates are represented as floats.) Most current drivers use a coordinate system with (0,0) in the upper left corner, with X increasing to the right and Y increasing toward the bottom. However, there is supposed to be nothing in the rest of Ghostscript that assumes this, and indeed some drivers use a coordinate system with (0,0) in the lower left corner.

Drivers must check (and, if necessary, clip) the coordinate parameters given to them: they should not assume the coordinates will be in bounds. The ``fit_fill`` and ``fit_copy`` macros in ``gxdevice.h`` are very helpful in doing this.

Color definition
~~~~~~~~~~~~~~~~~~

Between the Ghostscript graphics library and the device, colors are represented in three forms. Color components in a color space (Gray, RGB, DeviceN, etc.) represented as frac values. Device colorants are represented as ``gx_color_value`` values. For many procedures, colors are represented in a type called ``gx_color_index``. All three types are described in more detail in `Types`_.

The ``color_info`` member of the device structure defines the color and gray-scale capabilities of the device. Its type is defined as follows:


.. code-block:: c

   /*
    * The enlarged color model information structure: Some of the
    * information that was implicit in the component number in
    * the earlier conventions (component names, polarity, mapping
    * functions) are now explicitly provided.
    *
    * Also included is some information regarding the encoding of
    * color information into gx_color_index. Some of this information
    * was previously gathered indirectly from the mapping
    * functions in the existing code, specifically to speed up the
    * halftoned color rendering operator (see
    * gx_dc_ht_colored_fill_rectangle in gxcht.c). The information
    * is now provided explicitly because such optimizations are
    * more critical when the number of color components is large.
    *
    * Note: no pointers have been added to this structure, so there
    *       is no requirement for a structure descriptor.
    */
   typedef struct gx_device_color_info_s {

       /*
        * max_components is the maximum number of components for all
        * color models supported by this device. This does not include
        * any alpha components.
        */
       int max_components;

       /*
        * The number of color components. This does not include any
        * alpha-channel information, which may be integrated into
        * the gx_color_index but is otherwise passed as a separate
        * component.
        */
       int num_components;

       /*
        * Polarity of the components of the color space, either
        * additive or subtractive. This is used to interpret transfer
        * functions and halftone threshold arrays. Possible values
        * are GX_CM_POLARITY_ADDITIVE or GX_CM_POLARITY_SUBTRACTIVE
        */
       gx_color_polarity_t polarity;

       /*
        * The number of bits of gx_color_index actually used.
        * This must be <= sizeof(gx_color_index), which is usually 64.
        */
       byte depth;

       /*
        * Index of the gray color component, if any. The max_gray and
        * dither_gray values apply to this component only; all other
        * components use the max_color and dither_color values.
        *
        * This will be GX_CINFO_COMP_NO_INDEX if there is no gray
        * component.
        */
       byte gray_index;

       /*
        * max_gray and max_color are the number of distinct native
        * intensity levels, less 1, for the gray and all other color
        * components, respectively. For nearly all current devices
        * that support both gray and non-gray components, the two
        * parameters have the same value.
        *
        * dither_grays and dither_colors are the number of intensity
        * levels between which halftoning can occur, for the gray and
        * all other color components, respectively. This is
        * essentially redundant information: in all reasonable cases,
        * dither_grays = max_gray + 1 and dither_colors = max_color + 1.
        * These parameters are, however, extensively used in the
        * current code, and thus have been retained.
        *
        * Note that the non-gray values may now be relevant even if
        * num_components == 1. This simplifies the handling of devices
        * with configurable color models which may be set for a single
        * non-gray color model.
        */
       gx_color_value max_gray;    /* # of distinct color levels -1 */
       gx_color_value max_color;

       gx_color_value dither_grays;
       gx_color_value dither_colors;

       /*
        * Information to control super-sampling of objects to support
        * anti-aliasing.
        */
       gx_device_anti_alias_info anti_alias;

       /*
        * Flag to indicate if gx_color_index for this device may be divided
        * into individual fields for each component. This is almost always
        * the case for printers, and is the case for most modern displays
        * as well. When this is the case, halftoning may be performed
        * separately for each component, which greatly simplifies processing
        * when the number of color components is large.
        *
        * If the gx_color_index is separable in this manner, the comp_shift
        * array provides the location of the low-order bit for each
        * component. This may be filled in by the client, but need not be.
        * If it is not provided, it will be calculated based on the values
        * in the max_gray and max_color fields as follows:
        *
        *     comp_shift[num_components - 1] = 0,
        *     comp_shift[i] = comp_shift[i + 1]
        *                      + ( i == gray_index ? ceil(log2(max_gray + 1))
        *                                          : ceil(log2(max_color + 1)) )
        *
        * The comp_mask and comp_bits fields should be left empty by the client.
        * They will be filled in during initialization using the following
        * mechanism:
        *
        *     comp_bits[i] = ( i == gray_index ? ceil(log2(max_gray + 1))
        *                                      : ceil(log2(max_color + 1)) )
        *
        *     comp_mask[i] = (((gx_color_index)1 << comp_bits[i]) - 1)
        *                       << comp_shift[i]
        *
        * (For current devices, it is almost always the case that
        * max_gray == max_color, if the color model contains both gray and
        * non-gray components.)
        *
        * If separable_and_linear is not set, the data in the other fields
        * is unpredictable and should be ignored.
        */
       gx_color_enc_sep_lin_t separable_and_linear;
       byte                   comp_shift[GX_DEVICE_COLOR_MAX_COMPONENTS];
       byte                   comp_bits[GX_DEVICE_COLOR_MAX_COMPONENTS];
       gx_color_index         comp_mask[GX_DEVICE_COLOR_MAX_COMPONENTS];
       /*
        * Pointer to name for the process color model.
        */
       const char * cm_name;

   } gx_device_color_info;


.. note ::

   See `Changing color_info data`_ before changing any information in the ``color_info`` structure for a device.

It is recommended that the values for this structure be defined using one of the standard macros provided for this purpose. This allows for future changes to be made to the structure without changes being required in the actual device code.

The following macros (in ``gxdevcli.h``) provide convenient shorthands for initializing this structure for ordinary black-and-white or color devices:


.. code-block:: c

   #define dci_black_and_white ...
   #define dci_color(depth,maxv,dither) ...


The ``#define dci_black_and_white`` macro defines a single bit monochrome device (For example: a typical monochrome printer device.)

The ``#define dci_color(depth,maxv,dither)`` macro can be used to define a 24 bit RGB device or a 4 or 32 bit CMYK device.

The ``#define dci_extended_alpha_values`` macro (in ``gxdevcli.h``) specifies most of the current fields in the structure. However this macro allows only the default setting for the ``comp_shift``, ``comp_bits``, and ``comp_mask`` fields to be set. Any device which requires a non-default setting for these fields has to correctly these fields during the device open procedure. See `Separable and linear fields`_ and `Changing color_info data`_.

The idea is that a device has a certain number of gray levels (``max_gray+1``) and a certain number of colors (``max_rgb+1``) that it can produce directly. When Ghostscript wants to render a given color space color value as a device color, it first tests whether the color is a gray level and if so:

   If ``max_gray`` is large (``>= 31``), Ghostscript asks the device to approximate the gray level directly. If the device returns a valid ``gx_color_index``, Ghostscript uses it. Otherwise, Ghostscript assumes that the device can represent ``dither_gray`` distinct gray levels, equally spaced along the diagonal of the color cube, and uses the two nearest ones to the desired color for halftoning.

If the color is not a gray level:

   If ``max_rgb`` is large (``>= 31``), Ghostscript asks the device to approximate the color directly. If the device returns a valid ``gx_color_index``, Ghostscript uses it. Otherwise, Ghostscript assumes that the device can represent distinct colors, equally spaced throughout the color cube, and uses two of the nearest ones to the desired color for halftoning.


Separable and linear fields
""""""""""""""""""""""""""""""

The three fields ``comp_shift``, ``comp_bits``, and ``comp_mask`` are only used if the ``separable_and_linear`` field is set to ``GX_CINFO_SEP_LIN``. In this situation a ``gx_color_index`` value must represent a combination created by or'ing bits for each of the devices's output colorants. The ``comp_shift`` array defines the location (shift count) of each colorants bits in the output ``gx_color_index`` value. The ``comp_bits`` array defines the number of bits for each colorant. The ``comp_mask`` array contains a mask which can be used to isolate the bits for each colorant. These fields must be set if the device supports more than four colorants.

Changing color_info data
""""""""""""""""""""""""""""""

For most devices, the information in the device's ``color_info`` structure is defined by the various device definition macros and the data remains constant during the entire existence of the device. In general the Ghostscript graphics assumes that the information is constant. However some devices want to modify the data in this structure.

The device's ``put_params`` procedure may change ``color_info`` field values. After the data has been modified then the device should be closed (via a call to ``gs_closedevice``). Closing the device will erase the current page so these changes should only be made before anything has been drawn on a page.

The device's ``open_device`` procedure may change ``color_info`` field values. These changes should be done before any other procedures are called.

The Ghostscript graphics library uses some of the data in ``color_info`` to set the default procedures for the ``get_color_mapping_procs``, ``get_color_comp_index``, ``encode_color``, and ``decode_color`` procedures. These default procedures are set when the device is originally created. If any changes are made to the ``color_info`` fields then the device's ``open_device`` procedure has responsibility for insuring that the correct procedures are contained in the device structure. (For an example, see the display device open procedure ``display_open`` and its subroutine ``display_set_color_format`` (in ``gdevdsp.c``).



Types
~~~~~~~~~


Here is a brief explanation of the various types that appear as parameters or results of the drivers.

``frac`` (defined in ``gxfrac.h``)
   This is the type used to represent color values for the input to the color model mapping procedures. It is currently defined as a short. It has a range of ``frac_0`` to ``frac_1``.

``gx_color_value`` (defined in ``gxdevice.h``)
   This is the type used to represent RGB or CMYK color values. It is currently equivalent to unsigned short. However, Ghostscript may use less than the full range of the type to represent color values: ``gx_color_value_bits`` is the number of bits actually used, and ``gx_max_color_value`` is the maximum value, equal to ``(2^gx_max_color_value_bits)-1``.

``gx_device`` (defined in ``gxdevice.h``)
   This is the device structure, as explained above.

``gs_matrix`` (defined in ``gsmatrix.h``)
   This is a 2-D homogeneous coordinate transformation matrix, used by many Ghostscript operators.

``gx_color_index`` (defined in ``gxcindex.h``)
   This is meant to be whatever the driver uses to represent a device color. For example, it might be an index in a color map, or it might be R, G, and B values packed into a single integer. The Ghostscript graphics library gets ``gx_color_index`` values from the device's ``encode_color`` and hands them back as arguments to several other procedures. If the ``separable_and_linear`` field in the device's ``color_info`` structure is not set to ``GX_CINFO_SEP_LIN`` then Ghostscript does not do any computations with ``gx_color_index`` values.

   The special value ``gx_no_color_index`` (defined as ``(~(gx_color_index)(0))`` ) means "transparent" for some of the procedures.

   The size of ``gx_color_index`` can be either 32 or 64 bits. The choice depends upon the architecture of the CPU and the compiler. The default type definition is simply:

   .. code-block:: c

      typedef unsigned long gx_color_index;


   However if ``GX_COLOR_INDEX_TYPE`` is defined, then it is used as the type for ``gx_color_index``.

   .. code-block:: c

      typedef GX_COLOR_INDEX_TYPE gx_color_index;

   The smaller size (32 bits) may produce more efficient or faster executing code. The larger size (64 bits) is needed for representing either more bits per component or more components. An example of the later case is a device that supports 8 bit contone colorants using a ``DeviceCMYK`` process color model with its four colorants and also supports additional spot colorants.

   Currently ``autoconf`` attempts to find a 64 bit type definition for the compiler being used, and if a 64 bit type is found then ``GX_COLOR_INDEX_TYPE`` is set to the type.

   For Microsoft and the MSVC compiler, ``GX_COLOR_INDEX_TYPE`` will be set to unsigned ``_int64`` if ``USE_LARGE_COLOR_INDEX`` is set to 1 either on the make command line or by editing the definition in ``msvc32.mak``.


``gs_param_list`` (defined in gsparam.h)
   This is a parameter list, which is used to read and set attributes in a device. See the comments in ``gsparam.h``, and the description of the ``get_params`` and ``put_params`` procedures below, for more detail.

``gx_tile_bitmap`` (defined in ``gxbitmap.h``)

``gx_strip_bitmap`` (defined in ``gxbitmap.h``)
   These structure types represent bitmaps to be used as a tile for filling a region (rectangle). ``gx_tile_bitmap`` is an older, deprecated type lacking shift and ``rep_shift``; ``gx_strip_bitmap`` has superseded it, and should be used in new code. Here is a copy of the relevant part of the file:

.. code-block:: c

   /*
    * Structure for describing stored bitmaps.
    * Bitmaps are stored bit-big-endian (i.e., the 2^7 bit of the first
    * byte corresponds to x=0), as a sequence of bytes (i.e., you can't
    * do word-oriented operations on them if you're on a little-endian
    * platform like the Intel 80x86 or VAX).  Each scan line must start on
    * a (32-bit) word boundary, and hence is padded to a word boundary,
    * although this should rarely be of concern, since the raster and width
    * are specified individually.  The first scan line corresponds to y=0
    * in whatever coordinate system is relevant.
    *
    * For bitmaps used as halftone tiles, we may replicate the tile in
    * X and/or Y, but it is still valuable to know the true tile dimensions
    * (i.e., the dimensions prior to replication).  Requirements:
    *      width % rep_width = 0
    *      height % rep_height = 0
    *
    * For halftones at arbitrary angles, we provide for storing the halftone
    * data as a strip that must be shifted in X for different values of Y.
    * For an ordinary (non-shifted) halftone that has a repetition width of
    * W and a repetition height of H, the pixel at coordinate (X,Y)
    * corresponds to halftone pixel (X mod W, Y mod H), ignoring phase;
    * for a shifted halftone with shift S, the pixel at (X,Y) corresponds
    * to halftone pixel ((X + S * floor(Y/H)) mod W, Y mod H).    In other words,
    * each Y increment of H shifts the strip left by S pixels.
    *
    * As for non-shifted tiles, a strip bitmap may include multiple copies
    * in X or Y to reduce loop overhead.  In this case, we must distinguish:
    *      - The height of an individual strip, which is the same as
    *      the height of the bitmap being replicated (rep_height, H);
    *      - The height of the entire bitmap (size.y).
    * Similarly, we must distinguish:
    *      - The shift per strip (rep_shift, S);
    *      - The shift for the entire bitmap (shift).
    * Note that shift = (rep_shift * size.y / rep_height) mod rep_width,
    * so the shift member of the structure is only an accelerator.  It is,
    * however, an important one, since it indicates whether the overall
    * bitmap requires shifting or not.
    *
    * Note that for shifted tiles, size.y is the size of the stored bitmap
    * (1 or more strips), and NOT the height of the actual tile.  The latter
    * is not stored in the structure at all: it can be computed as H * W /
    * gcd(S, W).
    *
    * If the bitmap consists of a multiple of W / gcd(S, W) copies in Y, the
    * effective shift is zero, reducing it to a tile.  For simplicity, we
    * require that if shift is non-zero, the bitmap height be less than H * W /
    * gcd(S, W).  I.e., we don't allow strip bitmaps that are large enough to
    * include a complete tile but that don't include an integral number of
    * tiles.  Requirements:
    *      rep_shift < rep_width
    *      shift = (rep_shift * (size.y / rep_height)) % rep_width
    *
    * For the benefit of the planar device, we now have a num_planes field.
    * For chunky data this should be set to 1. For planar data, the data pointer
    * points to the first plane of data; subsequent planes of data follow
    * immediately after this as if there were num_planes * height lines of data.
    */
   typedef struct gx_strip_bitmap_s {
           byte *data;
           int raster;                     /* bytes per scan line */
           gs_int_point size;              /* width, height */
           gx_bitmap_id id;
           ushort rep_width, rep_height;   /* true size of tile */
           ushort rep_shift;
           ushort shift;
           int num_planes;
   } gx_strip_bitmap;


Coding conventions
----------------------------

All the driver procedures defined below that return ``int`` results return 0 on success, or an appropriate negative error code in the case of error conditions. The error codes are defined in ``gserrors.h``; they correspond directly to the errors defined in the PostScript language reference manuals. The most common ones for drivers are:

``gs_error_invalidfileaccess``
   An attempt to open a file failed.

``gs_error_ioerror``
   An error occurred in reading or writing a file.

``gs_error_limitcheck``
   An otherwise valid parameter value was too large for the implementation.

``gs_error_rangecheck``
   A parameter was outside the valid range.

``gs_error_VMerror``
   An attempt to allocate memory failed. (If this happens, the procedure should release all memory it allocated before it returns.)

If a driver does return an error, rather than a simple return statement it should use the ``return_error`` macro defined in ``gx.h``, which is automatically included by ``gdevprn.h`` but not by ``gserrors.h``. For example:

.. code-block:: c

   return_error(gs_error_VMerror);


Allocating storage
~~~~~~~~~~~~~~~~~~~~~~~~

While most drivers (especially printer drivers) follow a very similar template, there is one important coding convention that is not obvious from reading the code for existing drivers: driver procedures must not use ``malloc`` to allocate any storage that stays around after the procedure returns. Instead, they must use ``gs_malloc`` and ``gs_free``, which have slightly different calling conventions. (The prototypes for these are in ``gsmemory.h``, which is included in ``gx.h``, which is included in ``gdevprn.h``.) This is necessary so that Ghostscript can clean up all allocated memory before exiting, which is essential in environments that provide only single-address-space multi-tasking (some versions of Microsoft Windows).

.. code-block:: c

   char *gs_malloc(uint num_elements, uint element_size, const char *client_name);

Like ``calloc``, but unlike ``malloc``, ``gs_malloc`` takes an element count and an element size. For structures, ``num_elements`` is 1 and ``element_size`` is ``sizeof`` the structure; for byte arrays, ``num_elements`` is the number of bytes and ``element_size`` is 1. Unlike ``calloc``, ``gs_malloc`` does not clear the block of storage.

The ``client_name`` is used for tracing and debugging. It must be a real string, not ``NULL``. Normally it is the name of the procedure in which the call occurs.

.. code-block:: c

   void gs_free(char *data, uint num_elements, uint element_size, const char *client_name);


Unlike ``free``, ``gs_free`` demands that ``num_elements`` and ``element_size`` be supplied. It also requires a client name, like ``gs_malloc``.


Driver instance allocation
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

All driver instances allocated by Ghostscript's standard allocator must point to a "structure descriptor" that tells the garbage collector how to trace pointers in the structure. For drivers registered in the normal way (using the ``makefile`` approach described above), no special care is needed as long as instances are created only by calling the ``gs_copydevice`` procedure defined in ``gsdevice.h``. If you have a need to define devices that are not registered in this way, you must fill in the ``stype`` member in any dynamically allocated instances with a pointer to the same structure descriptor used to allocate the instance. For more information about structure descriptors, see ``gsmemory.h`` and ``gsstruct.h``.




Printer drivers
---------------------

Printer drivers (which include drivers that write some kind of raster file) are especially simple to implement. The printer driver must implement a ``print_page`` or ``print_page_copies`` procedure. There are macros in ``gdevprn.h`` that generate the device structure for such devices, of which the simplest is ``prn_device``; for an example, see ``gdevbj10.c``. If you are writing a printer driver, we suggest you start by reading ``gdevprn.h`` and the subsection on `Color mapping`_ below; you may be able to ignore all the rest of the driver procedures.

The ``print_page`` procedures are defined as follows:

.. code-block:: c

   int (*print_page)(gx_device_printer *, FILE *)
   int (*print_page_copies)(gx_device_printer *, FILE *, int)

This procedure must read out the rendered image from the device and write whatever is appropriate to the file. To read back one or more scan lines of the image, the ``print_page`` procedure must call one of several procedures. Traditionally devices have called ``gdev_prn_copy_scan_lines``, ``gdev_prn_get_bits``, or the generic ``get_bits_rectangle`` device entry point. Alternatively devices may now call the new ``process_page`` entrypoint, which can have significant performance advantages in :ref:`multi-threaded<Printer drivers mt>` situations.

.. code-block:: c

   int gdev_prn_copy_scan_lines(gx_device_printer *pdev, int y, byte *str, uint size)

For this procedure, ``str`` is where the data should be copied to, and ``size`` is the size of the buffer starting at ``str``. This procedure returns the number of scan lines copied, or ``<0`` for an error. ``str`` need not be aligned.

.. code-block:: c

   int gdev_prn_get_bits(gx_device_printer *pdev, int y, byte *str, byte **actual_data)


This procedure reads out exactly one scan line. If the scan line is available in the correct format already, ``*actual_data`` is set to point to it; otherwise, the scan line is copied to the buffer starting at str, and ``*actual_data`` is set to ``str``. This saves a copying step most of the time. ``str`` need not be aligned; however, if ``*actual_data`` is set to point to an existing scan line, it will be aligned. (See the description of the ``get_bits`` procedure below for more details.)

In either of these two cases, each row of the image is stored in the form described in the comment under ``gx_tile_bitmap`` above; each pixel takes the number of bits specified as ``color_info.depth`` in the device structure, and holds values returned by the device's ``encode_color`` procedure.

The ``print_page`` procedure can determine the number of bytes required to hold a scan line by calling:


.. code-block:: c

   uint gdev_prn_raster(gx_device_printer *)


For a very simple concrete example of this pattern of use, we suggest reading the code in ``bit_print_page`` in ``gdevbit.c``.

If the device provides ``print_page``, Ghostscript will call ``print_page`` the requisite number of times to print the desired number of copies; if the device provides ``print_page_copies``, Ghostscript will call ``print_page_copies`` once per page, passing it the desired number of copies.


.. _Printer drivers mt:

Printer drivers (Multi-threaded)
----------------------------------------

This interface is new, and subject to change without notice.

Ghostscript has supported multi-threaded rendering (controlled by the ``-dNumRenderingThreads`` command line option) since version 8.64. This uses multiple threads of execution to accelerate the rendering phase of operations, but driver specific operations (such as compression) have not been able to benefit in the same way.

As from Ghostscript 9.11 onwards, a new device function, ``process_page`` has been introduced to solve this. A printer driver will be called via the ``print_page`` or ``print_page_copies`` entry point as before, but rather than requesting a rectangle of pixels at a time (by calling ``get_bits_rectangle``), the driver can now invite Ghostscript to "process the page" in whatever sized chunks it chooses.

While the benefits of ``process_page`` come from its use with multiple rendering threads, it will work perfectly well in single threaded mode too. Devices do not need to implement both schemes.

.. code-block:: c

   int (*process_page)(gx_device *dev, gx_process_page_options_t *options)

The device should fill out a ``gx_process_page_options_t`` structure and pass the address of this to the ``process_page`` function. The entries within this structure will control exactly how Ghostscript will process the page. For forwards compatibility devices should ensure that any unknown fields/option bits within the structure are initialised to 0.

.. code-block:: c

   typedef struct gx_process_page_options_s gx_process_page_options_t;

   struct gx_process_page_options_s
   {
       int (*init_buffer_fn)(void *arg, gx_device *dev, gs_memory_t *memory, int w, int h, void **buffer);
       void (*free_buffer_fn)(void *arg, gx_device *dev, gs_memory_t *memory, void *buffer);
       int (*process_fn)(void *arg, gx_device *dev, gx_device *bdev, const gs_int_rect *rect, void *buffer);
       int (*output_fn)(void *arg, gx_device *dev, void *buffer);
       void *arg;
       int options; /* A mask of GX_PROCPAGE_... options bits */
   };


Ghostscript is free to process the page in 1 or more sections. The potential benefits of ``process_page`` come when Ghostscript chooses to use more than 1 section (or "band") and shares the job of rendering these bands between a set of rendering threads. The overall scheme of operation is as follows:

- Ghostscript will call ``init_buffer_fn`` in turn, once for each rendering thread in use. This function should (as far as possible) allocate any buffering that may be required to cope with a band of the given size.

- For each band rendered, Ghostscript will call process_fn to process the given rectangle of the page into the buffer. To achieve this ``process_fn`` is passed a buffer device that contains the rendered version of that rectangle (with the ``y`` range adjusted to start from 0). ``process_fn`` should call ``get_bits_rectangle`` as usual to extract the rendered area. If the options to this call are set correctly (using ``GB_RETURN_POINTER``) no copying or additional storage will be required. All the calls to ``process_fn`` will be for non-overlapping rectangles that cover the page, hence ``process_fn`` may overwrite the storage used in the returned buffer device as part of the processing. Several calls to ``process_fn`` may take place simultaneously in different threads, and there is no guarantee that they will happen 'in order'.

- Ghostscript will call output_fn for each band in turn, passing in the processed buffer containing the output of the ``process_fn`` stage. These calls are guaranteed to happen 'in order', and will be interleaved arbitrarily with the ``process_fn`` calls. Once an ``output_fn`` returns, the buffer may instantly be reused for another ``process_fn`` calls.

- Once the page has been processed, Ghostscript will call ``free_buffer_fn`` for each allocated buffer to allow the device to clean up.


At the time of writing the only defined bit in the options word is ``GX_PROCPAGE_BOTTOM_UP`` which signifies that Ghostscript should render bands from the bottom of the page to the top, rather than the default top to bottom.

The height of the bands used by Ghostscript when rendering the page can either be specified by the device itself (using the ``band_params`` structure), or can be calculated by Ghostscript based upon the space available to it. It can sometimes be much easier/more efficient to code a device if the band height can be relied upon to take only particular values - for instance, a device that downscales its output will prefer the height to be a multiple of the downscale used, or a device that uses DCT based compression may prefer a multiple of 8.

To accommodate such needs, before Ghostscript sets up its buffers, it will perform a ``gxdso_adjust_bandheight`` call. A device can catch this call to adjust the calculated band height to a value it would prefer. To avoid invalidating the calculated memory bounds this should generally be a 'fine' adjustment, and always downwards.

A simple example of how to use ``process_page`` may be found as the :title:`fpng` device. Within this device:

- The ``init_buffer_fn`` allocates a buffer large enough to hold the compressed version of each band.

- The ``process_fn`` applies the sub/paeth filters to the buffered image, then compresses each band with ``zlib``.

- The ``output_fn`` simply writes each compressed buffer to the file.

- The ``free_buffer_fn`` frees the buffers.

- In addition, the downscaler is called to demonstrate that it is possible to 'chain' ``process_page`` functions.


The :title:`fpng` device is broadly equivalent to the :title:`png16m` device, but performs much better when multiple threads are in use. Compression is potentially worse than with :title:`png16m` due to each band being compressed separately.

While the ``print_page`` entry point is specific to printer devices, the process_page device entry point is not. It will, however, only be useful for devices that involve rendering the page. As such, neither ``-dNumRenderingThreads`` or ``process_page`` will help accelerate devices such as :title:`pdfwrite` or :title:`ps2write`.


Driver procedures
------------------------

Most of the procedures that a driver may implement are optional. If a device doesn't supply an optional procedure ``WXYZ``, the entry in the procedure structure may be either ``gx_default_WXYZ``, for instance ``gx_default_strip_tile_rectangle``, or ``NULL`` or 0. (The device procedure must also call the ``gx_default_`` procedure if it doesn't implement the function for particular values of the arguments.) Since, by construction, device procedure entries are set to 0 at creation time, ones that are not explicitly initialised will continue to work even if new (optional) members are added.

Life cycle
~~~~~~~~~~~~~

When a device is first created, it will have an empty device procs table. The system will call the device's ``initialize_device_procs`` function pointer to fill out this table. This operation can never fail.

.. note::

   This operation is also used for creating temporary 'prototype' devices for copying device procedures from.


A device instance begins life in a closed state. In this state, no output operations will occur. Only the following procedures may be called:

``initialize_device``

``open_device``

``get_initial_matrix``

``get_params``

``put_params``

``get_hardware_params``


When ``setdevice`` installs a device instance in the graphics state, it checks whether the instance is closed or open. If the instance is closed, ``setdevice`` calls the open routine, and then sets the state to open.

There is no user-accessible operation to close a device instance. This is not an oversight – it is required in order to enforce the following invariant:

If a device instance is the current device in any graphics state, it must be open (have ``is_open`` set to true).

Device instances are only closed when they are about to be freed, which occurs in three situations:

- When a ``restore`` occurs, if the instance was created since the corresponding save and is in a VM being restored. I.e., if the instance was created in local VM since a ``save``, it will always be closed and freed by the corresponding ``restore``; if it was created in global VM, it will only be closed by the outermost ``restore``, regardless of the ``save`` level at the time the instance was created.

- By the garbage collector, if the instance is no longer accessible.

- When Ghostscript exits (terminates).


Open, close, sync, copy
~~~~~~~~~~~~~~~~~~~~~~~~~~~

``void (*initialize_device_procs)(gx_device *dev)``
   Called once a new device instance has been created. The function should initialize the device procedure tables. This cannot fail. NOTE: Clients should rarely need to call a device's ``initialize_device_procs`` procedure: this procedure is mostly used by the internal device creation code. The sole exception here is when a device implementation wishes to copy device function pointers from another device; then a blank ``gx_device`` can be created, and ``initialize_device_procs`` can be used to fill out the device procs table so it can be copied from.

``int (*initialize_device)(gx_device *dev) [OPTIONAL]``
   Called once a new device instance has been created and the device procs table has been initialized. This function should perform the minimum initialization to any internal device state required. If the initial setup fails, this procedure should return an error; the new instance will be freed.

   .. note ::
      Clients should never call a device's ``initialize_device`` procedure: this procedure is only intended for use by the internal device creation code.

``int (*open_device)(gx_device *) [OPTIONAL]``
   Open the device: do any initialization associated with making the device instance valid. This must be done before any output to the device. The default implementation does nothing.

   .. note ::
      Clients should never call a device's ``open_device`` procedure directly: they should always call ``gs_opendevice`` instead.

``void (*get_initial_matrix)(gx_device *, gs_matrix *) [OPTIONAL]``
   Construct the initial transformation matrix mapping user coordinates (nominally 1/72 inch per unit) to device coordinates. The default procedure computes this from width, height, and ``[xy]_pixels_per_inch`` on the assumption that the origin is in the upper left corner, that is:

   .. code-block:: c

      xx = x_pixels_per_inch/72, xy = 0,
      yx = 0, yy = -y_pixels_per_inch/72,
      tx = 0, ty = height.

``int (*sync_output)(gx_device *) [OPTIONAL]``
   Synchronize the device. If any output to the device has been buffered, send or write it now. Note that this may be called several times in the process of constructing a page, so printer drivers should not implement this by printing the page. The default implementation does nothing.

``int (*output_page)(gx_device *, int num_copies, int flush) [OPTIONAL]``
   Output a fully composed page to the device. The ``num_copies`` argument is the number of copies that should be produced for a hardcopy device. (This may be ignored if the driver has some other way to specify the number of copies.) The flush argument is true for ``showpage``, false for ``copypage``. The default definition just calls ``sync_output``. Printer drivers should implement this by printing and ejecting the page.

``int (*close_device)(gx_device *) [OPTIONAL]``
   Close the device: release any associated resources. After this, output to the device is no longer allowed. The default implementation does nothing.

   .. note ::
      Clients should never call a device's ``close_device`` procedure directly: they should always call ``gs_closedevice`` instead.


.. _Color mapping:

Color and alpha mapping
~~~~~~~~~~~~~~~~~~~~~~~~~~

Note that code in the Ghostscript library may cache the results of calling one or more of the color mapping procedures. If the result returned by any of these procedures would change (other than as a result of a change made by the driver's ``put_params`` procedure), the driver must call ``gx_device_decache_colors(dev)``.

The ``map_rgb_color``, ``map_color_rgb``, and ``map_cmyk_color`` are obsolete. They have been left in the device procedure list for backward compatibility. See the ``encode_color`` and ``decode_color`` procedures below. To insure that older device drivers are changed to use the new ``encode_color`` and ``decode_color`` procedures, the parameters for the older procedures have been changed to match the new procedures. To minimize changes in devices that have already been written, the ``map_rgb_color`` and ``map_cmyk_color`` routines are used as the default value for the ``encode_color`` routine. The ``map_cmyk_color`` routine is used if the number of components is four. The ``map_rgb_color`` routine is used if the number of components is one or three. This works okay for RGB and CMYK process color model devices. However this does not work properly for gray devices. The ``encode_color`` routine for a gray device is only passed one component. Thus the ``map_rgb_color`` routine must be modified to only use a single input (instead of three). (See the ``encode_color`` and ``decode_color`` routines below.)

Colors can be specified to the Ghostscript graphics library in a variety of forms. For example, there are a wide variety of color spaces that can be used such as Gray, RGB, CMYK, DeviceN, Separation, Indexed, CIEbasedABC, etc. The graphics library converts the various input color space values into four base color spaces: Gray, RGB, CMYK, and DeviceN. The DeviceN color space allows for specifying values for individual device colorants or spot colors.

Colors are converted by the device in a two step process. The first step is to convert a color in one of the base color spaces (Gray, RGB, CMYK, or DeviceN) into values for each device colorant. This transformation is done via a set of procedures provided by the device. These procedures are provided by the ``get_color_mapping_procs`` device procedure.

Between the first and second steps, the graphics library applies transfer functions to the device colorants. Where needed, the output of the results after the transfer functions is used by the graphics library for halftoning.

In the second step, the device procedure ``encode_color`` is used to convert the transfer function results into a ``gx_color_index`` value. The ``gx_color_index`` values are passed to specify colors to various routines. The choice of the encoding for a ``gx_color_index`` is up to the device. Common choices are indexes into a color palette or several integers packed together into a single value. The manner of this encoding is usually opaque to the graphics library. The only exception to this statement occurs when halftoning 5 or more colorants. In this case the graphics library assumes that if a colorant values is zero then the bits associated with the colorant in the ``gx_color_index`` value are zero.


``int get_color_comp_index(const gx_device * dev, const char * pname, int name_size, int src_index) [OPTIONAL]``
   This procedure returns the device colorant number of the given name. The possible return values are -1, 0 to ``GX_DEVICE_COLOR_MAX_COMPONENTS - 1``, or ``GX_DEVICE_COLOR_MAX_COMPONENTS``. A value of -1 indicates that the specified name is not a colorant for the device. A value of 0 to ``GX_DEVICE_COLOR_MAX_COMPONENTS - 1`` indicates the colorant number of the given name. A value of ``GX_DEVICE_COLOR_MAX_COMPONENTS`` indicates that the given name is a valid colorant name for the device but the colorant is not currently being used. This is used for implementing names which are in ``SeparationColorNames`` but not in ``SeparationOrder``.

   The default procedure returns results based upon process color model of DeviceGray, DeviceRGB, or DeviceCMYK selected by ``color_info.num_components``. This procedure must be defined if another process color model is used by the device or spot colors are supported by the device.

``const gx_cm_color_map_procs * get_color_mapping_procs(const gx_device * dev, const gx_device ** tdev) [OPTIONAL]``
   This procedure returns a list of three procedures, together with the device to pass to them. These procedures are used to translate values in either Gray, RGB, or CMYK color spaces into device colorant values. A separate procedure is not required for the :title:`devicen` and ``Separation`` color spaces since these already represent device colorants.

   In many cases, the device returned in tdev will be the same as ``dev``, but the caller should not rely on this. For cases where we have a chain of devices (perhaps because of a subclass or compositor device having been introduced internally as part of the rendering process), the actual device that needs to do the color mapping may be a child device of the original one. In such cases ``tdev`` will be returned as a different value to ``dev``.

   The default procedure returns a list of procedures based upon ``color_info.num_components``. These procedures are appropriate for DeviceGray, DeviceRGB, or DeviceCMYK process color model devices. A procedure must be defined if another process color model is used by the device or spot colors are to be supported. All these procedures take a ``gx_device`` pointer; these should be called with the value returned in ``tdev`` NOT the initial value of ``dev``.


``gx_color_index (*encode_color)(gx_device * dev, gx_color_value * cv) [OPTIONAL]``
   Map a set of device color values into a ``gx_color_index`` value. The range of legal values of the arguments is 0 to ``gx_max_color_value``. The default procedure packs bits into a ``gx_color_index`` value based upon the values in ``color_info.depth`` and ``color_info.num_components``.
   Note that the ``encode_color`` procedure must not return ``gx_no_color_index`` (all 1s).

``int (*decode_color)(gx_device *, gx_color_index color, gx_color_value * CV) [OPTIONAL]``
   This is the inverse of the ``encode_color`` procedure. Map a ``gx_color_index`` value to color values. The default procedure unpacks bits from the ``gx_color_index`` value based upon the values in ``color_info.depth`` and ``color_info.num_components``.

``typedef enum { go_text, go_graphics } graphic_object_type; int (*get_alpha_bits)(gx_device *dev, graphic_object_type type) [OPTIONAL]``
   This procedure returns the number of bits to use for anti-aliasing. The default implementation simply returns the ``color_info.anti_alias`` member of the driver structure.

``void (*update_spot_equivalent_colors)(gx_device *, const gs_state *) [OPTIONAL]``
   This routine provides a method for the device to gather an equivalent color for spot colorants. This routine is called when a ``Separation`` or :title:`devicen` color space is installed. See comments at the start of ``gsequivc.c``.

   .. note::
      This procedure is only needed for devices that support spot colorants and also need to have an equivalent color for simulating the appearance of the spot colorants.


Pixel-level drawing
~~~~~~~~~~~~~~~~~~~~~~~~~~~

This group of drawing operations specifies data at the pixel level. All drawing operations use device coordinates and device color values.

``int (*fill_rectangle)(gx_device *, int x, int y, int width, int height, gx_color_index color)``
   Fill a rectangle with a color. The set of pixels filled is ``{(px,py) | x <= px < x + width and y <= py < y + height}``. In other words, the point (x,y) is included in the rectangle, as are (x+w-1,y), (x,y+h-1), and (x+w-1,y+h-1), but not (x+w,y), (x,y+h), or (x+w,y+h). If width <= 0 or height <= 0, ``fill_rectangle`` should return 0 without drawing anything.

   Note that ``fill_rectangle`` is the only non-optional procedure in the driver interface.

Bitmap imaging
"""""""""""""""""""

Bitmap (or pixmap) images are stored in memory in a nearly standard way. The first byte corresponds to (0,0) in the image coordinate system: bits (or polybit color values) are packed into it left to right. There may be padding at the end of each scan line: the distance from one scan line to the next is always passed as an explicit argument.

``int (*copy_mono)(gx_device *, const unsigned char *data, int data_x, int raster, gx_bitmap_id id, int x, int y, int width, int height, gx_color_index color0, gx_color_index color1) [OPTIONAL]``
   Copy a monochrome image (similar to the PostScript image operator). Each scan line is raster bytes wide. Copying begins at (``data_x``,0) and transfers a rectangle of the given width and height to the device at device coordinate (x,y). (If the transfer should start at some non-zero y value in the data, the caller can adjust the data address by the appropriate multiple of the raster.) The copying operation writes device color ``color0`` at each 0-bit, and ``color1`` at each 1-bit: if ``color0`` or ``color1`` is ``gx_no_color_index``, the device pixel is unaffected if the image bit is 0 or 1 respectively. If ``id`` is different from ``gx_no_bitmap_id``, it identifies the bitmap contents unambiguously; a call with the same ``id`` will always have the same data, raster, and data contents.

   This operation, with ``color0 = gx_no_color_index``, is the workhorse for text display in Ghostscript, so implementing it efficiently is very important.

``int (*strip_tile_rectangle)(gx_device *, const gx_strip_bitmap *tile, int x, int y, int width, int height, gx_color_index color0, gx_color_index color1, int phase_x, int phase_y) [OPTIONAL]``
   Tile a rectangle. Tiling consists of doing multiple ``copy_mono`` operations to fill the rectangle with copies of the tile. The tiles are aligned with the device coordinate system, to avoid "seams". Specifically, the (``phase_x``, ``phase_y``) point of the tile is aligned with the origin of the device coordinate system. (Note that this is backwards from the PostScript definition of halftone phase.) ``phase_x`` and ``phase_y`` are guaranteed to be in the range ``[0..tile->width]`` and ``[0..tile->height]`` respectively.

   If ``color0`` and ``color1`` are both ``gx_no_color_index``, then the tile is a color pixmap, not a bitmap: see the next section.

   This operation is the workhorse for halftone filling in Ghostscript, so implementing it efficiently for solid tiles (that is, where either ``color0`` and ``color1`` are both ``gx_no_color_index``, for colored halftones, or neither one is ``gx_no_color_index``, for monochrome halftones) is very important.


Pixmap imaging
"""""""""""""""""""

Pixmaps are just like bitmaps, except that each pixel may occupy more than one bit. In "chunky" or "Z format", all the bits for each pixel are grouped together. For ``copy_color``, the number of bits per pixel is given by the ``color_info.depth`` parameter in the device structure. The legal values are 1, 2, 4, 8, 16, 24, 32, 40, 48, 56, or 64. The pixel values are device color codes (that is, whatever it is that ``encode_color`` returns).

If the data is planar, then each plane is contiguous, and the number of planes is given by ``color_info.num_components``. The bits per component is ``depth/num_components``.

``int (*copy_color)(gx_device *, const unsigned char *data, int data_x, int raster, gx_bitmap_id id, int x, int y, int width, int height) [OPTIONAL]``
   Copy a color image with multiple bits per pixel. The ``raster`` is in bytes, but ``x`` and ``width`` are in pixels, not bits. If ``id`` is different from ``gx_no_bitmap_id``, it identifies the bitmap contents unambiguously; a call with the same ``id`` will always have the same data, raster, and data contents.

``int (*copy_planes)(gx_device *, const unsigned char *data, int data_x, int raster, gx_bitmap_id id, int x, int y, int width, int height, int plane_height) [OPTIONAL]``
   Copy an image with data stored in planar format. The raster is in bytes, but ``x`` and ``width`` are in pixels, not bits. If ``id`` is different from ``gx_no_bitmap_id``, it identifies the bitmap contents unambiguously; a call with the same ``id`` will always have the same data, raster, and data contents.

   Each plane is ``depth/num_components`` number of bits and the distance between planes is ``plane_height`` number of rows. The height is always less than or equal to the ``plane_height``.

We do not provide a separate procedure for tiling with a pixmap; instead, ``strip_tile_rectangle`` can also take colored tiles. This is indicated by the ``color0`` and ``color1`` arguments' both being ``gx_no_color_index``. In this case, as for ``copy_color``, the raster and height in the "bitmap" are interpreted as for real bitmaps, but the ``x`` and ``width`` are in pixels, not bits.


.. code-block:: c

   typedef enum {
       transform_pixel_region_begin = 0,
       transform_pixel_region_data_needed = 1,
       transform_pixel_region_process_data = 2,
       transform_pixel_region_end = 3
       } transform_pixel_region_reason;
   typedef struct {
       void *state;
       union {
           struct {
               const gs_int_rect *clip;
               int w; /* source width */
               int h; /* source height */
               int spp;
               const gx_dda_fixed_point *pixels; /* DDA to enumerate the destination positions of pixels across a row */
               const gx_dda_fixed_point *rows; /* DDA to enumerate the starting position of each row */
               gs_logical_operation_t lop;
           } init;
           struct {
               const unsigned char *buffer[GX_DEVICE_COLOR_MAX_COMPONENTS];
               int data_x;
               gx_cmapper_t *cmapper;
               const gs_gstate *pgs;
           } process_data;
       } u;
   } transform_pixel_region_data;


``int (*transform_pixel_region)(gx_device *, transform_pixel_reason, transform_pixel_reason_data *data) [OPTIONAL]``
   Transform a 2-dimensional region of pixels into the destination. Given a 2d source block of pixels (supplied as scanline data), this function transforms that data, maps it through the supplied colour lookup function, clips it, and plots it into the device.

   In all calls to this function a negative return value indicates an error.

   Called first with the ``transform_pixel_region_init`` reason code, this prepares for subsequent calls to scale a region as described in the ``data.u.init`` structure. A pointer to any state required for this should be written into ``data.state``, and the caller must pass that in to subsequent calls.

   Subsequently this will be called with ``transform_pixel_region_data_needed``. The function will then check to see if the next scanline of data will be trivially clipped away. If so, then it will return zero to indicate that it is not needed. This can help the caller to avoid unnecessary processing. A positive return value indicates that the line is required.

   For every line where the data is required, the function will be called with ``transform_pixel_region_process_data``. The function will then read and process the line from ``data.u.process_data``. The data in the buffer is packed 8 bit values, which will be fed into the supplied cmapper to set the device color as required. This is then written into the device.

   Once all the scanlines have been fed through calls to ``transform_pixel_region_data_needed`` and ``transform_pixel_region_process_data``, a final call with ``transform_pixel_region_end`` is made that frees the state.

   The default implementation of this device function will generally break the pixel data down into calls to ``fill_rectangle``, though in some cases (notably the portrait 8 bit per component output case), a faster route through ``copy_color`` can be used.

   Memory devices offer a version of this device function that can accelerate direct plotting to the memory array.

   .. note::
      Currently the clipping rectangle is not honoured for skewed (not portrait or landscape) transformations. This is allowed for in the callers.


Compositing
""""""""""""""""

In addition to direct writing of opaque pixels, devices must also support compositing. Currently two kinds of compositing are defined (``RasterOp`` and alpha-based), but more may be added in the future.

``int (*copy_alpha)(gx_device *dev, const unsigned char *data, int data_x, int raster, gx_bitmap_id id, int x, int y, int width, int height, gx_color_index color, int depth) [OPTIONAL]``
   This procedure is somewhat misnamed: it was added to the interface before we really understood alpha channel and compositing.
   Fill a given region with a given color modified by an individual alpha value for each pixel. For each pixel, this is equivalent to alpha-compositing with a source pixel whose alpha value is obtained from the pixmap (``data``, ``data_x``, and ``raster``) and whose color is the given color (which has not been premultiplied by the alpha value), using the Sover rule.

   ``depth``, the number of bits per alpha value, is either 2, 4 or 8. Any copy_alpha routine must accept being called with an 8 bit depth. In addition they should accept either 2 or 4 if the corresponding ``get_alpha_bits`` procedure returns either of those values.

``int (*copy_alpha_hl_color)(gx_device *dev, const byte *data, int data_x, int raster, gx_bitmap_id id, int x, int y, int width, int height, const gx_drawing_color *pdcolor, int depth) [OPTIONAL]``
   Equivalent function to ``copy_alpha``, using high level color rather than a ``gx_color_index``.

``int (*composite)(dev_t *dev, gx_device_t **pcdev, const gs_composite_t *pcte, const gs_imager_state *pis, gs_memory_t *memory) [OPTIONAL]``
   Requests that a device perform a compositing operation; the device should composite data written to it with the device's existing data, according to the compositing function defined by ``*pcte``. If a device cannot perform such an operation itself, it will create a compositor device to wrap itself that will perform such operations for it. Accordingly, the caller must watch the return values from this function to understand if a new device has been created to which future calls should be made.

   Devices will normally implement this in one of the following standard ways:

   - Devices that don't do any imaging and don't forward any imaging operations (for example, the null device, the hit detection device, and the clipping list accumulation device) simply set ``*pcdev`` to themselves, and return 0, which effectively ignores the compositing function.

   - "Leaf" devices that do imaging and have no special optimizations for compositing (for example, some memory devices) ask the ``gs_composite_t`` to create a default compositor device that 'wraps' the current device. They put this in ``*pcdev`` and return 1.

   - Leaf devices that can implement some kinds of compositing operation efficiently (for example, monobit memory devices and ``RasterOp``) inspect the type and values of ``*pcte`` to determine whether it specifies such an operation: if so, they create a specialized compositor, and if not, they ask the ``gs_composite_t`` to create a default compositor. They place this in ``*pcdev`` and return 1.

   In short, on every non-error return, ``*pcdev`` will be set either to the leaf device (in the case where no special compositing is required), or to the device that does the compositing. If the return code is 1, then ``*pcdev`` is a new device that was created to wrap dev to perform the compositing for it. Callers may need to spot this case so as to update which device future operations are sent to.

   For forwarding devices, for example, if the call returns 1, then they should update their target device to be the new device. For the graphics library, if the call returns 1, then it should update the current device to be the new one.

   Other kinds of forwarding devices, which don't fall into any of these categories, require special treatment. In principle, what they do is ask their target to create a compositor, and then create and return a copy of themselves with the target's new compositor as the target of the copy. There is a possible default implementation of this approach: if the original device was D with target T, and T creates a compositor C, then the default implementation creates a device F that for each operation temporarily changes D's target to C, forwards the operation to D, and then changes D's target back to T. However, the Ghostscript library currently only creates a compositor with an imaging forwarding device as target in a few specialized situations (banding, and bounding box computation), and these are handled as special cases.

   Note that the compositor may have a different color space, color representation, or bit depth from the device to which it is compositing. For example, alpha-compositing devices use standard-format chunky color even if the underlying device doesn't.

   Closing a compositor frees all of its storage, including the compositor itself. However, since the composite call may return the same device, clients must check for this case, and only call the close procedure if a separate device was created.


Polygon-level drawing
~~~~~~~~~~~~~~~~~~~~~~~~

In addition to the pixel-level drawing operations that take integer device coordinates and pure device colors, the driver interface includes higher-level operations that draw polygons using fixed-point coordinates, possibly halftoned colors, and possibly a non-default logical operation.

The ``fill_*`` drawing operations all use the center-of-pixel rule: a pixel is colored if, and only if, its center falls within the polygonal region being filled. If a pixel center (X+0.5,Y+0.5) falls exactly on the boundary, the pixel is filled if, and only if, the boundary is horizontal and the filled region is above it, or the boundary is not horizontal and the filled region is to the right of it.

``int (*fill_trapezoid)(gx_device *dev, const  gs_fixed_edge *left, const gs_fixed_edge *right, fixed ybot, fixed ytop, bool swap_axes, const gx_drawing_color *pdcolor, gs_logical_operation_t lop) [OPTIONAL]``
   Fill a trapezoid. The bottom and top edges are parallel to the x axis, and are defined by ``ybot`` and ``ytop``, respectively. The left and right edges are defined by ``left`` and ``right``. Both of these represent lines (``gs_fixed_edge`` is defined in ``gxdevcli.h`` and consists of ``gs_fixed_point`` start and end points). The y coordinates of these lines need not have any specific relation to ``ybot`` and ``ytop``. The routine is defined this way so that the filling algorithm can subdivide edges and still guarantee that the exact same pixels will be filled. If ``swap_axes`` is set, the meanings of X and Y are interchanged.

``int (*fill_parallelogram)(gx_device *dev, fixed px, fixed py, fixed ax, fixed ay, fixed bx, fixed by, const gx_drawing_color *pdcolor, gs_logical_operation_t lop) [OPTIONAL]``
   Fill a parallelogram whose corners are (``px``, ``py``), (``px+ax``, ``py+ay``), (``px+bx``, ``py+by``), and (``px+ax+bx``, ``py+ay+by``). There are no constraints on the values of any of the parameters, so the parallelogram may have any orientation relative to the coordinate axes.

``int (*fill_triangle)(gx_device *dev, fixed px, fixed py, fixed ax, fixed ay, fixed bx, fixed by, const gx_drawing_color *pdcolor, gs_logical_operation_t lop) [OPTIONAL]``
   Fill a triangle whose corners are (``px``, ``py``), (``px+ax``, ``py+ay``), and (``px+bx``, ``py+by``).

``int (*draw_thin_line)(gx_device *dev, fixed fx0, fixed fy0, fixed fx1, fixed fy1, const gx_drawing_color *pdcolor, gs_logical_operation_t lop) [OPTIONAL]``
   Draw a one-pixel-wide line from (``fx0``, ``fy0``) to (``fx1``, ``fy1``).


Linear color drawing
~~~~~~~~~~~~~~~~~~~~~~~~

Linear color functions allow fast high quality rendering of shadings on continuous tone devices. They implement filling simple areas with a lineary varying color. These functions are not called if the device applies halftones, or uses a non-separable or a non-linear color model.

``int (*fill_linear_color_triangle) (dev_t *dev, const gs_fill_attributes *fa, const gs_fixed_point *p0, const gs_fixed_point *p1, const gs_fixed_point *p2, const frac31 *c0, const frac31 *c1, const frac31 *c2) [OPTIONAL]``
   This function is the highest level one within the linear color function group. It fills a triangle with a linearly varying color. Arguments specify 3 points in the device space - vertices of a triangle, and their colors. The colors are represented as vectors of positive fractional numbers, each of which represents a color component value in the interval [0,1]. The number of components in a vector in the number of color components in the device (process) color model.

   The implementation fills entire triangle. The filling rule is same as for Polygon-level drawing. The color of each pixel within the triangle is computed as a linear interpolation of vertex colors.

   The implementation may reject the request if the area or the color appears too complex for filling in a single action. For doing that the implementation returns 0 and must not paint any pixel. In this case the graphics library will perform a subdivision of the area into smaller triangles and call the function again with smaller areas.

   .. important::

      - Do not try to decompose the area within the implementation of ``fill_linear_color_triangle``, because it can break the plane coverage contiguity and cause a dropout. Instead request that the graphics library should perform the decomposition. The graphics library is smart enough to do that properly.

      - The implementation must handle a special case, when only 2 colors are specified. It happens if ``p2`` is ``NULL``. This means that the color does not depend on the X coordinate, i.e. it forms a linear gradient along the Y axis. The implementation must not reject (return 0) such cases.

      - The device color component value 1 may be represented with several hexadecimal values : ``0x7FFF0000``, ``0x7FFFF000``, ``0x7FFFFF00``, etc., because the precision here exceeds the color precision of the device. To convert a ``frac31`` value into a device color component value, first drop (ignore) the sign bit, then drop least significant bits - so many ones as you need to fit the device color precision.

      - The ``fa`` argument may contain the ``swap_axes`` bit set. In this case the implementation must swap (transpose) X and Y axes.

      - The implementation must not paint outside the clipping rectangle specified in the ``fa`` argument. If ``fa->swap_axes`` is true, the clipping rectangle is transposed.

   See ``gx_default_fill_linear_color_triangle`` in ``gdevddrw.c`` for sample code.

``int (*fill_linear_color_trapezoid) (dev_t *dev, const gs_fill_attributes *fa, const gs_fixed_point *p0, const gs_fixed_point *p1, const gs_fixed_point *p2, const gs_fixed_point *p3, const frac31 *c0, const frac31 *c1, const frac31 *c2, const frac31 *c2) [OPTIONAL]``
   This function is a lower level one within the linear color function group. The default implementation of ``fill_linear_color_triangle`` calls this function 1-2 times per triangle. Besides that, this function may be called by the graphics library for other special cases, when a decomposition into triangles appears undesirable.

   While the prototype can specify a bilinear color, we assume that the implementation handles linear colors only. This means that the implementation can ignore any of ``c0``, ``c1``, ``c2``, ``c3`` . The graphics library takes a special care of the color linearity when calling this function. The reason for passing all 4 color arguments is to avoid color precision problems.

   Similarly to ``fill_linear_color_triangle`` , this function may be called with only 2 colors, and may reject areas as being too complex. All those important notes are applicable here.

   Sample code may be found in in ``gxdtfill.h``; be aware it's rather complicated. A linear color function is generated from it as ``gx_fill_trapezoid_ns_lc`` with the following template parameters:

   .. code-block:: c

      #define LINEAR_COLOR 1
      #define EDGE_TYPE gs_linear_color_edge
      #define FILL_ATTRS const gs_fill_attributes *
      #define CONTIGUOUS_FILL 0
      #define SWAP_AXES 0
      #define FILL_DIRECT 1

   See the helplers ``init_gradient``, ``step_gradient`` (defined in ``gdevddrw.c``), how to manage colors.

   See ``check_gradient_overflow`` (defined in in ``gdevddrw.c``), as an example of an area that can't be painted in a single action due to 64-bits fixed overflows.


``int (*fill_linear_color_scanline) (dev_t *dev, const gs_fill_attributes *fa, int i, int j, int w, const frac31 *c0, const int32_t *c0_f, const int32_t *cg_num, int32_t cg_den) [OPTIONAL]``
   This function is the lowest level one within the linear color function group. It implements filling a scanline with a linearly varying color. The default implementation for ``fill_linear_color_trapezoid`` calls this function, and there are no other calls to it from the graphics libary. Thus if the device implements ``fill_linear_color_triangle`` and ``fill_linear_color_trapezoid`` by own means, this function may be left unimplemented.

   ``i`` and ``j`` specify device coordinates (indices) of the starting pixel of the scanline, ``w`` specifies the width of the scanline, i.e. the number of pixels to be painted to the right from the starting pixel, including the starting pixel.

   ``c0`` specifies the color for the starting pixel as a vector of fraction values, each of which represents a color value in the interval [0,1].

   ``c0_f`` specify a fraction part of the color for the starting pixel. See the formula below about using it.

   ``cg_num`` specify a numerator for the color gradient - a vector of values in [-1,1], each of which correspond to a color component.

   ``cg_den`` specify the denominator for the color gradient - a value in [-1,1].

   The color for the pixel ``[i + k, j]`` to be computed like this :

         ``(double)(c0[n] + (c0_f[n] + cg_num[n] * k) / cg_den) / (1 ^ 31 - 1)``

   where ``0 <= k <= w`` , and ``n`` is a device color component index.

   .. important::

         - The ``fa`` argument may contain the ``swap_axes`` bit set. In this case the implementation must swap (transpose) X and Y axes.

         - The implementation must not paint outside the clipping rectangle specified in the ``fa`` argument. If ``fa->swap_axes`` is true, the clipping rectangle is transposed.

   See ``gx_default_fill_linear_color_scanline`` in ``gdevdsha.c`` for sample code.



High-level drawing
~~~~~~~~~~~~~~~~~~~~~~~~

In addition to the lower-level drawing operations described above, the driver interface provides a set of high-level operations. Normally these will have their default implementation, which converts the high-level operation to the low-level ones just described; however, drivers that generate high-level (vector) output formats such as :title:`pdfwrite`, or communicate with devices that have firmware for higher-level operations such as polygon fills, may implement these high-level operations directly. For more details, please consult the source code, specifically:


.. list-table::
   :widths: 50 50
   :header-rows: 1

   * - Header
     - Defines
   * - gxpaint.h
     - ``gx_fill_params``, ``gx_stroke_params``
   * - gxfixed.h
     - fixed, ``gs_fixed_point`` (used by ``gx_*_params``)
   * - gxgstate.h
     - ``gs_imager_state`` (used by ``gx_*_params``)
   * - gxline.h
     - ``gx_line_params`` (used by ``gs_imager_state``)
   * - gslparam.h
     - line cap/join values (used by ``gx_line_params``)
   * - gxmatrix.h
     - ``gs_matrix_fixed`` (used by ``gs_imager_state``)
   * - gspath.h, gxpath.h, gzpath.h
     - ``gx_path``
   * - gxcpath.h, gzcpath.h
     - ``gx_clip_path``


For a minimal example of how to implement the high-level drawing operations, see ``gdevtrac.c``.

Paths
"""""""""""

``int (*fill_path)(gx_device *dev, const gs_imager_state *pis, gx_path *ppath, const gx_fill_params *params, const gx_drawing_color *pdcolor, const gx_clip_path *pcpath) [OPTIONAL]``
   Fill the given path, clipped by the given clip path, according to the given parameters, with the given color. The clip path pointer may be ``NULL``, meaning do not clip.

   The implementation must paint the path with the specified device color, which may be either a pure color, or a pattern. If the device can't handle non-pure colors, it should check the color type and call the default implementation ``gx_default_fill_path`` for cases which it can't handle. The default implementation will perform a subdivision of the area to be painted, and will call other device virtual functions (such as ``fill_linear_color_triangle``) with simpler areas.

``int (*stroke_path)(gx_device *dev, const gs_imager_state *pis, gx_path *ppath, const gx_stroke_params *params, const gx_drawing_color *pdcolor, const gx_clip_path *pcpath) [OPTIONAL]``
   Stroke the given path, clipped by the given clip path, according to the given parameters, with the given color. The clip path pointer may be ``NULL``, meaning not to clip.

``int (*fill_mask)(gx_device *dev, const byte *data, int data_x, int raster, gx_bitmap_id id, int x, int y, int width, int height, const gx_drawing_color *pdcolor, int depth, int command, const gx_clip_path *pcpath) [OPTIONAL]``
   Color the 1-bits in the given mask (or according to the alpha values, if depth > 1), clipped by the given clip path, with the given color and logical operation. The clip path pointer may be ``NULL``, meaning do not clip. The parameters ``data``, ``...``, ``height`` are as for ``copy_mono``; ``depth`` is as for ``copy_alpha``; ``command`` is as below.

The function specification f
""""""""""""""""""""""""""""""""""""

"Command" indicates the raster operation and transparency as follows:


.. list-table::
   :widths: 20 80
   :header-rows: 1

   * - Bits
     - Notes
   * - 7-0
     - raster op
   * - 8
     - 0 if source opaque, 1 if source transparent
   * - 9
     - 0 if texture opaque, 1 if texture transparent
   * - 10
     - 1 if pdf transparency is in use, 0 otherwise.

       This makes no difference to the rendering,

       but forces the raster operation to be considered non-idempotent by internal routines.
   * - 11
     - 1 if the target of this operation is a specific plane, rather than all planes.

       The plane in question is given by bits 13 upwards. This is only used by the planar device.
   * - 12-
     - If bit 11 = 1, then bits 1


In general most devices should just check to see that bits they do not handle (11 and above typically) are zero, and should jump to the default implementation, or return an error otherwise.

The raster operation follows the Microsoft and H-P specification. It is an 8-element truth table that specifies the output value for each of the possible 2×2×2 input values as follows:



.. list-table::
   :widths: 25 25 25 25
   :header-rows: 1

   * - Bit
     - Texture
     - Source
     - Destination
   * - 7
     - 1
     - 1
     - 1
   * - 6
     - 1
     - 1
     - 0
   * - 5
     - 1
     - 0
     - 1
   * - 4
     - 1
     - 0
     - 0
   * - 3
     - 0
     - 1
     - 1
   * - 2
     - 0
     - 1
     - 0
   * - 1
     - 0
     - 0
     - 1
   * - 0
     - 0
     - 0
     - 0


Transparency affects the output in the following way. A source or texture pixel is considered transparent if its value is all 1s (for instance, 1 for bitmaps, ``0xffffff`` for 24-bit RGB pixmaps) and the corresponding transparency bit is set in the command. For each pixel, the result of the Boolean operation is written into the destination if, and only if,  neither the source nor the texture pixel is transparent. (Note that the HP ``RasterOp`` specification, on which this is based, specifies that if the source and texture are both all 1s and the command specifies transparent source and opaque texture, the result should be written in the output. We think this is an error in the documentation.)


Images
""""""""""""""""""""""""""""""""""""

Similar to the high-level interface for fill and stroke graphics, a high-level interface exists for bitmap images. The procedures in this part of the interface are optional.

Bitmap images come in a variety of types, corresponding closely (but not precisely) to the PostScript ImageTypes. The generic or common part of all bitmap images is defined by:

.. code-block:: c

   typedef struct {
       const gx_image_type_t *type;
           gs_matrix ImageMatrix;
   } gs_image_common_t;

Bitmap images that supply data (all image types except image_type_from_device (2)) are defined by:

.. code-block:: c

   #define gs_image_max_components 5
   typedef struct {
           << gs_image_common_t >>
           int Width;
           int Height;
           int BitsPerComponent;
           float Decode[gs_image_max_components * 2];
           bool Interpolate;
   } gs_data_image_t;

Images that supply pixel (as opposed to mask) data are defined by:


.. code-block:: c

   typedef enum {
       /* Single plane, chunky pixels. */
       gs_image_format_chunky = 0,
       /* num_components planes, chunky components. */
       gs_image_format_component_planar = 1,
       /* BitsPerComponent * num_components planes, 1 bit per plane */
       gs_image_format_bit_planar = 2
   } gs_image_format_t;
   typedef struct {
           << gs_data_image_t >>
           const gs_color_space *ColorSpace;
           bool CombineWithColor;
   } gs_pixel_image_t;

Ordinary PostScript Level 1 or Level 2 (ImageType 1) images are defined by:

.. code-block:: c

   typedef enum {
       /* No alpha. */
       gs_image_alpha_none = 0,
       /* Alpha precedes color components. */
       gs_image_alpha_first,
       /* Alpha follows color components. */
       gs_image_alpha_last
   } gs_image_alpha_t;
   typedef struct {
           << gs_pixel_image_t >>
           bool ImageMask;
           bool adjust;
       gs_image_alpha_t Alpha;
   } gs_image1_t;
   typedef gs_image1_t gs_image_t;


Of course, standard PostScript images don't have an alpha component. For more details, consult the source code in ``gsiparam.h`` and ``gsiparm*.h``, which define parameters for an image.

The ``begin[_typed_]image`` driver procedures create image enumeration structures. The common part of these structures consists of:


.. code-block:: c

   typedef struct gx_image_enum_common_s {
           const gx_image_type_t *image_type;
       const gx_image_enum_procs_t *procs;
       gx_device *dev;
       gs_id id;
           int num_planes;
           int plane_depths[gs_image_max_planes];  /* [num_planes] */
       int plane_widths[gs_image_max_planes]    /* [num_planes] */
   } gx_image_enum_common_t;


where ``procs`` consists of:

.. code-block:: c

   typedef struct gx_image_enum_procs_s {

        /*
         * Pass the next batch of data for processing.
         */
         #define image_enum_proc_plane_data(proc)\
           int proc(gx_device *dev,\
             gx_image_enum_common_t *info, const gx_image_plane_t *planes,\
             int height)

                 image_enum_proc_plane_data((*plane_data));

                 /*
                  * End processing an image, freeing the enumerator.
                  */
         #define image_enum_proc_end_image(proc)\
           int proc(gx_device *dev,\
             gx_image_enum_common_t *info, bool draw_last)

                 image_enum_proc_end_image((*end_image));

             /*
              * Flush any intermediate buffers to the target device.
              * We need this for situations where two images interact
              * (currently, only the mask and the data of ImageType 3).
              * This procedure is optional (may be 0).
              */
         #define image_enum_proc_flush(proc)\
           int proc(gx_image_enum_common_t *info)

             image_enum_proc_flush((*flush));

   } gx_image_enum_procs_t;


In other words, ``begin[_typed]_image`` sets up an enumeration structure that contains the procedures that will process the image data, together with all variables needed to maintain the state of the process. Since this is somewhat tricky to get right, if you plan to create one of your own you should probably read an existing implementation of ``begin[_typed]_image``, such as the one in ``gdevbbox.c``.

The data passed at each call of ``image_plane_data`` consists of one or more planes, as appropriate for the type of image. ``begin[_typed]_image``  must initialize the ``plane_depths`` array in the enumeration structure with the depths (bits per element) of the planes. The array of ``gx_image_plane_t`` structures passed to each call of ``image_plane_data`` then defines where the data are stored, as follows:


.. code-block:: c

   typedef struct gx_image_plane_s {
     const byte *data;
     int data_x;
     uint raster;
   } gx_image_plane_t;


``int (*begin_typed_image)(gx_device *dev, const gs_imager_state *pis, const gs_matrix *pmat, const gs_image_common_t *pim, gs_int_rect *prect, const gx_drawing_color *pdcolor, const gx_clip_path *pcpath, gs_memory_t *memory, gx_image_enum_common_t **pinfo) [OPTIONAL]``
   Begin the transmission of an image. Zero or more calls of the ``image_plane_data`` function supplied in the returned image enumerator will follow, and then a call of ``end_image``. The parameters of ``begin_typed_image`` are as follows:



   .. list-table::
      :widths: 15 85
      :header-rows: 0

      * - ``pis``
        - pointer to an imager state.
          The only relevant elements of the imager state are the CTM (coordinate transformation matrix),

          the logical operation (``RasterOp`` or transparency), and the color rendering information.

          For mask images, if ``pmat`` is not NULL and the color is pure, ``pis`` may be NULL.

      * - ``pmat``
        - pointer to a ``gs_matrix`` structure that defines the image transformation matrix.

          If ``pis`` is non-NULL, and ``pmat`` is NULL, then the ``ctm`` from ``pis`` should be used.

      * - ``pim``
        - pointer to the ``gs_image_t`` structure that defines the image parameters.

      * - ``prect``
        -  if not NULL, defines a subrectangle of the image;

           only the data for this subrectangle will be passed to ``image_plane_data``,

           and only this subrectangle should be drawn.

      * - ``pdcolor``
        - defines a drawing color, only needed for masks or if ``CombineWithColor`` is true.

      * - ``pcpath``
        - if not NULL, defines an optional clipping path.

      * - ``memory``
        - defines the allocator to be used for allocating bookkeeping information.

      * - ``pinfo``
        -  the implementation should return a pointer to its state structure here.


   ``begin_typed_image`` is expected to allocate a structure for its bookkeeping needs, using the allocator defined by the memory parameter, and return it in ``*pinfo``. ``begin_typed_image`` should not assume that the structures in ``*pim``, ``*prect``, or ``*pdcolor`` will survive the call on ``begin_typed_image`` (except for the color space in ``*pim->ColorSpace``): it should copy any necessary parts of them into its own bookkeeping structure. It may, however, assume that ``*pis``, ``*pcpath``, and of course ``*memory`` will live at least until ``end_image`` is called.

   ``begin_typed_image`` returns 0 normally, or 1 if the image does not need any data. In the latter case, ``begin_typed_image`` does not allocate an enumeration structure.


The format of the image (how pixels are represented) is given by ``pim->format``.

The actual transmission of data uses the procedures in the enumeration structure, not driver procedures, since the handling of the data usually depends on the image type and parameters rather than the device. These procedures are specified as follows.


``int (*image_plane_data)(gx_device *dev, gx_image_enum_common_t *info, const gx_image_plane_t *planes, int height)``
   This call provides more of the image source data: specifically, height rows, with Width pixels supplied for each row.

   The data for each row are packed big-endian within each byte, as for copy_color. The ``data_x`` (starting X position within the row) and raster (number of bytes per row) are specified separately for each plane, and may include some padding at the beginning or end of each row. Note that for non-mask images, the input data may be in any color space and may have any number of bits per component (1, 2, 4, 8, 12); currently mask images always have 1 bit per component, but in the future, they might allow multiple bits of alpha. Note also that each call of ``image_plane_data`` passes complete pixels: for example, for a chunky image with 24 bits per pixel, each call of ``image_plane_data`` passes 3N bytes of data (specifically, 3 × Width × height).

   The interpretation of planes depends on the format member of the ``gs_image[_common]_t`` structure:

   If the format is ``gs_image_format_chunky``, ``planes[0].data`` points to data in "chunky" format, in which the components follow each other (for instance, RGBRGBRGB....)

   If the format is ``gs_image_format_component_planar``, ``planes[0 .. N-1].data`` point to data for the N components (for example, N=3 for RGB data); each plane contains samples for a single component, for instance, RR..., GG..., BB.... Note that the planes are divided by component, not by bit: for example, for 24-bit RGB data, N=3, with 8-bit values in each plane of data.

   If the format is ``gs_image_format_bit_planar``, ``planes[0 .. N*B-1].data`` point to data for the N components of B bits each (for example, N=3 and B=4 for RGB data with 4 bits per component); each plane contains samples for a single bit, for instance, R0 R1 R2 R3 G0 G1 G2 G3 B0 B1 B2 B3.

   Note that the most significant bit of each plane comes first.

   If, as a result of this call, ``image_plane_data`` has been called with all the data for the (sub-)image, it returns 1; otherwise, it returns 0 or an error code as usual.

   ``image_plane_data``, unlike most other procedures that take bitmaps as arguments, does not require the data to be aligned in any way.

   Note that for some image types, different planes may have different numbers of bits per pixel, as defined in the ``plane_depths`` array.


``int (*end_image)(gx_device *dev, void *info, bool draw_last)``
   Finish processing an image, either because all data have been supplied or because the caller has decided to abandon this image. ``end_image`` may be called at any time after ``begin_typed_image``. It should free the info structure and any subsidiary structures. If ``draw_last`` is true, it should finish drawing any buffered lines of the image.


.. note::

   - While there will almost never be more than one image enumeration in progress -- that is, after a ``begin_typed_image``, ``end_image`` will almost always be called before the next ``begin_typed_image`` -- driver code should not rely on this property; in particular, it should store all information regarding the image in the info structure, not in the driver structure.

   - If ``begin_typed_image`` saves its parameters in the info structure, it can decide on each call whether to use its own algorithms or to use the default implementation. (It may need to call ``gx_default_begin/end_image`` partway through.)


Text
""""""""""""""""""""""""""""""""""""

The third high-level interface handles text. As for images, the interface is based on creating an enumerator which then may execute the operation in multiple steps. As for the other high-level interfaces, the procedures are optional.


``int (*text_begin)(gx_device *dev, gs_imager_state *pis, const gs_text_params_t *text, gs_font *font, const gx_clip_path *pcpath, gs_text_enum_t **ppte) [OPTIONAL]``
   Begin processing text, by creating a state structure and storing it in ``*ppte``. The parameters of ``text_begin`` are as follows:

   .. list-table::
      :widths: 15 85
      :header-rows: 0

      * - ``dev``
        - The usual pointer to the device.
      * - ``pis``
        - A pointer to an imager state. All elements may be relevant, depending on how the text is rendered.
      * - ``text``
        - A pointer to the structure that defines the text operation and parameters. See ``gstext.h`` for details.
      * - ``font``
        - Defines the font for drawing.
      * - ``pcpath``
        - If not ``NULL``, defines an optional clipping path. Only relevant if the text operation includes ``TEXT_DO_DRAW``.
      * - ``ppte``
        - The implementation should return a pointer to its state structure here.


   ``text_begin`` must allocate a structure for its bookkeeping needs, using the allocator used by the graphics state, and return it in ``*ppte``. ``text_begin`` may assume that the structures passed as parameters will survive until text processing is complete.

   If the text operation includes ``TEXT_DO...PATH`` then the character outline will be appended to the current path in the ``pgs``. The current point of that path indicates where drawing should occur and will be updated by the string width (unless the text operation includes ``TEXT_DO_NONE``).

   If the text operation includes ``TEXT_DO_DRAW`` then the text color will be taken from the current colour in the graphics state. (Otherwise no colour is required).

   The bookkeeping information will be allocated using the memory allocator from the graphics state.

   Clients should not call the driver text_begin procedure directly. Instead, they should call ``gx_device_text_begin``, which takes the same parameters and also initializes certain common elements of the text enumeration structure, or ``gs_text_begin``, which takes many of the parameters from a graphics state structure. For details, see ``gstext.h``.

   The actual processing of text uses the procedures in the enumeration structure, not driver procedures, since the handling of the text may depend on the font and parameters rather than the device. Text processing may also require the client to take action between characters, either because the client requested it (``TEXT_INTERVENE`` in the operation) or because rendering a character requires suspending text processing to call an external package such as the PostScript interpreter. (It is a deliberate design decision to handle this by returning to the client, rather than calling out of the text renderer, in order to avoid potentially unknown stack requirements.) Specifically, the client must call the following procedures, which in turn call the procedures in the text enumerator.


``int gs_text_process(gs_text_enum_t *pte)``
   Continue processing text. This procedure may return 0 or a negative error code as usual, or one of the following values (see ``gstext.h`` for details).

   .. list-table::
      :widths: 30 70
      :header-rows: 0

      * - TEXT_PROCESS_RENDER
        - The client must cause the current character to be rendered.

          This currently only is used for PostScript Type 0-4 fonts and their CID-keyed relatives.

      * - TEXT_PROCESS_INTERVENE
        - The client has asked to intervene between characters. This is used for ``cshow`` and ``kshow``.



``int gs_text_release(gs_gstate * pgs, gs_text_enum_t *pte, client_name_t cname)``
   Finish processing text and release all associated structures. Clients must call this procedure after ``gs_text_process`` returns 0 or an error, and may call it at any time.

   There are numerous other procedures that clients may call during text processing. See ``gstext.h`` for details.


   .. note::

      Unlike many other optional procedures, the default implementation of ``text_begin`` cannot simply return: like the default implementation of ``begin[_typed]_image``, it must create and return an enumerator. Furthermore, the implementation of the process procedure (in the enumerator structure, called by ``gs_text_process``) cannot simply return without doing anything, even if it doesn't want to draw anything on the output. See the comments in ``gxtext.h`` for details.


Unicode support for high level (vector) devices
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

Implementing a new high level (also known as vector) device, one may need to translate Postscript character codes into Unicode. This can be done pretty simply.

For translating a Postscript text you need to implement the device virtual function ``text_begin``. It should create a new instance of ``gs_text_enum_t`` in the heap (let its pointer be ``pte``), and assign a special function to ``gs_text_enum_t::procs.process``. The function will receive ``pte``. It should take the top level font from ``pte->orig_font``, and iterate with ``font->procs.next_char_glyph(pte, ..., &glyph)``. The last argument receives a ``gs_glyph`` value, which encodes a Postscript character name or CID (and also stores it into ``pte->returned.current_glyph``). Then obtain the current subfont with ``gs_text_current_font(pte)`` (it can differ from the font) and call ``subfont->procs.decode_glyph(subfont, glyph)``. The return value will be an Unicode code, or ``GS_NO_CHAR`` if the glyph can't be translated to Unicode.


Reading bits back
~~~~~~~~~~~~~~~~~~~

``int (*get_bits_rectangle)(gx_device *dev, const gs_int_rect *prect, gs_get_bits_params_t *params) [OPTIONAL]``
   Read a rectangle of bits back from the device. The ``params`` structure consists of:

   .. list-table::
      :widths: 30 70
      :header-rows: 0

      * - ``options``
        - the allowable formats for returning the data
      * - ``data[32]``
        - pointers to the returned data
      * - ``x_offset``
        - the X offset of the first returned pixel in data
      * - ``raster``
        - the distance between scan lines in the returned data

   ``options`` is a bit mask specifying what formats the client is willing to accept. (If the client has more flexibility, the implementation may be able to return the data more efficiently, by avoiding representation conversions.) The options are divided into groups:

   *alignment*
      Specifies whether the returned data must be aligned in the normal manner for bitmaps, or whether unaligned data are acceptable.

   *pointer or copy*
      Specifies whether the data may be copied into storage provided by the client and/or returned as pointers to existing storage. (Note that if copying is not allowed, it is much more likely that the implementation will return an error, since this requires that the client accept the data in the implementation's internal format.)

   *X offset*
      Specifies whether the returned data must have a specific X offset (usually zero, but possibly other values to avoid skew at some later stage of processing) or whether it may have any X offset (which may avoid skew in the ``get_bits_rectangle`` operation itself).

   *raster*
      Specifies whether the raster (distance between returned scan lines) must have its standard value, must have some other specific value, or may have any value. The standard value for the raster is the device width padded out to the alignment modulus when using pointers, or the minimum raster to accommodate the X offset + width when copying (padded out to the alignment modulus if standard alignment is required).

   *format*
      Specifies whether the data are returned in chunky (all components of a single pixel together), component-planar (each component has its own scan lines), or bit-planar (each bit has its own scan lines) format.

   *color space*
      Specifies whether the data are returned as native device pixels, or in a standard color space. Currently the only supported standard space is RGB.

   *standard component depth*
      Specifies the number of bits per component if the data are returned in the standard color space. (Native device pixels use ``dev->color_info.depth`` bits per pixel.)

   *alpha*
      Specifies whether alpha channel information should be returned as the first component, the last component, or not at all. Note that for devices that have no alpha capability, the returned alpha values will be all 1s.


The client may set more than one option in each of the above groups; the implementation will choose one of the selected options in each group to determine the actual form of the returned data, and will update ``params[].options`` to indicate the form. The returned ``params[].options`` will normally have only one option set per group.

For further details on params, see ``gxgetbit.h``. For further details on options, see ``gxbitfmt.h``.

Define ``w = prect->q.x - prect->p.x, h = prect->q.y - prect->p.y``. If the bits cannot be read back (for example, from a printer), return ``gs_error_unknownerror``; if raster bytes is not enough space to hold ``offset_x + w pixels``, or if the source rectangle goes outside the device dimensions (``p.x < 0 || p.y < 0 || q.x > dev->width || q.y > dev->height``), return ``gs_error_rangecheck``; if any regions could not be read, return ``gs_error_ioerror`` if unpainted is ``NULL``, otherwise the number of rectangles (see below); otherwise return 0.

The caller supplies a buffer of ``raster × h`` bytes starting at ``data[0]`` for the returned data in chunky format, or ``N`` buffers of ``raster × h`` bytes starting at ``data[0]`` through ``data[N-1]`` in planar format where ``N`` is the number of components or bits. The contents of the bits beyond the last valid bit in each scan line (as defined by ``w``) are unpredictable. ``data`` need not be aligned in any way. If ``x_offset`` is non-zero, the bits before the first valid bit in each scan line are undefined. If the implementation returns pointers to the ``data``, it stores them into ``data[0]`` or ``data[0..N-1]``.


Parameters
~~~~~~~~~~~~~~~~~~~

Devices may have an open-ended set of parameters, which are simply pairs consisting of a name and a value. The value may be of various types: integer (int or long), boolean, float, string, name, NULL, array of integer, array of float, or arrays or dictionaries of mixed types. For example, the ``Name`` of a device is a string; the ``Margins`` of a device is an array of two floats. See ``gsparam.h`` for more details.

If a device has parameters other than the ones applicable to all devices (or, in the case of printer devices, all printer devices), it must provide ``get_params`` and ``put_params`` procedures. If your device has parameters beyond those of a straightforward display or printer, we strongly advise using the ``get_params`` and ``put_params`` procedures in an existing device (for example, ``gdevcdj.c`` or ``gdevbit.c``) as a model for your own code.


``int (*get_params)(gx_device *dev, gs_param_list *plist) [OPTIONAL]``
   Read the parameters of the device into the parameter list at plist, using the ``param_write_*`` macros or procedures defined in ``gsparam.h``.

``int (*get_hardware_params)(gx_device *dev, gs_param_list *plist) [OPTIONAL]``
   Read the hardware-related parameters of the device into the parameter list at plist. These are any parameters whose values are under control of external forces rather than the program -- for example, front panel switches, paper jam or tray empty sensors, etc. If a parameter involves significant delay or hardware action, the driver should only determine the value of the parameter if it is "requested" by the ``gs_param_list [param_requested(plist, key_name)]``. This function may cause the asynchronous rendering pipeline (if enabled) to be drained, so it should be used sparingly.

``int (*put_params)(gx_device *dev, gs_param_list *plist) [OPTIONAL]``
   Set the parameters of the device from the parameter list at plist, using the ``param_read_*`` macros/procedures defined in ``gsparam.h``. All ``put_params`` procedures must use a "two-phase commit" algorithm; see ``gsparam.h`` for details.

Default color rendering dictionary (CRD) parameters
""""""""""""""""""""""""""""""""""""""""""""""""""""""

Drivers that want to provide one or more default CIE color rendering dictionaries (CRDs) can do so through ``get_params``. To do this, they create the CRD in the usual way (normally using the ``gs_cie_render1_build`` and ``_initialize`` procedures defined in ``gscrd.h``), and then write it as a parameter using ``param_write_cie_render1`` defined in ``gscrdp.h``. However, the ``TransformPQR`` procedure requires special handling. If the CRD uses a ``TransformPQR`` procedure different from the default (identity), the driver must do the following:

- The ``TransformPQR`` element of the CRD must include a ``proc_name``, and optionally ``proc_data``. The ``proc_name`` is an arbitrary name chosen by the driver to designate the particular ``TransformPQR`` function. It must not be the same as any device parameter name; we strongly suggest it include the device name, for instance, "bitTPQRDefault".

- For each such named ``TransformPQR`` procedure, the driver's ``get_param`` procedure must provide a parameter of the same name. The parameter value must be a string whose bytes are the actual procedure address.


For a complete example, see the ``bit_get_params`` procedure in ``gdevbit.c``. Note that it is essential that the driver return the CRD or the procedure address only if specifically requested (``param_requested(...) > 0``); otherwise, errors will occur.



Device parameters affecting interpretation
""""""""""""""""""""""""""""""""""""""""""""""""""""""

Some parameters have been defined for high level (vector) device drivers which affect the operation of the interpreter. These are documented here so that other devices requiring the same behaviour can use these parameters.

``/HighLevelDevice``
   True if the device is a high level (vector) device. Currently this controls haltone emission during setpagedevice. Normally ``setpagdevice`` resets the halftone to a default value, which is unfortunate for high-level devices such as :title:`ps2write` and :title:`pdfwrite`, as they are unable to tell that this is caused by ``setpagdevice`` rather than a halftone set by the input file. In order to prevent spurious default halftones being embedded in the output, if ``/HighLevelDevice`` is present and true in the device paramters, then the default halftone will not be set during ``setpagedevice``. Also prevents interpolation of imagemasks during PDF interpretation.

``/AllowIncrementalCFF``
   :title:`pdfwrite` relies on font processing occuring in a particular order, which may not happen if CFF fonts are downloaded incrementally. Defining this parameter to true will prevent incremental CFF downloading (may raise an error during processing).

``/AllowPSRepeatFuncs``
   :title:`pdfwrite` emits functions as type 4, and as a result can't convert PostScript functions using the repeat operator into PDF functions. Defining this parameter as true will cause such functions to raise an error during processing.

``/IsDistiller``
   Defining this parameter as true will result in the operators relating to 'distillerparams' being defined (``setdistillerparams``/``currentdistillerparams``). Some PostScript files behave differently if these operators are present (e.g. rotating the page) so this parameter may be true even if the device is not strictly a Distiller. For example :title:`ps2write` defines this parameter to be true.

``/PreserveSMask``
   If this parameter is true then the PDF interpreter will not convert ``SMask`` (soft mask, ie transparent) images into opaque images. This should be set to true for devices which can handle transparency (e.g. :title:`pdfwrite`).

``/PreserveTrMode``
   If this parameter is true then the PDF interpreter will not handle Text Rendering modes by degenerating into a sequence of text operations, but will instead set the ``Tr`` mode, and emit the text once. This value should be true for devices which can handle PDF text rendering modes directly.

``/WantsToUnicode``
   In general, Unicode values are not of interest to rendering devices, but for high level (aka vector) devices, they can be extremely valuable. If this parameter is defined as true then ``ToUnicode CMaps`` and ``GlyphName2Unicode`` tables will be processed and stored.


Page devices
~~~~~~~~~~~~~~

``gx_device *(*get_page_device)(gx_device *dev) [OPTIONAL]``
   According to the Adobe specifications, some devices are "page devices" and some are not. This procedure returns NULL if the device is not a page device, or the device itself if it is a page device. In the case of forwarding devices, ``get_page_device`` returns the underlying page device (or NULL if the underlying device is not a page device).

Miscellaneous
~~~~~~~~~~~~~~~

``int (*get_band)(gx_device *dev, int y, int *band_start) [OPTIONAL]``
   If the device is a band device, this procedure stores in ``*band_start`` the scan line (device Y coordinate) of the band that includes the given Y coordinate, and returns the number of scan lines in the band. If the device is not a band device, this procedure returns 0. The latter is the default implementation.

``void (*get_clipping_box)(gx_device *dev, gs_fixed_rect *pbox) [OPTIONAL]``
   Stores in ``*pbox`` a rectangle that defines the device's clipping region. For all but a few specialized devices, this is *((0,0),(width,height))*.


Device Specific Operations
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

In order to enable the provision of operations that make sense only to a small range of devices/callers, we provide an extensible function. The operation to perform is specified by an integer, taken from an enumeration in ``gxdevsop.h``.

A typical user of this function might make a call to detect whether a device works in a particular way (such as whether it has a particular color mapping) to enable an optimisation elsewhere. Sometimes it may be used to detect a particular piece of functionality (such as whether ``copy_plane`` is supported); in other cases it may be used both to detect the presence of other functionality and to perform functions as well (such as with the pdf specific pattern management calls - moved here from their own dedicated device function).

This function is designed to be easy to chain through multiple levels of device without each intermediate device needing to know about the full range of operations it may be asked to perform.

``int (*dev_spec_op)(gx_device *dev, int dso, void *data, int size) [OPTIONAL]``
   Perform device specific operation ``dso``. Returns ``gs_error_undefined`` for an unknown (or unsupported operation), other negative values for errors, and (``dso`` specific) non-negative values to indicate success. For details of the meanings of ``dso``, ``data`` and ``size``, see ``gxdevsop.h``.


Tray selection
----------------------

The logic for selecting input trays, and modifying other parameters based on tray selection, can be complex and subtle, largely thanks to the requirement to be compatible with the PostScript language ``setpagedevice`` mechanism. This section will describe recipes for several common scenarios for tray selection, with special attention to the how the overall task factors into configuration options, generic logic provided by the PostScript language (or not, if the device is used with other PDL's), and implementation of the ``put_param`` / ``get_param`` device functions within the device.

In general, tray selection is determined primarily through the ``setpagedevice`` operator, which is part of the PostScript runtime. Ghostscript attempts to be as compatible as is reasonable with the PostScript standard, so for more details, see the description in the PostScript language specifications, including the "supplements", which tend to have more detail about ``setpagedevice`` behavior than the PLRM book itself.

The first step is to set up an ``/InputAttributes`` dictionary matching the trays and so on available in the device. The standard Ghostscript initialization files set up a large ``InputAttributes`` dictionary with many "known" page sizes (the full list is in ``gs_statd.ps``, under ``.setpagesize``). It's possible to edit this list in the Ghostscript source, of course, but most of the time it is better to execute a snippet of PostScript code after the default initialization but before sending any actual jobs.

Simply setting a new ``/InputAttributes`` dictionary with ``setpagedevice`` will not work, because the the language specification for ``setpagedevice`` demands a "merging" behavior - paper tray keys present in the old dictionary will be preserved even if the key is not present in the new ``/InputAttributes`` dictionary. Here is a sample invocation that clears out all existing keys, and installs three new ones: a US letter page size for trays 0 and 1, and 11x17 for tray 1. Note that you must add at least one valid entry into the ``/InputAttributes`` dictionary; if all are null, then the ``setpagedevice`` will fail with a ``/configurationerror``.


.. code-block:: postscript

   << /InputAttributes
     currentpagedevice /InputAttributes get
     dup { pop 1 index exch null put } forall

     dup 0 << /PageSize [612 792] >> put
     dup 1 << /PageSize [612 792] >> put
     dup 2 << /PageSize [792 1224] >> put
   >> setpagedevice

After this code runs, then requesting a letter page size (612x792 points) from ``setpagedevice`` will select tray 0, and requesting an 11x17 size will select tray 2. To explicitly request tray 1, run:

.. code-block:: postscript

   << /PageSize [612 792] /MediaPosition 1 >> setpagedevice


At this point, the chosen tray is sent to the device as the (nonstandard) ``%MediaSource`` device parameter. Devices with switchable trays should implement this device parameter in the ``put_params`` procedure. Unlike the usual protocol for device parameters, it is not necessary for devices to also implement ``get_params`` querying of this paramter; it is effectively a write-only communication from the language to the device. Currently, among the devices that ship with Ghostscript, only PCL (``gdevdjet.c``) and PCL/XL (``gdevpx.c``) implement this parameter, but that list may well grow over time.

If the device has dynamic configuration of trays, etc., then the easiest way to get that information into the tray selection logic is to send a ``setpagedevice`` request (if using the standard API, then using ``gsapi_run_string_continue``) to update the ``/InputAttributes`` dictionary immediately before beginning a job.


Tray rotation and the LeadingEdge parameter
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Large, sophisticated printers often have multiple trays supporting both short-edge and long-edge feed. For example, if the paper path is 11 inches wide, then 11x17 pages must always print short-edge, but letter size pages print with higher throughput if fed from long-edge trays. Generally, the device will expect the rasterized bitmap image to be rotated with respect to the page, so that it's always the same orientation with respect to the paper feed direction.

The simplest way to achieve this behavior is to call ``gx_device_request_leadingedge`` to request a ``LeadingEdge`` value ``LeadingEdge`` field in the device structure based on the ``%MediaSource`` tray selection index and knowledge of the device's trays. The default ``put_params`` implementation will then handle this request (it's done this way to preserve the transactional semantics of ``put_params``; it needs the new value, but the changes can't actually be made until all params succeed). For example, if tray 0 is long-edge, while trays 1 and 2 are short-edge, the following code outline should select the appropriate rotation:

.. code-block:: c

   my_put_params(gx_device *pdev, gs_param_list *plist) {
       my_device *dev = (my_device *)pdev;
       int MediaSource = dev->myMediaSource;

       code = param_read_int(plist, "%MediaSource", &MediaSource);

       switch (MediaSource) {
       case 0:
           gx_device_req_leadingedge(dev, 1);
           break;
       case 1:
       case 2:
           gx_device_req_leadingedge(dev, 0);
           break;
       }
       ...call default put_params, which makes the change...

       dev->myMediaSource = MediaSource;
       return 0;
   }

Ghostscript also supports explicit rotation of the page through setting the ``/LeadingEdge`` parameter with ``setpagedevice``. The above code snippet will simply override this request. To give manual setting through ``setpagedevice`` priority, don't change the ``LeadingEdge`` field in the device if its ``LEADINGEDGE_SET_MASK`` bit is set. In other words, simply enclose the above switch statement inside an ``if (!(dev->LeadingEdge & LEADINGEDGE_SET_MASK) { ... }`` statement.

Interaction between LeadingEdge and PageSize
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

As of LanguageLevel 3, PostScript now has two mechanisms for rotating the imaging of the page: the ``LeadingEdge`` parameter described in detail above, and the automatic rotation as enabled by the ``/PageSize`` page device parameter (described in detail in Table 6.2 of the PLRM3). Briefly, the ``PageSize`` autorotation handles the case where the page size requested in ``setpagedevice`` matches the swapped size of the paper source (as set in the ``InputAttributesDictionary``). This mechanism can be, and has been, used to implement long-edge feed, but has several disadvantages. Among other things, it's overly tied to the PostScript language, while the device code above will work with other languages. Also, it only specifies one direction of rotation (90 degrees counterclockwise). Thus, given the choice, ``LeadingEdge`` is to be preferred.

If ``PageSize`` is used, the following things are different:

- The ``PageSize`` array in ``InputAttributes`` is swapped, so it is ``[long short]``.

- The ``.MediaSize`` device parameter is similarly swapped.

- The initial matrix established by the device through the ``get_initial_matrix`` procedure is the same as for the non-rotated case.

- The CTM rotation is done in the ``setpagedevice`` implementation.


.. include:: footer.rst
