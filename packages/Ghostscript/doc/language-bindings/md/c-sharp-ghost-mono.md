# GhostMono

<div class="banner">
    <div class="c-sharp-text"></div>
    <div class="vendor-logo c-sharp-logo"></div>
</div>


## About

`GhostMono` is the `C#` interface into the `GhostAPI` library developed for Linux systems.

## Enums

### Tasks

The Ghostscript task type `enum` is used to inform `GhostAPI` of the type of operation which is being requested.


| Task | Description |
|---|---|
|PS_DISTILL|Task associated with converting a PostScript stream to a PDF document|
|CREATE_XPS|Task associated with outputting  a copy of a document to XPS|
|SAVE_RESULT|Task associated with saving documents|
|GET_PAGE_COUNT|Task associated with getting the page count of a document|
|GENERIC|Generic task identifier|
|DISPLAY_DEV_THUMBS|Display Device task associated with rendering thumbnails|
|DISPLAY_DEV_NON_PDF|Display Device task associated with non-PDF or non-XPS rendering <sup>[1]</sup>|
|DISPLAY_DEV_PDF|Display Device task associated with PDF & XPS rendering <sup>[1]</sup>|
|DISPLAY_DEV_RUN_FILE|Display Device task associated with running files|

Task types are defined as `GS_Task_t`.

<div class="tag enumDefinition csharp"></div>

```
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
```

### Results

Result types are defined as `GS_Result_t`.

<div class="tag enumDefinition csharp"></div>

```
public enum GS_Result_t
{
    gsOK,
    gsFAILED,
    gsCANCELLED
}
```

### Status

Status is defined as `gsStatus`.

<div class="tag enumDefinition csharp"></div>

```
public enum gsStatus
{
    GS_READY,
    GS_BUSY,
    GS_ERROR
};
```

## The Parameter Struct

The parameter struct `gsParamState_t` allows for bundles of information to be processed by Ghostscript to complete overall requests.

<div class="tag structDefinition csharp"></div>

```
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
```


### Parameters explained

Setting up your parameters (with any dedicated bespoke method(s) which your application requires) is needed when communicating directly with `GhostAPI`.

When requesting Ghostscript to process an operation an application developer should send a parameter payload which defines the details for the operation.

For example in `GhostMono` we can see the public method as follows:

<div class="tag sampleCode csharp"></div>

```
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
```

Here we can see a parameter payload being setup before being passed on to the asynchronous method `RunGhostscriptAsync` which sets up a worker thread to run according to the task type in the payload.

`GhostMono` handles many common operations on an application developer's behalf, however if you require to write your own methods to interface with `GhostAPI` then referring to the public methods in `GhostMono` is a good starting point.

For full documentation on parameters refer to [Ghostscript parameters].


## The Event class

`GhostMono` contains a public class `gsThreadCallBack`. This class is used to set and get callback information as they occur. `GhostMono` will create these payloads and deliver them back to the application layer's `ProgressCallBack` method [asynchronously].


<div class="tag classDefinition csharp"></div>

```
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
```


## GSMONO

This class should be instantiated as a member variable in your application with callback definitions setup as required.

Handlers for asynchronous operations can injected by providing your own bespoke callback methods to your instance's `ProgressCallBack` function.

<div class="tag sampleCode csharp"></div>

```
/* Set up ghostscript with callbacks for system updates */
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
```

> **NOTE**<br>
> Once a Ghostscript operation is in progress any defined callback functions will be called as the operation runs up unto completion. These callback methods are essential for your application to interpret activity events and react accordingly.
>

An explanation of callbacks and the available public methods within `GSMONO` are explained below.


### Delegates

To handle asynchronous events `GhostMONO` has four delegates which define callback methods that an application can assign to.


| Callback | Description |
|---|---|
|`DLLProblemCallBack`|Occurs if there is some issue with the Ghostscript Shared Object (`libgpdl.so`)|
|`StdIOCallBack`|Occurs if Ghostscript outputs something to `stderr` or `stdout`|
|`ProgressCallBack`|Occurs as Ghostscript makes its way through a file|
|`PageRenderedCallBack`|Occurs when a page has been rendered and the data from the display device is ready|


#### DLLProblemCallBack

<div class="tag callbackDefinition csharp"></div>

```
internal delegate void DLLProblem(String mess);
internal event DLLProblem DLLProblemCallBack;
```

#### StdIOCallBack

<div class="tag callbackDefinition csharp"></div>

```
internal delegate void StdIO(String mess,
                             int len);
internal event StdIO StdIOCallBack;
```

#### ProgressCallBack

<div class="tag callbackDefinition csharp"></div>

```
internal delegate void Progress(gsEventArgs info);
internal event Progress ProgressCallBack;
```

#### PageRenderedCallBack

<div class="tag callbackDefinition csharp"></div>

```
internal delegate void PageRendered(int width,
                                    int height,
                                    int raster,
                                 IntPtr data,
                         gsParamState_t state);
internal event PageRendered PageRenderedCallBack;
```


### GetVersion

Use this method to get Ghostscript version info as a handy `String`.

<div class="tag methodDefinition csharp"></div>

```
public String GetVersion()
```

<div class="tag sampleCode csharp"></div>

```
String gs_vers = m_ghostscript.GetVersion();
```

> **NOTE**<br>
> An exception will be thrown if there is any issue with the Ghostscript DLL.
>


### DisplayDeviceOpen

