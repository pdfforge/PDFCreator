using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using System;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Controls.Tab
{
    public interface ISubTabViewModel : IWhitelisted, IMountable

    {
        string Title { get; }
        bool IsChecked { get; set; }

        void Init<TTranslation>(Func<TTranslation, string> titleId, Func<ConversionProfile, IProfileSetting> setting, PrismNavigationValueObject navigationObject, Func<ConversionProfile, bool> hasNotSupportedFeatures) where TTranslation : ITranslatable, new();
    }
}
