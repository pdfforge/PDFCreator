using pdfforge.CustomScriptAction;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.PreparationActions.CsScript
{
    public class CsScriptViewModel : ActionViewModelBase<PreConversionScriptAction, CsScriptTranslation>
    {
        private readonly IDirectory _directory;
        private readonly ICustomScriptLoader _customScriptLoader;
        private readonly ErrorCodeInterpreter _errorCodeInterpreter;

        public override bool HideStatusInOverlay => true;
        public string LoadingResultText { get; private set; }
        public bool LoadingSucessful { get; private set; }
        public string LoadingResultExceptionMessage { get; private set; }
        public ICommand OpenScriptFolderCommand { get; }
        public AsyncCommand CheckScriptAsyncCommand { get; }
        public AsyncCommand ReloadScriptListAsyncCommand { get; }
        public ICollectionView ScriptfilesView { get; private set; }

        public CsScriptViewModel(
            ITranslationUpdater translationUpdater,
            IDispatcher dispatcher,
            IDirectory directory,
            ICustomScriptLoader customScriptLoader,
            ErrorCodeInterpreter errorCodeInterpreter,
            ICommandLocator commandLocator,
            IActionLocator actionLocator,
            ICurrentSettingsProvider currentSettingsProvider,
            IDefaultSettingsBuilder defaultSettingsBuilder,
            IActionOrderHelper actionOrderHelper)
            : base(actionLocator, errorCodeInterpreter, translationUpdater, currentSettingsProvider, dispatcher, defaultSettingsBuilder, actionOrderHelper)
        {
            _directory = directory;
            _customScriptLoader = customScriptLoader;
            _errorCodeInterpreter = errorCodeInterpreter;

            CheckScriptAsyncCommand = new AsyncCommand(CheckScriptExecute, CheckScriptCanExecute);

            OpenScriptFolderCommand = commandLocator.GetCommand<OpenCsScriptsFolderCommand>();

            ReloadScriptListAsyncCommand = new AsyncCommand(ReloadScriptfilesViewExecute);
        }

        private Task ReloadScriptfilesViewExecute(object obj)
        {
            return Task.Factory.StartNew(() =>
            {
                var files = new List<string>();
                try
                {
                    files = _directory.GetFiles(_customScriptLoader.ScriptFolder, "*", SearchOption.TopDirectoryOnly)
                                .Where(s => s.EndsWith(".cs") || s.EndsWith(".csx"))
                                .ToList();
                }
                catch { }

                string currentScriptFilename = null;
                var scriptfiles = new ObservableCollection<string>();
                foreach (var file in files)
                {
                    var filename = PathSafe.GetFileName(file);
                    scriptfiles.Add(filename);

                    if (filename.Equals(CurrentProfile.CustomScript.ScriptFilename))
                        currentScriptFilename = filename;
                }

                ScriptfilesView = new ListCollectionView(scriptfiles);
                ScriptfilesView.CurrentChanged += async (sender, args) =>
                {
                    if (CurrentProfile.CustomScript.ScriptFilename == null)
                        CurrentProfile.CustomScript.ScriptFilename = ""; //avoid null in profile
                    await CheckScriptAsyncCommand.ExecuteAsync(args);
                };
                RaisePropertyChanged(nameof(ScriptfilesView));

                CheckScriptAsyncCommand.RaiseCanExecuteChanged();

                ScriptfilesView.MoveCurrentTo(currentScriptFilename);
            });
        }

        private Task CheckScriptExecute(object obj)
        {
            return Task.Factory.StartNew(() =>
            {
                LoadingResultText = Translation.LoadingCsScript;
                RaisePropertyChanged(nameof(LoadingResultText));
                LoadingResultExceptionMessage = "";
                RaisePropertyChanged(nameof(LoadingResultExceptionMessage));

                var scriptFilename = CurrentProfile.CustomScript.ScriptFilename;
                var loadScriptResult = _customScriptLoader.ReLoadScriptWithValidation(scriptFilename);

                LoadingSucessful = loadScriptResult.Result;
                RaisePropertyChanged(nameof(LoadingSucessful));

                if (LoadingSucessful)
                    LoadingResultText = Translation.LoadingCsScriptSuccessful;
                else
                    LoadingResultText = _errorCodeInterpreter.GetFirstErrorText(loadScriptResult.Result, false);
                RaisePropertyChanged(nameof(LoadingResultText));

                if (ScriptfilesView.IsEmpty)
                    LoadingResultExceptionMessage = Translation.GetFormattedLicenseEnsureCsScriptsFolder(_customScriptLoader.ScriptFolder);
                else
                    LoadingResultExceptionMessage = loadScriptResult.ExceptionMessage;
                RaisePropertyChanged(nameof(LoadingResultExceptionMessage));
            });
        }

        private bool CheckScriptCanExecute(object obj)
        {
            if (ScriptfilesView == null)
                return false;
            return !ScriptfilesView.IsEmpty;
        }

        protected override string SettingsPreviewString => CurrentProfile?.CustomScript.ScriptFilename;
    }
}
