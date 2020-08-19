/* Copyright (C) 2001-2019 Artifex Software, Inc.
   All Rights Reserved.

   This software is provided AS-IS with no warranty, either express or
   implied.

   This software is distributed under license and may not be copied,
   modified or distributed except as expressly authorized under the terms
   of the license contained in the file LICENSE in this distribution.

   Refer to licensing information at http://www.artifex.com or contact
   Artifex Software, Inc.,  1305 Grant Avenue - Suite 200, Novato,
   CA 94945, U.S.A., +1(415)492-9861, for further information.
*/

/* ds32 device:32-bit CMYK TIFF device (downscaled output
 *              from 32-bit CMYK internal rendering)
 * Supports downscaler options similar to the tiffscaled32 device such
 * as DownScaleFactor, PostRenderProfile, TrapX, TrapY,and TrapOrder.
 *
 * No file is written (except for a debug mode that writes a PAM32 file)
 * so that raster lines can be passed in memory to a user function.
 */

#include "stdint_.h"
#include "gdevprn.h"
#include "gxiodev.h"
#include "stdio_.h"
#include "ctype_.h"
#include "gxgetbit.h"
#include "gsicc_cache.h"
#include "gscms.h"
#include "gxdownscale.h"
#include "gp.h"

/********************************************************************************
	The following section can be added to the end of device/devs.mak

### -------- Example 32-bit CMYK downscaled device --------------------- ###
# NB: downscale_ is standard in the lib (LIB1s)
$(DD)ds32.dev : $(DEVOBJ)gdevds32.$(OBJ) $(GLD)page.dev $(GDEV) $(DEVS_MAK) $(MAKEDIRS)
	$(SETPDEV2) $(DD)ds32 $(DEVOBJ)gdevds32.$(OBJ)
	$(ADDMOD) $(DD)ds32 -include $(GLD)page

$(DEVOBJ)gdevds32.$(OBJ) : $(DEVSRC)gdevds32.c $(gsicc_cache_h) $(gxdownscale_h) $(AK) \
  $(arch_h) $(gdevprn_h) $(stdio__h)  $(stdint__h) $(DEVS_MAK) $(MAKEDIRS)
	$(DEVCC) $(DEVO_)gdevds32.$(OBJ) $(C_) $(DEVSRC)gdevds32.c

***************** END OF devs.mak INSERTION *************************************/

/*
	Once that is added, then rebuild Ghostscript, for example, on linux:

    make DEVICE_DEVS_EXTRA=obj/ds32.dev

        or, on Windows:

    nmake -f psi/msvc32.mak DEVICE_DEVS_EXTRA=obj\ds32.dev

*/

/* ------ The device descriptors ------ */

/* Default X and Y resolution */
#define X_DPI 72
#define Y_DPI 72

/* ------ The ds32 device ------ */

static dev_proc_print_page(ds32_print_page);
static dev_proc_get_params(ds32_get_params_downscale_cmyk);
static dev_proc_put_params(ds32_put_params_downscale_cmyk);

static const gx_device_procs ds32_procs = {
        gdev_prn_open, NULL, NULL, gdev_prn_bg_output_page, gdev_prn_close,
        NULL, cmyk_8bit_map_color_rgb,
        NULL, NULL, NULL, NULL, NULL, NULL,
        ds32_get_params_downscale_cmyk,
        ds32_put_params_downscale_cmyk,
        cmyk_8bit_map_cmyk_color,
        NULL, NULL, NULL, gx_page_device_get_page_device
};

typedef struct gx_device_ds32_s {
    gx_device_common;
    gx_prn_device_common;
    gx_downscaler_params downscale;
    gsicc_link_t *icclink;
} gx_device_ds32;

const gx_device_ds32 gs_ds32_device = {
    prn_device_body(gx_device_ds32,
                    ds32_procs,
                    "ds32",
                    DEFAULT_WIDTH_10THS, DEFAULT_HEIGHT_10THS,
                    600, 600,   /* 600 dpi by default */
                    0, 0, 0, 0, /* Margins */
                    4,          /* num components */
                    32,         /* bits per sample */
                    255, 255, 256, 256,
                    ds32_print_page),
    GX_DOWNSCALER_PARAMS_DEFAULTS,
    0		/* icclink for PostRenderProfile */
};

static int
ds32_get_params_downscale_cmyk(gx_device * dev, gs_param_list * plist)
{
    gx_device_ds32 *const dsdev = (gx_device_ds32 *)dev;
    int code = gdev_prn_get_params(dev, plist);
    int ecode = code;

    code = gx_downscaler_write_params(plist, &dsdev->downscale,
                                      GX_DOWNSCALER_PARAMS_MFS |
                                      GX_DOWNSCALER_PARAMS_TRAP);
    if (code < 0) {
        ecode = code;
    }
    return ecode;
}

