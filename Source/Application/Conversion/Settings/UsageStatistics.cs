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
	public partial class UsageStatistics : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// Enable usage statistics
		/// </summary>
		public bool Enable { get; set; } = true;
		
		/// <summary>
		/// Show or hide the usage statistics info on first start
		/// </summary>
		public bool UsageStatsInfo { get; set; } = true;
		
		
		public void ReadValues(Data data, string path = "")
		{
			Enable = bool.TryParse(data.GetValue(@"" + path + @"Enable"), out var tmpEnable) ? tmpEnable : true;
			UsageStatsInfo = bool.TryParse(data.GetValue(@"" + path + @"UsageStatsInfo"), out var tmpUsageStatsInfo) ? tmpUsageStatsInfo : true;
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Enable", Enable.ToString());
			data.SetValue(@"" + path + @"UsageStatsInfo", UsageStatsInfo.ToString());
		}
		
		public UsageStatistics Copy()
		{
			UsageStatistics copy = new UsageStatistics();
			
			copy.Enable = Enable;
			copy.UsageStatsInfo = UsageStatsInfo;
			return copy;
		}
		
		public void ReplaceWith(UsageStatistics source)
		{
			if(Enable != source.Enable)
				Enable = source.Enable;
				
			if(UsageStatsInfo != source.UsageStatsInfo)
				UsageStatsInfo = source.UsageStatsInfo;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is UsageStatistics)) return false;
			UsageStatistics v = o as UsageStatistics;
			
			if (!Enable.Equals(v.Enable)) return false;
			if (!UsageStatsInfo.Equals(v.UsageStatsInfo)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
