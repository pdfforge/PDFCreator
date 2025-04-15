.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: Unsupported devices


.. include:: header.rst

.. _UnsupportedDevices.html:



Unsupported Devices
===================================




These devices are no longer supported and/or superceeded by newer methods. The documentation is kept here for reference. Be advised that these devices will be removed in future versions of Ghostscript.

Supported devices are descripted in :ref:`Details of Ghostscript output devices<Devices.html>`.

For other information, see the :ref:`Ghostscript overview<Readme.html>`. You may also be interested in :ref:`how to build Ghostscript<Make.html>` and :ref:`install it<Install.html>`, as well as the description of the :ref:`driver interface<Drivers.html>`.







H-P 8xx, 1100, and 1600 color inkjet printers
----------------------------------------------------

This section, written by `Uli Wortmann`_, deals with the DeskJet 670, 690, 850, 855, 870, 890, 1100, and 1600.



Drivers contained in ``gdevcd8.c``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The source module ``gdevcd8.c`` contains four generic drivers:


.. list-table::
   :widths: 50 50
   :header-rows: 0

   * - :title:`cdj670`
     - HP DeskJet 670 and 690
   * - :title:`cdj850`
     - HP DeskJet 850, 855, 870, and 1100
   * - :title:`cdj890`
     - HP DeskJet 890
   * - :title:`cdj1600`
     - HP DeskJet 1600


Further documentation
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Credits: Much of the driver is based on ideas derived from the :title:`cdj550` driver of George Cameron. The support for the hp670, hp690, hp890 and hp1600 was added by Martin Gerbershagen.


.. list-table::
   :widths: 10 20 70
   :header-rows: 1

   * - Date
     - Version
     - Comments
   * - 11.11.96
     - Version 1.0
     -
   * - 25.08.97
     - Version 1.2
     - Resolved all but one of the known bugs,

       introduced a couple of perfomance improvements.

       Complete new color-transfer-function handling (see gamma).

   * - 01.06.98
     - Version 1.3
     - Due to the most welcome contribution of

       `Martin Gerbershagen`_, support for the

       hp670, hp690 and hp890

       and hp1600 has been added.

       Martin has also resolved all known bugs.

       Problems: Dark colors are still pale.


The hp690 is supported through the hp670 device, the hp855, hp870 and the hp1100 through the hp850 device. The driver needs no longer special switches to be invoked except ``-sDEVICE=cdj850``, ``-sDEVICE=CDJ890``, ``-sDEVICE=CDJ670``, or ``-sDevice=CDJ1600``. The following switches are supported:


.. list-table::
   :widths: 30 20 50
   :header-rows: 0

   * - ``-dPapertype=``
     - 0
     - plain paper [default]
   * -
     - 1
     - bond paper
   * -
     - 2
     - special paper
   * -
     - 3
     - glossy film
   * -
     - 4
     - transparency film
   * -
     -
     - Currently the lookup tables are unsuited for printing on special

       paper or transparencies. For these please revert to the gamma functions.

   * - ``-dQuality=``
     - -1
     - draft
   * -
     - 0
     - normal [default]
   * -
     - 1
     - presentation
   * - ``-dRetStatus=``
     - 0
     - C-RET off
   * -
     - 1
     - C-RET on [default]
   * - ``-dMasterGamma=``
     - 3.0
     - [default = 1.0]


.. note::

      To take advantage of the calibrated color-transfer functions, be sure not to have any gamma statements left! If you need to (i.e., for overhead transparencies), you still can use the gamma functions, but they will override the built-in calibration. To use gamma in the traditional way, set ``MasterGamma`` to any value greater than 1.0 and less than 10.0. To adjust individual gamma values, you have to additionally set ``MasterGamma`` to a value greater than 1.0 and less than 10.0. With the next release, gamma functions will be dropped.


When using the driver, be aware that printing at 600dpi involves processing large amounts of data (> 188MB !). Therefore the driver is not what you would expect to be a fast driver ;-) This is no problem when printing a full-sized color page (because printing itself is slow), but it's really annoying if you print only text pages. Maybe I can optimize the code for text-only pages in a later release. Right now, it is recommended to use the highest possible optimisation level your compiler offers. For the time being, use the :title:`cdj550` device with ``-sBitsPerPixel=3`` for fast proof prints. If you simply want to print 600dpi BW data, use the cdj550 device with ``-sBitsPerPixel=8`` (or 1).

Since the printer itself is slow, it may help to set the process priority of the gs process to "regular" or even less. On a 486/100MHz this is still sufficient to maintain a continuous data flow. Note to OS/2 users: simply put the gs window into the background or minimize it. Also make sure that ``print01.sys`` is invoked without the ``/irq`` switch (great speed improvement under Warp4).

The printer default settings compensate for dot-gain by a calibrated color-transfer function. If this appears to be too light for your business graphs, or for overhead transparencies, feel free to set ``-dMasterGamma=1.7``. Furthermore, you may tweak the gamma values independently by setting ``-dGammaValC``, ``-dGammaValM``, ``-dGammaValY`` or ``-dGammaValK`` (if not set, the values default to ``MasterGamma``). This will only work when ``-dMasterGamma`` is set to a value greater than 1.0.

Depending on how you transfer the files, under UNIX you may need to remove the CRs of the CR-LF sequence used for end-of-line on DOS-based (MS Windows-based) systems. You can do this in unpacking the files with ``unzip -a hp850.zip``.

To compile with ``gs5.x`` or later, simply add to your ``makefile``:


.. code-block:: bash

   DEVICE_DEVS4=cdj850.dev cdj670.dev cdj890.dev cdj1600.dev





H-P 812, 815, 832, 880, 882, 895, and 970 color inkjet printers
-------------------------------------------------------------------------

This section, written by `Matthew Gelhaus`_, deals with the DeskJet 812, 815, 832, 880, 882, 895, and 970.

This is a modified version of the HP8xx driver written by `Uli Wortmann`_. More information and download are available at `gelhaus.net/hp880c`_.




Drivers contained in ``gdevcd8.c``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The source module ``gdevcd8.c`` contains one generic driver:


.. list-table::
   :widths: 50 50
   :header-rows: 0

   * - :title:`cdj880`
     - HP DeskJet 812, 815, 832, 880, 882, 895, and 970



Further documentation
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Credits: This driver is based on the :title:`cdj850` driver by Uli Wortmann, and shares the same internal structure, although the PCL3+ interpretation has changed.

.. list-table::
   :widths: 10 20 70
   :header-rows: 1

   * - Date
     - Version
     - Comments
   * - 15.03.99
     - Version 1.3
     - Initial version, based on Version 1.3 of Uli Wortmann's driver.
   * - 26.02.00
     - Version 1.4beta
     - Greatly improved color handling & dithering, but not yet complete enough to use for text.


All printers are supported through the cdj880 device. Invoke with ``-sDEVICE=cdj880``. The following switches are supported:


.. list-table::
   :widths: 30 20 50
   :header-rows: 0

   * - ``-dPapertype=``
     - 0
     - plain paper [default]
   * -
     - 1
     - bond paper
   * -
     - 2
     - special paper
   * -
     - 3
     - glossy film
   * -
     - 4
     - transparency film
   * -
     -
     - Currently the lookup tables are unsuited for printing on special

       paper or transparencies. For these please revert to the gamma functions.

   * - ``-dQuality=``
     - -1
     - draft
   * -
     - 0
     - normal [default]
   * -
     - 1
     - presentation
   * - ``-dMasterGamma=``
     - 3.0
     - [default = 1.0]



The printer default settings compensate for dot-gain by a pre-defined color-transfer function. If this appears to be too light for your business graphs, or for overhead transparencies, feel free to set ``-dMasterGamma=1.7``. Furthermore, you may tweak the gamma values independently by setting ``-dGammaValC``, ``-dGammaValM``, ``-dGammaValY`` or ``-dGammaValK`` (if not set, the values default to ``MasterGamma``). This will only work when ``-dMasterGamma`` is set to a value greater than 1.0.

To compile with ``gs6.x`` or later, simply add to your ``makefile``:


.. code-block:: bash

   DEVICE_DEVS4=$(DD)cdj880.dev




H-P color inkjet printers
--------------------------------

This section, written by George Cameron, deals with the DeskJet 500C, DeskJet 550C, PaintJet, PaintJet XL, PaintJet XL300, the DEC LJ250 operating in PaintJet-compatible mode.

Drivers contained in ``gdevcdj.c``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The source module ``gdevcdj.c`` contains six generic drivers:


.. list-table::
   :widths: 50 50
   :header-rows: 0

   * - :title:`cdj500`
     - HP DeskJet 500C and 540C
   * - :title:`cdj550`
     - HP DeskJet 550C, 560C, 660C, 660Cse
   * - :title:`pjxl300`
     - HP PaintJet XL300, DeskJet 1200C, and CopyJet
   * - ``pjtest``
     - HP PaintJet
   * - ``pjxltest``
     - HP PaintJet XL
   * - :title:`declj250`
     - DEC LJ250

All these drivers have 8-bit (monochrome), 16-bit and 24-bit (colour) and for the DJ 550C, 32-bit (colour, CMYK mode) options in addition to standard colour and mono drivers. It is also possible to set various printer-specific parameters from the command line, for example:


.. code-block:: bash

   gs -sDEVICE=cDeskJet -dBitsPerPixel=16 -dDepletion=1 -dShingling=2 tiger.eps


.. note ::

   The old names :title:`cdeskjet`, :title:`cdjcolor` and :title:`cdjmono` drivers have been retained; however, their functionality duplicates that available using the drivers above (and :title:`cdeskjet` is identical to :title:`cdj500`). That is, we can use:

   .. code-block:: bash

      gs -sDEVICE=cdj500 -dBitsPerPixel=24   for cdjcolor, and
      gs -sDEVICE=cdj500 -dBitsPerPixel=1 for cdjmono


Default paper size
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

If the preprocessor symbol A4 is defined, the default paper size is ISO A4; otherwise it is U.S. letter size (see about :ref:`paper sizes<Known paper sizes>` in the usage documentation). You can :ref:`specify other paper sizes<Use_PaperSize>` on the command line, including A3 for the PaintJet XL and PaintJet XL300, as also explained in the :ref:`usage documentation<Use.html>`.

DeskJet physical limits
~~~~~~~~~~~~~~~~~~~~~~~~~~

The DeskJet's maximum printing width is 2400 dots, or 8 inches (20.32cm). The printer manuals say that the maximum recommended printing height on the page is 10.3 inches (26.16cm), but since this is obviously not true for A4 paper, and I have been unable to detect any problems in printing longer page lengths, this would seem to be a rather artificial restriction.

All DeskJets have 0.5 inches (1.27cm) of unprintable bottom margin, due to the mechanical arrangement used to grab the paper. Side margins are approximately 0.25 inches (0.64cm) for U.S. letter paper, and 0.15 inches (0.38cm) for A4.

Printer properties (command-line parameters)
"""""""""""""""""""""""""""""""""""""""""""""""""""

Several printer "properties" have been implemented for these printers. Those available so far are all integer quantities, and thus may be specified, for instance, like

.. code-block:: bash

   gs -dBitsPerPixel=32 -dShingling=1 ...

which sets the ``BitsPerPixel`` parameter to 32 and the ``Shingling`` parameter to 1.


Bits per pixel
"""""""""""""""""""""""""""""""""""""""""""""""""""

If the preprocessor symbol ``BITSPERPIXEL`` is defined as an integer (see below for the range of allowable values), that number defines the default bits per pixel (bit depth) for the generic drivers. If the symbol is undefined, the default is 24 bits per pixel. It is, of course, still possible to specify the value from the command line as described below. Note also that the cDeskJet, cdjcolor and cdjmono drivers are unaffected by setting this symbol, as their default settings are predefined to be 1, 3 and 24 respectively.

All of the drivers in ``gdevcdj.c`` accept a command line option to set the ``BitsPerPixel`` property. This gives considerable flexibility in choosing various tradeoffs among speed, quality, colour, etc. The valid numbers are:


.. list-table::
   :widths: 30 70
   :header-rows: 1

   * - BITSPERPIXEL
     - Comments
   * - 1
     - A standard Ghostscript monochrome driver, using black

       ink (by installing the separate mono cartridge in the

       case of the DeskJet 500C, or automatically for the other printers).
   * - 3
     - A standard Ghostscript colour driver, using internal dithering.

       This is fast to compute and to print, but the clustered

       dithering can lose some detail and colour fidelity.
   * - 8
     - An "error-diffusion" monochrome driver which uses Floyd-Steinberg

       dithering to print greyscale images.

       The patterns are much more randomised than with the normal

       clustered dithering, but the data files can be much larger

       and somewhat slower to print.
   * - 16
     - A "cheaper" version of the 24-bit driver, which generates

       Floyd-Steinberg colour dithered output using the minimum

       memory (this may be helpful when using Ghostscript has not

       been compiled using a 16-bit build environment).

       The quality can be almost as good as the 24-bit version.
   * - 24
     - A high-quality colour driver using Floyd-Steinberg

       dithering for maximum detail and colour range.

       However, it is very memory-intensive, and thus can

       be slow to compute. It tends to produce rather

       larger raw data files, so they can also take longer to print.
   * - 32
     - Only for the DeskJet 550C, which uses the black cartridge

       and the colour cartridge simultaneously (that is, CMYK printing).

       This printer can both be faster and give higher quality than

       the DeskJet 500C, because of the true black ink.

       (Note that the 24-bit mode also permits CMYK printing on

       this printer, and uses less memory). Any differences

       between 24-bit and 32-bit should be small.


DeskJet properties
"""""""""""""""""""""""""""""""""""""""""""""""""""

.. list-table::
   :widths: 30 10 60
   :header-rows: 1

   * - Name
     - Type
     - Comments
   * - BlackCorrect
     - `int`
     - Colour correction to give better blacks when using

       the DJ500C in colour mode. For example, the

       default of 4 reduces the cyan component to 4/5.

       Range accepted: 0 - 9 (0 = none).
   * - Shingling
     - `int`
     - Interlaced, multi-pass printing:

       0 = none, 1 = 50%, 2 = 25%, 2 is best and slowest.
   * - Depletion
     - `int`
     - "Intelligent" dot-removal:

       0 = none, 1 = 25%, 2 = 50%, 1 best for graphics?

       Use 0 for transparencies.

PaintJet XL300 / PaintJet XL properties
"""""""""""""""""""""""""""""""""""""""""""""""""""


.. list-table::
   :widths: 30 10 60
   :header-rows: 1

   * - Name
     - Type
     - Comments
   * - PrintQuality
     - `int`
     - Mechanical print quality: -1 = fast, 0 = normal, 1 = presentation.

       Fast mode reduces ink usage and uses single-pass operation for

       some media types. Presentation uses more ink and the maximum number

       of passes, giving slowest printing for highest quality.
   * - RenderType
     - `int`
     - 0 driver does dithering

       1 snap to primaries

       2 snap black to white, others to black

       3 ordered dither

       4 error diffusion

       5 monochrome ordered dither

       6 monochrome error diffusion

       7 cluster ordered dither

       8 monochrome cluster ordered dither

       9 user-defined dither (not supported)

       10 monochrome user-defined dither ns.


The PaintJet (non-XL) has no additional properties.




Gamma correction
~~~~~~~~~~~~~~~~~~~~~

One consequence of using Floyd-Steinberg dithering rather than Ghostscript's default clustered ordered dither is that it is much more obvious that the ink dots are rather larger on the page than their nominal 1/180-inch or 1/300-inch size (clustering the dots tends to minimise this effect). Thus it is often the case that the printed result is rather too dark. A simple empirical correction for this may be achieved by preceding the actual PostScript file to be printed by a short file which effectively sets the gamma for the device, such as:


.. code-block:: bash

   gs ... gamma.ps colorpic.ps -c quit

where ``gamma.ps`` is:

.. code-block:: postscript

   %!
   /.fixtransfer {
     currentcolortransfer 4 {
       mark exch
       dup type dup /arraytype eq exch /packedarraytype eq or
       1 index xcheck and { /exec load } if
       0.333 /exp load
       ] cvx 4 1 roll
     } repeat setcolortransfer
   } bind odef
   .fixtransfer
   /setpagedevice { setpagedevice .fixtransfer } bind odef


This does the gamma correction after whatever correction the device might be doing already. To do the correction before the current correction:

.. code-block:: postscript

   %!
   /.fixtransfer {
     currentcolortransfer 4 {
       mark 0.333 /exp load 4 -1 roll
       dup type dup /arraytype eq exch /packedarraytype eq or
       1 index xcheck and { /exec load } if
       ] cvx 4 1 roll
     } repeat setcolortransfer
   } bind odef
   .fixtransfer
   /setpagedevice { setpagedevice .fixtransfer } bind odef


This example sets the gamma for R, G, and B to 3, which seems to work reasonably well in practice.



HP's resolution-enhanced mode for Inkjet printers
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

This feature is available on HP's more recent inkjet printers, including the DeskJet 520 (mono), 540 (mono or colour) and 560C (mono and colour). The colour and monochrome drivers for the HP DeskJet 550c are (probably) the best you will get for use with Ghostscript, for the following reasons.

These printers do not offer true 600×300dpi resolution. Those that print in colour are strictly 300×300dpi in colour mode, while in mono mode there is a pseudo 600×300dpi mode with the restriction that you can't print two adjacent dots. In effect what you have is 600dpi dot positioning, but on average you don't get more dots per line. This provides the possibility, for instance, to have sharper character outlines, because you can place dots on the edges nearer to their ideal positions. This is why it is worth doing.

However, HP will not support user-level programming of this resolution-enhanced mode, one reason being that (I understand) all the dot spacing has to be done by the driver, and if you get it wrong, you can actually damage the print head.

To summarise, you may lose a smidgin of (potential) text clarity using the 550c drivers (:title:`cdj550`, :title:`cdjcolor`, :title:`cdjmono` etc.), but other than that, they are the ones for the job.



General tips
~~~~~~~~~~~~~~~~~~~~~

For all the printers above, the choice of paper is critically important to the final results. The printer manuals suggest type of paper, but in general, smoother, less fibrous types give better results. In particular, the special ink-jet paper can make a big difference: colours are brighter, but most importantly, there is almost no colour bleed, even with adjacent areas of very heavy inking. Similarly the special coated transparencies also work well (and ordinary transparencies do not work at all!).

The Unix procedure :ref:`unix-lpr.sh<Unix-lpr.html>` provides one example of setting up a multi-option colour PostScript lpr queue on Unix systems, and includes the ability to choose a range of different colour options and printer accounting and error logging.

Caveat emptor! It is not always easy for me to test all of these drivers, as the only colour printer I have here is the DeskJet 500C. I rely on others to test drivers for the additional machines and report their findings back to me.



Canon BJC-8200 printer
-----------------------------

