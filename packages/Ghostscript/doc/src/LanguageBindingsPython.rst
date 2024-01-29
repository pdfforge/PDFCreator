.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: Python


.. include:: header.rst


Introduction
=================

The Python API is provided by the file :ref:`gsapi.py<python gsapi>` - this is the binding to the :ref:`Ghostscript C library<API.html exported functions>`.

In the :title:`GhostPDL` repository sample Python examples can be found in ``/demos/python/examples.py``.


Platform & setup
=======================


Ghostscript should be built as a shared library for your platform.

See :ref:`Building Ghostscript<Make.html>`.



Specifying the Ghostscript shared library
-------------------------------------------------

Two environmental variables can be used to specify where to find the Ghostscript shared library.

``GSAPI_LIB`` sets the exact path of the Ghostscript shared library, otherwise, ``GSAPI_LIBDIR`` sets the directory containing the Ghostscript shared library.

If neither is defined we will use the OS's default location(s) for shared libraries.

If ``GSAPI_LIB`` is not defined, the leafname of the shared library is inferred from the OS type:


.. list-table::
   :header-rows: 1

   * - Platform
     - Shared library file
   * - Windows
     - ``gsdll64.dll``
   * - MacOS
     - ``libgs.dylib``
   * - Linux / OpenBSD
     - ``libgs.so``


API test
------------

The ``gsapi.py`` file that provides the Python bindings can also be used to test the bindings, by running it directly.

Assuming that your Ghostscript library has successfully been created, then from the root of your ``ghostpdl`` repository checkout do the following:


**Windows**


   Run ``gsapi.py`` as a test script in a ``cmd.exe`` window:

   .. code-block:: bash

      set GSAPI_LIBDIR=debugbin&& python ./demos/python/gsapi.py


   Run ``gsapi.py`` as a test script in a ``PowerShell`` window:

   .. code-block:: bash

      cmd /C "set GSAPI_LIBDIR=debugbin&& python ./demos/python/gsapi.py"


**Linux/OpenBSD/MacOS**


   Run ``gsapi.py`` as a test script:

   .. code-block:: bash

      GSAPI_LIBDIR=sodebugbin ./demos/python/gsapi.py


.. note::

   If there are no errors then this will have validated that the Ghostscript library is present & operational.


.. _python gsapi:


The gsapi Python module
=====================================

Assuming that the above platform & setup has been completed, then ``gsapi`` should be imported into your own Python scripts for API usage.


- Implemented using Python's ``ctypes`` module.

- All functions have the same name as the C function that they wrap.

- Functions raise a ``GSError`` exception if the underlying function returned a negative error code.

- Functions that don't have out-params return ``None``. Out-params are returned directly (using tuples if there are more than one).



.. _python gsapi_revision:


``gsapi_revision()``
---------------------------------------------------------------

Returns a ``gsapi_revision_t``.

This method returns the revision numbers and strings of the Ghostscript interpreter library; you should call it before any other interpreter library functions to make sure that the correct version of the Ghostscript interpreter has been loaded.


**Sample code**:

.. code-block:: python

   version_info = gsapi.gsapi_revision()
   print(version_info)


**C code reference**: :ref:`gsapi_revision<API.html gsapi_revision>`



.. _python gsapi_new_instance:


``gsapi_new_instance(caller_handle)``
---------------------------------------------------------------


Returns a new instance of Ghostscript to be used with other :ref:`gsapi_*()<python gsapi_run_asterisk>` functions.


**Parameters:**

``caller_handle``
   Typically unused, but is passed to callbacks e.g. via :ref:`gsapi_set_stdio()<python gsapi_set_stdio>`. Must be convertible to a ``C void*``, so ``None`` or an ``integer`` is ok but other types such as strings will fail.


**Sample code**:

.. code-block:: python

   instance = gsapi.gsapi_new_instance(1)


**C code reference**: :ref:`gsapi_new_instance<API.html gsapi_new_instance>`


.. _python gsapi_delete_instance:


``gsapi_delete_instance(instance)``
---------------------------------------------------------------

