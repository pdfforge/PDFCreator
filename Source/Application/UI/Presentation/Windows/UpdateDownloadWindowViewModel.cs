using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.Helper.Update;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Windows
{
    public class UpdateDownloadWindowViewModel : OverlayViewModelBase<UpdateDownloadInteraction, UpdateDownloadWindowTranslation>
    {
        public readonly IDispatcher Dispatcher;
        private readonly IReadableFileSizeFormatter _readableFileSizeFormatter;
        private readonly ApplicationNameProvider _applicationNameProvider;
        private readonly IUpdateHelper _updateHelper;

        public UpdateDownloadWindowViewModel(
            ITranslationUpdater translationUpdater, IReadableFileSizeFormatter readableFileSizeFormatter, ApplicationNameProvider applicationNameProvider, IUpdateHelper updateHelper, IDispatcher dispatcher)
            : base(translationUpdater)
        {
            _readableFileSizeFormatter = readableFileSizeFormatter;
            _applicationNameProvider = applicationNameProvider;
            _updateHelper = updateHelper;
            Dispatcher = dispatcher;
            _updateHelper.OnDownloadFinished += OnDownloadFinished;
            _updateHelper.OnProgressChanged += UpdateProgress;
            CancelCommand = new DelegateCommand(ExecuteCancel);
        }

        private void OnDownloadFinished(object sender, UpdateProgressChangedEventArgs args)
        {
            Dispatcher.BeginInvoke(DownloadFinished, args.Done);
        }

        public int ProgressPercentage { get; protected set; }

        public string DownloadSpeedText { get; protected set; }

        public ICommand CancelCommand { get; }

        protected override void HandleInteractionObjectChanged()
        {
            Interaction.StartDownloadCallback.Invoke();
        }

        public override string Title => _applicationNameProvider.ApplicationNameWithEdition;

        private void ExecuteCancel(object o)
        {
            _updateHelper.AbortDownload();
            Interaction.Success = false;
            FinishInteraction();
        }

        private void DownloadFinished(bool success)
        {
            UpdateProgress(this, new UpdateProgressChangedEventArgs(true, 100, 0, 0));
            Interaction.Success = true;
            FinishInteraction();
        }

        private void UpdateProgress(object sender, UpdateProgressChangedEventArgs args)
        {
            Dispatcher.BeginInvoke((Action<int>)delegate (int progress)
            {
                ProgressPercentage = progress;

                DownloadSpeedText = string.Format("{0}/s - {1:0} s",
                    _readableFileSizeFormatter.GetFileSizeString(_updateHelper.DownloadSpeed.BytesPerSecond),
                    _updateHelper.DownloadSpeed.EstimatedRemainingDuration.TotalSeconds);

                RaisePropertyChanged(nameof(ProgressPercentage));
                if (args.Progress == 100)
                    DownloadSpeedText = "";

                RaisePropertyChanged(nameof(DownloadSpeedText));
            }, args.Progress);
        }
    }
}
