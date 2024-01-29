.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: API

.. include:: header.rst

.. _API.html:


API
=================================






What is the Ghostscript Interpreter API?
------------------------------------------


The Ghostscript interpreter can be built as a dynamic link library (DLL) on Microsoft Windows, as a shared object on the Linux, Unix and MacOS X platforms. With some changes, it could be built as a static library. This document describes the Application Programming Interface (API) for the Ghostscript interpreter library. This should not be confused with the :ref:`Ghostscript library<Lib.html>` which provides a graphics library but not the interpreter.

This supercedes the old DLL interface.

To provide the interface described in the :ref:`usage documentation<Use.html>`, a smaller independent executable loads the DLL/shared object. This executable must provide all the interaction with the windowing system, including image windows and, if necessary, a text window.

The Ghostscript interpreter library's name and characteristics differ for each platform:

- The Win32 DLL ``gsdll32.dll`` can be used by multiple programs simultaneously, but only once within each process.
- The OS/2 DLL ``gsdll2.dll`` has MULTIPLE NONSHARED data segments and can be called by multiple programs simultaneously.
- The Linux shared object ``libgs.so`` can be used by multiple programs simultaneously.

The source for the executable is in ``dw*.*`` (Windows), ``dp*.*`` (OS/2) and ``dx*.*`` (Linux/Unix). See these source files for examples of how to use the DLL.

The source file dxmainc.c can also serve as an example of how to use the shared library component on MacOS X, providing the same command-line tool it does on any linux, bsd or similar operating system.

At this stage, Ghostscript does not support multiple instances of the interpreter within a single process.


.. _API.html exported functions:
.. _gsapi_asterisk:

Exported functions
------------------------------------------

The functions exported by the DLL/shared object are described in the header file ``iapi.h`` and are summarised below. Omitted from the summary are the calling convention (e.g. ``__stdcall``), details of return values and error handling.


.. role:: c(code)
   :language: c


- :c:`int gsapi_revision (gsapi_revision_t *pr, int len);` :ref:`details<gsapi_revision>`
- :c:`int gsapi_new_instance (void **pinstance, void *caller_handle);` :ref:`details<gsapi_new_instance>`
- :c:`void gsapi_delete_instance (void *instance);` :ref:`details<gsapi_delete_instance>`
- :c:`int gsapi_set_stdio_with_handle (void *instance, int(*stdin_fn)(void *caller_handle, char *buf, int len), int(*stdout_fn)(void *caller_handle, const char *str, int len), int(*stderr_fn)(void *caller_handle, const char *str, int len), void *caller_handle);` :ref:`details<gsapi_set_stdio_with_handle>`
- :c:`int gsapi_set_stdio (void *instance, int(*stdin_fn)(void *caller_handle, char *buf, int len), int(*stdout_fn)(void *caller_handle, const char *str, int len), int(*stderr_fn)(void *caller_handle, const char *str, int len));` :ref:`details<gsapi_set_stdio>`
- :c:`int gsapi_set_poll_with_handle (void *instance, int(*poll_fn)(void *caller_handle), void *caller_handle);` :ref:`details<gsapi_set_poll_with_handle>`
- :c:`int gsapi_set_poll (void *instance, int(*poll_fn)(void *caller_handle));` :ref:`details<gsapi_set_poll>`
- :c:`int gsapi_set_display_callback (void *instance, display_callback *callback);` :ref:`details<gsapi_set_display_callback>`
- :c:`int gsapi_register_callout (void *instance, gs_callout callout, void *callout_handle);` :ref:`details<gsapi_register_callout>`
- :c:`void gsapi_deregister_callout (void *instance, gs_callout callout, void *callout_handle);` :ref:`details<gsapi_deregister_callout>`
- :c:`int gsapi_set_arg_encoding (void *instance, int encoding);` :ref:`details<gsapi_set_arg_encoding>`
- :c:`int gsapi_get_default_device_list(void *instance, char **list, int *listlen);` :ref:`details<gsapi_get_default_device_list>`
- :c:`int gsapi_set_default_device_list(void *instance, const char *list, int listlen);` :ref:`details<gsapi_set_default_device_list>`
- :c:`int gsapi_run_string_begin (void *instance, int user_errors, int *pexit_code);` :ref:`details<gsapi_run_asterisk>`
- :c:`int gsapi_run_string_continue (void *instance, const char *str, unsigned int length, int user_errors, int *pexit_code);` :ref:`details<gsapi_run_asterisk>`
- :c:`int gsapi_run_string_end (void *instance, int user_errors, int *pexit_code);` :ref:`details<gsapi_run_asterisk>`
- :c:`int gsapi_run_string_with_length (void *instance, const char *str, unsigned int length, int user_errors, int *pexit_code);` :ref:`details<gsapi_run_asterisk>`
- :c:`int gsapi_run_string (void *instance, const char *str, int user_errors, int *pexit_code);` :ref:`details<gsapi_run_asterisk>`
- :c:`int gsapi_run_file (void *instance, const char *file_name, int user_errors, int *pexit_code);` :ref:`details<gsapi_run_asterisk>`
- :c:`int gsapi_init_with_args (void *instance, int argc, char **argv);` :ref:`details<gsapi_init_with_args>`
- :c:`int gsapi_exit (void *instance);` :ref:`details<gsapi_exit>`
- :c:`int gsapi_set_param(void *instance, const char *param, const void *value, gs_set_param_type type);` :ref:`details<gsapi_set_param>`
- :c:`int gsapi_get_param(void *instance, const char *param, void *value, gs_set_param_type type);` :ref:`details<gsapi_get_param>`
- :c:`int gsapi_enumerate_params(void *instance, void **iter, const char **key, gs_set_param_type *type);` :ref:`details<gsapi_enumerate_params>`
- :c:`int gsapi_add_control_path(void *instance, int type, const char *path);` :ref:`details<gsapi_add_control_path>`
- :c:`int gsapi_remove_control_path(void *instance, int type, const char *path);` :ref:`details<gsapi_remove_control_path>`
- :c:`void gsapi_purge_control_paths(void *instance, int type);` :ref:`details<gsapi_purge_control_paths>`
- :c:`void gsapi_activate_path_control(void *instance, int enable);` :ref:`details<gsapi_activate_path_control>`
- :c:`int gsapi_is_path_control_active(void *instance);` :ref:`details<gsapi_is_path_control_active>`
- :c:`int gsapi_add_fs (void *instance, gsapi_fs_t *fs, void *secret);` :ref:`details<gsapi_add_fs>`
- :c:`void gsapi_remove_fs (void *instance, gsapi_fs_t *fs, void *secret);` :ref:`details<gsapi_remove_fs>`




.. _API.html gsapi_revision:
.. _gsapi_revision:


gsapi_revision()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

This function returns the revision numbers and strings of the Ghostscript interpreter library; you should call it before any other interpreter library functions to make sure that the correct version of the Ghostscript interpreter has been loaded.