static int
ds32_put_params_downscale_cmyk(gx_device * dev, gs_param_list * plist)
{
    gx_device_ds32 *const dsdev = (gx_device_ds32 *)dev;
    int code, ecode;

    code = gx_downscaler_read_params(plist, &dsdev->downscale,
                                     (GX_DOWNSCALER_PARAMS_MFS |
                                      GX_DOWNSCALER_PARAMS_TRAP));
    if (code < 0)
    {
        ecode = code;
    }
    if (ecode < 0)
        return ecode;
    code = gdev_prn_put_params(dev, plist);
    return code;
}

/* below is copied (renamed) from tiff_chunky_post_cm */
static int ds32_chunky_post_cm(void  *arg, byte **dst, byte **src, int w, int h,
                                int raster)
{
    gsicc_bufferdesc_t input_buffer_desc, output_buffer_desc;
    gsicc_link_t *icclink = (gsicc_link_t*)arg;

    /*
        void gsicc_init_buffer(gsicc_bufferdesc_t *buffer_desc, unsigned char num_chan,
                unsigned char bytes_per_chan, bool has_alpha, bool alpha_first,
                bool is_planar, int plane_stride, int row_stride, int num_rows,
                int pixels_per_row);
    */
    gsicc_init_buffer(&input_buffer_desc, icclink->num_input, 1, false,
        false, false, 0, raster, h, w);
    gsicc_init_buffer(&output_buffer_desc, icclink->num_output, 1, false,
        false, false, 0, raster, h, w);
    icclink->procs.map_buffer(NULL, icclink, &input_buffer_desc, &output_buffer_desc,
        src[0], dst[0]);
    return 0;
}

#define WRITE_PAM32	/* for debugging */

#ifdef WRITE_PAM32	/* debugging code to write a PAM 32 file. */
static int
debug_write_pam_32(gx_device_ds32 *dsdev, byte *data, int row, FILE *file)
{
    int code = 0;

    if (row == 0) {
        /* PAM "magic" ID == 7 */
        code = fprintf(file, "P7\nWIDTH %d\nHEIGHT %d\nDEPTH 4\nMAXVAL 255\nTUPLTYPE CMYK\n"
                              "# Image generated by GPL Ghostscript GIT PRERELEASE\nENDHDR\n",
                       dsdev->width, dsdev->height);
        if (code < 0)
            code = gs_error_ioerror;
    }
    if (code < 0 || fwrite(data, 1, 4*dsdev->width, file) != 4*dsdev->width)
        code = gs_error_ioerror;

    return code;
}
#endif

static int
ds32_print_page(gx_device_printer * pdev, FILE * file)
{
    gx_device_ds32 *const dsdev = (gx_device_ds32 *)pdev;
    int code = 0;
    byte *data = NULL;
    int size = gdev_mem_bytes_per_scan_line((gx_device *)pdev);
    int row;
    int factor = dsdev->downscale.downscale_factor;
    int height = pdev->height / factor;
    int bpc = 8;
    int num_comps = 4;
    int trap_w = dsdev->downscale.trap_w;
    int trap_h = dsdev->downscale.trap_h;
    int *trap_order = dsdev->downscale.trap_order;
    gx_downscaler_t ds;

    if (num_comps == 4) {
        if (dsdev->icclink == NULL) {
            code = gx_downscaler_init_trapped(&ds, (gx_device *)pdev, 8, bpc, num_comps,
                factor, 0 /*mfs*/, &fax_adjusted_width, 0 /*aw*/, trap_w, trap_h, trap_order);
        } else {
            code = gx_downscaler_init_trapped_cm(&ds, (gx_device *)pdev, 8, bpc, num_comps,
                factor, 0 /*mfs*/, &fax_adjusted_width, 0 /*aw*/, trap_w, trap_h, trap_order,
                ds32_chunky_post_cm, dsdev->icclink, dsdev->icclink->num_output);
        }
    }
    if (code < 0)
        return code;

    data = gs_alloc_bytes(pdev->memory, size, "ds32_print_page(data)");
    if (data == NULL) {
        gx_downscaler_fin(&ds);
        return_error(gs_error_VMerror);
    }

    for (row = 0; row < height && code >= 0; row++) {
        code = gx_downscaler_getbits(&ds, data, row);
        if (code < 0)
            break;
#ifdef WRITE_PAM32	/* debugging code to write a PAM 32 file. */
        if ((code = debug_write_pam_32(dsdev, data, row, file)) < 0)
            break;
#endif
        /*************************************************************/
        /* User code to handle the row of data (dsdev->width bytes)  */
        /* Set code to a negative value (e.g. gs_error_ioerror) if a */
        /* problem occurs.                                           */
        /* Note that nothing needs to be written to 'file' and this  */
        /* can be set to '-' (stdout) to avoid leaving an empty file */
        /* if (row == 0) can be used to perform any initialization   */
        /*     needed to process the user data                       */
        /*************************************************************/

        if (code < 0)
            break;
    }

    /**************************************************************/
    /* User code to finish writing and perform any cleanup needed */
    /**************************************************************/

    gx_downscaler_fin(&ds);
    gs_free_object(pdev->memory, data, "tiff_print_page(data)");

    return code;
}
