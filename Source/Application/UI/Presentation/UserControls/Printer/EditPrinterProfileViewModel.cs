using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SelectFiles;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Input;
using pdfforge.PDFCreator.Conversion.Settings;
using Prism.Commands;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Printer
{
    public class EditPrinterProfileViewModel : OverlayViewModelBase<EditPrinterProfileUserInteraction, EditPrinterProfileTranslatable>
    {
        public ObservableCollection<ConversionProfileWrapper> Profiles { get; set; }

        private ConversionProfileWrapper _selectedProfile;
        public ConversionProfileWrapper SelectedProfile
        {
            get { return _selectedProfile; }
            set
            {
                if (value != null)
                {
                    _selectedProfile = value;
                }
            }
        }

        public ICommand OkCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public EditPrinterProfileViewModel(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
            Title = Translation.EditPrinterProfileTitle;
            
            OkCommand = new DelegateCommand(OkInteraction);
            CancelCommand = new DelegateCommand(CancelInteraction);
        }

        private void CancelInteraction()
        {
            Interaction.ResultProfile = Interaction.PrinterMappingWrapper.Profile;
            FinishInteraction();
        }

        private void OkInteraction()
        {
            Interaction.ResultProfile = _selectedProfile;
            Interaction.Success = true;
            FinishInteraction();
        }

        public override string Title { get; }

        protected override void HandleInteractionObjectChanged()
        {
            base.HandleInteractionObjectChanged();

            Profiles = Interaction.ProfileWrappers;
            _selectedProfile = Profiles?.FirstOrDefault(wrapper => wrapper.Name == Interaction.PrinterMappingWrapper.Profile.Name);
            RaisePropertyChanged(nameof(Profiles));
            RaisePropertyChanged(nameof(SelectedProfile));
        }
    }
}
