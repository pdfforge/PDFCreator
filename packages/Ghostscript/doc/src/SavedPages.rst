.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: Using Saved Pages


.. include:: header.rst

.. _SavedPages.html:


Using Saved Pages
==========================================





Saved pages can be printed in any order, any number of times allowing for copy collation, manual 'duplex' printing or selecting a subset of pages.

Many raster printing devices and raster image format devices can use the command list (clist) to save the rendering information into files or memory based structures. Usually the clist data is rendered as soon as the page is complete, but it is` possible to defer rendering, then output (print) after the entire file has been converted to clist files.

Since each page is saved separately, the pages can be rendered from the clist files in any order, any number of times.

The saved pages are accumulated in clist form as files or in memory. The default clist storage is specified by the ``BAND_LIST_STORAGE=`` macro in the make file, but if ``BAND_LIST_STORAGE=file``, the clist can be forced to be in memory using the ``-sBandListStorage=memory`` option.







Saved Pages control keywords
---------------------------------

Deferring printing is controlled using the ``--saved-page=...`` option.

The string following the ``--saved-page=`` consists of keywords, separated by white-space, commas, or other non-alphanumeric characters such as '``:``', '``=``', '``;``'. If more than one keyword is given, the string should be enclosed in double-quotes (").

The '``-``' (dash), and the '``*``' asterisk), are used in page range selections, and cannot be keyword separators.

Keywords are case independent, thus ``begin``, ``BEGIN``, and ``Begin`` are all equivalent.

``begin``
   Begins accumulating pages to a list. The clist data for each page is saved in files or memory as discussed above, but no rendering is performed.
   If the device was set for rendering into memory, is will be switched to accumulating the page as clist files, thus ``-dMaxBitmap`` and ``-dBandingNever`` will be ignored, as if ``-dBandingAlways`` was specified.

``flush``
   Delete all clist files for all pages on the list, and reset the list to empty, but remain in ``saved-pages`` mode. Subsequent pages will be added to the list.

``end``
   Flush any list, deleting all clist files for any accumulated pages and stop accumulating pages, returning to rendering/output as soon as the page is complete.

   The device may render subsequent pages in memory (page buffer) as determined by ``-dMaxBitmap`` and ``-dBandingNever``.


``copies`` *copy_count*
   Set the copy count for the print action. The requested number of copies will be generated for all subsequent print actions, thus producing collated copies, rather than multiple copies of each page as in normal (non-deferred) rendering and output.

   A ``--saved-pages="copies #"`` option will be applied to all print actions until a new copies # is encountered that sets the copy count to a different value. Since the copies can follow the print command, it is recommended that only a single copies be in a each ``--saved-pages=...`` string.

   .. note::

      Unless ``-d.IgnoreNumCopies=true`` is specified prior to starting ``saved-pages`` mode, the copy count for each page will be saved with each page, and the job will contain the specified number of copies of each page, as well as producing the number of collated copies of the job set by the copies in a ``--saved-pages=...`` string.

``print`` *print keywords*
   Print from the list of saved pages. The print keywords following the print action keyword determine what is printed, and in what order. The keywords are described in the next section.


Printing Saved Pages
~~~~~~~~~~~~~~~~~~~~~~~~

The string following the print action keyword determines which pages from the list are printed, in what order. Any sequence of keywords, page numbers or page ranges can follow the print keyword. The first control keyword (other than copies) ends the list of printing keywords.

As with control keywords, print keywords can be any mixture of upper or lower case, and print keyword, page numbers and page ranges can be seperated by any non-alphanumeric character other than '``-``' or '``*``'.

The following are all equivalent:


.. code-block::

   1 3 5 - *
   1, 3, 5-*
   1,3,5-*


``copies`` *copy_count*
   As mentioned above, the copies keyword and count can follow the print keyword. See the description of the copies keyword in the previous section.
   The copy count will be applied to the entire sequence of print keywords, so it can appear anywhere in the sequence of page number, page ranges or print keywords.

``normal``
   Print all of the pages, from the first until the last page in the list. This keyword is used to print collated copies in the usual page order. This is the same as the page range: "``1 - *``"

``reverse``
   Print all of the pages, from the last backwards to the first page in the list. This keyword is used to print collated copies in the reverse page order. This is the same as the page range: "``* - 1``"

``odd``
   Print all odd number pages, from 1 to the last odd numbered page. This keyword, combined with the even keyword and user interaction to place pages that were printed into the input tray, can be used for manual duplexing.

``even``
   Print all even number pages, from 2 to the last even numbered page, then if the list has an odd number of pages print a blank 'pad' page to be the back of the last odd page. This keyword, combined with the odd keyword and user interaction to place pages that were printed into the input tray, can be used for manual duplexing.

``even0pad``
   Print all even number pages, from 2 to the last even numbered page, without any blank 'pad' page as with the even keyword.

*page_number*
   A number not followed by '``-``' specifies a single page from the list to be printed. The '``*``' character represents the last page in the list, interpreted as a number, either as a single *page_number* or as the *start_page* or *end_page* in a page range.

*start_page - end_page*
   A range of pages is two *page_numbers* or ``*`` separated by a '``-``', optionally surrounded by non-alphanumeric characters other than '``-``' which is a special character in page range specifications. The pages from the *start_page* to the *end_page* are printed.

   If the *start_page* is after the *end_page*, then the order of the pages is reversed, from the *start_page* back to the *end_page*.


Examples
----------------

The following examples each print 2 collated copies from the first page to the last page:


.. code-block:: bash

   --saved-pages="print: normal, copies=2"
   --saved-pages="print copies=2, normal"
   --saved-pages="copies 2 print normal"
   --saved-pages="print 1-* 1-*"
   --saved-pages="print normal normal"


Print the last page, followed by all pages in reverse order, then the first page (print reverse with two copies of the last and first page):

.. code-block:: bash

   --saved-pages="print * reverse 1"

or:

.. code-block:: bash

   --saved-pages="print * * - 1 1"


Using the command line arguments with a PostScript or PDF input file to print the odd pages, prompt the operator to reload the printer with the pages that were printed, then print the even pages:


.. code-block:: bash

   gs -sDEVICE=pgmraw -o /dev/lp0 --saved-pages="begin" examples/annots.pdf \
   --saved-pages="print odd" \
   -c "(Move printed pages to the input tray, then press enter.) = flush \
   (%stdin) (r) file 256 string readline clear" \
   --saved-pages="print even"


Print two collated copies of the first file, followed by 5 copies of the second file:


.. code-block:: bash

   gs -sDEVICE=pgmraw -o /dev/lp0 --saved-pages="begin" examples/annots.pdf \
   --saved-pages="copies=2 print normal flush" \
   examples/colorcir.ps \
   --saved-pages="copies=5 print normal"








.. include:: footer.rst