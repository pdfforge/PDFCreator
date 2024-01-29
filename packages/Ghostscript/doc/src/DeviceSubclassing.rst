.. Copyright (C) 2001-2023 Artifex Software, Inc.
.. All Rights Reserved.

.. title:: Device Subclassing


.. include:: header.rst

.. _DeviceSubclassing.html:


Device Subclassing
==========================================






Devices in the graphics state
--------------------------------

The 'device' is a member of the graphics state in PostScript, and is subject to gsave/grestore, like any other part of the graphics state. This is important for PostScript as it allows us to, for example, push the null device, perform some operations, and then grestore back to the original rendering device.

In PostScript and PDF, the graphics state is itself a garbage collected object, as is the device. This means that we store a pointer to the device in the graphics state, which forces the other interpreters to do so as well. Now an important implication of this is that it is then only possible, currently, to change devices by altering the graphics state entry to point at a different device structure, which means that the graphics state must be available in order to do so. Clearly at the interpreter level this isn't a problem, but at lower levels the graphics state may not be available, not all our device methods pass on a graphics state pointer for example. Without that pointer we can't change the graphics state and therefore can't point at a different device.



Chaining devices
--------------------------------
There are times when it is useful to be able to chain devices one after another, examples include the PDF transparency device, the pattern accumulator device and others. Some comments in early code indicate that the ability to chain devices was an original design goal, though its likely that originally this was only intended to work by installing devices from the interpreter level.

Currently there are a number of different ways to install devices, and the sheer number, and the methods themselves, show why there is a problem. Lets consider them separately.

- Forwarding devices. These devices generally intercept the usual graphics marking operations, and pass them on to the underlying device, or record some features of interest. Examples of these include the bbox device and the pattern accumulator. These work very well, but require the graphics state to be available when we need to insert them ahead of the 'underlying' device. This can be a problem, as we'll see later.

- The device filter stack. This appears to be an early attempt at providing a way to chain devices together. From traces remaining in the code this seems to originally have been intended to implement the PDF 1.4 transparency rendering. Its clear, however, that it is no longer used for that purpose and appears to be completely redundant. In any event it also requires access to the graphics state and so would suffer the same limitation.

- The clist. The clist code uses a very brute force approach to the problem, and only works for a single device. When a device is turned into a clist device, the code essentially guts the device and rewrites it (it replaces the overwritten entries later). This does work (though it seems rather horrendous to me) and doesn't require access to the graphics state because we simply reuse the existing memory of the original device. However this doesn't get us a 'chain', the clist simply morphs the existing device into something different.

- The PDF transparency compositor. This works in several ways to install itself, but the one of most interest is the case where we return from the device 'create_compositor' method. The interpreter is required to treat the return value as a pointer to a device, and if its not the same as the device currently in the graphics state, then it alters the graphics state to point to the new device. I'm not sure why its done this way as we do actually have access to the graphics state in this method and could presumably install the device ourselves. However it is not useful as a general purpose method for installing devices as it requires the return value to be acted upon by the interpreter. Making this general would require us to modify all the existing device methods, and have the interpreters check the return value, it would almost certainly be simpler (and more useful) to alter the methods to always include a graphics state.

- The 'spy' device. This is a somewhat non-standard device written by Robin, it works by copying the pointer to the device methods, and then replacing all the device methods, it is thus very similar to the clist device.



Taken together we have a number of different routes to install devices, but none of them is totally satisfactory for the goal of creating a chain of devices without access to the graphics state. However I think the existence of these manifold routes do indicate that there has been a recognition in the past that this would be useful to have.


Subclassing
------------------

From the above, its clear that we need to use some approach similar to the clist and spy devices. We must retain the original device memory, because that is what the graphics state points to, which means that we must reuse that memory in some fashion. By making a copy of the existing device elsewhere, we can then recycle the existing device.

