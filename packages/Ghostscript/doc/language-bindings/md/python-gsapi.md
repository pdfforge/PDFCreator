# gsapi.py

<div class="banner">
<div class="python-text"></div>
<div class="vendor-logo python-logo"></div>
</div>


## About

`gsapi.py` is the `Python` binding into the Ghostscript `C` library.

Assuming that the [Ghostscript library has been built] for your project then `gsapi` should be imported into your own `Python` scripts for API usage.

<div class="tag sampleCode python"></div>

```
import gsapi
```



## gsapi

### Overview

Implemented using Python's ctypes module.

All functions have the same name as the `C` function that they wrap.

Functions raise a `GSError` exception if the underlying function returned a
negative [error code].

Functions that don't have out-params return `None`. Out-params are returned
directly (using tuples if there are more than one).


### Return codes

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


### gsapi_revision

Returns a `gsapi_revision_t`.

This method returns the revision numbers and strings of the Ghostscript interpreter library; you should call it before any other interpreter library functions to make sure that the correct version of the Ghostscript interpreter has been loaded.

<div class="tag methodDefinition python"></div>

```
def gsapi_revision()
```

<div class="tag sampleCode python"></div>

```
version_info = gsapi.gsapi_revision()
print (version_info)
```

### gsapi_new_instance

Returns a new instance of Ghostscript to be used with other `gsapi_*()` functions.

<div class="tag methodDefinition python"></div>

```
def gsapi_new_instance(caller_handle)
```

#### Parameters

`caller_handle`: Typically unused, but is passed to callbacks e.g. via `gsapi_set_stdio()`. Must be convertible to a `C void*`, so `None` or an
    integer is ok but other types such as strings will fail.


<div class="tag sampleCode python"></div>

```
instance = gsapi.gsapi_new_instance(1)
```


### gsapi_delete_instance

Destroy an instance of Ghostscript. Before you call this, Ghostscript should ensure to have finished any processes.

<div class="tag methodDefinition python"></div>

```
def gsapi_delete_instance(instance)
```
#### Parameters

`instance`: Your instance of Ghostscript.

<div class="tag sampleCode python"></div>

```
gsapi.gsapi_delete_instance(instance)
```



### gsapi_set_stdio

Set the callback functions for `stdio`, together with the handle to use in the callback functions.

<div class="tag methodDefinition python"></div>

```
def gsapi_set_stdio(instance, stdin_fn, stdout_fn, stderr_fn)
```
#### Parameters

`instance`: Your instance of Ghostscript.

`stdin_fn`: If not `None`, will be called with:
- `(caller_handle, text, len_)`:
    - `caller_handle`: As passed originally to `gsapi_new_instance()`.
    - `text`: A `ctypes.LP_c_char` of length `len_`.

`stdout_fn` and `stderr_fn`:If not `None`, called with:
- `(caller_handle, text)`:
    - `caller_handle`: As passed originally to `gsapi_new_instance()`.
    - `text`: A Python bytes object.

Should return the number of bytes of `text` that they handled; for convenience `None` is converted to `len(text)`.

<div class="tag sampleCode python"></div>

```
def stdout_fn(caller_handle, bytes_):
    sys.stdout.write(bytes_.decode('latin-1'))

gsapi.gsapi_set_stdio(instance, None, stdout_fn, None)
print('gsapi_set_stdio() ok.')
```


### gsapi_set_poll

Set the callback function for polling.

<div class="tag methodDefinition python"></div>

```
def gsapi_set_poll(instance, poll_fn)
```
#### Parameters

`instance`: Your instance of Ghostscript.

`poll_fn`: Will be called with `caller_handle` as passed
    to `gsapi_new_instance()`.

<div class="tag sampleCode python"></div>

```
def poll_fn(caller_handle, bytes_):
    sys.stdout.write(bytes_.decode('latin-1'))

gsapi.gsapi_set_poll(instance, poll_fn)
print('gsapi_set_poll() ok.')
```

### gsapi_set_display_callback

Sets the [display] callback.

<div class="tag methodDefinition python"></div>

```
def gsapi_set_display_callback(instance, callback)
```

#### Parameters

`instance`: Your instance of Ghostscript.

`callback`: Must be a `display_callback` instance.

<div class="tag sampleCode python"></div>

