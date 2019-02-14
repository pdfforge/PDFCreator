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
	/// Appends one or more pages at the end of the converted document
	/// </summary>
	[ImplementPropertyChanged]
	public partial class AttachmentPage : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// Enables the AttachmentPage action
		/// </summary>
		public bool Enabled { get; set; } = false;
		
		/// <summary>
		/// Filename of the PDF that will be appended
		/// </summary>
		public string File { get; set; } = "";
		
		
		public void ReadValues(Data data, string path = "")
		{
			Enabled = bool.TryParse(data.GetValue(@"" + path + @"Enabled"), out var tmpEnabled) ? tmpEnabled : false;
			try { File = Data.UnescapeString(data.GetValue(@"" + path + @"File")); } catch { File = "";}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"File", Data.EscapeString(File));
		}
		
		public AttachmentPage Copy()
		{
			AttachmentPage copy = new AttachmentPage();
			
			copy.Enabled = Enabled;
			copy.File = File;
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is AttachmentPage)) return false;
			AttachmentPage v = o as AttachmentPage;
			
			if (!Enabled.Equals(v.Enabled)) return false;
			if (!File.Equals(v.File)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
