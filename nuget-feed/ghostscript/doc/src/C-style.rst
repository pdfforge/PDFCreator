.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: Ghostscript C Coding Guidelines


.. include:: header.rst

.. _C-style.html:


Ghostscript C Coding Guidelines
===================================


This document describes Ghostscript's C coding conventions. It is primarily prescriptive, documenting what developers should do when writing new code; the companion :ref:`developer documentation<Develop.html>` is primarily descriptive, documenting the way things are.

We encourage following the general language usage and stylistic rules for any code that will be integrated with Ghostscript, even if the code doesn't use Ghostscript's run-time facilities or have anything to do with PostScript, PDF, or page description languages. Ghostscript itself follows some additional conventions; these are documented separately under :ref:`Ghostscript conventions<Conventions>` below.



C language do's and don'ts
------------------------------

There are several different versions of the C language, and even of the ANSI C standard. Ghostscript versions through 7.0 were designed to compile on pre-ANSI compilers as well as on many compilers that implemented the ANSI standard with varying faithfulness. Ghostscript versions since 7.0 do not cater for pre-ANSI compilers: they must conform to the ANSI 1989 standard (ANS X3.159-1989), with certain restrictions and a few conventional additions.

Preprocessor
~~~~~~~~~~~~~~~~

Conditionals
"""""""""""""""

Restrictions:

- Don't assume that ``#if`` will treat undefined names as 0. While the ANSI standard requires this, it may produce a warning.

- In ``.c`` files, don't use preprocessor conditionals that test for individual platforms or compilers. Use them only in header files named ``xxx_.h``.


Macros
"""""""""""

Restrictions:

- Don't redefine a macro, even with the same definition, without using ``#undef``.

- ``CAPITALIZE`` macro names unless there is a good reason *not* to.

- Even though the legacy code contains some macros which contain control flow statments, avoid the use of this in new code and do not create macros which contain hidden control flow, especially 'return'. The use of control flow in macros complicates debug significantly requiring tedious expansion of macros to build a module to be debugged or resorting to disassembly windows to set breakpoints or to trace flow.

- Don't use a macro call within a macro argument if the call expands to a token sequence that includes any commas not within parentheses: this produces different results depending on whether the compiler expands the inner call before or after the argument is substituted into the macro body. (The ANSI standard says that calls must be expanded after substitution, but some compilers do it the other way.)

- Don't use macro names, even inadvertently, in string constants. Some compilers erroneously try to expand them.

- Don't use macros to define shorthands for casted pointers. For instance, avoid:


   .. code-block:: c

      #define fdev ((gx_device_fubar *)dev)

   and instead use:


   .. code-block:: c

      gx_device_fubar * const fdev = (gx_device_fubar *)dev;


   The use of ``const`` alerts the reader that this is effectively a synonym.

- If a macro generates anything larger than a single expression (that is, one or more statements), surround it with ``BEGIN`` and ``END``. These work around the fact that simple statements and compound statements in C can't be substituted for each other syntactically.


- If a macro introduces local variables, only use names that end with an underscore (``_``), such as ``code_``. This avoids clashes both with ordinary variable names (which should never end with an underscore) and with system-defined names (which may begin with an underscore).


Other
~~~~~~~~~~~~~~~~

Restrictions:

Only use ``#pragma`` in files that are explicitly identified as being platform-dependent. Many compilers complain if this is used at all, and some complain if they don't recognize the specific ``pragma`` being requested (both incorrect according to the ANSI standard).


Lexical elements
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Do not use:

- ANSI trigraphs (``??x``).

- Nested comments (``/* /* */ */``) (not ANSI compliant, but often accepted).

- Multi-character character constants ('abc').

- Wide-character character or string constants (``L'x'``, ``L"x"``).

Restrictions:

- Procedure and static variable names must be 31 characters or less.

- Externally visible procedure and variable names must be unique in the first 23 characters.


Scoping (extern, static, ...)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Do not use:

- ``register``

Restrictions:

- Do not allow a global variable (constant) to have more than one non-``extern`` definition, even though some ANSI C compilers allow this. Every global constant should have exactly one definition, in a ``.c`` file, and preferably just one ``extern`` declaration, in a header file.


- ``static`` or variables must be ``const`` and initialized: non-``const`` statically allocated variables are incompatible with reentrancy, and we're in the process of eliminating all of them.

- Do not use ``extern`` in ``.c`` files, only in ``.h`` files, unless you have a very good reason for it (e.g., as in ``iconf.c``). There are too many such externs in the code now: we are eliminating them over time.