.. code-block:: c

   typedef struct gsapi_revision_s {
       const char *product;
       const char *copyright;
       long revision;
       long revisiondate;
   } gsapi_revision_t;
   gsapi_revision_t r;

   if (gsapi_revision(&r, sizeof(r)) == 0) {
       if (r.revision < 650)
          printf("Need at least Ghostscript 6.50");
   }
   else {
       printf("revision structure size is incorrect");
   }


.. _API.html gsapi_new_instance:
.. _gsapi_new_instance:

gsapi_new_instance()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


Create a new instance of Ghostscript. This instance is passed to most other gsapi functions. The caller_handle is the default value that will be provided to callback functions. On some platforms (those that do not support threading), only one instance of Ghostscript is supported at a time; any attempt to create more than one at a time would result in ``gsapi_new_instance`` returning an error.

While the core Ghostscript devices are believed to be thread safe now, a handful of devices are known not to be (at least the :title:`x11` devices, :title:`uniprint`, and the open printing devices). A new mechanism has been implemented that allows devices to check for concurrent use and to refuse to start up. The devices shipped with Ghostscript known to use global variables have had these calls added to them. Any authors of non-standard Ghostscript devices that use global variables should consider adding the same calls to their own code.

The first parameter, is a pointer to an opaque pointer (``void **``). The opaque pointer (``void *``) must be initialised to NULL before the call to ``gsapi_new_instance()``. See `Example 1`_.

.. _API.html gsapi_delete_instance:
.. _gsapi_delete_instance:

gsapi_delete_instance()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Destroy an instance of Ghostscript. Before you call this, Ghostscript must have finished. If Ghostscript has been initialised, you must call ``gsapi_exit`` before ``gsapi_delete_instance``.

.. _API.html gsapi_set_stdio_with_handle:
.. _gsapi_set_stdio_with_handle:

gsapi_set_stdio_with_handle()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Set the callback functions for stdio, together with the handle to use in the callback functions. The stdin callback function should return the number of characters read, 0 for EOF, or -1 for error. The stdout and stderr callback functions should return the number of characters written.

NOTE: These callbacks do not affect output device I/O when using "%stdout" as the output file. In that case, device output will still be directed to the process "stdout" file descriptor, not to the stdio callback.

.. _API.html gsapi_set_stdio:
.. _gsapi_set_stdio:

gsapi_set_stdio()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Set the callback functions for stdio. The handle used in the callbacks will be taken from the value passed to `gsapi_new_instance`_. Otherwise the behaviour of this function matches `gsapi_set_stdio_with_handle`_.


.. _API.html gsapi_set_poll_with_handle:
.. _gsapi_set_poll_with_handle:

gsapi_set_poll_with_handle()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Set the callback function for polling, together with the handle to pass to the callback function. This function will only be called if the Ghostscript interpreter was compiled with CHECK_INTERRUPTS as described in gpcheck.h.

The polling function should return zero if all is well, and return negative if it wants Ghostscript to abort. This is often used for checking for a user cancel. This can also be used for handling window events or cooperative multitasking.

The polling function is called very frequently during interpretation and rendering so it must be fast. If the function is slow, then using a counter to return 0 immediately some number of times can be used to reduce the performance impact.


.. _API.html gsapi_set_poll:
.. _gsapi_set_poll:

gsapi_set_poll()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Set the callback function for polling. The handle passed to the callback function will be taken from the handle passed to `gsapi_new_instance`_. Otherwise the behaviour of this function matches `gsapi_set_poll_with_handle`_.


.. _API.html gsapi_set_display_callback:
.. _gsapi_set_display_callback:

gsapi_set_display_callback()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


This call is deprecated; please use `gsapi_register_callout`_ to register a `callout`_ handler for the `display`_ device in preference. Set the callback structure for the display device. The handle passed in the callback functions is taken from the ``DisplayHandle`` parameter (or ``NULL`` if there is no such parameter). If the `display`_ device is used, this must be called after ``gsapi_new_instance()`` and before ``gsapi_init_with_args()``. See ``gdevdsp.h`` for more details.


.. _API.html gsapi_register_callout:
.. _gsapi_register_callout:

gsapi_register_callout()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

This call registers a `callout`_ handler.



.. _API.html gsapi_deregister_callout:
.. _gsapi_deregister_callout:

gsapi_deregister_callout()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

This call deregisters a `callout`_ handler previously registered with `gsapi_register_callout`_. All three arguments must match exactly for the callout handler to be deregistered.


.. _API.html gsapi_set_arg_encoding:
.. _gsapi_set_arg_encoding:

gsapi_set_arg_encoding()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Set the encoding used for the interpretation of all subsequent args supplied via the gsapi interface on this instance. By default we expect args to be in encoding 0 (the 'local' encoding for this OS). On Windows this means "the currently selected codepage". On Linux this typically means utf8. This means that omitting to call this function will leave Ghostscript running exactly as it always has. Please note that use of the 'local' encoding is now deprecated and should be avoided in new code. This must be called after ``gsapi_new_instance()`` and before ``gsapi_init_with_args()``.


.. _API.html gsapi_set_default_device_list:
.. _gsapi_set_default_device_list:

gsapi_set_default_device_list()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Set the string containing the list of default device names, for example "display x11alpha x11 bbox". Allows the calling application to influence which device(s) gs will try, in order, in it's selection of the default device. This must be called after ``gsapi_new_instance()`` and before ``gsapi_init_with_args()``.


.. _API.html gsapi_get_default_device_list:
.. _gsapi_get_default_device_list:


gsapi_get_default_device_list()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Returns a pointer to the current default device string. This must be called after ``gsapi_new_instance()`` and before ``gsapi_init_with_args()``.

.. _API.html gsapi_init_with_args:
.. _gsapi_init_with_args:


gsapi_init_with_args()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Initialise the interpreter. This calls ``gs_main_init_with_args()`` in ``imainarg.c`` . See below for `return codes`_. The arguments are the same as the "C" main function: ``argv[0]`` is ignored and the user supplied arguments are ``argv[1]`` to ``argv[argc-1]``.


.. _API.html gsapi_run_asterisk:
.. _gsapi_run_asterisk:


gsapi_run_*()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The ``gsapi_run_*`` functions are like ``gs_main_run_*`` except that the error_object is omitted. If these functions return <= -100, either quit or a fatal error has occured. You must call ``gsapi_exit()`` next. The only exception is ``gsapi_run_string_continue()`` which will return ``gs_error_NeedInput`` if all is well. See below for `return codes`_.

The address passed in ``pexit_code`` will be used to return the exit code for the interpreter in case of a quit or fatal error. The ``user_errors`` argument is normally set to zero to indicate that errors should be handled through the normal mechanisms within the interpreted code. If set to a negative value, the functions will return an error code directly to the caller, bypassing the interpreted language. The interpreted language's error handler is bypassed, regardless of ``user_errors`` parameter, for the ``gs_error_interrupt`` generated when the polling callback returns a negative value. A positive ``user_errors`` is treated the same as zero.

