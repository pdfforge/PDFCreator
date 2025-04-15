.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: The GhostPDL Interpreter Framework


.. include:: header.rst

.. _GPDL.html:


The GhostPDL Interpreter Framework
=====================================================================





What is the GhostPDL Interpreter Framework?
---------------------------------------------

The GhostPDL interpreter framework (henceforth, just GhostPDL) is a framework into which multiple interpreters can be fitted, together with a set of interpreters for different input "languages".

The intent is that a build of GPDL will behave as much as possible like a build of Ghostscript, but should be able to transparently cope with as many different input formats as it has interpreters built into it.

It can be built as a dynamic link library (DLL) on Microsoft Windows, as a shared object on the Linux, Unix and MacOS X platforms. With some changes, it could be built as a static library.

The reason we dub it a "Framework" is that it is designed to be modular, and to readily allow new interpreters to be swapped in and out to extend and customise its capabilities.

Jobs must be separated by UEL (Universal End of Language) code sequences (or PJL fragments, which implicitly start with a ``UEL`` marker).



The API
---------------------------------------------

The API for GPDL (found in ``plapi.h``) is deliberately designed to be nearly identical to that for Ghostscript itself (found in ``iapi.h``). Details of Ghostscript's API can be found :ref:`here<API.html>`, and we refer you to that. Only differences from that API will be documented here. In particular the differences in the handling of switches within the ``gsapi_init_with_args`` are documented here.

Our intent is that existing users of the Ghostscript API should be able to drop a GPDL DLL in as a replacement with little to no code changes.



The run_string APIs
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The ``run_string`` APIs have parameters for int ``user_errors`` and int ``*error_code``. Within Ghostscript, these allow a caller to bypass the language's standard error handling, and to trap errors externally. This makes no sense within a printer implementation (and, in particular, no sense for a postscript interpreter running jobs within a JobServer loop as GPDL does). Thus these parameters are kept for ABI compatibility, but are largely ignored within the GPDL implementation of gsapi. For sanity, pass 0 for ``user_errors``, and expect ``*error_code`` to be set to 0 on exit.

String vs File functions
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Some file types, such as Postscript and PCL, are designed to be 'streamable'; that is to say that the files can be fed in and consumed as they are interpreted. Other file types, such as PDF or XPS require the whole file to be available so that the interpreters can seek back and forth within it to extract the data they require.

Traditionally, Ghostscript has relied upon the caller knowing which type of data is which; streamable data can be fed in to the system using the ``gsapi_run_string`` APIs, and complete files of data can be fed in using ``gsapi_run_file`` or ``gsapi_init_with_args`` APIs. Streamable data contained with a file is simple to deal with, as it's trivial for an interpreter to read bytes from a file, but doing random access on a stream is much harder.

In systems where data is being streamed in, but it is required to be available for random access, the entire stream must be buffered for presentation to the language interpreters. With Ghostscript, the responsibility for doing this fell to the caller. Unfortunately, this means that the caller also has to be responsible for scanning the incoming data to spot when it is in a format that requires buffering. With the explosion of formats that GPDL supports this quickly becomes unpalatable.

While the caller is still free to do such buffering of data itself, GPDL will do it automatically if required. If a language spots that data being fed to it via the ``gsapi_run_string`` APIs is in a format that requires buffering, the entire string of data will be automatically collected together, and then be represented internally to the ``gsapi_run_file`` API.

The current implementation buffers the data in memory. Alternative implementations could use a backing store if such a thing were available. For server based applications the use of memory is not likely to be a problem (assuming reasonably sized input files at least). For printer integrations, it's entirely possible that no backing store will be available, so RAM may be the only option. Integrators may wish to place a limit on the size of files that can be buffered in this way.



The GPDL executable
------------------------------------

The GPDL executable is a small executable that loads the DLL/shared object, and implements, pretty much, the interface described in the :ref:`usage documentation<Use.html>` for Ghostscript, but capable of reading any of the configured languages. This executable provides all the interaction with the windowing system, including image windows.

Areas where the GPDL executable differs from the Ghostscript executable are described :ref:`below<Differences in switches from Ghostscript>`. These are primarily to do with (slightly) esoteric requirements when running Postscript in an interactive environment. Whereas Ghostscript is a generic Postscript interpreter, GPDL is unashamedly targeted as a print processor.