Sets up the [display device] ahead of time.


<div class="tag methodDefinition csharp"></div>

```
public gsParamState_t DisplayDeviceOpen()
```

<div class="tag sampleCode csharp"></div>

```
m_ghostscript.DisplayDeviceOpen();
```

> **NOTE**<br>
> Calling this method [instantiates ghostscript] and configures the encoding and the callbacks for the display device.
>


### DisplayDeviceClose

Closes the [display device] and deletes the instance.



<div class="tag methodDefinition csharp"></div>

```
public gsParamState_t DisplayDeviceClose()
```

<div class="tag sampleCode csharp"></div>

```
m_ghostscript.DisplayDeviceClose();
```

> **NOTE**<br>
> Calling this method [deletes ghostscript].
>


### GetPageCount

Use this method to get the number of pages in a supplied document.

<div class="tag methodDefinition csharp"></div>

```
public int GetPageCount(String fileName)
```


<div class="tag sampleCode csharp"></div>

```
int page_number = m_ghostscript.GetPageCount("my_document.pdf");
```


> **NOTE**<br>
> If Ghostscript is unable to determine the page count then this method will return `-1`.
>


### DistillPS

Launches a thread rendering all the pages of a supplied PostScript file to a PDF.

<div class="tag methodDefinition csharp async"></div>

```
public gsStatus DistillPS(String fileName, int resolution)
```

<div class="tag sampleCode csharp"></div>

```
m_ghostscript.DistillPS("my_postscript_document.ps", 300);
```

<span class="smallText alignRight">

_[asynchronous]_

</span>



### DisplayDeviceRenderAll


Launches a thread rendering all the document pages with the [display device]. For use with languages that can be indexed via pages which include PDF and XPS. [1]

<div class="tag methodDefinition csharp async"></div>

```
public gsStatus DisplayDeviceRenderAll(String fileName, double zoom, bool aa, GS_Task_t task)
```

<div class="tag sampleCode csharp"></div>

```
m_ghostscript.DisplayDeviceRenderAll("my_document.pdf",
                                     0.1,
                                     false,
                                     GS_Task_t.DISPLAY_DEV_THUMBS_NON_PDF);
```

<span class="smallText alignRight">

_[asynchronous]_

</span>


### DisplayDeviceRenderThumbs

Launches a thread rendering all the pages with the [display device] to collect thumbnail images.

Recommended zoom level for thumbnails is between 0.05 and 0.2, additionally anti-aliasing is probably not required.

<div class="tag methodDefinition csharp async"></div>

```
public gsStatus DisplayDeviceRenderThumbs(String fileName,
                                          double zoom,
                                            bool aa)
```

<div class="tag sampleCode csharp"></div>

```
m_ghostscript.DisplayDeviceRenderThumbs("my_document.pdf",
                                        0.1,
                                        false);
```

<span class="smallText alignRight">

_[asynchronous]_

</span>


### DisplayDeviceRenderPages


Launches a thread rendering a set of pages with the [display device]. For use with languages that can be indexed via pages which include PDF and XPS. <sup>[1]</sup>




<div class="tag methodDefinition csharp async"></div>

```
public gsStatus DisplayDeviceRenderPages(String fileName,
                                            int first_page,
                                            int last_page,
                                         double zoom)
```

<div class="tag sampleCode csharp"></div>

```
m_ghostscript.DisplayDeviceRenderPages("my_document.pdf",
                                       0,
                                       9,
                                       1.0);
```

<span class="smallText alignRight">

_[asynchronous]_

</span>


### GetStatus

Returns the current [status] of `Ghostscript`.

<div class="tag methodDefinition csharp"></div>

```
public gsStatus GetStatus()
```

<div class="tag sampleCode csharp"></div>

```
gsStatus status = m_ghostscript.GetStatus();
```


### GhostscriptException

An application developer can log any exceptions in this public class as required by editing the constructor.


<div class="tag classDefinition csharp"></div>

```
public class GhostscriptException : Exception
{
    public GhostscriptException(string message) : base(message)
    {
        // Report exceptions as required
    }
}
```


## Notes

#### 1: Ghostscript & Page Description Languages

Ghostscript handles the following [PDLs]: `PCL` `PDF` `PS` `XPS`.

`PCL` and `PS` do not allow random access, meaning that, to print page 2 in a 100 page document, Ghostscript has to read the entire document stream of 100 pages.

On the other hand, `PDF` and `XPS` allow for going directly to page 2 and then only dealing with that content. The tasks `DISPLAY_DEV_NON_PDF` and `DISPLAY_DEV_PDF` keep track of what sort of input Ghostscript is dealing with and enables the application to direct progress or completion callbacks accordingly.


[PDLs]: https://en.wikipedia.org/wiki/Page_description_language
[GhostAPI]: c-sharp-ghost-api.html
[gsapi_revision]: c-sharp-ghost-api.html#gsapi_revision
[Ghostscript parameters]: https://www.ghostscript.com/doc/current/Use.htm#Parameters
[display device]: https://ghostscript.com/doc/current/Devices.htm#Display_devices
[instantiates ghostscript]: c-sharp-ghost-api.html#gsapi_new_instance
[deletes ghostscript]: c-sharp-ghost-api.html#gsapi_delete_instance
[status]: #status
[notes]: #notes
[1]: #1-ghostscript-page-description-languages

[GSMono instantiation]: #gsmono
[asynchronous]: #delegates
[asynchronously]: #delegates