As a short summary, that's precisely what the device subclassing code does. It examines the existing device, makes a new device structure sufficiently large to take a copy of it, and copies the existing device and all its state to that new memory. It then uses the prototype supplied for the new class, and writes that into the original device memory.

New device members
~~~~~~~~~~~~~~~~~~~~

In order to chain the devices together, I've added three new pointers to the device structure (so that all devices can be chained), a child, a parent and a 'data' pointer (see below). Whenever we subclass a device the copied device gets its parent member set to the original device pointer, and the new device points to the copied one using the child member.

Now a subtle point here is that, because we cannot change the size of the memory allocated for the original device (as that might relocate it, invalidating the graphics state pointer) its absolutely vital that the new device must be no larger than the old device. This means we have to be careful about data which the new device needs, we can't simply store it in the device structure as we usually do. To resolve that I've also added a void pointer to hold private data, the new device should allocate as much data as it requires (eg a structure) and store a pointer to that data in the subclass_data pointer. I'd recommend that this is allocated from non-GC memory except in special cases. This should mean that subclassing devices are never larger than a gx_device structure and so never overflow.

Subclassing made easy
~~~~~~~~~~~~~~~~~~~~~~~~~

In order to minimise the number of methods that a subclassing device needs to define I've created 'default' methods for all the possible device methods. Subclassing devices should use these in their prototype for any methods they don't handle. Additionally these can be used to pass on any methods after processing by the subclassing device, if required.



Example uses
-----------------

The original justification for this work was to create a device which would add 'first page' and 'last page' functionality to all the languages and all the devices. The gdevflp.c file incorporates exactly that, it has been added to gdevprn.c and gdevvec.c so that any device based on either of these two basic devices will simply work. Additionally other devices which are not based on either of these (eg the 'bit' device) have been appropriately modified.

If ``-dFirstPage`` or ``-dLastPage`` is set the parameter is parsed out and the existing device is subclassed by the new First/Last page device. This device simply drops all marking operations, and 'output_page' operations until we reach the page defined by FirstPage, and then passes everything through to the real device until we pass the page defined by ``LastPage`` at which point it throws everything away again until the end of the input.

Now, this is not as efficient as the PDF interpreter's usage, which only passes those pages required to be rendered. So the old functionality has been preserved in the PDF interpreter, exactly as before. In order to prevent the subclassing device addionally acting on the same parameters, there is a ``DisablePageHandler`` flag which is set by the PDF interpreter.

In order to test the chaining of devices together, I also created an 'object filtering' device; as with the first/last page device this has been added to all devices and any device based off gdevprn or gdevvec should inherit the functionality directly. This device allows objects to be dropped based on their type (text, image or linework) using the parameters ``-dFILTERTEXT`` ``-dFILTERIMAGE`` and ``-dFILTERVECTOR``, if a parameter is set then objects of that type will not be rendered.

Finally the PCL interpreter now uses a subclassing device to implement the 'monochrome palette' rendering. Previously this directly modified the ``color_procs`` in the current device, which I suspect was prone to potential failure (for example if a forwarding device had been pushed). This eliminated a somewhat ugly hack (in fairness its not obvious what would have been better), as well as allowing me to do away with some global variables in the PCL interpreter.



Observations
---------------

The monochrome palette device illuminated an interesting problem in the graphics library. Normally the device calls the graphics library for rendering services, but the ``color_procs`` work in reverse, the graphics library calls the device directly in order to map the colours. In the case of chained devices this meant that only the final device was being called. This was unacceptable in a situation involving chained devices, it seemed obvious to me that the graphics library should pass the request to the head of the device chain for processing. There is no simple way to do this (no access to the graphics state!) so instead I used the linked list of child/parent pointers to walk up to the head of the list, and then submit the request to that device.

