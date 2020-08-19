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
	public partial class TextSettings : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// Text Format (0 outputs XML-escaped Unicode along with information regarding the format of the text | 1  same XML output format, but attempts similar processing to MuPDF, and will output blocks of text | 2 outputs Unicode (UCS2) text (with a Byte Order Mark) which approximates the original text layout | 3 same as 2 encoded in UTF-8)
		/// </summary>
		public int Format { get; set; } = 2;
		
		
		public void ReadValues(Data data, string path = "")
		{
			Format = int.TryParse(data.GetValue(@"" + path + @"Format"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpFormat) ? tmpFormat : 2;
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Format", Format.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}
		
		public TextSettings Copy()
		{
			TextSettings copy = new TextSettings();
			
			copy.Format = Format;
			return copy;
		}
		
		public void ReplaceWith(TextSettings source)
		{
			if(Format != source.Format)
				Format = source.Format;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is TextSettings)) return false;
			TextSettings v = o as TextSettings;
			
			if (!Format.Equals(v.Format)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