There is a 64 KB length limit on any buffer submitted to a ``gsapi_run_*`` function for processing. If you have more than 65535 bytes of input then you must split it into smaller pieces and submit each in a separate ``gsapi_run_string_continue()`` call.


.. _API.html gsapi_exit:
.. _gsapi_exit:


gsapi_exit()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Exit the interpreter. This must be called on shutdown if ``gsapi_init_with_args()`` has been called, and just before ``gsapi_delete_instance()``.


.. _API.html gsapi_set_param:
.. _gsapi_set_param:


gsapi_set_param()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Set a parameter. Broadly, this is equivalent to setting a parameter using ``-d``, ``-s`` or ``-p`` on the command line. This call cannot be made during a ``run_string`` operation.
Parameters in this context are not the same as 'arguments' as processed by ``gsapi_init_with_args``, but often the same thing can be achieved. For example, with ``gsapi_init_with_args``, we can pass "-r200" to change the resolution. Broadly the same thing can be achieved by using ``gsapi_set_param`` to set a parsed value of "<</HWResolution [ 200.0 200.0 ]>>".

Note, that internally, when we set a parameter, we perform an ``initgraphics`` operation. This means that using ``set_param`` other than at the start of a page is likely to give unexpected results.

Further, note that attempting to set a parameter that the device does not recognise will be silently ignored, and that parameter will not be found in subsequent ``gsapi_get_param`` calls.

The ``type`` argument dictates the kind of object that ``value`` points to:


.. code-block:: c

   typedef enum {
       gs_spt_invalid = -1,
       gs_spt_null    = 0,   /* void * is NULL */
       gs_spt_bool    = 1,   /* void * is a pointer to an int (0 false,
                              * non-zero true). */
       gs_spt_int     = 2,   /* void * is a pointer to an int */
       gs_spt_float   = 3,   /* void * is a float * */
       gs_spt_name    = 4,   /* void * is a char * */
       gs_spt_string  = 5,   /* void * is a char * */
       gs_spt_long    = 6,   /* void * is a long * */
       gs_spt_i64     = 7,   /* void * is an int64_t * */
       gs_spt_size_t  = 8,   /* void * is a size_t * */
       gs_spt_parsed  = 9,   /* void * is a pointer to a char * to be parsed */

       /* Setting a typed param causes it to be instantly fed to to the
        * device. This can cause the device to reinitialise itself. Hence,
        * setting a sequence of typed params can cause the device to reset
        * itself several times. Accordingly, if you OR the type with
        * gs_spt_more_to_come, the param will held ready to be passed into
        * the device, and will only actually be sent when the next typed
        * param is set without this flag (or on device init). Not valid
        * for get_typed_param. */
       gs_spt_more_to_come = 1<<31
   } gs_set_param_type;


Combining a type value by ORRing it with the ``gs_spt_more_to_come`` flag will cause the set_param operation to be queued internally, but not actually be sent to the device. Thus a series of ``set_param`` operations can be queued, for example as below:


.. code-block:: c

  int code = gsapi_set_param(instance,
                             "HWResolution",
                             "[300 300]",
                             gs_spt_parsed | gs_spt_more_to_come);
  if (code >= 0) {
    int i = 1;
    code = gsapi_set_param(instance,
                           "FirstPage",
                           &i,
                           gs_spt_int | gs_spt_more_to_come);
  }
  if (code >= 0) {
    int i = 3;
    code = gsapi_set_param(instance,
                           "DownScaleFactor",
                           &i,
                           gs_spt_int);
  }


This enables a series of set operations to be performed 'atomically'. This can be useful for performance, in that any reconfigurations to the device (such as page size changes or memory reallocations) will only happen when all the parameters are sent, rather than potentially each time each one is sent.


.. _API.html gsapi_get_param:
.. _API_gsapi_get_param:
.. _gsapi_get_param:


gsapi_get_param()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Get a parameter. Retrieve the current value of a parameter.

If an error occurs, the return value is negative. Otherwise the return value is the number of bytes required for storage of the value. Call once with ``value = NULL`` to get the number of bytes required, then call again with ``value`` pointing to at least the required number of bytes where the value will be copied out. Note that the caller is required to know the type of value in order to get it. For all types other than ``string``, ``name``, and ``parsed`` knowing the type means you already know the size required.

This call retrieves parameters/values that have made it to the device. Thus, any values set using the ``gs_spt_more_to_come`` without a following call without that flag will not be retrieved. Similarly, attempting to get a parameter before ``gsapi_init_with_args`` has been called will not list any, even if ``gsapi_set_param`` has been used.

Attempting to read a parameter that is not set will return ``gs_error_undefined (-21)``. Note that calling ``gsapi_set_param`` followed by ``gsapi_get_param`` may not find the value, if the device did not recognise the key as being one of its configuration keys.


.. _API.html gsapi_enumerate_params:
.. _gsapi_enumerate_params:


gsapi_enumerate_params()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Enumerate the current parameters. Call repeatedly to list out the current parameters.

The first call should have ``*iter = NULL``. Subsequent calls should pass the same pointer in so the iterator can be updated. Negative return codes indicate error, 0 success, and 1 indicates that there are no more keys to read. On success, key will be updated to point to a null terminated string with the key name that is guaranteed to be valid until the next call to ``gsapi_enumerate_params``. If ``type`` is non ``NULL`` then ``*type`` will be updated to have the type of the parameter.

Note that only one enumeration can happen at a time. Starting a second enumeration will reset the first.

The enumeration only returns parameters/values that have made it to the device. Thus, any values set using the ``gs_spt_more_to_come`` without a following call without that flag will not be retrieved. Similarly, attempting to enumerate parameters before ``gsapi_init_with_args`` has been called will not list any, even if ``gsapi_set_param`` has been used.


.. _API.html gsapi_add_control_path:
.. _gsapi_add_control_path:

gsapi_add_control_path()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Add a (case sensitive) path to one of the lists of permitted paths for file access. See :ref:`dSAFER<Use Safer>` for more information about permitted paths.

.. _API.html gsapi_remove_control_path:
.. _gsapi_remove_control_path:

gsapi_remove_control_path()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Remove a (case sensitive) path from one of the lists of permitted paths for file access. See :ref:`dSAFER<Use Safer>` for more information about permitted paths.


.. _API.html gsapi_purge_control_paths:
.. _gsapi_purge_control_paths:

gsapi_purge_control_paths()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


Clear all the paths from one of the lists of permitted paths for file access. See :ref:`dSAFER<Use Safer>` for more information about permitted paths.


.. _API.html gsapi_activate_path_control:
.. _gsapi_activate_path_control:

gsapi_activate_path_control()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


Enable/Disable path control (i.e. whether paths are checked against permitted paths before access is granted). See :ref:`dSAFER<Use Safer>` for more information about permitted paths.


.. _API.html gsapi_is_path_control_active:
.. _gsapi_is_path_control_active:

gsapi_is_path_control_active()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


Query whether path control is activated or not. See :ref:`dSAFER<Use Safer>` for more information about permitted paths.