The ICC profile code has a ``get_profile`` method in the device API to retrieve a profile, but it has no corresponding ``set_profile`` method, it simply sets the structure member in the current device. This caused some serious problems. Consider the case where the ICC code executes a ``get_profile`` and no profile is yet set (returning NULL). The code would then create a profile and attach it directly to the current device. It then executes ``get_profile`` again. If the current device was a subclassing device, or a forwarding device, then the 'set' would have set the profile in that device, but the 'get' would retrieve it from the underlying device, which would still be NULL. Since the ICC code didn't test the result on the second call this caused a segmentation fault. I've modified the ICC code to set the profile in the lowest device, but this probably ought to be improved to define a new ``set_profile`` method.

The tag devices write an extra colour plane in the output, where the value of the sample encodes the type of graphic that was rendered at that point (text, image, path, untouched or unknown). This is done by encoding the type in the color, which is performed by the graphics library. Now, when the type of operation changes (eg from image to text) we need to tell the graphics library that there has been a change. We do this by calling the ``set_graphics_type_tag`` device method. However, when encoding a color, the graphics library does not, as one might expect call a matching ``get_graphics_type`` method, instead it directly inspects the devcie structure and reads the ``graphics_type_tag`` member. This means that all devices in the chain must maintain this member, the implication is that if a subclassing device should implement its own ``set_graphics_type_tag`` method, it must update the ``graphics_type_tag`` in its device structure appropriately, and pass the method on to the next device in the chain. The default method already does this.

Its pretty clear from reading through the code that the original intention of the device methods is that all methods in a device should be filled in, they should never be NULL (exception; ``fill_rectangle`` is deliberately excluded from this) if a device does not implement a method then ``fill_in_procs`` should set a default method (which may legitimately simply throw an error). Over time this decision has not been enforced with the result that we now have some places where methods may be NULL. This has meant that there are places in the code which have to check for a method being NULL before using it, which is (I think) exactly what we were originally trying to avoid. Worse, since there is no list of which methods may be NULL a sensible developer would have to test all methods before use. Worst of all, it looks like some places may take a NULL method as having specific meaning (``gsdparam.c`` line 272). We should really tidy this up.

General observations are recorded in comments in ``gdevsclass.c``.


Worked example
---------------

To see how to use the device subclassing we'll take a concrete example and implement it in a real device. For the purposes of the example we'll do a 'force black text' device and add it to the TIFF device(s). The reason for choosing the TIFF devices is that these already have an ``open_device`` method which we are going to use to install the subclassing device. This isn't essential, you could install the device at any point, but its convenient.

The first thing we need to do is add some control to turn this feature off and on, this is normally done by setting device parameters on the command line. To implement this we will need to modify the ``put_params`` and ``get_params`` methods. Note it is important to add new parameters to both the get and put methods, or an error will occur. We'll call our new parameter ``ForceBlackText``.

First we add the new parameter to the TIFF device, defined in ``gdevtifs.h``:


.. code-block:: c

   typedef struct gx_device_tiff_s {
   gx_device_common;
   gx_prn_device_common;
   bool ForceBlackText;
   bool BlackTextHandlerPushed;
   ....
   }

Note that we also have a boolean value ``BlackTextHandlerPushed`` to track whether the device is already pushed or not, we'll want to use this later.

We also need to add default values to the device prototypes, which are defined in ``gdevtfnx.c``:


