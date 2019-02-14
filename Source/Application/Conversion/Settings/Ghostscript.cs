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
	/// Ghostscript settings
	/// </summary>
	[ImplementPropertyChanged]
	public partial class Ghostscript : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// These parameters will be provided to Ghostscript in addition to the PDFCreator parameters
		/// </summary>
		public string AdditionalGsParameters { get; set; } = "";
		
		
		public void ReadValues(Data data, string path = "")
		{
			try { AdditionalGsParameters = Data.UnescapeString(data.GetValue(@"" + path + @"AdditionalGsParameters")); } catch { AdditionalGsParameters = "";}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"AdditionalGsParameters", Data.EscapeString(AdditionalGsParameters));
		}
		
		public Ghostscript Copy()
		{
			Ghostscript copy = new Ghostscript();
			
			copy.AdditionalGsParameters = AdditionalGsParameters;
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is Ghostscript)) return false;
			Ghostscript v = o as Ghostscript;
			
			if (!AdditionalGsParameters.Equals(v.AdditionalGsParameters)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
