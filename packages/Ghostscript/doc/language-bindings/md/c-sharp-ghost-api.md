# GhostAPI

<div class="banner">
    <div class="c-sharp-text"></div>
    <div class="vendor-logo c-sharp-logo"></div>
</div>


## About

`GhostAPI` is the `C#` bridge into the Ghostscript `C` library.

`GhostAPI` contains some essential [structs and enums] as well as a static class for some [constants], finally it contains the main [GSAPI] class which holds the key methods which interface with the `C` library.

## Structs and Enums


### gsapi_revision_t

This `struct` is used to contain information pertinent to the version of Ghostscript.

<div class="tag structDefinition csharp"></div>

```
public struct gsapi_revision_t
{
    public IntPtr product;
    public IntPtr copyright;
    public int revision;
    public int revisiondate;
}
```


### gs_set_param_type


<div class="tag enumDefinition csharp"></div>

```
public enum gs_set_param_type
{
    gs_spt_invalid = -1,
    gs_spt_null =   0, /* void * is NULL */
    gs_spt_bool =   1, /* void * is NULL (false) or non-NULL (true) */
    gs_spt_int = 2, /* void * is a pointer to an int */
    gs_spt_float = 3, /* void * is a float * */
    gs_spt_name = 4, /* void * is a char * */
    gs_spt_string = 5, /* void * is a char * */
    gs_spt_long =   6, /* void * is a long * */
    gs_spt_i64 = 7, /* void * is an int64_t * */
    gs_spt_size_t = 8, /* void * is a size_t * */
    gs_spt_parsed = 9, /* void * is a pointer to a char * to be parsed */
    gs_spt_more_to_come = 1 << 31
};
```


### gsEncoding


<div class="tag enumDefinition csharp"></div>


```
public enum gsEncoding
{
    GS_ARG_ENCODING_LOCAL = 0,
    GS_ARG_ENCODING_UTF8 = 1,
    GS_ARG_ENCODING_UTF16LE = 2
};
```

## Constants

Constants are stored in the static class `gsConstants` for direct referencing.

### gsConstants


<div class="tag classDefinition csharp"></div>

```
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
```

## GSAPI

Methods contained within are explained below.

`gsapi_run_*` and `gsapi_exit` methods return an `int` code which can be interpreted as follows:

| code | status |
|---|---|
|`0`| no error |
|`gsConstants.E_QUIT`| "quit" has been executed. This is not an error. [gsapi_exit] must be called next |
|`<0` | error |

>**NOTE**<br>
> For full detail on these return code please see:
https://www.ghostscript.com/doc/current/API.htm#return_codes
>

> **NOTE**<br>
> All [GSAPI] methods aside from [gsapi_revision] and [gsapi_new_instance] should pass an instance of Ghostscript as their first parameter with `IntPtr instance`
>

### gsapi_revision

This method returns the revision numbers and strings of the Ghostscript interpreter library; you should call it before any other interpreter library functions to make sure that the correct version of the Ghostscript interpreter has been loaded.



<div class="tag methodDefinition csharp"></div>

```
public static extern int gsapi_revision(ref gsapi_revision_t vers,
                                                         int size);
```


> **NOTE**<br>
> The method should write to a reference variable which conforms to the struct [gsapi_revision_t].
>

### gsapi_new_instance

Creates a new instance of Ghostscript. This instance is passed to most other [GSAPI] methods. Unless Ghostscript has been compiled with the `GS_THREADSAFE` define, only one instance at a time is supported.

<div class="tag methodDefinition csharp"></div>

```
public static extern int gsapi_new_instance(out IntPtr pinstance,
                                                IntPtr caller_handle);
```


> **NOTE**<br>
> The method returns a pointer which represents your instance of Ghostscript.
>

### gsapi_delete_instance

Destroy an instance of Ghostscript. Before you call this, Ghostscript must have finished. If Ghostscript has been initialised, you must call [gsapi_exit] beforehand.

<div class="tag methodDefinition csharp"></div>

```
public static extern void gsapi_delete_instance(IntPtr instance);
```

<div class="tag sampleCode csharp"></div>

```
GSAPI.gsapi_delete_instance(gsInstance);
gsInstance = IntPtr.Zero;
```

### gsapi_set_stdio_with_handle

Set the callback functions for `stdio`, together with the handle to use in the callback functions. The `stdin` callback function should return the number of characters read, 0 for EOF, or -1 for error. The `stdout` and `stderr` callback functions should return the number of characters written.