The GPDL framework's library name and characteristics differ for each platform:

- The Win32 DLL ``gpdldll32.dll`` can be used by multiple programs simultaneously, but only once within each process.
- The Linux shared object ``libgs.so`` can be used by multiple programs simultaneously.

The source for the executable is in ``plw*.*`` (Windows) and ``plx*.*`` (Linux/Unix). See these source files for examples of how to use the DLL.

At this stage, GhostPDL does not support multiple instances of the interpreter within a single process.



Differences in switches from Ghostscript
------------------------------------------------------------------------

The ``gsapi_init_with_args`` API entrypoint takes a set of arguments, which can include various switches. We document the differences between Ghostscript's handling of switches and GhostPDL's here. The GhostPDL executable directly maps parameters given on the command-line into this list, so this list of differences applies both to API calls and uses of the executable.

GhostPDL does not support the ``-_``, ``-+``, ``-@``, ``-B``, ``-F``, ``-M``, ``-N``, ``-P``, ``-u``, and ``-X`` switches that Ghostscript does.

GhostPDL supports a few switches that the Ghostscript executable does not.

- ``-E #``: Sets the "error reporting mode" for PCL/PXL.
- ``-H #x#x#x#``: Sets the hardware margins to the given left/bottom/right/top values (in points).
- ``-j <string>``: Passes the string to the PJL level. The string is split on '``;``'.
- ``-J``: Same as ``-j``.
- ``-l {RTL,PCL5E,PCL5C}``: Sets the "personality" of the PCL/PXL interpreter.
- ``-L <language>``: Sets the language to be used. Run with -L and no string to see a list of languages supported in your build.
- ``-m #x#``: Sets the margin values to the left/bottom values (in points).


Supported languages
------------------------------------------------------------------------

The following is a (possibly non-exhaustive) list of languages for which implementations exist for GhostPDL. We use the term "language" to include both full page description languages (such as PCL, Postscript and XPS), and handlers for simpler formats (such as JPEG, PNG and TIFF).

It is inherent in the design that new handlers can be included, and unwanted formats can be dropped from any given build/integration.

PJL
~~~~~~~~~~~~~~

PJL is special, in that it is the central language implementation to which we return between all jobs. As such it always appears first in the list, and must be present in all builds.

PCL
~~~~~~~~~~~~~~

The PCL language implementation supports PCL5C/PXL5E and HPGL/2 with RTL.

PCLXL
~~~~~~~~~~~~~~

The PCLXL language implementation supports PCLXL.

XPS
~~~~~~~~~~~~~~

The XPS language implementation supports the XML Paper Specification format, as used in modern Windows systems printing pipelines.

POSTSCRIPT
~~~~~~~~~~~~~~

The POSTSCRIPT language implementation is the Ghostscript Postscript interpreter, as described in the accompanying documentation.

URF
~~~~~~~~~~~~~~

Currently available to commercial customers only, the URF language implementation implements support for the URF file format, as used by Apple Airprint.

JPG
~~~~~~~~~~~~~~

The JPG language implementation supports JPEG files (in both JFIF and EXIF formats).

PWG
~~~~~~~~~~~~~~

The PWG language implementation supports PWG (Printer Working Group) format files.

TIFF
~~~~~~~~~~~~~~

The TIFF language implementation supports TIFF format files in a variety of compression formats, depending on the exact configuration of libtiff used in the build.

JBIG2
~~~~~~~~~~~~~~

The JBIG2 language implementation supports JBIG2 format images.

JP2K
~~~~~~~~~~~~~~

The JP2K language implementation supports JPEG2000 and JPX format images.

PNG
~~~~~~~~~~~~~~

The PNG language implementation supports PNG format images.



Adding a new language
--------------------------------

Each language implementation in the system appears as an instance of a ``pl_interp_implementation_t`` structure.


