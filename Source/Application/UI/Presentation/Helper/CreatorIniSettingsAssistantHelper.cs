using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public class CreatorIniSettingsAssistantHelper : CreatorIniSettingsAssistant
    {
        public CreatorIniSettingsAssistantHelper(IInteractionInvoker interactionInvoker, ITranslationUpdater translationUpdater, ISettingsManager settingsManager,
                                                    IDataStorageFactory dataStorageFactory, IIniSettingsLoader iniSettingsLoader, IPrinterProvider printerProvider,
                                                    IUacAssistant uacAssistant) :
                                                base(interactionInvoker, translationUpdater, settingsManager, dataStorageFactory, iniSettingsLoader, printerProvider, uacAssistant)
        {
        }

        public new void QueryAndDeleteUnusedPrinters(List<string> unusedPrinters)
        {
            base.QueryAndDeleteUnusedPrinters(unusedPrinters);
        }

        public new void QueryAndAddMissingPrinters(List<string> missingPrinters)
        {
            base.QueryAndAddMissingPrinters(missingPrinters);
        }
    }
}