- Do not declare the same name as both ``static`` and non-``static`` within the same compilation. (Some compilers complain, some do not.) This is especially a problem for procedures: it is easy to declare a procedure as ``static`` near the beginning of a file and accidentally not declare it ``static`` where it is defined later in the file.

- Even though the ANSI standard allows initialized external declarations (``extern int x = 0``), don't use them.


Scalars
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


Restrictions:

- Avoid using ``char``, except for ``char *`` for a pointer to a string. Don't assume that ``char`` is signed; also don't assume it is unsigned.

- Never cast a ``float`` to a ``double`` explicitly. ANSI compilers in their default mode do all floating point computations in double precision, and handle such casts automatically.

- Don't use ``long long``: even though it is in the ANSI standard, not all compilers support it. Use the ``(u)int64_t`` types from ``stdint_.h`` instead.

- Don't assume anything about whether ``sizeof(long)`` is less than, equal to, or greater than ``sizeof(ptr)``. (However, you can make such comparisons in preprocessor conditionals using ``ARCH_SIZEOF_LONG`` and ``ARCH_SIZEOF_PTR``).


Arrays
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Restrictions:

- Don't declare arrays of size 0. (The newer ANSI standard allows this, but the older one doesn't).

- Don't declare an array of size 1 at the end of a structure to indicate that a variable-size array follows.

- Don't declare initialized ``auto`` arrays.


Typedefs
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Restrictions:

- Don't use ``typedef`` for function types, such as:

.. code-block:: c

   typedef int proc_xyz_t(double, int *);


Many compilers don't handle this correctly -- they will give errors, or do the wrong thing, when declaring variables of type ``proc_xyz_t`` and/or ``proc_xyz_t *``. Instead, do this:

.. code-block:: c

   #define PROC_XYZ(proc) int proc(double, int *)
   PROC_XYZ(some_proc); /* declare a procedure of this type */
   typedef PROC_XYZ((*proc_xyz_ptr_t)); /* define a type for procedure ptrs */

   proc_xyz_ptr_t pp; /* pointer to procedure */


Don't redefine ``typedef``'ed names, even with the same definition. Some compilers complain about this, and the standard doesn't allow it.


Structures
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Restrictions:

- Don't use anonymous structures if you can possibly avoid it, except occasionally as components of other structures. Ideally, use the ``struct`` keyword only for declaring named structure types, like this:

.. code-block:: c

   typedef struct xxx_s {
      ... members ...
   } xxx_t;

- Use ``struct`` only when declaring structure types, never for referring to them (e.g., never declare a variable as type ``struct xxx_s *``).

- Don't assume that the compiler will (or won't) insert padding in structures to align members for best performance. To preserve alignment, only declare structure members that are narrower than an ``int`` if there will be a lot of instances of that structure in memory. For such structures, insert ``byte`` and/or ``short`` padding members as necessary to re-establish ``int`` alignment.

- Don't declare initialized ``auto`` structures.


Unions
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Restrictions:

- Use unions only as components of structures, not as ``typedefs`` in their own right.

- Don't attempt to initialize unions: not all compilers support this, even though it is in the 1989 ANSI standard.


Expressions
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Restrictions:

- Don't assign a larger integer data type to a smaller one without a cast (``int_x = long_y``).

- It's OK to use the address of a structure or array element (``&p->e, &a[i]``) locally, or pass it to a procedure that won't store it, but don't store such an address in allocated storage unless you're very sure of what you're doing.

- Don't use conditional expressions with structure or union values. (Pointers to structures or unions are OK).

- For calling a variable or parameter procedure, use ``ptr->func(...)``. Some old code uses explicit indirection, ``(*ptr->func)(...)``: don't use this in new code.

- Don't write expressions that depend on order of evaluation, unless the order is created explicitly by use of ``||``, ``&&``, ``?:``, ``,``, or function nesting (the arguments of a function must be evaluated before the function is called). In particular, don't assume that the arguments of a function will be evaluated left-to-right, or that the left side of an assignment will be evaluated before the right.

- Don't mix integer and enumerated types ad lib: treat enumerated types as distinct from integer types, and use casts to convert between the two. (- Some compilers generate warnings if you do not do this).


Statements
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Restrictions:

- If you use an expression as a statement, other than an assignment or a function call with void return value, enclose it explicitly in ``DISCARD()``.

- The type of the operand of a ``switch`` must match the type of the case labels, whether the labels are ``ints`` or the members of an ``enum`` type. (Use a cast if necessary).