.. _API.html gsapi_add_fs:

gsapi_add_fs
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Adds a new 'Filing System' to the interpreter. This enables callers to implement their own filing systems. The system starts with just the conventional 'file' handlers installed, to allow access to the local filing system. Whenever files are to be opened from the interpreter, the file paths are offered around each registered filing system in turn (from most recently registered to oldest), until either an error is given, or the file is opened successfully.

Details of the ``gsapi_fs_t`` are given :ref:`below <gsapi_fs_t>`.


.. _API.html gsapi_remove_fs:

gsapi_remove_fs
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


Remove a previously registered 'Filing System' from the interpreter. Both the function pointers within the ``gs_fs_t`` and the secret value must match exactly.



.. _API.html gsapi_fs_t:

gsapi_fs_t
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


Each 'filing system' within ``gs`` is a structure of function pointers; each function pointer gives a handler from taking a different named resource (a file, a pipe, a printer, a scratch file etc) and attempts to open it.


.. code-block:: c

      typedef struct
      {
          int (*open_file)(const gs_memory_t *mem,
                                 void        *secret,
                           const char        *fname,
                           const char        *mode,
                                 gp_file    **file);
          int (*open_pipe)(const gs_memory_t *mem,
                                 void        *secret,
                           const char        *fname,
                                 char        *rfname, /* 4096 bytes */
                           const char        *mode,
                                 gp_file    **file);
          int (*open_scratch)(const gs_memory_t *mem,
                                    void        *secret,
                              const char        *prefix,
                                    char        *rfname, /* 4096 bytes */
                              const char        *mode,
                                    int          rm,
                                    gp_file    **file);
          int (*open_printer)(const gs_memory_t *mem,
                                    void        *secret,
                                    char        *fname, /* 4096 bytes */
                                    int          binary,
                                    gp_file    **file);
          int (*open_handle)(const gs_memory_t *mem,
                                   void        *secret,
                                   char        *fname, /* 4096 bytes */
                             const char        *mode,
                                   gp_file    **file);
      } gsapi_fs_t;


If the filename (always given in utf-8 format) is recognised as being one that the filing system handles (perhaps by the prefix used), then it should open the file, fill in the ``gp_file`` pointer and return 0.

If the filename is not-recognised as being one that the filing system handles, then returning 0 will cause the filename to be offered to other registered filing systems.

If an error is returned (perhaps ``gs_error_invalidfileaccess``), then no other filing system will be allowed to try to open the file. This provides a mechanism whereby a caller to gsapi can completely control access to all files accessed via ``gp_fopen`` at runtime.

Note, that while most file access within Ghostscript will be redirected via these functions, stdio will not; see the existing mechanisms within Ghostscript for intercepting/replacing this.

- The ``open_file`` function pointer will be called when something (most often a call to ``gp_fopen``) attempts to open a file.

- The ``open_pipe`` function pointer will be called when something (most often a call to ``gp_popen``) attempts to open a pipe. ``rfname`` points to a 4K buffer in which the actual name of the opened pipe should be returned.

- The ``open_scratch`` function pointer will be called when something (most often a call to ``gp_open_scratch_file`` or ``gp_open_scratch_file_rm``) attempts to open a temporary file. rfname points to a 4K buffer in which the actual name of the opened pipe should be returned. If ``rm`` is true, then the file should be set to delete itself when all handles to it are closed.

- The ``open_printer`` function pointer will be called when something (most often a call to ``gp_open_printer``) attempts to open a stream to a printer. If ``binary`` is true, then the stream should be opened as binary; most streams will be binary by default - this has historical meaning on OS/2.

- The ``open_handle`` function pointer will be called when something (most often a call via the postscript %handle% IO device) attempts to open a Windows handle. This entry point will never be called on non-Windows builds.

Any of these which are left as ``NULL`` will never be called; a filing system with all of the entries left as ``NULL`` is therefore pointless.

The most complex part of the implementation of these functions is the creation of a ``gp_file`` instance to return. There are some helper functions for this, best explained by example.

Let us consider a hypothetical filing system that encrypts data as it is written, and decrypts it as it is read back. As each file is read and written the encryption/decryption routines will need to use some state, carried between calls to the filing system. We therefore might define a new type 'derived' from ``gp_file`` as follows:


.. code-block:: c

      typedef struct
      {
         gp_file base;
         /* State private to the implementation of this file for encryption/decryption */
         /* For example: */
         int foo;
         char *bar;
      } gp_file_crypt;

An implementation of ``gs_fs_t`` for our 'crypt' filing system might then look like this:


.. code-block:: c

      gsapi_fs_t gs_fs_crypt =
      {
          crypt_open_file,
          NULL,            /* open_pipe */
          NULL,            /* open_scratch */
          NULL,            /* open_printer */
          NULL             /* open_handle */
      };


In the above definition, we define a single handler, to cope with the opening of our input/output files. If we wanted to encrypt/decrypt other files too (perhaps the temporary files we produce) we'd need to define additional handlers (such as ``open_scratch``).

Our handler might look as follows:


.. code-block:: c

      int crypt_open_file(const gs_memory_t  *mem,
                          void         *secret,
                    const char         *filename,
                    const char         *mode,
                          gp_file     **file)
      {
          gp_file_crypt crypt;

          /* Ignore any filename not starting with "crypt://" */
          if (strncmp(filename, "crypt://", 8) != 0)
              return 0;

          /* Allocate us an instance (and fill in the non-crypt-specific
           * internals) */
          crypt = (gp_file_crypt *)gp_file_alloc(mem, &crypt_ops, sizeof(*crypt), "gp_file_crypt");
          if (crypt == NULL)
              return gs_error_VMerror; /* Allocation failed */

          /* Setup the crypt-specific state */
          crypt->foo = 1;
          crypt->bar = gs_alloc_bytes(mem->non_gc_memory, 256, "bar");
          /* If allocations fail, we need to clean up before exiting */
          if (crypt->bar) {
              gp_file_dealloc(crypt);
         return gs_error_VMerror;
          }

          /* Return the new instance */
          *file = &crypt.base;
          return 0;
      }


The crucial part of this function is the definition of ``crypt_ops``, an instance of the ``gp_file_ops_t`` type; a table of function pointers that implement the actual operations required.

.. code-block:: c

      typedef struct {
          int          (*close)(gp_file *);
          int          (*getc)(gp_file *);
          int          (*putc)(gp_file *, int);
          int          (*read)(gp_file *, size_t size, unsigned int count, void *buf);
          int          (*write)(gp_file *, size_t size, unsigned int count, const void *buf);
          int          (*seek)(gp_file *, gs_offset_t offset, int whence);
          gs_offset_t  (*tell)(gp_file *);
          int          (*eof)(gp_file *);
          gp_file     *(*dup)(gp_file *, const char *mode);
          int          (*seekable)(gp_file *);
          int          (*pread)(gp_file *, size_t count, gs_offset_t offset, void *buf);
          int          (*pwrite)(gp_file *, size_t count, gs_offset_t offset, const void *buf);
          int          (*is_char_buffered)(gp_file *file);
          void         (*fflush)(gp_file *file);
          int          (*ferror)(gp_file *file);
          FILE        *(*get_file)(gp_file *file);
          void         (*clearerr)(gp_file *file);
          gp_file     *(*reopen)(gp_file *f, const char *fname, const char *mode);
      } gp_file_ops_t;


