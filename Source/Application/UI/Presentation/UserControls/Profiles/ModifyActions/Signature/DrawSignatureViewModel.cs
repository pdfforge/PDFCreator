using NLog;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Signature
{
    public class DrawSignatureViewModel : DrawSignatureViewModelBase<DrawSignatureInteraction, SignatureTranslation>
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IInteractionRequest _interactionRequest;
        private readonly ICanvasToFileHelper _canvasToFileHelper;
        private readonly IInteractionInvoker _interactionInvoker;
        public Canvas PaintSurface { get; set; }
        public bool CanSaveAndReset { get; set; }
        public float SignatureWidth { get; private set; }
        public float SignatureHeight { get; private set; }
        public Color BrushColor { get; set; } = Colors.Black;
        public float BrushSize { get; set; } = 1f;
        public static List<float> PossibleBrushSizes => new() { 1, 2, 3, 4, 5 };

        public DrawSignatureViewModel(ITranslationUpdater translationUpdater,
            IInteractionRequest interactionRequest,
            IInteractionInvoker interactionInvoker,
            ICanvasToFileHelper canvasToFileHelper,
            ICurrentSettingsProvider currentSettingsProvider) : base(translationUpdater)
        {
            _interactionRequest = interactionRequest;
            _interactionInvoker = interactionInvoker;
            _canvasToFileHelper = canvasToFileHelper;

            SetSignatureSize(currentSettingsProvider.SelectedProfile.PdfSettings.Signature.RightX - currentSettingsProvider.SelectedProfile.PdfSettings.Signature.LeftX,
                currentSettingsProvider.SelectedProfile.PdfSettings.Signature.RightY - currentSettingsProvider.SelectedProfile.PdfSettings.Signature.LeftY);
        }

        public DelegateCommand ChooseBrushColorCommand => new(ChooseBrushColorExecute);

        private void ChooseBrushColorExecute(object obj)
        {
            var interaction = new ColorInteraction
            {
                Color = System.Drawing.Color.FromArgb(BrushColor.A, BrushColor.R, BrushColor.G, BrushColor.B)
            };

            _interactionInvoker.Invoke(interaction);

            if (!interaction.Success)
                return;

            BrushColor = Color.FromArgb(interaction.Color.A, interaction.Color.R, interaction.Color.G, interaction.Color.B);
            RaisePropertyChanged(nameof(BrushColor));
        }

        protected override void SaveExecute()
        {
            if (!SetSignatureFilePath(out var signatureFilePath))
                return;

            try
            {
                var renderTargetBitmap = _canvasToFileHelper.CanvasToBitmap(PaintSurface);
                var croppedSignature = _canvasToFileHelper.CropSignature(renderTargetBitmap);
                if (!_canvasToFileHelper.SaveToFile(croppedSignature, signatureFilePath))
                    throw new Exception();
            }
            catch (Exception e)
            {
                _logger.Error(e);
                var messageInteraction = new MessageInteraction("Could not save your signature file. Please try again.",
                    Translation.Title,
                    MessageOptions.Ok,
                    MessageIcon.Error);
                _interactionRequest.Raise(messageInteraction);
                return;
            }

            Interaction.SignatureFilePath = signatureFilePath;
            Interaction.Success = true;
            FinishInteraction();
        }

        protected override bool SaveCanExecute()
        {
            return CanSaveAndReset;
        }

        protected override void ResetExecute()
        {
            PaintSurface.Children.Clear();
            CanSaveAndReset = false;
            SaveCommand.RaiseCanExecuteChanged();
            ResetCommand.RaiseCanExecuteChanged();
        }

        protected override bool ResetCanExecute()
        {
            return CanSaveAndReset;
        }

        private bool SetSignatureFilePath(out string signatureFilePath)
        {
            var saveFileInteraction = new SaveFileInteraction
            {
                FileName = Translation.Title.ToLower(),
                Filter = Translation.PNGFiles
                         + @" (*.png)|*.png;|"
                         + Translation.AllFiles
                         + @" (*.*)|*.*"
            };

            _interactionInvoker.Invoke(saveFileInteraction);

            if (!saveFileInteraction.Success)
            {
                signatureFilePath = "";
                return false;
            }

            signatureFilePath = saveFileInteraction.FileName;
            return true;
        }

        private void SetSignatureSize(float width, float height)
        {
            var aspectRatio = width / height;
            switch (aspectRatio)
            {
                case > 1:
                    SignatureWidth = 550;
                    SignatureHeight = SignatureWidth / aspectRatio;
                    break;

                case < 1 and > 0:
                    SignatureHeight = 550;
                    SignatureWidth = SignatureHeight * aspectRatio;
                    break;

                default:
                    SignatureWidth = 450;
                    SignatureHeight = 450;
                    break;
            }
        }
    }
}
