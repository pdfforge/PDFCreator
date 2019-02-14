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
	/// Compression settings for monochrome images
	/// </summary>
	[ImplementPropertyChanged]
	public partial class CompressMonochrome : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// Settings for the compression method.
		/// </summary>
		public CompressionMonochrome Compression { get; set; } = CompressionMonochrome.CcittFaxEncoding;
		
		/// <summary>
		/// Images will be resampled to this maximum resolution of the images, if resampling is enabled
		/// </summary>
		public int Dpi { get; set; } = 1200;
		
		/// <summary>
		/// If true, monochrome images will be processed according to the algorithm. If false, they will remain uncompressed
		/// </summary>
		public bool Enabled { get; set; } = true;
		
		/// <summary>
		/// If true, the images will be resampled to a maximum resolution
		/// </summary>
		public bool Resampling { get; set; } = false;
		
		
		public void ReadValues(Data data, string path = "")
		{
			Compression = Enum.TryParse<CompressionMonochrome>(data.GetValue(@"" + path + @"Compression"), out var tmpCompression) ? tmpCompression : CompressionMonochrome.CcittFaxEncoding;
			Dpi = int.TryParse(data.GetValue(@"" + path + @"Dpi"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpDpi) ? tmpDpi : 1200;
			Enabled = bool.TryParse(data.GetValue(@"" + path + @"Enabled"), out var tmpEnabled) ? tmpEnabled : true;
			Resampling = bool.TryParse(data.GetValue(@"" + path + @"Resampling"), out var tmpResampling) ? tmpResampling : false;
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Compression", Compression.ToString());
			data.SetValue(@"" + path + @"Dpi", Dpi.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"Resampling", Resampling.ToString());
		}
		
		public CompressMonochrome Copy()
		{
			CompressMonochrome copy = new CompressMonochrome();
			
			copy.Compression = Compression;
			copy.Dpi = Dpi;
			copy.Enabled = Enabled;
			copy.Resampling = Resampling;
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is CompressMonochrome)) return false;
			CompressMonochrome v = o as CompressMonochrome;
			
			if (!Compression.Equals(v.Compression)) return false;
			if (!Dpi.Equals(v.Dpi)) return false;
			if (!Enabled.Equals(v.Enabled)) return false;
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