Destroy an instance of Ghostscript. Before you call this, Ghostscript should ensure to have finished any processes.

**Parameters:**

``instance``
   Your instance of Ghostscript.

**Sample code**:

.. code-block:: python

   gsapi.gsapi_delete_instance(instance)


**C code reference**: :ref:`gsapi_delete_instance<API.html gsapi_delete_instance>`

.. _python gsapi_set_stdio:


``gsapi_set_stdio(instance, stdin_fn, stdout_fn, stderr_fn)``
---------------------------------------------------------------

Set the callback functions for ``stdio``, together with the handle to use in the callback functions.



**Parameters:**

``instance``
   Your instance of Ghostscript.

``stdin_fn``
   If not ``None``, will be called with:

   - ``(caller_handle, text, len_)``:
      - ``caller_handle``: As passed originally to ``gsapi_new_instance()``.
      - ``text``: A ``ctypes.LP_c_char`` of length ``len_``.


``stdout_fn`` , ``stderr_fn``
   If not ``None``, called with:

   - ``(caller_handle, text)``:
      - ``caller_handle``: As passed originally to ``gsapi_new_instance()``.
      - ``text``: A Python bytes object.


Should return the number of bytes of text that they handled; for convenience ``None`` is converted to ``len(text)``.


**Sample code**:

.. code-block:: python

   def stdout_fn(caller_handle, bytes_):
      sys.stdout.write(bytes_.decode('latin-1'))

   gsapi.gsapi_set_stdio(instance, None, stdout_fn, None)
   print('gsapi_set_stdio() ok.')


**C code reference**: :ref:`gsapi_set_stdio<API.html gsapi_set_stdio>`

.. _python gsapi_set_poll:

``gsapi_set_poll(instance, poll_fn)``
---------------------------------------------------------------

Set the callback function for polling.

**Parameters:**

``instance``
   Your instance of Ghostscript.

``poll_fn``
   Will be called with ``caller_handle`` as passed to :ref:`gsapi_new_instance<python gsapi_new_instance>`.


**Sample code**:

.. code-block:: python

   def poll_fn(caller_handle, bytes_):
       sys.stdout.write(bytes_.decode('latin-1'))

   gsapi.gsapi_set_poll(instance, poll_fn)
   print('gsapi_set_poll() ok.')


**C code reference**: :ref:`gsapi_set_poll<API.html gsapi_set_poll>`

.. _python gsapi_set_display_callback:

``gsapi_set_display_callback(instance, callback)``
---------------------------------------------------------------

Sets the display callback.

**Parameters:**

``instance``
   Your instance of Ghostscript.

``callback``
   Must be a ``display_callback`` instance.

**Sample code**:

.. code-block:: python

   d = display_callback()
   gsapi.gsapi_set_display_callback(instance, d)
   print('gsapi_set_display_callback() ok.')


**C code reference**: :ref:`gsapi_set_display_callback<API.html gsapi_set_display_callback>`

.. _python gsapi_set_arg_encoding:

``gsapi_set_arg_encoding(instance, encoding)``
---------------------------------------------------------------

Set the encoding used for the interpretation of all subsequent arguments supplied via the :title:`GhostAPI` interface on this instance. By default we expect args to be in encoding ``0`` (the 'local' encoding for this OS). On Windows this means "the currently selected codepage". On Linux this typically means utf8. This means that omitting to call this function will leave Ghostscript running exactly as it always has.

This must be called after :ref:`gsapi_new_instance<python gsapi_new_instance>` and before :ref:`gsapi_init_with_args<python gsapi_init_with_args>`.

**Parameters:**

``instance``
   Your instance of Ghostscript.

``encoding``
   Encoding must be one of:

   .. list-table::
      :header-rows: 1

      * - Encoding enum
        - Value
      * - ``GS_ARG_ENCODING_LOCAL``
        - 0
      * - ``GS_ARG_ENCODING_UTF8``
        - 1
      * - ``GS_ARG_ENCODING_UTF16LE``
        - 2

**Sample code**:

.. code-block:: python

   gsapi.gsapi_set_arg_encoding(instance, gsapi.GS_ARG_ENCODING_UTF8)


.. note::

   Please note that use of the 'local' encoding (``GS_ARG_ENCODING_LOCAL``) is now deprecated and should be avoided in new code.



**C code reference**: :ref:`gsapi_set_arg_encoding<API.html gsapi_set_arg_encoding>`


.. _python gsapi_set_default_device_list:

``gsapi_set_default_device_list(instance, list_)``
---------------------------------------------------------------

Set the string containing the list of default device names, for example "display x11alpha x11 bbox". Allows the calling application to influence which device(s) Ghostscript will try, in order, in its selection of the default device. This must be called after :ref:`gsapi_new_instance<python gsapi_new_instance>` and before :ref:`gsapi_init_with_args<python gsapi_init_with_args>`.


**Parameters:**

``instance``
   Your instance of Ghostscript.

``list_``
   A string of device names.


**Sample code**:

.. code-block:: python

   gsapi.gsapi_set_default_device_list(instance, 'bmp256 bmp32b bmpgray cdeskjet cdj1600 cdj500')



**C code reference**: :ref:`gsapi_set_default_device_list<API.html gsapi_set_default_device_list>`


.. _python gsapi_get_default_device_list:

``gsapi_get_default_device_list(instance)``
---------------------------------------------------------------

Returns a string containing the list of default device names. This must be called after :ref:`gsapi_new_instance<python gsapi_new_instance>` and before :ref:`gsapi_init_with_args<python gsapi_init_with_args>`.


**Parameters:**

``instance``
   Your instance of Ghostscript.


**Sample code**:

.. code-block:: python

   device_list = gsapi.gsapi_get_default_device_list(instance)
   print(device_list)


**C code reference**: :ref:`gsapi_get_default_device_list<API.html gsapi_get_default_device_list>`


.. _python gsapi_init_with_args:

``gsapi_init_with_args(instance, args)``
---------------------------------------------------------------

To initialise the interpreter, pass your ``instance`` of Ghostscript and your argument variables with ``args``.


**Parameters:**

``instance``
   Your instance of Ghostscript.

``args``
   A list/tuple of strings.


**Sample code**:

.. code-block:: python

   in_filename = 'tiger.eps'
   out_filename = 'tiger.pdf'
   params = ['gs', '-dNOPAUSE', '-dBATCH', '-sDEVICE=pdfwrite',
             '-o', out_filename, '-f', in_filename]
   gsapi.gsapi_init_with_args(instance, params)

**C code reference**: :ref:`gsapi_init_with_args<API.html gsapi_init_with_args>`

.. _python gsapi_run_asterisk:

``gsapi_run_*``
---------------------------------------------------------------

There is a 64 KB length limit on any buffer submitted to a :ref:`gsapi_run_*<python gsapi_run_asterisk>` function for processing. If you have more than 65535 bytes of input then you must split it into smaller pieces and submit each in a separate :ref:`gsapi_run_string_continue<python gsapi_run_string_continue>` call.

