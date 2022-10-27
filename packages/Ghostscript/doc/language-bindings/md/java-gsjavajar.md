# gsjava.jar

<div class="banner">
<div class="java-text"></div>
<div class="vendor-logo java-logo"></div>
</div>


## About

`gsjava.jar` is the Java library which contains classes and interfaces which enable API calls required to use Ghostscript.

Assuming that the JAR for your project has been [built] and [properly linked] with your own project then the Ghostscript API should be available by importing the required classes within your project's `.java` files.


### GSAPI & GSInstance

- [GSAPI] is the main Ghostscript API class which bridges into the Ghostscript C library.
- [GSInstance] is a wrapper class for [GSAPI] which encapsulates an instance of Ghostscript and allows for simpler API calls.


<div class="tag sampleCode java"></div>


```
// to use GSAPI
import static com.artifex.gsjava.GSAPI.*;

// to use GSInstance
import com.artifex.gsjava.GSInstance;
```


## GSAPI


### gsapi_revision

This method returns the revision numbers and strings of the Ghostscript interpreter library; you should call it before any other interpreter library functions to make sure that the correct version of the Ghostscript interpreter has been loaded.


<div class="tag methodDefinition java"></div>

```
public static native int gsapi_revision(GSAPI.Revision revision,
                                                   int len);
```


> **NOTE**<br>
> The method should write to a reference variable which conforms to the class [GSAPI.Revision].
>


#### GSAPI.Revision

This class is used to store information about Ghostscript and provides handy getters for the product and the copyright information.


<div class="tag classDefinition java"></div>

```
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
```


### gsapi_new_instance

Creates a new instance of Ghostscript. This instance is passed to most other [GSAPI] methods. Unless Ghostscript has been compiled with the `GS_THREADSAFE` define, only one instance at a time is supported.

<div class="tag methodDefinition java"></div>

```
public static native int gsapi_new_instance(Reference<Long> instance,
                                                       long callerHandle);
```



> **NOTE**<br>
> The method returns a reference which represents your instance of Ghostscript.
>


### gsapi_delete_instance

Destroy an instance of Ghostscript. Before you call this, Ghostscript must have finished. If Ghostscript has been initialised, you should call [gsapi_exit] beforehand.

<div class="tag methodDefinition java"></div>

```
public static native void gsapi_delete_instance(long instance);
```


### gsapi_set_stdio_with_handle

Set the callback functions for `stdio`, together with the handle to use in the callback functions. The `stdin` callback function should return the number of characters read, 0 for EOF, or -1 for error. The `stdout` and `stderr` callback functions should return the number of characters written.

<div class="tag methodDefinition java"></div>

```
public static native int gsapi_set_stdio_with_handle(long instance,
                                           IStdInFunction stdin,
                                          IStdOutFunction stdout,
                                          IStdErrFunction stderr,
                                                     long callerHandle);
```


### gsapi_set_stdio

Set the callback functions for `stdio`. The handle used in the callbacks will be taken from the value passed to [gsapi_new_instance]. Otherwise the behaviour of this function matches [gsapi_set_stdio_with_handle].

<div class="tag methodDefinition java"></div>

```
public static native int gsapi_set_stdio(long instance,
                               IStdInFunction stdin,
                              IStdOutFunction stdout,
                              IStdErrFunction stderr);
```


### gsapi_set_poll_with_handle

Set the callback function for polling, together with the handle to pass to the callback function. This function will only be called if the Ghostscript interpreter was compiled with `CHECK_INTERRUPTS` as described in `gpcheck.h`.

The polling function should return zero if all is well, and return negative if it wants ghostscript to abort. This is often used for checking for a user cancel. This can also be used for handling window events or cooperative multitasking.

The polling function is called very frequently during interpretation and rendering so it must be fast. If the function is slow, then using a counter to `return 0` immediately some number of times can be used to reduce the performance impact.


<div class="tag methodDefinition java"></div>

```
public static native int gsapi_set_poll_with_handle(long instance,
                                           IPollFunction pollfun,
                                                    long callerHandle);
```

### gsapi_set_poll

Set the callback function for polling. The handle passed to the callback function will be taken from the handle passed to [gsapi_new_instance]. Otherwise the behaviour of this function matches [gsapi_set_poll_with_handle].

<div class="tag methodDefinition java"></div>

