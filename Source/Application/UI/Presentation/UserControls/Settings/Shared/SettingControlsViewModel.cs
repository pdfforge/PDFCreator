using System.Windows.Input;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.EvaluateSettingsCommands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.Shared
{
    public class SettingControlsViewModel : TranslatableViewModelBase<SettingControlsTranslation>
    {
        public SettingControlsViewModel(ITranslationUpdater translationUpdater, ICommandLocator commandLocator) : base(translationUpdater)
        {
            SaveCommand = commandLocator.CreateMacroCommand()
                .AddCommand<EvaluateSavingRelevantSettingsAndNotifyUserCommand>()
                .AddCommand<ISaveChangedSettingsCommand>()
                .AddCommand<NavigateToMainTabCommand>()
                .AddCommand<RaiseEditSettingsFinishedEventCommand>()
                .Build();

            CancelCommand = commandLocator.CreateMacroCommand()
                .AddCommand<CancelApplicationSettingsChangesCommand>()
                .AddCommand<NavigateToMainTabCommand>()
                .AddCommand<RaiseEditSettingsFinishedEventCommand>()
                .Build();
        }

        //public set to overrite HomeViewName for PDFCreator Server
        public string HomeViewName { get; set; } = MainRegionViewNames.HomeView; 

        public ICommand SaveCommand { get; private set; }

        public ICommand CancelCommand { get; private set; }
    }
}