.. code-block:: c

   const gx_device_tiff gs_tiff12nc_device = {
   prn_device_std_body(gx_device_tiff, tiff12_procs, "tiff12nc",
   DEFAULT_WIDTH_10THS, DEFAULT_HEIGHT_10THS,
   X_DPI, Y_DPI,
   0, 0, 0, 0,
   24, tiff12_print_page),
   0, /* ForceBlacktext */
   0, /* BlacktextHandlerPushed */
   arch_is_big_endian /* default to native endian (i.e. use big endian iff the platform is so*/,


Repeat for ``gs_tiff24nc_device`` and ``gs_tiff48nc_device``. For unknown reasons, the remaining TIFF devices are in ``gdevtsep.c``, we need to make the same changes to the prototypes there; ``gs_tiffgray_device`` ``gs_tiffscaled_device`` ``gs_tiffscaled8_device`` ``gs_tiffscaled24_device`` ``gs_tiffscaled32_device`` ``gs_tiffscaled4_device`` ``gs_tiff32nc_device`` ``gs_tiff64nc_device`` and finally ``tiffsep_devices_body``.

Now we add code to ``put_params`` and ``get_params`` to set the parameter, and report its value back to the interpreter. In ``gdevtifs.c``:


.. code-block:: c

   static int tiff_get_some_params(gx_device * dev, gs_param_list * plist, int which)
   {
      gx_device_tiff *const tfdev = (gx_device_tiff *)dev;
      int code = gdev_prn_get_params(dev, plist);
      int ecode = code;
      gs_param_string comprstr;

      if ((code = param_write_bool(plist, "ForceBlackText", &tfdev->ForceBlackText)) < 0)
      ecode = code;
      ....
   }

   static int tiff_put_some_params(gx_device * dev, gs_param_list * plist, int which)
   {
      gx_device_tiff *const tfdev = (gx_device_tiff *)dev; int ecode = 0;
      int code;
      const char *param_name;
      bool big_endian = tfdev->BigEndian;
      bool usebigtiff = tfdev->UseBigTIFF;
      uint16 compr = tfdev->Compression;
      gs_param_string comprstr;
      long downscale = tfdev->DownScaleFactor;
      long mss = tfdev->MaxStripSize;
      long aw = tfdev->AdjustWidth;
      long mfs = tfdev->MinFeatureSize;
      bool ForceBlackText = tfdev->ForceBlackText;

      /* Read ForceBlackText option as bool */
      switch (code = param_read_bool(plist, (param_name = "ForceBlackText"), &ForceBlackText)) {
         default:
         ecode = code;
         param_signal_error(plist, param_name, ecode);
         case 0:
         case 1:
         break;
      }

      /* Read BigEndian option as bool */
      switch (code = param_read_bool(plist, (param_name = "BigEndian"), &big_endian)) {
         ...
         tfdev->MinFeatureSize = mfs;
         tfdev->ForceBlackText = ForceBlackText;
         return code;
      }
   }


Checking the TIFF device's open method (tiff_open) we see that it already potentially has two subclassing devices, this is because it doesn't inherit from ``gdev_prn``, if it did this functionality would be inherited as well. We can just go ahead and add our new subclassing device in here.


.. code-block:: c

   int tiff_open(gx_device *pdev)
   {
      gx_device_printer * const ppdev = (gx_device_printer *)pdev;
      gx_device_tiff *tfdev = (gx_device_tiff *)pdev;
      int code;
      bool update_procs = false;

      /* Use our own warning and error message handlers in libtiff */
      tiff_set_handlers();

      if (!tfdev->BlackTextHandlerPushed && (tfdev->ForceBlackText != 0)) {
      gx_device *dev;

      gx_device_subclass(pdev, (gx_device *)&gs_black_text_device, sizeof(black_text_subclass_data));
      tfdev = (gx_device_tiff *)pdev->child;
      tfdev->BlackTextHandlerPushed = true;

      while(pdev->child)
      pdev = pdev->child;
      pdev->is_open = true;
      update_procs = true;
   }


You might notice that this is a little simpler than the other device installations, that's because the other devices can potentially be installed by any device, and we need to track them more carefully. Our new device can only be installed by the TIFF device, so we don't need to note that its been installed already, in all the parent and child devices. This is only done so that we don't accidentally install these more basic devices more than once.

So the first thing we do is check to see if we've already installed the device to force black text (we can potentially call the open method more than once and we don't want to install the device multiple times). If we haven't installed the device yet, and the feature has been requested, then we call the code to subclass the current device using the ``black_text_device`` as the prototype for the new device, and pass in the amount of data it requires for its private usage.

We will be defining this device as the next steps, for now just notice that after we call this, the 'current' device in the graphics state will be the ``black_text_device``, and its 'child' member will be pointing to the TIFF device. So we note in that child device that we have pushed the black text handling device. We also note that the TIFF device is 'open' (the calling code will do this for the device pointed to by the graphics state, i.e. the black text device). Finally we set a boolean to 'update procs'. This is needed because the ``tiff_open`` routine calls ``gdev_prn_allocate_memory`` which resets the device methods, we need to put them back to be correct for our device(s), which is done at the end of the ``tiff_open`` code:


.. code-block:: c

   if (update_procs) {
      if (pdev->ObjectHandlerPushed) {
         gx_copy_device_procs(&pdev->parent->procs, &pdev->procs, (gx_device_procs *)&gs_obj_filter_device.procs);
         pdev = pdev->parent;
      }
      if (pdev->PageHandlerPushed) {
         gx_copy_device_procs(&pdev->parent->procs, &pdev->procs, (gx_device_procs *)&gs_flp_device.procs);
         pdev = pdev->parent;
      }
      /* set tfdev to the bottom device in the chain */
      tfdev = (gx_device_tiff *)pdev;
      while (tfdev->child)
      tfdev = (gx_device_tiff *)tfdev->child;
      if (tfdev->BlackTextHandlerPushed)
      gx_copy_device_procs(&pdev->parent->procs, &pdev->procs, (gx_device_procs *)&gs_black_text_device.procs);
   }

In essence this simply works backwards through the chain of devices, restoring the correct methods as it goes. Note that because the ``BlackTextHandlerPushed`` variable isn't defined in the basic device structure, we have to cast the device pointer to a ``gx_device_tiff *`` so that we can set and check the variable which is defined there.

That completes the changes we need to make to the TIFF device to add support for our new code. Now we need to create the ``black_text`` device which will do the actual work.

We don't need any extras in the device structure, so we'll just define our subclassing device as being a ``gx_device``:


.. code-block:: c

   /* Black Text device */
   typedef struct gx_device_s gx_device_black_text;

We also don't need any extra storage, but we do need to carry round the basic storage required by a subclassing device, so we do need to define a structure for that:


.. code-block:: c

   typedef struct {
   subclass_common;
   } black_text_subclass_data;

Now, we are going to deal with text (obviously) and text, like images, is handled rather awkwardly in the current device interface. Instead of the interpreter calling the various device methods, the ``text_begin`` method creates a text enumerator which it returns to the interpreter. The text enumerator contains its own set of methods for dealing with text, and the interpreter calls these, thus bypassing the device itself. Since our subclassing requires us to pass down a chain of devices, when dealing with text and images we must also implement a chain of enumerators. For subclassing devices which don't process text or images this is catered for in the ``default_subclass_*`` methods, but in this case we need to handle the situation.

We start by creating our own text enumerator structure:

.. code-block:: c

   typedef struct black_text_text_enum_s {
      gs_text_enum_common;
      gs_text_enum_t *child;
   } black_text_text_enum_t;


Here we have a structure which contains the elements common to all enumerators, and a pointer to the next enumerator in the chain, we'll fill that in later.

Ghostscript uses several memory managers, but for PostScript and PDF objects we use a garbage collector. Because we may want to store pointers to garbage collected objects in the text enumerator, and our device, we need to make the garbage collector aware of them. This is because the garbage collector can relocate objects in memory, in order to do so safely it needs to update any pointers referencing those objects with the new location and so it needs to be told about those pointers.

So, we define a garbage collector structure for the text enumerator:

.. code-block:: c

   extern_st(st_gs_text_enum);
   gs_private_st_suffix_add1(st_black_text_text_enum, black_text_text_enum_t, "black_text_text_enum_t", black_text_text_enum_enum_ptrs, black_text_text_enum_reloc_ptrs, st_gs_text_enum, child);


The macro ``extern_st`` simply defines the structure (``st_gs_text_enum``) as being of type ``gs_memory_struct_type_t``. We need this structure for the 'superstructure' defined below.

The next macro ``gs_private_st_suffix_add1`` is one of a family of macros used to define structures for the garbage collector. These all end up defining the named structure as being of type ``gc_struct_data_t`` and fill in various members of that structure. In this case we are declaring that the structure:

   - Has one pointer to additional garbage collected objects (``add_1``)
   - Its name is ``st_black_text_text_enum``
   - It is of type ``black_text_text_enum_t``
   - Has the readable name for error messages ``black_text_text_enum_t``
   - The routine to be called to enumerate the garbage collected pointers in the structure is ``black_text_text_enum_enum_ptrs``
   - The routine to be called when relocating garbage collcted objects pointed to by the structure is ``black_text_text_enum_reloc_ptrs``
   - The 'superstructure' (i.e. the structure type this is based on) is ``st_gs_text_enum``. This means that all the enumeration and relocation methods for any pointers in the 'common' text enumerator structure get handled by this structure.
   - The one additional pointer to a garbage collected object is the 'child' member.

The macro takes care of creating all the code we need to deal with this object, essentially it will create ``black_text_text_enum_enum_ptrs`` and ``black_text_text_enum_reloc_ptrs`` for us.

So now we have to do a similar job for the actual device itself. We start by defining the enumeration and relocation routines for the structure. There are extensive macros for doing this, and we make use of some of them here. Explaining these is out of the scope of this document, but they are defined and easy to locate in the Ghostscript source tree.

.. code-block:: c

   static
   ENUM_PTRS_WITH(black_text_enum_ptrs, gx_device *dev);
   return 0; /* default case */
   case 0:ENUM_RETURN(gx_device_enum_ptr(dev->parent));
   case 1:ENUM_RETURN(gx_device_enum_ptr(dev->child));
   ENUM_PTRS_END

   static RELOC_PTRS_WITH(black_text_reloc_ptrs, gx_device *dev)
   {
      dev->parent = gx_device_reloc_ptr(dev->parent, gcst);
      dev->child = gx_device_reloc_ptr(dev->child, gcst);
   }RELOC_PTRS_END


Essentially these simply tell the garbage collector that we are maintaining two pointers to garbage collected objects, the parent and child devices. Now we use those in the garbage collector 'structure descriptor' for the device. Note this is not the actual device structure, its the structure used by the garbage collector when dealing with the device.



.. code-block:: c

   gs_public_st_complex_only(st_black_text_device, gx_device, "black_text", 0, black_text_enum_ptrs, black_text_reloc_ptrs, gx_device_finalize);


Again ``gs_public_st_complex_only`` is a macro, and its one of a family, these end up defining structures of type ``gs_memory_struct_t``. In this case we are declaring:

   - The structure type is named ``st_black_text_device``
   - The structure is of size ``gx_device`` (this only works because we don't need any other storage, otherwise we would have to define the device structure and pass that here).
   - The human readable name for error messages is ``black_text``
   - We don't define a ``clear`` routine.
   - The enumerator for GC objects is called ``black_text_enum_ptrs``
   - The relocator for GC objects is called ``black_text_reloc_ptrs``
   - We use the regular ``gx_device_finalize`` routine when freeing the object, we don't declare a special one of our own.

Now we've got all the garbage collector machinery out of the way we can deal with the actual device itself. Because we're only interested in text, there's only one device method we want to handle, the ``text_begin`` method, so we start by prototyping that:


.. code-block:: c

   /* Device procedures */
   static dev_proc_text_begin(black_text_text_begin);



Then we define the procedure to initialize the device procs:

.. code-block:: c

   void black_text_init_dev_procs(gx_device *dev)
   {
       default_subclass_initialize_device_procs(dev);

       set_dev_proc(dev, text_begin, black_text_text_begin);
   }


Then we define the actual device:


.. code-block:: c

   const gx_device_black_text gs_black_text_device =
   {
       std_device_dci_type_body(gx_device_black_text, 0, "black_text", &st_black_text_device,
           MAX_COORD, MAX_COORD,
           MAX_RESOLUTION, MAX_RESOLUTION,
           1, 8, 255, 0, 256, 1),
       black_text_initialize_device_procs
   };

The call to ``default_subclass_initialize_device_procs`` assigns the default methods for a subclassing device. We then override the ``text_begin`` method to our specific one. The main body of the macro is:


.. code-block:: c

   const gx_device_black_text gs_black_text_device =
   {
       std_device_dci_type_body(gx_device_black_text, 0, "black_text", &st_black_text_device,
           MAX_COORD, MAX_COORD,
           MAX_RESOLUTION, MAX_RESOLUTION,
           1, 8, 255, 0, 256, 1),


This simply defines an instance of the ``gx_device_black_text`` structure (which is simply a ``gx_device_s`` structure) initialised using a macro (yet another family of macros). Here we use the ``st_black_text_device`` structure descriptor we created above, and some dummy values for the resolution, colour depth etc. Since this device doesn't do rendering these values aren't useful and these defaults should be fine.

Now, for text we need to fill in all the procedures in the text enumerator, we'll start by defining the ``resync`` method, and then use this as a template for most of the other methods in the text enumerator:


.. code-block:: c

   static int black_text_text_resync(gs_text_enum_t *pte, const gs_text_enum_t *pfrom) {
      black_text_text_enum_t *penum;
      gs_text_enum_t * child = penum->child;
      int code;

      code = child->procs->resync(child, pfrom);
      gs_text_enum_copy_dynamic(pte, child, true);
      return code;
   }


The routine starts by casting the generic text enumerator to our specific type of text enumerator. From that we can get the child enumerator, and we simply pass the operation on directly to the child (remembering to pass the child enumerator as an argument, not our own one!).

Now, on return it may be that the child has modified the contents of the enumerator, so we must copy anything that might have changed from the child enumerator to our own. That's what the ``gs_text_enum_copy_dynamic`` routine does. After that we simply return the saved return value.

All the other routines are basically the same as this, we don't really want to do anything in the text enumeration so we just hand off the processing to the child.


.. code-block:: c

   static int black_text_text_process(gs_text_enum_t *pte) {
      black_text_text_enum_t *penum = (black_text_text_enum_t *)pte;
      gs_text_enum_t * child = penum->child;
      int code;

      code = child->procs->process(child);
      gs_text_enum_copy_dynamic(pte, child, true);
      return code;
   }

   static bool black_text_text_is_width_only(const gs_text_enum_t *pte) {
      black_text_text_enum_t *penum = (black_text_text_enum_t *)pte;
      gs_text_enum_t * child = penum->child;
      int code;

      code = child->procs->is_width_only(child);
      gs_text_enum_copy_dynamic((gs_text_enum_t *)pte, child, true);
      return code;
   }

   static int black_text_text_current_width(const gs_text_enum_t *pte, gs_point *pwidth) {
      black_text_text_enum_t *penum = (black_text_text_enum_t *)pte;
      gs_text_enum_t * child = penum->child;
      int code;

      code = child->procs->current_width(child, pwidth);
      gs_text_enum_copy_dynamic((gs_text_enum_t *)pte, child, true);
      return code;
   }

   static int black_text_text_set_cache(gs_text_enum_t *pte, const double *pw, gs_text_cache_control_t control) {
      black_text_text_enum_t *penum = (black_text_text_enum_t *)pte;
      gs_text_enum_t * child = penum->child;
      int code;

      code = child->procs->set_cache(child, pw, control);
      gs_text_enum_copy_dynamic(pte, child, true);
      return code;
   }

   static int black_text_text_retry(gs_text_enum_t *pte) {
      black_text_text_enum_t *penum = (black_text_text_enum_t *)pte;
      gs_text_enum_t * child = penum->child;
      int code;

      code = child->procs->retry(child);
      gs_text_enum_copy_dynamic(pte, child, true);
      return code;
   }

   static void black_text_text_release(gs_text_enum_t *pte, client_name_t cname) {
      black_text_text_enum_t *penum = (black_text_text_enum_t *)pte;
      gs_text_enum_t * child = penum->child;

      child->procs->release(child, cname);
      gx_default_text_release(pte, cname);
   }


The 'release' method is very slightly different, because we need to free the enumerator, so we call ``gx_default_text_release``.

Now, finally, we can define the ``text_begin`` device method, the first thing we do is a convenience, we define a ``gs_text_enum_procs`` instance which is initialised to point to all the text enumerator methods we defined above:


.. code-block:: c

   static const gs_text_enum_procs_t black_text_text_procs = {
      black_text_text_resync, black_text_text_process, black_text_text_is_width_only, black_text_text_current_width, black_text_text_set_cache, black_text_text_retry, black_text_text_release
   };

So on to the ``text_begin`` method:

.. code-block:: c

   /* The device method which we do actually need to define. */
   int black_text_text_begin(gx_device *dev, gs_imager_state *pis, const gs_text_params_t *text,
   gs_font *font, gx_path *path, const gx_device_color *pdcolor, const gx_clip_path *pcpath,
   gs_memory_t *memory, gs_text_enum_t **ppte)
   {
      black_text_text_enum_t *penum;
      int code;
      gs_text_enum_t *p;

      rc_alloc_struct_1(penum, black_text_text_enum_t, &st_black_text_text_enum, memory,
      return_error(gs_error_VMerror), "black_text_text_begin");
      penum->rc.free = rc_free_text_enum;
      code = gs_text_enum_init((gs_text_enum_t *)penum, &black_text_text_procs,
      dev, pis, text, font, path, pdcolor, pcpath, memory);
      if (code < 0) {
         gs_free_object(memory, penum, "black_text_text_begin");
         return code;
      }
      *ppte = (gs_text_enum_t *)penum;

      code = default_subclass_text_begin(dev, pis, text, font, path, pdcolor, pcpath, memory, &p);
      if (code < 0) {
         gs_free_object(memory, penum, "black_text_text_begin");
         return code;
      }
      penum->child = p;

      return 0;
   }


This uses library macros to create and initialise the text enumerator. Text enumerators are reference counted (and garbage collected.....) so we use the ``rc_alloc_struct`` family of macros, the '1' is the initial reference count that we want to have. We then call ``gs_text_enum_init`` to initialise the newly created structure, passing in the text procs we just created as one of the arguments.

We then set the returned enumerator to point to the newly created text enumerator. Finally we pass the ``text_begin`` method on to the child device using the ``default_subclass_text_begin`` method and we store the returned text enumerator in the child pointer of our own enumerator.

At this point the code should compile and run properly, but it won;t actually do anything yet. For that we need to modify the current colour before we run the text. Fortunately we don't need to deal with the graphics state, the ``text_begin`` method is given the colour index to be used so all we need to do is alter that. We do, however, have to cater for what the device thinks 'black' actually is, but there are graphics library calls to deal with both finding that information and setting a colour index from it:


.. code-block:: c

   int black_text_text_begin(gx_device *dev, gs_imager_state *pis, const gs_text_params_t *text,
   gs_font *font, gx_path *path, const gx_device_color *pdcolor, const gx_clip_path *pcpath,
   gs_memory_t *memory, gs_text_enum_t **ppte)
   {
      black_text_text_enum_t *penum;
      int code;
      gs_text_enum_t *p;

      /* Set the colour index in 'pdcolor' to be whatever the device thinks black shoudl be */
      set_nonclient_dev_color((gx_device_color *)pdcolor, gx_device_black((gx_device *)dev));






.. include:: footer.rst
