using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Signature
{
    public class SignaturePositionAndSizeViewModel : OverlayViewModelBase<SignaturePositionAndSizeInteraction, SignatureTranslation>
    {
        private readonly ISignaturePositionAndSizeHelper _signaturePositionAndSizeHelper;

        private SignaturePositionForUi _signaturePositionForUi;

        public float LeftX
        {
            get => _signaturePositionForUi?.X ?? 0f;
            set
            {
                _signaturePositionForUi.X = value;
                SignatureForUiChanged();
            }
        }

        public float LeftY
        {
            get => _signaturePositionForUi?.Y ?? 0f;
            set
            {
                _signaturePositionForUi.Y = value;
                SignatureForUiChanged();
            }
        }

        public float SignatureWidth
        {
            get => _signaturePositionForUi?.Width ?? 0f;
            set
            {
                _signaturePositionForUi.Width = value;
                SignatureForUiChanged();
            }
        }

        public float SignatureHeight
        {
            get => _signaturePositionForUi?.Height ?? 0f;
            set
            {
                _signaturePositionForUi.Height = value;
                SignatureForUiChanged();
            }
        }

        public UnitOfMeasurement UnitOfMeasurement
        {
            get => Interaction?.UnitOfMeasurement ?? UnitOfMeasurement.Centimeter;
            set
            {
                Interaction.UnitOfMeasurement = value;
                HandleInteractionObjectChanged();
            }
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public SignaturePositionAndSizeViewModel(
            ITranslationUpdater translationUpdater,
            ISignaturePositionAndSizeHelper signaturePositionAndSizeHelper)
            : base(translationUpdater)
        {
            _signaturePositionAndSizeHelper = signaturePositionAndSizeHelper;

            OkCommand = new DelegateCommand(o =>
            {
                Interaction.Success = true;
                FinishInteraction();
            });

            CancelCommand = new DelegateCommand(o =>
            {
                FinishInteraction();
            });
        }

        protected override void HandleInteractionObjectChanged()
        {
            _signaturePositionForUi = _signaturePositionAndSizeHelper.GetSignaturePositionForUi(Interaction.Signature, Interaction.UnitOfMeasurement);
            RaisePropertyChanged(nameof(UnitOfMeasurement));
            RaisePropertyChanged(nameof(LeftX));
            RaisePropertyChanged(nameof(LeftY));
            RaisePropertyChanged(nameof(SignatureWidth));
            RaisePropertyChanged(nameof(SignatureHeight));
        }

        private void SignatureForUiChanged()
        {
            _signaturePositionAndSizeHelper.ApplySignatureForSettings(Interaction.Signature, _signaturePositionForUi, Interaction.UnitOfMeasurement);
        }

        public override string Title => Translation.Title;
    }
}
