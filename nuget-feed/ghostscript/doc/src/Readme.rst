.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: Ghostscript Introduction


.. include:: header.rst

.. _Ghostscript Introduction:
.. _Readme.html:


Introduction
===================================


This document is a roadmap to the Ghostscript documentation.
After looking through it, if you want to install Ghostscript and not only use it, we recommend you read :ref:`How to install Ghostscript<Install.html>`, and :ref:`How to compile Ghostscript<Make.html>` from source code (which is necessary before installing it on Unix and VMS systems).


.. _WhatIsGhostscript:

What is Ghostscript?
----------------------

There are various products in the Ghostscript family; this document describes what they are, and how they are related.





Ghostscript
~~~~~~~~~~~~~~~~

Ghostscript is an interpreter for PostScript® and Portable Document Format (PDF) files.

Ghostscript consists of a PostScript interpreter layer, and a graphics library. The graphics library is shared with all the other products in the Ghostscript family, so all of these technologies are sometimes referred to as Ghostscript, rather than the more correct GhostPDL.

Binaries for Ghostscript and (see below) GhostPDF (included in the Ghostscript binaries) for various systems can be downloaded from `ghostscript.com/download`_. The source can be found in both the Ghostscript and GhostPDL downloads from the same site.





GhostPDF
~~~~~~~~~~~~~~~~

Prior to release 9.55.0 GhostPDF was an interpreter for the PDF page description language built on top of Ghostscript, and written in the PostScript programming language. From 9.55.0 onwards there is a new GhostPDF executable, separate from Ghostscript and written in C rather than PostScript.

This new interpreter has also been integrated into Ghostscript itself, in order to preserve the PDF functionality of that interpreter. For now, the old PostScript-based interpreter remains the default, but the new interpreter is built-in alongside it.

The intention is that the new interpreter will replace the old one, which will be withdrawn.

It is possible to control which interpreter is used with the NEWPDF command-line switch. When this is false (the current default) the old PostScript-based interpreter is used, when NEWPDF is true then the new C-based interpreter is used.





GhostPDL
~~~~~~~~~~~~~~~~

Historically, we’ve used GhostPDL as an umbrella term to encompass our entire line of products. We've now brought all these disparate products together into a single package, called, appropriately enough, GhostPDL.

When running on a printer (or server) GhostPDL now automatically detects the type of data being fed to it and processes it accordingly. The individual interpreters all plug into a top-level module that handles both automatic language detection and Printer Job Language (PJL) based configuration.

The exact set of interpreters present in an installation can be tuned by the integrator for their specific product/use cases.

In addition to our existing PDL modules (PS, PDF, PCL, PXL, and XPS) we have now added new modules to handle a range of common image formats. With these installed, GhostPDL will handle JPEGs (both JFIF and EXIF), PWGs, TIFFs, PNGs, JBIG2s, and JPEG2000s.

GhostPDL is available both under the `GNU Affero GPL license`_ and for `commercial licensing`_ from `Artifex`_.

The source code for GhostPDL can be found from `ghostscript.com/download`_.





GhostPCL
~~~~~~~~~~~~~~~~

GhostPCL is an interpreter for PCL™ and PXL files. This consists of an PCL/PXL interpreter hooked up to the Ghostscript graphics library.

GhostPCL is available both under the `GNU Affero GPL license`_ and for `commercial licensing`_ from `Artifex`_.

Binaries for GhostPCL for various systems can be downloaded from `ghostscript.com/download`_. The source can be found in the GhostPCL/GhostPDL downloads from the same site.




GhostXPS
~~~~~~~~~~~~~~~~

GhostXPS is an interpreter for XPS (XML Paper Specfication) files. This consists of an XPS interpreter hooked up to the Ghostscript graphics library.

GhostXPS is available both under the `GNU Affero GPL license`_ and for `commercial licensing`_ from `Artifex`_.

Binaries for GhostXPS for various systems can be downloaded from `ghostscript.com/download`_. The source can be found in the GhostXPS/GhostPDL downloads from the same site.



Ghostscript Enterprise
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

:title:`Ghostscript Enterprise` is a commercial version of :title:`GhostPDL` which can also read and process a range of common office documents, including :title:`Word`, :title:`PowerPoint` and :title:`Excel`. Find out more in the :ref:`Ghostscript Enterprise section<Ghostscript_Enterprise>`.

.. _URWFontInformation:


URW++ Font Information
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

We rely on two sets of fonts for our products, both from URW++.

Firstly, there is a PostScript Language Level 2 font set (also required
for PDF), in Type 1 font format. These are included with Ghostscript and
GhostPDL, and are distributed under the GNU GPLv2, with an exemption to
allowing embedding in PDF and PostScript files.


Secondly, there is the PCL5 set, in TrueType format. These are required
for GhostPCL and GhostPDL (since the latter includes PCL5 support).