- It is OK for one case of a switch to "fall through" into another (i.e., for the statement just before a case label not to be a control transfer), but a comment ``/* falls through */`` is required.

- If you are returning an error code specified explicitly (e.g., ``return gs_error_rangecheck``), use ``return_error()`` rather than plain ``return``. However, if the program is simply propagating an error code generated elsewhere, as opposed to generating the error, use return (e.g., ``if (code < 0) return code``).


Procedures
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Restrictions:

- Provide a prototype for every procedure, and make sure the prototype is available at every call site. If the procedure is local to a file (static), the prototype should precede the procedure, in the same file; if the procedure is global, the prototype should be in a header file.

- If a procedure parameter is itself a procedure, do list its parameter types rather than just using ``()``. For example:

   .. code-block:: c

      int foo(int (*callback)(int, int));

   rather than just:

   .. code-block:: c

      int foo(int (*callback)());

- Don't use the ``P*`` macros in new code. (See the Procedures section of `Language extensions`_ below for more information.)

- Always provide an explicit return type for procedures, in both the prototype and the definition: don't rely on the implicit declaration as ``int``.

- Don't use ``float`` as the return type of a procedure, unless there's a special reason. Floating point hardware typically does everything in double precision internally and has to do extra work to convert between double and single precision.

- Don't declare parameters as being of type ``float``, ``short``, or ``char``. If you do this and forget to include the prototype at a call site, ANSI compilers will generate incompatible calling sequences. Use ``double`` instead of ``float``, and use ``int`` or ``uint`` instead of ``short`` or ``char``.


Standard library
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Restrictions:

- Only use library features that are documented in the established ANSI standard (e.g., Harbison & Steele's book). Do not use procedures that are "standards" promulgated by Microsoft (e.g., ``stricmp``), or originate in BSD Unix (e.g., ``strcasecmp``), or were added in later versions of the standard such as C 9X.

- Do not use any features from ``stdio.h`` that assume the existence of ``stdin``, ``stdout``, or ``stderr``. See ``gsio.h`` for the full list. Instead, use ``gs_stdin`` *et al*.



Language extensions
-----------------------


Scoping
~~~~~~~~~~

``static``
   Ghostscript assumes the compiler supports the ``static`` keyword for declaring variables and procedures as local to a particular source file.

``inline``
   ``inline`` is available even if the compiler does not support it. Be aware, however, that it may have no effect. In particular, do not use ``inline`` in header files. Instead, use the ``extern_inline`` facility described just below.

``extern_inline``
   Compilers that do support ``inline`` vary in how they decide whether to (also) compile a closed-code copy of the procedure. Because of this, putting an ``inline`` procedure in a header file may produce multiple closed copies, causing duplicate name errors at link time. ``extern_inline`` provides a safe way to put ``inline`` procedures in header files, regardless of compiler. Unfortunately, the only way we've found to make this fully portable involves a fair amount of boilerplate. For details, please see ``stdpre.h``.

Scalar types
~~~~~~~~~~~~~~~~~~

``bool``, ``true``, ``false``
   ``bool`` is intended as a ``Boolean`` type, with canonical values ``true`` and ``false``. In a more reasonable language, such as Java, ``bool`` is an enumerated type requiring an explicit cast to or from ``int``; however, because C's conditionals are defined as producing ``int`` values, we can't even define ``bool`` as a C ``enum`` without provoking compiler warnings.

   Even though ``bool`` is a synonym for ``int``, treat them as conceptually different types:

      - Initialize or set ``bool`` variables to ``true`` or ``false``, not ``0`` or ``1``.

      - Use the ``Boolean`` operators ``!``, ``&&``, and ``||`` only with ``Booleans``. Don't use the idiom ``!!x`` to create a ``Boolean`` that is true ``iff x != 0``: use ``x != 0``.

      - Use an explicit (``int``) cast to convert a ``Boolean`` to an ``integer``.

``byte``, ``ushort``, ``uint``, ``ulong``
   These types are simply shorthands for unsigned ``char``, ``short``, ``int``, ``long``.

   In addition, the use of ``byte *`` indicates a Ghostscript-style string, with explicit length given separately, as opposed to a null terminated C-style string, which is ``char *``.

``bits8``, ``bits16``, ``bits32``
   These are unsigned ``integer`` types of the given width. Use them wherever the actual width matters: do *not*, for example, use short assuming that it is 16 bits wide. New code can use the C99 fixed-width types from ``stdint_.h``.



.. _Conventions:


Stylistic conventions
--------------------------------

Ghostscript's coding rules cover not only the use of the language, but also many stylistic issues like the choice of names and the use of whitespace. The stylistic rules are meant to produce code that is easy to read. It's important to observe them as much as possible in order to maintain a consistent style, but if you find these rules getting in your way or producing ugly-looking results once in a while, it's OK to break it.

Formatting
~~~~~~~~~~~~~

Indentation
"""""""""""""""

We've formatted all of our code using the GNU ``indent`` program.

.. code-block:: bash

   indent -bad -nbap -nsob -br -ce -cli4 -npcs -ncs \
   -i4 -di0 -psl -lp -lps -nut somefile.c

does a 98% accurate job of producing our preferred style. Unfortunately, there are bugs in all versions of GNU ``indent``, requiring both pre- and post-processing of the code.

Put indentation points every 4 spaces. Never use tab stops!

Don't indent the initial ``#`` of preprocessor commands. However, for nested preprocessor commands, do use indentation between the ``#`` and the command itself. Use 2 spaces per level of nesting, e.g.:

.. code-block:: c

   #ifndef xyz
   #  define xyz 0
   #endif

For assignments (including chain assignments), put the entire statement on one line if it will fit; if not, break it after a ``=`` and indent all the following lines. I.e., format like this:

.. code-block:: c

   var1 = value;
   var1 = var2 = value;
   var1 =
       value;
   var1 =
       var2 = value;
   var1 = var2 =
       value;


But not like this:

.. code-block:: c

   var1 =
   var2 = value;



Indent in-line blocks thus:

.. code-block:: c

   {
      ... declarations ...
      {{ blank line if any declarations above }}
      ... statements ...
   }

Similarly, indent procedures thus:

.. code-block:: c

   return_type
   proc_name(... arguments ...)
   {
      ... declarations ...
      {{ blank line if any declarations above }}
      ... statements ...
   }


If a control construct (if, do, while, or for) has a one-line body, use this:

.. code-block:: c

   ... control construct ...
      ... subordinate simple statement ...

This is considered particularly important so that a breakpoint can be set inside the conditional easily.

If it has a multi-line body, use this:

.. code-block:: c

   ... control construct ... {
      ... subordinate code ...
   }


If the subordinate code has declarations, see blocks above.

For if-else statements, do this:


.. code-block:: c

   if ( ... ) {
      ... subordinate code ...
   } else if ( ... ) {
      ... subordinate code ...
   } else {
      ... subordinate code ...
   }



When there are more than two alternatives, as in the example above, use the above ("parallel") syntax rather than the following ("nested") syntax:


.. code-block:: c

   if ( ... ) {
      ... subordinate code ...
   } else {
      if ( ... ) {
         ... subordinate code ...
      } else {
         ... subordinate code ...
      }
   }

Similarly, for do-while statements, do this:

.. code-block:: c

   do {
      ... body ...
   } while ( ... condition ... );




Spaces
"""""""""""""""

Do put a space:

- After every comma and semicolon, unless it ends a line.

- Around every binary operator other than "``->``" and "``.``", although you can omit the spaces around the innermost operator in a nested expression if you like.

- On both sides of the parentheses of an if, for, or while.


Don't put a space:

- At the end of a line.

- Before a comma or semicolon.

- After unary prefix operators.

- Before the parenthesis of a macro or procedure call.



Parentheses
"""""""""""""""

Parentheses are important in only a few places:

- Around the inner subexpressions in expressions that mix ``&&`` and ``||``, even if they are not required by precedence, for example:
   .. code-block:: c

      (xx && yy) || zz

- Similarly around inner subexpressions in expressions that mix ``&``, ``|``, or shifts, especially if mixing these with other operators, for instance:
   .. code-block:: c

      (x << 3) | (y >> 5)

- In macro definitions around every use of an argument that logically could be an expression, for example:
   .. code-block:: c

      ((x) * (x) + (y) * (y))


Anywhere else, given the choice, use fewer parentheses.




For stylistic consistency with the existing Ghostscript code, put parentheses around conditional expressions even if they aren't syntactically required, unless you really dislike doing this. Note that the parentheses should go around the entire expression, not the condition. For instance, instead of:

.. code-block:: c

   hpgl_add_point_to_path(pgls, arccoord_x, arccoord_y,
      (pgls->g.pen_down) ? gs_lineto : gs_moveto);

use:

.. code-block:: c

   hpgl_add_point_to_path(pgls, arccoord_x, arccoord_y,
      (pgls->g.pen_down ? gs_lineto : gs_moveto));






Preprocessor
~~~~~~~~~~~~~~

Conditionals
""""""""""""""

Using preprocessor conditionals can easily lead to unreadable code, since the eye really wants to read linearly rather than having to parse the conditionals just to figure out what code is relevant. It's OK to use conditionals that have small scope and that don't change the structure or logic of the program (typically, they select between different sets of values depending on some configuration parameter), but where possible, break up source modules rather than use conditionals that affect the structure or logic.

Macros
"""""""""""""

Ghostscript code uses macros heavily to effectively extend the rather weak abstraction capabilities of the C language, specifically in the area of memory management and garbage collection: in order to read Ghostscript code effectively, you simply have to learn some of these macros as though they were part of the language. The current code also uses macros heavily for other purposes, but we are trying to phase these out as rapidly as possible, because they make the code harder to read and debug, and to use the rules that follow consistently in new code.

Define macros in the smallest scope you can manage (procedure, file, or ``.h`` file), and ``#undef`` them at the end of that scope: that way, someone reading the code can see the definitions easily when reading the uses. If that isn't appropriate, define them in as large a scope as possible, so that they effectively become part of the language. This places an additional burden on the reader, but it can be amortized over reading larger amounts of code.

Try hard to use procedures instead of macros. Use "``inline``" if you really think the extra speed is needed, but only within a ``.c`` file: don't put inline procedures in ``.h`` files, because most compilers don't honor "``inline``" and you'll wind up with a copy of the procedure in every ``.c`` file that includes the ``.h`` file.

If you define a macro that looks like a procedure, make sure it will work wherever a procedure will work. In particular, put parentheses around every use of an argument within the macro body, so that the macro will parse correctly if some of the arguments are expressions, and put parentheses around the entire macro body. (This is still subject to the problem that an argument may be evaluated more than once, but there is no way around this in C, since C doesn't provide for local variables within expressions.)

If you define macros for special loop control structures, make their uses look somewhat like ordinary loops, for instance:


.. code-block:: c

   BEGIN_RECT(xx, yy) {
     ... body indented one position ...
   } END_RECT(xx, yy);



If at all possible, don't use free variables in macros -- that is, variables that aren't apparent as arguments of the macro. If you must use free variables, list them all in a comment at the point where the macro is defined.

If you define new macros or groups of macros, especially if they aren't simply inline procedures or named constant values, put some extra effort into documenting them, to compensate for the fact that macros are intrinsically harder to understand than procedures.


Comments
~~~~~~~~~~~~~~

The most important descriptive comments are ones in header files that describe structures, including invariants; but every procedure or structure declaration, or group of other declarations, should have a comment. Don't spend a lot of time commenting executable code unless something unusual or subtle is going on.

Naming
~~~~~~~~~~~~~~

Use fully spelled-out English words in names, rather than contractions. This is most important for procedure and macro names, global variables and constants, values of ``#define`` and ``enum``, ``struct`` and other ``typedef`` names, and structure member names, and for argument and variable names which have uninformative types like ``int``. It's not very important for arguments or local variables of distinctive types, or for local index or count variables.

Avoid names that run English words together: "``hpgl_compute_arc_center``" is better than "``hpgl_compute_arccenter``". However, for terms drawn from some predefined source, like the names of PostScript operators, use a term in its well-known form (for instance, ``gs_setlinewidth`` rather than ``gs_set_line_width``).

Procedures, variables, and structures visible outside a single ``.c`` file should generally have prefixes that indicate what subsystem they belong to (in the case of Ghostscript, ``gs_`` or ``gx_``). This rule isn't followed very consistently.


Types
~~~~~~~~~~~~~~

Many older structure names don't have ``_t`` on the end, but this suffix should be used in all new code. (The ``_s`` structure name is needed only to satisfy some debuggers. No code other than the structure declaration should refer to it.)

Declare structure types that contain pointers to other instances of themselves like this:

.. code-block:: c

   typedef struct xxx_s xxx_t;
   struct xxx_s {
      ... members ...
      xxx_t *ptr_member_name;
      ... members ...
   };


If, to maintain data abstraction and avoid including otherwise unnecessary header files, you find that you want the type ``xxx_t`` to be available in a header file that doesn't include the definition of the structure ``xxx_s``, use this approach:


.. code-block:: c

   #ifndef xxx_DEFINED
   #  define xxx_DEFINED
   typedef struct xxx_s xxx_t;
   #endif
   struct xxx_s {
      ... members ...
   };


You can then copy the first 4 lines in other header files. (Don't ever include them in an executable code file.)

Don't bother using ``const`` for anything other than with pointers as described below. However, in those places where it is necessary to cast a pointer of type ``const T *`` to type ``T *``, always include a comment that explains why you are "breaking const".

Pointers
"""""""""""""""""""""

Use ``const`` for pointer referents (that is, ``const T *``) wherever possible and appropriate.

If you find yourself wanting to use ``void *``, try to find an alternative using unions or (in the case of super- and subclasses) casts, unless you're writing something like a memory manager that really treats memory as opaque.




Procedures
~~~~~~~~~~~~~~~~~~~~~~~

In general, don't create procedures that are private and only called from one place. However, if a compound statement (especially an arm of a conditional) is too long for the eye easily to match its enclosing braces "``{...}``" -- that is, longer than 10 or 15 lines -- and it doesn't use or set a lot of state held in outer local variables, it may be more readable if you put it in a procedure.

Miscellany
""""""""""""""""

Local variables
^^^^^^^^^^^^^^^^^^^^^

Don't assign new values to procedure parameters. It makes debugging very confusing when the parameter values printed for a procedure are not the ones actually supplied by the caller. Instead use a separate local variable initialized to the value of the parameter.

If a local variable is only assigned a value once, assign it that value at its declaration, if possible. For example,

.. code-block:: c

   int x = some expression ;

rather than:

.. code-block:: c

   int x;
   ...
   x = some expression ;

Use a local pointer variable like this to "narrow" pointer types:

.. code-block:: c

   int
   someproc(... gx_device *dev ...)
   {
      gx_device_printer *const pdev = (gx_device_printer *)dev;
      ...
   }

Don't "shadow" a local variable or procedure parameter with an inner local variable of the same name. I.e., don't do this:

.. code-block:: c

   int
   someproc(... int x ...)
   {
      ...
      int x;
      ...
   }



Compiler warnings
~~~~~~~~~~~~~~~~~~~~~~~

We want Ghostscript to compile with no warnings. This is a constant battle as compilers change and new code is added. Work hard to eliminate warnings by improving the code structure instead of patching over them. If the compiler can't figure out that a variable is always initialized, a future reader will probably have trouble as well.




File structuring
--------------------

All files
~~~~~~~~~~~~~~~~~~

Keep file names within the "8.3" format for portability:

- Use only letters, digits, dash, and underscore in file names.

- Don't assume upper and lower case letters are distinct.

- Put no more than 8 characters before the ".", if any.

- If there is a ".", put between 1 and 3 characters after the ".".

For files other than documentation files, use only lower case letters in the names; for HTML documentation files, capitalize the first letter.

Every code file should start with comments containing:

#. A copyright notice.

#. The name of the file in the form of an RCS Id:

.. code-block::

   /* $Id: filename.ext $*/

(using the comment convention appropriate to the language of the file), and

3. A summary, no more than one line, of what the file contains.

If you create a file by copying the beginning of another file, be sure to update the copyright year and change the file name.



.. _CStyle_Makefiles:

Makefiles
~~~~~~~~~~~~~~~~~~


Use the extension ``.mak`` for ``makefiles``.

For each:

.. code-block:: c

   #include "xxx.h"

make sure there is a dependency on ``$(xxx_h)`` in the makefile. If ``xxx`` ends with a "``_``", this rule still holds, so that if you code:

.. code-block:: c

   #include "math_.h"

the ``makefile`` must contain a dependency on "``$(math__h)``" (note the two underscores "``__``").

List the dependencies bottom-to-top, like the ``#include`` statements themselves; within each level, list them alphabetically. Do this also with ``#include`` statements themselves whenever possible (but sometimes there are inter-header dependencies that require bending this rule).

For compatibility with the build utilities on OpenVMS, always put a space before the colon that separates the target(s) of a rule from the dependents.


General C code
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

List ``#include`` statements from "bottom" to "top", that is, in the following order:

#. System includes ("``xxx_.h``").

#. ``gs*.h``

#. ``gx*.h`` (yes, ``gs`` and ``gx`` are in the wrong order).

#. ``s*.h``

#. ``i*.h`` (or other interpreter headers that don't start with "``i``").


Headers (.h files)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

In header files, always use the following at the beginning of a header file to prevent double inclusion:

.. code-block:: c

   {{ Copyright notice etc. }}

   #ifndef <filename>_INCLUDED
   #define <filename>_INCLUDED

   {{ The contents of the file }}

   #endif /* <filename>_INCLUDED */


The header file is the first place that a reader goes for information about procedures, structures, constants, etc., so ensure that every procedure and structure has a comment that says what it does. Divide procedures into meaningful groups set off by some distinguished form of comment.



Source (.c files)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

After the initial comments, arrange C files in the following order:

#. ``#include`` statements.

#. Exported data declarations.

#. Explicit externs (if necessary).

#. Forward declarations of procedures.

#. Private data declarations.

#. Exported procedures.

#. Private procedures.


Be flexible about the order of the declarations if necessary to improve readability. Many older files don't follow this order, often without good reason.


Ghostscript conventions
-----------------------------

Specific names
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The Ghostscript code uses certain names consistently for certain kinds of values. Some of the commonest and least obvious are these two:



code
"""""""

A value to be returned from a procedure.

.. list-table::
   :widths: 50 50
   :header-rows: 0

   * - < 0
     - An error code defined in ``gserrors.h`` (or ``ierrors.h``)
   * - 0
     - Normal return
   * - > 0
     - A non-standard but successful return (which must be documented, preferably with the procedure's prototype)



status
"""""""

A value returned from a *stream* procedure.

.. list-table::
   :widths: 50 50
   :header-rows: 0

   * - < 0
     - An exceptional condition as defined in ``scommon.h``
   * - 0
     - Normal return (or, from the "process" procedure, means that more input is needed)
   * - 1
     - More output space is needed (from the "process" procedure)




Structure type descriptors
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The Ghostscript memory manager requires run-time type information for every structure. (We don't document this in detail here: see the :ref:`Structure descriptors<Develop_Structure_Descriptors>` section of the developer documentation for details.) Putting the descriptor for a structure next to the structure definition will help keep the two consistent, so immediately after the definition of a structure ``xxx_s``, define its structure descriptor:

.. code-block:: c

   struct xxx_s {
      ... members ...
   };
   #define private_st_xxx()  /* in <filename>.c */\
     gs_private_st_<whatever>(st_xxx, xxx_t,\
       "xxx_t", xxx_enum_ptrs, xxx_reloc_ptrs,\
       ... additional parameters as needed ...)

The file that implements operations on this structure (``<filename>.c``) should then include, near the beginning, the line:


.. code-block:: c

   private_st_xxx();


In much existing code, structure descriptors are declared as public, which allows clients to allocate instances of the structure directly. We now consider this bad design. Instead, structure descriptors should always be static; the implementation file should provide one or more procedures for allocating instances, e.g.,

.. code-block:: c

   xxx_t *gs_xxx_alloc(P1(gs_memory_t *mem));


If it is necessary to make a structure descriptor public, it should be declared in its clients as:

.. code-block:: c

   extern_st(st_xxx);


.. _CStyle_Objects:


"Objects"
~~~~~~~~~~~~~~~~~~~~~~~~~

Ghostscript makes heavy use of object-oriented constructs, including analogues of classes, instances, subclassing, and class-associated procedures. However, these constructs are implemented in C rather than C++, for two reasons:

The first Ghostscript code was written in 1986, long before C++ was codified or was well supported by tools. Even today, C++ tools rarely support C++ as well as C tools support C.

C++ imposes its own implementations for virtual procedures, inheritance, run-time type information, and (to some extent) memory management. Ghostscript requires use of its own memory manager, and also sometimes requires the ability to change the virtual procedures of an object dynamically.

Classes
"""""""""

The source code representation of a class is simply a ``typedef`` for a C ``struct``. See Structures_, above, for details.

Procedures
"""""""""""

Ghostscript has no special construct for non-virtual procedures associated with a class. In some cases, the ``typedef`` for the class is in a header file but the struct declaration is in the implementation code file: this provides an extra level of opaqueness, since clients then can't see the representation and must make all accesses through procedures. You should use this approach in new code, if it doesn't increase the size of the code too much or require procedure calls for very heavily used accesses.

Ghostscript uses three different approaches for storing and accessing virtual procedures, plus a fourth one that is recommended but not currently used. For exposition, we assume the class (type) is named ``xxx_t``, it has a virtual procedure ``void (*virtu)(P1(xxx_t *))``, and we have a variable declared as ``xxx_t *pxx``.



#. The procedures are stored in a separate, constant structure of type ``xxx_procs``, of which ``virtu`` is a member. The structure definition of ``xxx_t`` includes a member defined as ``const xxx_procs *procs`` (always named ``procs``). The construct for calling the virtual procedure is ``pxx->procs->virtu(pxx)``.

#. The procedures are defined in a structure of type ``xxx_procs`` as above. The structure definition of ``xxx_t`` includes a member defined as ``xxx_procs procs`` (always named ``procs``). The construct for calling the virtual procedure is ``pxx->procs.virtu(pxx)``.

#. The procedures are not defined in a separate structure: each procedure is a separate member of ``xxx_t``. The construct for calling the virtual procedure is ``pxx->virtu(pxx)``.

#. The procedures are defined in a structure of type ``xxx_procs`` as above. The structure definition of ``xxx_t`` includes a member defined as ``xxx_procs procs[1]`` (always named ``procs``). The construct for calling the virtual procedure is again ``pxx->procs->virtu(pxx)``.

Note that in approach 1, the procedures are in a shared constant structure; in approaches 2 - 4, they are in a per-instance structure that can be changed dynamically, which is sometimes important.

In the present Ghostscript code, approach 1 is most common, followed by 2 and 3; 4 is not used at all. For new code, you should use 1 or 4: that way, all virtual procedure calls have the same form, regardless of whether the procedures are shared and constant or per-instance and mutable.

Subclassing
""""""""""""""""""

Ghostscript's class mechanism allows for subclasses that can add data members, or can add procedure members if approach 1 or 3 (above) is used. Since C doesn't support subclassing, we use a convention to accomplish it. In the example below, ``gx_device`` is the root class; it has a subclass ``gx_device_forward``, which in turn has a subclass ``gx_device_null``. First we define a macro for all the members of the root class, and the root class type. (As for structures in general, classes need a structure descriptor, as discussed in Structures_ above: we include these in the examples below.)


.. code-block:: c

   #define gx_device_common\
       type1 member1;\
       ...
       typeN memberN

   typedef struct gx_device_s {
       gx_device_common;
   } gx_device;

   #define private_st_gx_device()  /* in gsdevice.c */\
     gs_private_st_<whatever>(st_gx_device, gx_device,\
       "gx_device", device_enum_ptrs, device_reloc_ptrs,\
       ... additional parameters as needed ...)


We then define a similar macro and type for the subclass.


.. code-block:: c

   #define gx_device_forward_common\
       gx_device_common;\
       gx_device *target

   typedef struct gx_device_forward_s {
       gx_device_forward_common;
   } gx_device_forward;

   #define private_st_device_forward()  /* in gsdevice.c */\
     gs_private_st_suffix_add1(st_device_forward, gx_device_forward,\
       "gx_device_forward", device_forward_enum_ptrs, device_forward_reloc_ptrs,\
       gx_device, target)


Finally, we define a leaf class, which doesn't need a macro because we don't currently subclass it. (We can create the macro later if needed, with no changes anywhere else.) In this particular case, the leaf class has no additional data members, but it could have some.


.. code-block:: c

   typedef struct gx_device_null_s {
       gx_device_forward_common;
   };

   #define private_st_device_null()  /* in gsdevice.c */\
     gs_private_st_suffix_add0_local(st_device_null, gx_device_null,\
       "gx_device_null", device_null_enum_ptrs, device_null_reloc_ptrs,\
       gx_device_forward)


.. note::

   The above example is not the actual definition of the ``gx_device`` structure type: the actual type has some additional complications because it has a finalization procedure. See ``base/gxdevcli.h`` for the details.

If you add members to a root class (such as ``gx_device`` in this example), or change existing members, do this in the ``gx_device_common`` macro, not the ``gx_device`` structure definition. Similarly, to change the ``gx_device_forward`` class, modify the ``gx_device_forward_common`` macro, not the structure definition. Only change the structure definition if the class is a leaf class (one with no ``_common`` macro and no possibility of subclassing), like ``gx_device_null``.


Error handling
~~~~~~~~~~~~~~~~~~~~~~

Every caller should check for error returns and, in general, propagate them to its callers. By convention, nearly every procedure returns an ``int`` to indicate the outcome of the call:


.. list-table::
   :widths: 50 50
   :header-rows: 0

   * - < 0
     - Error return
   * - 0
     - Normal return
   * - > 0
     - Non-error return other than the normal case



To make a procedure generate an error and return it, as opposed to propagating an error generated by a lower procedure, you should use:


.. code-block:: c

   return_error(error_number);


Sometimes it is more convenient to generate the error in one place and return it in another. In this case, you should use:


.. code-block:: c

   code = gs_note_error(error_number);
   ...
   return code;

In executables built for debugging, the ``-E`` (or ``-Z#``) command line switch causes ``return_error`` and ``gs_note_error`` to print the error number and the source file and line: this is often helpful for identifying the original cause of an error.

See the file ``base/gserrors.h`` for the error return codes used by the graphics library, most of which correspond directly to PostScript error conditions.








.. include:: footer.rst




















