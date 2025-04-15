.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: C#


.. include:: header.rst

Introduction
=======================

In the GhostPDL repository a sample C# project can be found in ``/demos/csharp``.

Within this project the following namespaces and corresponding C# files are of relevance:

- GhostAPI_ ``ghostapi.cs``
- GhostNET_ ``ghostnet.cs``
- GhostMono_ ``ghostmono.cs``



Platform & setup
=======================

Ghostscript should be built as a shared library for your platform.

See :ref:`Building Ghostscript<Make.html>`.


GhostAPI
=======================

:title:`GhostAPI` is the main wrapper responsible for bridging over to the C library and ensuring that the correct DLLs are imported.

:title:`GhostAPI` contains the ``ghostapi`` class which does not need to be instantiated as it provides public static methods. These methods, which mirror their C counterparts, are as follows:


.. list-table::
   :header-rows: 1
   :widths: 25 75

   * - Method
     - Description
   * - gsapi_revision_
     - Returns the revision numbers and strings of the Ghostscript interpreter library.
   * - gsapi_new_instance_
     - Create a new instance of Ghostscript.
   * - gsapi_delete_instance_
     - Destroy an instance of Ghostscript.
   * - gsapi_set_stdio_with_handle_
     - Set the callback functions for ``stdio``, together with the handle to use in the callback functions.
   * - gsapi_set_stdio_
     - Set the callback functions for ``stdio``.
   * - gsapi_set_poll_with_handle_
     - Set the callback function for polling, together with the handle to pass to the callback function.
   * - gsapi_set_poll_
     - Set the callback function for polling.
   * - gsapi_set_display_callback_
     - *deprecated*
   * - gsapi_register_callout_
     - This call registers a callout handler.
   * - gsapi_deregister_callout_
     - This call deregisters a previously registered callout handler.
   * - gsapi_set_arg_encoding_
     - Set the encoding used for the interpretation of all subsequent args supplied via the gsapi interface on this instance.
   * - gsapi_set_default_device_list_
     - Set the string containing the list of default device names.
   * - gsapi_get_default_device_list_
     - Returns a pointer to the current default device string.
   * - gsapi_init_with_args_
     - Initialise the interpreter.
   * - :ref:`gsapi_run_*<gsapi_run_asterisk>`
     - (Wildcard for various "run" methods).
   * - gsapi_exit_
     - Exit the interpreter.
   * - gsapi_set_param_
     - Set a parameter.
   * - gsapi_get_param_
     - Get a parameter.
   * - gsapi_enumerate_params_
     - Enumerate the current parameters.
   * - gsapi_add_control_path_
     - Add a (case sensitive) path to one of the lists of permitted paths for file access.
   * - gsapi_remove_control_path_
     - Remove a (case sensitive) path from one of the lists of permitted paths for file access.
   * - gsapi_purge_control_paths_
     - Clear all the paths from one of the lists of permitted paths for file access.
   * - gsapi_activate_path_control_
     - Enable/Disable path control.
   * - gsapi_is_path_control_active_
     - Query whether path control is activated or not.


:title:`GhostAPI` contains some essential structs & enums as well as a static class for some constants, finally it contains the main ``GSAPI`` class which holds the key methods which interface with the C library.





Structs and Enums
-------------------


``gsapi_revision_t``
~~~~~~~~~~~~~~~~~~~~~~~~~

This struct is used to contain information pertinent to the version of Ghostscript.


.. code-block:: csharp

  public struct gsapi_revision_t
  {
      public IntPtr product;
      public IntPtr copyright;
      public int revision;
      public int revisiondate;
  }


``gs_set_param_type``
~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: csharp

  public enum gs_set_param_type
  {
      gs_spt_invalid = -1,
      gs_spt_null =    0, /* void * is NULL */
      gs_spt_bool =    1, /* void * is NULL (false) or non-NULL (true) */
      gs_spt_int = 2, /* void * is a pointer to an int */
      gs_spt_float = 3, /* void * is a float * */
      gs_spt_name = 4, /* void * is a char * */
      gs_spt_string =    5, /* void * is a char * */
      gs_spt_long =    6, /* void * is a long * */
      gs_spt_i64 = 7, /* void * is an int64_t * */
      gs_spt_size_t =    8, /* void * is a size_t * */
      gs_spt_parsed =    9, /* void * is a pointer to a char * to be parsed */
      gs_spt_more_to_come = 1 << 31
  };


``gsEncoding``
~~~~~~~~~~~~~~~~~~~

.. code-block:: csharp

  public enum gsEncoding
  {
      GS_ARG_ENCODING_LOCAL = 0,
      GS_ARG_ENCODING_UTF8 = 1,
      GS_ARG_ENCODING_UTF16LE = 2
  };



Constants
-------------------

Constants are stored in the static class ``gsConstants`` for direct referencing.


``gsConstants``
~~~~~~~~~~~~~~~~~~~


.. code-block:: csharp

  static class gsConstants
  {
      public const int E_QUIT = -101;
      public const int GS_READ_BUFFER = 32768;
      public const int DISPLAY_UNUSED_LAST = (1 << 7);
      public const int DISPLAY_COLORS_RGB = (1 << 2);
      public const int DISPLAY_DEPTH_8 = (1 << 11);
      public const int DISPLAY_LITTLEENDIAN = (1 << 16);
      public const int DISPLAY_BIGENDIAN = (0 << 16);
  }




GSAPI
-------------------



Methods contained within are explained below.

:ref:`gsapi_run_*<csharp_gsapi_run_asterisk>` and gsapi_exit_ methods return an ``int`` code which can be interpreted as follows:


.. list-table::
   :header-rows: 1

   * - code
     - status
   * - ``0``
     - no error
   * - ``gsConstants.E_QUIT``
     - "quit" has been executed. This is not an error. gsapi_exit_ must be called next
   * - ``<0``
     - error



.. note::

  For full details on these return codes please see :ref:`The C API return codes<API_Return codes>`.

  All :title:`GSAPI` methods aside from ``gsapi_revision`` and ``gsapi_new_instance`` should pass an instance of Ghostscript as their first parameter with an ``IntPtr`` instance





``gsapi_revision``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

This method returns the revision numbers and strings of the Ghostscript interpreter library; you should call it before any other interpreter library functions to make sure that the correct version of the Ghostscript interpreter has been loaded.



.. code-block:: csharp

  public static extern int gsapi_revision(ref gsapi_revision_t vers, int size);


.. note::

  The method should write to a reference variable which conforms to the struct `gsapi_revision_t`_.




``gsapi_new_instance``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Creates a new instance of Ghostscript. This instance is passed to most other :title:`GSAPI` methods. Unless Ghostscript has been compiled with the ``GS_THREADSAFE`` define, only one instance at a time is supported.

