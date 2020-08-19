using pdfforge.DataStorage.Storage;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
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
	/// Settings for the JPEG output format
	/// </summary>
	public partial class JpegSettings : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// Number of colors.
		/// </summary>
		public JpegColor Color { get; set; } = JpegColor.Color24Bit;
		
		/// <summary>
		/// Resolution of the JPEG files
		/// </summary>
		public int Dpi { get; set; } = 150;
		
		/// <summary>
		/// Quality factor of the resulting JPEG (100 is best, 0 is worst)
		/// </summary>
		public int Quality { get; set; } = 75;
		
		
		public void ReadValues(Data data, string path = "")
		{
			Color = Enum.TryParse<JpegColor>(data.GetValue(@"" + path + @"Color"), out var tmpColor) ? tmpColor : JpegColor.Color24Bit;
			Dpi = int.TryParse(data.GetValue(@"" + path + @"Dpi"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpDpi) ? tmpDpi : 150;
			Quality = int.TryParse(data.GetValue(@"" + path + @"Quality"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpQuality) ? tmpQuality : 75;
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Color", Color.ToString());
			data.SetValue(@"" + path + @"Dpi", Dpi.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"Quality", Quality.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}
		
		public JpegSettings Copy()
		{
			JpegSettings copy = new JpegSettings();
			
			copy.Color = Color;
			copy.Dpi = Dpi;
			copy.Quality = Quality;
			return copy;
		}
		
		public void ReplaceWith(JpegSettings source)
		{
			if(Color != source.Color)
				Color = source.Color;
				
			if(Dpi != source.Dpi)
				Dpi = source.Dpi;
				
			if(Quality != source.Quality)
				Quality = source.Quality;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is JpegSettings)) return false;
			JpegSettings v = o as JpegSettings;
			
			if (!Color.Equals(v.Color)) return false;
			if (!Dpi.Equals(v.Dpi)) return false;
			if (!Quality.Equals(v.Quality)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
