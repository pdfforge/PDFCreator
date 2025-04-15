.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: Ghostscript PostScript Coding Guidelines


.. include:: header.rst

.. _Ps-style.html:


Ghostscript PostScript Coding Guidelines
============================================



Summary of the coding guidelines
-----------------------------------

- Don't store into literals.

- Use ``loop`` to create a block with multiple exits.

- Use a dictionary or an array for multi-way switches.

- Start every file with a copyright notice, the file name, and a one-line summary.

- Comment every procedure with the arguments and result, and with the function of the procedure unless it's obvious.

- Comment the stack contents ad lib, and particularly at the beginning of every loop body.

- Indent every 2 spaces.

- Always put { at the end of a line, and } at the beginning of a line, unless the contents are very short.

- Always put spaces between adjacent tokens.

- Use only lower-case letters and digits for names, or :ref:`Vienna style names<Naming>`, except for an initial "." for names only used within a single file.

- Don't allocate objects in heavily used code.

- Consider factoring out code into a procedure if it is used more than once.


The many rules that Ghostscript's code follows almost everywhere are meant to produce code that is easy to read. It's important to observe them as much as possible in order to maintain a consistent style, but if you find a rule getting in your way or producing ugly-looking results once in a while, it's OK to break it.



Use of PostScript language features
----------------------------------------

Restrictions
~~~~~~~~~~~~~~~~~

If you need to store a value temporarily, don't write into a literal in the code, as in this fragment to show a character given the character code:


.. code-block:: postscript

   ( ) dup 0 4 -1 roll put show

Instead, allocate storage for it:


.. code-block:: postscript

   1 string dup 0 4 -1 roll put show


Protection
~~~~~~~~~~~~~~~~~

If an object is never supposed to change, use ``readonly`` to make it read-only. This applies especially to permanently allocated objects such as constant strings or dictionaries.

During initialization, and occasionally afterwards, it may be necessary to store into a read-only dictionary, or to store a pointer to a dictionary in local VM into a dictionary in global VM. The operators ``.forceput`` and ``.forceundef`` are available for this purpose. To make these operators inaccessible to ordinary programs, they are removed from ``systemdict`` at the end of initialization: system code that uses them should always use ``bind`` and ``odef`` (or ``executeonly``) to make uses of them inaccessible as well.


Standard constructions
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


Multi-way conditionals
"""""""""""""""""""""""""""

If you write a block of code with more than about 3 exit points, the usual way to do it would be like this:


.. code-block:: postscript

   {
     ... {
       ...1
     } {
       ... {
         ...2
       } {
         ... {
           ...3
         } {
           ...4
         } ifelse
       } ifelse
     } ifelse
   }


However, this causes the 4 logically somewhat parallel code blocks to be indented differently, and as the indentation increases, it becomes harder to see the structure visually. As an alternative, you can do it this way:

.. code-block:: postscript

   {       % The loop doesn't actually loop: it just provides a common exit.
     ... {
       ...1
       exit
     } if
     ... {
       ...2
       exit
     } if
     ... {
       ...3
       exit
     } if
     ...4
     exit
   } loop


Don't forget the final exit, to prevent the loop from actually looping.



Switches
"""""""""""""""""""""""""""

Use a dictionary or an array of procedures to implement a 'switch', rather than a series of conditionals, if there are more than about 3 cases. For example, rather than:


.. code-block:: postscript

   dup /a eq {
     pop ...a
   } {
     dup /b eq {
       pop ...b
     } {
       dup /c eq {
         pop ...c
       } {
         ...x
       } ifelse
     } ifelse
   } ifelse

(or using the loop/exit construct suggested above), consider:

.. code-block:: postscript

   /xyzdict mark
     /a {...a} bind
     /b {...b} bind
     /c {...c} bind
   .dicttomark readonly def
   ...
   //xyzdict 1 index .knownget {
     exch pop exec
   } {
     ...x
   } ifelse


File structuring
~~~~~~~~~~~~~~~~~~~

Every code file should start with comments containing

1. A copyright notice.

2. The name of the file in the form of an RCS Id:

.. code-block:: postscript

   % $Id: filename.ps $

3. A very brief summary (preferably only one line) of what the file contains.

If you create a file by copying the beginning of another file, be sure to update the copyright year and change the file name.



Commenting
------------



If a file has well-defined functional sections, put a comment at the beginning of each section to describe its purpose or function.

Put a comment before every procedure to describe what the procedure does, unless it's obvious or the procedure's function is defined by the PLRM. In case of doubt, don't assume it's obvious. If the procedure may execute a deliberate 'stop' or 'exit' not enclosed in 'stopped' or a loop respectively, that should be mentioned. However, information about the arguments and results should go in the argument and result comment (described just below) if possible, not the functional comment.

