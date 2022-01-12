using pdfforge.DataStorage.Storage;
using pdfforge.DataStorage;
using System.Collections.Generic;
using System.Text;
using System;

// ! This file is generated automatically.
// ! Do not edit it outside the sections for custom code.
// ! These changes will be deleted during the next generation run

namespace pdfforge.PDFCreator.Core.SettingsManagement.GPO.Settings
{
	public partial class CreatorAppSettings {
		
		/// <summary>
		/// Set the number of minutes PDFCreator will remain in standby after running. Set to 0 to disable.
		/// </summary>
		public int HotStandbyMinutes { get; set; } = -1;
		
		
		public void ReadValues(Data data, string path = "")
		{
			HotStandbyMinutes = int.TryParse(data.GetValue(@"" + path + @"HotStandbyMinutes"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpHotStandbyMinutes) ? tmpHotStandbyMinutes : -1;
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"HotStandbyMinutes", HotStandbyMinutes.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}
		
		public CreatorAppSettings Copy()
		{
			CreatorAppSettings copy = new CreatorAppSettings();
			
			copy.HotStandbyMinutes = HotStandbyMinutes;
			return copy;
		}
		
		public void ReplaceWith(CreatorAppSettings source)
		{
			if(HotStandbyMinutes != source.HotStandbyMinutes)
				HotStandbyMinutes = source.HotStandbyMinutes;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is CreatorAppSettings)) return false;
			CreatorAppSettings v = o as CreatorAppSettings;
			
			if (!HotStandbyMinutes.Equals(v.HotStandbyMinutes)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
