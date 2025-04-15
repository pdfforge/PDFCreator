.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: The Ghostscript Library



.. include:: header.rst

.. _Lib.html:


The Core Library
===================================





This document describes the Ghostscript library, a set of procedures to implement the graphics and filtering capabilities that are primitive operations in the PostScript language and in Adobe Portable Document Format (PDF).

Ghostscript is actually two programs: a language interpreter, and a graphics library. The library provides, in the form of C procedures, all the graphics functions of the language, that is, approximately those facilities listed in section 8.1 of the PostScript Language Reference Manual, starting with the graphics state operators. In addition, the library provides some lower-level graphics facilities that offer higher performance in exchange for less generality.



PostScript operator API
--------------------------------------

The highest level of the library, which is the one that most clients will use, directly implements the PostScript graphics operators with procedures named ``gs_XXX``, for instance ``gs_moveto`` and ``gs_fill``. Nearly all of these procedures take graphics state objects as their first arguments, such as:

.. code-block:: bash

   int gs_moveto(gs_state *, double, double);

Nearly every procedure returns an integer code which is ``>= 0`` for a successful return or ``<0`` for a failure. The failure codes correspond directly to PostScript errors, and are defined in ``gserrors.h``.

The library implements all the operators in the following sections of the PostScript Language Reference Manual, with the indicated omissions and with the APIs defined in the indicated ``.h`` files. A header of the form ``A.h`` (``B.h``) indicates that ``A.h`` is included in ``B.h``, so ``A.h`` need not be included explicitly if ``B.h`` is included. Operators marked with * in the "omissions" column are not implemented directly; the library provides lower-level procedures that can be used to implement the operator.

There are slight differences in the operators that return multiple values, since C's provisions for this are awkward. Also, the control structure for the operators involving callback procedures (``pathforall``, ``image``, ``colorimage``, ``imagemask``) is partly inverted: the client calls a procedure to set up an enumerator object, and then calls another procedure for each iteration. The ``...show`` operators, ``charpath``, and ``stringwidth`` also use an inverted control structure.




.. list-table::
      :widths: 30 30 40
      :header-rows: 1

      * - Section (operators)
        - Headers
        - Omissions
      * - Graphics state – device-independent
        - ``gscolor.h`` (``gsstate.h``)

          ``gscolor1.h``

          ``gscolor2.h``

          ``gscspace.h``

          ``gshsb.h``

          ``gsline.h`` (``gsstate.h``)

          ``gsstate.h``

        -
      * - Graphics state – device-dependent
        - ``gscolor.h`` (``gsstate.h``)

          ``gscolor1.h``

          ``gscolor2.h``

          ``gsht.h`` (``gsht1.h``, ``gsstate.h``)

          ``gsht1.h``

          ``gsline.h`` (``gsstate.h``)

        -
      * - Coordinate system and matrix

        - ``gscoord.h``

          ``gsmatrix.h``
        - ``*matrix``, ``*identmatrix``, ``*concatmatrix``, ``*invertmatrix``

      * - Path construction

        - ``gspath.h``

          ``gspath2.h``
        - ``*arct``, ``*pathforall``, ``ustrokepath``, ``uappend``, ``upath``, ``ucache``

      * - Painting

        - ``gsimage.h``

          ``gspaint.h``

          ``gspath2.h``
        - ``*image``, ``*colorimage``, ``*imagemask``, ``ufill``, ``ueofill``, ``ustroke``

      * - Form and pattern
        - ``gscolor2.h``
        - ``execform``

      * - Device setup and output
        - ``gsdevice.h``
        - ``*showpage``, ``*set/currentpagedevice``
      * - Character and font


        - ``gschar.h``

          ``gsfont.h``
        -  ``*`` (all the ``show`` operators), ``definefont``, ``undefinefont``,

           ``findfont``, ``*scalefont``, ``*makefont``, ``selectfont``,

           ``[Global]FontDirectory``, ``Standard/ISOLatin1Encoding``, ``findencoding``



The following procedures from the list above operate differently from their PostScript operator counterparts, as explained here:

``gs_makepattern`` (``gscolor2.h``)
   Takes an explicit current color, rather than using the current color in the graphics state. Takes an explicit allocator for allocating the pattern implementation. See below for more details on gs_makepattern.

``gs_setpattern`` (``gscolor2.h``), ``gs_setcolor`` (``gscolor2.h``), ``gs_currentcolor`` (``gscolor2.h``)
   Use ``gs_client_color`` rather than a set of color parameter values. See below for more details on ``gs_setpattern``.