.. code-block:: csharp

  public static extern int gsapi_new_instance(out IntPtr pinstance,
                                                  IntPtr caller_handle);

.. note::

  The method returns a pointer which represents your instance of Ghostscript.





``gsapi_delete_instance``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Destroy an instance of Ghostscript. Before you call this, Ghostscript must have finished. If Ghostscript has been initialised, you must call gsapi_exit_ beforehand.


.. code-block:: csharp

  public static extern void gsapi_delete_instance(IntPtr instance);


**Sample code:**

.. code-block:: csharp

  GSAPI.gsapi_delete_instance(gsInstance);
  gsInstance = IntPtr.Zero;




``gsapi_set_stdio_with_handle``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Set the callback functions for ``stdio``, together with the handle to use in the callback functions. The stdin callback function should return the number of characters read, ``0`` for ``EOF``, or ``-1`` for ``error``. The ``stdout`` and ``stderr`` callback functions should return the number of characters written.

.. note::

  These callbacks do not affect output device I/O when using "%stdout" as the output file. In that case, device output will still be directed to the process "stdout" file descriptor, not to the ``stdio`` callback.


.. code-block:: csharp

  public static extern int gsapi_set_stdio_with_handle(IntPtr instance,
                                             gs_stdio_handler stdin,
                                             gs_stdio_handler stdout,
                                             gs_stdio_handler stderr,
                                                       IntPtr caller_handle);




``gsapi_set_stdio``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Set the callback functions for ``stdio``. The handle used in the callbacks will be taken from the value passed to gsapi_new_instance_. Otherwise the behaviour of this function matches gsapi_set_stdio_with_handle_.


.. code-block:: csharp

  public static extern int gsapi_set_stdio_with_handle(IntPtr instance,
                                             gs_stdio_handler stdin,
                                             gs_stdio_handler stdout,
                                             gs_stdio_handler stderr);




``gsapi_set_poll_with_handle``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Set the callback function for polling, together with the handle to pass to the callback function. This function will only be called if the Ghostscript interpreter was compiled with ``CHECK_INTERRUPTS`` as described in ``gpcheck.h``.

The polling function should return zero if all is well, and return negative if it wants Ghostscript to abort. This is often used for checking for a user cancel. This can also be used for handling window events or cooperative multitasking.

The polling function is called very frequently during interpretation and rendering so it must be fast. If the function is slow, then using a counter to return 0 immediately some number of times can be used to reduce the performance impact.


.. code-block:: csharp

  public static extern int gsapi_set_poll_with_handle(IntPtr instance,
                                               gsPollHandler pollfn,
                                                      IntPtr caller_handle);



``gsapi_set_poll``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Set the callback function for polling. The handle passed to the callback function will be taken from the handle passed to gsapi_new_instance_. Otherwise the behaviour of this function matches gsapi_set_poll_with_handle_.


.. code-block:: csharp

  public static extern int gsapi_set_poll(IntPtr instance,
                                    gsPollHandler pollfn);



``gsapi_set_display_callback``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

This call is deprecated; please use gsapi_register_callout_ to register a :ref:`callout handler<gs_callout>` for the display device in preference.


.. code-block:: csharp

  public static extern int gsapi_set_display_callback(IntPtr pinstance,
                                                      IntPtr caller_handle);


``gsapi_register_callout``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

This call registers a :ref:`callout handler<gs_callout>`.

.. code-block:: csharp

  public static extern int gsapi_register_callout(IntPtr instance,
                                               gsCallOut callout,
                                                  IntPtr callout_handle);



``gsapi_deregister_callout``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

This call deregisters a :ref:`callout handler<gs_callout>` previously registered with gsapi_register_callout_. All three arguments must match exactly for the :ref:`callout handler<gs_callout>` to be deregistered.

.. code-block:: csharp

  public static extern int gsapi_deregister_callout(IntPtr instance,
                                                 gsCallOut callout,
                                                    IntPtr callout_handle);




``gsapi_set_arg_encoding``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Set the encoding used for the interpretation of all subsequent arguments supplied via the :title:`GhostAPI` interface on this instance. By default we expect args to be in encoding 0 (the 'local' encoding for this OS). On Windows this means "the currently selected codepage". On Linux this typically means ``utf8``. This means that omitting to call this function will leave Ghostscript running exactly as it always has. Please note that use of the 'local' encoding is now deprecated and should be avoided in new code. This must be called after gsapi_new_instance_ and before gsapi_init_with_args_.


.. code-block:: csharp

  public static extern int gsapi_set_arg_encoding(IntPtr instance,
                                                     int encoding);



``gsapi_set_default_device_list``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Set the string containing the list of default device names, for example "display x11alpha x11 bbox". Allows the calling application to influence which device(s) Ghostscript will try, in order, in its selection of the default device. This must be called after gsapi_new_instance_ and before gsapi_init_with_args_.

.. code-block:: csharp

  public static extern int gsapi_set_default_device_list(IntPtr instance,
                                                         IntPtr list,
                                                        ref int listlen);



``gsapi_get_default_device_list``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Returns a pointer to the current default device string. This must be called after gsapi_new_instance_ and before gsapi_init_with_args_.


.. code-block:: csharp

  public static extern int gsapi_get_default_device_list(IntPtr instance,
                                                     ref IntPtr list,
                                                        ref int listlen);

``gsapi_init_with_args``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

To initialise the interpreter, pass your ``instance`` of Ghostscript, your argument count, ``argc`` and your argument variables, ``argv``.


.. code-block:: csharp

  public static extern int gsapi_init_with_args(IntPtr instance,
                                                   int argc,
                                                IntPtr argv);




.. _csharp_gsapi_run_asterisk:


``gsapi_run_*``
~~~~~~~~~~~~~~~~~~

If these functions return ``<= -100``, either quit or a fatal error has occured. You must call gsapi_exit_ next. The only exception is gsapi_run_string_continue_ which will return ``gs_error_NeedInput`` if all is well.

There is a 64 KB length limit on any buffer submitted to a ``gsapi_run_*`` function for processing. If you have more than 65535 bytes of input then you must split it into smaller pieces and submit each in a separate gsapi_run_string_continue_ call.


``gsapi_run_string_begin``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: csharp

  public static extern int gsapi_run_string_begin(IntPtr instance,
                                                     int usererr,
                                                 ref int exitcode);

``gsapi_run_string_continue``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: csharp

  public static extern int gsapi_run_string_continue(IntPtr instance,
                                                     IntPtr command,
                                                        int count,
                                                        int usererr,
                                                    ref int exitcode);