```
d = display_callback()
gsapi.gsapi_set_display_callback(instance, d)
print('gsapi_set_display_callback() ok.')
```

### gsapi_set_arg_encoding

Set the encoding used for the interpretation of all subsequent arguments supplied via the `GhostAPI` interface on this instance. By default we expect args to be in encoding `0` (the 'local' encoding for this OS). On Windows this means "the currently selected codepage". On Linux this typically means `utf8`. This means that omitting to call this function will leave Ghostscript running exactly as it always has.

This must be called after [gsapi_new_instance] and before [gsapi_init_with_args].


<div class="tag methodDefinition python"></div>

```
def gsapi_set_arg_encoding(instance, encoding)
```

#### Parameters

`instance`: Your instance of Ghostscript.

`encoding`: Encoding must be one of:

|Encoding enum|Value|
|---|---|
|`GS_ARG_ENCODING_LOCAL`|0|
|`GS_ARG_ENCODING_UTF8`|1|
|`GS_ARG_ENCODING_UTF16LE`|2|

<div class="tag sampleCode python"></div>

```
gsapi.gsapi_set_arg_encoding(instance, gsapi.GS_ARG_ENCODING_UTF8)
```

>**NOTE**<br>
> Please note that use of the 'local' encoding (`GS_ARG_ENCODING_LOCAL`) is now deprecated and should be avoided in new code.

### gsapi_set_default_device_list

Set the string containing the list of default device names, for example "display x11alpha x11 bbox". Allows the calling application to influence which device(s) Ghostscript will try, in order, in its selection of the default device. This must be called after [gsapi_new_instance] and before [gsapi_init_with_args].


<div class="tag methodDefinition python"></div>

```
def gsapi_set_default_device_list(instance, list_)
```

#### Parameters

`instance`: Your instance of Ghostscript.

`list_`: A string of device names.

<div class="tag sampleCode python"></div>

```
gsapi.gsapi_set_default_device_list(instance, 'bmp256 bmp32b bmpgray cdeskjet cdj1600 cdj500')
```


### gsapi_get_default_device_list

Returns a string containing the list of default device names. This must be called after [gsapi_new_instance] and before [gsapi_init_with_args].

<div class="tag methodDefinition python"></div>

```
def gsapi_get_default_device_list(instance)
```

#### Parameters

`instance`: Your instance of Ghostscript.

<div class="tag sampleCode python"></div>

```
device_list = gsapi.gsapi_get_default_device_list(instance)
print(device_list)
```


### gsapi_init_with_args

To initialise the interpreter, pass your `instance` of Ghostscript and your argument variables with `args`.

<div class="tag methodDefinition python"></div>

```
def gsapi_init_with_args(instance, args)
```

#### Parameters

`instance`: Your instance of Ghostscript.

`args`: A list/tuple of strings.

<div class="tag sampleCode python"></div>

```
in_filename = 'tiger.eps'
out_filename = 'tiger.pdf'
params = ['gs', '-dNOPAUSE', '-dBATCH', '-sDEVICE=pdfwrite',
          '-o', out_filename, '-f', in_filename]
gsapi.gsapi_init_with_args(instance, params)
```

### gsapi_run_*

Returns an [exit code] or an exception on error.

There is a 64 KB length limit on any buffer submitted to a `gsapi_run_*` function for processing. If you have more than 65535 bytes of input then you must split it into smaller pieces and submit each in a separate [gsapi_run_string_continue] call.

> **NOTE**<br>
> All these functions return an [exit code]

### gsapi_run_string_begin

Starts a `run_string_` operation.

<div class="tag methodDefinition python"></div>

```
def gsapi_run_string_begin(instance, user_errors)
```

#### Parameters

`instance`: Your instance of Ghostscript.

`user_errors`: An `int`, for more see [user errors parameter explained].


<div class="tag sampleCode python"></div>

```
exitcode = gsapi.gsapi_run_string_begin(instance, 0)
```

### gsapi_run_string_continue

Processes file byte data (`str_`) to feed as chunks into Ghostscript. This method should typically be called within a buffer context.

> **NOTE**<br>
> An exception is _not_ raised for the `gs_error_NeedInput` return code.

<div class="tag methodDefinition python"></div>

