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
	/// <summary>
	/// AutoSave allows to create PDF files without user interaction
	/// </summary>
	[ImplementPropertyChanged]
	public partial class AutoSave : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		public bool Enabled { get; set; } = false;
		
		/// <summary>
		/// Existing files will not be overwritten. Existing filenames automatically get an appendix.
		/// </summary>
		public bool EnsureUniqueFilenames { get; set; } = true;
		
		
		public void ReadValues(Data data, string path = "")
		{
			Enabled = bool.TryParse(data.GetValue(@"" + path + @"Enabled"), out var tmpEnabled) ? tmpEnabled : false;
			EnsureUniqueFilenames = bool.TryParse(data.GetValue(@"" + path + @"EnsureUniqueFilenames"), out var tmpEnsureUniqueFilenames) ? tmpEnsureUniqueFilenames : true;
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"EnsureUniqueFilenames", EnsureUniqueFilenames.ToString());
		}
		
		public AutoSave Copy()
		{
			AutoSave copy = new AutoSave();
			
			copy.Enabled = Enabled;
			copy.EnsureUniqueFilenames = EnsureUniqueFilenames;
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is AutoSave)) return false;
			AutoSave v = o as AutoSave;
			
			if (!Enabled.Equals(v.Enabled)) return false;
			if (!EnsureUniqueFilenames.Equals(v.EnsureUniqueFilenames)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