> **NOTE**<br>
> These callbacks do not affect output device I/O when using "%stdout" as the output file. In that case, device output will still be directed to the process "stdout" file descriptor, not to the `stdio` callback.
>

<div class="tag methodDefinition csharp"></div>

```
public static extern int gsapi_set_stdio_with_handle(IntPtr instance,
                                           gs_stdio_handler stdin,
                                           gs_stdio_handler stdout,
                                           gs_stdio_handler stderr,
                                                     IntPtr caller_handle);
```


### gsapi_set_stdio

Set the callback functions for `stdio`. The handle used in the callbacks will be taken from the value passed to [gsapi_new_instance]. Otherwise the behaviour of this function matches [gsapi_set_stdio_with_handle].

<div class="tag methodDefinition csharp"></div>

```
public static extern int gsapi_set_stdio_with_handle(IntPtr instance,
                                           gs_stdio_handler stdin,
                                           gs_stdio_handler stdout,
                                           gs_stdio_handler stderr);
```


### gsapi_set_poll_with_handle

Set the callback function for polling, together with the handle to pass to the callback function. This function will only be called if the Ghostscript interpreter was compiled with `CHECK_INTERRUPTS` as described in `gpcheck.h`.

The polling function should return zero if all is well, and return negative if it wants ghostscript to abort. This is often used for checking for a user cancel. This can also be used for handling window events or cooperative multitasking.

The polling function is called very frequently during interpretation and rendering so it must be fast. If the function is slow, then using a counter to `return 0` immediately some number of times can be used to reduce the performance impact.


<div class="tag methodDefinition csharp"></div>

```
public static extern int gsapi_set_poll_with_handle(IntPtr instance,
                                             gsPollHandler pollfn,
                                                    IntPtr caller_handle);
```

### gsapi_set_poll

Set the callback function for polling. The handle passed to the callback function will be taken from the handle passed to [gsapi_new_instance]. Otherwise the behaviour of this function matches [gsapi_set_poll_with_handle].

<div class="tag methodDefinition csharp"></div>

```
public static extern int gsapi_set_poll(IntPtr instance,
                                 gsPollHandler pollfn);
```


### gsapi_set_display_callback

This call is deprecated; please use [gsapi_register_callout] to register a [callout handler] for the display device in preference.

<div class="tag methodDefinition csharp"></div>

```
public static extern int gsapi_set_display_callback(IntPtr pinstance,
                                                    IntPtr caller_handle);
```

### gsapi_register_callout

This call registers a [callout handler].

<div class="tag methodDefinition csharp"></div>

```
public static extern int gsapi_register_callout(IntPtr instance,
                                             gsCallOut callout,
                                                IntPtr callout_handle);
```

### gsapi_deregister_callout


This call deregisters a [callout handler] previously registered with [gsapi_register_callout]. All three arguments must match exactly for the [callout handler] to be deregistered.


<div class="tag methodDefinition csharp"></div>

```
public static extern int gsapi_deregister_callout(IntPtr instance,
                                               gsCallOut callout,
                                                  IntPtr callout_handle);
```

### gsapi_set_arg_encoding

Set the encoding used for the interpretation of all subsequent arguments supplied via the `GhostAPI` interface on this instance. By default we expect args to be in encoding `0` (the 'local' encoding for this OS). On Windows this means "the currently selected codepage". On Linux this typically means `utf8`. This means that omitting to call this function will leave Ghostscript running exactly as it always has. Please note that use of the 'local' encoding is now deprecated and should be avoided in new code. This must be called after [gsapi_new_instance] and before [gsapi_init_with_args].

<div class="tag methodDefinition csharp"></div>

```
public static extern int gsapi_set_arg_encoding(IntPtr instance,
                                                   int encoding);
```

### gsapi_set_default_device_list

Set the string containing the list of default device names, for example "display x11alpha x11 bbox". Allows the calling application to influence which device(s) Ghostscript will try, in order, in its selection of the default device. This must be called after [gsapi_new_instance] and before [gsapi_init_with_args].

<div class="tag methodDefinition csharp"></div>

```
public static extern int gsapi_set_default_device_list(IntPtr instance,
                                                       IntPtr list,
                                                      ref int listlen);
```

### gsapi_get_default_device_list

Returns a pointer to the current default device string. This must be called after [gsapi_new_instance] and before [gsapi_init_with_args].

