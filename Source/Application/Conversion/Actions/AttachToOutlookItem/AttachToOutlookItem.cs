using NLog;
using System;
using System.Collections.Generic;
using Microsoft.Office.Interop.Outlook;
using static pdfforge.PDFCreator.Conversion.Actions.AttachToOutlookItem.DisposableCom;
using Exception = System.Exception;
using System.IO;

namespace pdfforge.PDFCreator.Conversion.Actions.AttachToOutlookItem
{
    public enum AttachToOutlookItemResult
    {
        Success,
        NoOutlook,
        NoOpenItems,
        ItemCouldNotBeFound,
        ErrorWhileAddingAttachment
    }

    public interface IAttachToOutlookItem
    {
        IList<string> GetOutlookItemCaptions();
        AttachToOutlookItemResult ExportToOutlookItem(string itemCaption, IList<string> attachmentFiles);
    }

    public class AttachToOutlookItem : IAttachToOutlookItem
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public IList<string> GetOutlookItemCaptions()
        {
            var captionList = new List<string>();
            try
            {
                using var outlookApplication = GetOutlookApplication();
                if (outlookApplication == null)
                {
                    return captionList;
                }

                using var inspectors = DisposableCom.Create(outlookApplication.ComObject.Inspectors);
                if (inspectors == null || inspectors.ComObject.Count == 0)
                {
                    return captionList;
                }

                //Limit to 10 captions. First caption has index 1. Ask Microsoft...
                var numberOfItems = Math.Min(inspectors.ComObject.Count, 11);
                for (var i = 1; i <= numberOfItems; i++)
                {
                    using var inspector = DisposableCom.Create((_Inspector)inspectors.ComObject[i]);
                    //Only list mails
                    if (inspector.ComObject.CurrentItem is MailItem)
                        captionList.Add(inspector.ComObject.Caption);
                }
            }
            catch (Exception e)
            {
                Logger.Warn("If outlook is not installed than this method throughs an file not found expection", e);
                captionList = new List<string>();
            }

            return captionList;
        }

        /// <summary>
        ///     Call this to get the running Outlook application, returns null if there isn't any.
        /// </summary>
        /// <returns>IDisposableCom for Outlook.Application or null</returns>
        private IDisposableCom<Application> GetOutlookApplication()
        {
            // first only try to get the type to prevent any side effect execution
            Type officeType = Type.GetTypeFromProgID("Outlook.Application");
            if (officeType == null)
            {
                return null;
            }

            IDisposableCom<Application> outlookApplication;
            try
            {
                outlookApplication = OleAuth32Api.OleAut32Api.GetActiveObject<Application>("Outlook.Application");
            }
            catch (Exception)
            {
                // Ignore, probably no outlook running
                return null;
            }

            outlookApplication ??= DisposableCom.Create(new Application());

            return outlookApplication ?? null;
        }

        public AttachToOutlookItemResult ExportToOutlookItem(string itemCaption, IList<string> attachmentFiles)
        {
            try
            {

                using var outlookApplication = GetOutlookApplication();

                if (outlookApplication == null)
                {
                    return AttachToOutlookItemResult.NoOutlook;
                }

                using var inspectors = DisposableCom.Create(outlookApplication.ComObject.Inspectors);
                if (inspectors == null || inspectors.ComObject.Count == 0)
                {
                    return AttachToOutlookItemResult.NoOpenItems;
                }

                Logger.Debug($"Got {inspectors.ComObject.Count} inspectors to check");

                //It's not 0. Ask Microsoft...
                for (int i = 1; i <= inspectors.ComObject.Count; i++)
                {
                    using var inspector = DisposableCom.Create((_Inspector)inspectors.ComObject[i]);
                    if (!inspector.ComObject.Caption.StartsWith(itemCaption))
                    {
                        continue;
                    }

                    if (inspector.ComObject.CurrentItem is MailItem mailItem)
                    {
                        if (mailItem.Sent)
                        {
                            continue;
                        }

                        try
                        {
                            return ExportToMail(inspector, mailItem, attachmentFiles);
                        }
                        catch (Exception exExport)
                        {
                            Logger.Error($"Export to {inspector.ComObject.Caption} failed.", exExport);
                            return AttachToOutlookItemResult.ErrorWhileAddingAttachment;
                        }
                        break;
                    }
                }

            }
            catch (Exception e)
            {
                return AttachToOutlookItemResult.NoOutlook;
            }
            return AttachToOutlookItemResult.ItemCouldNotBeFound;
        }

        private AttachToOutlookItemResult ExportToMail(IDisposableCom<_Inspector> inspector, MailItem mailItem, IList<string> attachmentFiles)
        {
            try
            {
                // Make sure the inspector is activated, only this way the word editor is active!
                // This also ensures that the window is visible!
                inspector?.ComObject.Activate();

                Logger.Info("Item '{0}' has format: {1}", mailItem.Subject, mailItem.BodyFormat);

                var inlinePossible = false;
                // Create the attachment (if inlined the attachment isn't visible as attachment!)
                using var attachments = DisposableCom.Create(mailItem.Attachments);

                foreach (var attachmentFile in attachmentFiles)
                {
                    var attachmentFileName = Path.GetFileName(attachmentFile);
                    Logger.Info("Older Outlook (<2007) found, using filename as contentid.");
                    var contentId = Path.GetFileName(attachmentFileName) + "_" + Guid.NewGuid();

                    attachments.ComObject.Add(attachmentFile, OlAttachmentType.olByValue, inlinePossible ? 0 : 1, attachmentFileName);
                }

                using var attachment = DisposableCom.Create(attachments.ComObject);

                return AttachToOutlookItemResult.Success;
            }
            catch (Exception ex)
            {
                string caption = "n.a.";
                if (inspector != null)
                {
                    caption = inspector.ComObject.Caption;
                }

                Logger.Warn($"Problem while trying to add attachment to Item '{caption}'", ex);
                return AttachToOutlookItemResult.ErrorWhileAddingAttachment;
            }
        }
    }
}