```
public static native int gsapi_set_poll(long instance,
                               IPollFunction pollfun);
```

### gsapi_set_display_callback

This call is deprecated; please use [gsapi_register_callout] to register a [callout handler] for the display device in preference.

<div class="tag methodDefinition java"></div>

```
public static native int gsapi_set_display_callback(long instance,
                                         DisplayCallback displayCallback);
```


### gsapi_register_callout

This call registers a [callout handler].

<div class="tag methodDefinition java"></div>

```
public static native int gsapi_register_callout(long instance,
                                    ICalloutFunction callout,
                                                long calloutHandle);
```


### gsapi_deregister_callout


This call deregisters a [callout handler] previously registered with [gsapi_register_callout]. All three arguments must match exactly for the [callout handler] to be deregistered.


<div class="tag methodDefinition java"></div>

```
public static native void gsapi_deregister_callout(long instance,
                                       ICalloutFunction callout,
                                                   long calloutHandle);
```

### gsapi_set_arg_encoding

Set the encoding used for the interpretation of all subsequent arguments supplied via the `GSAPI` interface on this instance. By default we expect args to be in encoding `0` (the 'local' encoding for this OS). On Windows this means "the currently selected codepage". This means that omitting to call this function will leave Ghostscript running exactly as it always has. Please note that use of the 'local' encoding is now deprecated and should be avoided in new code. This must be called after [gsapi_new_instance] and before [gsapi_init_with_args].

<div class="tag methodDefinition java"></div>

```
public static native int gsapi_set_arg_encoding(long instance,
                                                 int encoding);
```

### gsapi_set_default_device_list

Set the string containing the list of default device names, for example "display x11alpha x11 bbox". Allows the calling application to influence which device(s) Ghostscript will try, in order, in its selection of the default device. This must be called after [gsapi_new_instance] and before [gsapi_init_with_args].

<div class="tag methodDefinition java"></div>

```
public static native int gsapi_set_default_device_list(long instance,
                                                     byte[] list,
                                                        int listlen);
```

### gsapi_get_default_device_list

Returns a pointer to the current default device string. This must be called after [gsapi_new_instance] and before [gsapi_init_with_args].

<div class="tag methodDefinition java"></div>

```
public static native int gsapi_get_default_device_list(long instance,
                                          Reference<byte[]> list,
                                         Reference<Integer> listlen);
```

### gsapi_init_with_args

To initialise the interpreter, pass your `instance` of Ghostscript, your argument count: `argc`, and your argument variables: `argv`.

<div class="tag methodDefinition java"></div>

```
public static native int gsapi_init_with_args(long instance,
                                               int argc,
                                          byte[][] argv);
```


> **NOTE**<br>
> There are also simpler utility methods which eliminates the need to send through your argument count and allows for simpler `String` passing for your argument array.
>

Utility methods:

<div class="tag methodDefinition java"></div>

```
public static int gsapi_init_with_args(long instance,
                                   String[] argv);
```

```
public static int gsapi_init_with_args(long instance,
                               List<String> argv);
```

### gsapi\_run\_\*

If these functions return `<= -100`, either quit or a fatal error has occured. You must call [gsapi_exit] next. The only exception is [gsapi_run_string_continue] which will return `gs_error_NeedInput` if all is well.

There is a 64 KB length limit on any buffer submitted to a `gsapi_run_*` function for processing. If you have more than 65535 bytes of input then you must split it into smaller pieces and submit each in a separate [gsapi_run_string_continue] call.

### gsapi_run_string_begin

<div class="tag methodDefinition java"></div>

```
public static native int gsapi_run_string_begin(long instance,
                                                 int userErrors,
                                  Reference<Integer> pExitCode);
```



### gsapi_run_string_continue

<div class="tag methodDefinition java"></div>

```
public static native int gsapi_run_string_continue(long instance,
                                                 byte[] str,
                                                    int length,
                                                    int userErrors,
                                     Reference<Integer> pExitCode);
```


> **NOTE**<br>
> There is a simpler utility method which allows for simpler `String` passing for the `str` argument.
>


Utility method:

<div class="tag methodDefinition java"></div>

```
public static int gsapi_run_string_continue(long instance,
                                          String str,
                                             int length,
                                             int userErrors,
                              Reference<Integer> pExitCode);
```


### gsapi_run_string_with_length

