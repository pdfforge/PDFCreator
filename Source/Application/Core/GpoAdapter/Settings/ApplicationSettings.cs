using pdfforge.DataStorage.Storage;
using pdfforge.DataStorage;
using System.Collections.Generic;
using System.Text;
using System;

// ! This file is generated automatically.
// ! Do not edit it outside the sections for custom code.
// ! These changes will be deleted during the next generation run

namespace pdfforge.PDFCreator.Core.GpoAdapter.Settings
{
	public partial class ApplicationSettings {
		
		/// <summary>
		/// Set the display language of main program.
		/// </summary>
		public string Language { get; set; } = "";
		
		/// <summary>
		/// Set the update interval. Set "Never" to disable updates.
		/// </summary>
		public string UpdateInterval { get; set; } = "";
		
		
		public void ReadValues(Data data, string path = "")
		{
			try { Language = Data.UnescapeString(data.GetValue(@"" + path + @"Language")); } catch { Language = "";}
			try { UpdateInterval = Data.UnescapeString(data.GetValue(@"" + path + @"UpdateInterval")); } catch { UpdateInterval = "";}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Language", Data.EscapeString(Language));
			data.SetValue(@"" + path + @"UpdateInterval", Data.EscapeString(UpdateInterval));
		}
		
		public ApplicationSettings Copy()
		{
			ApplicationSettings copy = new ApplicationSettings();
			
			copy.Language = Language;
			copy.UpdateInterval = UpdateInterval;
			return copy;
		}
		
		public void ReplaceWith(ApplicationSettings source)
		{
			if(Language != source.Language)
				Language = source.Language;
				
			if(UpdateInterval != source.UpdateInterval)
				UpdateInterval = source.UpdateInterval;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is ApplicationSettings)) return false;
			ApplicationSettings v = o as ApplicationSettings;
			
			if (!Language.Equals(v.Language)) return false;
			if (!UpdateInterval.Equals(v.UpdateInterval)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
