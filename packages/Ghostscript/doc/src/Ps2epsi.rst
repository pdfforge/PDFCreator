.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: Convert PostScript to Encapsulated PostScript Interchange Format


.. include:: header.rst

.. _Ps2epsi.html:


Convert PostScript to Encapsulated PostScript Interchange Format
======================================================================




The information in this document was contributed by `George Cameron`_; please direct any questions about it to him. Because the software described here is no longer being maintained, this document may be obsolete, or inconsistent with ``ps2epsi.1``.

For other information, see the :ref:`Ghostscript overview<Ghostscript Introduction>`.



Overview
-----------

``ps2epsi`` is a utility based on Ghostscript, which takes as input a PostScript file and generates as output a new file which conforms to Adobe's Encapsulated PostScript Interchange (EPSI) format, a special form of Encapsulated PostScript (EPS) which adds to the beginning of the file, as PostScript comments, a low-resolution monochrome bitmap image of the final displayed page. Programs which understand EPSI can use this bitmap as a preview on screen of the full PostScript page. The displayed quality is often not very good, but the final printed version uses the "real" PostScript, and thus has the normal full PostScript quality. Framemaker can use EPSI.

The `Adobe Framemaker`_ DTP system is one application which understands EPSI files, and ``ps2epsi`` has been tested using Framemaker 3.0 on a Sun workstation with a number of PostScript diagrams from a variety of sources. Framemaker on other platforms may also be able to use files made with ``ps2epsi``, although this has not been tested.


Usage
-------

MS-DOS
~~~~~~~~~~

Using the supplied batch file ``ps2epsi.bat``, the command is:

.. code-block:: bash

   ps2epsi infile.ps outfile.epi


where ``infile.ps`` is the original PostScript file, and ``outfile.epi`` is the output EPSI file to be created.


Unix
~~~~~~~~

Using the supplied shell script ``ps2epsi``, the command is:

.. code-block:: bash

   ps2epsi infile.ps [outfile.epsi]

where ``infile.ps`` is the input file and ``outfile.epsi`` is the output EPSI file to be created. If the output filename is omitted, ``ps2epsi`` generates one from the input filename; and any standard extension (``.ps``, ``.cps``, ``.eps`` or ``.epsf``) of the input file is replaced in the output file with the extension ``.epsi``.


Limitations
------------------

Not all PostScript files can be encapsulated, because there are restrictions in what is permitted in a PostScript file for it to be properly encapsulated. ``ps2epsi`` does a little extra work to try to help encapsulation, and it automatically calculates the bounding box required for all encapsulated PostScript files, so most of the time it does a pretty good job. There are certain to be cases, however, when encapsulation fails because of the nature of the original PostScript file.


Files
--------

.. list-table::
   :widths: 50 50
   :header-rows: 1

   * - File
     - Contents
   * - ``ps2epsi.bat``
     - MS-DOS batch file
   * - ``ps2epsi``
     - Unix shell script
   * - ``ps2epsi.ps``
     - Ghostscript program which does the work








.. External links

.. _Adobe Framemaker: http://www.adobe.com/products/framemaker/main.html
.. _George Cameron: george@bio-medical-physics.aberdeen.ac.uk

.. include:: footer.rst