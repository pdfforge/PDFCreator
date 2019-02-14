using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities.Threading;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class DesignTimeProfilesViewModel : ProfilesViewModel
    {
        public DesignTimeProfilesViewModel() : base(new CurrentSettingsProvider(new DefaultSettingsProvider()), new TranslationUpdater(new TranslationFactory(), new ThreadManager()), new DesignTimeCommandLocator(), null, null, null)
        {
        }
    }
}