<div class="tag methodDefinition csharp"></div>

```
public static extern int gsapi_get_default_device_list(IntPtr instance,
                                                   ref IntPtr list,
                                                      ref int listlen);
```

### gsapi_init_with_args

To initialise the interpreter, pass your `instance` of Ghostscript, your argument count, `argc`, and your argument variables, `argv`.

<div class="tag methodDefinition csharp"></div>

```
public static extern int gsapi_init_with_args(IntPtr instance,
                                                 int argc,
                                              IntPtr argv);
```

### gsapi\_run\_\*

If these functions return `<= -100`, either quit or a fatal error has occured. You must call [gsapi_exit] next. The only exception is [gsapi_run_string_continue] which will return `gs_error_NeedInput` if all is well.

There is a 64 KB length limit on any buffer submitted to a `gsapi_run_*` function for processing. If you have more than 65535 bytes of input then you must split it into smaller pieces and submit each in a separate [gsapi_run_string_continue] call.

### gsapi_run_string_begin

<div class="tag methodDefinition csharp"></div>

```
public static extern int gsapi_run_string_begin(IntPtr instance,
                                                   int usererr,
                                               ref int exitcode);
```

### gsapi_run_string_continue

<div class="tag methodDefinition csharp"></div>

```
public static extern int gsapi_run_string_continue(IntPtr instance,
                                                   IntPtr command,
                                                      int count,
                                                      int usererr,
                                                  ref int exitcode);
```

### gsapi_run_string_with_length

<div class="tag methodDefinition csharp"></div>

```
public static extern int gsapi_run_string_with_length(IntPtr instance,
                                                      IntPtr command,
                                                        uint length,
                                                         int usererr,
                                                     ref int exitcode);
```

### gsapi_run_string


<div class="tag methodDefinition csharp"></div>

```
public static extern int gsapi_run_string(IntPtr instance,
                                          IntPtr command,
                                             int usererr,
                                         ref int exitcode);
```

### gsapi_run_string_end

<div class="tag methodDefinition csharp"></div>

```
public static extern int gsapi_run_string_end(IntPtr instance,
                                                 int usererr,
                                             ref int exitcode);
```

### gsapi_run_file

<div class="tag methodDefinition csharp"></div>

```
public static extern int gsapi_run_file(IntPtr instance,
                                        IntPtr filename,
                                           int usererr,
                                       ref int exitcode);
```

### gsapi_exit

Exit the interpreter. This must be called on shutdown if [gsapi_init_with_args] has been called, and just before [gsapi_delete_instance].

<div class="tag methodDefinition csharp"></div>

```
public static extern int gsapi_exit(IntPtr instance);
```

### gsapi_set_param

Sets a parameter.

Broadly, this is equivalent to setting a parameter using -d, -s or -p on the command line. This call cannot be made during a [gsapi_run_string] operation.

Parameters in this context are not the same as 'arguments' as processed by [gsapi_init_with_args], but often the same thing can be achieved. For example, with [gsapi_init_with_args], we can pass "-r200" to change the resolution. Broadly the same thing can be achieved by using [gsapi_set_param] to set a parsed value of "<</HWResolution [ 200.0 200.0 ]>>".

Internally, when we set a parameter, we perform an `initgraphics` operation. This means that using [gsapi_set_param] other than at the start of a page is likely to give unexpected results.

Attempting to set a parameter that the device does not recognise will be silently ignored, and that parameter will not be found in subsequent [gsapi_get_param] calls.


<div class="tag methodDefinition csharp"></div>

```
public static extern int gsapi_set_param(IntPtr instance,
                                         IntPtr param,
                                         IntPtr value,
                              gs_set_param_type type);
```


> **NOTE**<br>
> The `type` argument, as a [gs_set_param_type], dictates the kind of object that the `value` argument points to.
>

> **NOTE**<br>
> For more on the C implementation of parameters see: [Ghostscript parameters in C].
>

### gsapi_get_param

Retrieve the current value of a parameter.

If an error occurs, the return value is negative. Otherwise the return value is the number of bytes required for storage of the value. Call once with value `NULL` to get the number of bytes required, then call again with value pointing to at least the required number of bytes where the value will be copied out. Note that the caller is required to know the type of value in order to get it. For all types other than [gs_spt_string], [gs_spt_name], and [gs_spt_parsed] knowing the type means you already know the size required.