```
def gsapi_run_string_continue(instance, str_, user_errors)
```

#### Parameters

`instance`: Your instance of Ghostscript.

`str_`: Should be either a Python string or a bytes object. If the former,
it is converted into a bytes object using `utf-8` encoding.

`user_errors`: An `int`, for more see [user errors parameter explained].

<div class="tag sampleCode python"></div>

```
exitcode = gsapi.gsapi_run_string_continue(instance, data, 0)
```

> **NOTE**<br>
> For the return code, we don't raise an exception for `gs_error_NeedInput`.

### gsapi_run_string_with_length

Processes file byte data (`str_`) to feed into Ghostscript when the `length` is known and the file byte data is immediately available.

<div class="tag methodDefinition python"></div>

```
def gsapi_run_string_with_length(instance, str_, length, user_errors)
```

#### Parameters

`instance`: Your instance of Ghostscript.

`str_`: Should be either a Python string or a bytes object. If the former,
it is converted into a bytes object using `utf-8` encoding.

`length`: An `int` representing the length of `gsapi_run_string_with_length`.

`user_errors`: An `int`, for more see [user errors parameter explained].

<div class="tag sampleCode python"></div>

```
gsapi.gsapi_run_string_with_length(instance,"hello",5,0)
```


> **NOTE**<br>
> If using this method then ensure that the file byte data will fit into a single (<64k) buffer.
>


### gsapi_run_string

Processes file byte data (`str_`) to feed into Ghostscript.

<div class="tag methodDefinition python"></div>

```
def gsapi_run_string(instance, str_, user_errors)
```

#### Parameters

`instance`: Your instance of Ghostscript.

`str_`: Should be either a Python string or a bytes object. If the former,
it is converted into a bytes object using `utf-8` encoding.

`user_errors`: An `int`, for more see [user errors parameter explained].

<div class="tag sampleCode python"></div>

```
gsapi.gsapi_run_string(instance,"hello",0)
```


> **NOTE**<br>
> This method can only work on a standard, null terminated C string.
>


### gsapi_run_string_end

Ends a `run_string_` operation.

<div class="tag methodDefinition python"></div>

```
def gsapi_run_string_end(instance, user_errors)
```

#### Parameters

`instance`: Your instance of Ghostscript.

`user_errors`: An `int`, for more see [user errors parameter explained].

<div class="tag sampleCode python"></div>

```
exitcode = gsapi.gsapi_run_string_end(instance, 0)
```



### gsapi_run_file

Runs a file through Ghostscript.

<div class="tag methodDefinition python"></div>

```
def gsapi_run_file(instance, filename, user_errors)
```

#### Parameters

`instance`: Your instance of Ghostscript.

`filename`: String representing file name.

`user_errors`: An `int`, for more see [user errors parameter explained].

<div class="tag sampleCode python"></div>

```
in_filename = 'tiger.eps'
gsapi.gsapi_run_file(instance, in_filename, 0)
```


> **NOTE**<br>
> This will process the supplied input file with any previously supplied [argument parameters].
>


### gsapi_exit

Exit the interpreter. This must be called on shutdown if [gsapi_init_with_args] has been called, and just before [gsapi_delete_instance].

<div class="tag methodDefinition python"></div>


```
def gsapi_exit(instance)
```

<div class="tag sampleCode python"></div>

```
gsapi.gsapi_exit(instance)
```


### gsapi_set_param

Sets a parameter. Broadly, this is equivalent to setting a parameter using -d, -s or -p on the command line. This call cannot be made during a [gsapi_run_string] operation.

Parameters in this context are not the same as 'arguments' as processed by [gsapi_init_with_args], but often the same thing can be achieved. For example, with [gsapi_init_with_args], we can pass "-r200" to change the resolution. Broadly the same thing can be achieved by using [gsapi_set_param] to set a parsed value of "<</HWResolution [ 200.0 200.0 ]>>".

Internally, when we set a parameter, we perform an `initgraphics` operation. This means that using [gsapi_set_param] other than at the start of a page is likely to give unexpected results.

Attempting to set a parameter that the device does not recognise will be silently ignored, and that parameter will not be found in subsequent [gsapi_get_param] calls.

<div class="tag methodDefinition python"></div>

```
def gsapi_set_param(instance, param, value, type_=None)
```