These functions generally follow the same patterns as the posix functions that match them, and so in many cases we will describe these with references to such. Whenever these routines are called, they will be passed a ``gp_file`` pointer. This pointer will have originated from the ``crypt_open_file`` call, and so can safely be cast back to a ``gp_file_crypt`` pointer to allow private data to be accessed.




   :c:`close(gp_file *)`
     - close the given file; free any storage in the crypt specific parts of ``gp_file_crypt``, but not the ``gp_file_crypt`` structure itself.

   :c:`int getc(gp_file *)`
     - Get a single character from the file, returning it as an int (or -1 for EOF). Behaves like ``fgetc(FILE *)``.

   :c:`int putc(gp_file *, int)`
     - Put a single character to the file, returning the character on success, or EOF (and setting the error indicator) on error. Behaves like ``fgetc(FILE *)``.

   :c:`int read(gp_file *, size_t size, unsigned int count, void *buf)`
     - Reads count entries of size bytes the file into ``buf``, returning the number of entries read. Behaves like ``fread(FILE *, size, count, buf)``.

   :c:`int write(gp_file *, size_t size, unsigned int count, const void *buf)`
     - Writes count entries of size bytes from ``buf`` into the file, returning the number of entries written. Behaves like ``fwrite(FILE *, size, count, buf)``.

   :c:`int seek(gp_file *, gs_offset_t offset, int whence)`
     - Seeks within the file. Behaves like ``fseek(FILE *, offset, whence)``.

   :c:`gs_offset_t tell(gp_file *)`
     - Returns the current offset within the file. Behaves like ``ftell(FILE *)``.

   :c:`int eof(gp_file *)`
     - Returns 1 if we are at the end of the file, 0 otherwise. Behaves like ``feof(FILE *)``.

   :c:`gp_file * dup(gp_file *, const char *mode)`
     - Optional function, only used if clist files are to be stored in this filing system. Behaves like ``fdup(FILE *)``. Leave ``NULL`` if not implemented.

   :c:`int seekable(gp_file *)`
     - Returns 1 if the file is seekable, 0 otherwise. Certain output devices will only work with seekable files.

   :c:`int pread(gp_file *, size_t count, gs_offset_t offset, void *buf)`
     - Optional function, only used if clist files are to be stored in this filing system. Behaves like an atomic ``fseek(FILE *, offset, 0)`` and ``fread(FILE *, 1, count, buf)``. Akin to ``pread``.

   :c:`int pwrite(gp_file *, size_t count, gs_offset_t offset, const void *buf)`
     - Optional function, only used if clist files are to be stored in this filing system. Behaves like an atomic ``fseek(FILE *, offset, 0)`` and ``fwrite(FILE *, 1, count, buf)``. Akin to ``pwrite``.

   :c:`int is_char_buffered(gp_file *file)`
     - Returns 1 if the file is character buffered, 0 otherwise. Used for handling reading from terminals. Very unlikely to be used, so returning 0 all the time should be safe. Leave ``NULL`` to indicate "always 0".

   :c:`void fflush(gp_file *file)`
     - Ensures that any buffered data is written to the file. Behaves like ``fflush(FILE *)``. Leave ``NULL`` to indicate that no flushing is ever required.

   :c:`int ferror(gp_file *file)`
     - Returns non-zero if there has been an error, or 0 otherwise. Behaves like ``ferror(FILE *)``.

   :c:`FILE * get_file(gp_file *file)`
     - Optional: Gets the ``FILE *`` pointer that backs this file. Required for a few devices that insist on working with ``FILE *``'s direct. Generally safe to leave this set to ``NULL``, and those devices will fail gracefully.

   :c:`void clearerr(gp_file *file)`
     - Clear the error and EOF values for a file. Behaves like ``clearerror(FILE *)``.

   :c:`gp_file * reopen(gp_file *f, const char *fname, const char *mode)`
     - Optional function, only used if the ``gp_file`` came from an ``open_scratch`` call; can be left as ``NULL`` if the ``open_scratch`` pointer is set to ``NULL``. Reopen a stream with a different mode. Behaves like ``freopen(fname, mode, FILE *)``.







.. _callout:


Callouts
------------------------------------------

Callouts are a mechanism for the core code (specifically devices) to communicate with the user of gsapi. This communication can take the form of passing information out vis-a-vis what devices are doing, or requesting configuration from the caller to affect exactly how the device itself works.

This is deliberately an extensible system, so exact details of callouts should be documented with the device in question. In general however a callout handler will be of the form:


.. code-block:: c

   typedef int (*gs_callout)(void *callout_handle,
                             const char *device_name,
                             int id,
                             int size,
                             void *data);


The ``callout_handle`` value passed to the callout will be the value passed in at registration. The device_name should be a null-terminated string giving the name of the device (though care should be taken to cope with the case where ``device_name`` is ``NULL`` for potential future uses). The id value will have a (device-specific) meaning; see the documentation for the device in question for more details. The same id value may be used to mean different things in different devices. Finally, size and data have callout specific meanings, but typically, data will be a pointer to data block (which may either be uninitialised or wholly/partially initialised on entry, and may be updated on exit), and size will be the size (in bytes) of the block pointed to by data.

A return value of -1 (``gs_error_unknownerror``) means the callout was not recognised by the handler, and should be passed to more handlers. Other negative values are interpreted as standard Ghostscript error values, and stop the propagation of the callout. Non-negative return codes mean the callout was handled and should not be passed to any more registered callout handlers.



.. _API_Return codes:


Return codes
--------------


The gsapi_init_with_args_, :ref:`gsapi_run_*<gsapi_run_asterisk>` and gsapi_exit_ functions return an integer code.


Return Codes from ``gsapi_*()``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. list-table::
   :widths: 50 50
   :header-rows: 1

   * - CODE
     - STATUS
   * - 0
     - No errors
   * - gs_error_Quit
     - "quit" has been executed. This is not an error. ``gsapi_exit()`` must be called next.
   * - gs_error_interrupt
     - The polling callback function returned a negative value, requesting Ghostscript to abort.
   * - gs_error_NeedInput
     - More input is needed by ``gsapi_run_string_continue()``. This is not an error.
   * - gs_error_Info
     - "gs -h" has been executed. This is not an error. ``gsapi_exit()`` must be called next.
   * - < 0
     - Error
   * - <= gs_error_Fatal
     - Fatal error. ``gsapi_exit()`` must be called next.


The ``gsapi_run_*()`` functions do not flush stdio. If you want to see output from Ghostscript you must do this explicitly as shown in the example below.

