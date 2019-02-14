using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.NavigationChecks;
using System.Collections.Generic;

namespace Presentation.UnitTest.Navigation
{
    namespace Presentation.UnitTest.Navigation
    {
        public class RelevantRegionCheckTest
        {
            private TabSwitchSettingsCheck _tabSwitchSettingsCheck;
            private IRegionHelper _regionHelper;
            private IEnumerable<ISettingsNavigationCheck> _settingsNavigationChecks;

            private const string RegionNoChangesNoErrors = "RegionNoChangesNoErrors";
            private const string RegionWithChangesNoErrors = "RegionWithChangesNoErrors";
            private const string RegionNoChangesWithErrors = "RegionNoChangesWithErrors";
            private const string RegionWithChangesWithErrors = "RegionWithChangesWithErrors";

            private ISettingsNavigationCheck _regionCheckNoChangesNoErrors;
            private ISettingsNavigationCheck _regionCheckWithChangesNoErrors;
            private ISettingsNavigationCheck _regionCheckNoChangesWithErrors;
            private ISettingsNavigationCheck _regionCheckWithChangesWithErrors;

            private readonly ActionResultDict _actionResultDictNoChangesNoErrors = new ActionResultDict { { RegionNoChangesNoErrors, new ActionResult() } };
            private readonly ActionResultDict _actionResultDictWithChangesNoErrors = new ActionResultDict { { RegionWithChangesNoErrors, new ActionResult() } };
            private readonly ActionResultDict _actionResultDictNoChangesWithErrors = new ActionResultDict { { RegionNoChangesWithErrors, new ActionResult(0) } };
            private readonly ActionResultDict _actionResultDictWithChangesWithErrors = new ActionResultDict { { RegionWithChangesWithErrors, new ActionResult(0) } };

            [SetUp]
            public void SetUp()
            {
                _regionHelper = Substitute.For<IRegionHelper>();

                _regionCheckNoChangesNoErrors = Substitute.For<ISettingsNavigationCheck>();
                _regionCheckNoChangesNoErrors.IsRelevantForRegion(RegionNoChangesNoErrors).Returns(true);
                _regionCheckNoChangesNoErrors.CheckSettings().Returns(new SettingsCheckResult(_actionResultDictNoChangesNoErrors, false));

                _regionCheckWithChangesNoErrors = Substitute.For<ISettingsNavigationCheck>();
                _regionCheckWithChangesNoErrors.IsRelevantForRegion(RegionWithChangesNoErrors).Returns(true);
                _regionCheckWithChangesNoErrors.CheckSettings().Returns(new SettingsCheckResult(_actionResultDictWithChangesNoErrors, true));

                _regionCheckNoChangesWithErrors = Substitute.For<ISettingsNavigationCheck>();
                _regionCheckNoChangesWithErrors.IsRelevantForRegion(RegionNoChangesWithErrors).Returns(true);
                _regionCheckNoChangesWithErrors.CheckSettings().Returns(new SettingsCheckResult(_actionResultDictNoChangesWithErrors, false));

                _regionCheckWithChangesWithErrors = Substitute.For<ISettingsNavigationCheck>();
                _regionCheckWithChangesWithErrors.IsRelevantForRegion(RegionWithChangesWithErrors).Returns(true);
                _regionCheckWithChangesWithErrors.CheckSettings().Returns(new SettingsCheckResult(_actionResultDictWithChangesWithErrors, true));

                _settingsNavigationChecks = new[] { _regionCheckNoChangesNoErrors, _regionCheckWithChangesNoErrors, _regionCheckNoChangesWithErrors, _regionCheckWithChangesWithErrors };

                _tabSwitchSettingsCheck = new TabSwitchSettingsCheck(_regionHelper, _settingsNavigationChecks);
            }

            private void SetCurrentRegion(string region)
            {
                _regionHelper.CurrentRegionName.Returns(region);
            }

            [Test]
            public void CheckRelevantRegion_CurrentRegionIsUnknown_ActionResultDictIsValid_SettingsHaveChangedIsFalse()
            {
                SetCurrentRegion("Unknown Region");

                var result = _tabSwitchSettingsCheck.CheckAffectedSettings();

                Assert.IsTrue(result.Result, "ActionResultDict");
                Assert.IsFalse(result.SettingsHaveChanged, "SettingsHaveChanged");
            }

            [Test]
            public void CheckRelevantRegion_RegionNoChangesNoErrors()
            {
                SetCurrentRegion("RegionNoChangesNoErrors");

                var result = _tabSwitchSettingsCheck.CheckAffectedSettings();

                Assert.AreSame(result.Result, _actionResultDictNoChangesNoErrors);
                Assert.IsFalse(result.SettingsHaveChanged, "SettingsHaveChanged");
            }

            [Test]
            public void CheckRelevantRegion_RegionWithChangesNoErrors()
            {
                SetCurrentRegion("RegionWithChangesNoErrors");

                var result = _tabSwitchSettingsCheck.CheckAffectedSettings();

                Assert.AreSame(result.Result, _actionResultDictWithChangesNoErrors);
                Assert.IsTrue(result.SettingsHaveChanged, "SettingsHaveChanged");
            }

            [Test]
            public void CheckRelevantRegion_RegionNoChangesWithErrors()
            {
                SetCurrentRegion("RegionNoChangesWithErrors");

                var result = _tabSwitchSettingsCheck.CheckAffectedSettings();

                Assert.AreSame(result.Result, _actionResultDictNoChangesWithErrors);
                Assert.IsFalse(result.SettingsHaveChanged, "SettingsHaveChanged");
            }