This section was contributed by the author of the :title:`uniprint` configuration files for the Canon BJC-8200, `Stephan C. Buchert`_. These files also handle the Japanese Canon F850 printer.


.. warning::

   Usage of this program is neither supported nor endorsed by the Canon corporation. Please see the Ghostscript license regarding warranty.


Introduction
~~~~~~~~~~~~~~

The Canon Bubble Jet printer BJC-8200 is designed for printing digital photos and halftone images. Software drivers for Windows 95-2000 and Mac are usually included and can be downloaded from the Canon web sites for the US market. If these drivers cannot be used for some reason, then at present Ghostscript is probably the alternative giving the best results.

The BJC-8200 has features not found among the specs of earlier bubble jet models (except the even more advanced BJC-8500) and is advertised to offer:

#. Microfine droplet technology.
#. Support for printing on a new type of paper, Photo Paper Pro.
#. A printhead capable of printing up to 1200 DpI.
#. Individual ink tanks for 6 colors.
#. An internal status monitor reporting low ink back to a driver.
#. An optional color scanner cartridge for up to 600 DpI resolution.

Access to features 5 and 6 requires use of the original Canon drivers for the foreseeable future. This README is about getting the printer features 1-3 working with Ghostscript. No (re)compilation of Ghostscript is normally required.

Ghostscript comes with a relatively highly configurable driver, called uniprint_, for printers which understand raster images in various propriety formats. Most options for this driver are usually organized into files having the suffix ``.upp``. Ghostscript versions >= 5.10 (or even earlier) include such :title:`uniprint` control files for the Canon BJC-610. They work also well for some other Canon Bubble Jet models, for example for my BJC-35vII. But when using them for a BJC-8200 the result is unsatisfactory.

The :title:`uniprint` control files for the BJC-8200
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

After some experimenting with the options for :title:`uniprint` I have obtained quite satisfactory prints with :ref:`my printer<myPrinter>`. This distribution includes six new :title:`uniprint` control files:

- ``bj8pp12f.upp``
- ``bj8hg12f.upp``
- ``bj8gc12f.upp``
- ``bj8oh06n.upp``
- ``bj8ts06n.upp``
- ``bj8pa06n.upp``

They are included in Ghostscript ``>=6.21``. For older versions you can put them anywhere in the Ghostscript search path (type ``gs -h`` to see the path), but should perhaps add the files to the directory with the other ``*.upp`` files. This is ``/usr/share/ghostscript/gs6.01/lib`` in my RedHat 6.1 Linux box with Aladdin Ghostscript 6.01.

Here is an explanation of my file name convention: the prefix "bj8" should perhaps be used for the Canon BJC-8200 and compatible (like the Japanese F850 and perhaps the non-Japanese BJC-8500) models. The next two letters indicate the print media:

- **pp** - "Photo Paper Pro".
- **hg** - "High Gloss Photo Film".
- **gc** - "Glossy Photo Cards".
- **oh** - "OHP transparencies".
- **ts** - "T-shirt transfer".
- **pa** - "Plain Paper".


The numbers at positions 6 and 7 indicate the resolution

- **12** - 1200x1200 DpIxDpI.
- **06** - 600x600 DpIxDpI.

The last letter stands for a quality factor that effects also the print speed (presumably related to the number of passes that the printhead makes).

- **f** - highest quality.
- **n** - normal quality.

Printing a postcard size (~10x15 cm^2) image at 1200x1200 DpI^2 takes about 3 minutes. The output of Ghostscript is then typically 4-5 MByte. The bootleneck seems to be the transfer of the raster image in run-length encoded Canon format to the printer (via the parallel port on my system) or the printer's speed, not Ghostscript or the :title:`uniprint` renderer.


Further Optimization for the Canon BJC-8200
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

So far I have only experimented with the printer initialization code at the beginning of each page (``-dupBeginPageCommand``) and the resolution (``-r``). Other options, particularly the transfer arrays (``-dupBlackTransfer``, ``-dupCyanTransfer``, ``-dupMagentaTransfer``, ``-dupYellowTransfer``) and the margins (``-dupMargins``) were simply copied from the files for the BJC-610, but they may need to be changed for optimized performance.

Here is information useful for changing or adding :title:`uniprint` control files for the BJC-8200:

In ``-dupBeginPageCommand=...`` use the line:

.. code-block:: postscript

   1b28 64 0400 04b0 04b0


for 1200x1200 resolution, and:

.. code-block:: postscript

1b28 64 0400 0258 0258

for 600x600. The ``-r`` option in the control file must of course match this line. Other resolutions might work as well, but I didn't try.


Crucial are the numbers in the lines like:


.. code-block:: postscript

   1b28 63 0300 3005 04
                   ^  ^
       Plain Paper 0  4 Highest quality
  OHP transparency 2  .
  T-shirt transfer 3  .
 Glossy Photo Film 5  .
  High Gloss Paper 6  0 Lowest quality
   Photo Paper Pro 9


Outlook
~~~~~~~~~~~~

Presently :title:`uniprint` can use the black (K), cyan (C), magenta (M), and yellow (Y) colors in the BJC-8200. The unused colors are photo (or light) cyan (c) and magenta (m). Also the Canon driver seems to use only CMYK, for example when printing on Photo Paper Pro in "Camera" or "SuperPhoto" mode. These modes supposedly produce prints of the best quality that the Canon driver can offer. Other modes of Canon driver do use up to all six color cartridges (CMYKcm). Therefore expanding :title:`uniprint`'s capabilities for six colors would be interesting, but it may not increase the output quality of 6-color printers such as the BJC-8200 drastically.

More control files for :title:`uniprint` could be added in order to offer more versatility for controlling the BJC-8200 within a Ghostscript installation. The number of possible combinations for media type, resolution and print quality factor is very large, many combinations would not make much sense, many might be used here and there, but relatively rarely. The user would have to remember a name for each combination that is used.

A better way would be to let the user patch optionally a user owned or system wide :title:`uniprint` control file before each print via some print tool. This is similar to the approach taken by Canon with their driver for Windows. Similarly a :title:`uniprint` tool could also incorporate other functions such as printing test and demo pages and the low ink warning once the protocol for this is known. Clearly it would be difficult to code such a :title:`uniprint` tool for all the platforms where Ghostscript is running.


Usage on RedHat Linux
~~~~~~~~~~~~~~~~~~~~~~~~

In order to install a BJC-8200 printer on a RedHat Linux system with RedHat's printtool, you need also to insert with a text editor the contents of the file ``bj8.rpd`` into the RedHat printer database ``/usr/lib/rhs/rhs-printfilters/printerdb``. Insert it most appropriately after the section:


.. code-block::

   StartEntry: U_CanonBJC610
   .
   .
   .
   EndEntry

   < --- insert here "bj8.rpd" from this distribution:
   < --- StartEntry: U_CanonBJC8200
         .
         .
         .

.. _myPrinter:

.. note::

    Actually I have a F850, not a BJC-8200. That model is sold for the Japanese market only. The specs and also the external look are the same as those of the BJC-8200 models for the American and European markets. I expect that the raster image mode which is used exclusively by Ghostscript is entirely compatible for both models.




Other Canon BubbleJet (BJC) printers
---------------------------------------

This section was contributed by the author of the drivers, Yves Arrouye. The drivers handle Canon BJC-600, BJC-4xxx, BJC-70, Stylewriter 2x00, and BJC-800 printers.

History
~~~~~~~~~~

The BJC-600 driver was written in the first place by Yoshio Kuniyoshi and later modified by Yves Arrouye. We tried to make it evolve synchronously, though Yoshio cannot be reached since a long time ago. The drivers are based on code for the HP printers by George Cameron (in fact, they are in the same file!), so he's the first person to thank.

The 2.00 version of the drivers was a complete rewrite of the driver (arguments, optimization, colour handling, in short: everything!) by Yves Arrouye. That release was also the first one to be able to use the full width of an A3 paper size. PostScript Printer Description (PPD) files for the drivers were released with version 2.15. They are incomplete, but they can be used to drive the printers' main features.

Configuring and building the BJC drivers
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Modify values in ``gdevbjc.h``.

Configure the drivers by modifying the default values in the file ``gdevbjc.h`` or on the compilation line. If you don't do that, the drivers use reasonable defaults that make them work "as expected". All default values shown here are defined in that file.


CMYK-to-RGB color conversion
""""""""""""""""""""""""""""""""""

By default, the drivers use the same algorithm as Ghostscript to convert CMYK colors to RGB. If you prefer to use Adobe formulas, define ``USE_ADOBE_CMYK_RGB`` when compiling. (See the top of the file ``gdevcdj.c`` to see the difference between the two.)

Vertical centering of the printable area
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

The drivers center the imageable area horizontally but not vertically, so that what can be printed does use the most of the output media. If you define ``BJC_DEFAULT_CENTEREDAREA`` when compiling, then the top and bottom margins will be the same, resulting in a (smaller) vertically centered imageable area also.

Page margins
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

If you define ``USE_RECOMMENDED_MARGINS``, then the top and bottom margins will be the same (that is, ``BJC_DEFAULT_CENTEREDAREA`` will be defined for you) and the margins will be the 12.4mm recommended by Canon. Since margins are complicated (because one must rely on the mechanical precision of the printer), the drivers do something about the bottom margin: by default the bottom margin is 9.54mm for the BJC-600 driver and 7mm for the BJC-800. If you define ``USE_TIGHT_MARGINS``, then the bottom margin is 7mm for both drivers (but I never managed to get my own BJC-600 to print a line on this low bound, hence the larger default). Regardless of the presence of this definition, ``USE_FIXED_MARGINS`` will not allow the BJC-800 to use the lower 7mm bottom margin, so if you have a problem with the bottom margin on a BJC-800, just define that (without defining ``USE_TIGHT_MARGINS``, of course).

A quick way to be sure the margins you selected is to print a file whose contents are:

.. code-block::

   %!
   clippath stroke showpage

If the margins are okay, you will get a rectangle visibly surrounding the printable area. If they're not correct, one or more of the sides will be either incomplete or completely unprinted.


Makefile and compilation
""""""""""""""""""""""""""""""

Make sure the :title:`bjc600` or :title:`bjc800` devices are in :ref:`DEVICE_DEVS in the makefile<Selecting features and devices>`; that is, look in the ``makefile`` for your platform and add them if necessary -- they may already be there. As of Ghostscript 5.10, for instance, one ``makefile`` has:

.. code-block:: bash

   DEVICE_DEVS6=bj10e.dev bj200.dev bjc600.dev bjc800.dev


Use of the drivers
~~~~~~~~~~~~~~~~~~~~~~

There are two drivers here. The :title:`bjc600` one supports the BJC-600 and BJC-4xxx (maybe the BJC-70 as well) and the :title:`bjc800` one supports the BJC-800 series. Remarks here that apply to both drivers use the name ``bjc``.

Supported Options and Defaults
"""""""""""""""""""""""""""""""""""""""""

.. note ::

   "options", "properties", and "parameters" designate the same thing: device parameters that you can change.

Giving an option an incorrect value causes an error. Unless stated otherwise, this error will be a ``rangecheckerror``. Options may be set from the Ghostscript command line (using the ``-d`` and ``-s`` switches or other predetermined switches if they have an effect on the driver) or using the PostScript Level 2 ``setpagedevice`` operator if Ghostscript has been compiled with the ``level2`` or ``level3`` device (which it should ;-)). There are no special-purpose operators such as one was able to find in Level 1 printers.

The ``bjc`` uses 24 bits per pixel by default (unless you change the value of ``BJC_BITSPERPIXEL``), corresponding to CMYK printing. Supported modes are 1 bpp and 4 bpp (gray levels), 8 bpp, 16 bpp, 24 bpp and 32 bpp (colours). Colours are preferably stored in the CMYK model (which means, for example, that with 16 bpp there are only 16 different shades of each color) but it is possible to store them as RGB color for some depths. Some modes do Floyd-Steinberg dithering and some don't, but use the default Ghostscript halftoning (in fact, when halftoning is used, dithering takes also place but because of the low point density it is usually not efficient, and thus invisible).

**Descriptions of printing modes by bpp and Colors**


.. list-table::
   :widths: 10 10 80
   :header-rows: 1

   * - bpp
     - Colors
     - Mode
   * - 32
     - 4
     - CMYK colour printing, Floyd-Steinberg dithering
   * - 24
     - 4
     - The same. (But each primary colour is stored on 6 bits instead of 8.)
   * - 24
     - 3
     - RGB colour printing, Floyd-Steinberg dithering. This mode does not use the black

       cartridge (that's why it exists, for when you don't want to use it ;-)).

       Each primary colour is stored in 8 bits as in the 32/4 mode, but black generation

       and under-color removal are done on the driver side and not by Ghostscript,

       so you have no control over it. (This mode is no longer supported in this driver.)

   * - 16
     - 4
     - CMYK colour printing, halftoned by Ghostscript. F-S dithering is still

       visible here (but the halftone patterns are visible too!).

   * - 8
     - 4
     - The same.(But each primary colour is stored in 2 bits instead of 4.)
   * - 8
     - 3
     - RGB colour printing. This mode is not intended for use. What I mean is

       that it should be used only if you want to use custom halftone screens

       and the halftoning is broken using the 8/4 mode (some versions of

       Ghostscript have this problem).
   * - 8
     - 1
     - Gray-level printing, Floyd-Steinberg dithering
   * - 1
     - 1
     - Gray-level printing halftoned by Ghostscript


These modes are selected using the ``BitsPerPixel`` and ``Colors`` integer options (either from the command line or in a PostScript program using ``setpagedevice``). See below.

A note about darkness of what is printed: Canon printers do print dark, really. And the Floyd-Steinberg dithering may eventually darken your image too. So you may need to apply gamma correction by calling Ghostscript as in:

.. code-block:: bash

   gs -sDEVICE=bjc600 gamma.ps myfile.ps

where ``gamma.ps`` changes the gamma correction (here to 3 for all colors); 0.45 gives me good results, but your mileage may vary. The bigger the value the lighter the output:

.. code-block:: bash

   { 0.45 exp } dup dup currenttransfer setcolortransfer

The drivers support printing at 90dpi, 180dpi and 360dpi. Horizontal and vertical resolutions must be the same or a limitcheck error will happen. A ``rangecheck`` will happen too if the resolution is not 90 ×2^N. If the driver is compiled with ``-DBJC_STRICT`` a ``rangecheck`` also happens if the resolution is not one of those supported. This is not the case, as we expect that there may be a 720dpi bjc some day.


Here are the various options supported by the ``bjc`` drivers, along with their types, supported values, effects, and usage:

``BitsPerPixel (int)``
   Choose the depth of the page. Valid values are 1, 8, 16, 24 (the default) and 32.
   Note that when this is set for the first time, the ``Colors`` property is automatically adjusted unless it is also specified. The table here shows the corresponding color models and the rendering method visible: "``GS``" for Ghostscript halftoning and "``F-S``" for Floyd-Steinberg dithering. When both are present it means that the dithering of halftones is visible. Default choices are indicated by asterisk "``*``".

   **Valid colors values for allowed BitsPerPixel values**


   .. list-table::
      :header-rows: 1

      * - bpp
        - Colors
        - Default
        - Color model
        - Dithering
      * - 32
        - 4
        -
        - CMYK
        - F-S
      * - 24
        - 4
        - ``*``
        - CMYK
        - F-S
      * -
        - 3
        -
        - RGB
        - F-S
      * - 16
        - 4
        -
        - CMYK
        - GS, F-S
      * - 8
        - 4
        - ``*``
        - CMYK
        - GS
      * -
        - 3
        -
        - RGB
        - GS
      * -
        - 1
        -
        - K (CMYK)
        - F-S
      * - 1
        - 1
        - ``*``
        - K (CMYK)
        - GS

   Also note that automagical change of one parameter depending on the other one does not work in a ``setpagedevice`` call. This means that if you want to change ``BitsPerPixel`` to a value whose valid ``Colors`` values do not include the actual ``Colors`` value, you must change ``Colors`` too.


``Colors (int)``
   Choose the number of color components from among 1, 3 and 4 (the default). This setting cannot be used in a PostScript program, only on Ghostscript's command line. See ProcessColorModel below for what to use to change the number of colors with PostScript code.
   Note that setting this property does limit the choices of BitsPerPixel. As for the previous property, its first setting may induce a setting of the "other value" (BitsPerPixel here). The table here indicates valid combinations with "V", default values with asterisk "*".

   **Valid BitsPerPixel values for allowed Colors values**

   *BitsPerPixel OK values*

   .. list-table::
      :header-rows: 1

      * - Colors
        - Type
        - 32
        - 24
        - 16
        - 8
        - 1
      * - 4
        - CMYK
        - V
        -
        - V
        - V
        -
      * - 3
        - RGB
        -
        - ``*``
        -
        - V
        -
      * - 1
        - K
        -
        -
        -
        - V
        - ``*``


   Also note that automagical change of one parameter depending on the other one does not work in a ``setpagedevice`` call. This means that if you want to change ``Colors`` to a value whose valid ``BitsPerPixel`` values don't include the actual ``BitsPerPixel`` value, you must change ``BitsPerPixel`` too.



``ProcessColorModel`` *(symbol)*
   A symbol taken from ``/DeviceGray``, ``/DeviceRGB`` or ``/DeviceCMYK`` which can be used to select 1, 3 or 4 colors respectively. Note that this parameter takes precedence over ``Colors``, and that both affect the same variable of the driver. (See ``Colors`` above for values combined with ``BitsPerPixel``.)

``HWResolution`` *(floats array)*
   An array of two floats giving the horizontal and vertical resolution in dots per inch from among 90, 180 and 360 (the default). Both values must be the same. On the Ghostscript command line, the resolution may be changed with the ``-r`` switch.

``ManualFeed`` *(bool)*
   Indicate that the sheets won't be fed automatically by the printer, false by default. (Not meaningful on the BJC-600, I fear.)

``MediaType`` *(string)*
   The media to print on, chosen from among "PlainPaper", "CoatedPaper", "TransparencyFilm", "Envelope", "Card" and "Other". Default is "PlainPaper". For "Envelope", "Card" or "Other" the driver puts the printer into thick mode automatically regardless of the actual media weight.

``MediaWeight`` *(int or null)*
   The weight of the media in grams per square meter. Null (the default) indicates that the weight is of no importance. If the specified media weight is greater than 105 (that is, the value of the compilation default ``BJC???_MEDIAWEIGHT_THICKLIMIT``) then the printer will be set to use thick paper.

