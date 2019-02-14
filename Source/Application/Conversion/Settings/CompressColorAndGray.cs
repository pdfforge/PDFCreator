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
	/// Compression settings for color and greyscale images
	/// </summary>
	[ImplementPropertyChanged]
	public partial class CompressColorAndGray : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// Settings for the compression method.
		/// </summary>
		public CompressionColorAndGray Compression { get; set; } = CompressionColorAndGray.Automatic;
		
		/// <summary>
		/// Images will be resampled to this maximum resolution of the images, if resampling is enabled
		/// </summary>
		public int Dpi { get; set; } = 300;
		
		/// <summary>
		/// If true, color and grayscale images will be processed according to the algorithm. If false, they will remain uncompressed
		/// </summary>
		public bool Enabled { get; set; } = true;
		
		/// <summary>
		/// Define a custom compression factor (requires JpegManual as method)
		/// </summary>
		public double JpegCompressionFactor { get; set; } = 0.66;
		
		/// <summary>
		/// If true, the images will be resampled to a maximum resolution
		/// </summary>
		public bool Resampling { get; set; } = false;
		
		
		public void ReadValues(Data data, string path = "")
		{
			Compression = Enum.TryParse<CompressionColorAndGray>(data.GetValue(@"" + path + @"Compression"), out var tmpCompression) ? tmpCompression : CompressionColorAndGray.Automatic;
			Dpi = int.TryParse(data.GetValue(@"" + path + @"Dpi"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpDpi) ? tmpDpi : 300;
			Enabled = bool.TryParse(data.GetValue(@"" + path + @"Enabled"), out var tmpEnabled) ? tmpEnabled : true;
			JpegCompressionFactor = double.TryParse(data.GetValue(@"" + path + @"JpegCompressionFactor"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpJpegCompressionFactor) ? tmpJpegCompressionFactor : 0.66;
			Resampling = bool.TryParse(data.GetValue(@"" + path + @"Resampling"), out var tmpResampling) ? tmpResampling : false;
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Compression", Compression.ToString());
			data.SetValue(@"" + path + @"Dpi", Dpi.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"JpegCompressionFactor", JpegCompressionFactor.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"Resampling", Resampling.ToString());
		}
		
		public CompressColorAndGray Copy()
		{
			CompressColorAndGray copy = new CompressColorAndGray();
			
			copy.Compression = Compression;
			copy.Dpi = Dpi;
			copy.Enabled = Enabled;
			copy.JpegCompressionFactor = JpegCompressionFactor;
			copy.Resampling = Resampling;
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is CompressColorAndGray)) return false;
			CompressColorAndGray v = o as CompressColorAndGray;
			
			if (!Compression.Equals(v.Compression)) return false;
			if (!Dpi.Equals(v.Dpi)) return false;
			if (!Enabled.Equals(v.Enabled)) return false;
			if (!JpegCompressionFactor.Equals(v.JpegCompressionFactor)) return false;
			if (!Resampling.Equals(v.Resampling)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
