.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: Ghostscript Enterprise

.. include:: header.rst

.. _Ghostscript_Enterprise:


Ghostscript Enterprise
===================================

:title:`Ghostscript Enterprise` is a commercial version of :title:`GhostPDL` which can also read and process a range of common office documents, including :title:`Word`, :title:`PowerPoint` and :title:`Excel`.


File Support
---------------

.. raw:: html

   <style>
      .flexbox {
        display:flex;
        justify-content: flex-start;
        align-items: center;
        min-height: 80px;
      }

      .flexbox div {
          min-width: 70px;
      }

      .flexbox .icon {
          width:50px;
          height:50px;
      }
   </style>

   <table class="docutils" width=100%>
       <thead>
       <tr>
           <th>File type</th>
           <th>File extension</th>
       </tr>
       </thead>
       <tbody>
           <tr>
               <td>
                   <div class="flexbox">
                       <div><img class="icon borderless" src="_images/icon-docx.svg" alt="doc icon" /></div>
                       <div><strong>Word</strong></div>
                   </div>
               </td>
               <td><div class="flexbox"><code>docx</code>&nbsp;
               <code>doc</code>&nbsp;
               <code>dotx</code>&nbsp;
               <code>docm</code>&nbsp;
               <code>dotm</code></div></td>
           </tr>

           <tr>
               <td>
                   <div class="flexbox">
                       <div><img class="icon borderless" src="_images/icon-odt.svg" alt="doc icon" /></div>
                       <div><strong>Open Office Word</strong></div>
                   </div>
               </td>
               <td><div class="flexbox"><code>odt</code></div></td>
           </tr>

           <tr>
               <td>
                   <div class="flexbox">
                       <div><img class="icon borderless" src="_images/icon-xlsx.svg" alt="doc icon" /></div>
                       <div><strong>Excel</strong></div>
                   </div>
               </td>
               <td><div class="flexbox"><code>xlsx</code>&nbsp;
               <code>xls</code>&nbsp;
               <code>xlt</code>&nbsp;
               <code>xlsm</code>&nbsp;
               <code>xltm</code></div></td>
           </tr>

           <tr>
               <td>
                   <div class="flexbox">
                       <div><img class="icon borderless" src="_images/icon-pptx.svg" alt="doc icon" /></div>
                       <div><strong>PowerPoint</strong></div>
                   </div>
               </td>
               <td><div class="flexbox"><code>pptx</code>&nbsp;
               <code>ppt</code>&nbsp;
               <code>pps</code>&nbsp;
               <code>pptm</code>&nbsp;
               </div></td>
           </tr>

           <tr>
               <td>
                   <div class="flexbox">
                       <div><img class="icon borderless" src="_images/icon-txt.svg" alt="doc icon" /></div>
                       <div><strong>Text</strong></div>
                   </div>
               </td>
               <td><div class="flexbox"><code>txt</code>&nbsp;
               <code>csv</code>&nbsp;
               </div></td>
           </tr>

      </tbody>
   </table>



Licensing
------------------------------------------------------

If you are interested in using :title:`Ghostscript Enterprise`, please contact our sales team to discuss a commercial license. Each :title:`Artifex` commercial license is crafted based on your individual use case.

.. raw:: html

   <button class="cta orange" onclick="window.location='https://artifex.com/contact/ghostscript-inquiry.php?utm_source=rtd-ghostscript&utm_medium=rtd&utm_content=cta-button'">CONTACT US</button>
   <p></p>


----


Using
------------------------------------

Once you have acquired a commercial license agreement you will be supplied with a built binary (``gse``) for your system to run :title:`Ghostscript Enterprise`.

- Use :title:`Ghostscript Enterprise` in the regular way in which you would use :title:`Ghostscript` itself.
- Just declare your :ref:`device<Devices.html>`, your output & input files and then any other optional parameters as required.
- Ensure to run your commands against your ``gse`` binary as opposed to any other installation of :title:`Ghostscript`.


Interpreter flags
~~~~~~~~~~~~~~~~~~~~~~~~


:title:`Ghostscript Enterprise` specific interpreter flags are as follows:

.. list-table::
      :widths: 50 50
      :header-rows: 1

      * - Flag name
        - Description
      * - ``-sSOLOCALE``
        - The locale to set for the document.
      * - ``-sSOPASSWORD``
        - The password to set to open the file.


Examples
~~~~~~~~~~~~~~~

Create a :title:`PDF` file from a :title:`Word` file
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

A simple use case to create a new :title:`PDF` document from a :title:`Word` document.

.. code-block:: bash

   ./gse -sDEVICE=pdfwrite -o output.pdf input.docx


Convert a :title:`Word` document into a series of image files
"""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

This snippet outputs files in :title:`PNG` image format, one per page at a 300 DPI resolution.

.. code-block:: bash

   ./gse -sDEVICE=png16m -o output%d.png -r300 input.docx


Extracting text
""""""""""""""""""""""""""""""""""""""""""""""

A simple use-case of extracting text from a :title:`PowerPoint` file.

.. code-block:: bash

   ./gse -sDEVICE=txtwrite -o output.txt input.pptx


Handle a password protected document
""""""""""""""""""""""""""""""""""""""""

Here we send through a password to ensure we can open a password-protected file.

.. code-block:: bash

   ./gse -sDEVICE=txtwrite -o output.txt -sSOPASSWORD=clyde30 password-is-clyde30.docx


Handle a document with a specific locale
"""""""""""""""""""""""""""""""""""""""""""""""""""""""

In this snippet, the document locale should be set to Korean, so text extraction knows to use Korean versions of characters that are shared with other languages.

.. code-block:: bash

   ./gse -sDEVICE=txtwrite -o output.txt -sSOLOCALE=ko-kr input.xls





.. External links




.. include:: footer.rst

