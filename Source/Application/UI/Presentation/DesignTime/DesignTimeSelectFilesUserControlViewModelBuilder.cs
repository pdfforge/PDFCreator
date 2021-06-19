using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SelectFiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    internal class DesignTimeSelectFilesUserControlViewModelFactory : ISelectFilesUserControlViewModelFactory
    {
        public ISelectFilesUserControlViewModelBuilder Builder()
        {
            return new DesignTimeSelectFilesUserControlViewModelBuilder();
        }
    }

    internal class DesignTimeSelectFilesUserControlViewModelBuilder : ISelectFilesUserControlViewModelBuilder
    {
        public ISelectFilesUserControlViewModelBuilder WithTitleGetter(Func<string> getSelectFileInteractionTitle)
        {
            return this;
        }

        public ISelectFilesUserControlViewModelBuilder WithFileListGetter(Func<ConversionProfile, List<string>> getFileList)
        {
            return this;
        }

        public ISelectFilesUserControlViewModelBuilder WithFileFilter(string filter)
        {
            return this;
        }

        public ISelectFilesUserControlViewModelBuilder WithTokens(List<string> tokens)
        {
            return this;
        }

        public ISelectFilesUserControlViewModelBuilder WithPropertyChanged(PropertyChangedEventHandler propertyChanged)
        {
            return this;
        }

        public SelectFilesUserControlViewModel Build()
        {
            return new DesignTimeSelectFilesUserControlViewModel();
        }
    }
}