<div class="tag methodDefinition java"></div>

```
public static native int gsapi_run_string_with_length(long instance,
                                                    byte[] str,
                                                       int length,
                                                       int userErrors,
                                        Reference<Integer> pExitCode);
```


> **NOTE**<br>
> There is a simpler utility method which allows for simpler `String` passing for the `str` argument.
>


Utility method:


<div class="tag methodDefinition java"></div>

```
public static int gsapi_run_string_with_length(long instance,
                                             String str,
                                                int length,
                                                int userErrors,
                                 Reference<Integer> pExitCode);
```


### gsapi_run_string


<div class="tag methodDefinition java"></div>

```
public static native int gsapi_run_string(long instance,
                                        byte[] str,
                                           int userErrors,
                            Reference<Integer> pExitCode);
```




> **NOTE**<br>
> There is a simpler utility method which allows for simpler `String` passing for the `str` argument.
>


Utility method:


<div class="tag methodDefinition java"></div>

```
public static int gsapi_run_string(long instance,
                                 String str,
                                    int userErrors,
                     Reference<Integer> pExitCode);
```



### gsapi_run_string_end

<div class="tag methodDefinition java"></div>

```
public static native int gsapi_run_string_end(long instance,
                                               int userErrors,
                                Reference<Integer> pExitCode);
```

### gsapi_run_file

<div class="tag methodDefinition java"></div>

```
public static native int gsapi_run_file(long instance,
                                      byte[] fileName,
                                         int userErrors,
                          Reference<Integer> pExitCode);
```



> **NOTE**<br>
> There is a simpler utility method which allows for simpler `String` passing for the `fileName` argument.
>


Utility method:


<div class="tag methodDefinition java"></div>

```
public static int gsapi_run_file(long instance,
                               String fileName,
                                  int userErrors,
                   Reference<Integer> pExitCode);
```




### gsapi_exit

Exit the interpreter. This must be called on shutdown if [gsapi_init_with_args] has been called, and just before [gsapi_delete_instance].

<div class="tag methodDefinition java"></div>

```
public static native int gsapi_exit(long instance);
```



### gsapi_set_param

Sets a parameter. Broadly, this is equivalent to setting a parameter using -d, -s or -p on the command line. This call cannot be made during a [gsapi_run_string] operation.

Parameters in this context are not the same as 'arguments' as processed by [gsapi_init_with_args], but often the same thing can be achieved. For example, with [gsapi_init_with_args], we can pass "-r200" to change the resolution. Broadly the same thing can be achieved by using [gsapi_set_param] to set a parsed value of "<</HWResolution [ 200.0 200.0 ]>>".

Internally, when we set a parameter, we perform an `initgraphics` operation. This means that using [gsapi_set_param] other than at the start of a page is likely to give unexpected results.

Attempting to set a parameter that the device does not recognise will be silently ignored, and that parameter will not be found in subsequent [gsapi_get_param] calls.


<div class="tag methodDefinition java"></div>

```
public static native int gsapi_set_param(long instance,
                                       byte[] param,
                                       Object value,
                                          int paramType);
```

> **NOTE**<br>
> The `type` argument, as a [gs_set_param_type], dictates the kind of object that the `value` argument points to.
>

> **NOTE**<br>
> For more on the C implementation of parameters see: [Ghostscript parameters in C].
>

> **NOTE**<br>
> There are also simpler utility methods which allows for simpler `String` passing for your arguments.
>


Utility methods:

<div class="tag methodDefinition java"></div>

```
public static int gsapi_set_param(long instance,
                                String param,
                                String value,
                                   int paramType);
```

```
public static int gsapi_set_param(long instance,
                                String param,
                                Object value,
                                   int paramType);
```


### gsapi_get_param

Retrieve the current value of a parameter.

If an error occurs, the return value is negative. Otherwise the return value is the number of bytes required for storage of the value. Call once with value `NULL` to get the number of bytes required, then call again with value pointing to at least the required number of bytes where the value will be copied out. Note that the caller is required to know the type of value in order to get it. For all types other than [gs_spt_string], [gs_spt_name], and [gs_spt_parsed] knowing the type means you already know the size required.

This call retrieves parameters/values that have made it to the device. Thus, any values set using [gs_spt_more_to_come] without a following call omitting that flag will not be retrieved. Similarly, attempting to get a parameter before [gsapi_init_with_args] has been called will not list any, even if [gsapi_set_param] has been used.

