.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: Java


.. include:: header.rst

Introduction
==============

In the ``GhostPDL`` repository sample Java projects can be found in ``/demos/java``.

Within this location the following folders are of relevance:

- :ref:`jni<jni: Building the Java Native Interface>` ``jni``
- :ref:`gsjava<java gsjava>` ``gsjava``
- :ref:`gstest<java gstest>` ``gstest``
- :ref:`gsviewer<java gsviewer>` ``gsviewer``



Platform & setup
=======================


Ghostscript should be built as a shared library for your platform.

See :ref:`Building Ghostscript<Make.html>`.



jni: Building the Java Native Interface
--------------------------------------------------

Before building the JNI ensure that Ghostscript has already been built for your platform and that you have JDK installed.

The JNI is for use in the Java interface, this object must be placed somewhere on your Java ``PATH``. On Windows, the DLL can be placed in the working directory, next to ``gsjava.jar``.



.. list-table::
   :header-rows: 1

   * - Platform
     - JNI file
   * - Windows
     - ``gs_jni.dll``
   * - MacOS
     - ``gs_jni.dylib``
   * - Linux / OpenBSD
     - ``gs_jni.so``


Preparing your include folder
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The build scripts require the header ``jni.h``, which defines all JNI functions, and ``jni_md.h``, which defines all system-specific integer types. The build scripts expect an include folder relative to their location which contain these header files from your system.

These headers are typically found in the following directories:


.. list-table::
   :header-rows: 1

   * - Platform
     - jni.h
     - jni_md.h
   * - Windows
     - C:\Program Files\Java\<JDK Install>\include\jni.h
     - C:\Program Files\Java\<JDK Install>\include\win32\jni_md.h
   * - MacOS
     - /Library/Java/JavaVirtualMachines/<JDK Install>/Contents/Home/include/jni.h
     - /Library/Java/JavaVirtualMachines/<JDK Install>/Contents/Home/include/darwin/jni_md.h
   * - Linux
     - /lib/jvm/<JDK Install>/include/jni.h
     - /lib/jvm/<JDK Install>/include/linux/jni_md.h



Once your ``include`` folder has been located folder you can copy it and place it in your ``ghostpdl/demos/java/jni/gs_jni`` folder.

Your build scripts should now be ready to run as they will be able to find the required JNI header files in their own relative include folder.

Building on Windows
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The ``jni`` folder contains a Visual Studio Solution file ``/jni/gs_jni/gs_jni.sln`` which you should use to build the required JNI ``gs_jni.dll`` library file.

With the project open in Visual Studio, select the required architecture from the drop down - then right click on 'gs_jni' in the solution explorer and choose "Build".


Building on MacOS
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

On your command line, navigate to ``ghostpdl/demos/java/jni/gs_jni`` and ensure that the build script is executable and then run it, with:

.. code-block:: bash

   chmod +x build_darwin.sh
   ./build_darwin.sh


Building on Linux
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

On your command line, navigate to ``ghostpdl/demos/java/jni/gs_jni`` and ensure that the build script is executable and then run it, with:

.. code-block:: bash

   chmod +x build_linux.sh
   ./build_linux.sh



gsjava: Building the JAR
---------------------------------

``gsjava.jar`` is the Java library which contains classes and interfaces which enable API calls required to use Ghostscript.

Assuming that the JAR for your project has been built and properly linked with your own project then the Ghostscript API should be available by importing the required classes within your project's ``.java`` files.



Building with the command line
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Navigate to ``ghostpdl/demos/java/gsjava`` and use the following:


.. list-table::
   :header-rows: 1

   * - Platform
     - Run file
   * - Windows
     - build_win32.bat
   * - MacOS
     - build_darwin.sh
   * - Linux
     - build_linux.sh

.. note::

   ``gsjava`` has a dependency on ``jni``, please ensure that :ref:`gs_jni<jni: Building the Java Native Interface>` is able to be built beforehand.



Building with Eclipse
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Alternatively you can use Eclipse_ to build the JAR file.

Using Eclipse_ import the source folder ``gsjava`` as a project and select ``Export > Java > JAR File`` as shown in the screenshot example below:


