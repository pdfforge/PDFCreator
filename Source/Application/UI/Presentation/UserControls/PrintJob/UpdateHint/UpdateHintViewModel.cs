using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Assistants.Update;
using pdfforge.PDFCreator.UI.Presentation.Events;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.UI.Presentation.Windows;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.UpdateHint
{
    public class UpdateHintViewModel : TranslatableViewModelBase<UpdateManagerTranslation>, IWorkflowViewModel, IInteractionAware
    {
        private readonly TaskCompletionSource<object> _taskCompletionSource = new TaskCompletionSource<object>();

        private readonly IUpdateAssistant _updateAssistant;
        private readonly IProcessStarter _processStarter;
        private readonly IEventAggregator _eventAggregator;
        private readonly IUpdateLauncher _updateLauncher;
        private readonly IDispatcher _dispatcher;
        public string WhatsNewUrl { get; }
        public string CurrentVersionDate { get; set; }
        public string AvailabeVersionText { get; }

        private bool IsInWorkflow = false;

        public UpdateHintViewModel(
            IUpdateAssistant updateAssistant,
            IProcessStarter processStarter,
            ITranslationUpdater translationUpdater,
            IEventAggregator eventAggregator,
            IVersionHelper versionHelper,
            IUpdateLauncher updateLauncher,
            IDispatcher dispatcher)
            :
            base(translationUpdater)
        {
            _updateAssistant = updateAssistant;
            _processStarter = processStarter;
            _eventAggregator = eventAggregator;
            _updateLauncher = updateLauncher;
            _dispatcher = dispatcher;
            WhatsNewUrl = Urls.PDFCreatorWhatsNewUrl;

            SetCurrentDateFormat();

            AvailabeVersionText = Translation.GetNewUpdateMessage(updateAssistant.OnlineVersion.Version.ToString(3),
                                                                    versionHelper.ApplicationVersion.ToString(3),
                                                                    CurrentVersionDate);
        }

        public ICommand InstallUpdateCommand => new AsyncCommand(InstallUpdate);

        private void SetCurrentDateFormat()
        {
            CurrentVersionDate = _updateAssistant.CurrentReleaseVersion != null ? _updateAssistant.CurrentReleaseVersion.ReleaseDate : " - ";

            if (DateTime.TryParse(CurrentVersionDate, out var currentDateTime))
                CurrentVersionDate = currentDateTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
        }

        private async Task InstallUpdate(object obj)
        {
            _eventAggregator.GetEvent<SetShowUpdateEvent>().Publish(false);
            FinishStep();

            await _updateLauncher.LaunchUpdate(_updateAssistant.OnlineVersion);
        }

        public ICommand SkipVersionCommand => new DelegateCommand(o => SkipVersion());
        public ICommand AskLaterCommand => new DelegateCommand(o => UpdateLater());

        public ICommand WhatsNewCommand => new DelegateCommand(o => _processStarter.Start(WhatsNewUrl));

        private void SkipVersion()
        {
            _updateAssistant.SkipVersion();
            _eventAggregator.GetEvent<SetShowUpdateEvent>().Publish(false);
            FinishStep();
        }

        private void UpdateLater()
        {
            _updateAssistant.SetNewUpdateTime();
            _eventAggregator.GetEvent<SetShowUpdateEvent>().Publish(false);
            FinishStep();
        }

        private void FinishStep()
        {
            StepFinished?.Invoke(this, EventArgs.Empty);
            _taskCompletionSource.SetResult(null);
            FinishInteraction?.Invoke();
        }

        public Task ExecuteWorkflowStep(Job job)
        {
            _eventAggregator.GetEvent<TryCloseApplicationEvent>().Subscribe(TryCloseApplicationEvent);
            return _taskCompletionSource.Task;
        }

        public List<Release> UpdateVersions
        {
            get
            {
                var list = new List<Release>();

                foreach (var onlineVersionVersionInfo in _updateAssistant.OnlineVersion.VersionInfos)
                {
                    list.Add(onlineVersionVersionInfo.Copy());
                }

                return list;
            }
        }

        private void TryCloseApplicationEvent()
        {
            _dispatcher.BeginInvoke(() =>
                throw new InterruptWorkflowException()
            );
        }

        public event EventHandler StepFinished;

        public void SetInteraction(IInteraction interaction)
        {
        }

        public Action FinishInteraction { get; set; }
        public string Title => Translation.NewUpdateIsAvailable;

        protected override void OnTranslationChanged()
        {
            base.OnTranslationChanged();
            RaisePropertyChanged(nameof(Title));
        }
    }
}