Attempting to read a parameter that is not set will return `gs_error_undefined` (-21). Note that calling [gsapi_set_param] followed by [gsapi_get_param] may not find the value, if the device did not recognise the key as being one of its configuration keys.

For the C documentation please refer to [Ghostscript get_param].


<div class="tag methodDefinition java"></div>

```
public static native int gsapi_get_param(long instance,
                                       byte[] param,
                                         long value,
                                          int paramType);
```



> **NOTE**<br>
> There is a simpler utility method which allows for simpler `String` passing for the `param` argument.
>


Utility method:


<div class="tag methodDefinition java"></div>

```
public static int gsapi_get_param(long instance,
                                String param,
                                  long value,
                                   int paramType);
```



### gsapi_enumerate_params

Enumerate the current parameters. Call repeatedly to list out the current parameters.

The first call should have `iter` = NULL. Subsequent calls should pass the same pointer in so the iterator can be updated. Negative return codes indicate error, 0 success, and 1 indicates that there are no more keys to read. On success, key will be updated to point to a null terminated string with the key name that is guaranteed to be valid until the next call to [gsapi_enumerate_params]. If `type` is non NULL then the pointer `type` will be updated to have the `type` of the parameter.


> **NOTE**<br>
> Only one enumeration can happen at a time. Starting a second enumeration will reset the first.
>

The enumeration only returns parameters/values that have made it to the device. Thus, any values set using the [gs_spt_more_to_come] without a following call omitting that flag will not be retrieved. Similarly, attempting to enumerate parameters before [gsapi_init_with_args] has been called will not list any, even if [gsapi_set_param] has been used.

<div class="tag methodDefinition java"></div>

```
public static native int gsapi_enumerate_params(long instance,
                                     Reference<Long> iter,
                                   Reference<byte[]> key,
                                  Reference<Integer> paramType);
```


### gsapi_add_control_path

Add a (case sensitive) path to one of the lists of [permitted paths] for file access.

<div class="tag methodDefinition java"></div>

```
public static native int gsapi_add_control_path(long instance,
                                                 int type,
                                              byte[] path);
```

> **NOTE**<br>
> There is a simpler utility method which allows for simpler `String` passing for the `path` argument.
>

Utility method:


<div class="tag methodDefinition java"></div>

```
public static int gsapi_add_control_path(long instance,
                                          int type,
                                       String path);
```

### gsapi_remove_control_path

Remove a (case sensitive) path from one of the lists of [permitted paths] for file access.

<div class="tag methodDefinition java"></div>

```
public static native int gsapi_remove_control_path(long instance,
                                                    int type,
                                                 byte[] path);
```

> **NOTE**<br>
> There is a simpler utility method which allows for simpler `String` passing for the `path` argument.
>

Utility method:


<div class="tag methodDefinition java"></div>

```
public static int gsapi_remove_control_path(long instance,
                                             int type,
                                          String path);
```


### gsapi_purge_control_paths

Clear all the paths from one of the lists of [permitted paths] for file access.
<div class="tag methodDefinition java"></div>

```
public static native void gsapi_purge_control_paths(long instance,
                                                     int type);
```


### gsapi_activate_path_control

Enable/Disable path control (i.e. whether paths are checked against [permitted paths] before access is granted).

<div class="tag methodDefinition java"></div>

```
public static native void gsapi_activate_path_control(long instance,
                                                   boolean enable);
```


### gsapi_is_path_control_active


Query whether path control is activated or not.

<div class="tag methodDefinition java"></div>


```
public static native boolean gsapi_is_path_control_active(long instance);
```




## Callback & Callout interfaces

`gsjava.jar` also defines some functional interfaces for callbacks & callouts with `package com.artifex.gsjava.callback` which are defined as follows.

### IStdInFunction

<div class="tag functionalInterface java"></div>

```
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
```


### IStdOutFunction

<div class="tag functionalInterface java"></div>

```
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
```


### IStdErrFunction

<div class="tag functionalInterface java"></div>

```
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
```


### IPollFunction

<div class="tag functionalInterface java"></div>

```
public interface IPollFunction {
    public int onPoll(long callerHandle);
}
```


### ICalloutFunction

