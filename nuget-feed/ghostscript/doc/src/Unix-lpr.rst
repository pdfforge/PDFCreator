.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: Setting Up a Unix lpr Filter for Ghostscript


.. include:: header.rst

.. _Unix-lpr.html:



Setting Up a Unix lpr Filter for Ghostscript
==============================================



*"How do I set up Ghostscript to provide PostScript queues in a standard lpr environment on Unix systems?"* is a Frequently Asked Question amongst Ghostscript users. The two shell scripts described by this document are designed to make this task a little easier. They are:


``unix-lpr.sh``
   A flexible, multi-option print filter.

``lprsetup.sh``
   A shell script which sets up soft links and creates a template insert for the ``printcap`` file.

.. note::

   These shell scripts can be found in the ``lib`` folder of the Ghostscript source tree.


What it can do
----------------------

The print filter resides in the Ghostscript installation directory (often ``/usr/local/bin`` as the default location, but may be something else at your installation), together with a dummy filter directory containing various soft links which point to the filter. It offers the following features:

- Multiple devices supported by a single filter.

- Multiple bit-depths for the same device.

- Multiple number of colours for the same device.

- Direct (single-queue) and indirect (two-queue) setup.

- Support for the standard preprocessing filters if you have the corresponding (whatever)-to-PostScript translators.

- Redirection of diagnostic and programmed output to a ``logfile`` in the spooling directory.

- Maintaining of printer accounting records of the numbers of pages printed by each user (compatible with the ``pac`` command).

- Straightforward editing for further customisation.


Setting it up
---------------

The ``lprsetup.sh`` script needs to have two lines edited before running, to set the printer devices to use and the list of filters available. With this information, it:

- Creates a "filt" subdirectory under the Ghostscript installation directory.

- Creates the links in this directory which enable the filter to determine the parameters for running Ghostscript.

- Automatically generates ``printcap`` entries which should need only a little editing before adding to your system ``printcap`` file.


Editing the device list DEVICES
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

At the top of ``lprsetup.sh`` is a line of the form ``DEVICES={list}``. Replace the example list with your own list of devices. Each entry is the name of a device, followed by three more optional fields, separated by dots "``.``".


.. _field-bits-per-pixel:

Field 1: bits per pixel
""""""""""""""""""""""""

The first field is required only if the printer device understands the ``-dBitsPerPixel=`` switch, which applies only to colour devices. For a particular number N of bits per pixel, add the suffix ``.N`` to the device name, for instance ``cdj500.3``, ``cdj500.24``, etc.

Field 2: colours
""""""""""""""""""""""""

The second field is required only if the printer device understands the setting of the Colors device parameter (as in ``-dColors=``), which applies only to colour devices (and at present is only supported by the ``bjc*`` family of drivers). For a particular number N of colours, suffix ``.N`` to the device name, such as ``bjc600.24.3``, ``bjc600.8.1`` etc.


.. _field_dq:

Field 3: dual queues
""""""""""""""""""""""""

The third field is required in order to use two separate queues for the device, a "raw" queue and a PostScript queue (see `Single or dual queues`_ below). If you want dual queues, add the suffix ``.dq`` ("dual queue") to the name, whether or not a :ref:`bits-per-pixel<field-bits-per-pixel>` suffix has already been added.


Example definition of DEVICES
""""""""""""""""""""""""""""""""""""""""""""""""

Thus the following list supports a :title:`cdj550` device at three different bit depths (24 bpp, 3 bpp and 1 bpp), with a dual queue (that is, a separate queue for the raw data); a monochrome deskjet device with a single queue; and a :title:`djet500` device using a separate queue:


.. code-block:: postscript

   DEVICES="cdj550.24.dq cdj550.3.dq cdj550.1.dq deskjet djet500.dq"

Editing the filter list
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The standard list contains only the generic "``if``" filter, but a commented-out list shows other filters which may be available. If you wish to use the support for these filters, you may need to edit the ``bsd-if`` file to add to the ``PATH`` the directories where the translators are stored, or to change the names of the filters if yours are different. The ``bsd-if`` script is supplied with an example setup using Transcript (a commercial package from Adobe), and ``PBMPLUS``, a freeware package by Jef Poskanzer and others.

Editing the printer port and type
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

You can set the port and port type (parallel or printer) for an attached printer, but for remote printers you'll have to modify the ``printcap.insert`` file yourself.

Modifying printcap.insert
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Running ``lprsetup.sh`` generates a file ``printcap.insert`` which has a template setup for your printer queues. It cannot guarantee to do the whole job, and you will probably need to consult your system documentation and edit this file before you add it to your ``printcap`` file. The file has good defaults for serial printers, as these often cause problems in getting binary data to the printer. However, you may need to change the baud rate, or the handshaking method. Only a small change is required in the ``printcap`` file to use a networked remote printer instead of an attached printer, and an example is given in ``printcap.insert``.

Single or dual queues
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

If you wish to provide a PostScript-only queue (for example, so that all pages printed go through accounting), and the printer port is local to the host machine, a single queue is appropriate -- Ghostscript simply converts PostScript into the printer's native data format and sends it to the port. But if the printer is on a remote networked machine, or if you need to send raw printer data to the printer, you must use two queues. Simply specify the :ref:`dq<field_dq>` option above.




.. include:: footer.rst