using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SelectFiles
{
    public interface ISelectFilesUserControlViewModelBuilder
    {
        ISelectFilesUserControlViewModelBuilder WithTitleGetter(Func<string> getSelectFileInteractionTitle);

        ISelectFilesUserControlViewModelBuilder WithFileListGetter(Func<ConversionProfile, List<string>> getFileList);

        ISelectFilesUserControlViewModelBuilder WithFileFilter(string filter);

        ISelectFilesUserControlViewModelBuilder WithTokens(List<string> tokens);

        ISelectFilesUserControlViewModelBuilder WithPropertyChanged(PropertyChangedEventHandler propertyChanged);

        SelectFilesUserControlViewModel Build();
    }

    public class SelectFilesUserControlViewModelBuilder : ISelectFilesUserControlViewModelBuilder
    {
        private readonly ITranslationUpdater _translationUpdater;
        private readonly ISelectedProfileProvider _selectedProfileProvider;
        private readonly IDispatcher _dispatcher;
        private readonly IInteractionRequest _interactionRequest;

        private Func<string> _getSelectFileInteractionTitle;
        private Func<ConversionProfile, List<string>> _getFileList;
        private List<string> _tokens;
        private string _filter;
        private PropertyChangedEventHandler _propertyChangedEventHandler;

        public SelectFilesUserControlViewModelBuilder(ITranslationUpdater translationUpdater,
            ISelectedProfileProvider selectedProfileProvider,
            IDispatcher dispatcher,
            IInteractionRequest interactionRequest)
        {
            _translationUpdater = translationUpdater;
            _selectedProfileProvider = selectedProfileProvider;
            _dispatcher = dispatcher;
            _interactionRequest = interactionRequest;
        }

        public ISelectFilesUserControlViewModelBuilder WithTitleGetter(Func<string> getSelectFileInteractionTitle)
        {
            _getSelectFileInteractionTitle = getSelectFileInteractionTitle;
            return this;
        }

        public ISelectFilesUserControlViewModelBuilder WithFileListGetter(Func<ConversionProfile, List<string>> getFileList)
        {
            _getFileList = getFileList;
            return this;
        }

        public ISelectFilesUserControlViewModelBuilder WithFileFilter(string filter)
        {
            _filter = filter;
            return this;
        }

        public ISelectFilesUserControlViewModelBuilder WithTokens(List<string> tokens)
        {
            _tokens = tokens;
            return this;
        }

        public ISelectFilesUserControlViewModelBuilder WithPropertyChanged(PropertyChangedEventHandler propertyChanged)
        {
            _propertyChangedEventHandler += propertyChanged;
            return this;
        }

        public SelectFilesUserControlViewModel Build()
        {
            var vm = new SelectFilesUserControlViewModel(_translationUpdater,
                _selectedProfileProvider, _dispatcher, _interactionRequest,
                _getSelectFileInteractionTitle, _getFileList, _tokens, _filter);

            vm.PropertyChanged += _propertyChangedEventHandler;

            return vm;
        }
    }
}