When executing a string with ``gsapi_run_string_*()``, ``currentfile`` is the input from the string. Reading from ``%stdin`` uses the stdin callback.


Example Usage
--------------


To try out the following examples in a development environment like Microsoft's developer tools or Metrowerks Codewarrior, create a new project, save the example source code as a ``.c`` file and add it, along with the Ghostscript dll or shared library. You will also need to make sure the Ghostscript headers are available, either by adding their location (the src directory in the Ghostscript source distribution) to the project's search path, or by copying ``ierrors.h`` and ``iapi.h`` into the same directory as the example source.



Example 1
~~~~~~~~~~~~~~~

.. code-block:: c

   /* Example of using GS DLL as a ps2pdf converter. */

   #if defined(_WIN32) && !defined(_Windows)
   # define _Windows
   #endif
   #ifdef _Windows
   /* add this source to a project with gsdll32.dll, or compile it directly with:
    *   cl -D_Windows -Isrc -Febin\ps2pdf.exe ps2pdf.c bin\gsdll32.lib
    */
   # include <windows.h>
   # define GSDLLEXPORT __declspec(dllimport)
   #endif

   #include "ierrors.h"
   #include "iapi.h"

   void *minst = NULL;

   int main(int argc, char *argv[])
   {
       int code, code1;
       const char * gsargv[7];
       int gsargc;
       gsargv[0] = "";
       gsargv[1] = "-dNOPAUSE";
       gsargv[2] = "-dBATCH";
       gsargv[3] = "-dSAFER";
       gsargv[4] = "-sDEVICE=pdfwrite";
       gsargv[5] = "-sOutputFile=out.pdf";
       gsargv[6] = "input.ps";
       gsargc=7;

       code = gsapi_new_instance(&minst, NULL);
       if (code < 0)
           return 1;
       code = gsapi_set_arg_encoding(minst, GS_ARG_ENCODING_UTF8);
       if (code == 0)
           code = gsapi_init_with_args(minst, gsargc, gsargv);
       code1 = gsapi_exit(minst);
       if ((code == 0) || (code == gs_error_Quit))
           code = code1;

       gsapi_delete_instance(minst);

       if ((code == 0) || (code == gs_error_Quit))
           return 0;
       return 1;
   }


Example 2
~~~~~~~~~~~~~~~

.. code-block:: c

   /* Similar to command line gs */

   #if defined(_WIN32) && !defined(_Windows)
   # define _Windows
   #endif
   #ifdef _Windows
   /* Compile directly with:
    *   cl -D_Windows -Isrc -Febin\gstest.exe gstest.c bin\gsdll32.lib
    */
   # include <windows.h>
   # define GSDLLEXPORT __declspec(dllimport)
   #endif
   #include <stdio.h>
   #include "ierrors.h"
   #include "iapi.h"

   /* stdio functions */
   static int GSDLLCALL
   gsdll_stdin(void *instance, char *buf, int len)
   {
       int ch;
       int count = 0;
       while (count < len) {
           ch = fgetc(stdin);
           if (ch == EOF)
               return 0;
           *buf++ = ch;
           count++;
           if (ch == '\n')
               break;
       }
       return count;
   }

   static int GSDLLCALL
   gsdll_stdout(void *instance, const char *str, int len)
   {
       fwrite(str, 1, len, stdout);
       fflush(stdout);
       return len;
   }

   static int GSDLLCALL
   gsdll_stderr(void *instance, const char *str, int len)
   {
       fwrite(str, 1, len, stderr);
       fflush(stderr);
       return len;
   }

   void *minst = NULL;
   const char start_string[] = "systemdict /start get exec\n";

   int main(int argc, char *argv[])
   {
       int code, code1;
       int exit_code;

       code = gsapi_new_instance(&minst, NULL);
       if (code < 0)
           return 1;
       gsapi_set_stdio(minst, gsdll_stdin, gsdll_stdout, gsdll_stderr);
       code = gsapi_set_arg_encoding(minst, GS_ARG_ENCODING_UTF8);
       if (code == 0)
           code = gsapi_init_with_args(minst, argc, argv);
       if (code == 0)
           code = gsapi_run_string(minst, start_string, 0, &exit_code);
       code1 = gsapi_exit(minst);
       if ((code == 0) || (code == gs_error_Quit))
           code = code1;

       gsapi_delete_instance(minst);

       if ((code == 0) || (code == gs_error_Quit))
           return 0;
       return 1;
   }


Example 3
~~~~~~~~~~~~~~~


Replace ``main()`` in either of the above with the following code, showing how you can feed Ghostscript piecemeal:


.. code-block:: c

   const char *command = "1 2 add == flush\n";

   int main(int argc, char *argv[])
   {
       int code, code1;
       int exit_code;

       code = gsapi_new_instance(&minst, NULL);
       if (code < 0)
           return 1;
       code = gsapi_set_arg_encoding(minst, GS_ARG_ENCODING_UTF8);
       if (code == 0)
           code = gsapi_init_with_args(minst, argc, argv);

       if (code == 0) {
           gsapi_run_string_begin(minst, 0, &exit_code);
           gsapi_run_string_continue(minst, command, strlen(command), 0, &exit_code);
           gsapi_run_string_continue(minst, "qu", 2, 0, &exit_code);
           gsapi_run_string_continue(minst, "it", 2, 0, &exit_code);
           gsapi_run_string_end(minst, 0, &exit_code);
       }

       code1 = gsapi_exit(minst);
       if ((code == 0) || (code == gs_error_Quit))
           code = code1;

       gsapi_delete_instance(minst);

       if ((code == 0) || (code == gs_error_Quit))
           return 0;
       return 1;
   }


Example 4
~~~~~~~~~~~~~~~

When feeding Ghostscript piecemeal buffers, one can use the normal operators to configure things and invoke library routines. For example, to parse a PDF file one could say:


.. code-block:: c

    code = gsapi_run_string(minst, "(example.pdf) .runlibfile", 0, &exit_code);

and Ghostscript would open and process the file named "example.pdf" as if it had been passed as an argument to ``gsapi_init_with_args()``.




Multiple Threads
--------------------

The Ghostscript library should have been compiled with a thread safe run time library. Synchronisation of threads is entirely up to the caller. The exported ``gsapi_*()`` functions must be called from one thread only.



Standard Input and Output
----------------------------------------

When using the Ghostscript interpreter library interface, you have a choice of two standard input/output methods.

- If you do nothing, the "C" stdio will be used.

- If you use ``gsapi_set_stdio()``, all stdio will be redirected to the callback functions you provide. This would be used in a graphical user interface environment where stdio is not available, or where you wish to process Ghostscript input or output.

The callback functions are described in ``iapi.h``.



.. _display:

Display Device
-----------------

The display device is available for use with the Ghostscript interpreter library. While originally designed for allowing screen display of rendered output from Ghostscript, this is now powerful enough to provide a simple mechanism for getting rendered output suitable for use in all manner of output scenarios, including printing.

