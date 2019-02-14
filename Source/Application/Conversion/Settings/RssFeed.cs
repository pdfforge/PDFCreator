using pdfforge.DataStorage.Storage;
using pdfforge.DataStorage;
using PropertyChanged;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System;

// ! This file is generated automatically.
// ! Do not edit it outside the sections for custom code.
// ! These changes will be deleted during the next generation run

namespace pdfforge.PDFCreator.Conversion.Settings
{
	[ImplementPropertyChanged]
	public partial class RssFeed : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// Enables the RSS news feed
		/// </summary>
		public bool Enable { get; set; } = true;
		
		/// <summary>
		/// Date of last RSS feed content
		/// </summary>
		public DateTime LatestRssUpdate { get; set; } = DateTime.Now;
		
		
		public void ReadValues(Data data, string path = "")
		{
			Enable = bool.TryParse(data.GetValue(@"" + path + @"Enable"), out var tmpEnable) ? tmpEnable : true;
			LatestRssUpdate = DateTime.TryParse(data.GetValue(@"" + path + @"LatestRssUpdate"), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var tmpLatestRssUpdate) ? tmpLatestRssUpdate : DateTime.Now;
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Enable", Enable.ToString());
			data.SetValue(@"" + path + @"LatestRssUpdate", LatestRssUpdate.ToString("yyyy-MM-dd HH:mm:ss"));
		}
		
		public RssFeed Copy()
		{
			RssFeed copy = new RssFeed();
			
			copy.Enable = Enable;
			copy.LatestRssUpdate = LatestRssUpdate;
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is RssFeed)) return false;
			RssFeed v = o as RssFeed;
			
			if (!Enable.Equals(v.Enable)) return false;
			if (!LatestRssUpdate.Equals(v.LatestRssUpdate)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
