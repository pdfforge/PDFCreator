using NUnit.Framework;
using System;
using System.Linq;

namespace Banners.UnitTest
{
    [TestFixture]
    public class BannerSelectorTest
    {
        private BannerSelector _bannerSelector;

        private BannerDefinition MakeValidBanner(string bundleId, string slot)
        {
            return new BannerDefinition
            {
                BundleId = bundleId,
                Slot = slot,
                ValidFrom = DateTime.Today,
                ValidTill = DateTime.Now.AddDays(1)
            };
        }

        private BannerDefinition MakeInvalidBanner(string bundleId, string slot, bool beforeToday)
        {
            if (beforeToday)
                return new BannerDefinition
                {
                    BundleId = bundleId,
                    Slot = slot,
                    ValidFrom = DateTime.Today.AddDays(-5),
                    ValidTill = DateTime.Today.AddDays(-1)
                };
            else
                return new BannerDefinition
                {
                    BundleId = bundleId,
                    Slot = slot,
                    ValidFrom = DateTime.Today.AddDays(3),
                    ValidTill = DateTime.Now.AddDays(5)
                };
        }

        [SetUp]
        public void Setup()
        {
            _bannerSelector = new BannerSelector();
        }

        [Test]
        public void SelectBanner_WithEmptyList_ReturnsNone()
        {
            var banners = new BannerDefinition[] { };

            var banner = _bannerSelector.SelectBanner(banners, "slot1");

            Assert.IsFalse(banner.HasValue);
        }

        [Test]
        public void SelectBanner_WithMatchingSlot_ReturnsBanner()
        {
            var banners = new[]
            {
                MakeValidBanner("banner1", "slot1"),
                MakeValidBanner("banner2", "slot1")
            };

            var banner = _bannerSelector.SelectBanner(banners, "slot1");

            Assert.IsTrue(banner.HasValue);

            banner.MatchSome(b => Assert.IsTrue(banners.Contains(b)));
        }

        [Test]
        public void SelectBanner_TestRandomness()
        {
            var banners = new[]
            {
                MakeValidBanner("banner1", "slot1"),
                MakeValidBanner("banner2", "slot1")
            };

            var banner1 = _bannerSelector.SelectBanner(banners, "slot1");

            Assert.IsTrue(banner1.HasValue);

            for (int i = 0; i < 1000; i++)
            {
                var banner2 = _bannerSelector.SelectBanner(banners, "slot1");
                Assert.IsTrue(banner2.HasValue);

                if (banner1.ValueOr((BannerDefinition)null) != banner2.ValueOr((BannerDefinition)null))
                    return;
            }

            Assert.Fail("Randomness does not seem to work: Selector was unable to choose second banner in 1000 attempts!");
        }

        [Test]
        public void SelectBanner_WithoutMatchingSlot_ReturnsNone()
        {
            var banners = new[] { MakeValidBanner("banner1", "slot1"), MakeValidBanner("banner2", "slot1") };

            var banner = _bannerSelector.SelectBanner(banners, "unknown-slot");

            Assert.IsFalse(banner.HasValue);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void SelectBanner_WithInvalidBanner_ReturnsNone(bool beforeToday)
        {
            var banners = new[] { MakeInvalidBanner("banner1", "slot1", beforeToday) };

            var banner = _bannerSelector.SelectBanner(banners, "unknown-slot");

            Assert.IsFalse(banner.HasValue);
        }
    }
}