``PrintQuality`` *(string)*
   The quality of printing.

   .. list-table::
      :header-rows: 1

      * - Value
        - bjc600
        - bjc800
        - Comments
      * - Low
        -
        - X
        - Has the effect of making only two printing passes instead of four,

          so should be twice the speed; known as "CN" (Color Normal) mode

      * - Draft
        - X
        - X
        - Unlights the "HQ" light on a BJC-600
      * - Normal
        - X
        - X
        - Default for both drivers; lights the "HQ" light on a BJC-600
      * - High
        - X
        - X
        - Means 200% black and 100% C


``DitheringType`` *(string)*
   Dithering algorithm from between "Floyd-Steinberg" and "None". "None" is the default for 1/1 print mode, "Floyd-Steinberg" for other modes. At the moment this parameter is read-only, though no error is generated if one tries to change it. This parameter is not of much value at the moment and is here mainly to reserve the name for future addition of dithering algorithms.

``PrintColors`` *(int)*
   Mask for printing color. If 0, use black for any color; otherwise the value must be the sum of any of 1 (cyan), 2 (magenta), 4 (yellow) and 8 (black), indicating which colors will be used for printing. When printing colour, only colours specified will be printed (this means that some planes will be missing if a color's value above is omitted). When printing grays, black is used if it is present in the PrintColors; otherwise, the image is printed by superimposing each requested color.

``MonochromePrint`` *(bool)*
   For :title:`bjc600` only, false by default. Substitute black for Cyan, Magenta and Yellow when printing -- useful, for example, to get some monochrome output of a dithered printing This is a hardware mechanism as opposed to the previous software one. I think that using this or setting ``PrintColors`` to 0 will give the same results.


.. note::

   The ``MediaType`` and ``ThickMedia`` options will be replaced by the use of the device ``InputAttributes`` and ``OutputAttributes`` as soon as possible. Please note too that the print mode may be reset at the start of printing, not at the end. This is the expected behaviour. If you need to reset the printer to its default state, simply print a file that does just a ``showpage``.


Device information
"""""""""""""""""""""""

Here is other information published by the driver that you will find in the ``deviceinfo`` dictionary.

``OutputFaceUp`` *(bool)*
   This has the boolean value true, indicating that the sheets are stacked face up.

``Version`` *(float)*
   In the form ``M.mmpp``, where ``M`` is the major version, ``mm`` the bjc driver's minor version, and ``pp`` the specific driver minor version (that is, ``M.mm`` will always be the same for the :title:`bjc600` and :title:`bjc800` drivers).

``VersionString`` *(string)*
   A string showing the driver version and other indications. At the moment, things like "a" or "b" may follow the version to indicate alpha or beta versions. The date of the last change to this version is given in the form MM/DD/YY (no, it won't adapt to your locale).


Hardware margins
"""""""""""""""""""""""

The BJC printers have top and bottom hardware margins of 3mm and 7.1mm respectively (Canon says 7mm, but this is unusable because of the rounding of paper sizes to PostScript points). The left margin is 3.4mm for A4 and smaller paper sizes, 6.4mm for U.S. paper sizes, envelopes and cards. It is 4.0mm for A3 paper on the BJC-800.

The maximum printing width of a BJC-600 printer is 203mm. The maximum printing width of a BJC-800 printer is 289mm on A3 paper, 203mm on U.S. letter and ISO A4 paper.

PostScript printer description (PPD) files
""""""""""""""""""""""""""""""""""""""""""""""

The files ``CBJC600.PPD`` and ``CBJC800.PPD`` (whose long names are, respectively, ``Canon_BubbleJetColor_600.ppd`` and ``Canon_BubbleJetColor_800.ppd``) are PPD files to drive the features of the :title:`bjc600` and :title:`bjc800` drivers. They can be used, for example, on NextStep systems (presumably on OpenStep systems too) and on Unix systems with Adobe's TranScript and ``pslpr`` (not tested). The files are not complete at the moment. Please note that NextStep's printing interface does not correctly enforce constraints specified in these files (in ``UIConstraints`` descriptions): you must force yourself to use valid combinations of options.

Customizing the PPD files
""""""""""""""""""""""""""""""""""""""""""""""

By default the PPD files are set for U.S. letter size paper, and they use a normalized transfer function. If you choose to use A4 printing by default, you must replace "Letter" with "A4" in these (noncontiguous) lines:

.. code-block::

   [...]
   *DefaultPageSize: Letter
   [...]
   *DefaultRegion: Letter
   [...]
   *DefaultImageableArea: Letter
   [...]

Some versions of Ghostscript have problems with normalized colors, which makes them add magenta in gray levels. If you experience this problem, in the PPD file replace the line:

.. code-block::

   *DefaultTransfer: Normalized

with the alternate line:

.. code-block::

   *DefaultTransfer: Null

The "thick media" option is implemented by choosing a value of 120 or 80 (for thick and thin media respectively) for the ``MediaWeight`` feature of the drivers. If you ever change the threshold for thick media in the driver code, you may need to change the values in the PPD files too.

All customization should be done using the "``*Include:``" feature of PPD files so that your local changes will be retained if you update the PPD files.



How to report problems
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Yves Arrouye no longer maintains this driver, and will not answer questions about it. If you are posting a question about it in a public form, please be as descriptive as possible, and please send information that can be used to reproduce the problem. Don't forget to say which driver you use, and in what version. Version information can be found in the source code of the driver or by issuing the following command in a shell:

.. code-block:: bash

   echo "currentpagedevice /VersionString get ==" | gs -q -sDEVICE=bjc600 -


Acknowledgements
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

I am particularly grateful to `Yoshio Kuniyoshi`_ without whom I'd never make these drivers, and also to L. Peter Deutsch, who answered all my (often silly) questions about Ghostscript's driver interface.

Thanks also to the people who volunteered to beta-test the v2.x BJC drivers: `David Gaudine`_, `Robert M. Kenney`_, `James McPherson`_ and `Ian Thurlbeck`_ (listed alphabetically) were particularly helpful by discovering bugs and helping find out exact paper margins on printers I don't have access to.

And many thanks to `Klaus-Gunther Hess`_ for looking at the dithering code and devising a good CMYK dithering algorithm for the Epson Stylus Color, which I then adapted to the code of these drivers.




Epson Stylus color printer (see also :title:`uniprint`)
----------------------------------------------------------

This section was contributed by `Gunther Hess`_, who also wrote uniprint_, a later set of drivers. You should probably see the section on uniprint_ for whether it might be better for your uses than this driver.

Usage
~~~~~~~~~

This driver is selected with ``-sDEVICE=stcolor``, producing output for an Epson Stylus Color at 360dpi resolution by default. But it can do much more with this printer, and with significantly better quality, than with the default mode; and it can also produce code for monochrome versions of the printer. This can be achieved via either command-line options or Ghostscript input. For convenience a PostScript file is supplied for use as an initial input file. Try the following command:

.. code-block:: bash

   gs -sDEVICE=stcolor -r{Xdpi}x{Ydpi} stcolor.ps {YourFile.ps}


where {Xdpi} is one of 180, 360, or 720 and {Ydpi} is one of 90, 180, 360, or 720. The result should be significantly better. You may use stcolor.ps with other devices too, but I do not recommend this, since it does nothing then. stcolor.ps should be available with binary distributions and should reside in the same directory as other Ghostscript initialization files or in the same directory as the files to be printed. Thus if Ghostscript is part of your printer-spooler, you can insert:

.. code-block:: postscript

   (stcolor.ps) findlibfile { pop run } if pop

in files you want to use the improved algorithms. You may want to adapt ``stcolor.ps`` file to your specific needs. The methods and options for this are described here, but this description is restricted to Ghostscript options, while their manipulation at the PostScript level is documented in the material on the relationship of Ghostscript and PostScript and in ``stcolor.ps``.


Options
~~~~~~~~~

Now to explain the options (as written on my UNIX system). The order is somehow related to their use during the printing process:

``-dUnidirectional``
   Force unidirectional printing, recommended for transparencies

``-dMicroweave``
   Enable the printer's "microweave" feature; see "What is weaving?" below.

``-dnoWeave``
   Disable any Weaving (overrides ``-dMicroweave``)

``-dSoftweave``
   Enable the driver's internal weaving. Note that Softweave works only with the original Stylus Color and the PRO-Series.

``-sDithering=`` *{name}*
   Select another dithering algorithm (name) from among:

   .. list-table::
      :widths: 20 80
      :header-rows: 1

      * - Dithering name
        - Comments
      * - ``gscmyk``
        - fast color output, CMYK process color model (default)
      * - ``gsmono``
        - fast monochrome output
      * - ``gsrgb``
        - fast color output, RGB process color model
      * - ``fsmono``
        - Floyd-Steinberg, monochrome
      * - ``fsrgb``
        - Floyd-Steinberg, RGB process color model (almost identical to

          the ``cdj550/bjc`` algorithm)
      * - ``fsx4``
        - Floyd-Steinberg, CMYK process color model (shares code with ``fsmono`` and

          ``fsrgb``, but is algorithmically really bad)
      * - ``fscmyk``
        - Floyd-Steinberg, CMYK process color model and proper modifications for CMYK
      * - ``hscmyk``
        - modified Floyd-Steinberg with CMYK model ("hs" stands for "hess" not

          for "high speed", but the major difference from ``fscmyk`` is speed)
      * - ``fs2``
        - algorithm by Steven Singer (RGB) should be identical to ``escp2cfs2``.


``-dBitsPerPixel={1...32}``
   number of bits used for pixel storage; the larger the value, the better the quality -- at least in theory. In ``fsrgb`` one can gain some speed by restricting to 24 bits rather than the default 30.

``-dFlag0``
   causes some algorithms to select a uniform initialisation rather than a set of random values. May yield a sharper image impression at the cost of dithering artifacts. (Applies to ``hscmyk`` and all ``fs`` modes, except for ``fs2``, which always uses a constant initialization.)

``-dFlag1 ... -dFlag4``
   Available for future algorithms.

``-dColorAdjustMatrix='{three, nine, or sixteen floating-point values}'``
   This is a matrix to adjust the colors. Values should be between -1.0 and 1.0, and the number of values depends on the color model the selected algorithm uses. In RGB and CMYK modes a matrix with 1.0 on the diagonal produces no transformation. This feature is really required, but I could not identify a similar feature at the language level, so I implemented it, but I don't know reasonable values yet.

``-dCtransfer='{float float ...}'`` or ``-dMtransfer=..., -dY..., -dK...`` or ``-dRtransfer='{float float ...}'`` or ``-dG..., -dB...`` or ``-dKtransfer='{float float ...}'``
   Which you use depends on the algorithm, which may be either either CMYK, RGB or monochrome. The values are arrays of floats in the range from 0 to 1.0, representing the visible color intensity for the device. One may achieve similar effects with ``setcolortransfer`` at the language level, but this takes more time and the underlying code for the driver-specific parameters is still required. The size of the arrays is arbitrary and the defaults are "{0.0 1.0}", which is a linear characteristic. Most of the code in stcolor.ps are better transfer arrays.


``-dKcoding='{float...}'`` , ``-dC..., -dM...`` etc.
   Arrays between 0.0 and 1.0, controlling the internal coding of the color values. Clever use of these arrays may yield further enhancements, but I have no experience yet. (To be discontinued with version 2.x.)

``-sModel=st800``
   Causes output to be suitable for the monochrome Stylus 800 (no weaving, no color).

``-sOutputCode=`` *{name}*
   Can be either "``plain``", "``runlength``" or "``deltarow``" and changes the ESC/P2 coding technique used by the driver. The default is to use runlength encoding. "``plain``" selects uncompressed encoding and generates enormous amounts of data.

``-descp_Band=`` *1/8/15/24*
   Number of nozzles of scanlines used in printing, Useful only with -dnoWeave. Larger Values yield smaller code, but this doesn't increase the printing speed.

``-descp_Width=`` *N*
   Number of pixels Printed in each scan Line. (Useful only when tuning margins; see below)

``-descp_Height=`` *pixels*
   Length of the entire page in pixels. (Parameter of "``ESC(C``" in default initialization.)

``-descp_Top=`` *scan lines*
   Top margin in scan lines. (First parameter of "``ESC(c``" in default initialization.)

``-descp_Bottom=`` *scan lines*
   Bottom margin in scan lines. (Second parameter of "``ESC(c``" in default initialization.)

``-sescp_Init=`` *"string"*
   Override for the initialization sequence. (Must set graphics mode 1 and units.)

``-sescp_Release=`` *"string"*
   Overrides the release sequence, "``ESC @ FF``" by default.

ESC/P2 allows any resolutions to be valid in theory, but only ``-r360x360`` (the default) and ``-r720x720`` (not on STC-IIs ? and :title:`st800`) are known to work with most printers.


**Valid option combinations – Stylus I & Pro-Series only**

.. list-table::
      :widths: 25 25 25 25
      :header-rows: 1

      * - Resolution
        - escp_Band
        - Weave usable
        - escp_Band & number of passes
      * - 180x90
        - 15
        - noWeave
        -
      * - 180x180
        - 1, 8, 24
        - noWeave, Microweave
        - 15/2 SoftWeave
      * - 180x360
        -
        -
        - 15/4 SoftWeave
      * - 180x720
        -
        -
        - 15/8 SoftWeave
      * - 360x90
        - 15
        - noWeave
        -
      * - 360x180
        - 1, 8, 24
        - noWeave, Microweave
        - 15/2 SoftWeave
      * - 360x360
        - 1, 8, 24
        - noWeave, Microweave
        - 15/4 SoftWeave
      * - 360x720
        -
        -
        - 15/8 SoftWeave
      * - 720x90
        - 15
        - noWeave
        -
      * - 720x180
        -
        -
        - 15/2 SoftWeave
      * - 720x360
        -
        -
        - 15/4 SoftWeave
      * - 720x720  1  noWeave, Microweave
        - 1
        - noWeave, Microweave
        - 15/8 SoftWeave


.. warning ::

   Beware: there are only few validity checks for parameters. A good example is ``escp_Band``: if you set this, the driver uses your value even if the value is not supported by the printer. You asked for it and you got it!



Application note and FAQ
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Quite a bunch of parameters. Hopefully you never need any of them, besides feeding stcolor.ps to Ghostscript in front of your input.

After answering some questions over fifty times I prepared a FAQ. Here is version 1.3 of the FAQ, as of stcolor version 1.20 (for Ghostscript 3.50).

Support for A3 paper
"""""""""""""""""""""""""""

Yes, this driver supports the A3-size printer: merely set the required ``pagesize`` and margins. A simple way to do this is to specify the command-line switch ``-sPAPERSIZE=a3`` or include the procedure call ``a3`` in the PostScript prolog section. To optimize the printable area or set the proper margins, see the next paragraph.

Margins, PageSize
"""""""""""""""""""""

I refuse to add code to :title:`stcolor` that tries to guess the proper margins or page size, because I found that such guessing is usually wrong and needs correction in either the source or the parameters. You can modify ``stcolor.ps`` to do that, however. After the line:

.. code-block:: postscript

   mark % prepare stack for "setpagedevice"

insert these lines, which define page size and margins in points:

.. code-block:: postscript

   /.HWMargins [9.0 39.96 12.6 9.0]     % Left, bottom, right, top (1/72")
   /PageSize   [597.6 842.4]            % Paper, including margins (1/72")
   /Margins [ % neg. Offset to Left/Top in Pixels
      4 index 0 get STCold /HWResolution get 0 get mul 72 div neg
      5 index 3 get STCold /HWResolution get 1 get mul 72 div neg
   ]


Feel free to change the values of ``.HWMargins`` and ``PageSize`` to match your needs; the values given are the defaults when the driver is compiled with "``-DA4``". This option or its omission may cause trouble: the Stylus Color can print up to exactly 8 inches (2880 pixels) at 360dpi. The remaining paper is the margin, where the left margin varies only slightly with the paper size, while the right margin is significantly increased for wider paper, such as U.S. letter size.


.. note ::

   If you are using an ISO paper size with a version of stcolor after 1.20 and compiled without "``-DA4``", then the default margin is too large, and you need to add the proper "``.HWMargins``" to the command line or to ``stcolor.ps``.


Stylus Color II / IIs and 1500
""""""""""""""""""""""""""""""""""""""""""

First the good news: the driver can print on the Stylus Color II. Now the bad news:

- According to Epson support the driver "abuses" the color capabilities. (See "Future Plans" for details).
- You need some parameters on the command line (or in ``stcolor.ps``).
- I doubted that it would be usable with the Stylus Color IIs, but it is usable and suffers from mixing problems!

To make things work, you MUST disable the driver's internal weaving (Softweave), in one of these two ways:


.. code-block:: bash

   gs -dMicroweave ...
   gs -dnoWeave -descp_Band=1 ...


Version 1.90, current as of Ghostscript 5.10, fixes this bug by new default behaviour. I experienced significantly increased printing speed with the second variant on the old Stylus Color, when printing mostly monochrome data.


Recommendations
~~~~~~~~~~~~~~~~~~~~~

The next section is a contribution from `Jason Patterson`_ who evaluated a previous version (1.17). Ghostscript was invoked as follows:

.. code-block:: bash

   gs -sDEVICE=stcolor -r720x720 -sDithering=... -sOutputFile=escp.out stcolor.ps whatsoever.ps


where "``...``" is the name of the desired algorithm. ``stcolor.ps`` was omitted for the gs-algorithms (``gsmono``, ``gsrgb`` and ``gscmyk``), for which it is useless and would not allow the selection of "``gscmyk``".

Color dithering experiments with gdevstc 1.21
""""""""""""""""""""""""""""""""""""""""""""""""""""""""

Here are data about the EPSON Stylus Color driver's different dithering methods, based on a little experiment using four good quality scanned images of quite varied nature, to begin with, a summary of the results of the four experiments. Sanity note: the results here are from only four images and a total of 24 printouts (eight on 720dpi paper, sixteen on plain paper). Your results will almost certainly vary, and your standards might not be the same as mine, so use these results only as a guide, not as a formal evaluation.


**Quality of output by method**


.. list-table::
      :widths: 15 85
      :header-rows: 0

      * - ``gsmono``
        - Pretty much what you'd expect from a mono ordered pattern. Looks like what a lot of mono laser printers produce.
      * - ``fsmono``
        - Excellent for monochrome.
      * - ``gscmyk``
        - Not very good, but expected from an ordered pattern.
      * - ``gsrgb``
        - A little better than gscmyk. More consistent looking.
      * - ``fs2``
        - Good, but not quite as good as fsrgb. Gets the brightness wrong: too light at 720dpi, too dark at 360dpi.
      * - ``fsrgb``
        - Very good, but a little too dark and has a slight blue tint.
      * - ``hscmyk``
        - Excellent. Slightly better than fsrgb and fs2. Better than fscmyk on some images, almost the same on most.
      * - ``fscmyk``
        - Best. Very, very slightly better than hscmyk. On some images nearly as good as the EPSON demos done with the MS Windows driver.



**Overall visual quality (1-10), best to worst**


.. list-table::
      :widths: 15 85
      :header-rows: 1

      * -
        - 0 1 2 3 4 5 6 7 8 9 10
      * - Monochrome
        -
      * - ``fsmono``
        - ``******************``
      * - ``gsmono``
        - ``**********``
      * - Colour
        -
      * - ``fscmyk``
        - ``*******************``
      * - ``hscmyk``
        - ``*******************``
      * - ``fsrgb``
        - ``******************``
      * - ``fs2``
        - ``*****************``
      * - ``gsrgb``
        - ``**********``
      * - ``gscmyk``
        - ``*********``



Color transformation
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

In the initial version of the driver distributed with Ghostscript 3.33, the parameter ``SpotSize`` was the only way to manipulate the colors at the driver level. According to the parameters enumerated above, this has changed significantly with version 1.16 and above as a result an ongoing discussion about dithering algorithms and "false color" on the Epson Stylus Color. This initiated the transformation of the :title:`stcolor` driver into a framework for different dithering algorithms, providing a generalized interface to the internal Ghostscript color models and the other data structures related to Ghostscript drivers.

The main thing such a framework should be able to do is to deliver the values the dithering algorithm needs; and since this directly influences the optical image impression, this transformation should be adjustable without the need for recompilation and relinking.


Due to the limitations on raster storage, information is lost in the first transformation step, except for the 16-bit monochrome mode. So any color adjustment should take place before this step and this is where the optional ``ColorAdjustMatrix`` works.

The first transformation step, called "coding", is controlled by the ``?coding`` arrays. The decoding process expands the range of values expontentially to a larger range than that provided by the initial Ghostscript color model, and is therefore a reasonable place to make device- or algorithm-specific adjustments. This is where the ``?transfer`` arrays are used. Array access might be not the fastest method, but its generality is superior, so this step is always based upon internally algorithm-specific array access. If 8 bits are stored per color component and if the algorithm uses bytes too, the second transformation is included within the first, which saves significant computation time when printing the data.


ColorAdjustMatrix
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The driver supports different values for ``ProcessColorModel``, which raises the need for different color adjustments. Here "CAM" stands for "ColorAdjustMatrix".

.. code-block::

   DeviceGray (three floats)
   if ((r == g) && (g == b))
      K' = 1.0 - R;
   else
      K' = 1.0 - CAM[0] * R + CAM[1] * G + CAM[2] * B;


According to the documentation on drivers, the latter (the "else" clause) should never happen.


.. code-block::

   DeviceRGB (nine floats)
   if((r == g) && (g == b))
      R' = B' = G' = R;
   else
      R' = CAM[0]*R + CAM[1]*G + CAM[2]*B;
      G' = CAM[3]*R + CAM[4]*G + CAM[5]*B;
      B' = CAM[6]*R + CAM[7]*G + CAM[8]*B;

The printer always uses four inks, so a special treatment of black is provided. Algorithms may take special action if R, G, and B are all equal.


.. code-block::

   DeviceCMYK (sixteen floats)
   if((c == m) && (m == y))
      K' = max(C,K);
      C' = M' = Y' = 0;
   else
      K  = min(C,M,Y);
      if((K > 0) && ColorAdjustMatrix_present) { => UCR
         C -= K;
         M -= K;
         Y -= K;
      }

      C' = CAM[ 0]*C + CAM[ 1]*M + CAM[ 2]*Y + CAM[ 3]*K;
      M' = CAM[ 4]*C + CAM[ 5]*M + CAM[ 6]*Y + CAM[ 7]*K;
      Y' = CAM[ 8]*C + CAM[ 9]*M + CAM[10]*Y + CAM[11]*K;
      K' = CAM[12]*C + CAM[13]*M + CAM[14]*Y + CAM[15]*K;


Again we have a special black treatment. "``max(C,K)``" was introduced because of a slight misbehaviour of Ghostscript, which delivers black under certain circumstances as (1,1,1,0). Normally, when no special black separation and undercolor removal procedures are defined at the PostScript level, either (C,M,Y,0) or (0,0,0,K) values are mapped. This would make the extended ``ColorAdjustMatrix`` quite tedious, and so during mapping, black separation is done for (C,M,Y,0) requests; and if there is a ``ColorAdjustMatrix``, undercolor removal is used too. In other words the default matrix is:


.. list-table::
      :header-rows: 0

      * - 1
        - 0
        - 0
        - 1
      * - 0
        - 1
        - 0
        - 1
      * - 0
        - 0
        - 1
        - 1
      * - 0
        - 0
        - 0
        - 1

and it is applied to CMYK values with separated and removed black. Raising the CMY coefficients while lowering the K coefficients reduces black and intensifies color. But be careful, because even small deviations from the default cause drastic changes.


If no ``ColorAdjustMatrix`` is set, the matrix computations are skipped. Thus the transformation reduces to range inversion in monochrome mode and black separation in CMYK mode.



RGB / CMYK coding and transfer, and ``BitsPerPixel``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

These two (groups of) parameters are arrays of floating-point numbers in the range 0.0 to 1.0. They control the truncation to the desired number of bits stored in raster memory (``BitsPerPixel``) and the ink density. The "truncation" may become a nonlinear function if any of the ``?coding`` arrays is set. Assume the following Ghostscript invocation:


.. code-block:: bash

   gs -sDEVICE=stcolor -sDithering=fscmyk -dBitsPerPixel=16 \
        -dKcoding='{ 0.0 0.09 0.9 1.0 }' \
        -dMcoding='{ 0.0 0.09 0.9 1.0 }' \
      -dKtransfer='{ 0.0 0.09 0.9 1.0 }' \
      -dYtransfer='{ 0.0 0.09 0.9 1.0 }'


We may have either or both of ``?coding`` and ``?transfer``, giving four possible combinations. (These four combinations appear in the given example.) The resulting mapping appears in the following tables, where except for the internal Indices (4 components × 4 bits = 16 ``BitsPerPixel``), all values are normalized to the range 0 to 1. The actual range is 0 to 65535 for the Ghostscript color and 0 to 16777215 for the ink values delivered to the ``fscmyk`` algorithm. Sorry for the bunch of numbers following, but you may try this example in conjunction with ``stcinfo.ps``, which should give you a graphical printout of the following numbers when you issue a ``showpage`` command.



.. raw:: html

   <table width=100% border=1>
   <tr>
       <th></th>
       <th colspan="3">Cyan</th>
       <th colspan="3">Magenta</th>
   </tr>
   <tr>
       <th>CI/15</th>
       <th>gs_color_values</th>
       <th>CI</th>
       <th>Ink</th>
       <th>gs_color_values</th>
       <th>CI</th>
       <th>Ink</th>
   </tr>
   <tr>
       <td align="center">0.000</td>
       <td align="center">0.000 - 0.062</td>
       <td align="center">0</td>
       <td align="center">0.000</td>
       <td align="center">-0.123 - 0.123</td>
       <td align="center">0</td>
       <td align="center">0.000</td>
   </tr>
   <tr>
       <td align="center">0.067</td>
       <td align="center">0.063 - 0.125</td>
       <td align="center">1</td>
       <td align="center">0.067</td>
       <td align="center">0.123 - 0.299</td>
       <td align="center">1</td>
       <td align="center">0.247</td>
   </tr>
   <tr>
       <td align="center">0.133</td>
       <td align="center">0.125 - 0.187</td>
       <td align="center">2</td>
       <td align="center">0.133</td>
       <td align="center">0.299 - 0.365</td>
       <td align="center">2</td>
       <td align="center">0.351</td>
   </tr>
   <tr>
       <td align="center">0.200</td>
       <td align="center">0.188 - 0.250</td>
       <td align="center">3</td>
       <td align="center">0.200</td>
       <td align="center">0.365 - 0.392</td>
       <td align="center">3</td>
       <td align="center">0.379</td>
   </tr>
   <tr>
       <td align="center">0.267</td>
       <td align="center">0.250 - 0.312</td>
       <td align="center">4</td>
       <td align="center">0.267</td>
       <td align="center">0.392 - 0.420</td>
       <td align="center">4</td>
       <td align="center">0.406</td>
   </tr>
   <tr>
       <td align="center">0.333</td>
       <td align="center">0.313 - 0.375</td>
       <td align="center">5</td>
       <td align="center">0.333</td>
       <td align="center">0.420 - 0.447</td>
       <td align="center">5</td>
       <td align="center">0.433</td>
   </tr>
   <tr>
       <td align="center">0.400</td>
       <td align="center">0.375 - 0.437</td>
       <td align="center">6</td>
       <td align="center">0.400</td>
       <td align="center">0.447 - 0.475</td>
       <td align="center">6</td>
       <td align="center">0.461</td>
   </tr>
   <tr>
       <td align="center">0.467</td>
       <td align="center">0.438 - 0.500</td>
       <td align="center">7</td>
       <td align="center">0.467</td>
       <td align="center">0.475 - 0.502</td>
       <td align="center">7</td>
       <td align="center">0.488</td>
   </tr>
   <tr>
       <td align="center">0.533</td>
       <td align="center">0.500 - 0.562</td>
       <td align="center">8</td>
       <td align="center">0.533</td>
       <td align="center">0.502 - 0.529</td>
       <td align="center">8</td>
       <td align="center">0.516</td>
   </tr>
   <tr>
       <td align="center">0.600</td>
       <td align="center">0.563 - 0.625</td>
       <td align="center">9</td>
       <td align="center">0.600</td>
       <td align="center">0.529 - 0.557</td>
       <td align="center">9</td>
       <td align="center">0.543</td>
   </tr>
   <tr>
       <td align="center">0.667</td>
       <td align="center">0.625 - 0.687</td>
       <td align="center">10</td>
       <td align="center">0.667</td>
       <td align="center">0.557 - 0.584</td>
       <td align="center">10</td>
       <td align="center">0.571</td>
   </tr>
   <tr>
       <td align="center">0.733</td>
       <td align="center">0.688 - 0.750</td>
       <td align="center">11</td>
       <td align="center">0.733</td>
       <td align="center">0.584 - 0.612</td>
       <td align="center">11</td>
       <td align="center">0.598</td>
   </tr>
   <tr>
       <td align="center">0.800</td>
       <td align="center">0.750 - 0.812</td>
       <td align="center">12</td>
       <td align="center">0.800</td>
       <td align="center">0.612 - 0.639</td>
       <td align="center">12</td>
       <td align="center">0.626</td>
   </tr>
   <tr>
       <td align="center">0.867</td>
       <td align="center">0.813 - 0.875</td>
       <td align="center">13</td>
       <td align="center">0.867</td>
       <td align="center">0.639 - 0.715</td>
       <td align="center">13</td>
       <td align="center">0.653</td>
   </tr>
   <tr>
       <td align="center">0.933</td>
       <td align="center">0.875 - 0.937</td>
       <td align="center">14</td>
       <td align="center">0.933</td>
       <td align="center">0.715 - 0.889</td>
       <td align="center">14</td>
       <td align="center">0.778</td>
   </tr>
   <tr>
       <td align="center">1.000</td>
       <td align="center">0.938 - 1.000</td>
       <td align="center">15</td>
       <td align="center">1.000</td>
       <td align="center">0.889 - 1.111</td>
       <td align="center">15</td>
       <td align="center">1.000</td>
   </tr>
   </table>

|

The difference between cyan and magenta is the presence of a coding array. The coding process must map a range of color values to each of the sixteen component indices. If no coding array is given, this is accomplished by dividing by 4096, equivalent to a right shift by 12 bits. The final ink density resides in the given interval and moves from the left to the right side from 0 to 15. For magenta there is a coding array and the ink value matches the center of the intervals. But the distribution of the mapped intervals follows the given coding array and is nonlinear in the linear color space of Ghostscript.

Now let us take a look at the case with transfer arrays:


.. raw:: html

   <table width=100% border=1>
   <tr>
       <th></th>
       <th colspan="3">Yellow</th>
       <th colspan="3">Black</th>
   </tr>
   <tr>
       <th>CI/15</th>
       <th>gs_color_values</th>
       <th>CI</th>
       <th>Ink</th>
       <th>gs_color_values</th>
       <th>CI</th>
       <th>Ink</th>
   </tr>
   <tr>
       <td align="center">0.000</td>
       <td align="center">0.000 - 0.062</td>
       <td align="center">0</td>
       <td align="center">0.000</td>
       <td align="center">-0.123 - 0.123</td>
       <td align="center">0</td>
       <td align="center">0.000</td>
   </tr>
   <tr>
       <td align="center">0.067</td>
       <td align="center">0.063 - 0.125</td>
       <td align="center">1</td>
       <td align="center">0.018</td>
       <td align="center">0.123 - 0.299</td>
       <td align="center">1</td>
       <td align="center">0.067</td>
   </tr>
   <tr>
       <td align="center">0.13</td>
       <td align="center">0.125 - 0.187</td>
       <td align="center">2</td>
       <td align="center">0.036</td>
       <td align="center">0.299 - 0.365</td>
       <td align="center">2</td>
       <td align="center">0.133</td>
   </tr>
   <tr>
       <td align="center">0.200</td>
       <td align="center">0.188 - 0.250</td>
       <td align="center">3</td>
       <td align="center">0.054</td>
       <td align="center">0.365 - 0.392</td>
       <td align="center">3</td>
       <td align="center">0.200</td>
   </tr>
   <tr>
       <td align="center">0.267</td>
       <td align="center">0.250 - 0.312</td>
       <td align="center">4</td>
       <td align="center">0.072</td>
       <td align="center">0.392 - 0.420</td>
       <td align="center">4</td>
       <td align="center">0.267</td>
   </tr>
   <tr>
       <td align="center">0.333</td>
       <td align="center">0.313 - 0.375</td>
       <td align="center">5</td>
       <td align="center">0.090</td>
       <td align="center">0.420 - 0.447</td>
       <td align="center">5</td>
       <td align="center">0.333</td>
   </tr>
   <tr>
       <td align="center">0.400</td>
       <td align="center">0.375 - 0.437</td>
       <td align="center">6</td>
       <td align="center">0.252</td>
       <td align="center">0.447 - 0.475</td>
       <td align="center">6</td>
       <td align="center">0.400</td>
   </tr>
   <tr>
       <td align="center">0.467</td>
       <td align="center">0.438 - 0.500</td>
       <td align="center">7</td>
       <td align="center">0.414</td>
       <td align="center">0.475 - 0.502</td>
       <td align="center">7</td>
       <td align="center">0.467</td>
   </tr>
   <tr>
       <td align="center">0.533</td>
       <td align="center">0.500 - 0.562</td>
       <td align="center">8</td>
       <td align="center">0.576</td>
       <td align="center">0.502 - 0.529</td>
       <td align="center">8</td>
       <td align="center">0.533</td>
   </tr>
   <tr>
       <td align="center">0.600</td>
       <td align="center">0.563 - 0.625</td>
       <td align="center">9</td>
       <td align="center">0.738</td>
       <td align="center">0.529 - 0.557</td>
       <td align="center">9</td>
       <td align="center">0.600</td>
   </tr>
   <tr>
       <td align="center">0.667</td>
       <td align="center">0.625 - 0.687</td>
       <td align="center">10</td>
       <td align="center">0.900</td>
       <td align="center">0.557 - 0.584</td>
       <td align="center">10</td>
       <td align="center">0.667</td>
   </tr>
   <tr>
       <td align="center">0.733</td>
       <td align="center">0.688 - 0.750</td>
       <td align="center">11</td>
       <td align="center">0.920</td>
       <td align="center">0.584 - 0.612</td>
       <td align="center">11</td>
       <td align="center">0.733</td>
   </tr>
   <tr>
       <td align="center">0.800</td>
       <td align="center">0.750 - 0.812</td>
       <td align="center">12</td>
       <td align="center">0.940</td>
       <td align="center">0.612 - 0.639</td>
       <td align="center">12</td>
       <td align="center">0.800</td>
   </tr>
   <tr>
       <td align="center">0.867</td>
       <td align="center">0.813 - 0.875</td>
       <td align="center">13</td>
       <td align="center">0.960</td>
       <td align="center">0.639 - 0.715</td>
       <td align="center">13</td>
       <td align="center">0.867</td>
   </tr>
   <tr>
       <td align="center">0.933</td>
       <td align="center">0.875 - 0.937</td>
       <td align="center">14</td>
       <td align="center">0.980</td>
       <td align="center">0.715 - 0.889</td>
       <td align="center">14</td>
       <td align="center">0.933</td>
   </tr>
   <tr>
       <td align="center">1.000</td>
       <td align="center">0.938 - 1.000</td>
       <td align="center">15</td>
       <td align="center">1.000</td>
       <td align="center">0.889 - 1.111</td>
       <td align="center">15</td>
       <td align="center">1.000</td>
   </tr>
   </table>

|

Yellow uses a transfer array. There is no linear correspondence between the color and the ink values: this correspondence is defined through the given array. In other words, the transfer arrays define a nonlinear ink characteristic, which is exactly the same functionality that PostScript's "(color)transfer" function provides.

While for yellow the intervals match the intervals used with cyan, for black the intervals match the magenta intervals. But watch the correspondence between the CI/15 values and the ink density for black: this is a linear distribution in the ink domain.

Not a bad idea, I think. Consider the ``fs2`` algorithm: it uses values in the range 0 to 255. If any transfer array were alone, some of the 256 possible values would never be used and others would be used for adjacent intervals several times. Establishing an identical coding array solves this problem, so the full potential of the algorithm is used.

Another useful feature of the coding arrays is that they are internally normalized to the range 0-1. In 720x720dpi mode the transfer arrays in ``stcolor.ps`` limit the dot density to about 50%, so these arrays end at 0.5 (and begin at 0.5 for RGB). Because of automatic normalization, these arrays can also be used as coding arrays. But of course in the ``fs2`` case mentioned above, values from 0 to 127 will never be delivered to the algorithm, while values 128-255 are delivered for adjacent intervals.

To clarify the intended use of the three parameters (parameter groups), keep this in mind:


- ``ColorAdjustMatrix`` is never used when transferring gray values. This restricts it to what the name says: adjustment of colors, that is, correction for miscolored ink. Do not use it for saturation or brightness control.

- ``?transfer`` arrays control the values delivered to the driver, which in turn controls the ink quantity. Use these arrays to control saturation and brightness. In general these arrays are identical for all inks. If they differ they provide a simpler scheme for color correction, which is not necessarily faster than the ``ColorAdjustMatrix``.

- ``?coding`` arrays control the color value intervals mapped to the internal color indices.


What is weaving?
~~~~~~~~~~~~~~~~~~~~~

The Epson Stylus Color has a head assembly that contains two physically identifiable heads, one for black and one for cyan, magenta, and yellow (CMY). This makes four "logical" heads, one for each color component. Each of these four heads has several jets at some vertical (Y) distance from one another, so several horizontal lines can be printed of a given color during one pass of the heads. From experience I think there are fifteen jets per color, spaced at 1/90in.

So the question arises of how to print at a Y resolution of 360dpi with 90dpi jets. Simply by division one gets 360dpi/90dpi = 4, which tells us that 4 passes of the head assembly are needed to achieve a Y resolution of 360dpi.

Weaving is the method of how the fifteen jets are used to print adjacent horizontal rows separated here by 1/360 inch:


.. raw:: html

   <table width=100% border=1>
   <tr>
       <th colspan="10">Print-head jets used with and without weaving</th>
   </tr>
   <tr>
       <th></th>
       <th></th>
       <th colspan="4">Weaving</th>
       <th colspan="4">noWeave</th>
   </tr>
   <tr>
       <td></td>
       <td>Pass</td>
       <td>1</td>
       <td>2</td>
       <td>3</td>
       <td>4</td>
       <td>1</td>
       <td>2</td>
       <td>3</td>
       <td>4</td>
   </tr>
   <tr>
       <td colspan="10">Row</td>
   </tr>
   <tr>
       <td align="center">0</td>
       <td></td>
       <td align="center">jet 0</td>
       <td align="center">--</td>
       <td align="center">--</td>
       <td align="center">--</td>
       <td align="center">jet 0</td>
       <td align="center">--</td>
       <td align="center">--</td>
       <td align="center">--</td>
   </tr>
   <tr>
       <td align="center">1</td>
       <td></td>
       <td align="center">--</td>
       <td align="center">jet 1</td>
       <td align="center">--</td>
       <td align="center">--</td>
       <td align="center">--</td>
       <td align="center">jet 0</td>
       <td align="center">--</td>
       <td align="center">--</td>
   </tr>
   <tr>
       <td align="center">2</td>
       <td></td>
       <td align="center">--</td>
       <td align="center">--</td>
       <td align="center">jet 2</td>
       <td align="center">--</td>
       <td align="center">--</td>
       <td align="center">--</td>
       <td align="center">jet 0</td>
       <td align="center">--</td>
   </tr>
   <tr>
       <td align="center">3</td>
       <td></td>
       <td align="center">--</td>
       <td align="center">--</td>
       <td align="center">--</td>
       <td align="center">jet 3</td>
       <td align="center">--</td>
       <td align="center">--</td>
       <td align="center">--</td>
       <td align="center">jet 0</td>
   </tr>
   <tr>
       <td align="center">4</td>
       <td></td>
       <td align="center">jet 1</td>
       <td align="center">--</td>
       <td align="center">--</td>
       <td align="center">--</td>
       <td align="center">jet 1</td>
       <td align="center">--</td>
       <td align="center">--</td>
       <td align="center">--</td>
   </tr>
   <tr>
       <td align="center">5</td>
       <td></td>
       <td align="center">--</td>
       <td align="center">jet 2</td>
       <td align="center">--</td>
       <td align="center">--</td>
       <td align="center">--</td>
       <td align="center">jet 1</td>
       <td align="center">--</td>
       <td align="center">--</td>
   </tr>
   <tr>
       <td align="center">6</td>
       <td></td>
       <td align="center">--</td>
       <td align="center">--</td>
       <td align="center">jet 3</td>
       <td align="center">--</td>
       <td align="center">--</td>
       <td align="center">--</td>
       <td align="center">jet 1</td>
       <td align="center">--</td>
   </tr>
   <tr>
       <td colspan="10">...</td>
   </tr>
   </table>


Now let's assume that the dot diameter is different for each individual jet, but the average among the jets matches the desired resolution. With weaving, adjacent rows are printed by different jets, thus some averaging takes place. Without weaving, adjacent rows are printed by the same jet and this makes the dot diameter deviations visible as 1/90in stripes on the paper.


Print mode parameters
~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The parameters "``Unidirectional``", "``Microweave``", "``noWeave``", "``OutputCode``", "``Model``" and the given resolution control the data generated for the printer.

Unidirectional
"""""""""""""""""""

Simply toggles the unidirectional mode of the printer. Setting "``Unidirectional``" definitely slows printing speed, but may improve the quality. I use this for printing transparencies, where fast head movement could smear the ink.

Microweave, noWeave and OutputCode=deltarow
"""""""""""""""""""""""""""""""""""""""""""""""""""""""""

The first are two booleans, which implies that four combinations are possible. Actually only three exist (if you don't count for ``deltarow``): ``Softweave``, ``Microweave``, and ``noWeave``. The first and second are functionally identical, the difference being whether the driver or the printer does the job.

In the default ``Softweave`` mode the driver sends the data properly arranged to the printer, while in ``Microweave`` mode, it is the printer that arranges the data. But in general the host processor is much faster than the printer's processor, and thus it is faster for the host do the job. In addition to that, for 720dpi eight passes are required, and the amount of buffer space needed to buffer the data for the passes is far beyond the printer's memory. ``Softweave`` requires an odd value of "``escp_Band``"; the Stylus Color provides fifteen for that.

"OutputCode" controls the encoding used. In the basic modes, the choice consists of "plain" and "runlength". The computation of runlength-encoded data does not take much time, less than the data tranfer to the printer; thus this is the recommended mode, and of course the default. With the Stylus Color, Epson introduced some new encoding principles, namely "tiff" and "deltarow". While the first was omitted from this driver for lack of apparent advantages, ``deltarow`` is available as an option. ``Softweave`` cannot be used with this encoding, so if ``OutputCode=deltarow`` is set, ``Microweave`` becomes the default. Maybe that the size of the ESC/P2 code becomes smaller, but I have never observed faster printing speed. Things tend to become slower with ``deltarow`` compared to ``Softweave``.


Model
"""""""""""""""""""

Some ESC/P2 printers such as the Stylus 800 do not offer ``Microweave`` or the commands required to do ``Softweave``. Setting ``Model`` just changes the defaults and omits some parts of the initialization sequence which are not compatible with the given printer model. Currently only ":title:`st800`" is supported besides the default :title:`stcolor`.


Bugs and pitfalls
~~~~~~~~~~~~~~~~~~~~~


- The given ``?coding`` and ``?transfer`` arrays should be strictly monotonic.

- It is impossible to change ``WHITE``: that's your paper. Thus RGB transfer should end at 1.0 and CMYK transfer should start at 0.0.

- Usually 8 bits per component yields fastest operation.

- The ``ColorAdjustMatrix`` is not used in the reverse transformation used when Ghostscript does the dithering (``gs*`` modes). Expect funny results.

- If ``BitsPerPixel`` is less than 6, the entire coding and transfer process does not work. This is always true for the ``gs*`` modes and becomes true for the other modes if ``BitsPerPixel`` is forced to low values.

- 720×720dpi printing should never select the ``gs*`` modes and should always use ``stcolor.ps``. (I prefer 360×720.)


Tests
~~~~~~~~~~~~~~~~~~~~~

This section gives an overview of performance in terms of processing and printing times, from tests run after version 1.13. Printing was done offline (simply copying a processed file to the printer) to measure real printing speed without regard to speed of processing on the host, since at high resolutions, processing time is the same order of magnitude and thus may become the limiting factor.

**The various OutputCodes**

I ran several files though Ghostscript and recorded the size of the resulting print code, the processing time, and the printing time, at least for some of the files, always using these options:


.. code-block:: bash

   gs -sDEVICE=stcolor -sPAPERSIZE=a4 stcolor.ps - < file.ps

(Actually "``-sPAPERSIZE=a4``" is in my ``gs_init.ps`` since I'm a germ.)


"``deltarow``" is the new encoding principle ("``ESC . 3 10 10 1``") with ``Microweave`` on. It is activated with "``-sOutputCode=deltarow``".

"``Softweave``" actually means that nothing else was used: it is the default, and implies that odd v=40/h=10/m=15 mode ("``ESC . 1 40 10 15``").

"``Microweave``" means "``-dMicroweave``", equivalent to "``ESC . 1 10 10 1``", with full skip optimization and microweave activated.


Finally I wanted to see the plain `Kathy Ireland`_, and used "``-sOutputCode=plain``", which just replaces runlength encoding (RLE) by no encoding, thus using "``ESC . 0 40 10 15``". [So sorry ;-) Kathy was still dressed in blue in front of the blue sea on a blue air cushion – nice to see but hard to dither.]


So here are the results.


.. raw:: html

   <table width=100% border=1>
   <tr>
       <th colspan="5">File sizes and printing speeds with various weaving methods</th>
   </tr>
   <tr>
       <td>&nbsp;</td>
       <td><code>golfer.ps</code></td>
       <td><code>colorcir.ps</code></td>
       <td><code>drawing.ps</code></td>
       <td><code>brief.ps</code></td>
   </tr>
   <tr>
       <td><code>deltarow</code></td>
       <td>572751/48.180u</td>
       <td>643374/41.690u</td>
       <td>90142/46.180u/1:50</td>
       <td>178563/49.350u/2:22</td>
   </tr>
   <tr>
       <td><code>Softweave</code></td>
       <td>559593/46.810u</td>
       <td>669966/44.960u</td>
       <td>296168/48.160u/1:30</td>
       <td>269808/43.320u/1:55</td>
   </tr>
   <tr>
       <td><code>Microweave</code></td>
       <td>590999/56.060u</td>
       <td>754276/42.890u</td>
       <td>338885/47.060u/1:50</td>
       <td>282314/44.690u/2:22</td>
   </tr>
   </table>


|

.. _Kathy Ireland:

.. raw:: html

   <table width=100% border=1>
   <tr>
       <th colspan="2">Kathy Ireland</th>
   </tr>
   <tr>
       <td></td>
       <td><code>kathy.ps</code></td>
   </tr>
   <tr>
       <td><code>deltarow</code></td>
       <td>3975334/111.940u/5:35</td>
   </tr>
   <tr>
       <td><code>Softweave</code></td>
       <td>3897112/101.940u/3:10</td>
   </tr>
   <tr>
       <td><code>Microweave</code></td>
       <td>4062829/100.990u/3:15</td>
   </tr>
   <tr>
       <td><code>plain/soft</code></td>
       <td>5072255/104.390u/3:05</td>
   </tr>
   </table>


|


It may be that I've not chosen the optimal ``deltarow`` code, but even if it saves at lot of bytes, printing-speed is not increased.

At least the printer prefers plain Kathy. In other words, sending 1 Megabyte or 20% more data has no impact on printing speed. ``drawing.ps`` is an exception to this rule: plain prints slower than RLE.

"Unclever" coding -- especially with ``deltarow`` -- can significantly slow down printing. But even if very significant advantages in the size of the code are achieved, ``deltarow`` is not competitive. ``colorcir.ps`` shows savings with ``deltarow``, but printing is a mess.


.. raw:: html

   <table width=100% border=1>
   <tr>
       <th colspan="6">Printing time related to other options <small>(*Full page halftone images printed, unless otherwise noted.)</small></th>
   </tr>
   <tr>
       <th align="right">dpi</th>
       <th align="right">Print mode</th>
       <th align="right">Size KB</th>
       <th align="right">Time</th>
       <th colspan="2" align="left">Comments</th>
   </tr>
   <tr>
       <td align="right">180x180 mono</td>
       <td align="right">-/uni</td>
       <td align="right">358</td>
       <td align="right">1:15</td>
       <td colspan="2">&nbsp;</td>
   </tr>
   <tr>
       <td>&nbsp;</td>
       <td align="right">-/bi</td>
       <td align="right">358</td>
       <td align="right">0:45</td>
       <td colspan="2">&nbsp;</td>
   </tr>
   <tr>
       <td>&nbsp;</td>
       <td align="right">micro/bi</td>
       <td align="right">205</td>
       <td align="right">0:45</td>
       <td colspan="2">Not Weaving</td>
   </tr>
   <tr>
       <td>&nbsp;</td>
       <td align="right">soft/bi</td>
       <td align="right">179</td>
       <td align="right">1:25</td>
       <td colspan="2">&nbsp;</td>
   </tr>
   <tr>
       <td align="right">color</td>
       <td align="right">-/bi</td>
       <td align="right">641</td>
       <td align="right">2:45</td>
       <td colspan="2">&nbsp;</td>
   </tr>
   <tr>
       <td>&nbsp;</td>
       <td align="right">soft/bi</td>
       <td align="right">556</td>
       <td align="right">1:32</td>
       <td colspan="2">&nbsp;</td>
   </tr>
   <tr>
       <td align="right">360x360 mono</td>
       <td align="right">-/uni</td>
       <td align="right">269</td>
       <td align="right">0:50</td>
       <td colspan="2">Monochrome text</td>
   </tr>
   <tr>
       <td>&nbsp;</td>
       <td align="right">-/bi</td>
       <td align="right">269</td>
       <td align="right">0:35</td>
       <td colspan="2">Monochrome text</td>
   </tr>
   <tr>
       <td>&nbsp;</td>
       <td align="right">micro/bi</td>
       <td align="right">269</td>
       <td align="right">2:25</td>
       <td colspan="2">Monochrome text</td>
   </tr>
   <tr>
       <td>&nbsp;</td>
       <td align="right">soft/uni</td>
       <td align="right">250</td>
       <td align="right">3:15</td>
       <td colspan="2">Monochrome text</td>
   </tr>
   <tr>
       <td>&nbsp;</td>
       <td align="right">soft/bi</td>
       <td align="right">250</td>
       <td align="right">1:55</td>
       <td colspan="2">Monochrome text</td>
   </tr>
   <tr>
       <td align="right">color</td>
       <td align="right">-/bi</td>
       <td align="right">346</td>
       <td align="right">1:00</td>
       <td colspan="2">Sparse-color page, visible displacements</td>
   </tr>
   <tr>
       <td>&nbsp;</td>
       <td align="right">micro/bi</td>
       <td align="right">346</td>
       <td align="right">1:50</td>
       <td colspan="2">Sparse-color page, looks buggy – printer?</td>
   </tr>
   <tr>
       <td>&nbsp;</td>
       <td align="right">soft/bi</td>
       <td align="right">294</td>
       <td align="right">1:30</td>
       <td colspan="2">Sparse-color page, O.K.</td>
   </tr>
   <tr>
       <td>&nbsp;</td>
       <td align="right">-/bi</td>
       <td align="right">2218</td>
       <td align="right">2:45</td>
       <td colspan="2">Visible stripes</td>
   </tr>
   <tr>
       <td>&nbsp;</td>
       <td align="right">micro/bi</td>
       <td align="right">5171</td>
       <td align="right">3:17</td>
       <td colspan="2">&nbsp;</td>
   </tr>
   <tr>
       <td>&nbsp;</td>
       <td align="right">soft/bi</td>
       <td align="right">3675</td>
       <td align="right">3:05</td>
       <td colspan="2">&nbsp;</td>
   </tr>
   <tr>
       <td align="right">360x720 mono</td>
       <td align="right">soft/bi</td>
       <td align="right">2761</td>
       <td align="right">5:40</td>
       <td colspan="2">&nbsp;</td>
   </tr>
   <tr>
       <td align="right">color</td>
       <td align="right">soft/bi</td>
       <td align="right">7789</td>
       <td align="right">6:15</td>
       <td colspan="2">Just a small difference!</td>
   </tr>
   <tr>
       <td align="right">720x360 color</td>
       <td align="right">soft/bi</td>
       <td align="right">7182</td>
       <td align="right">5:40</td>
       <td colspan="2">&nbsp;</td>
   </tr>
   <tr>
       <td align="right">720x720 color</td>
       <td align="right">micro/bi</td>
       <td align="right">14748</td>
       <td align="right">30:26</td>
       <td colspan="2">Actually beyond printer's capabilities</td>
   </tr>
   <tr>
       <td>&nbsp;</td>
       <td align="right">soft/bi</td>
       <td align="right">14407</td>
       <td align="right">11:08</td>
       <td colspan="2">&nbsp;</td>
   </tr>
   </table>



Acknowledgments
~~~~~~~~~~~~~~~~~~~~~~~~

This driver was copied from ``gdevcdj.c`` (Ghostscript 3.12), which was contributed by George Cameron, Koert Zeilstra, and Eckhard Rueggeberg. Some of the ESC/P2 code was drawn from Richard Brown's ``gdevescp.c``. The POSIX interrupt code (compilation option -DSTC_SIGNAL) is from Frederic Loyer. Several improvements are based on discussions with Brian Converse, Bill Davidson, Gero Guenther, Jason Patterson, ? Rueschstroer, and Steven Singer.

While I wish to thank everyone mentioned above, they are by no means responsible for bugs in the :title:`stcolor` driver -- just for the features.


.. _uniprint:


uniprint, a flexible unified printer driver
----------------------------------------------------

:title:`uniprint` is a unified parametric driver by `Gunther Hess`_ for several kinds of printers and devices, including:

- Any Epson Stylus Color, Stylus, or Stylus Pro.
- HP PCL/RTL.
- Canon BubbleJet Color 610.
- NEC P2X.
- Sun raster file format.

This driver is intended to become a unified printer driver. If you consider it ugly, please send me your suggestions for improvements. The driver will be updated with them. Thus the full explanation of the driver's name is: Ugly- -> Updated- -> Unified Printer Driver


But you probably want to know something about the functionality. At the time of this writing :title:`uniprint` drives:

- NEC Pinwriter P2X (24-pin monochrome impact printer, ESC/P style).
- Several Epson Stylus Color models (ESC/P2 style).
- HP-DeskJet 550c (basic HP-RTL).
- Canon BJC 610.

It can be configured for various other printers without recompilation and offers uncompressed (ugly) Sun rasterfiles as another format, but this format is intended for testing purposes rather than real use. The usage of this driver is quite simple. The typical command line looks like this:


.. code-block:: bash

   gs @{MODEL}.upp -sOutputFile={printable file} MyFile.ps -c quit


For example, from my Linux box:

.. code-block:: bash

   gs @stc.upp -sOutputFile=/dev/lp1 tiger.eps -c quit



.. raw:: html

   <table width=100% border=1>
   <tr>
       <th colspan="3">Unified Printer Parameter files distributed with Ghostscript</th>
   </tr>
   <tr>
       <th align="left" colspan="3">Canon BJC 610 (color, rendered)</th>
   </tr>
   <tr>

       <td><code>bjc610a0.upp</code></td>

       <td>360&times;360dpi</td>

       <td>plain paper, high speed</td>
   </tr>
   <tr>

       <td><code>bjc610a1.upp</code></td>

       <td>360&times;360dpi</td>

       <td>plain paper</td>
   </tr>
   <tr>

       <td><code>bjc610a2.upp</code></td>

       <td>360&times;360dpi</td>

       <td>coated paper</td>
   </tr>
   <tr>

       <td><code>bjc610a3.upp</code></td>

       <td>360&times;360dpi</td>

       <td>transparency film</td>
   </tr>
   <tr>

       <td><code>bjc610a4.upp</code></td>

       <td>360&times;360dpi</td>

       <td>back print film</td>
   </tr>
   <tr>

       <td><code>bjc610a5.upp</code></td>

       <td>360&times;360dpi</td>

       <td>fabric sheet</td>
   </tr>
   <tr>

       <td><code>bjc610a6.upp</code></td>

       <td>360&times;360dpi</td>

       <td>glossy paper</td>
   </tr>
   <tr>

       <td><code>bjc610a7.upp</code></td>

       <td>360&times;360dpi</td>

       <td>high gloss film</td>
   </tr>
   <tr>

       <td><code>bjc610a8.upp</code></td>

       <td>360&times;360dpi</td>

       <td>high resolution paper</td>
   </tr>
   <tr>
       <th colspan="3"></th>
   </tr>
   <tr>

       <td><code>bjc610b1.upp</code></td>

       <td>720&times;720dpi</td>

       <td>plain paper</td>
   </tr>
   <tr>

       <td><code>bjc610b2.upp</code></td>

       <td>720&times;720dpi</td>

       <td>coated paper</td>
   </tr>
   <tr>

       <td><code>bjc610b3.upp</code></td>

       <td>720&times;720dpi</td>

       <td>transparency film</td>
   </tr>
   <tr>

       <td><code>bjc610b4.upp</code></td>

       <td>720&times;720dpi</td>

       <td>back print film</td>
   </tr>
   <tr>

       <td><code>bjc610b6.upp</code></td>

       <td>720&times;720dpi</td>

       <td>glossy paper</td>
   </tr>
   <tr>

       <td><code>bjc610b7.upp</code></td>

       <td>720&times;720dpi</td>

       <td>high-gloss paper</td>
   </tr>
   <tr>

       <td><code>bjc610b8.upp</code></td>

       <td>720&times;720dpi</td>

       <td>high resolution paper</td>
   </tr>
   <tr>
       <th align="left" colspan="3">HP Ink-Printers</th>
   </tr>
   <tr>

       <td><code>cdj550.upp</code></td>

       <td>300&times;300dpi</td>

       <td>32-bit CMYK</td>
   </tr>
   <tr>

       <td><code>cdj690.upp</code></td>

       <td>300&times;300dpi</td>

       <td>Normal mode</td>
   </tr>
   <tr>

       <td><code>cdj690ec.upp</code></td>

       <td>300&times;300dpi</td>

       <td>Economy mode</td>
   </tr>
   <tr>

       <td><code>dnj750c.upp</code></td>

       <td>300&times;300dpi</td>

       <td>Color – also good for 450C</td>
   </tr>
   <tr>

       <td><code>dnj750m.upp</code></td>

       <td>600&times;600dpi</td>

       <td>Monochrome</td>
   </tr>
   <tr>
       <th align="left" colspan="3">NEC P2X</th>
   </tr>
   <tr>

       <td><code>necp2x.upp</code></td>

       <td>360&times;360dpi</td>

       <td>8-bit (Floyd-Steinberg)</td>
   </tr>
   <tr>
       <th align="left" colspan="3">Any Epson Stylus Color</th>
   </tr>
   <tr>

       <td><code>stcany.upp</code></td>

       <td>360&times;360dpi</td>

       <td>4-bit, PostScript halftoning</td>
   </tr>
   <tr>

       <td><code>stcany_h.upp</code></td>

       <td>720&times;720dpi</td>

       <td>4-bit, PostScript halftoning</td>
       </tr>
   <tr>
       <th align="left" colspan="3">Original Epson Stylus and Stylus Pro Color</th>
       </tr>
   <tr>

       <td><code>stc.upp</code></td>

       <td>360&times;360dpi</td>

       <td>32-bit CMYK, 15-pin</td>
       </tr>
   <tr>

       <td><code>stc_l.upp</code></td>

       <td>360&times;360dpi</td>

       <td>4-bit, PostScript halftoning, weaved noWeave</td>
       </tr>
   <tr>

       <td><code>stc_h.upp</code></td>

       <td>720&times;720dpi</td>

       <td>32-bit CMYK, 15-pin Weave</td>
       </tr>
   <tr>
       <th align="left" colspan="3">Epson Stylus Color II</th>
       </tr>
   <tr>

       <td><code>stc2.upp</code></td>

       <td>360&times;360dpi</td>

       <td>32-bit CMYK, 20-pin, Epson Stylus Color II(s)</td>
       </tr>
   <tr>

       <td><code>stc2_h.upp</code></td>

       <td>720&times;720dpi</td>

       <td>32-bit CMYK, 20-pin, Epson Stylus Color II</td>
       </tr>
   <tr>

       <td><code>stc2s_h.upp</code></td>

       <td>720&times;720dpi</td>

       <td>32-bit CMYK, 20-pin, Epson Stylus Color IIs</td>
       </tr>
   <tr>
       <th align="left" colspan="3">Epson Stylus Color 200</th>
       </tr>
   <tr>

       <td><code>stc200.upp</code></td>

       <td>360&times;720dpi</td>

       <td>Plain Paper</td>
       </tr>
   <tr>
       <th align="left" colspan="3">Epson Stylus Color 300</th>
       </tr>
   <tr>

       <td><code>stc300.upp</code></td>

       <td>360&times;360dpi</td>

       <td>32-bit CMYK, plain paper</td>
       </tr>
   <tr>

       <td><code>stc300bl.upp</code></td>

       <td>180&times;180dpi</td>

       <td>black only, plain paper</td>
       </tr>
   <tr>

       <td><code>stc300bm.upp</code></td>

       <td>360&times;360dpi</td>

       <td>black only, plain paper</td>
       </tr>
   <tr>
       <th align="left" colspan="3">Epson Stylus Color 500 (good transfer curves for plain paper)</th>
       </tr>
   <tr>

       <td><code>stc500p.upp</code></td>

       <td>360&times;360dpi</td>

       <td>32-bit CMYK, noWeave, plain paper</td>
       </tr>
   <tr>

       <td><code>stc500ph.upp</code></td>

       <td>720&times;720dpi</td>

       <td>32-bit CMYK, noWeave, plain paper</td>
       </tr>
   <tr>
       <th align="left" colspan="3">Epson Stylus Color 600, 32/90-inch weaving</th>
       </tr>
   <tr>

       <td><code>stc600pl.upp</code></td>

       <td>360&times;360dpi</td>

       <td>32-bit CMYK, 32-pin, plain paper</td>
       </tr>
   <tr>

       <td><code>stc600p.upp</code></td>

       <td>720&times;720dpi</td>

       <td>32-bit CMYK, 32-pin, plain paper</td>
       </tr>
   <tr>

       <td><code>stc600ih.upp</code></td>

       <td>1440&times;720dpi</td>

       <td>32-bit CMYK, 30-pin, inkjet paper</td>
       </tr>
   <tr>
       <th align="left" colspan="3">Epson Stylus Color 640</th>
       </tr>
   <tr>

       <td><code>stc640p.upp</code></td>

       <td>720&times;720dpi</td>

       <td>plain paper?</td>
       </tr>
   <tr>

       <td><code>st640p.upp</code></td>

       <td>720&times;720dpi</td>

       <td>CMYK, plain paper</td>
       </tr>
   <tr>

       <td><code>st640pg.upp</code></td>

       <td>720&times;720dpi</td>

       <td>grayscale, plain paper</td>
       </tr>
   <tr>

       <td><code>st640pl.upp</code></td>

       <td>360&times;360dpi</td>

       <td>CMYK, plain paper</td>
       </tr>
   <tr>

       <td><code>st640plg.upp</code></td>

       <td>360&times;360dpi</td>

       <td>grayscale, plain paper</td>
       </tr>
   <tr>

       <td><code>st640ih.upp</code></td>

       <td>1440&times;720dpi</td>

       <td>CMYK, inkjet paper</td>
       </tr>
   <tr>

       <td><code>st640ihg.upp</code></td>

       <td>1440&times;720dpi</td>

       <td>grayscale, inkjet paper</td>
       </tr>
   <tr>
       <th align="left" colspan="3">Epson Stylus Color 800, 64/180-inch weaving</th>
       </tr>
   <tr>

       <td><code>stc800pl.upp</code></td>

       <td>360&times;360dpi</td>

       <td>32-bit CMYK, 64-pin, plain paper</td>
       </tr>
   <tr>

       <td><code>stc800p.upp</code></td>

       <td>720&times;720dpi</td>

       <td>32-bit CMYK, 64-pin, plain paper</td>
       </tr>
   <tr>

       <td><code>stc800ih.upp</code></td>

       <td>1440&times;720dpi</td>

       <td>32-bit CMYK, 62-pin, inkjet paper</td>
       </tr>
   <tr>

       <td><code>stc1520.upp</code></td>

       <td>1440&times;720dpi</td>

       <td>32-bit CMYK, 62-pin, inkjet paper</td>
       </tr>
   <tr>
       <th align="left" colspan="3">Sun raster file</th>
       </tr>
   <tr>

       <td><code>ras1.upp</code></td>

       <td>1-bit</td>

       <td>monochrome (Ghostscript)</td>
       </tr>
   <tr>

       <td><code>ras3.upp</code></td>

       <td>3-bit</td>

       <td>RGB (Ghostscript)</td>
       </tr>
   <tr>

       <td><code>ras4.upp</code></td>

       <td>4-bit</td>

       <td>CMYK (Ghostscript)</td>
       </tr>
   <tr>

       <td><code>ras8m.upp</code></td>

       <td>8-bit</td>

       <td>grayscale (Floyd-Steinberg)</td>
       </tr>
   <tr>

       <td><code>ras24.upp</code></td>

       <td>24-bit</td>

       <td>RGB (Floyd-Steinberg)</td>
       </tr>
   <tr>

       <td><code>ras32.upp</code></td>

       <td>32-bit</td>

       <td>CMYK (CMYK-Floyd-Steinberg)</td>
   </tr>
   </table>


|


Thanks to Danilo Beuche, Guido Classen, Mark Goldberg and Hans-Heinrich Viehmann for providing the files for the ``stc200``, ``hp690``, ``stc500`` and the ``stc640``. Thanks to `Michael Lossin`_ for the newer ``st640`` parameter sets.


.. note::

   - Changing the resolution with Ghostscript's ``-r`` switch is usually not possible.

   - For Epson Stylus Color models not listed above, the two ``stc500`` variants are likely to work in addition to ``stcany``, but their gamma correction might be wrong.


The state of this driver
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The coding of :title:`uniprint` was triggered by the requirements of the various Stylus Color models and some personal needs for HP and NEC drivers. Thus the Epson models are well represented among the distributed parameter files. When this driver entered the beta test phase, three other drivers appeared on the scene that could be at least partially integrated into :title:`uniprint`: :title:`cdj850` by Uli Wortmann, ``hpdj`` by Martin Lottermoser, and ``bjc610`` by Helmut Riegler.

Uli addresses features of the more recent DeskJet models that will not be available in :title:`uniprint` soon. Martin taught me a lesson on HP-PCL3 headers that will be available in :title:`uniprint` soon. Helmut in turn followed an almost similar idea, but targetted primarily for printing on Canon printers from the ``pbmplus`` library. Starting with version 1.68 of :title:`uniprint`, BJC support is available. Work on the ``hpdj`` integration will start after the update of my website.


Notes on :title:`uniprint`'s background
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

:title:`uniprint` is actually an update of :title:`stcolor`, but much more versatile than its predecessor; :title:`stcolor`, in its turn, started as a clone of the color DeskJet family of drivers (``cdj*``). Finally, ``cdj*`` can be considered an addition of features to the simpler monochrome drivers of Ghostscript. This addition of features is useful to get an idea of the functionality of :title:`uniprint`:

Monochrome to advanced color (``cdj*``)
   This adds color mapping and rendering functions to the driver. Error diffusion is especially important for the quality of printing.

HP color to Epson Color (:title:`stcolor`)
   The Epson Stylus Color offered two features simultaneously: it could produce 720×720dpi output and it could soak the paper. In other words, it required more color management features inside the driver. This is still the major conceptual difference in the data generation for HP and Epson printers.

Weaving techniques (:title:`stcolor`)
   Besides the internal color management, the Stylus Color did not provide enough buffer space to operate the printer fast at 720×720dpi. The use of weaving could yield triple the print speed. Weaving, also called interleaving, is present in some monochrome drivers too. The new thing in :title:`stcolor` was the combination with error diffusion. Unfortunately the weaving was somehow hard-coded, as the problems with the newer members of the Stylus Color family of printers demonstrated.

Generalized output format and weaving (:title:`uniprint`)
   The features mentioned above yield about 90% of :title:`stcolor`'s source code; only 10% is related to the formatting of the output. The idea to make the output format switchable came up soon after completing stcolor, but its final design was triggered by the (personal) necessity to drive a NEC P2X and a Designjet 750c.


Thus :title:`uniprint` accumulates almost any features that can be found among the other printer drivers, which clearly has some disadvantage in processing speed -- true in particular of version 1.75, since it was targetted for functionality, and several speed-gaining features were (knowingly) omitted.

To summarize and to introduce the terms used in the description of the parameters, the features of :title:`uniprint` that can be parameterized are:

- Color mapping.
- Color rendering (error diffusion or Floyd-Steinberg).
- Output format, including weaving.


Godzilla's guide to the creation of Unified Printer Parameter (``.upp``) files
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Here is one of the distributed parameter files (``stc_l.upp``) with some added comments. Also see the section that describes all :title:`uniprint`'s parameters in brief.


.. code-block::

   -supModel="Epson Stylus Color I (and PRO Series), 360x360DpI, noWeave"
   -sDEVICE=uniprint                    -- Select the driver
   -dNOPAUSE                            -- Useful with printers
   -dSAFER                              -- Provides some security
   -dupColorModel=/DeviceCMYK           -- Selects the color mapping
   -dupRendering=/ErrorDiffusion        -- Selects the color rendering
   -dupOutputFormat=/EscP2              -- Selects the output format
   -r360x360                            -- Adjusts the resolution
   -dupMargins="{ 9.0 39.96 9.0 9.0}"   -- Establishes (L/B/R/T margins in points)
   -dupComponentBits="{1 1 1 1}"        -- Map: bits per component (default: 8)
   -dupWeaveYPasses=4                   -- Weave: Y-passes (default: 1)
   -dupOutputPins=15                    -- Format/weave: scans per Command
   -dupBeginPageCommand="<              -- Goes to the printer
     1b40   1b40                        -- ESC '@' ESC '@'    -> dual reset
     1b2847 0100 01                     -- ESC '(' 'G' 1 0 1  -> graphics
     1b2869 0100 00                     -- ESC '(' 'i' 1 0 1  -> no HW weave
     1b2855 0100 0A                     -- ESC '(' 'U' 1 0 10 -> 360dpi
     1b5500                             -- ESC 'U'  0         -> bidir print
     1b2843 0200 0000                   -- ESC '(' 'C' 2 0 xx -> page length
     1b2863 0400 0000 0000              -- ESC '(' 'c' 4 0 xxxx -> margins
   >"                                   -- as it is, unless:
   -dupAdjustPageLengthCommand          -- Adjust page length in BOP requested
   -dupAdjustTopMarginCommand           -- Adjust top margin in BOP
   -dupAdjustBottomMarginCommand        -- Adjust bottom margin in BOP
   -dupEndPageCommand="(\033@\014)"     -- Last (but one) data to the printer
   -dupAbortCommand="(\033@\15\12\12\12\12    Printout-Aborted\15\014)"


That's short, and if one removes ``upWeaveYPasses`` and ``upOutputPins`` it becomes shorter, almost ``stcany.upp``. This miniature size is because I am most familiar with ESC/P2, and was able to add defaults for the omitted parameters. Now a few notes about the parameters used in this example:


- ``upModel`` is a string serving as a comment (and nothing else).
- ``DEVICE``, ``NOPAUSE``, ``SAFER`` are well-known Ghostscript parameters described in the :ref:`usage documentation<Use.html>`.
- ``upColorModel`` is one of the major :title:`uniprint` parameters: it selects the color mapping and in turn the PostScript color model. It supports the devices ``/DeviceGray``, ``/DeviceRGBW``, ``/DeviceRGB``, ``/DeviceCMYK``, and ``/DeviceCMYKgenerate``.
- ``upRendering`` selects the (color) rendering, supporting the values ``/ErrorDiffusion`` and ``/FSCMYK32``. ``/ErrorDiffusion`` is similar to ``fsmono``, ``fsrgb`` and ``fsx4`` of :title:`stcolor`, while ``/FSCMYK32`` is (almost) identical to ``fscmyk`` and ``hscmyk``, but is restricted to 32-bit data and should be used in conjunction with ``/DeviceCMYKgenerate``.
- ``upOutputFormat`` selects the output method, supporting the values ``/SunRaster``, ``/Epson``, ``/EscP2``, ``/EscP2XY``, and ``/Pcl``.


   .. list-table::
      :widths: 50 50
      :header-rows: 0

      * - ``/SunRaster``
        - creates Sun raster files and requires no other parameters
      * - ``/Epson``
        - is used for the elderly ESC/P format (used by many printers)
      * - ``/EscP2``
        - is used by more recent Epson printers (no X weaving supported)
      * - ``/EscP2XY``
        - supports X-Weaving, used with 1440dpi printers and in ``stc2s_h``
      * - ``/Pcl``
        - HP PCL/RTL-style output formatter without weaving



- ``-r360x360`` is Ghostscript's standard resolution switch.
- ``upMargins="{ 9.0 39.96 9.0 9.0}"`` has function similar to the Ghostscript parameter ``.HWMargins``: it sets the left, bottom, right, and top margins in points. :title:`uniprint` provides this parameter to enable automatic left-right exchange if ``upYFlip`` is active.
- ``upComponentBits`` is an array of integers that selects the bits stored in raster memory, by default 8 bits per component. In this example, 1 bit is selected for each component, thus turning down the Floyd-Steinberg algorithm (but still carrying out the time-consuming computation). The related parameter ``upComponentShift`` controls positioning the components within raster memory. Each of the numbers given corresponds to a component which depends on the selected ``upColorModel``:


   .. list-table::
      :header-rows: 1

      * -
        - /DeviceGray
        - /DeviceRGBW
        - /DeviceRGB
        - /DeviceCMYK
        - /DeviceCMYKgenerate
      * - 0
        - White
        - White
        - Red
        - Black
        - Black
      * - 1
        - --
        - Red
        - Green
        - Cyan
        - Cyan
      * - 2
        - --
        - Green
        - Blue
        - Magenta
        - Magenta
      * - 3
        - --
        - Blue
        - --
        - Yellow
        - Yellow


   This order may not be suitable for some printers, so another parameter ``upOutputComponentOrder``, also an array of integers, selects the output order using the numbers on the left.


   One group of very important parameters not used in the example above deserves to be mentioned here: the transfer arrays, named ``up{color}Transfer``, where ``{color}`` is one of the names in the table above. These are arrays of floats in the range 0.0 - 1.0 representing the color transfer functions. They are used during mapping and rendering. In the simplest case, these arrays ensure an equidistant distribution of the stored values within the device space (which means a nonlinear mapping from Ghostscript's point of view). If the given array does not cover the entire range from 0 to 1, which applies for the Stylus Color family at high resolution for some media, only the relevant part gets mapped to raster memory (meaning that is's fully utilized) and the rendering takes care of the "overhang" (in this case the post-diffusion of 1-bit components makes sense).

   Finally an important note on the transfer arrays: for monochrome devices the stored component is ``White``, which is the way PostScript defines these devices, but most printers require ``Black``. Thus one has to provide a falling ``upWhiteTransfer`` for such printers.


- ``upWeaveYPasses`` is an integer that gives the number of print head passes required to achieve the requested ``Ydpi``. This makes sense only if ``upOutputPins`` is set to something greater than 1. Thus multiple pins or nozzles are transferred with a single command, and of course such a command must be supported by the device.


If no other weave parameters are given, :title:`uniprint` computes several defaults which together do no weaving. The ``/Epson`` and ``/EscP2XY`` formats take care of ``upWeaveXPasses`` too.


- ``upBeginPageCommand`` represents the data transferred to the printer whenever a new page begins. Before that, ``upBeginJobCommand`` is written to the device only once per output file. (Intended for the HP PJL sequences).

- ``upAdjustBottomMarginCommand``, ``upAdjustMediaSize``, ``upAdjustPageLengthCommand``, ``upAdjustPageWidthCommand``, ``upAdjustResolutionCommand``, and ``upAdjustTopMarginCommand``.

   Normally :title:`uniprint` does not change the ``upBeginPageCommand``, nor does it provide a default. However, if the above boolean values are set, the corresponding values are changed (provided that the code of the formatters supports this change and the commands to be adjusted are included in the BOP string).

- ``upEndPageCommand`` is the fixed termination sequence for each page, and of course there is an ``upEndJobCommand`` too.

- ``upAbortCommand`` is written if :title:`uniprint`'s interrupt detection is enabled and a signal is caught. It replaces ``upEndPageCommand`` and ``upEndJobCommand``, thus allowing the indication of an aborted job. (Ghostscript gets an error return from :title:`uniprint` in this case, and abandons further processing).


For the ESC/P(2) formats all commands represent binary data, while for the PCL/RTL formatter some of them are formats for ``fprintf``. These strings must explicitly have a trailing ``"\0'``.

I should write more, but the only recommendation is to take a look at the various parameter files. Here are a few more hints.

- If the Driver rejects a configuration, nothing happens until showpage; then an error is raised and a message with ``CALL-REJECTED upd_print_page...`` is printed on ``stderr``.
- :title:`uniprint` has lots of messages that can be activated by setting bits in the preprocessor macro ``UPD_MESSAGES``. I usually use the compile-time option ``-DUPD_MESSAGES=0x17`` for configuration development. (For the semantics, check the ``UPD_M_`` macros in the source).
- A program ``"uninfo.ps"`` distributed with Ghostscript displays interactively in alphabetical order the contents of the current ``pagedevice`` dictionary. This includes any parameters generated or changed by :title:`uniprint`.



All parameters in brief
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

This table gives a brief explanation of every parameter known to :title:`uniprint`, listing them in alphabetical order. "``[ ]``" denotes that a parameter is an array, and "``(RO)``" that it is read-only.



.. raw:: html

   <table width=100% border=1>
   <tr>
       <th colspan="3">All uniprint parameters</th>
       </tr>
   <tr>
       <th align="left">Parameter</th>

       <th align="left">Type</th>

       <th align="left">Use</th>
       </tr>
   <tr>
       <td><code>upAbortCommand</code></td>

       <td>String</td>

       <td>End of page and file on interrupt</td>
       </tr>
   <tr>
       <td><code>upAdjustBottomMarginCommand</code></td>

       <td>Bool</td>

       <td>Manipulate bottom margin in <code>upBeginPageCommand</code></td>
       </tr>
   <tr>
       <td><code>upAdjustMediaSizeCommand</code></td>

       <td>Bool</td>

       <td>Manipulate <code>Mediasize</code> [intended]</td>
       </tr>
   <tr>
       <td><code>upAdjustPageLengthCommand</code></td>

       <td>Bool</td>

       <td>Manipulate page length in <code>upBeginPageCommand</code></td>
       </tr>
   <tr>
       <td><code>upAdjustPageWidthCommand</code></td>

       <td>Bool</td>

       <td>Manipulate page width in <code>upBeginPageCommand</code></td>
       </tr>
   <tr>
       <td><code>upAdjustResolutionCommand</code></td>

       <td>Bool</td>

       <td>Manipulate resolution</td>
       </tr>
   <tr>
       <td><code>upAdjustTopMarginCommand</code></td>

       <td>Bool</td>

       <td>Manipulate top margin in <code>upBeginPageCommand</code></td>
       </tr>
   <tr>
       <td><code>upBeginJobCommand</code></td>

       <td>String</td>

       <td>Begin each output file</td>
       </tr>
   <tr>
       <td><code>upBeginPageCommand</code></td>

       <td>String</td>

       <td>Begin each page</td>
       </tr>
   <tr>
       <td><code>upBlackTransfer</code></td>

       <td>Float[&nbsp;]</td>

       <td>Black transfer (CMYK only!)</td>
       </tr>
   <tr>
       <td><code>upBlueTransfer</code></td>

       <td>Float[&nbsp;]</td>

       <td>Blue transfer</td>
       </tr>
   <tr>
       <td><code>upColorInfo</code></td>

       <td>Int[&nbsp;]</td>

       <td>struct <code>gx_device_color_info</code></td>
       </tr>
   <tr>
       <td><code>upColorModel</code></td>

       <td>Name</td>

       <td>Select color mapping</td>
       </tr>
   <tr>
       <td><code>upColorModelInitialized</code></td>

       <td>Bool (RO)</td>

       <td>Color mapping OK</td>
       </tr>
   <tr>
       <td><code>upComponentBits</code></td>

       <td>Int[&nbsp;]</td>

       <td>Bits stored per component</td>
       </tr>
   <tr>
       <td><code>upComponentShift</code></td>

       <td>Int[&nbsp;]</td>

       <td>Positioning within <code>gx_color_index</code></td>
       </tr>
   <tr>
       <td><code>upCyanTransfer</code></td>

       <td>Float[&nbsp;]</td>

       <td>Cyan transfer</td>
       </tr>
   <tr>
       <td><code>upEndJobCommand</code></td>

       <td>String</td>

       <td>End each file unless <code>upAbortCommand</code></td>
       </tr>
   <tr>
       <td><code>upEndPageCommand</code></td>

       <td>String</td>

       <td>End each page unless <code>upAbortCommand</code></td>
       </tr>
   <tr>
       <td><code>upErrorDetected</code></td>

       <td>Bool (RO)</td>

       <td>Severe (VM) error, not fully operational</td>
       </tr>
   <tr>
       <td><code>upFSFixedDirection</code></td>

       <td>Bool</td>

       <td>Inhbits direction toggling in rendering</td>
       </tr>
   <tr>
       <td><code>upFSProcessWhiteSpace</code></td>

       <td>Bool</td>

       <td>Causes white-space rendering</td>
       </tr>
   <tr>
       <td><code>upFSReverseDirection</code></td>

       <td>Bool</td>

       <td>Run rendering in reverse (if fixed)</td>
       </tr>
   <tr>
       <td><code>upFSZeroInit</code></td>

       <td>Bool</td>

       <td>Non-random rendering initialization</td>
       </tr>
   <tr>
       <td><code>upFormatXabsolute</code></td>

       <td>Bool</td>

       <td>Write absolute X coordinates</td>
       </tr>
   <tr>
       <td><code>upFormatYabsolute</code></td>

       <td>Bool</td>

       <td>Write absolute Y coordinates</td>
       </tr>
   <tr>
       <td><code>upGreenTransfer</code></td>

       <td>Float[&nbsp;]</td>

       <td>Green transfer</td>
       </tr>
   <tr>
       <td><code>upMagentaTransfer</code></td>

       <td>Float[&nbsp;]</td>

       <td>Magenta transfer</td>
       </tr>
   <tr>
       <td><code>upMargins</code></td>

       <td>Float[&nbsp;]</td>

       <td>L/B/R/T margins in points</td>
       </tr>
   <tr>
       <td><code>upModel</code></td>

       <td>String</td>

       <td>Comment string, holds some info</td>
       </tr>
   <tr>
       <td><code>upOutputAborted</code></td>

       <td>Bool (RO)</td>

       <td>Caught an interrupt</td>
       </tr>
   <tr>
       <td><code>upOutputBuffers</code></td>

       <td>Int</td>

       <td>Number of rendering buffers (2^<small><sup><b>N</b></sup></small>)</td>
       </tr>
   <tr>
       <td><code>upOutputComponentOrder</code></td>

       <td>Int[&nbsp;]</td>

       <td>Order of components when printing</td>
       </tr>
   <tr>
       <td><code>upOutputComponents</code></td>

       <td>Int</td>

       <td>Number of written components, not fully operational</td>
   <tr>
       <td><code>upOutputFormat</code></td>

       <td>Name</td>

       <td>Select output format</td>
       </tr>
   <tr>
       <td><code>upOutputFormatInitialized</code></td>

       <td>Bool (RO)</td>

       <td>Format data OK</td>
       </tr>
   <tr>
       <td><code>upOutputHeight</code></td>

       <td>Int</td>

       <td>Output height in pixels</td>
       </tr>
   <tr>
       <td><code>upOutputPins</code></td>

       <td>Int</td>

       <td>Number of pins / nozzles per command</td>
       </tr>
   <tr>
       <td><code>upOutputWidth</code></td>

       <td>Int</td>

       <td>Output width in pixels</td>
       </tr>
   <tr>
       <td><code>upOutputXOffset</code></td>

       <td>Int</td>

       <td>Offset in pixels, if <code>upFormatXabsolute</code></td>
   </tr>
   <tr>
       <td><code>upOutputXStep</code></td>

       <td>Int</td>

       <td>Divisor or multiplier for X coords</td>
       </tr>
   <tr>
       <td><code>upOutputYOffset</code></td>

       <td>Int</td>

       <td>Offset in pixels, if <code>upFormatYabsolute</code></td>
       </tr>
   <tr>
       <td><code>upOutputYStep</code></td>

       <td>Int</td>

       <td>Divisor or multiplier for Y coords</td>
       </tr>
   <tr>
       <td><code>upRasterBufferInitialized</code></td>

       <td>Bool (RO)</td>

       <td>GS buffer OK</td>
       </tr>
   <tr>
       <td><code>upRedTransfer</code></td>

       <td>Float[&nbsp;]</td>

       <td>Red transfer</td>
   </tr>
   <tr>
       <td><code>upRendering</code></td>

       <td>Name</td>

       <td>Select rendering algorithm</td>
       </tr>
   <tr>
       <td><code>upRenderingInitialized</code></td>

       <td>Bool (RO)</td>

       <td>Rendering parameters OK</td>
       </tr>
   <tr>
       <td><code>upSelectComponentCommands</code></td>

       <td>String[&nbsp;]</td>

       <td>Establish color (output order!)</td>
       </tr>
   <tr>
       <td><code>upSetLineFeedCommand</code></td>

       <td>String</td>

       <td>Adjust linefeed (Epson only)</td>
       </tr>
   <tr>
       <td><code>upVersion</code></td>

       <td>String (RO)</td>

       <td>Source code version</td>
       </tr>
   <tr>
       <td><code>upWeaveFinalPins</code></td>

       <td>Int[&nbsp;]</td>

       <td>Number of bottom pins on EOP passes</td>
       </tr>
   <tr>
       <td><code>upWeaveFinalScan</code></td>

       <td>Int</td>

       <td>Begin EOP passes (Y-coord)</td>
       </tr>
   <tr>
       <td><code>upWeaveFinalXStarts</code></td>

       <td>Int[&nbsp;]</td>

       <td>X-pass indices for EOP passes</td>
       </tr>
   <tr>
       <td><code>upWeaveFinalYFeeds</code></td>

       <td>Int[&nbsp;]</td>

       <td>Y increments for EOP passes</td>
       </tr>
   <tr>
       <td><code>upWeaveInitialPins</code></td>

       <td>Int[&nbsp;]</td>

       <td>Number of top pins on BOP passes</td>
       </tr>
   <tr>
       <td><code>upWeaveInitialScan</code></td>

       <td>Int</td>

       <td>End BOP passes (Y coord)</td>
       </tr>
   <tr>
       <td><code>upWeaveInitialXStarts</code></td>

       <td>Int[&nbsp;]</td>

       <td>X-pass indices for BOP passes</td>
       </tr>
   <tr>
       <td><code>upWeaveInitialYFeeds</code></td>

       <td>int[&nbsp;]</td>

       <td>Y increments for BOP passes</td>
       </tr>
   <tr>
       <td><code>upWeavePasses</code></td>

       <td>Int</td>

       <td>XPasses &times; YPasses</td>
       </tr>
   <tr>
       <td><code>upWeaveXPasses</code></td>

       <td>Int</td>

       <td>Number of X passes</td>
   </tr>
   <tr>
       <td><code>upWeaveXStarts</code></td>

       <td>Int[&nbsp;]</td>

       <td>X-pass indices for normal passes</td>
       </tr>
   <tr>
       <td><code>upWeaveYFeeds</code></td>

       <td>Int[&nbsp;]</td>

       <td>Y increments for normal passes</td>
       </tr>
   <tr>
       <td><code>upWeaveYOffset</code></td>

       <td>Int</td>

       <td>Number of blank or incomplete scans at BOP</td>
       </tr>
   <tr>
       <td><code>upWeaveYPasses</code></td>

       <td>Int</td>

       <td>Number of X passes</td>
       </tr>
   <tr>
       <td><code>upWhiteTransfer</code></td>

       <td>Float[&nbsp;]</td>

       <td>White transfer (monochrome devices!)</td>
       </tr>
   <tr>
       <td><code>upWriteComponentCommands</code></td>

       <td>String[&nbsp;]</td>

       <td>Commands to write each component</td>
       </tr>
   <tr>
       <td><code>upWroteData</code></td>

       <td>Bool (RO)</td>

       <td>Something (<code>BeginJob</code>) written to output</td>
       </tr>
   <tr>
       <td><code>upXMoveCommand</code></td>

       <td>String</td>

       <td>X positioning command</td>
       </tr>
   <tr>
       <td><code>upXStepCommand</code></td>

       <td>String</td>

       <td>Single step to the right</td>
       </tr>
   <tr>
       <td><code>upYFlip</code></td>

       <td>Bool</td>

       <td>Flips output along the Y axis</td>
       </tr>
   <tr>
       <td><code>upYMoveCommand</code></td>

       <td>String</td>

       <td>Y positioning command</td>
       </tr>
   <tr>
       <td><code>upYStepCommand</code></td>

       <td>String</td>

       <td>Single step down</td>
       </tr>
   <tr>
       <td><code>upYellowTransfer</code></td>

       <td>Float[&nbsp;]</td>

       <td>Yellow transfer</td>
       </tr>
   </table>



:title:`uniprint`'s Roll of Honor
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

I should mention all of the people who were involved in :title:`stcolor`'s evolution, but I've decided to start from scratch here for :title:`uniprint`:

John P. Beale
   for testing the ``stc600`` modes

Bill Davidson
   who triggered some weaving research and tested ``stc2s_h``

`L. Peter Deutsch`_
   who triggered ease of configuration

Mark Goldberg
   who prepared the ``stc500`` transfers

Scott F. Johnston and Scott J. Kramer
   for testing the ``stc800`` modes

Martin Lottermoser
   for his great commented H-P DeskJet driver

Helmut Riegler
   for the BJC extension

Hans-Gerd Straeter
   for some measured transfer curves and more

Uli Wortmann
   for discussions and his :title:`cdj850` driver

My family
   for tolerating my printer-driver hacking


`Gunther Hess`_
Duesseldorfer Landstr. 16b, D-47249 Duisburg ,Germany, +49 203 376273 telephone (MET evening hours)



Uniprint weaving parameters HowTo
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

This section was contributed by Glenn Ramsey.

I wrote this because the documentation was very brief and I really struggled with it for a while, but it is very simple once you understand what is going on.

This only describes how to work out the Y parameters, I haven't looked at the X parameters yet.



1. Determine the nozzle geometry (``upOutputPins``)
   You need to know how many nozzles the printer has and the spacing between them. Usually you can find this out from the printer manual, or the printer supplier, but you may have to dissect a couple of printer output files produced with the driver supplied with the printer. There is a utility called ``escp2ras*`` that will help with that. Sometimes the term pin is used instead of nozzle but they mean the same thing.

   The number of nozzles will be the value assigned to the ``upOutputPins`` parameter.

   Actually you don't have to print with all the pins available but for the purpose of demonstration I'll assume that we are using them all.

2. Determine how many passes are required (``upWeaveYPasses``).

3. The number of passes required is going to depend on the required resolution and the nozzle spacing.


   .. code-block::

      passes = resolution * nozzle spacing

   This will be the value assigned to the ``upWeaveYPasses`` parameter.

   For example if the desired resolution is 360 dpi and the nozzles are spaced at 1/90in then 360 * 1/90 = 4 passes are required. For 720 dpi 8 passes would be required. The printer would, of course, have to be capable of moving the paper in increments of either 360 or 720 dpi too.

4. Determine the normal Y feed increment (``upWeaveYFeeds``)

   You need to work out how much to feed the paper so that when the paper has moved by one head length in however many passes you have then each row space on the paper has been passed over by at least one nozzle. There will be one feed value for each pass and the feed values must comply with the following rules:

   .. code-block::

      sum of feeds = passes * nozzles
      feed%passes != 0 (feed is not exactly divisible by passes)
      sum of (nozzles - feed) = 0

   For example if ``passes=4`` and ``nozzles=15``, then sum of ``feeds=60``. The feed values could be ``1,1,1,57`` or ``15,15,15,15`` or ``14,15,18,13``.

   These values will be assigned to the ``upWeaveYFeeds`` parameter.

   You would need to experiment to see what combination looks best on the printer.

   I found it convenient to draw several lines of nozzles and then move them around to see how the different combinations would fill the paper. A computer drawing tool makes this easier than pencil and paper (I used Dia, a GNOME app). The number of nozzles would probably be be a good place to start.

   Remember that if the number of passes is more than 1 then the feed increment will be less than the nozzle spacing and passes × feed increment size must equal the physical distance between each nozzle.

5. Determine the beginning of page pins (``upWeaveInitialPins``).

   These values will be assigned to the ``upWeaveInitialPins`` parameter and are the numbers of nozzles to operate in each of the initial passes at the top of a page. The nozzles that the values refer to are the topmost nozzles on the head, nearest the top margin. If the image doesn't start at the top margin then uniprint doesn't use these feeds.

   I don't know a mathematical relation for this except that at least one of the values must be the number of nozzles, but I'm sure that there must be one. I used a graphical method, the description that follows refers to the ascii diagram in below.

   Draw a line of nozzles for each pass arranged as they would be using the normal Y feed increment determined in step 3. In the diagram below this would be passes 5-8.

   Draw a line of nozzles that would print just before the first normal pass. The feed increment for this pass will be close to and most likely 1 or 2 units less than the feed increment of the last normal pass. In the example below this line is pass 4 and the feed increment is 13 whereas the normal feed increment is 15.

   Draw each pass before that with a small feed increment so that if all of the nozzles appearing above the first nozzle of the first normal pass operate then all of the spaces will be filled. This feed increment is usually 1 except in cases where some jiggery pokery is going on to make the printer print at an apparent higher resolution than the nozzle diameter.

   Now select the nozzles that will operate in each of theses initial passes so that the paper is filled. In each pass the nozzles must be adjacent to each other and at least one of the passes will have all the nozzles operating. I suspect that for each combination of normal Y feed increments there will only be one set of valid beginning of page increments.


Example: stc.upp from Aladdin Ghostscript 6.01
""""""""""""""""""""""""""""""""""""""""""""""""""""""

15 nozzles spaced at 1/90 in, 360 dpi requires 4 passes.


.. code-block::

   -dupWeaveYPasses=4
   -dupOutputPins=15
   -dupWeaveYFeeds="{15 15 15 15}"
   -dupWeaveInitialYFeeds="{1 1 1 13}"
   -dupWeaveInitialPins="{ 4 15 11 7}"

The following diagram shows which nozzles operate during each pass.

Passes 1-4 are beginning of page passes and passes 5-8 are normal passes.



.. raw:: html

   <blockquote>
   <p>x=nozzle operates, o=nozzle not used in this pass<tt></tt></p>
   <p><tt>&nbsp; 1 2 3 4 5 6 7 8 - pass no</tt></p>
   <br><tt>0 x</tt>
   <br><tt>1&nbsp;&nbsp; x</tt>
   <br><tt>2&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>3&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>4 x</tt>
   <br><tt>5&nbsp;&nbsp; x</tt>
   <br><tt>6&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>7&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>8 x</tt>
   <br><tt>9&nbsp;&nbsp; x</tt>
   <br><tt>0&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>1&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>2 x</tt>
   <br><tt>3&nbsp;&nbsp; x</tt>
   <br><tt>4&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>5&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>6 o&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>7&nbsp;&nbsp; x</tt>
   <br><tt>8&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>9&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>0 o&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>1&nbsp;&nbsp; x</tt>
   <br><tt>2&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>3&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>4 o&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>5&nbsp;&nbsp; x</tt>
   <br><tt>6&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>7&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>8 o&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>9&nbsp;&nbsp; x</tt>
   <br><tt>0&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>1&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; o&nbsp;&nbsp; x</tt>
   <br><tt>2 o&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>3&nbsp;&nbsp; x</tt>
   <br><tt>4&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>5&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; o&nbsp;&nbsp; x</tt>
   <br><tt>6 o&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>7&nbsp;&nbsp; x</tt>
   <br><tt>8&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>9&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; o&nbsp;&nbsp; x</tt>
   <br><tt>0 o&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>1&nbsp;&nbsp; x</tt>
   <br><tt>2&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>3&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; o&nbsp;&nbsp; x</tt>
   <br><tt>4 o&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>5&nbsp;&nbsp; x</tt>
   <br><tt>6&nbsp;&nbsp;&nbsp;&nbsp; o&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>7&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; o&nbsp;&nbsp; x</tt>
   <br><tt>8 o&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>9&nbsp;&nbsp; x</tt>
   <br><tt>0&nbsp;&nbsp;&nbsp;&nbsp; o&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>1&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; o&nbsp;&nbsp; x</tt>
   <br><tt>2 o&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>3&nbsp;&nbsp; x</tt>
   <br><tt>4&nbsp;&nbsp;&nbsp;&nbsp; o&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>5&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; o&nbsp;&nbsp; x</tt>
   <br><tt>6 o&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>7&nbsp;&nbsp; x</tt>
   <br><tt>8&nbsp;&nbsp;&nbsp;&nbsp; o&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>9&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; o&nbsp;&nbsp; x</tt>
   <br><tt>0&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>1&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>2&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>3&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>4&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>5&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>6&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>7&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>8&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>9&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>0&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>1&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>2&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>3&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>4&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>5&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>6</tt>
   <br><tt>7&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>8&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>9&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>0</tt>
   <br><tt>1&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>2&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>3&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>4</tt>
   <br><tt>5&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>6&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>7&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; x</tt>
   <br><tt>8</tt>
   <br><tt>9&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>0&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>1</tt>
   <br><tt>2</tt>
   <br><tt>3&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>4&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>5</tt>
   <br><tt>6</tt>
   <br><tt>7&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>8&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>9</tt>
   <br><tt>0</tt>
   <br><tt>1&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>2&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>3</tt>
   <br><tt>4</tt>
   <br><tt>5&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>6</tt>
   <br><tt>7</tt>
   <br><tt>8</tt>
   <br><tt>9&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>0</tt>
   <br><tt>1</tt>
   <br><tt>2</tt>
   <br><tt>3&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   <br><tt>4</tt>
   <br><tt>5</tt>
   <br><tt>6</tt>
   <br><tt>7&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
   x</tt>
   </blockquote>


These parameters would also work:


.. code-block::

   -dupWeaveYPasses=4
   -dupOutputPins=15
   -dupWeaveYFeeds="{14 15 18 13}"
   -dupWeaveInitialYFeeds="{1 1 1 13}"
   -dupWeaveInitialPins="{ 4 11 7 15}"


Extension to :title:`uniprint` for the Epson Stylus Color 300
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

This section was contributed by Glenn Ramsey.

The Epson Stylus Color 300 uses a different command set to other Epson Stylus Color printers that use the ESC/P2 language. As far as I can tell its commands are a subset of ESC/P2. In ESC/P2 the colour to be printed is selected by a 'set colour' command and then the data sent is only printed in that colour until the colour is changed with another 'set colour' command. The Stylus Color 300 lacks this functionality. The data sent to the printer maps directly to the ink nozzles and colour of an output scan line in the printed output is determined by the position of the scan line within the data. This means that the driver must know how the nozzles are arranged and must format the output accordingly. The extension adds a format that I have called ``EscNozzleMap`` and adds some additional parameters to :title:`uniprint`.


- upOutputFormatselects the output method, and should be set to the value ``/EscNozzleMap`` to select this format.

   ``/EscNozzleMap``
        produces output for the Epson Stylus Color 300


   .. raw:: html

      <table width=100% border=1>
         <tr>
           <th colspan="3">uniprint parameters for the EscNozzleMap format</th>
         </tr>
         <tr>
           <th align="Left">Parameter</th>

           <th align="Left">Type</th>

           <th align="Left">Use</th>
         </tr>
         <tr>
           <td colspan="3"></td>
         </tr>
         <tr>
           <td><code>upNozzleMapRowsPerPass</code></td>

           <td>Int
       </td>


           <td>output rows to generate for each pass of the head
       </td>
         </tr>
         <tr>
           <td><code>upNozzleMapPatternRepeat</code></td>

           <td>Int
       </td>

           <td>no. of rows that correspond to the repeat pattern of the
           nozzles
       </td>
         </tr>
         <tr>
           <td><code>upNozzleMapRowMask</code></td>
           <td>Int[]
       </td>
           <td>mask indicating the colour of the nozzles
       </td>
         </tr>
         <tr>
           <td><code>upNozzleMapMaskScanOffset</code></td>

           <td>Int[]
       </td>
           <td>mask indicating the physical position of the nozzles
       </td>
         </tr>
     </table>


**A more detailed description of the new parameters**

``upNozzleMapRowsPerPass``
   The number of rows of data that are required to address all nozzles for a single pass of the head. There will always be this number of rows of output data generated. I'd expect it to be the same as the total number of nozzles but it wouldn't break the formatter if it wasn't. So if you wanted to print with only the 10th nozzle then row 10 would contain data corresponding to the bit pattern and all of the others would be padded with zeros.

``upNozzleMapPatternRepeat``
   The number of nozzles in each repeated group on the printing head. This parameter must correspond with the length of the ``upNozzleMapRowMask`` array.

``upNozzleMapRowMask``
   An array of integers that defines the colour of the nozzles on the head and whether the nozzles will be used to print. The array index defines the row index for the nozzle in the output data and the value defines the colour of the nozzle. The mapping of colours to values is defined in the table below.


   .. list-table::
      header-rows: 1


      * - colour   mask value
        -
      * - ``K``
        - 1
      * - ``C``
        - 2
      * - ``M``
        - 3
      * - ``Y``
        - 4
      * - ``no data``
        - 0


A value of 0 means that the nozzle is not used and the row in the output data will be padded with zeros.

``upNozzleMapMaskScanOffset``
   An array of integers that defines the physical position of the nozzles relative to the first nozzle in the repeated group. The relative distance is measured in printed line widths and will be different for different printing resolutions. This parameter is used because the physical spacing of the nozzles may not correspond to their mapping in the output data. For example the ESC300 has nozzles physically arranged something like this:



.. raw:: html

   <table width=100% border=1>
       <tr>
         <td style="background-color: #00ffff !important;">&nbsp;
         </td>
         <td style="background-color: #ffff00 !important;">&nbsp;
         </td>
         <td style="background-color: #000000 !important;">&nbsp;
         </td>
         <td style="background-color: #ff00ff !important;">&nbsp;
         </td>
         <td style="background-color: #000000 !important;">&nbsp;
         </td>
         <td style="background-color: rgba(0,0,0,0) !important;">&nbsp;
         </td>
         <td style="background-color: #000000 !important;">&nbsp;
         </td>
         <td style="background-color: rgba(0,0,0,0) !important;">&nbsp;
         </td>
         <td style="background-color: rgba(0,0,0,0) !important;">&nbsp;
         </td>
         <td style="background-color: rgba(0,0,0,0) !important;">&nbsp;
         </td>
         <td style="background-color: rgba(0,0,0,0) !important;">&nbsp;
         </td>
         <td style="background-color: rgba(0,0,0,0) !important;">&nbsp;
         </td>
         <td style="background-color: #00ffff !important;">&nbsp;
         </td>
         <td style="background-color: #ffff00 !important;">&nbsp;
         </td>
         <td style="background-color: #000000 !important;">&nbsp;
         </td>
         <td style="background-color: #ff00ff !important;">&nbsp;
         </td>
         <td style="background-color: #000000 !important;">&nbsp;
         </td>
         <td style="background-color: rgba(0,0,0,0) !important;">&nbsp;
         </td>
         <td style="background-color: #000000 !important;">&nbsp;
         </td>
         <td style="background-color: #ffffff !important;">&nbsp;
         </td>
       </tr>
       <tr>
         <td style="background-color: rgba(0,0,0,0) !important;">&nbsp;
         </td>
         <td style="background-color: rgba(0,0,0,0) !important;">&nbsp;
         </td>
         <td style="background-color: rgba(0,0,0,0) !important;">&nbsp;
         </td>
         <td style="background-color: rgba(0,0,0,0) !important;">&nbsp;
         </td>
         <td style="background-color: rgba(0,0,0,0) !important;">&nbsp;
         </td>
         <td style="background-color: rgba(0,0,0,0) !important;">&nbsp;
         </td>
         <td style="background-color: #00ffff !important;">&nbsp;
         </td>
         <td style="background-color: #ffff00 !important;">&nbsp;
         </td>
         <td style="background-color: #000000 !important;">&nbsp;
         </td>
         <td style="background-color: #ff00ff !important;">&nbsp;
         </td>
         <td style="background-color: #000000 !important;">&nbsp;
         </td>
         <td style="background-color: rgba(0,0,0,0) !important;">&nbsp;
         </td>
         <td style="background-color: #000000 !important;">&nbsp;
         </td>
         <td style="background-color: rgba(0,0,0,0) !important;">&nbsp;
         </td>
         <td style="background-color: rgba(0,0,0,0) !important;">&nbsp;
         </td>
         <td style="background-color: rgba(0,0,0,0) !important;">&nbsp;
         </td>
         <td style="background-color: rgba(0,0,0,0) !important;">&nbsp;
         </td>
         <td style="background-color: rgba(0,0,0,0) !important;">&nbsp;
         </td>
         <td style="background-color: #00ffff !important;">&nbsp;
         </td>
         <td style="background-color: #ffff00 !important;">&nbsp;
         </td>
         <td style="background-color: rgba(0,0,0,0) !important;">etc ...
         </td>
       </tr>
     </table>


There is a one nozzle width space between the last two nozzles in each group. In the output data the data for the last nozzle in the group would be in row 5 (numbering starts at 0) but the nozzle is physically positioned at 6 spaces from the first nozzle.


Example 1 - Epson Stylus Color 300 - 360 dpi colour
""""""""""""""""""""""""""""""""""""""""""""""""""""""


.. code-block::

   -dupWeaveYPasses=6
   -dupOutputPins=11
   -dupWeaveYFeeds="{ 11 11 11 11 11 11 }"
   -dupWeaveInitialYFeeds="{ 1 1 1 1 1 7 }"
   -dupWeaveInitialPins="{ 2 11 9 7 5 3 }"
   -dupNozzleMapRowsPerPass=64
   -dupNozzleMapPatternRepeat=6
   -dupNozzleMapRowMask="{ 2 4 1 3 0 0 }"
   -dupNozzleMapMaskScanOffset="{ 0 1 2 3 0 0 }"


The weaving parameters are the same as for any other :title:`uniprint` driver but they must be consistent with the nozzle map parameters. In this printer the coloured nozzles are spaced at 1/60" so 6 passes are required for 360 dpi resolution.

In the example there are 64 rows of data required for each head pass. Each row must be completely filled with data for each pass so if certain nozzles do not print in the pass then the rows for those nozzles will be padded with zeroes.

The row mask translates to "C Y K M 0 0" so in the output data rows 0,7,13,... will contain data for cyan, rows 1,8,14,... will contain data for yellow, etc. Rows 4,10,16,... and 5, 11,15,... will always be padded with zeroes. The ``upNozzleMapPatternRepeat`` parameter defines the length of the mask.

The row mask is repeated for each group of ``upNozzleMapPatternRepeat`` rows in the output data. In this case there are 64 rows so there will be 10 groups of "C Y K M 0 0" followed by "C Y K M" which is equivalent to 11 output pins.

The ``upNozzleMaskScanOffset`` array indicates how the data from the scan buffer is mapped to the output data. The data is presented to the formatter as a buffer of four colour scanlines. The index of the scanline being printed, lets call it y, always corresponds, in this example, to the physical position of the cyan nozzle but since the nozzles are not on the same horizontal line then the other colours for the current pass must come from other scanlines in the scan buffer. The example is { 0 1 2 3 0 0 }, this means that when printing a 4 colour image the magenta data would come from scanline y+3, the black from scanline y+2, etc. It would have been possible in this case to use the array index instead of the ``upNozzleMaskScanOffset`` parameter however the parameter is necessary to be able to use the full capability of the printer in black only mode.



Example 2 - Epson Stylus Color 300 - 180 dpi black only
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

.. code-block::

   -dupMargins="{ 9.0 39.96 9.0 9.0}"
   -dupWeaveYPasses=1
   -dupOutputPins=31
   -dupNozzleMapRowsPerPass=64
   -dupNozzleMapPatternRepeat=6
   -dupNozzleMapRowMask="{ 0 0 1 0 1 1}"
   -dupNozzleMapMaskScanOffset="{ 0 0 0 0 1 2 }"


In this example there is no weaving.

The ESC300 has black nozzles evenly physically arranged as ``K K K`` but the data must be sent to the printer as ``00K0KK``. This is handled by the ``upNozzleMapRowMask`` and ``upNozzleMaskScanOffset`` arrays. The ``upNozzleMapRowMask`` array is ``{ 0 0 1 0 1 1}`` which translates to ``{ 0 0 K 0 K K }`` so rows 0, 1 and 3 will always contain zeros and the other rows will contain data.

The ``upNozzleMaskScanOffset`` array in this case is ``{ 0 0 0 0 1 2 }`` so if the data for the 1st nozzle comes from row y in the scan buffer then the data for the 2nd and 3rd nozzles will come from rows y+1 and y+2.


Example 3 - Epson Stylus Color 300 - 360 dpi black only
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

.. code-block::

   -dupWeaveYPasses=2
   -dupOutputPins=31
   -dupWeaveYFeeds="{31 31}"
   -dupWeaveInitialYFeeds="{1 31}"
   -dupWeaveInitialPins="{16 31}"
   -dupNozzleMapRowsPerPass=64
   -dupNozzleMapPatternRepeat=6
   -dupNozzleMapRowMask="{ 0 0 1 0 1 1}"
   -dupNozzleMapMaskScanOffset="{ 0 0 0 0 2 4 }"

In this example 2 weave passes are required to achieve the desired resolution.

The ``upNozzleMaskScanOffset`` array in this case is ``{ 0 0 0 0 2 4 }`` because there are two weave passes so if the data for the first nozzle comes from row y in the scan buffer then the data for the 2nd and 3rd nozzles must come from rows ``y+(1*2)`` and ``y+(2*2)``.


Glenn Ramsey

glennr at users.sourceforge.net

February 2001


.. External links

.. _Uli Wortmann: uliw@erdw.ethz.ch
.. _Martin Gerbershagen: ger@ulm.temic.de
.. _Matthew Gelhaus: hp880@gelhaus.net
.. _gelhaus.net/hp880c: http://www.gelhaus.net/hp880c/
.. _Stephan C. Buchert: scb@stelab.nagoya-u.ac.jp
.. _Yoshio Kuniyoshi: yoshio@nak.math.keio.ac.jp

.. _David Gaudine: david@donald.concordia.ca
.. _Robert M. Kenney: rmk@unh.edu
.. _James McPherson: someone@erols.com
.. _Ian Thurlbeck: ian@stams.strath.ac.uk
.. _Klaus-Gunther Hess: ghess@elmos.de
.. _Gunther Hess: ghess@elmos.de

.. _Jason Patterson: jason@reflections.com.au
.. _Michael Lossin: losse@germanymail.com

.. _L. Peter Deutsch: https://en.wikipedia.org/wiki/L._Peter_Deutsch

.. include:: footer.rst