On success (underlying C function's return value is ``>=0``), these functions return the underlying C function's ``exit_code`` out-parameter (and the return value is discarded). Otherwise they raise a ``GSError`` in the usual way (and the underlying ``exit_code`` out-parameter is discarded).

For full details on these return codes please see :ref:`The C API return codes<API_Return codes>`.


.. _User errors parameter explained:


.. note::

   **User errors parameter explained**

   The ``user_errors`` argument is normally set to zero to indicate that errors should be handled through the normal mechanisms within the interpreted code. If set to a negative value, the functions will return an error code directly to the caller, bypassing the interpreted language. The interpreted language's error handler is bypassed, regardless of ``user_errors`` parameter, for the ``gs_error_interrupt`` generated when the polling callback returns a negative value. A positive ``user_errors`` is treated the same as zero.


**C code reference**: :ref:`gsapi_run_*<API.html gsapi_run_asterisk>`

.. _python gsapi_run_string_begin:

``gsapi_run_string_begin(instance, user_errors)``
---------------------------------------------------------------

Starts a ``run_string_`` operation.


**Parameters:**

``instance``
   Your instance of Ghostscript.

``user_errors``
   An ``int``, for more see: `User errors parameter explained`_.

**Sample code**:

.. code-block:: python

   exitcode = gsapi.gsapi_run_string_begin(instance, 0)

**C code reference**: :ref:`gsapi_run_*<API.html gsapi_run_asterisk>`


.. _python gsapi_run_string_continue:


``gsapi_run_string_continue(instance, str_, user_errors)``
---------------------------------------------------------------

Processes file byte data (``str_``) to feed as chunks into Ghostscript. This method should typically be called within a buffer context.

.. note::

   An exception is not raised for the ``gs_error_NeedInput`` return code.


**Parameters:**

``instance``
   Your instance of Ghostscript.

``str_``
   Should be either a Python string or a bytes object. If the former, it is converted into a bytes object using utf-8 encoding.

``user_errors``
   An ``int``, for more see: `User errors parameter explained`_.


**Sample code**:

.. code-block:: python

   exitcode = gsapi.gsapi_run_string_continue(instance, data, 0)


.. note::

   For the return code, we don't raise an exception for ``gs_error_NeedInput``.


**C code reference**: :ref:`gsapi_run_*<API.html gsapi_run_asterisk>`


.. _python gsapi_run_string_with_length:


``gsapi_run_string_with_length(instance, str_, length, user_errors)``
------------------------------------------------------------------------------------------------------------------------------

Processes file byte data (``str_``) to feed into Ghostscript when the length is known and the file byte data is immediately available.


**Parameters:**

``instance``
   Your instance of Ghostscript.

``str_``
   Should be either a Python string or a bytes object. If the former, it is converted into a bytes object using utf-8 encoding.

``length``
   An ``int`` representing the length of ``str_``.

``user_errors``
   An ``int``, for more see: `User errors parameter explained`_.

**Sample code**:

.. code-block:: python

   gsapi.gsapi_run_string_with_length(instance,"hello",5,0)


.. note::

   If using this method then ensure that the file byte data will fit into a single (<64k) buffer.


**C code reference**: :ref:`gsapi_run_*<API.html gsapi_run_asterisk>`


.. _python gsapi_run_string:


``gsapi_run_string(instance, str_, user_errors)``
---------------------------------------------------------------

Processes file byte data (``str_``) to feed into Ghostscript.


**Parameters:**

``instance``
   Your instance of Ghostscript.

``str_``
   Should be either a Python string or a bytes object. If the former, it is converted into a bytes object using utf-8 encoding.

``user_errors``
   An ``int``, for more see: `User errors parameter explained`_.

**Sample code**:

.. code-block:: python

   gsapi.gsapi_run_string(instance,"hello",0)

.. note::

   This method can only work on a standard, null terminated C string.


**C code reference**: :ref:`gsapi_run_*<API.html gsapi_run_asterisk>`


.. _python gsapi_run_string_end:


``gsapi_run_string_end(instance, user_errors)``
---------------------------------------------------------------

Ends a ``run_string_`` operation.


**Parameters:**

``instance``
   Your instance of Ghostscript.

``user_errors``
   An ``int``, for more see: `User errors parameter explained`_.



**Sample code**:

.. code-block:: python

   exitcode = gsapi.gsapi_run_string_end(instance, 0)


**C code reference**: :ref:`gsapi_run_*<API.html gsapi_run_asterisk>`

.. _python gsapi_run_file:


``gsapi_run_file(instance, filename, user_errors)``
---------------------------------------------------------------

Runs a file through Ghostscript.


**Parameters:**

``instance``
   Your instance of Ghostscript.

``filename``
   String representing file name.

``user_errors``
   An ``int``, for more see: `User errors parameter explained`_.

**Sample code**:

.. code-block:: python

   in_filename = 'tiger.eps'
   gsapi.gsapi_run_file(instance, in_filename, 0)


.. note::

   This will process the supplied input file with any previously supplied argument parameters.


**C code reference**: :ref:`gsapi_run_*<API.html gsapi_run_asterisk>`


.. _python gsapi_exit:

``gsapi_exit(instance)``
---------------------------------------------------------------

Returns a successful exit code ``0``, or raises a ``GSError`` exception on error.


Exit the interpreter. This must be called on shutdown if :ref:`gsapi_init_with_args<python gsapi_init_with_args>` has been called, and just before :ref:`gsapi_delete_instance<python gsapi_delete_instance>`.


**Parameters:**

``instance``
   Your instance of Ghostscript.

**Sample code**:

.. code-block:: python

   gsapi.gsapi_exit(instance)



**C code reference**: :ref:`gsapi_exit<API.html gsapi_exit>`


.. _python gsapi_set_param:

``gsapi_set_param(instance, param, value, type_=None)``
---------------------------------------------------------------

Sets a parameter.

We behave much like the underlying ``gsapi_set_param()`` C function, except that we also support automatic inference of type ``type_`` arg by looking at the type of ``value``.


**Parameters:**

``instance``
   Your instance of Ghostscript.

``param``
   Name of parameter, either a ``bytes`` or a ``str``; if ``str`` it is encoded using ``latin-1``.

``value``
   A ``bool``, ``int``, ``float``, ``bytes`` or ``str``. If ``str``, it is encoded into a ``bytes`` using ``utf-8``.

``type_``
   If ``type_`` is not ``None``, ``value`` must be convertible to the Python type implied by ``type_``:


   .. list-table::
      :header-rows: 1

      * - ``type_``
        - Python type(s)
      * - ``gs_spt_null``
        - [Ignored]
      * - ``gs_spt_bool``
        - bool
      * - ``gs_spt_int``
        - int
      * - ``gs_spt_float``
        - float
      * - ``gs_spt_name``
        - [Error]
      * - ``gs_spt_string``
        - (bytes, str)
      * - ``gs_spt_long``
        - int
      * - ``gs_spt_i64``
        - int
      * - ``gs_spt_size_t``
        - int
      * - ``gs_spt_parsed``
        - (bytes, str)
      * - ``gs_spt_more_to_come``
        - (bytes, str)

   An exception is raised if ``type_`` is an integer type and ``value`` is outside its range.

   If ``type_`` is ``None``, we choose something suitable for type of ``value``:

   .. list-table::
      :header-rows: 1

      * - Python type of ``value``
        - ``type_``
      * - bool
        - ``gs_spt_bool``
      * - int
        - ``gs_spt_i64``
      * - float
        - ``gs_spt_float``
      * - bytes
        - ``gs_spt_parsed``
      * - str
        - ``gs_spt_parsed`` (encoded with utf-8)


   If value is ``None``, we use ``gs_spt_null``.

   Otherwise ``type_`` must be a ``gs_spt_*`` except for ``gs_spt_invalid`` and ``gs_spt_name`` (we don't allow ``psapi_spt_name`` because the underlying C does not copy the string, so cannot be safely used from Python).



**Sample code**:

.. code-block:: python

   set_margins = gsapi.gsapi_set_param(instance, "Margins", "[10 10]")



**C code reference**: :ref:`gsapi_set_param<API.html gsapi_set_param>`


.. _python gsapi_get_param:

``gsapi_get_param(instance, param, type_=None, encoding=None)``
------------------------------------------------------------------------------------------------------------------------------

Returns value of specified parameter, or ``None`` if parameter ``type`` is ``gs_spt_null``.


**Parameters:**

``instance``
   Your instance of Ghostscript.

``param``
   Name of parameter, either a ``bytes`` or ``str``; if a ``str`` it is encoded using ``latin-1``.

``type_``
   A ``gs_spt_*`` constant or ``None``. If ``None`` we try each ``gs_spt_*`` until one succeeds; if none succeeds we raise the last error.

``encoding``
   Only affects string values. If ``None`` we return a ``bytes`` object, otherwise it should be the encoding to use to decode into a string, e.g. 'utf-8'.


**Sample code**:

.. code-block:: python

   get_margins = gsapi.gsapi_get_param(instance, "Margins")


**C code reference**: :ref:`gsapi_get_param<API.html gsapi_get_param>`

.. _python gsapi_enumerate_params:


``gsapi_enumerate_params(instance)``
---------------------------------------------------------------

Enumerate the current parameters on the instance of Ghostscript.

Yields ``(key, value)`` for each ``param``. ``key`` is decoded as ``latin-1``.


**Parameters:**

``instance``
   Your instance of Ghostscript.


**Sample code**:

.. code-block:: python

   for param, type_ in gsapi.gsapi_enumerate_params(instance):
       val = gsapi.gsapi_get_param(instance,param, encoding='utf-8')
       print('%-24s : %s' % (param, val))


**C code reference**: :ref:`gsapi_enumerate_params<API.html gsapi_enumerate_params>`

.. _python gsapi_add_control_path:

``gsapi_add_control_path(instance, type_, path)``
---------------------------------------------------------------

Add a (case sensitive) path to one of the lists of :ref:`permitted paths<Use Safer>` for file access.



**Parameters:**

``instance``
   Your instance of Ghostscript.

``type_``
   An ``int`` which must be one of:

   .. list-table::
      :header-rows: 1

      * - Enum
        - Value
      * - ``GS_PERMIT_FILE_READING``
        - 0
      * - ``GS_PERMIT_FILE_WRITING``
        - 1
      * - ``GS_PERMIT_FILE_CONTROL``
        - 2

``path``
   A string representing the file path.

**Sample code**:

.. code-block:: python

   gsapi.gsapi_add_control_path(instance, gsapi.GS_PERMIT_FILE_READING, "/docs/secure/")


**C code reference**: :ref:`gsapi_add_control_path<API.html gsapi_add_control_path>`


.. _python gsapi_remove_control_path:


``gsapi_remove_control_path(instance, type_, path)``
---------------------------------------------------------------

Remove a (case sensitive) path from one of the lists of :ref:`permitted paths<Use Safer>` for file access.



**Parameters:**

``instance``
   Your instance of Ghostscript.

``type_``
   An int representing the permission type.

``path``
   A string representing the file path.


**Sample code**:

.. code-block:: python

   gsapi.gsapi_remove_control_path(instance, gsapi.GS_PERMIT_FILE_READING, "/docs/secure/")


**C code reference**: :ref:`gsapi_remove_control_path<API.html gsapi_remove_control_path>`

.. _python gsapi_purge_control_paths:

``gsapi_purge_control_paths(instance, type_)``
---------------------------------------------------------------


Clear all the paths from one of the lists of :ref:`permitted paths<Use Safer>` for file access.


**Parameters:**

``instance``
   Your instance of Ghostscript.

``type_``
   An ``int`` representing the permission type.

**Sample code**:

.. code-block:: python

   gsapi.gsapi_purge_control_paths(instance, gsapi.GS_PERMIT_FILE_READING)


**C code reference**: :ref:`gsapi_purge_control_paths<API.html gsapi_purge_control_paths>`


.. _python gsapi_activate_path_control:

``gsapi_activate_path_control(instance, enable)``
---------------------------------------------------------------

Enable/Disable path control (i.e. whether paths are checked against :ref:`permitted paths<Use Safer>` before access is granted).


**Parameters:**

``instance``
   Your instance of Ghostscript.

``enable``
   ``bool`` to enable/disable path control.


**Sample code**:

.. code-block:: python

   gsapi.gsapi_activate_path_control(instance, true)



**C code reference**: :ref:`gsapi_activate_path_control<API.html gsapi_activate_path_control>`

.. _python gsapi_is_path_control_active:


``gsapi_is_path_control_active(instance)``
---------------------------------------------------------------

Query whether path control is activated or not.


**Parameters:**

``instance``
   Your instance of Ghostscript.


**Sample code**:

.. code-block:: python

   isActive = gsapi.gsapi_is_path_control_active(instance)


**C code reference**: :ref:`gsapi_is_path_control_active<API.html gsapi_is_path_control_active>`