.. note we embedd the image with raw HTML because Sphinx is incapable of doing percentage style widths ... :(

.. raw:: html

   <img src="_static/export-jar.png" width=70%/>




Linking the JAR
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The built JAR should be properly linked within your project Java Build Path as follows:


.. raw:: html

   <img src="_static/linking-jar.png" width=80%/>



Demo projects
==================

.. _java gstest:

gstest
----------------------------------------

Use this to quickly test Ghostscript methods.

This project can be opened in Eclipse and used to test the Ghostscript API. The sample here simply sets up an instance of Ghostscript and then sets and gets some parameters accordingly.



.. _java gsviewer:

gsviewer
------------------------------------

A handy file viewer.

This project can be used to test the Ghostscript API alongside a UI which handles opening PostScript and PDF files. The sample application here allows for file browsing and Ghostscript file viewing.

Below is a screenshot of the sample application with a PDF open:


.. raw:: html

   <img src="_static/gsviewer.png" width=100%/>



To run the project navigate to the ``demos/java/gsviewer`` location and ensure that the required libraries are in the directory:


.. list-table::
   :header-rows: 1
   :widths: 15 70 15

   * - Platform
     - Ghostscript library file
     - JNI library file
   * - Windows
     - ``gpdldll64.dll``
     - ``gs_jni.dll``
   * - MacOS
     - ``libgpdl.dylib``
     - ``gs_jni.dylib``
   * - Linux / OpenBSD
     - ``libgpdl.so`` (this may have been built as ``libgs.so``,

       so it should be copied into this directory and

       renamed to ``libgpdl.so``)
     - ``gs_jni.so``


Building on Windows
~~~~~~~~~~~~~~~~~~~~~~~

Run the ``build_win32.bat`` script.

Running on Windows
~~~~~~~~~~~~~~~~~~~~~~~

To run, open ``gsviewer.jar`` either through File Explorer or in the command line through the following command:


.. code-block:: bash

   java -jar gsviewer.jar


Building on MacOS
~~~~~~~~~~~~~~~~~~~~~~~

On your command line, navigate to ``ghostpdl/demos/java/gsviewer`` and ensure that the build script is executable and then run it, with:

.. code-block:: bash

   chmod +x build_darwin.sh
   ./build_darwin.sh

This will automatically ``build gs_jni.dylib`` (in the ``ghostpdl/demos/java/jni/gs_jni/`` location) and ``gsjava.jar`` ``gsviewer.jar`` in the ``gsviewer`` directory.


Running on MacOS
~~~~~~~~~~~~~~~~~~~~~~~

Ensure that the Ghostscript library exists in the ``gsviewer`` directory. (Copy and move the built library from ``ghostpdl/sobin`` as required).

Ensure that the run script is executable and then run it, with:

.. code-block:: bash

   chmod +x start_darwin.sh
   ./start_darwin.sh


Building on Linux
~~~~~~~~~~~~~~~~~~~~~~~

On your command line, navigate to ``ghostpdl/demos/java/gsviewer`` and ensure that the build script is executable and then run it, with:

.. code-block:: bash

   chmod +x build_linux.sh
   ./build_linux.sh

This will automatically build ``gs_jni.so`` (in the ``ghostpdl/demos/java/jni/gs_jni/`` location) and ``gsjava.jar`` ``gsviewer.jar`` in the ``gsviewer`` directory.


.. note::

   On Linux, when using OpenJDK, the property "assistive_technologies" may need to be modified for the Java code to build. It can be modified by editing the "accessibility.properties" file. This is located at:

   ``/etc/java-8-openjdk/accessibility.properties``


Running on Linux
~~~~~~~~~~~~~~~~~~~~~~~

Ensure that the Ghostscript library exists in the ``gsviewer`` directory. (Copy and move the built library from ``ghostpdl/sobin`` as required).

Ensure that the run script is executable and then run it, with:

.. code-block:: bash

   chmod +x start_linux.sh
   ./start_linux.sh



.. _java gsjava:

Using the Java library
==================================


gsjava
----------

There are two main classes within the ``gsjava.jar`` library to consider:


GSAPI & GSInstance
---------------------

:title:`GSAPI` is the main Ghostscript API class which bridges into the :ref:`Ghostscript C library<API.html exported functions>`.

:title:`GSInstance` is a wrapper class for :title:`GSAPI` which encapsulates an instance of Ghostscript and allows for simpler API calls.


**Sample code:**


.. code-block:: java

   // to use GSAPI
   import static com.artifex.gsjava.GSAPI.*;

   // to use GSInstance
   import com.artifex.gsjava.GSInstance;


GSAPI
--------


.. _java gsapi_revision:

``gsapi_revision``
~~~~~~~~~~~~~~~~~~~~

This method returns the revision numbers and strings of the Ghostscript interpreter library; you should call it before any other interpreter library functions to make sure that the correct version of the Ghostscript interpreter has been loaded.


.. code-block:: java

   public static native int gsapi_revision(GSAPI.Revision revision,
                                                      int len);

.. note::

   The method should write to a reference variable which conforms to the class ``GSAPI.Revision``.

``GSAPI.Revision``
""""""""""""""""""""""

This class is used to store information about Ghostscript and provides handy getters for the product and the copyright information.



.. code-block:: java

   public static class Revision {
       public volatile byte[] product;
       public volatile byte[] copyright;
       public volatile long revision;
       public volatile long revisionDate;

       public Revision() {
           this.product = null;
           this.copyright = null;
           this.revision = 0L;
           this.revisionDate = 0L;
       }

       /**
        * Returns the product information as a String.
        *
        * @return The product information.
        */
       public String getProduct() {
           return new String(product);
       }

       /**
        * Returns the copyright information as a String.
        *
        * @return The copyright information.
        */
       public String getCopyright() {
           return new String(copyright);
       }
   }


.. _java gsapi_new_instance:

``gsapi_new_instance``
~~~~~~~~~~~~~~~~~~~~~~~

Creates a new instance of Ghostscript. This instance is passed to most other :title:`GSAPI` methods. Unless Ghostscript has been compiled with the ``GS_THREADSAFE`` define, only one instance at a time is supported.

.. code-block:: java

   public static native int gsapi_new_instance(Reference<Long> instance,
                                                          long callerHandle);

.. note::

   The method returns a reference which represents your instance of Ghostscript.


.. _java gsapi_delete_instance:


``gsapi_delete_instance``
~~~~~~~~~~~~~~~~~~~~~~~~~~~

Destroy an instance of Ghostscript. Before you call this, Ghostscript must have finished. If Ghostscript has been initialised, you should call :ref:`gsapi_exit<java gsapi_exit>` beforehand.


.. code-block:: java

   public static native void gsapi_delete_instance(long instance);



.. _java gsapi_set_stdio_with_handle:

``gsapi_set_stdio_with_handle``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Set the callback functions for ``stdio``, together with the handle to use in the callback functions. The ``stdin`` callback function should return the number of characters read, ``0`` for ``EOF``, or ``-1`` for ``error``. The ``stdout`` and ``stderr`` callback functions should return the number of characters written.


.. code-block:: java

   public static native int gsapi_set_stdio_with_handle(long instance,
                                              IStdInFunction stdin,
                                             IStdOutFunction stdout,
                                             IStdErrFunction stderr,
                                                        long callerHandle);


.. _java gsapi_set_stdio:

``gsapi_set_stdio``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Set the callback functions for ``stdio``. The handle used in the callbacks will be taken from the value passed to :ref:`gsapi_new_instance<java gsapi_new_instance>`. Otherwise the behaviour of this function matches :ref:`gsapi_set_stdio_with_handle<java gsapi_set_stdio_with_handle>`.


.. code-block:: java

   public static native int gsapi_set_stdio(long instance,
                                  IStdInFunction stdin,
                                 IStdOutFunction stdout,
                                 IStdErrFunction stderr);




.. _java gsapi_set_poll_with_handle:

``gsapi_set_poll_with_handle``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Set the callback function for polling, together with the handle to pass to the callback function. This function will only be called if the Ghostscript interpreter was compiled with ``CHECK_INTERRUPTS`` as described in ``gpcheck.h``.

The polling function should return zero if all is well, and return negative if it wants Ghostscript to abort. This is often used for checking for a user cancel. This can also be used for handling window events or cooperative multitasking.

The polling function is called very frequently during interpretation and rendering so it must be fast. If the function is slow, then using a counter to ``return 0`` immediately some number of times can be used to reduce the performance impact.

.. code-block:: java

   public static native int gsapi_set_poll_with_handle(long instance,
                                              IPollFunction pollfun,
                                                       long callerHandle);



.. _java gsapi_set_poll:

``gsapi_set_poll``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Set the callback function for polling. The handle passed to the callback function will be taken from the handle passed to :ref:`gsapi_new_instance<java gsapi_new_instance>`. Otherwise the behaviour of this function matches :ref:`gsapi_set_poll_with_handle<java gsapi_set_poll_with_handle>`.

.. code-block:: java

   public static native int gsapi_set_poll(long instance,
                                  IPollFunction pollfun);


.. _java gsapi_set_display_callback:

``gsapi_set_display_callback``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

This call is deprecated; please use :ref:`gsapi_register_callout<java gsapi_register_callout>` to register a callout handler for the display device in preference.


.. code-block:: java

   public static native int gsapi_set_display_callback(long instance,
                                            DisplayCallback displayCallback);


.. _java gsapi_register_callout:

``gsapi_register_callout``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

This call registers a callout handler.

.. code-block:: java

   public static native int gsapi_register_callout(long instance,
                                       ICalloutFunction callout,
                                                   long calloutHandle);


.. _java gsapi_deregister_callout:

``gsapi_deregister_callout``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

This call deregisters a callout handler previously registered with :ref:`gsapi_register_callout<java gsapi_register_callout>`. All three arguments must match exactly for the callout handler to be deregistered.

.. code-block:: java

   public static native void gsapi_deregister_callout(long instance,
                                          ICalloutFunction callout,
                                                      long calloutHandle);


.. _java gsapi_set_arg_encoding:

``gsapi_set_arg_encoding``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Set the encoding used for the interpretation of all subsequent arguments supplied via the GSAPI_ interface on this instance. By default we expect args to be in encoding 0 (the 'local' encoding for this OS). On Windows this means "the currently selected codepage". This means that omitting to call this function will leave Ghostscript running exactly as it always has. Please note that use of the 'local' encoding is now deprecated and should be avoided in new code. This must be called after :ref:`gsapi_new_instance<java gsapi_new_instance>` and before :ref:`gsapi_init_with_args<java gsapi_init_with_args>`.


.. code-block:: java

   public static native int gsapi_set_arg_encoding(long instance,
                                                    int encoding);


.. _java gsapi_set_default_device_list:

``gsapi_set_default_device_list``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Set the string containing the list of default device names, for example "display x11alpha x11 bbox". Allows the calling application to influence which device(s) Ghostscript will try, in order, in its selection of the default device. This must be called after :ref:`gsapi_new_instance<java gsapi_new_instance>` and before :ref:`gsapi_init_with_args<java gsapi_init_with_args>`.


.. code-block:: java

   public static native int gsapi_set_default_device_list(long instance,
                                                        byte[] list,
                                                           int listlen);


.. _java gsapi_get_default_device_list:

``gsapi_get_default_device_list``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Returns a pointer to the current default device string. This must be called after :ref:`gsapi_new_instance<java gsapi_new_instance>` and before :ref:`gsapi_init_with_args<java gsapi_init_with_args>`.


.. code-block:: java

   public static native int gsapi_get_default_device_list(long instance,
                                             Reference<byte[]> list,
                                            Reference<Integer> listlen);


.. _java gsapi_init_with_args:


``gsapi_init_with_args``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

To initialise the interpreter, pass your ``instance`` of Ghostscript, your argument count: ``argc``, and your argument variables: ``argv``.


.. code-block:: java

   public static native int gsapi_init_with_args(long instance,
                                                  int argc,
                                             byte[][] argv);

.. note::

   There are also simpler utility methods which eliminates the need to send through your argument count and allows for simpler ``String`` passing for your argument array.

   *Utility methods*:

   .. code-block:: java

      public static int gsapi_init_with_args(long instance,
                                         String[] argv);

      public static int gsapi_init_with_args(long instance,
                                     List<String> argv);




.. _java gsapi_run_asterisk:

``gsapi_run_*``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

If these functions return ``<= -100``, either quit or a fatal error has occured. You must call :ref:`java gsapi_exit<gsapi_exit>` next. The only exception is :ref:`gsapi_run_string_continue<java gsapi_run_string_continue>` which will return ``gs_error_NeedInput`` if all is well.

There is a 64 KB length limit on any buffer submitted to a ``gsapi_run_*`` function for processing. If you have more than 65535 bytes of input then you must split it into smaller pieces and submit each in a separate :ref:`gsapi_run_string_continue<java gsapi_run_string_continue>` call.


.. _java gsapi_run_string_begin:


``gsapi_run_string_begin``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


.. code-block:: java

   public static native int gsapi_run_string_begin(long instance,
                                                    int userErrors,
                                     Reference<Integer> pExitCode);

.. _java gsapi_run_string_continue:

``gsapi_run_string_continue``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: java

   public static native int gsapi_run_string_continue(long instance,
                                                    byte[] str,
                                                        int length,
                                                        int userErrors,
                                        Reference<Integer> pExitCode);


.. note::

   There is a simpler utility method which allows for simpler ``String`` passing for the ``str`` argument.

   *Utility method*:

   .. code-block:: java

      public static int gsapi_run_string_continue(long instance,
                                                String str,
                                                   int length,
                                                   int userErrors,
                                    Reference<Integer> pExitCode);


.. _java gsapi_run_string_with_length:

``gsapi_run_string_with_length``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: java

   public static native int gsapi_run_string_with_length(long instance,
                                                       byte[] str,
                                                          int length,
                                                          int userErrors,
                                           Reference<Integer> pExitCode);

.. note::

   There is a simpler utility method which allows for simpler ``String`` passing for the ``str`` argument.

   *Utility method*:

   .. code-block:: java

      public static int gsapi_run_string_with_length(long instance,
                                                    String str,
                                                       int length,
                                                       int userErrors,
                                       Reference<Integer> pExitCode);


.. _java gsapi_run_string:


``gsapi_run_string``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: java

   public static native int gsapi_run_string(long instance,
                                           byte[] str,
                                              int userErrors,
                               Reference<Integer> pExitCode);


.. note::

   There is a simpler utility method which allows for simpler ``String`` passing for the ``str`` argument.

   *Utility method*:

   .. code-block:: java

      public static int gsapi_run_string(long instance,
                                         String str,
                                            int userErrors,
                             Reference<Integer> pExitCode);


.. _java gsapi_run_string_end:


``gsapi_run_string_end``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: java

   public static native int gsapi_run_string_end(long instance,
                                                  int userErrors,
                                   Reference<Integer> pExitCode);


.. _java gsapi_run_file:

``gsapi_run_file``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: java

   public static native int gsapi_run_file(long instance,
                                         byte[] fileName,
                                            int userErrors,
                             Reference<Integer> pExitCode);

.. note::

   There is a simpler utility method which allows for simpler ``String`` passing for the ``fileName`` argument.

   *Utility method*:

   .. code-block:: java

      public static int gsapi_run_file(long instance,
                                     String fileName,
                                        int userErrors,
                         Reference<Integer> pExitCode);


.. _java gsapi_exit:


``gsapi_exit``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Exit the interpreter. This must be called on shutdown if :ref:`gsapi_init_with_args<java gsapi_init_with_args>` has been called, and just before :ref:`gsapi_delete_instance<java gsapi_delete_instance>`.

.. code-block:: java

   public static native int gsapi_exit(long instance);



.. _java gsapi_set_param:

``gsapi_set_param``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Sets a parameter. Broadly, this is equivalent to setting a parameter using ``-d``, ``-s`` or ``-p`` on the command line. This call cannot be made during a :ref:`gsapi_run_string<java gsapi_run_string>` operation.

Parameters in this context are not the same as 'arguments' as processed by :ref:`gsapi_init_with_args<java gsapi_init_with_args>`, but often the same thing can be achieved. For example, with :ref:`gsapi_init_with_args<java gsapi_init_with_args>`, we can pass "-r200" to change the resolution. Broadly the same thing can be achieved by using :ref:`gsapi_set_param<java gsapi_set_param>` to set a parsed value of "<</HWResolution [ 200.0 200.0 ]>>".

Internally, when we set a parameter, we perform an ``initgraphics`` operation. This means that using :ref:`gsapi_set_param<java gsapi_set_param>`  other than at the start of a page is likely to give unexpected results.

Attempting to set a parameter that the device does not recognise will be silently ignored, and that parameter will not be found in subsequent :ref:`gsapi_get_param<java gsapi_get_param>`  calls.


.. code-block:: java

   public static native int gsapi_set_param(long instance,
                                           byte[] param,
                                           Object value,
                                              int paramType);

.. note::

   For more on the C implementation of parameters see: :ref:`Ghostscript parameters in C<Use_Setting Parameters>`.

   There are also simpler utility methods which allows for simpler String passing for your arguments.

   *Utility methods*:


   .. code-block:: java

      public static int gsapi_set_param(long instance,
                                        String param,
                                        String value,
                                           int paramType);

      public static int gsapi_set_param(long instance,
                                      String param,
                                      Object value,
                                         int paramType);


.. _java gsapi_get_param:

``gsapi_get_param``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Retrieve the current value of a parameter.

If an error occurs, the return value is negative. Otherwise the return value is the number of bytes required for storage of the value. Call once with value ``NULL`` to get the number of bytes required, then call again with value pointing to at least the required number of bytes where the value will be copied out. Note that the caller is required to know the type of value in order to get it. For all types other than ``gs_spt_string``, ``gs_spt_name``, and ``gs_spt_parsed`` knowing the type means you already know the size required.

This call retrieves parameters/values that have made it to the device. Thus, any values set using ``gs_spt_more_to_come`` without a following call omitting that flag will not be retrieved. Similarly, attempting to get a parameter before :ref:`gsapi_init_with_args<java gsapi_init_with_args>` has been called will not list any, even if :ref:`gsapi_set_param<java gsapi_set_param>` has been used.

Attempting to read a parameter that is not set will return ``gs_error_undefined`` (-21). Note that calling :ref:`gsapi_set_param<java gsapi_set_param>` followed by :ref:`gsapi_get_param<java gsapi_get_param>` may not find the value, if the device did not recognise the key as being one of its configuration keys.

For further documentation please refer to :ref:`the C API<API_gsapi_get_param>`.


.. code-block:: java

   public static native int gsapi_get_param(long instance,
                                          byte[] param,
                                            long value,
                                             int paramType);
.. note::

   There is a simpler utility method which allows for simpler ``String`` passing for the ``param`` argument.

   *Utility method*:

   .. code-block:: java

      public static int gsapi_get_param(long instance,
                                       String param,
                                         long value,
                                          int paramType);


.. _java gsapi_enumerate_params:

``gsapi_enumerate_params``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Enumerate the current parameters. Call repeatedly to list out the current parameters.

The first call should have ``iter = NULL``. Subsequent calls should pass the same pointer in so the iterator can be updated. Negative return codes indicate error, 0 success, and 1 indicates that there are no more keys to read. On success, key will be updated to point to a null terminated string with the key name that is guaranteed to be valid until the next call to :ref:`gsapi_enumerate_params<java gsapi_enumerate_params>`. If type is non ``NULL`` then the pointer type will be updated to have the type of the parameter.

.. note::

  Only one enumeration can happen at a time. Starting a second enumeration will reset the first.

The enumeration only returns parameters/values that have made it to the device. Thus, any values set using the ``gs_spt_more_to_come`` without a following call omitting that flag will not be retrieved. Similarly, attempting to enumerate parameters before :ref:`gsapi_init_with_args<java gsapi_init_with_args>` has been called will not list any, even if :ref:`gsapi_set_param<java gsapi_set_param>` has been used.


.. code-block:: java

   public static native int gsapi_enumerate_params(long instance,
                                        Reference<Long> iter,
                                      Reference<byte[]> key,
                                     Reference<Integer> paramType);


.. _java gsapi_add_control_path:

``gsapi_add_control_path``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Add a (case sensitive) path to one of the lists of :ref:`permitted paths<Use Safer>` for file access.


.. code-block:: java

   public static native int gsapi_add_control_path(long instance,
                                                      int type,
                                                   byte[] path);

.. note::
   There is a simpler utility method which allows for simpler ``String`` passing for the ``path`` argument.

   *Utility method*:

   .. code-block:: java

      public static int gsapi_add_control_path(long instance,
                                                int type,
                                             String path);



.. _java gsapi_remove_control_path:

``gsapi_remove_control_path``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Remove a (case sensitive) path from one of the lists of :ref:`permitted paths<Use Safer>` for file access.


.. code-block:: java

   public static native int gsapi_remove_control_path(long instance,
                                                        int type,
                                                     byte[] path);
.. note::

   There is a simpler utility method which allows for simpler ``String`` passing for the ``path`` argument.

   *Utility method*:

   .. code-block:: java

      public static int gsapi_remove_control_path(long instance,
                                                   int type,
                                                String path);



.. _java gsapi_purge_control_paths:

``gsapi_purge_control_paths``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Clear all the paths from one of the lists of :ref:`permitted paths<Use Safer>` for file access.

.. code-block:: java

   public static native void gsapi_purge_control_paths(long instance,
                                                        int type);


.. _java gsapi_activate_path_control:

``gsapi_activate_path_control``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Enable/Disable path control (i.e. whether paths are checked against :ref:`permitted paths<Use Safer>` before access is granted).

.. code-block:: java

   public static native void gsapi_activate_path_control(long instance,
                                                      boolean enable);


.. _java gsapi_is_path_control_active:


``gsapi_is_path_control_active``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Query whether path control is activated or not.


.. code-block:: java

   public static native boolean gsapi_is_path_control_active(long instance);



Callback & Callout interfaces
--------------------------------------

``gsjava.jar`` also defines some functional interfaces for callbacks & callouts with package ``com.artifex.gsjava.callback`` which are defined as follows.


``IStdInFunction``
~~~~~~~~~~~~~~~~~~~~~

.. code-block:: java

   public interface IStdInFunction {
       /**
        * @param callerHandle The caller handle.
        * @param buf A string represented by a byte array.
        * @param len The number of bytes to read.
        * @return The number of bytes read, must be <code>len</code>/
        */
       public int onStdIn(long callerHandle,
                        byte[] buf,
                           int len);
   }


``IStdOutFunction``
~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: java

   public interface IStdOutFunction {
       /**
        * Called when something should be written to the standard
        * output stream.
        *
        * @param callerHandle The caller handle.
        * @param str The string represented by a byte array to write.
        * @param len The number of bytes to write.
        * @return The number of bytes written, must be <code>len</code>.
        */
       public int onStdOut(long callerHandle,
                         byte[] str,
                            int len);
   }


``IStdErrFunction``
~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: java

   public interface IStdErrFunction {
       /**
        * Called when something should be written to the standard error stream.
        *
        * @param callerHandle The caller handle.
        * @param str The string represented by a byte array to write.
        * @param len The length of bytes to be written.
        * @return The amount of bytes written, must be <code>len</code>.
        */
       public int onStdErr(long callerHandle,
                         byte[] str,
                            int len);
   }


``IPollFunction``
~~~~~~~~~~~~~~~~~~

.. code-block:: java

   public interface IPollFunction {
       public int onPoll(long callerHandle);
   }


``ICalloutFunction``
~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: java

   public interface ICalloutFunction {
       public int onCallout(long instance,
                            long calloutHandle,
                          byte[] deviceName,
                             int id,
                             int size,
                            long data);
   }



GSInstance
---------------

This is a utility class which makes Ghostscript calls easier by storing a Ghostscript instance and, optionally, a caller handle. Essentially the class acts as a handy wrapper for the standard GSAPI_ methods.


Constructors
~~~~~~~~~~~~~~~~~~~~

.. code-block:: java

   public GSInstance() throws IllegalStateException;
   public GSInstance(long callerHandle) throws IllegalStateException;


``delete_instance``
~~~~~~~~~~~~~~~~~~~~

Wraps :ref:`gsapi_delete_instance<java gsapi_delete_instance>`.

.. code-block:: java

   public void delete_instance();


``set_stdio``
~~~~~~~~~~~~~~~~~~~~

Wraps :ref:`gsapi_set_stdio<java gsapi_set_stdio>`.

.. code-block:: java

   public int set_stdio(IStdInFunction stdin,
                       IStdOutFunction stdout,
                       IStdErrFunction stderr);

``set_poll``
~~~~~~~~~~~~~~~~~~~~

Wraps :ref:`gsapi_set_poll<java gsapi_set_poll>`.

.. code-block:: java

   public int set_poll(IPollFunction pollfun);


``set_display_callback``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Wraps :ref:`gsapi_set_display_callback<java gsapi_set_display_callback>`.

.. code-block:: java

   public int set_display_callback(DisplayCallback displaycallback);


``register_callout``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Wraps :ref:`gsapi_register_callout<java gsapi_register_callout>`.

.. code-block:: java

   public int register_callout(ICalloutFunction callout);


``deregister_callout``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Wraps :ref:`gsapi_deregister_callout<java gsapi_deregister_callout>`.

.. code-block:: java

   public void deregister_callout(ICalloutFunction callout);


``set_arg_encoding``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Wraps :ref:`gsapi_set_arg_encoding<java gsapi_set_arg_encoding>`.

.. code-block:: java

   public int set_arg_encoding(int encoding);


``set_default_device_list``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Wraps :ref:`gsapi_set_default_device_list<java gsapi_set_default_device_list>`.

.. code-block:: java

   public int set_default_device_list(byte[] list,
                                         int listlen);


``get_default_device_list``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


Wraps :ref:`gsapi_get_default_device_list<java gsapi_get_default_device_list>`.

.. code-block:: java

   public int get_default_device_list(Reference<byte[]> list,
                                      Reference<Integer> listlen);

``init_with_args``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Wraps :ref:`gsapi_init_with_args<java gsapi_init_with_args>`.

.. code-block:: java

   public int init_with_args(int argc,
                        byte[][] argv);

   public int init_with_args(String[] argv);

   public int init_with_args(List<String> argv);



``run_string_begin``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Wraps :ref:`gsapi_run_string_begin<java gsapi_run_string_begin>`.

.. code-block:: java

   public int run_string_begin(int userErrors,
                Reference<Integer> pExitCode);

``run_string_continue``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Wraps :ref:`gsapi_run_string_continue<java gsapi_run_string_continue>`.

.. code-block:: java

   public int run_string_continue(byte[] str,
                                     int length,
                                     int userErrors,
                      Reference<Integer> pExitCode);

   public int run_string_continue(String str,
                                     int length,
                                     int userErrors,
                      Reference<Integer> pExitCode);


``run_string``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Wraps :ref:`gsapi_run_string<java gsapi_run_string>`.


.. code-block:: java

   public int run_string(byte[] str,
                            int userErrors,
             Reference<Integer> pExitCode);

   public int run_string(String str,
                            int userErrors,
             Reference<Integer> pExitCode);


``run_file``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Wraps :ref:`gsapi_run_file<java gsapi_run_file>`.


.. code-block:: java

   public int run_file(byte[] fileName,
                          int userErrors,
           Reference<Integer> pExitCode);

   public int run_file(String filename,
                          int userErrors,
           Reference<Integer> pExitCode);


``exit``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Wraps :ref:`gsapi_exit<java gsapi_exit>`.

.. code-block:: java

   public int exit();


``set_param``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Wraps :ref:`gsapi_set_param<java gsapi_set_param>`.

.. code-block:: java

   public int set_param(byte[] param,
                         Object value,
                            int paramType);

   public int set_param(String param,
                        String value,
                           int paramType);

   public int set_param(String param,
                        Object value,
                           int paramType);


``get_param``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Wraps :ref:`gsapi_get_param<java gsapi_get_param>`.


.. code-block:: java

   public int get_param(byte[] param,
                          long value,
                           int paramType);

   public int get_param(String param,
                          long value,
                           int paramType);

``enumerate_params``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Wraps :ref:`gsapi_enumerate_params<java gsapi_enumerate_params>`.

.. code-block:: java

   public int enumerate_params(Reference<Long> iter,
                             Reference<byte[]> key,
                            Reference<Integer> paramType);



``add_control_path``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Wraps :ref:`gsapi_add_control_path<java gsapi_add_control_path>`.


.. code-block:: java

   public int add_control_path(int type,
                            byte[] path);

   public int add_control_path(int type,
                            String path);


``remove_control_path``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Wraps :ref:`gsapi_remove_control_path<java gsapi_remove_control_path>`.


.. code-block:: java

   public int remove_control_path(int type,
                               byte[] path);

   public int remove_control_path(int type,
                               String path);

``purge_control_paths``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Wraps :ref:`gsapi_purge_control_paths<java gsapi_purge_control_paths>`.


.. code-block:: java

   public void purge_control_paths(int type);


``activate_path_control``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Wraps :ref:`gsapi_activate_path_control<java gsapi_activate_path_control>`.

.. code-block:: java

   public void activate_path_control(boolean enable);


``is_path_control_active``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Wraps :ref:`gsapi_is_path_control_active<java gsapi_is_path_control_active>`.

.. code-block:: java

   public boolean is_path_control_active();


Utility classes
------------------

The ``com.artifex.gsjava.util`` package contains a set of classes and interfaces which are used throughout the API.

``com.artifex.gsjava.util.Reference``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

``Reference<T>`` is used in many of the Ghostscript calls, it stores a reference to a generic value of type ``T``. This class exists to emulate pointers being passed to a native function. Its value can be fetched with ``getValue()`` and set with ``setValue(T value)``.


.. code-block:: java

   public class Reference<T> {

       private volatile T value;

       public Reference() {
           this(null);
       }

       public Reference(T value) {
           this.value = value;
       }

       public void setValue(T value) {
           this.value = value;
       }

       public T getValue() {
           return value;
       }
       ...
   }

.. External links

.. _Eclipse: https://www.eclipse.org/eclipseide/