.. code-block:: c

   typedef struct pl_interp_implementation_s
   {
       /* Procedure vector */
       pl_interp_proc_characteristics_t proc_characteristics;
       pl_interp_proc_allocate_interp_instance_t proc_allocate_interp_instance;
       pl_interp_proc_get_device_memory_t proc_get_device_memory;
       pl_interp_proc_set_param_t proc_set_param;
       pl_interp_proc_add_path_t proc_add_path;
       pl_interp_proc_post_args_init_t proc_post_args_init;
       pl_interp_proc_init_job_t proc_init_job;
       pl_interp_proc_run_prefix_commands_t proc_run_prefix_commands;
       pl_interp_proc_process_file_t proc_process_file;
       pl_interp_proc_process_begin_t proc_process_begin;
       pl_interp_proc_process_t proc_process;
       pl_interp_proc_process_end_t proc_process_end;
       pl_interp_proc_flush_to_eoj_t proc_flush_to_eoj;
       pl_interp_proc_process_eof_t proc_process_eof;
       pl_interp_proc_report_errors_t proc_report_errors;
       pl_interp_proc_dnit_job_t proc_dnit_job;
       pl_interp_proc_deallocate_interp_instance_t
           proc_deallocate_interp_instance;
       void *interp_client_data;
   } pl_interp_implementation_t;

This structure consists of series of function pointers, each of which performs some portion of the processing required to handle detection of language type and processing of data. These function pointers are described in detail below.

In addition, the ``interp_client_data`` field is used to hold the running state of a given interpreter.

All the languages to be supported in the build are listed in the ``pdl_implementations`` array in ``plimpl.c``. To add a new implementation the name of the appropriate ``pl_interp_implementation_t`` should be added here.

The existing range of language implementations may prove useful as references when implementing new ones. They can be found as ``gpdl/*top.c``. In particular, ``pngtop.c`` is a simple implementation of a well-defined, relatively simple file format (PNG), based upon a well-known and well-documented library (``libpng``), so is probably a good place to start.


proc_characteristics
~~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: c

   typedef const pl_interp_characteristics_t
       *(*pl_interp_proc_characteristics_t) (const pl_interp_implementation_t *);


This entrypoint is called to request details of the characteristics of the language. This must be implemented in all instances.

This returns a pointer to a ``pl_interp_characteristics_t`` structure:


.. code-block:: c

   typedef struct pl_interp_characteristics_s
   {
       const char *language;       /* generic language should correspond with
                                      HP documented PJL name */
       int (*auto_sense)(const char *string, int length);      /* routine used to detect language - returns a score: 0 is definitely not, 100 is definitely yes. */
       const char *manufacturer;   /* manuf str */
       const char *version;        /* version str */
       const char *build_date;     /* build date str */
   } pl_interp_characteristics_t;


The language entry contains a simple NULL terminated string that names the interpreter. Similarly, the manufacturer, version, and ``build_date`` fields are for informational purposes only.

The ``auto_sense`` function is called with a prefix of data from each new source. Each language is passed the data in turn, and "scores;" according to how sure it is the file is that format.

For many file formats this means looking for known in the first few bytes (such as PNG or TIFF looking for their signature bytes). For others, such as XPS, the best that can be done is to spot that it's a zip file. For still others, such as PCL, heuristics have to be used.

A 'definite' match is returned as 100, a definite rejection as 0, with intermediate values used appropriately.


proc_allocate_interp_instance
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: c

   typedef int (*pl_interp_proc_allocate_interp_instance_t)
                                                   (pl_interp_implementation_t *,
                                                    gs_memory_t *);

On startup, the GPDL library calls around all the languages via this function, getting them to allocate themselves an instance. What this means will vary between languages, but typically it involves allocating a state structure, and storing the pointer to that in the ``interp_client_data`` pointer of the ``pl_interp_implementation_t *``. Part of this state structure will typically be a ``gstate`` to use to drive the graphics library.

proc_get_device_memory
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: c

   typedef gs_memory_t *(*pl_interp_proc_get_device_memory_t)
                                                  (pl_interp_implementation_t *);


On shutdown, the GPDL library calls around all the languages via this function, getting them to release their resources and deallocate any memory.


proc_set_param
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: c

   typedef int (*pl_interp_proc_set_param_t) (pl_interp_implementation_t *,
                                              pl_set_param_type,
                                              const char *,
                                              const void *);

Some languages (particularly Postscript) can have their behaviour changed by the use of parameters. This function provides a generic method for the GPDL library to pass parameters into a language. Each language is free to ignore the parameters that do not apply to it. For most languages, this can safely be passed as NULL.