``gsapi_run_string_with_length``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: csharp

  public static extern int gsapi_run_string_with_length(IntPtr instance,
                                                           IntPtr command,
                                                             uint length,
                                                              int usererr,
                                                          ref int exitcode);

``gsapi_run_string``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: csharp

  public static extern int gsapi_run_string(IntPtr instance,
                                            IntPtr command,
                                               int usererr,
                                           ref int exitcode);

``gsapi_run_string_end``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: csharp

  public static extern int gsapi_run_string_end(IntPtr instance,
                                                   int usererr,
                                               ref int exitcode);


``gsapi_run_file``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: csharp

  public static extern int gsapi_run_file(IntPtr instance,
                                          IntPtr filename,
                                             int usererr,
                                         ref int exitcode);




``gsapi_exit``
~~~~~~~~~~~~~~~~


Exit the interpreter. This must be called on shutdown if gsapi_init_with_args_ has been called, and just before gsapi_delete_instance_.


.. code-block:: csharp

  public static extern int gsapi_exit(IntPtr instance);



``gsapi_set_param``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Sets a parameter.

Broadly, this is equivalent to setting a parameter using ``-d``, ``-s`` or ``-p`` on the command line. This call cannot be made during a gsapi_run_string_ operation.

Parameters in this context are not the same as 'arguments' as processed by gsapi_init_with_args_, but often the same thing can be achieved. For example, with gsapi_init_with_args_, we can pass "-r200" to change the resolution. Broadly the same thing can be achieved by using gsapi_set_param_ to set a parsed value of "<</HWResolution [ 200.0 200.0 ]>>".

Internally, when we set a parameter, we perform an ``initgraphics`` operation. This means that using gsapi_set_param_ other than at the start of a page is likely to give unexpected results.

Attempting to set a parameter that the device does not recognise will be silently ignored, and that parameter will not be found in subsequent gsapi_get_param_ calls.


.. code-block:: csharp

  public static extern int gsapi_set_param(IntPtr instance,
                                           IntPtr param,
                                           IntPtr value,
                                gs_set_param_type type);



.. note::

  The type argument, as a ``gs_set_param_type``, dictates the kind of object that the value argument points to.

  For more on the C implementation of parameters see: :ref:`Ghostscript parameters in C<Use_Setting Parameters>`.



``gsapi_get_param``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Retrieve the current value of a parameter.

If an error occurs, the return value is negative. Otherwise the return value is the number of bytes required for storage of the value. Call once with value ``NULL`` to get the number of bytes required, then call again with value pointing to at least the required number of bytes where the value will be copied out. Note that the caller is required to know the type of value in order to get it. For all types other than ``gs_spt_string``, ``gs_spt_name``, and ``gs_spt_parsed`` knowing the type means you already know the size required.

This call retrieves parameters/values that have made it to the device. Thus, any values set using ``gs_spt_more_to_come`` without a following call omitting that flag will not be retrieved. Similarly, attempting to get a parameter before gsapi_init_with_args_ has been called will not list any, even if gsapi_set_param_ has been used.

Attempting to read a parameter that is not set will return ``gs_error_undefined`` (-21). Note that calling gsapi_set_param_ followed by gsapi_get_param_ may not find the value, if the device did not recognise the key as being one of its configuration keys.

For further documentation please refer to :ref:`the C API<API_gsapi_get_param>`.


.. code-block:: csharp

  public static extern int gsapi_get_param(IntPtr instance,
                                           IntPtr param,
                                           IntPtr value,
                                gs_set_param_type type);



``gsapi_enumerate_params``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Enumerate the current parameters. Call repeatedly to list out the current parameters.

The first call should have ``iter = NULL``. Subsequent calls should pass the same pointer in so the iterator can be updated. Negative return codes indicate error, 0 success, and 1 indicates that there are no more keys to read. On success, key will be updated to point to a null terminated string with the key name that is guaranteed to be valid until the next call to gsapi_enumerate_params_. If type is non ``NULL`` then the pointer type will be updated to have the type of the parameter.

.. note::

  Only one enumeration can happen at a time. Starting a second enumeration will reset the first.

The enumeration only returns parameters/values that have made it to the device. Thus, any values set using the ``gs_spt_more_to_come`` without a following call omitting that flag will not be retrieved. Similarly, attempting to enumerate parameters before gsapi_init_with_args_ has been called will not list any, even if gsapi_set_param_ has been used.


.. code-block:: csharp

  public static extern int gsapi_enumerate_params(IntPtr instance,
                                              out IntPtr iter,
                                              out IntPtr key,
                                                  IntPtr type);



``gsapi_add_control_path``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Add a (case sensitive) path to one of the lists of :ref:`permitted paths<Use Safer>` for file access.


.. code-block:: csharp

  public static extern int gsapi_add_control_path(IntPtr instance,
                                                     int type,
                                                  IntPtr path);

``gsapi_remove_control_path``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Remove a (case sensitive) path from one of the lists of :ref:`permitted paths<Use Safer>` for file access.


.. code-block:: csharp

  public static extern int gsapi_remove_control_path(IntPtr instance,
                                                        int type,
                                                     IntPtr path);

``gsapi_purge_control_paths``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Clear all the paths from one of the lists of :ref:`permitted paths<Use Safer>` for file access.


.. code-block:: csharp

  public static extern void gsapi_purge_control_paths(IntPtr instance,
                                                         int type);



``gsapi_activate_path_control``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Enable/Disable path control (i.e. whether paths are checked against :ref:`permitted paths<Use Safer>` before access is granted).


.. code-block:: csharp

  public static extern void gsapi_activate_path_control(IntPtr instance,
                                                           int enable);


``gsapi_is_path_control_active``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Query whether path control is activated or not.

.. code-block:: csharp

  public static extern int gsapi_is_path_control_active(IntPtr instance);



Callback and Callout prototypes
--------------------------------------

:title:`GSAPI` also defines some prototype pointers which are defined as follows.

``gs_stdio_handler``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: csharp

  /* Callback proto for stdio */
  public delegate int gs_stdio_handler(IntPtr caller_handle,
                                       IntPtr buffer,
                                          int len);

``gsPollHandler``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: csharp

  /* Callback proto for poll function */
  public delegate int gsPollHandler(IntPtr caller_handle);


.. _gs_callout:

``gsCallOut``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: csharp

  /* Callout proto */
  public delegate int gsCallOut(IntPtr callout_handle,
                                IntPtr device_name,
                                   int id,
                                   int size,
                                IntPtr data);


GhostNET
=======================

:title:`GhostNET` is the `.NET`_ interface into :title:`GhostAPI`. It exemplifies how to do more complex operations involving multiple API calls and sequences. See the table below for the main methods:


.. list-table::
   :header-rows: 1
   :widths: 20 75 5

   * - Method
     - Description
     - Notes
   * - :ref:`GetVersion<GhostNET_GetVersion>`
     - Returns the version of Ghostscript.
     -
   * - :ref:`DisplayDeviceOpen<GhostNET_DisplayDeviceOpen>`
     - Sets up the display device ahead of time.
     -
   * - :ref:`DisplayDeviceClose<GhostNET_DisplayDeviceClose>`
     - Closes the display device and deletes the instance.
     -
   * - :ref:`GetPageCount<GhostNET_GetPageCount>`
     - Returns the page count for the document.
     -
   * - :ref:`CreateXPS<GhostNET_CreateXPS>`
     - Launches a thread to create an XPS document for Windows printing.
     - :ref:`asynchronous<Delegates>`
   * - :ref:`DistillPS<GhostNET_DistillPS>`
     - Launches a thread rendering all the pages of a supplied PostScript file to a PDF.
     - :ref:`asynchronous<GhostNET_Delegates>`
   * - :ref:`DisplayDeviceRunFile<GhostNET_DisplayDeviceRunFile>`
     - Launches a thread to run a file with the display device.
     - :ref:`asynchronous<GhostNET_Delegates>`
   * - :ref:`DisplayDeviceRenderThumbs<GhostNET_DisplayDeviceRenderThumbs>`
     - Launches a thread rendering all the pages with the display device to collect thumbnail images.
     - :ref:`asynchronous<GhostNET_Delegates>`
   * - :ref:`DisplayDeviceRenderPages<GhostNET_DisplayDeviceRenderPages>`
     - Launches a thread rendering a set of pages with the display device.
     - :ref:`asynchronous<GhostNET_Delegates>`
   * - :ref:`GetStatus<GhostNET_GetStatus>`
     - Returns the current status of Ghostscript.
     -
   * - :ref:`Cancel<GhostNET_Cancel>`
     - Cancels asynchronous operations.
     -
   * - :ref:`GhostscriptException<GhostNET_GhostscriptException>`
     - An application developer can log any exceptions in this public class.
     -


In ``demos/csharp/windows/ghostnet.sln`` there is a sample C# demo project.

This project can be opened in `Visual Studio`_ and used to test the Ghostscript API alongside a UI which handles opening PostScript and PDF files. The sample application here allows for file browsing and Ghostscript file viewing.

Below is a screenshot of the sample application with a PDF open:


