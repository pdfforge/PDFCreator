using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using pdfforge.PDFCreator.UI.PrismHelper.Prism.SimpleInjector;
using Prism.Regions;
using SimpleInjector;
using System;
using Translatable;

namespace pdfforge.PDFCreator.UI.PrismHelper.Tab
{
    public interface ISubTab
    {
        void Register(IRegionManager regionManager, IWhitelistedServiceLocator serviceLocator, string masterTabItemsRegion, string masterTabContentRegion, bool isFirstTab);

        void RegisterNavigationViews(Container container);
    }

    public class SubTab<TView, TTranslation> : ISubTab
        where TView : class
        where TTranslation : ITranslatable, new()
    {
        private readonly Func<TTranslation, string> _titleId;
        private readonly Func<ConversionProfile, IProfileSetting> _setting;
        private readonly Func<ConversionProfile, bool> _hasNotSupportedFeature;

        public SubTab(Func<TTranslation, string> titleTranslationFunc, Func<ConversionProfile, IProfileSetting> setting, Func<ConversionProfile, bool> hasNotSupportedFeature = null)
        {
            _titleId = titleTranslationFunc;
            _setting = setting;
            _hasNotSupportedFeature = hasNotSupportedFeature;
        }

        public void Register(IRegionManager regionManager, IWhitelistedServiceLocator serviceLocator, string masterTabItemsRegion, string masterTabContentRegion, bool isFirstTab)
        {
            regionManager.RegisterSubTab<TView, ProfileSubTabViewModel, TTranslation>(masterTabItemsRegion, masterTabContentRegion, _titleId, _setting, serviceLocator, _hasNotSupportedFeature, isFirstTab);
        }

        public void RegisterNavigationViews(Container container)
        {
            container.RegisterTypeForNavigation<TView>();
        }
    }
}