Details of the API and options are given in the file ``gdevdsp.h``. This device provides you with access to the raster output of Ghostscript. It is the callers responsibility to copy this raster to a display window or printer.

In order for this device to operate, it needs access to a structure containing a set of callback functions, and a callback handle (an opaque ``void *`` that can be used by caller to locate its own state). There are 2 ways that the device can get this information, a legacy method, and a modern method.


Legacy method
~~~~~~~~~~~~~~~~

The address of the callback structure, is provided using ``gsapi_set_display_callback()``. This must be called after ``gsapi_new_instance()`` and before ``gsapi_init_with_args()``.

With this call, the callback handle is passed as ``NULL`` by default, but can be overridden by using a parameter. We actively dislike this way of working, as we consider passing addresses via the command line distasteful. The handle can be set using:


.. code-block:: bash

   -sDisplayHandle=1234

Where "1234" is a string. The API was changed to use a string rather than an integer/long value when support for 64 bit systems arrived. A display "handle" is often a pointer, and since these command line options have to survive being processed by Postscript machinery, and Postscript only permits 32 bit number values, a different representation was required. Hence changing the value to a string, so that 64 bit values can be supported. The string formats allowed are:



.. list-table::
   :widths: 50 50

   * - 1234
     - implicit base 10
   * - 10#1234
     - explicit base 10
   * - 16#04d2
     - explicit base 16

The "number string" is parsed by the display device to retrieve the number value, and is then assigned to the void pointer parameter "pHandle" in the display device structure. Thus, for a trivial example, passing ``-sDisplayHandle=0`` will result in the first parameter passed to your display device callbacks being: ``(void *)0``.

The previous API, using a number value:

.. code-block:: bash

   -dDisplayHandle=1234

is still supported on 32 bit systems, but will cause a "typecheck" error on 64 bit systems, and is considered deprecated. It should not be used in new code.



Modern method
~~~~~~~~~~~~~~~~


The preferred method is to register a callout handler using gsapi_register_callout_. When this handler is called for the "display" device, with ``id = 0 (= DISPLAY_CALLOUT_GET_CALLBACK)``, then data should point to an empty ``gs_display_get_callback_t`` block, with ``size = sizeof(gs_display_get_callback_t)``.


.. code-block:: c

   typedef struct {
       display_callback *callback;
       void *caller_handle;
   } gs_display_get_callback_t;


The handler should fill in the structure before returning, with a return code of 0.

Note, that the ``DisplayHandle`` value is only consulted for display device callbacks registered using the (legacy, now deprecated) ``gsapi_set_display_callback`` API, not the preferred ``gsapi_register_callout`` based mechanism.

The device raster format can be configured using:


.. code-block:: bash

   -dDisplayFormat=NNNN


Options include:

- native, gray, RGB, CMYK or separation color spaces.
- alpha byte (ignored).
- 1 to 16 bits/component.
- bigendian (RGB) or littleendian (BGR) order.
- top first or bottom first raster.
- 16 bits/pixel with 555 or 565 bitfields.
- Chunky, Planar and Planar interleaved formats.
- "Full screen" or "Rectangle Request" modes of operation.

The operation of the device is best described with a walkthrough of some example code that uses it. For simplicity and clarity, we have omitted the error handling code in this example; in production code, every place where we get a code value returned we should check it for failure (a negative value) and clean up accordingly. First, we create an instance of Ghostscript:


.. code-block:: c

   void *minst = NULL;
   code = gsapi_new_instance(&minst, NULL);
   code = gsapi_set_stdio(minst, gsdll_stdin, gsdll_stdout, gsdll_stderr);

Next, we have to give the display device the address of our callback structure. In old code, we would do so using something like this:

.. code-block:: c

   code = gsapi_set_display_callback(minst, &display_callback);

We strongly recommend that you don't do that, but instead use the more modern :ref:`callout<Modern method>` mechanism:


.. code-block:: c

   code = gsapi_register_callout(minst, my_callout_handler, state);

where ``state`` is any ``void *`` value you like, usually a pointer to help you reach any internal state you may need. Earlier in your code you would have the definition of ``my_callout_handler`` that might look like this:


.. code-block:: c

   static int
   my_callout_handler(void *instance,
                        void *callout_handle,
                        const char *device_name,
                        int id,
                        int size,
                        void *data)
   {
       /* On entry, callout_handle == the value of state passed in
        * to gsapi_register_callout. */
       /* We are only interested in callouts from the display device. */
       if (device_name == NULL || strcmp(device_name, "display"))
           return -1;

       if (id == DISPLAY_CALLOUT_GET_CALLBACK)
       {
           /* Fill in the supplied block with the details of our callback
            * handler, and the handle to use. In this instance, the handle
            * is the pointer to our test structure. */
           gs_display_get_callback_t *cb = (gs_display_get_callback_t *)data;
           cb->callback = &display_callback;
           cb->caller_handle = callout_handle;
           return 0;
       }
       return -1;
   }


As you can see, this callout handler only responds to callouts for the display device, and then only for one particular function (``id``). It returns the same ``display_callback`` structure as the deprecated, legacy mechanism passed in using ``gsapi_set_display_callback``, with the added benefit that the ``caller_handle`` value can be passed in too. In this example we pass in the same value as was used for ``callout_handle``, but implementations are free to use any value they want.

Returning to our example, we now set up a set of arguments to setup Ghostscript:


.. code-block:: c

   int argc = 0;
   /* Allow for up to 32 args of up to 64 chars each. */
   char argv[32][64];
   sprintf(argc[argc++], "gs");
   sprintf(argv[argc++], "-sDEVICE=display");


The zeroth arg is a dummy argument to match the standard C mechanism for passing arguments to a program. Traditionally this is the name of the program being invoked. Next, we tell Ghostscript to use the display device.

.. code-block:: c

   sprintf(argv[argc++], "-sDEVICE=display");

Next we tell the display device what output format to use. The format is flexible enough to support common Windows, OS/2, Linux and Mac raster formats.

The format values are described in ``gdevdsp.h``. To select the display device with a Windows 24-bit RGB raster:

.. code-block:: c

   sprintf(argv[argc++], "-dDisplayFormat=%d",
        DISPLAY_COLORS_RGB | DISPLAY_ALPHA_NONE | DISPLAY_DEPTH_8 |
        DISPLAY_LITTLEENDIAN | DISPLAY_BOTTOMFIRST);


If (and only if) you used the legacy mechanism described above, you will need another argument to pass in the ``caller_handle`` value to be parroted back to the functions listed within ``display_callback``:


.. code-block:: c

   sprintf(arg2, "-dDisplayHandle=%d", callout_handle);


Any other arguments that you want can be added to the end of the command line, typically including a file to run. Then we pass that all to Ghostscript:


.. code-block:: c

   code = gsapi_init_with_args(minst, argc, argv);


At this point you should start to see your display callback functions being called. Exactly which callback functions are provided, and how they respond will determine exactly how the display device operates. The primary choice will be whether the device runs in "full page" or "rectangle request" mode. Details of these are given below.

