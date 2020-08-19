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
	public partial class ForwardToFurtherProfile : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		public bool Enabled { get; set; } = false;
		
		public string ProfileGuid { get; set; } = "";
		
		
		public void ReadValues(Data data, string path = "")
		{
			Enabled = bool.TryParse(data.GetValue(@"" + path + @"Enabled"), out var tmpEnabled) ? tmpEnabled : false;
			try { ProfileGuid = Data.UnescapeString(data.GetValue(@"" + path + @"ProfileGuid")); } catch { ProfileGuid = "";}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"ProfileGuid", Data.EscapeString(ProfileGuid));
		}
		
		public ForwardToFurtherProfile Copy()
		{
			ForwardToFurtherProfile copy = new ForwardToFurtherProfile();
			
			copy.Enabled = Enabled;
			copy.ProfileGuid = ProfileGuid;
			return copy;
		}
		
		public void ReplaceWith(ForwardToFurtherProfile source)
		{
			if(Enabled != source.Enabled)
				Enabled = source.Enabled;
				
			if(ProfileGuid != source.ProfileGuid)
				ProfileGuid = source.ProfileGuid;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is ForwardToFurtherProfile)) return false;
			ForwardToFurtherProfile v = o as ForwardToFurtherProfile;
			
			if (!Enabled.Equals(v.Enabled)) return false;
			if (!ProfileGuid.Equals(v.ProfileGuid)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