#### Parameters

`instance`: Your instance of Ghostscript.

`param`: Name of parameter, either a bytes or a str; if str it is encoded using latin-1.

`value`: A bool, int, float, bytes or str. If str, it is encoded into a bytes using utf-8.


`type_`: If `type_` is not `None`, `value` must be convertible to the Python type implied by `type_`:

#### Parameter list

| `type_` | Python type(s) |
|---|---|
|gs_spt_null | [Ignored] |
|gs_spt_bool  | bool |
|gs_spt_int    | int |
|gs_spt_float   | float |
|gs_spt_name     | [Error] |
|gs_spt_string  | (bytes, str) |
|gs_spt_long    | int |
|gs_spt_i64     | int |
|gs_spt_size_t | int |
|gs_spt_parsed  | (bytes, str) |
|gs_spt_more_to_come | (bytes, str) |


An exception is raised if `type_` is an integer type and `value` is outside its range.

If `type_` is `None`, we choose something suitable for type of `value`:

| Python type of `value` | `type_` |
|---|---|
| bool        |            gs_spt_bool |
| int            |         gs_spt_i64 |
| float           |        gs_spt_float |
| bytes           |        gs_spt_parsed |
| str              |       gs_spt_parsed (encoded with utf-8) |

If `value` is `None`, we use `gs_spt_null`.

Otherwise `type_` must be a `gs_spt_*` except for `gs_spt_invalid` and `gs_spt_name`.

> **NOTE**<br>
> This implementation supports automatic inference of type by looking at the type of `value`.
>

<div class="tag sampleCode python"></div>

```
set_margins = gsapi.gsapi_set_param(instance, "Margins", "[10 10]")
```


> **NOTE**<br>
> For more on the `C` implementation of parameters see: [Ghostscript parameters in C].
>


### gsapi_get_param

Retrieve the current value of a parameter.

If an error occurs, the return value is negative. Otherwise the return value is the number of bytes required for storage of the value. Call once with value `NULL` to get the number of bytes required, then call again with value pointing to at least the required number of bytes where the value will be copied out. Note that the caller is required to know the type of value in order to get it. For all types other than [gs_spt_string], [gs_spt_name], and [gs_spt_parsed] knowing the type means you already know the size required.

This call retrieves parameters/values that have made it to the device. Thus, any values set using [gs_spt_more_to_come] without a following call omitting that flag will not be retrieved. Similarly, attempting to get a parameter before [gsapi_init_with_args] has been called will not list any, even if [gsapi_set_param] has been used.

Attempting to read a parameter that is not set will return `gs_error_undefined` (-21). Note that calling [gsapi_set_param] followed by [gsapi_get_param] may not find the value, if the device did not recognise the key as being one of its configuration keys.

For the `C` documentation please refer to [Ghostscript get_param].

<div class="tag methodDefinition python"></div>

```
def gsapi_get_param(instance, param, type_=None, encoding=None)
```

#### Parameters

`instance`: Your instance of Ghostscript.

`param`: Name of parameter, either a `bytes` or `str`; if a `str` it is encoded using `latin-1`.

`type_`: A `gs_spt_*` constant or `None`. If `None` we try each `gs_spt_*` until one succeeds; if none succeeds we raise the last error.

`encoding`: Only affects string values. If `None` we return a `bytes` object, otherwise it should be the encoding to use to decode into a string, e.g. 'utf-8'.

<div class="tag sampleCode python"></div>

```
get_margins = gsapi.gsapi_get_param(instance, "Margins")
```


### gsapi_enumerate_params


Enumerate the current parameters on the instance of Ghostscript.

Returns an array of `(key, value)` for each parameter. `key` is decoded as `latin-1`.

<div class="tag methodDefinition python"></div>

```
def gsapi_enumerate_params(instance)
```

#### Parameters

`instance`: Your instance of Ghostscript.

<div class="tag sampleCode python"></div>

```
for param, type_ in gsapi.gsapi_enumerate_params(instance):
    val = gsapi.gsapi_get_param(instance,param, encoding='utf-8')
    print('%-24s : %s' % (param, val))
```





### gsapi_add_control_path


Add a (case sensitive) path to one of the lists of [permitted paths] for file access.

<div class="tag methodDefinition python"></div>