<div class="tag functionalInterface java"></div>

```
public interface ICalloutFunction {
    public int onCallout(long instance,
                         long calloutHandle,
                       byte[] deviceName,
                          int id,
                          int size,
                         long data);
}
```


## GSInstance

This is a utility class which makes Ghostscript calls easier by storing a Ghostscript instance and, optionally, a caller handle. Essentially the class acts as a handy wrapper for the standard [GSAPI] methods.


### Constructors

<div class="tag methodDefinition java"></div>

```
public GSInstance() throws IllegalStateException;
```

```
public GSInstance(long callerHandle) throws IllegalStateException;
```

### delete_instance

Wraps [gsapi_delete_instance].

<div class="tag methodDefinition java"></div>

```
public void delete_instance();
```

### set_stdio


Wraps [gsapi_set_stdio].

<div class="tag methodDefinition java"></div>

```
public int set_stdio(IStdInFunction stdin,
                    IStdOutFunction stdout,
                    IStdErrFunction stderr);
```

### set_poll

Wraps [gsapi_set_poll].

<div class="tag methodDefinition java"></div>

```
public int set_poll(IPollFunction pollfun);
```

### set_display_callback

Wraps [gsapi_set_display_callback].

<div class="tag methodDefinition java"></div>

```
public int set_display_callback(DisplayCallback displaycallback);
```

### register_callout

Wraps [gsapi_register_callout].

<div class="tag methodDefinition java"></div>

```
public int register_callout(ICalloutFunction callout);
```


### deregister_callout

Wraps [gsapi_deregister_callout].

<div class="tag methodDefinition java"></div>

```
public void deregister_callout(ICalloutFunction callout);
```


### set_arg_encoding

Wraps [gsapi_set_arg_encoding].

<div class="tag methodDefinition java"></div>

```
public int set_arg_encoding(int encoding);
```


### set_default_device_list

Wraps [gsapi_set_default_device_list].

<div class="tag methodDefinition java"></div>

```
public int set_default_device_list(byte[] list,
                                      int listlen);
```

### get_default_device_list

Wraps [gsapi_get_default_device_list].

<div class="tag methodDefinition java"></div>

```
public int get_default_device_list(Reference<byte[]> list,
                                  Reference<Integer> listlen);
```


### init_with_args

Wraps [gsapi_init_with_args].

<div class="tag methodDefinition java"></div>

```
public int init_with_args(int argc,
                     byte[][] argv);
```

```
public int init_with_args(String[] argv);
```

```
public int init_with_args(List<String> argv);
```


### run_string_begin

Wraps [gsapi_run_string_begin].

<div class="tag methodDefinition java"></div>

```
public int run_string_begin(int userErrors,
             Reference<Integer> pExitCode);
```


### run_string_continue

Wraps [gsapi_run_string_continue].

<div class="tag methodDefinition java"></div>

```
public int run_string_continue(byte[] str,
                                  int length,
                                  int userErrors,
                   Reference<Integer> pExitCode);
```

```
public int run_string_continue(String str,
                                  int length,
                                  int userErrors,
                   Reference<Integer> pExitCode);
```

### run_string

Wraps [gsapi_run_string].

<div class="tag methodDefinition java"></div>

```
public int run_string(byte[] str,
                         int userErrors,
          Reference<Integer> pExitCode);
```

```
public int run_string(String str,
                         int userErrors,
          Reference<Integer> pExitCode);
```


### run_file

Wraps [gsapi_run_file].

<div class="tag methodDefinition java"></div>

```
public int run_file(byte[] fileName,
                       int userErrors,
        Reference<Integer> pExitCode);
```

```
public int run_file(String filename,
                       int userErrors,
        Reference<Integer> pExitCode);
```

### exit

Wraps [gsapi_exit].

<div class="tag methodDefinition java"></div>

```
public int exit();
```


### set_param

Wraps [gsapi_set_param].

<div class="tag methodDefinition java"></div>

```
public int set_param(byte[] param,
                     Object value,
                        int paramType);
```

```
public int set_param(String param,
                     String value,
                        int paramType);
```

```
public int set_param(String param,
                     Object value,
                        int paramType);
```


### get_param

Wraps [gsapi_get_param].

<div class="tag methodDefinition java"></div>

```
public int get_param(byte[] param,
                       long value,
                        int paramType);
```