These PCL fonts are NOT FREE SOFTWARE and are NOT distributed under any
GNU GPL/AGPL variant. They are, instead, distributed under the `AFPL license`_
which prohibits commercial use. A copy of this license in included in
the GhostPDL source distribution.


Document roadmap by theme
-----------------------------

What should I read if I'm a new user?
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

- :ref:`How to use Ghostscript<Use.html>`. This includes both a quickstart introduction to the command line version and more extensive reference material.
- detailed information about :ref:`specific devices<Devices.html>` that Ghostscript can use for output.
- more detailed information about how to use Ghostscript under Unix with ``lpr`` :ref:`as a filter<Unix-lpr.html>` for printing.
- for information about known problems or to report a new one, please visit `bugs.ghostscript.com`_ but remember that free versions of Ghostscript come with with NO WARRANTY and NO SUPPORT.


GPL and commercial Ghostscript
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

GPL Ghostscript, Artifex Ghostscript and AFPL Ghostscript are different releases.

- additional information about `GPL Ghostscript releases`_ that is not relevant to commercial versions.

If you run into any questions, or if you are going to be using Ghostscript extensively, you should at least skim, and probably eventually read:

- about the :ref:`fonts distributed with Ghostscript<Fonts.html>`, including how to add or replace fonts.
- a description of :ref:`the Ghostscript language<Language.html>`, and its differences from the documented PostScript language.
- about the :ref:`postscript files distributed with Ghostscript<Psfiles.html>` (other than fonts).


Before building Ghostscript
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

If you are going to compile Ghostscript from source, rather than just use an executable you got from somewhere, you may want to read:

- :ref:`How to build Ghostscript<Make.html>` and :ref:`install it<Install.html>`.


What should I read if I'm *not* a new user?
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

If you have already used Ghostscript, when you receive a new release you should begin by reading this file, then:

- :ref:`News<News.html>`, for incompatible changes and new features in the current release.


What if I'm a developer?
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

If you are going to do any development on or with Ghostscript at all, you should at least look at:

- the :ref:`roadmap documentation<Develop.html>` for Ghostscript's source files and architecture.

If you are going to write a new driver for Ghostscript, you should read:

- the :ref:`guide to the Ghostscript source code<Source.html>`.
- the interface between Ghostscript and :ref:`device drivers<Drivers.html>`.

If you are considering distributing GPL Ghostscript in conjunction with a commercial product, you should read the `license`_ carefully, and you should also read:

- additional clarification of the circumstances under which Ghostscript can be distributed with a commercial product.

If you intend to use Ghostscript in the form of a dynamic link library (DLL) under OS/2 or Microsoft Windows or in the form of shared object under Linux, read:

- documentation on :ref:`Ghostscript Interpreter API<API.html>`.

If you want to use Ghostscript as part of another program, as a callable PostScript language interpreter, and not as a DLL or as a self-contained executable application, you should begin by reading:

- the source file ``imain.h``, the documented API for Ghostscript not as a DLL.

or if you are going to use only the Ghostscript graphics library:

- about the structure of the :ref:`Ghostscript library<Lib.html>` and its interfaces.

What if I'm writing documentation?
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

If you are editing or adding to Ghostscript's existing documentation you should contact us on our Discord channel or the gs-devel mailing list for guidance, links to those are on: `www.ghostscript.com`_.





Presence on the World Wide Web
------------------------------------


Ghostscript's home page
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Ghostscript has a home page on the World Wide Web with helpful information such as the FAQ (Frequently Asked Questions):

`www.ghostscript.com`_


Other material on the WWW
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Much other material about Ghostscript is available on the World Wide Web, both as web pages and as archived Usenet and mailing list discussions. Use the well-known search engines to find such material.




.. External links

.. _www.ghostscript.com: http://www.ghostscript.com/?utm_source=rtd-ghostscript&utm_medium=rtd&utm_content=inline-link
.. _ghostscript.com/download: http://www.ghostscript.com/download/?utm_source=rtd-ghostscript&utm_medium=rtd&utm_content=inline-link
.. _AGPL:
.. _GNU Affero GPL license: http://www.gnu.org/licenses/agpl-3.0.htmll
.. _AFPL license:
.. _AFPL: https://en.wikipedia.org/wiki/Aladdin_Free_Public_License

.. _commercial licensing: https://artifex.com/licensing/commercial?utm_source=rtd-ghostscript&utm_medium=rtd&utm_content=inline-link
.. _Artifex: https://artifex.com/?utm_source=rtd-ghostscript&utm_medium=rtd&utm_content=inline-link
.. _bugs.ghostscript.com: https://bugs.ghostscript.com
.. _license:
.. _GPL Ghostscript releases: https://github.com/ArtifexSoftware/ghostpdl/blob/master/doc/COPYING


.. include:: footer.rst