``gs_currentdash_length/pattern/offset`` (``gsline.h``)
   Splits up ``currentdash`` into three separate procedures.

``gs_screen_init/currentpoint/next/install`` (``gsht.h``)
   Provide an "enumeration style" interface to ``setscreen``. (``gs_setscreen`` is also implemented.)

``gs_rotate/scale/translate`` (``gscoord.h``), ``gs_[i][d]transform`` (``gscoord.h``)
   These always operate on the graphics state CTM. The corresponding operations on free-standing matrices are in ``gsmatrix.h`` and have different names.

``gs_path_enum_alloc/init/next/cleanup`` (``gspath.h``)
   Provide an "enumeration style" implementation of ``pathforall``.

``gs_image_enum_alloc`` (``gsimage.h``), ``gs_image_init/next/cleanup`` (``gsimage.h``)
   Provide an "enumeration style" interface to the equivalent of ``image``, ``imagemask``, and ``colorimage``. In the ``gs_image_t``, ``ColorSpace`` provides an explicit color space, rather than using the current color space in the graphics state; ``ImageMask`` distinguishes ``imagemask`` from ``[color]image``.

``gs_get/putdeviceparams`` (``gsdevice.h``)
   Take a ``gs_param_list`` for specifying or receiving the parameter values. See ``gsparam.h`` for more details.

``gs_show_enum_alloc/release`` (``gschar.h``), ``gs_xxxshow_[n_]init`` (``gschar.h``), ``gs_show_next`` (``gschar.h``)
   Provide an "enumeration style" interface to writing text. Note that control returns to the caller if the character must be rasterized.


This level of the library also implements the following operators from other sections of the Manual:



.. list-table::
      :widths: 30 20 50
      :header-rows: 1

      * - Section (operators)
        - Headers
        - Operators
      * - Interpreter parameter
        - ``gsfont.h``
        - ``cachestatus``, ``setcachelimit``, ``*set/currentcacheparams``
      * - Display PostScript
        - ``gsstate.h``
        - ``set/currenthalftonephase``



In order to obtain the full PostScript Level 2 functionality listed above, ``FEATURE_DEVS`` must be set in the ``makefile`` to include at least the following:


.. code-block::bash

   FEATURE_DEVS=patcore.dev cmykcore.dev psl2core.dev dps2core.dev ciecore.dev path1core.dev hsbcore.dev

The ``*lib.mak`` makefiles mentioned below do not always include all of these features.

Files named ``gs*.c`` implement the higher level of the graphics library. As might be expected, all procedures, variables, and structures available at this level begin with "``gs_``". Structures that appear in these interfaces, but whose definitions may be hidden from clients, also have names beginning with "``gs_``", that is, the prefix, not the implementation, reflects at what level the abstraction is made available.


Patterns
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Patterns are the most complicated PostScript language objects that the library API deals with. As in PostScript, defining a pattern color and using the color are two separate operations.

``gs_makepattern`` defines a pattern color. Its arguments are as follows:

.. list-table::
      :widths: 25 75
      :header-rows: 0

      * - ``gs_client_color *``
        - The resulting ``Pattern`` color is stored here.

          This is different from PostScript, which has no color objects per se,

          and hence returns a modified copy of the dictionary.

      * - ``const gs_client_pattern *``
        - The analogue of the original Pattern dictionary, described in detail just below.

      * - ``const gs_matrix *``
        - Corresponds to the matrix argument of the ``makepattern`` operator.

      * - ``gs_state *``
        - The current graphics state.

      * - ``gs_memory_t *``
        - The allocator to use for allocating the saved data for the Pattern color.

          If this is ``NULL``, ``gs_makepattern`` uses the same allocator that allocated

          the graphics state. Library clients should probably always use ``NULL``.


The ``gs_client_pattern`` structure defined in ``gscolor2.h`` corresponds to the ``Pattern`` dictionary that is the argument to the PostScript language ``makepattern`` operator. This structure has one extra member, ``void *client_data``, which is a place for clients to store a pointer to additional data for the ``PaintProc``; this provides the same functionality as putting additional keys in the ``Pattern`` dictionary at the PostScript language level. The ``PaintProc`` is an ordinary C procedure that takes as parameters a ``gs_client_color *``, which is the ``Pattern`` color that is being used for painting, and a ``gs_state *``, which is the same graphics state that would be presented to the ``PaintProc`` in PostScript. Currently the ``gs_client_color *`` is always the current color in the graphics state, but the ``PaintProc`` should not rely on this. The ``PaintProc`` can retrieve the ``gs_client_pattern *`` from the ``gs_client_color *`` with the ``gs_getpattern`` procedure, also defined in ``gscolor2.h``, and from there, it can retrieve the ``client_data`` pointer.