```
public int get_param(String param,
                       long value,
                        int paramType);
```

### enumerate_params

Wraps [gsapi_enumerate_params].

<div class="tag methodDefinition java"></div>

```
public int enumerate_params(Reference<Long> iter,
                          Reference<byte[]> key,
                         Reference<Integer> paramType);
```


### add_control_path

Wraps [gsapi_add_control_path].

<div class="tag methodDefinition java"></div>

```
public int add_control_path(int type,
                         byte[] path);
```

```
public int add_control_path(int type,
                         String path);
```

### remove_control_path

Wraps [gsapi_remove_control_path].

<div class="tag methodDefinition java"></div>

```
public int remove_control_path(int type,
                            byte[] path);
```

```
public int remove_control_path(int type,
                            String path);
```


### purge_control_paths

Wraps [gsapi_purge_control_paths].

<div class="tag methodDefinition java"></div>

```
public void purge_control_paths(int type);
```


### activate_path_control

Wraps [gsapi_activate_path_control].

<div class="tag methodDefinition java"></div>

```
public void activate_path_control(boolean enable);
```


### is_path_control_active

Wraps [gsapi_is_path_control_active].

<div class="tag methodDefinition java"></div>

```
public boolean is_path_control_active();
```

## Utility classes

The com.artifex.gsjava.util package contains a set of classes and interfaces which are used throughout the API.

### com.artifex.gsjava.util.Reference

`Reference<T>` is used in many of the Ghostscript calls, it stores a reference to a generic value of type `T`. This class exists to emulate pointers being passed to a native function. Its value can be fetched with `getValue()` and set with `setValue(T value)`.

<div class="tag classDefinition java"></div>

```
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
```

[Ghostscript parameters in C]: https://www.ghostscript.com/doc/current/Use.htm#Parameters
[Ghostscript get_param]: https://www.ghostscript.com/doc/current/API.htm#get_param
[permitted paths]: https://ghostscript.com/doc/current/Use.htm#Safer

[GSAPI]: #gsapi
[GSInstance]: #gsinstance
[built]: java-intro.html#building-the-jar
[properly linked]: java-intro.html#linking-the-jar
[GSAPI.Revision]: #gsapi-revision


[gsapi_set_stdio]: #gsapi_set_stdio
[gsapi_set_poll]: #gsapi_set_poll
[gsapi_set_display_callback]: #gsapi_set_display_callback
[gsapi_deregister_callout]: #gsapi_deregister_callout
[gsapi_run_string_begin]: #gsapi_run_string_begin
[gsapi_run_file]: #gsapi_run_file
[gsapi_add_control_path]: #gsapi_add_control_path
[gsapi_remove_control_path]: #gsapi_remove_control_path
[gsapi_purge_control_paths]: #gsapi_purge_control_paths
[gsapi_activate_path_control]: #gsapi_activate_path_control
[gsapi_is_path_control_active]: #gsapi_is_path_control_active
[gsapi_revision]: #gsapi_revision
[gsapi_exit]: #gsapi_exit
[gsapi_new_instance]: #gsapi_new_instance
[gsapi_set_stdio_with_handle]: #gsapi_set_stdio_with_handle
[gsapi_set_poll_with_handle]: #gsapi_set_poll_with_handle
[gsapi_register_callout]: #gsapi_register_callout
[gsapi_init_with_args]: #gsapi_init_with_args
[gsapi_delete_instance]: #gsapi_delete_instance
[gsapi_exit]: #gsapi_exit
[gsapi_run_string]: #gsapi_run_string
[gsapi_run_string_continue]: #gsapi_run_string_continue
[gs_set_param_type]: #gs_set_param_type
[gs_spt_more_to_come]: #gs_set_param_type
[gs_spt_string]: #gs_set_param_type
[gs_spt_name]: #gs_set_param_type
[gs_spt_parsed]: #gs_set_param_type
[gsapi_set_param]: #gsapi_set_param
[gsapi_get_param]: #gsapi_get_param
[gsapi_enumerate_params]: #gsapi_enumerate_params
[gsapi_set_arg_encoding]: #gsapi_set_arg_encoding
[gsapi_set_default_device_list]: #gsapi_set_default_device_list
[gsapi_get_default_device_list]: #gsapi_get_default_device_list


[callout handler]: #callback-callout-interfaces
