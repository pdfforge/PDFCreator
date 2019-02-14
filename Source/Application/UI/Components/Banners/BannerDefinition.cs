using System;
using System.Collections.Generic;

namespace Banners
{
    public class BannerDefinition
    {
        public string BundleId { get; set; }
        public BundleType BundleType { get; set; }
        public int Version { get; set; }
        public string DownloadLink { get; set; }
        public string DownloadMd5 { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTill { get; set; }
        public int Priority { get; set; }
        public int ProbabilityFactor { get; set; }
        public string Slot { get; set; }
        public string Campaign { get; set; }
        public string Link { get; set; }
        public Dictionary<string, string> LinkParameters { get; set; }
    }

    public class BannerData
    {
        public IList<BannerDefinition> Banners { get; set; }
        public string Source { get; set; }
        public string Language { get; set; }
    }
}