The normal way to set a ``Pattern`` color is to call ``gs_setpattern`` with the graphics state and with the ``gs_client_color`` returned by ``gs_makepattern``. After that, one can use ``gs_setcolor`` to set further ``Pattern`` colors (colored, or uncolored with the same underlying color space); the rules are the same as those in PostScript. Note that for ``gs_setpattern``, the ``paint.values`` in the ``gs_client_color`` must be filled in for uncolored patterns; this corresponds to the additional arguments for the PostScript ``setpattern`` operator in the uncolored case.

There is a special procedure ``gs_makebitmappattern`` for creating bitmap-based patterns. Its API is documented in ``gscolor2.h``; its implementation, in ``gspcolor.c``, may be useful as an example of a pattern using a particularly simple ``PaintProc``.


Lower-level API
~~~~~~~~~~~~~~~~~~~~~~~~

Files named ``gx*.c`` implement the lower level of the graphics library. The interfaces at the ``gx`` level are less stable, and expose more of the implementation detail, than those at the ``gs`` level: in particular, the ``gx`` interfaces generally use device coordinates in an internal fixed-point representation, as opposed to the ``gs`` interfaces that use floating point user coordinates. Named entities at this level begin with ``gx_``.

Files named ``gz*.c`` and ``gz*.h`` are internal to the Ghostscript implementation, and are not designed to be called by clients.

.. _Lib_VisualTrace:


Visual Trace instructions
-----------------------------

Visual Trace instructions may be inserted in code to provide debug output in a graphical form. Graphics Library doesn't provide a rasterisation of the output, because it is platform dependent. Instead this, client application shpuld set ``vd_trace0`` external variable to Graphics Library, passing a set of callbacks which provide the rasterization.

Visual Trace instructions are defined in ``vdtrace.h``. Debug output must be opened with ``vd_get_dc`` instruction, which obtains a drawing context for the debug output, and must be closed with ``vd_release_dc`` instruction. After opening the output, scale, origin and shift to be set for mapping the debugee coordinate space to window coordinate space. Than painting instructions to be used. Painting may be either immediate or indirect.

Indirect painting uses ``vd_beg_path`` before line output and ``vd_end_path`` after line output, to store a path into a temporary storage. After this ``vd_stroke`` may be used for stroking the path, or ``vd_fill`` may be used for filling the region inside the path.

Immediate painting happens when path construction instructions are involved without ``vd_beg_path`` and ``vd_end_path``. In this case lines and curves are being drawed immediately, when a path construction instruction is executed.

The following table explains the semantics of Visual Trace instructions.