            [Test]
            public void CheckRelevantRegion_RegionWithChangesWithErrors()
            {
                SetCurrentRegion("RegionWithChangesWithErrors");

                var result = _tabSwitchSettingsCheck.CheckAffectedSettings();

                Assert.AreSame(result.Result, _actionResultDictWithChangesWithErrors);
                Assert.IsTrue(result.SettingsHaveChanged, "SettingsHaveChanged");
            }

            [Test]
            public void CheckAllRegions_NoRegionsWithChanges_OverallResultHasNoChanges()
            {
                _settingsNavigationChecks = new[] { _regionCheckNoChangesNoErrors, _regionCheckNoChangesWithErrors };
                _tabSwitchSettingsCheck = new TabSwitchSettingsCheck(null, _settingsNavigationChecks);

                var overallResult = _tabSwitchSettingsCheck.CheckAllSettings();

                Assert.IsFalse(overallResult.SettingsHaveChanged);
            }

            [Test]
            public void CheckAllRegions_OneRegionWithChanges_OverallResultHasChanges()
            {
                _settingsNavigationChecks = new[] { _regionCheckWithChangesNoErrors, _regionCheckNoChangesNoErrors, _regionCheckNoChangesWithErrors };
                _tabSwitchSettingsCheck = new TabSwitchSettingsCheck(null, _settingsNavigationChecks);

                var overallResult = _tabSwitchSettingsCheck.CheckAllSettings();

                Assert.IsTrue(overallResult.SettingsHaveChanged);
            }

            [Test]
            public void CheckAllRegions_NoRegionsWithErrors_OverallResultMergesToValidResult()
            {
                _settingsNavigationChecks = new[] { _regionCheckWithChangesNoErrors, _regionCheckNoChangesNoErrors };
                _tabSwitchSettingsCheck = new TabSwitchSettingsCheck(null, _settingsNavigationChecks);

                var overallResult = _tabSwitchSettingsCheck.CheckAllSettings();

                Assert.IsTrue(overallResult.Result);
                Assert.AreEqual(2, overallResult.Result.Count);

                Assert.Contains(RegionWithChangesNoErrors, overallResult.Result.Keys);
                Assert.AreEqual(overallResult.Result[RegionWithChangesNoErrors], _actionResultDictWithChangesNoErrors[RegionWithChangesNoErrors]);
                overallResult.Result.Remove(RegionWithChangesNoErrors);

                Assert.Contains(RegionNoChangesNoErrors, overallResult.Result.Keys);
                Assert.AreEqual(overallResult.Result[RegionNoChangesNoErrors], _actionResultDictNoChangesNoErrors[RegionNoChangesNoErrors]);
                overallResult.Result.Remove(RegionNoChangesNoErrors);

                Assert.IsEmpty(overallResult.Result, "Overall result contains unexcpeted ActionResultDicts");
            }

            [Test]
            public void CheckAllRegions_AllRegions_OverallResultUnionsActionResultDicts()
            {
                _tabSwitchSettingsCheck = new TabSwitchSettingsCheck(null, _settingsNavigationChecks);

                var overallResult = _tabSwitchSettingsCheck.CheckAllSettings();

                Assert.IsFalse(overallResult.Result);
                Assert.AreEqual(4, overallResult.Result.Count);

                Assert.Contains(RegionNoChangesNoErrors, overallResult.Result.Keys);
                Assert.AreEqual(overallResult.Result[RegionNoChangesNoErrors], _actionResultDictNoChangesNoErrors[RegionNoChangesNoErrors]);
                overallResult.Result.Remove(RegionNoChangesNoErrors);

                Assert.Contains(RegionWithChangesNoErrors, overallResult.Result.Keys);
                Assert.AreEqual(overallResult.Result[RegionWithChangesNoErrors], _actionResultDictWithChangesNoErrors[RegionWithChangesNoErrors]);
                overallResult.Result.Remove(RegionWithChangesNoErrors);

                Assert.Contains(RegionNoChangesWithErrors, overallResult.Result.Keys);
                Assert.AreEqual(overallResult.Result[RegionNoChangesWithErrors], _actionResultDictNoChangesWithErrors[RegionNoChangesWithErrors]);
                overallResult.Result.Remove(RegionNoChangesWithErrors);

                Assert.Contains(RegionWithChangesWithErrors, overallResult.Result.Keys);
                Assert.AreEqual(overallResult.Result[RegionWithChangesWithErrors], _actionResultDictWithChangesWithErrors[RegionWithChangesWithErrors]);
                overallResult.Result.Remove(RegionWithChangesWithErrors);

                Assert.IsEmpty(overallResult.Result, "Overall result contains unexcpeted ActionResultDicts");
            }

            [Test]
            public void CheckAllRegions_RegionDuplicates_OverallReusltContainsActionResultsOnlyOnceAndRegionsOnlyOnce()
            {
                _settingsNavigationChecks = new[]
                {
                    _regionCheckNoChangesWithErrors, _regionCheckNoChangesWithErrors, _regionCheckNoChangesWithErrors,
                    _regionCheckWithChangesWithErrors, _regionCheckWithChangesWithErrors, _regionCheckWithChangesWithErrors
                };
                _tabSwitchSettingsCheck = new TabSwitchSettingsCheck(null, _settingsNavigationChecks);

                var overallResult = _tabSwitchSettingsCheck.CheckAllSettings();

                Assert.AreEqual(2, overallResult.Result.Count);
                Assert.Contains(RegionNoChangesWithErrors, overallResult.Result.Keys);
                var actionResult = overallResult.Result[RegionNoChangesWithErrors];
                Assert.AreEqual(1, actionResult.Count);
            }
        }
    }
}
