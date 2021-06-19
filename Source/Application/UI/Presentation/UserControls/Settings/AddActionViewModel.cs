using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings
{
   public class AddActionViewModel : TranslatableViewModelBase<DebugSettingsTranslation>
    {
        private readonly IInteractionRequest _interactionRequest;

        public AddActionViewModel(IInteractionRequest interactionRequest,
            ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
            _interactionRequest = interactionRequest;
            OpenAddActionOverviewCommand = new DelegateCommand(OpenAddActionOverview);
        }
        public DelegateCommand OpenAddActionOverviewCommand { get; set; }

        private async void OpenAddActionOverview(object obj)
        {
            await _interactionRequest.RaiseAsync(new AddActionOverlayInteraction());
        }

    }
}