This call retrieves parameters/values that have made it to the device. Thus, any values set using [gs_spt_more_to_come] without a following call omitting that flag will not be retrieved. Similarly, attempting to get a parameter before [gsapi_init_with_args] has been called will not list any, even if [gsapi_set_param] has been used.

Attempting to read a parameter that is not set will return `gs_error_undefined` (-21). Note that calling [gsapi_set_param] followed by [gsapi_get_param] may not find the value, if the device did not recognise the key as being one of its configuration keys.

For the C documentation please refer to [Ghostscript get_param].


<div class="tag methodDefinition csharp"></div>

```
public static extern int gsapi_get_param(IntPtr instance,
                                         IntPtr param,
                                         IntPtr value,
                              gs_set_param_type type);
```

### gsapi_enumerate_params

Enumerate the current parameters. Call repeatedly to list out the current parameters.

The first call should have `iter` = NULL. Subsequent calls should pass the same pointer in so the iterator can be updated. Negative return codes indicate error, 0 success, and 1 indicates that there are no more keys to read. On success, key will be updated to point to a null terminated string with the key name that is guaranteed to be valid until the next call to [gsapi_enumerate_params]. If `type` is non NULL then the pointer `type` will be updated to have the `type` of the parameter.

> **NOTE**<br>
> Only one enumeration can happen at a time. Starting a second enumeration will reset the first.
>

The enumeration only returns parameters/values that have made it to the device. Thus, any values set using the [gs_spt_more_to_come] without a following call omitting that flag will not be retrieved. Similarly, attempting to enumerate parameters before [gsapi_init_with_args] has been called will not list any, even if [gsapi_set_param] has been used.

<div class="tag methodDefinition csharp"></div>

```
public static extern int gsapi_enumerate_params(IntPtr instance,
                                            out IntPtr iter,
                                            out IntPtr key,
                                                IntPtr type);
```

### gsapi_add_control_path

Add a (case sensitive) path to one of the lists of [permitted paths] for file access.

<div class="tag methodDefinition csharp"></div>

```
public static extern int gsapi_add_control_path(IntPtr instance,
                                                   int type,
                                                IntPtr path);
```

### gsapi_remove_control_path

Remove a (case sensitive) path from one of the lists of [permitted paths] for file access.

<div class="tag methodDefinition csharp"></div>

```
public static extern int gsapi_remove_control_path(IntPtr instance,
                                                      int type,
                                                   IntPtr path);
```

### gsapi_purge_control_paths

Clear all the paths from one of the lists of [permitted paths] for file access.
<div class="tag methodDefinition csharp"></div>

```
public static extern void gsapi_purge_control_paths(IntPtr instance,
                                                       int type);
```

### gsapi_activate_path_control

Enable/Disable path control (i.e. whether paths are checked against [permitted paths] before access is granted).

<div class="tag methodDefinition csharp"></div>

```
public static extern void gsapi_activate_path_control(IntPtr instance,
                                                         int enable);
```

### gsapi_is_path_control_active


Query whether path control is activated or not.

<div class="tag methodDefinition csharp"></div>


```
public static extern int gsapi_is_path_control_active(IntPtr instance);
```



## Callback and Callout prototypes

[GSAPI] also defines some prototype pointers which are defined as follows.

### gs_stdio_handler

<div class="tag callbackDefinition csharp"></div>

```
/* Callback proto for stdio */
public delegate int gs_stdio_handler(IntPtr caller_handle,
                                     IntPtr buffer,
                                        int len);
```


### gsPollHandler

<div class="tag callbackDefinition csharp"></div>

```
/* Callback proto for poll function */
public delegate int gsPollHandler(IntPtr caller_handle);
```


### gsCallOut

<div class="tag callbackDefinition csharp"></div>

```
/* Callout proto */
public delegate int gsCallOut(IntPtr callout_handle,
                              IntPtr device_name,
                                 int id,
                                 int size,
                              IntPtr data);

```


[structs and enums]: #structs-and-enums
[constants]: #constants
[GSAPI]: #gsapi

[Ghostscript parameters in C]: https://www.ghostscript.com/doc/current/Use.htm#Parameters
[Ghostscript get_param]: https://www.ghostscript.com/doc/current/API.htm#get_param
[gsapi_revision_t]: #gsapi_revision_t

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

[permitted paths]: https://ghostscript.com/doc/current/Use.htm#Safer

[callout handler]: #gscallout