```
def gsapi_add_control_path(instance, type_, path)
```

#### Parameters

`instance`: Your instance of Ghostscript.

`type_`: An `int` which must be one of:

|Enum|Value|
|---|---|
|GS_PERMIT_FILE_READING|0|
|GS_PERMIT_FILE_WRITING|1|
|GS_PERMIT_FILE_CONTROL|2|

`path`: A `string` representing the file path.

<div class="tag sampleCode python"></div>

```
gsapi.gsapi_add_control_path(instance, gsapi.GS_PERMIT_FILE_READING, "/docs/secure/")
```



### gsapi_remove_control_path

Remove a (case sensitive) path from one of the lists of [permitted paths] for file access.

<div class="tag methodDefinition python"></div>

```
def gsapi_remove_control_path(instance, type_, path)
```

#### Parameters

`instance`: Your instance of Ghostscript.

`type_`: An `int` representing the permission type.

`path`: A `string` representing the file path.

<div class="tag sampleCode python"></div>

```
gsapi.gsapi_remove_control_path(instance, gsapi.GS_PERMIT_FILE_READING, "/docs/secure/")
```




### gsapi_purge_control_paths

Clear all the paths from one of the lists of [permitted paths] for file access.

<div class="tag methodDefinition python"></div>

```
def gsapi_purge_control_paths(instance, type_)
```

#### Parameters

`instance`: Your instance of Ghostscript.

`type_`: An `int` representing the permission type.


<div class="tag sampleCode python"></div>

```
gsapi.gsapi_purge_control_paths(instance, gsapi.GS_PERMIT_FILE_READING)
```


### gsapi_activate_path_control

Enable/Disable path control (i.e. whether paths are checked against [permitted paths] before access is granted).

<div class="tag methodDefinition python"></div>

```
def gsapi_activate_path_control(instance, enable)
```

#### Parameters

`instance`: Your instance of Ghostscript.

`enable`: `bool` to enable/disable path control.

<div class="tag sampleCode python"></div>

```
gsapi.gsapi_activate_path_control(instance, true)
```

### gsapi_is_path_control_active

Query whether path control is activated or not.


<div class="tag methodDefinition python"></div>

```
def gsapi_is_path_control_active(instance)
```

#### Parameters

`instance`: Your instance of Ghostscript.

<div class="tag sampleCode python"></div>

```
isActive = gsapi.gsapi_is_path_control_active(instance)
```

## Notes

#### 1: User errors parameter

The `user_errors` argument is normally set to zero to indicate that errors should be handled through the normal mechanisms within the interpreted code. If set to a negative value, the functions will return an error code directly to the caller, bypassing the interpreted language. The interpreted language's error handler is bypassed, regardless of `user_errors` parameter, for the `gs_error_interrupt` generated when the polling callback returns a negative value. A positive `user_errors` is treated the same as zero.

[Ghostscript library has been built]: python-intro.html#platform-setup

[display]: https://ghostscript.com/doc/current/API.htm#display
[Ghostscript parameters in C]: https://www.ghostscript.com/doc/current/Use.htm#Parameters
[Ghostscript get_param]: https://www.ghostscript.com/doc/current/API.htm#get_param


[error code]: #return-codes
[exit code]: #return-codes
[return code]: #return-codes
[user errors parameter explained]: #1-user-errors-parameter

[argument parameters]: #gsapi_init_with_args
[gsapi_new_instance]: #gsapi_new_instance
[gsapi_delete_instance]: #gsapi_delete_instance
[gsapi_init_with_args]: #gsapi_init_with_args
[gsapi_run_string_continue]: #gsapi_run_string_continue
[gsapi_run_string_begin]: #gsapi_run_string_begin
[gsapi_run_string]: #gsapi_run_string
[gsapi_exit]: #gsapi_exit

[permitted paths]: https://ghostscript.com/doc/current/Use.htm#Safer


[gsapi_set_param]: #gsapi_set_param
[gsapi_get_param]: #gsapi_get_param
[gsapi_enumerate_params]: #gsapi_enumerate_params

[gs_spt_string]: #parameter-list
[gs_spt_name]: #parameter-list
[gs_spt_parsed]: #parameter-list
[gs_spt_more_to_come]: #parameter-list
