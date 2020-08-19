using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Services.Update;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Assistants.Update;
using pdfforge.PDFCreator.UI.Presentation.Events;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.Helper.Version;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.UI.Presentation.Windows;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using pdfforge.PDFCreator.Utilities;
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

        private readonly IUpdateHelper _updateHelper;
        private readonly IEventAggregator _eventAggregator;
        private readonly IUpdateLauncher _updateLauncher;
        private readonly IDispatcher _dispatcher;
        private readonly IOnlineVersionHelper _onlineVersionHelper;
        private readonly IAssemblyHelper _assemblyHelper;
        public string CurrentVersionDate { get; set; }
        public string AvailableVersionText { get; }

        public UpdateHintViewModel(
            IUpdateHelper updateHelper,
            ITranslationUpdater translationUpdater,
            IEventAggregator eventAggregator,
            IVersionHelper versionHelper,
            IUpdateLauncher updateLauncher,
            IDispatcher dispatcher,
            IOnlineVersionHelper onlineVersionHelper,
            IAssemblyHelper assemblyHelper)
            :
            base(translationUpdater)
        {
            _updateHelper = updateHelper;
            _eventAggregator = eventAggregator;
            _updateLauncher = updateLauncher;
            _dispatcher = dispatcher;
            _onlineVersionHelper = onlineVersionHelper;
            _assemblyHelper = assemblyHelper;

            SetCurrentDateFormat();

            AvailableVersionText = Translation.GetNewUpdateMessage(_onlineVersionHelper.GetOnlineVersion().Version.ToString(3),
                                                                    versionHelper.ApplicationVersion.ToString(3),
                                                                    CurrentVersionDate);
        }

        public ICommand InstallUpdateCommand => new AsyncCommand(InstallUpdate);

        private void SetCurrentDateFormat()
        {
            var currentRelease = _onlineVersionHelper.CurrentReleaseVersion;

            if (currentRelease == null || !DateTime.TryParse(currentRelease.ReleaseDate, out var currentReleaseDate))
                currentReleaseDate = _assemblyHelper.GetLinkerTime();

            CurrentVersionDate = currentReleaseDate.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
        }

        private async Task InstallUpdate(object obj)
        {
            _eventAggregator.GetEvent<SetShowUpdateEvent>().Publish(false);
            FinishStep();

            var onlineVersion = _onlineVersionHelper.GetOnlineVersion();
            await _updateLauncher.LaunchUpdateAsync(onlineVersion);
        }

        public ICommand SkipVersionCommand => new DelegateCommand(o => SkipVersion());
        public ICommand AskLaterCommand => new DelegateCommand(o => UpdateLater());

        private void SkipVersion()
        {
            _updateHelper.SkipVersion();
            _eventAggregator.GetEvent<SetShowUpdateEvent>().Publish(false);
            FinishStep();
        }

        private void UpdateLater()
        {
            _updateHelper.SetNewUpdateTime();
            _eventAggregator.GetEvent<SetShowUpdateEvent>().Publish(false);
            FinishStep();
        }

        private void FinishStep()
        {
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

                foreach (var onlineVersionVersionInfo in _onlineVersionHelper.GetOnlineVersion().VersionInfos)
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