Visual Trace instructions semantics
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. list-table::
      :header-rows: 1

      * - Instruction
        - Function
        - Parameters
      * - ``vd_get_dc``
        - Obtain drawing context
        - ``-T`` option flag value, for which the subsequent output is enabled.
      * - ``vd_release_dc``
        - Release drawing context
        -
      * - ``vd_enabled``
        - Is trace currently enabled ?
        -
      * - ``vd_get_size_unscaled_x``
        - Get the horizontal size of the output window in pixels.
        -
      * - ``vd_get_size_unscaled_y``
        - Get the vertical size of the output window in pixels.
        -
      * - ``vd_get_size_caled_x``
        - Get the horizontal size of the output window in debuggee coordinate units.
        -
      * - ``vd_get_size_caled_y``
        - Get the vertical size of the output window in debuggee coordinate units.
        -
      * - ``vd_get_scale_x``
        - Get the horizontal scale.
        -
      * - ``vd_get_scale_y``
        - Get the vertical scale.
        -
      * - ``vd_get_origin_x``
        - Get the horizontal position of the draft origin in debuggee coordinate space.
        -
      * - ``vd_get_origin_y``
        - Get the vertical position of the draft origin in debuggee coordinate space.
        -
      * - ``vd_set_scale(s)``
        - Set isotripic scale.
        - Debugee space to window space mapping scale, same for both dimentions.
      * - ``vd_set_scaleXY(sx,sy)``
        - Set anisotripic scale.
        - Debugee space to window space mapping scale, one for each dimention.
      * - ``vd_set_origin(x,y)``
        - Set the draft origin.
        - Origin of the draft in debugee space.
      * - ``vd_set_shift(x,y)``
        - Set the draft position.
        - Position of the draft origin in window space (in pixels).
      * - ``vd_set_central_shift``
        - Set the draft position to window center.
        -
      * - ``vd_erase(c)``
        - Fill entire window.
        - Color to fill.
      * - ``vd_beg_path``
        - Begin path construction.
        -
      * - ``vd_end_path``
        - End path construction.
        -
      * - ``vd_moveto(x,y)``
        - Path construction : Set the draft current point.
        - Debugee coordinates.
      * - ``vd_lineto(x,y)``
        - Path construction : Line from current point to specified point.
        - Debugee coordinates.
      * - ``vd_lineto_mupti(p,n)``
        - Path construction : Polyline from current point to specified points.
        - Array of points and its size, debugee coordinates.
      * - ``vd_curveto(x0,y0,x1,y1,x2,y2)``
        - Path construction : Curve (3rd-order Bezier) from current point to

          specified point, with specified poles.
        - 2 poles and the curve ending point, debuggee coordinates.
      * - ``vd_closepath``
        - Path construction : Close the path (is necessary for filling an area).
        -
      * - ``vd_bar(x0,y0,x1,y1,w,c)``
        - Bar from point to point.
        - 2 points (debugee coordinates), width (in pixels) and color.
      * - ``vd_square(x0,y0,w,c)``
        - Square with specified center and size.
        - The center (debugee coordinates), size (in pixels) and color.
      * - ``vd_rect(x0,y0,x1,y1,w,c)``
        - Canonic rectangle with specified coordinites.
        - Coordinates of boundaries (debugee coordinates),

          line width (in pixels) and color.
      * - ``vd_quad(x0,y0,x1,y1,x2,y2,x3,y3,w,c)``
        - Quadrangle with specified coordinites.
        - Coordinates of vertices (debugee coordinates),

          line width (in pixels) and color.
      * - ``vd_curve(x0,y0,x1,y1,x2,y2,x3,y3,c,w)``
        - Curve with width.
        - 4 curve poles (debugee coordinates), color, and width (in pixels).
      * - ``vd_circle(x,y,r,c)``
        - Circle.
        - Center (debugee coordinates), radius (in pixels) and color.
      * - ``vd_round(x,y,r,c)``
        - Filled circle.
        - Center (debugee coordinates), radius (in pixels) and color.
      * - ``vd_stroke``
        - Stroke a path constructed with:

          ``vd_beg_path``, ``vd_moveto``, ``vd_lineto``,

          ``vd_curveto``, ``vd_closepath``, ``vd_end_path``.
        -
      * - ``vd_fill``
        - Fill a path constructed with:

          ``vd_beg_path``, ``vd_moveto``, ``vd_lineto``,

          ``vd_curveto``, ``vd_closepath``, ``vd_end_path``.
        -
      * - ``vd_setcolor(c)``
        - Set a color.
        - Color (an integer consisting of red, green, blue bytes).
      * - ``vd_setlinewidth(w)``
        - Set line width.
        - Width (in pixels).
      * - ``vd_text(x,y,s,c)``
        - Paint a text.
        - Origin point (debugee coordinates), a string, and a color.
      * - ``vd_wait``
        - Delay execution until a resuming command is entered through user interface.
        -


Graphics Library doesn't provide a rasterization of the debug output. Instead it calls callbacks, which are specified by a client, and which may have a platform dependent implementation. The implementation must not use Graphics Library to exclude recursive calls to it from Visual Trace instructions. The callbacks and auxiliary data are collected in the structure ``vd_trace_interface``, explained in the table below.




