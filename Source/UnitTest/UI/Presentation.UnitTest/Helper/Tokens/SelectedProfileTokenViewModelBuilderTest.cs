using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using System.ComponentModel;
using pdfforge.PDFCreator.UI.Presentation;

namespace Presentation.UnitTest.Helper.Tokens
{
    [TestFixture]
    public class SelectedProfileTokenViewModelBuilderTest
    {
        private SelectedProfileTokenViewModelBuilder _builder;
        private ISelectedProfileProvider _selectedProfileProvider;
        private ConversionProfile _currentProfile;
        private TokenHelper _tokenHelper;

        [SetUp]
        public void Setup()
        {
            _currentProfile = new ConversionProfile
            {
                Name = "Profile1"
            };

            _selectedProfileProvider = Substitute.For<ISelectedProfileProvider>();
            _selectedProfileProvider.SelectedProfile.Returns(_currentProfile);

            _tokenHelper = new TokenHelper(new DesignTimeTranslationUpdater());

            _builder = new SelectedProfileTokenViewModelBuilder(_selectedProfileProvider, _tokenHelper);
        }

        [Test]
        public void SelectedProfileTokenViewModelBuilder_IsTokenViewModelBuilder()
        {
            Assert.IsTrue(_builder is TokenViewModelBuilder<ConversionProfile>);
        }

        [Test]
        public void CreatedViewModel_WhenSelectedProfileChanges_HasNewProfile()
        {
            var vm = _builder
                .WithTokenCustomPreview(s => s)
                .WithSelector(p => p.Name)
                .Build();

            var newProfile = new ConversionProfile { Name = "Profile2" };

            _selectedProfileProvider.SelectedProfile.Returns(newProfile);
            _selectedProfileProvider.SelectedProfileChanged += Raise.Event<PropertyChangedEventHandler>(_selectedProfileProvider, new PropertyChangedEventArgs(nameof(_selectedProfileProvider.SelectedProfile)));

            Assert.AreSame(newProfile, vm.CurrentValue);
        }

        [Test]
        public void CreatedViewModel_WhenSettingsChange_HasNewProfile()
        {
            var vm = _builder
                .WithTokenCustomPreview(s => s)
                .WithSelector(p => p.Name)
                .Build();

            var newProfile = new ConversionProfile { Name = "Profile2" };

            _selectedProfileProvider.SelectedProfile.Returns(newProfile);
            _selectedProfileProvider.SettingsChanged += Raise.Event();

            Assert.AreSame(newProfile, vm.CurrentValue);
        }
    }
}