.. note we embedd the image with raw HTML because Sphinx is incapable of doing percentage style widths ... :(

.. raw:: html

   <img src="_static/ghostnet-wpf-example.png" width=100%/>




Enums
--------


Tasks
~~~~~~~~~~~~~~~~

The Ghostscript task type ``enum`` is used to inform :title:`GhostAPI` of the type of operation which is being requested.


.. list-table::
   :header-rows: 1

   * - Task
     - Description
   * - ``PS_DISTILL``
     - Task associated with converting a PostScript stream to a PDF document.
   * - ``CREATE_XPS``
     - Task associated with outputting a copy of a document to XPS.
   * - ``SAVE_RESULT``
     - Task associated with saving documents.
   * - ``GET_PAGE_COUNT``
     - Task associated with getting the page count of a document.
   * - ``GENERIC``
     - Generic task identifier.
   * - ``DISPLAY_DEV_THUMBS``
     - Display Device task associated with rendering thumbnails.
   * - ``DISPLAY_DEV_NON_PDF``
     - Display Device task associated with non-PDF or non-XPS rendering (see: :ref:`Ghostscript & Page Description Languages<gs_and_PDL>`).
   * - ``DISPLAY_DEV_PDF``
     - Display Device task associated with PDF & XPS rendering (see: :ref:`Ghostscript & Page Description Languages<gs_and_PDL>`).
   * - ``DISPLAY_DEV_RUN_FILE``
     - Display Device task associated with running files.


Task types are defined as ``GS_Task_t``.


.. code-block:: csharp

  public enum GS_Task_t
  {
      PS_DISTILL,
      CREATE_XPS,
      SAVE_RESULT,
      GET_PAGE_COUNT,
      GENERIC,
      DISPLAY_DEV_THUMBS,
      DISPLAY_DEV_NON_PDF,
      DISPLAY_DEV_PDF,
      DISPLAY_DEV_RUN_FILE
  }







Results
~~~~~~~~~~~~~~~~

Result types are defined as ``GS_Result_t``.


.. code-block:: csharp

  public enum GS_Result_t
  {
      gsOK,
      gsFAILED,
      gsCANCELLED
  }


.. _GhostNET_Status:

Status
~~~~~~~~~~~~~~~~

Status types are defined as ``gsStatus``.


.. code-block:: csharp

  public enum gsStatus
  {
      GS_READY,
      GS_BUSY,
      GS_ERROR
  }



The Parameter Struct
-------------------------

The parameter struct ``gsParamState_t`` allows for bundles of information to be processed by Ghostscript to complete overall requests.


.. code-block:: csharp

  public struct gsParamState_t
  {
      public String outputfile;
      public String inputfile;
      public GS_Task_t task;
      public GS_Result_t result;
      public int num_pages;
      public List<int> pages;
      public int firstpage;
      public int lastpage;
      public int currpage;
      public List<String> args;
      public int return_code;
      public double zoom;
      public bool aa;
      public bool is_valid;
  };


Parameters explained
~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Setting up your parameters (with any dedicated bespoke method(s) which your application requires) is needed when communicating directly with :title:`GhostAPI`.

When requesting Ghostscript to process an operation an application developer should send a parameter payload which defines the details for the operation.

For example in :title:`GhostNET` we can see the public method as follows:


.. code-block:: csharp

  public gsStatus DistillPS(String fileName, int resolution)
  {
      gsParamState_t gsparams = new gsParamState_t();
      gsparams.args = new List<string>();

      gsparams.inputfile = fileName;
      gsparams.args.Add("gs");
      gsparams.args.Add("-sDEVICE=pdfwrite");
      gsparams.outputfile = Path.GetTempFileName();
      gsparams.args.Add("-o" + gsparams.outputfile);
      gsparams.task = GS_Task_t.PS_DISTILL;

      return RunGhostscriptAsync(gsparams);
  }



Here we can see a parameter payload being setup before being passed on to the asynchronous method ``RunGhostscriptAsync`` which sets up a worker thread to run according to the task type in the payload.

:title:`GhostNET` handles many common operations on an application developer's behalf, however if you require to write your own methods to interface with :title:`GhostAPI` then referring to the public methods in :title:`GhostNET` is a good starting point.

For full documentation on parameters refer to :ref:`Ghostscript parameters<Use_Setting Parameters>`.





The Event class
------------------

:title:`GhostNET` contains a public class ``gsEventArgs`` which is an extension of the C# class ``EventArgs``. This class is used to set and get events as they occur. :title:`GhostNET` will create these payloads and deliver them back to the application layer's ``ProgressCallBack`` method asynchronously.


.. code-block:: csharp

  public class gsEventArgs : EventArgs
  {
      private bool m_completed;
      private int m_progress;
      private gsParamState_t m_param;
      public bool Completed
      {
          get { return m_completed; }
      }
      public gsParamState_t Params
      {
          get { return m_param; }
      }
      public int Progress
      {
          get { return m_progress; }
      }
      public gsEventArgs(bool completed, int progress, gsParamState_t param)
      {
          m_completed = completed;
          m_progress = progress;
          m_param = param;
      }
  }


GSNET
-----------

This class should be instantiated as a member variable in your application with callback definitions setup as required.

Handlers for asynchronous operations can injected by providing your own bespoke callback methods to your instance's ``ProgressCallBack`` function.


Sample code
~~~~~~~~~~~~~~~~

.. code-block:: csharp

  /* Set up ghostscript with callbacks for system updates */
  m_ghostscript = new GSNET();
  m_ghostscript.ProgressCallBack += new GSNET.Progress(gsProgress);
  m_ghostscript.StdIOCallBack += new GSNET.StdIO(gsIO);
  m_ghostscript.DLLProblemCallBack += new GSNET.DLLProblem(gsDLL);
  m_ghostscript.PageRenderedCallBack += new GSNET.PageRendered(gsPageRendered);
  m_ghostscript.DisplayDeviceOpen();

  /* example callback stubs for asynchronous operations */
  private void gsProgress(gsEventArgs asyncInformation)
  {
      Console.WriteLine($"gsProgress().progress:{asyncInformation.Progress}");

      if (asyncInformation.Completed) // task complete
      {
          // what was the task?
          switch (asyncInformation.Params.task)
          {
              case GS_Task_t.CREATE_XPS:
                  Console.WriteLine($"CREATE_XPS.outputfile:");
                  Console.WriteLine($"{asyncInformation.Params.result.outputfile}");
                  break;

              case GS_Task_t.PS_DISTILL:
                  Console.WriteLine($"PS_DISTILL.outputfile:");
                  Console.WriteLine($"{asyncInformation.Params.result.outputfile}");
                  break;

              case GS_Task_t.SAVE_RESULT:

                  break;

              case GS_Task_t.DISPLAY_DEV_THUMBS:

                  break;

              case GS_Task_t.DISPLAY_DEV_RUN_FILE:

                  break;

              case GS_Task_t.DISPLAY_DEV_PDF:

                  break;

              case GS_Task_t.DISPLAY_DEV_NON_PDF:

                  break;

              default:

                  break;
          }

          // task failed
          if (asyncInformation.Params.result == GS_Result_t.gsFAILED)
          {
              switch (asyncInformation.Params.task)
              {
                  case GS_Task_t.CREATE_XPS:

                      break;

                  case GS_Task_t.PS_DISTILL:

                      break;

                  case GS_Task_t.SAVE_RESULT:

                      break;

                  default:

                      break;
              }
              return;
          }

          // task cancelled
          if (asyncInformation.Params.result == GS_Result_t.gsCANCELLED)
          {

          }
      }
      else // task is still running
      {
          switch (asyncInformation.Params.task)
          {
              case GS_Task_t.CREATE_XPS:

                  break;

              case GS_Task_t.PS_DISTILL:

                  break;

              case GS_Task_t.SAVE_RESULT:

                  break;
          }
      }
  }

  private void gsIO(String message, int len)
  {
      Console.WriteLine($"gsIO().message:{message}, length:{len}");
  }

  private void gsDLL(String message)
  {
      Console.WriteLine($"gsDLL().message:{message}");
  }

  private void gsPageRendered(int width,
                              int height,
                              int raster,
                              IntPtr data,
                              gsParamState_t state)
  {

  };


.. note::

  Once a Ghostscript operation is in progress any defined callback functions will be called as the operation runs up unto completion. These callback methods are essential for your application to interpret activity events and react accordingly.

An explanation of callbacks and the available public methods within ``GSNET`` are explained below.



.. _GhostNET_Delegates:

Delegates
~~~~~~~~~~~~~~~

To handle *asynchronous* events :title:`GhostNET` has four delegates which define callback methods that an application can assign to.



.. list-table::
   :header-rows: 1

   * - Callback
     - Description
   * - ``DLLProblemCallBack``
     - Occurs if there is some issue with the Ghostscript DLL.
   * - ``StdIOCallBack``
     - Occurs if Ghostscript outputs something to ``stderr`` or ``stdout``.
   * - ``ProgressCallBack``
     - Occurs as Ghostscript makes its way through a file.
   * - ``PageRenderedCallBack``
     - Occurs when a page has been rendered and the data from the display device is ready.



``DLLProblemCallBack``
"""""""""""""""""""""""""

.. code-block:: csharp

  internal delegate void DLLProblem(String mess);
  internal event DLLProblem DLLProblemCallBack;


``StdIOCallBack``
""""""""""""""""""""""

.. code-block:: csharp

  internal delegate void StdIO(String mess,
                               int len);
  internal event StdIO StdIOCallBack;

``ProgressCallBack``
""""""""""""""""""""""


.. code-block:: csharp

  internal delegate void Progress(gsEventArgs info);
  internal event Progress ProgressCallBack;


``PageRenderedCallBack``
""""""""""""""""""""""""""""

.. code-block:: csharp

  internal delegate void PageRendered(int width,
                                      int height,
                                      int raster,
                                   IntPtr data,
                           gsParamState_t state);
  internal event PageRendered PageRenderedCallBack;



.. _GhostNET_GetVersion:


``GetVersion``
~~~~~~~~~~~~~~

Use this method to get Ghostscript version info as a ``String``.


.. code-block:: csharp

  public String GetVersion()


**Sample code:**

.. code-block:: csharp

  String gs_vers = m_ghostscript.GetVersion();


.. note::

  An exception will be thrown if there is any issue with the Ghostscript DLL.




.. _GhostNET_DisplayDeviceOpen:

``DisplayDeviceOpen``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Sets up the :ref:`display device<Devices_Display_Devices>` ahead of time.

.. code-block:: csharp

  public gsParamState_t DisplayDeviceOpen()


**Sample code:**

.. code-block:: csharp

  m_ghostscript.DisplayDeviceOpen();


.. note::

  Calling this method instantiates Ghostscript and configures the encoding and the callbacks for the display device.



.. _GhostNET_DisplayDeviceClose:


``DisplayDeviceClose``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Closes the :ref:`display device<Devices_Display_Devices>` and deletes the instance.


.. code-block:: csharp

  public gsParamState_t DisplayDeviceClose()

**Sample code:**

.. code-block:: csharp

  m_ghostscript.DisplayDeviceClose();


.. note::

  Calling this method :ref:`deletes Ghostscript<gsapi_delete_instance>`.



.. _GhostNET_GetPageCount:


``GetPageCount``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Use this method to get the number of pages in a supplied document.


.. code-block:: csharp

  public int GetPageCount(String fileName)

**Sample code:**

.. code-block:: csharp

  int page_number = m_ghostscript.GetPageCount("my_document.pdf");

.. note::

  If Ghostscript is unable to determine the page count then this method will return ``-1``.


.. _GhostNET_CreateXPS:

``CreateXPS``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Launches a thread to create an XPS document for Windows printing. This method is :ref:`asynchronous<GhostNET_Delegates>` and logic should be hooked into your application upon :ref:`GSNET instantiation<GSNET>` to interpret progress.


.. code-block:: csharp

  public gsStatus CreateXPS(String fileName,
                               int resolution,
                               int num_pages,
                            double width,
                            double height,
                              bool fit_page,
                               int firstpage,
                               int lastpage)

**Sample code:**

.. code-block:: csharp

  m_ghostscript.CreateXPS("my_document.pdf",
                          300,
                          10,
                          1000,
                          1000,
                          true,
                          0,
                          9);


.. _GhostNET_DistillPS:


``DistillPS``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Launches a thread rendering all the pages of a supplied PostScript file to a PDF.


.. code-block:: csharp

  public gsStatus DistillPS(String fileName, int resolution)


**Sample code:**

.. code-block:: csharp

  m_ghostscript.DistillPS("my_postscript_document.ps", 300);



.. _GhostNET_DisplayDeviceRunFile:


``DisplayDeviceRunFile``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Launches a thread to run a file with the :ref:`display device<Devices_Display_Devices>`.


.. code-block:: csharp

  public gsStatus DisplayDeviceRunFile(String fileName,
                                       double zoom,
                                         bool aa, // anti-aliasing value
                                          int firstpage,
                                          int lastpage)

**Sample code:**

.. code-block:: csharp

  m_ghostscript.DisplayDeviceRunFile("my_document.pdf",
                                     1.0,
                                     true,
                                     0,
                                     9);


.. _GhostNET_DisplayDeviceRenderThumbs:

``DisplayDeviceRenderThumbs``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Launches a thread rendering all the pages with the :ref:`display device<Devices_Display_Devices>` to collect thumbnail images.

Recommended zoom level for thumbnails is between 0.05 and 0.2, additionally anti-aliasing is probably not required.


.. code-block:: csharp

  public gsStatus DisplayDeviceRenderThumbs(String fileName,
                                            double zoom,
                                              bool aa)

**Sample code:**

.. code-block:: csharp

  m_ghostscript.DisplayDeviceRenderThumbs("my_document.pdf",
                                          0.1,
                                          false);



.. _GhostNET_DisplayDeviceRenderPages:


``DisplayDeviceRenderPages``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Launches a thread rendering a set of pages with the :ref:`display device<Devices_Display_Devices>`. For use with languages that can be indexed via pages which include PDF and XPS. (see: :ref:`Ghostscript & Page Description Languages<gs_and_PDL>`)


.. code-block:: csharp

  public gsStatus DisplayDeviceRenderPages(String fileName,
                                              int first_page,
                                              int last_page,
                                           double zoom)

**Sample code:**

.. code-block:: csharp

  m_ghostscript.DisplayDeviceRenderPages("my_document.pdf",
                                         0,
                                         9,
                                         1.0);


.. _GhostNET_GetStatus:

``GetStatus``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Returns the current :ref:`status<GhostNET_Status>` of Ghostscript.


.. code-block:: csharp

  public gsStatus GetStatus()

**Sample code:**

.. code-block:: csharp

  gsStatus status = m_ghostscript.GetStatus();



.. _GhostNET_Cancel:


``Cancel``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Cancels :ref:`asynchronous<GhostNET_Delegates>` operations.


.. code-block:: csharp

  public void Cancel()


**Sample code:**

.. code-block:: csharp

  m_ghostscript.Cancel();



.. _GhostNET_GhostscriptException:


``GhostscriptException``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

An application developer can log any exceptions in this public class as required by editing the constructor.


.. code-block:: csharp

  public class GhostscriptException : Exception
  {
      public GhostscriptException(string message) : base(message)
      {
          // Report exceptions as required
      }
  }










GhostMono
=======================

:title:`GhostMono` is the C# interface into the :title:`GhostAPI` library and is developed for Linux systems.

As such :title:`GhostMono` is the Mono_ equivalent of :title:`GhostNET` with no dependency on a Windows environment.






Enums
--------


Tasks
~~~~~~~~~~~~~~~~

The Ghostscript task type ``enum`` is used to inform :title:`GhostAPI` of the type of operation which is being requested.


.. list-table::
   :header-rows: 1

   * - Task
     - Description
   * - ``PS_DISTILL``
     - Task associated with converting a PostScript stream to a PDF document.
   * - ``CREATE_XPS``
     - Task associated with outputting a copy of a document to XPS.
   * - ``SAVE_RESULT``
     - Task associated with saving documents.
   * - ``GET_PAGE_COUNT``
     - Task associated with getting the page count of a document.
   * - ``GENERIC``
     - Generic task identifier.
   * - ``DISPLAY_DEV_THUMBS``
     - Display Device task associated with rendering thumbnails.
   * - ``DISPLAY_DEV_NON_PDF``
     - Display Device task associated with non-PDF or non-XPS rendering (see: :ref:`Ghostscript & Page Description Languages<gs_and_PDL>`).
   * - ``DISPLAY_DEV_PDF``
     - Display Device task associated with PDF & XPS rendering (see: :ref:`Ghostscript & Page Description Languages<gs_and_PDL>`).
   * - ``DISPLAY_DEV_RUN_FILE``
     - Display Device task associated with running files.


Task types are defined as ``GS_Task_t``.


.. code-block:: csharp

  public enum GS_Task_t
  {
      PS_DISTILL,
      CREATE_XPS,
      SAVE_RESULT,
      GET_PAGE_COUNT,
      GENERIC,
      DISPLAY_DEV_THUMBS,
      DISPLAY_DEV_NON_PDF,
      DISPLAY_DEV_PDF,
      DISPLAY_DEV_RUN_FILE
  }







Results
~~~~~~~~~~~~~~~~

Result types are defined as ``GS_Result_t``.


.. code-block:: csharp

  public enum GS_Result_t
  {
      gsOK,
      gsFAILED,
      gsCANCELLED
  }


Status
~~~~~~~~~~~~~~~~

Status types are defined as ``gsStatus``.


.. code-block:: csharp

  public enum gsStatus
  {
      GS_READY,
      GS_BUSY,
      GS_ERROR
  }



The Parameter Struct
-------------------------

The parameter struct ``gsParamState_t`` allows for bundles of information to be processed by Ghostscript to complete overall requests.


.. code-block:: csharp

  public struct gsParamState_t
  {
      public String outputfile;
      public String inputfile;
      public GS_Task_t task;
      public GS_Result_t result;
      public int num_pages;
      public List<int> pages;
      public int firstpage;
      public int lastpage;
      public int currpage;
      public List<String> args;
      public int return_code;
      public double zoom;
  };


Parameters explained
~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Setting up your parameters (with any dedicated bespoke method(s) which your application requires) is needed when communicating directly with :title:`GhostAPI`.

When requesting Ghostscript to process an operation an application developer should send a parameter payload which defines the details for the operation.

For example in :title:`GhostMono` we can see the public method as follows:

.. code-block:: csharp

  public gsStatus DistillPS(String fileName, int resolution)
  {
      gsParamState_t gsparams = new gsParamState_t();
      gsparams.args = new List<string>();

      gsparams.inputfile = fileName;
      gsparams.args.Add("gs");
      gsparams.args.Add("-dNOPAUSE");
      gsparams.args.Add("-dBATCH");
      gsparams.args.Add("-I%rom%Resource/Init/");
      gsparams.args.Add("-dSAFER");
      gsparams.args.Add("-sDEVICE=pdfwrite");
      gsparams.outputfile = Path.GetTempFileName();
      gsparams.args.Add("-o" + gsparams.outputfile);
      gsparams.task = GS_Task_t.PS_DISTILL;

      return RunGhostscriptAsync(gsparams);
  }


Here we can see a parameter payload being setup before being passed on to the asynchronous method ``RunGhostscriptAsync`` which sets up a worker thread to run according to the task type in the payload.

:title:`GhostMono` handles many common operations on an application developer's behalf, however if you require to write your own methods to interface with :title:`GhostAPI` then referring to the public methods in :title:`GhostMono` is a good starting point.

For full documentation on parameters refer to :ref:`Ghostscript parameters in C<Use_Setting Parameters>`.




The Event class
--------------------

:title:`GhostMono` contains a public class ``gsThreadCallBack``. This class is used to set and get callback information as they occur. :title:`GhostMono` will create these payloads and deliver them back to the application layer's ``ProgressCallBack`` method asynchronously.


.. code-block:: csharp

  public class gsThreadCallBack
  {
      private bool m_completed;
      private int m_progress;
      private gsParamState_t m_param;
      public bool Completed
      {
          get { return m_completed; }
      }
      public gsParamState_t Params
      {
          get { return m_param; }
      }
      public int Progress
      {
          get { return m_progress; }
      }
      public gsThreadCallBack(bool completed, int progress, gsParamState_t param)
      {
          m_completed = completed;
          m_progress = progress;
          m_param = param;
      }
  }


GSMONO
----------

This class should be instantiated as a member variable in your application with callback definitions setup as required.

Handlers for asynchronous operations can injected by providing your own bespoke callback methods to your instance's ``ProgressCallBack`` function.


.. code-block:: csharp

  /* Set up Ghostscript with callbacks for system updates */
  m_ghostscript = new GSMONO();
  m_ghostscript.ProgressCallBack += new GSMONO.Progress(gsProgress);
  m_ghostscript.StdIOCallBack += new GSMONO.StdIO(gsIO);
  m_ghostscript.DLLProblemCallBack += new GSMONO.DLLProblem(gsDLL);
  m_ghostscript.PageRenderedCallBack += new GSMONO.PageRendered(gsPageRendered);
  m_ghostscript.DisplayDeviceOpen();

  /* example callback stubs for asynchronous operations */
  private void gsProgress(gsThreadCallBack asyncInformation)
  {
      Console.WriteLine($"gsProgress().progress:{asyncInformation.Progress}");

      if (asyncInformation.Completed) // task complete
      {
          // what was the task?
          switch (asyncInformation.Params.task)
          {
              case GS_Task_t.CREATE_XPS:
                  Console.WriteLine($"CREATE_XPS.outputfile:");
                  Console.WriteLine($"{asyncInformation.Params.result.outputfile}");
                  break;

              case GS_Task_t.PS_DISTILL:
                  Console.WriteLine($"PS_DISTILL.outputfile:");
                  Console.WriteLine($"{asyncInformation.Params.result.outputfile}");
                  break;

              case GS_Task_t.SAVE_RESULT:

                  break;

              case GS_Task_t.DISPLAY_DEV_THUMBS:

                  break;

              case GS_Task_t.DISPLAY_DEV_RUN_FILE:

                  break;

              case GS_Task_t.DISPLAY_DEV_PDF:

                  break;

              case GS_Task_t.DISPLAY_DEV_NON_PDF:

                  break;

              default:

                  break;
          }

          // task failed
          if (asyncInformation.Params.result == GS_Result_t.gsFAILED)
          {
              switch (asyncInformation.Params.task)
              {
                  case GS_Task_t.CREATE_XPS:

                      break;

                  case GS_Task_t.PS_DISTILL:

                      break;

                  case GS_Task_t.SAVE_RESULT:

                      break;

                  default:

                      break;
              }
              return;
          }

          // task cancelled
          if (asyncInformation.Params.result == GS_Result_t.gsCANCELLED)
          {

          }
      }
      else // task is still running
      {
          switch (asyncInformation.Params.task)
          {
              case GS_Task_t.CREATE_XPS:

                  break;

              case GS_Task_t.PS_DISTILL:

                  break;

              case GS_Task_t.SAVE_RESULT:

                  break;
          }
      }
  }

  private void gsIO(String message, int len)
  {
      Console.WriteLine($"gsIO().message:{message}, length:{len}");
  }

  private void gsDLL(String message)
  {
      Console.WriteLine($"gsDLL().message:{message}");
  }

  private void gsPageRendered(int width,
                              int height,
                              int raster,
                              IntPtr data,
                              gsParamState_t state)
  {

  };



.. note::

  Once a Ghostscript operation is in progress any defined callback functions will be called as the operation runs up unto completion. These callback methods are essential for your application to interpret activity events and react accordingly.


An explanation of callbacks and the available public methods within :title:`GSMONO` are explained below.




Delegates
~~~~~~~~~~~~~~

To handle *asynchronous events* :title:`GhostMONO` has four delegates which define callback methods that an application can assign to.


.. list-table::
   :header-rows: 1

   * - Callback
     - Description
   * - ``DLLProblemCallBack``
     - Occurs if there is some issue with the Ghostscript Shared Object (libgpdl.so)
   * - ``StdIOCallBack``
     - Occurs if Ghostscript outputs something to ``stderr`` or ``stdout``.
   * - ``ProgressCallBack``
     - Occurs as Ghostscript makes its way through a file.
   * - ``PageRenderedCallBack``
     - Occurs when a page has been rendered and the data from the display device is ready.



``DLLProblemCallBack``
"""""""""""""""""""""""""

.. code-block:: csharp

  internal delegate void DLLProblem(String mess);
  internal event DLLProblem DLLProblemCallBack;


``StdIOCallBack``
""""""""""""""""""""""

.. code-block:: csharp

  internal delegate void StdIO(String mess,
                               int len);
  internal event StdIO StdIOCallBack;

``ProgressCallBack``
""""""""""""""""""""""


.. code-block:: csharp

  internal delegate void Progress(gsEventArgs info);
  internal event Progress ProgressCallBack;


``PageRenderedCallBack``
""""""""""""""""""""""""""""

.. code-block:: csharp

  internal delegate void PageRendered(int width,
                                      int height,
                                      int raster,
                                   IntPtr data,
                           gsParamState_t state);
  internal event PageRendered PageRenderedCallBack;





``GetVersion``
~~~~~~~~~~~~~~

Use this method to get Ghostscript version info as a ``String``.


.. code-block:: csharp

  public String GetVersion()


**Sample code:**

.. code-block:: csharp

  String gs_vers = m_ghostscript.GetVersion();


.. note::

  An exception will be thrown if there is any issue with the Ghostscript DLL.






``DisplayDeviceOpen``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Sets up the :ref:`display device<Devices_Display_Devices>` ahead of time.

.. code-block:: csharp

  public gsParamState_t DisplayDeviceOpen()


**Sample code:**

.. code-block:: csharp

  m_ghostscript.DisplayDeviceOpen();


.. note::

  Calling this method instantiates Ghostscript and configures the encoding and the callbacks for the display device.




``DisplayDeviceClose``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Closes the :ref:`display device<Devices_Display_Devices>` and deletes the instance.


.. code-block:: csharp

  public gsParamState_t DisplayDeviceClose()

**Sample code:**

.. code-block:: csharp

  m_ghostscript.DisplayDeviceClose();


.. note::

  Calling this method :ref:`deletes Ghostscript<gsapi_delete_instance>`.




``GetPageCount``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Use this method to get the number of pages in a supplied document.


.. code-block:: csharp

  public int GetPageCount(String fileName)

**Sample code:**

.. code-block:: csharp

  int page_number = m_ghostscript.GetPageCount("my_document.pdf");

.. note::

  If Ghostscript is unable to determine the page count then this method will return ``-1``.



``DistillPS``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Launches a thread rendering all the pages of a supplied PostScript file to a PDF.


.. code-block:: csharp

  public gsStatus DistillPS(String fileName, int resolution)


**Sample code:**

.. code-block:: csharp

  m_ghostscript.DistillPS("my_postscript_document.ps", 300);



``DisplayDeviceRenderAll``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Launches a thread rendering all the document pages with the :ref:`display device<Devices_Display_Devices>`. For use with languages that can be indexed via pages which include PDF and XPS. (see: :ref:`Ghostscript & Page Description Languages<gs_and_PDL>`)


.. code-block:: csharp

  public gsStatus DisplayDeviceRenderAll(String fileName, double zoom, bool aa, GS_Task_t task)


**Sample code:**

.. code-block:: csharp

  m_ghostscript.DisplayDeviceRenderAll("my_document.pdf",
                                       0.1,
                                       false,
                                       GS_Task_t.DISPLAY_DEV_THUMBS_NON_PDF);



``DisplayDeviceRenderThumbs``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Launches a thread rendering all the pages with the :ref:`display device<Devices_Display_Devices>` to collect thumbnail images.

Recommended zoom level for thumbnails is between 0.05 and 0.2, additionally anti-aliasing is probably not required.


.. code-block:: csharp

  public gsStatus DisplayDeviceRenderThumbs(String fileName,
                                            double zoom,
                                              bool aa)

**Sample code:**

.. code-block:: csharp

  m_ghostscript.DisplayDeviceRenderThumbs("my_document.pdf",
                                          0.1,
                                          false);



``DisplayDeviceRenderPages``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Launches a thread rendering a set of pages with the :ref:`display device<Devices_Display_Devices>`. For use with languages that can be indexed via pages which include PDF and XPS. (see: :ref:`Ghostscript & Page Description Languages<gs_and_PDL>`)


.. code-block:: csharp

  public gsStatus DisplayDeviceRenderPages(String fileName,
                                              int first_page,
                                              int last_page,
                                           double zoom)

**Sample code:**

.. code-block:: csharp

  m_ghostscript.DisplayDeviceRenderPages("my_document.pdf",
                                         0,
                                         9,
                                         1.0);



``GetStatus``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Returns the current :ref:`status<GhostNET_Status>` of Ghostscript.


.. code-block:: csharp

  public gsStatus GetStatus()

**Sample code:**

.. code-block:: csharp

  gsStatus status = m_ghostscript.GetStatus();



``GhostscriptException``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

An application developer can log any exceptions in this public class as required by editing the constructor.


.. code-block:: csharp

  public class GhostscriptException : Exception
  {
      public GhostscriptException(string message) : base(message)
      {
          // Report exceptions as required
      }
  }




.. _gs_and_PDL:


.. note::

  Ghostscript & Page Description Languages

  Ghostscript handles the following `PDLs`_: PCL PDF PS XPS.

  PCL and PS do not allow random access, meaning that, to print page 2 in a 100 page document, Ghostscript has to read the entire document stream of 100 pages.

  On the other hand, PDF and XPS allow for going directly to page 2 and then only dealing with that content. The tasks ``DISPLAY_DEV_NON_PDF`` and ``DISPLAY_DEV_PDF`` keep track of what sort of input Ghostscript is dealing with and enables the application to direct progress or completion callbacks accordingly.










.. External links

.. _commercial license: https://artifex.com/licensing/commercial/
.. _.NET: https://dotnet.microsoft.com/
.. _Visual Studio: https://visualstudio.microsoft.com/
.. _Mono: https://www.mono-project.com/
.. _GhostPDL repository: https://ghostscript.com/releases/gpdldnld.html


.. _PDLs: https://en.wikipedia.org/wiki/Page_description_language