Put a comment on every procedure to describe the arguments and results:

.. code-block:: postscript

   /hypot {    % <num1> <num2> hypot <real>
     dup mul exch dup mul add sqrt
   } def

There is another commenting style that some people prefer to the above:

.. code-block:: postscript

   /hypot {    % num1 num2 --> realnum
     dup mul exch dup mul add sqrt
   } def


We have adopted the first style for consistency with Adobe's documentation, but we recognize that there are technical arguments for and against both styles, and might consider switching some time in the future. If you have strong feelings either way, please make your opinion known to us.

Put comments describing the stack contents wherever you think they will be helpful; put such a comment at the beginning of every loop body unless you have a good reason not to.

When you change a piece of code, do *not* include a comment with your name or initials. Also, do *not* retain the old code in a comment, unless you consider it essential to explain something about the new code; in that case, retain as little as possible. (CVS logs do both of these things better than manual editing.) However, if you make major changes in a procedure or a file, you may put your initials, the date, and a brief comment at the head of the procedure or file respectively.


Formatting
-------------

Indentation
~~~~~~~~~~~~~~~~~

Indent 2 spaces per indentation level. You may use tabs at the left margin for indentation, with 1 tab = 8 spaces, but you should not use tabs anywhere else, except to place comments.

Indent { } constructs like this:

.. code-block:: postscript

   ... {
     ...
   } {
     ...
   } ...


If the body of a conditional or loop is no more than about 20 characters, you can put the entire construct on a single line if you want:

.. code-block:: postscript

   ... { ... } if

rather than:


.. code-block:: postscript

   ... {
     ...
   } if


There is another indentation style that many people prefer to the above:

.. code-block:: postscript

   ...
   { ...
   }
   { ...
   } ...

We have adopted the first style for consistency with our C code, but we recognize that there are technical arguments for and against both styles, and might consider switching some time in the future. If you have strong feelings either way, please make your opinion known to us.



Spaces
~~~~~~~~~~~~~~~

Always put spaces between two adjacent tokens, even if this isn't strictly required. E.g.,

.. code-block:: postscript

   /Halftone /Category findresource

not:


.. code-block:: postscript

   /Halftone/Category findresource



Naming
--------

All names should consist only of letters and digits, possibly with an initial ".", except for names drawn from the PostScript or PDF reference manual, which must be capitalized as in the manual. In general, an initial "." should be used for those and only those names that are not defined in a private dictionary but that are meant to be used only in the file where they are defined.

For edits to existing code, names made up of multiple words should not use any punctuation, or capitalization, to separate the words, again except for names that must match a specification. For new code, you may use this convention, or you may use the "Vienna" convention of capitalizing the first letter of words, e.g., ``readSubrs`` rather than ``readsubrs``. If you use the Vienna convention, function names should start with an upper case letter, variable names with a lower case letter. Using the first letter of a variable name to indicate the variable's type is optional, but if you do it, you should follow existing codified usage.



Miscellany
--------------

Some useful non-standard operators
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

``<obj1> <obj2> ... <objn> <n> .execn ...``
   This executes ``obj1`` through ``objn`` in that order, essentially equivalent to: ``<obj1> <obj2> ... <objn> <n> array astore {exec} forall`` except that it doesn't actually create the array.

``<dict> <key> .knownget <value> true``, ``<dict> <key> .knownget false``
   This combines known and get in the obvious way.

``<name> <proc> odef -``
   This defines name as a "pseudo-operator". The value of name will be executable, will have type ``operatortype``, and will be executed if it appears directly in the body of a procedure (like an operator, unlike a procedure), but what will actually be executed will be ``proc``. In addition, if the execution of ``proc`` is ended prematurely (by ``stop``, including the ``stop`` that is normally executed when an error occurs, or ``exit``) and the operand and dictionary stacks are at least as deep as they were when the "operator" was invoked, the stacks will be cut back to their original depths before the error is processed. Thus, if pseudo-operator procedures are careful not to remove any of their operands until they reach a point in execution beyond which they cannot possibly cause an error, they will behave just like operators in that the stacks will appear to be unchanged if an error occurs.

Some useful procedures
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

``<object> <errorname> signalerror -``
   Signal an error with the given name and the given "current object". This does exactly what the interpreter does when an error occurs.


Other
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

If you can avoid it, don't allocate objects (strings, arrays, dictionaries, gstates, etc.) in commonly used operators or procedures: these will need to be garbage collected later, slowing down execution. Instead, keep values on the stack, if you can. The ``.execn`` operator discussed above may be helpful in doing this.

If you find yourself writing the same stretch of code (more than about half a dozen tokens) more than once, ask yourself whether it performs a function that could be made into a procedure.



.. include:: footer.rst