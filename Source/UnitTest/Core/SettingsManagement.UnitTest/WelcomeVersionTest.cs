using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Utilities;
using System;
using SystemInterface.Microsoft.Win32;

namespace pdfforge.PDFCreator.UnitTest.Core.SettingsManagement
{
    [TestFixture]
    public class WelcomeVersionTest
    {
        private IVersionHelper _versionHelper;
        private IGpoSettings _gpoSettings;
        private IRegistry _registry;

        [SetUp]
        public void Setup()
        {
            _versionHelper = new VersionHelper(new Version(1, 2, 3, 4));
            _gpoSettings = Substitute.For<IGpoSettings>();

            _registry = Substitute.For<IRegistry>();
        }

        private void SetVersionInRegistryStub(string version)
        {
            _registry.GetValue(WelcomeSettingsHelper.RegistryKeyForWelcomeSettings, WelcomeSettingsHelper.RegistryValueForWelcomeVersion, null).Returns(version);
        }

        [Test]
        public void FirstRunTest_NoWelcomeVersionSet_IsFirstRun()
        {
            SetVersionInRegistryStub("");

            var welcomeSettingsHelper = new WelcomeSettingsHelper(_registry, _versionHelper, _gpoSettings);

            Assert.IsTrue(welcomeSettingsHelper.CheckIfRequiredAndSetCurrentVersion(), "Empty registry value for WelcomeVersion not detected as FirstRun.");
        }

        [Test]
        public void FirstRunTest_WelcomeVersionIsBiggerThanCurrentVersion_IsRequiredReturnsTrue()
        {
            SetVersionInRegistryStub("999999999.9.9");

            var welcomeSettingsHelper = new WelcomeSettingsHelper(_registry, _versionHelper, _gpoSettings);

            Assert.IsTrue(welcomeSettingsHelper.CheckIfRequiredAndSetCurrentVersion(), "Bigger WelcomeVersion not detected as FirstRun.");
        }

        [Test]
        public void FirstRunTest_WelcomeVersionIsSmallerThanCurrentVersion_IsRequiredReturnsTrue()
        {
            SetVersionInRegistryStub("0.0.0");

            var welcomeSettingsHelper = new WelcomeSettingsHelper(_registry, _versionHelper, _gpoSettings);

            Assert.IsTrue(welcomeSettingsHelper.CheckIfRequiredAndSetCurrentVersion(), "Smaller WelcomeVersion in registry not detected as FirstRun.");
        }

        [Test]
        public void FirstRunTest_WelcomeVersionIsTheCurrentVersion_IsRequiredReturnsFalse()
        {
            SetVersionInRegistryStub(_versionHelper.FormatWithBuildNumber()); //current version

            var welcomeSettingsHelper = new WelcomeSettingsHelper(_registry, _versionHelper, _gpoSettings);

            Assert.IsFalse(welcomeSettingsHelper.CheckIfRequiredAndSetCurrentVersion(), "Current Version as WelcomeVersion detected as FirstRun.");
        }

        [Test]
        public void FirstRunTestWithEnabledGPO_WelcomeVersionIsSmallerThanCurrentVersion_IsRequiredReturnsTrue()
        {
            SetVersionInRegistryStub("Not the current version");

            var welcomeSettingsHelper = new WelcomeSettingsHelper(_registry, _versionHelper, _gpoSettings);

            Assert.IsTrue(welcomeSettingsHelper.CheckIfRequiredAndSetCurrentVersion(), "Enabled GPO settnig should not hide the Welcome Window");
        }

        [Test]
        public void SetCurrentVersionAsWelcomeVersionTest_RegistrySetValueGetsCalled()
        {
            var welcomeSettingsHelper = new WelcomeSettingsHelper(_registry, _versionHelper, _gpoSettings);
            var currentVersion = _versionHelper.FormatWithBuildNumber();

            welcomeSettingsHelper.SetCurrentApplicationVersionAsWelcomeVersionInRegistry();

            _registry.Received(1).SetValue(WelcomeSettingsHelper.RegistryKeyForWelcomeSettings,
                WelcomeSettingsHelper.RegistryValueForWelcomeVersion, currentVersion);
        }
    }
}
