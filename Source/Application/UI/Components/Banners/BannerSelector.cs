using Optional;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Banners
{
    public class BannerSelector
    {
        private readonly Random _random = new Random();

        public Option<BannerDefinition> SelectBanner(IEnumerable<BannerDefinition> banners, string slot)
        {
            var activeBanners = GetActiveBanners(banners, slot).ToList();

            if (!activeBanners.Any())
                return Option.None<BannerDefinition>();

            var highestPriority = activeBanners.Max(b => b.Priority);

            var prioritizedBanners = activeBanners.Where(b => b.Priority == highestPriority);

            return SelectRandomBanner(prioritizedBanners).Some();
        }

        private BannerDefinition SelectRandomBanner(IEnumerable<BannerDefinition> banners)
        {
            var lotteryList = new List<BannerDefinition>();

            foreach (var banner in banners)
            {
                if (banner.ProbabilityFactor < 1)
                    banner.ProbabilityFactor = 1;

                for (int i = 0; i < banner.ProbabilityFactor; i++)
                {
                    lotteryList.Add(banner);
                }
            }

            return lotteryList[_random.Next(lotteryList.Count)];
        }

        private IEnumerable<BannerDefinition> GetActiveBanners(IEnumerable<BannerDefinition> banners, string slot)
        {
            return banners.Where(b => b.Slot == slot && b.ValidFrom <= DateTime.Now && b.ValidTill >= DateTime.Now);
        }
    }
}