vd_trace_interface structure
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. list-table::
      :header-rows: 1

      * - Field
        - Purpose
        - Parameters
      * - ``host``
        - A pointer to the rasterizer control block -

          to be provided by client application.

          The type of the field is client dependent.
        -
      * - ``scale_x``, ``scale_y``
        - Scale of debugee coordinate to window coordinate mapping -

          internal work data, don't change.
        -
      * - ``orig_x``, ``orig_y``
        - Draft origin in debugee coordinates -

          internal work data, don't change.
        -
      * - ``shift_x``, ``shift_y``
        - Draft shift in window coordinates -

          internal work data, don't change.
        -
      * - ``get_size_x(I)``
        - Get window width in pixels.
        -
      * - ``get_size_y(I)``
        - Get window height in pixels.
        -
      * - ``get_dc(I,I1)``
        - Obtain drawing context.
        - Pointer to interface block,

          and pointer to copy of the pointer.

          Implementation must set ``*I1`` if it succeeds

          to get a drawing context.
      * - ``release_dc(I,I1)``
        - Release drawing context.
        - Pointer to interface block,

          and pointer to copy of the pointer.

          Implementation must reset ``*I1`` if it succeeds

          to release the drawing context.
      * - ``erase(I,c)``
        - Erase entire window.
        - Background color.
      * - ``beg_path(I)``
        - Begin path construction.
        -
      * - ``end_path(I)``
        - End path construction.
        -
      * - ``moveto(I,x,y)``
        - Set current point.
        - A point in window coordinates.
      * - ``lineto(I,x,y)``
        - Line from current point to specified point.
        - A point in window coordinates.
      * - ``curveto(I,x0,y0,x1,y1,x2,y2)``
        - Curve from current point with specified

          poles to specified point.
        - 3 points in window coordinates.
      * - ``closepath(I)``
        - Close the path.
        -
      * - ``circle(I,x,y,r)``
        - Circle.
        - Center and radius, window coordinates.
      * - ``round(I,x,y,r)``
        - Filled circle.
        - Center and radius, window coordinates.
      * - ``fill(I)``
        - Fill the path.
        -
      * - ``stroke(I)``
        - Stroke the path.
        -
      * - ``setcolor(I,c)``
        - Set color.
        - An integer, consisting of red, green, blue bytes.
      * - ``setlinewidth(I,w)``
        - Set line width.
        - Line width in pixels.
      * - ``text(I,x,y,s)``
        - Draw a text.
        - Coordinates in pixels, and a string.
      * - ``wait(I)``
        - Delay execution until resume command is

          inputted from user.
        -



A full example
-----------------------

The file ``gslib.c`` in the Ghostscript fileset is a complete example program that initializes the library and produces output using it; files named ``*lib.mak`` (such as ``ugcclib.mak`` and ``bclib.mak``) are ``makefiles`` using ``gslib.c`` as the main program. The following annotated excerpts from this file are intended to provide a roadmap for applications that call the library.


.. code-block:: c

   /* Capture stdin/out/err before gs.h redefines them. */
   #include <stdio.h>
   static FILE *real_stdin, *real_stdout, *real_stderr;
   static void
   get_real(void)
   {       real_stdin = stdin, real_stdout = stdout, real_stderr = stderr;
   }

Any application using Ghostscript should include the fragment above at the very beginning of the main program.

.. code-block:: c

   #include "gx.h"

The ``gx.h`` header includes a wealth of declarations related to the Ghostscript memory manager, portability machinery, debugging framework, and other substrate facilities. Any application file that calls any Ghostscript API functions should probably include ``gx.h``.


.. code-block:: c

   /* Configuration information imported from gconfig.c. */
   extern gx_device *gx_device_list[];

   /* Other imported procedures */
   /* from gsinit.c */
   extern void gs_lib_init(P1(FILE *));
   extern void gs_lib_finit(P2(int, int));
   /* from gsalloc.c */
   extern gs_ref_memory_t *ialloc_alloc_state(P2(gs_memory_t *, uint));


The externs above are needed for initializing the library.

.. code-block:: c

   gs_ref_memory_t *imem;
   #define mem ((gs_memory_t *)imem)
   gs_state *pgs;
   gx_device *dev = gx_device_list[0];

   gp_init();
   get_real();
   gs_stdin = real_stdin;
   gs_stdout = real_stdout;
   gs_stderr = real_stderr;
   gs_lib_init(stdout);
   ....
   imem = ialloc_alloc_state(&gs_memory_default, 20000);
   imem->space = 0;
   ....
   pgs = gs_state_alloc(mem);


The code above initializes the library and its memory manager. ``pgs`` now points to the graphics state that will be passed to the drawing routines in the library.

.. code-block:: c

   gs_setdevice_no_erase(pgs, dev);    /* can't erase yet */
   {  gs_point dpi;
      gs_screen_halftone ht;
      gs_dtransform(pgs, 72.0, 72.0, &dpi);
      ht.frequency = min(fabs(dpi.x), fabs(dpi.y)) / 16.001;
      ht.angle = 0;
      ht.spot_function = odsf;
      gs_setscreen(pgs, &ht);
   }

The code above initializes the default device and sets a default halftone screen. (For brevity, we have omitted the definition of ``odsf``, the spot function.)

.. code-block:: c

   /* gsave and grestore (among other places) assume that */
   /* there are at least 2 gstates on the graphics stack. */
   /* Ensure that now. */
   gs_gsave(pgs);


The call above completes initializing the graphics state. When the program is finished, it should execute:

.. code-block:: c

   gs_lib_finit(0, 0);








.. include:: footer.rst










