using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using System;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Controls.Tab
{
    public interface ISubTabViewModel : IWhitelisted, IHasNotSupportedFeatures

    {
        string Title { get; }
        bool IsChecked { get; set; }

        void Init<TTranslation>(Func<TTranslation, string> titleId, Func<ConversionProfile, IProfileSetting> setting, PrismNavigationValueObject navigationObject, Func<ConversionProfile, bool> hasNotSupportedFeatures) where TTranslation : ITranslatable, new();
    }
}