proc_add_path
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: c

   typedef int (*pl_interp_proc_add_path_t) (pl_interp_implementation_t *,
                                             const char *);

Some languages (particularly Postscript) have the ability to open files from the local storage. These files can be found in a variety of different locations within the local storage. As such this call allows the GPDL library to add paths to the current list of locations that will be searched. For most languages, this can safely be passed as NULL.


proc_post_args_init
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: c

   typedef int (*pl_interp_proc_post_args_init_t) (pl_interp_implementation_t *);



proc_init_job
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: c

   typedef int (*pl_interp_proc_init_job_t) (pl_interp_implementation_t *,
                                             gx_device *);

Once the GPDL library has identified which language should be used for an incoming job, it will call this entrypoint to initialise the language for the job. What this means will vary between languages, but at the very minimum the job will need to take note of the device to be used.

proc_run_prefix_commands
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: c

   typedef int (*pl_interp_proc_run_prefix_commands_t)
                                                  (pl_interp_implementation_t *,
                                                   const char *prefix);

The GPDL library (and executable) allow language commands to be sent in the argument parameters using the ``-c`` switch. These are collected into a buffer, and forwarded to a language to be run as part of the same job as any following file.

Currently, only the Postscript language handler implements this function, all others should pass NULL.


proc_process_file
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: c

   typedef int (*pl_interp_proc_process_file_t) (pl_interp_implementation_t *,
                                                 const char *);

If the GPDL library is given a filename to process, and this function is non-NULL, it will call this to run the file. For file formats such as PDF (which need to be buffered so they can be read out of order), this can avoid the need to feed in all the data via ``proc_process``, buffer it somewhere, and then process it at the end.

For many languages this can be NULL.

proc_process_begin
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: c

   typedef int (*pl_interp_proc_process_begin_t) (pl_interp_implementation_t *);

Once the GPDL library has data to process (that it cannot process with ``proc_process_file``, it will call this function to setup the transfer of data.


proc_process
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: c

   typedef int (*pl_interp_proc_process_t) (pl_interp_implementation_t *,
                                            stream_cursor_read *);

After the GPDL library has called ``proc_process_begin`` this function may be called multiple times to actually transfer the data in. The implementation is expected to consume as many bytes as it can (but not necessarily all of them) before returning with an updated read pointer. If this function cannot advance without more data, it should return with ``gs_error_NeedInput``.

proc_process_end
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: c

   typedef int (*pl_interp_proc_process_end_t) (pl_interp_implementation_t *);

After the GPDL library has called ``proc_process_begin`` (and possibly made a number of calls to ``proc_process``) it will call ``proc_process_end`` to signify the end of the data. This does not necessarily signal the end of the job.

proc_flush_to_eoj
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: c

   typedef int (*pl_interp_proc_flush_to_eoj_t) (pl_interp_implementation_t *,
                                                 stream_cursor_read *);

In the event of a problem while processing data, GPDL may seek to abandon processing of a transfer in progress by calling ``proc_flush_to_eoj``. If possible, the language should continue to process data until a reasonable stopping point, or until UEL is reached.


proc_process_eof
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: c

   typedef int (*pl_interp_proc_process_eof_t) (pl_interp_implementation_t *);


Called when GPDL reaches EOF in processing a job. A language implementation should assume no more data is forthcoming.

proc_report_errors
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: c

   typedef int (*pl_interp_proc_report_errors_t) (pl_interp_implementation_t *,
                                                  int,
                                                  long,
                                                  bool);

Called after running a job to give the language implementation the chance to report any errors it may have detected as it ran.

proc_dnit_job
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: c

   typedef int (*pl_interp_proc_dnit_job_t) (pl_interp_implementation_t *);

Called after a job is complete so that the language implementation may clean up. The interpreter is kept open so that more jobs can be fed to it, but no state should be kept from this job to the next.


proc_deallocate_interp_instance
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: c

   typedef int (*pl_interp_proc_deallocate_interp_instance_t)
                                                 (pl_interp_implementation_t *);

Called on shutdown of the GPDL library to close down the language instance and free all the resources.





















.. include:: footer.rst
