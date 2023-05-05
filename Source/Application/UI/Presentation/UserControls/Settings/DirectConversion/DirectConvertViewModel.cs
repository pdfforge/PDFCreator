using System;
using System.Collections.Generic;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DirectConversion
{
    public class DirectConvertViewModel : TranslatableViewModelBase<DirectConversionTranslation>
    {
        public ApplicationSettings ApplicationSettings { get; }
        public IEnumerable<PageSize> PageSizeValues => Enum.GetValues(typeof(PageSize)) as PageSize[];
        public IEnumerable<PageOrientation> PageOrientationValues => Enum.GetValues(typeof(PageOrientation)) as PageOrientation[];


        public DirectConvertViewModel(ITranslationUpdater translationUpdater, ICurrentSettings<ApplicationSettings> applicationSettings) :
            base(translationUpdater)
        {
            ApplicationSettings = applicationSettings.Settings;
        }
    }
}