Once we have finished processing the file, we can process other files using ``gsapi_run_file``, or feed in data using ``gsapi_run_string``. Once you have finished, you can shut the interpreter down and exit, using:

.. code-block:: c

   code = gsapi_exit(minst);
   gsapi_delete_instance(minst);

A full list of the display callback functions can be found in ``gdevdsp.h``. There are several different versions of the callback, corresponding to different "generations" of the device. In general you should use the latest one. The size field of the structure should be initialised to the size of the structure in bytes.


display_open()
~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: c

   int (*display_open)(void *handle, void *device);

This function will be called when the display device is opened. The device may be opened and closed many times, sometimes without any output being produced.


display_preclose()
~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: c

   int (*display_preclose)(void *handle, void *device);


This function will be called when the display device is about to be closed. The device will not actually be closed until this function returns.



display_close()
~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: c

   int (*display_close)(void *handle, void *device);


This function will be called once the display device has been closed. There will be no more events from the device unless/until it is reopened.



display_presize()
~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: c

   int (*display_presize)(void *handle, void *device,
           int width, int height, int raster, unsigned int format);


This function will be called when the display device is about to be resized. The device will only be resized if this function returns 0.




display_size()
~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: c

   int (*display_size)(void *handle, void *device, int width, int height,
               int raster, unsigned int format, unsigned char *pimage);


This function will be called when the display device is has been resized. The pointer to the raster image is pimage.



display_sync()
~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: c

   int (*display_sync)(void *handle, void *device);


This function may be called periodically during display to flush the page to the display.



display_page()
~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: c

   int (*display_page)(void *handle, void *device, int copies, int flush);


This function is called on a "showpage" operation (i.e. at the end of every page). Operation will continue as soon as this function returns.




display_update()
~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: c

   int (*display_update)(void *handle, void *device,
           int x, int y, int w, int h);


This function may get called repeatedly during rendering to indicate that an area of the output has been updated. Certain types of rendering will not see this function called back at all (in particular files using transparency).




display_memalloc()
~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: c

   int (*display_memalloc)(void *handle, void *device,
           size_t long size);


Note: In older versions of this API, size is an unsigned long rather than a ``size_t``.


If this function pointer is sent as ``NULL``, then the display device will handle all the memory allocations internally, and will always work in full page rendering mode.

Otherwise, this function will be called to allocate the storage for the page to be rendered into. If a non-NULL value is returned, then the device will proceed to render the full page into it. If ``NULL`` is returned, then the device will check a) whether we are using a V2 or greater display callback structure and b) whether that structure specifies a rectangle_request function pointer.

If both of those conditions are true, then the device will continue in rectangle request mode. Otherwise it will fail with an out of memory error.




display_memfree()
~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: c

   int (*display_memfree)(void *handle, void *device, void *ptr);


This function should be ``NULL`` if and only if ``display_memalloc`` is ``NULL``. Any memory allocated using ``display_memalloc`` will be freed via this function.


display_separation()
~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: c

   int (*display_separation)(void *handle, void *device,
           int component, const char *component_name,
           unsigned short c, unsigned short m,
           unsigned short y, unsigned short k);


When using ``DISPLAY_COLORS_SEPARATION``, this function will be called once for every separation component - first "Cyan", "Magenta", "Yellow" and "Black", then any spot colors used. The supplied c, m, y and k values give the equivalent color for each spot. Each colorant value ranges from 0 (for none) to 65535 (full).

In separation color mode you are expected to count the number of calls you get to this function after each ``display_size`` to know how many colors you are dealing with.



display_adjust_band_height()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: c

   int (*display_adjust_band_height)(void *handle, void *device,
         int bandheight);


When running in "rectangle request mode" the device first renders the page to a display list internally. It can then be played back repeatedly so that different regions (rectangles) of the page can be extracted in sequence. A common use of this is to support "banded" operation, where the page is divided into multiple non-overlapping bands of a fixed height.

The display device itself will pick an appropriate band height for it to use. If this function pointer is left as ``NULL`` then this value will be used unchanged. Otherwise, the proposed value will be offered to this function. This function can override the choice of bandheight, by returning the value that it would like to be used in preference.

In general, this figure should (as much as possible) only be adjusted downwards. For example, a device targeting an inkjet printer with 200 nozzles in the print head might like to extract bands that are a multiple of 200 lines high. So the function might ``return max(200, 200*(bandheight/200))``. If the function returns 0, then the existing value will be used unchanged.


Any size rectangle can be chosen with any size bandheight, so ultimately the value chosen here will not matter much. It may make some small difference in speed in some cases.



display_rectangle_request()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: c

   int (*display_rectangle_request)(void *handle, void *device,
         void **memory, int *ox, int *oy,
         int *raster, int *plane_raster,
         int *x, int *y, int *w, int *h);


If the display device chooses to use rectangle request mode, this function will be called repeatedly to request a rectangle to render. Ghostscript will render the rectangle, and call this function again. The implementer is expected to handle the rectangle that has just been rendered, and to return the details of another rectangle to render. This will continue until a rectangle with zero height or width is returned, whereupon Ghostscript will continue operation.

On entry, ``*raster`` and ``*plane_raster`` are set to the values expected by the format in use. All the other pointers point to uninitialised values.

On exit, the values should be updated appropriately. The implementor is expected to store the values returned so that the rendered output given can be correctly interpreted when control returns to this function.

``memory`` should be updated to point to a block of memory to use for the rendered output. Pixel (``*ox``, ``*oy``) is the first pixel represented in that block.

``*raster`` is the number of bytes difference between the address of component 0 of Pixel(``*ox``, ``*oy``) and the address of component 0 of Pixel(``*ox``, 1 + ``*oy``).

``*plane_raster`` is the number of bytes difference between the address of component 0 of Pixel(``*ox``, ``*oy``) and the address of component 1 of Pixel(``*ox``, ``*oy``), if in planar mode, 0 otherwise. ``*x``, ``*y``, ``*w`` and ``*h`` give the rectangle requested within that memory block.

Any set of rectangles can be rendered with this method, so this can be used to drive Ghostscript in various ways. Firstly, it is simple to request a set of non-overlapping "bands" that cover the page, to drive a printer. Alternatively, rectangles can be chosen to fill a given block of memory to implement a window panning around a larger page. Either the whole image could be redrawn each time, or smaller rectangles around the edge of the panned area could be requested. The choice is down to the caller.

Some examples of driving this code in full page mode are in ``dwmain.c`` (Windows), ``dpmain.c`` (OS/2) and ``dxmain.c`` (X11/Linux), and ``dmmain.c`` (MacOS Classic or Carbon).

Alternatively an example that drives this code in both full page and rectangle request mode can be found in ``api_test.c``.

On some platforms, the calling convention for the display device callbacks in ``gdevdsp.h`` is not the same as the exported :ref:`gsapi_*() <gsapi_asterisk>` functions in ``iapi.h``.



.. include:: footer.rst









