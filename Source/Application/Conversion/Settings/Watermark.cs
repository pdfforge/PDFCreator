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
	/// Adds a watermark to the resulting document
	/// </summary>
	public partial class Watermark : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// Enables the WatermarkAction
		/// </summary>
		public bool Enabled { get; set; } = false;
		
		/// <summary>
		/// Filename of the PDF that will be used as watermark
		/// </summary>
		public string File { get; set; } = "";
		
		/// <summary>
		/// Enable to resize the watermark to fit the document page
		/// </summary>
		public bool FitToPage { get; set; } = false;
		
		/// <summary>
		/// Opacity for watermark in percent
		/// </summary>
		public int Opacity { get; set; } = 50;
		
		/// <summary>
		/// Defines the way the watermark document is repeated
		/// </summary>
		public BackgroundRepetition Repetition { get; set; } = BackgroundRepetition.RepeatLastPage;
		
		
		public void ReadValues(Data data, string path = "")
		{
			Enabled = bool.TryParse(data.GetValue(@"" + path + @"Enabled"), out var tmpEnabled) ? tmpEnabled : false;
			try { File = Data.UnescapeString(data.GetValue(@"" + path + @"File")); } catch { File = "";}
			FitToPage = bool.TryParse(data.GetValue(@"" + path + @"FitToPage"), out var tmpFitToPage) ? tmpFitToPage : false;
			Opacity = int.TryParse(data.GetValue(@"" + path + @"Opacity"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpOpacity) ? tmpOpacity : 50;
			Repetition = Enum.TryParse<BackgroundRepetition>(data.GetValue(@"" + path + @"Repetition"), out var tmpRepetition) ? tmpRepetition : BackgroundRepetition.RepeatLastPage;
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"File", Data.EscapeString(File));
			data.SetValue(@"" + path + @"FitToPage", FitToPage.ToString());
			data.SetValue(@"" + path + @"Opacity", Opacity.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"Repetition", Repetition.ToString());
		}
		
		public Watermark Copy()
		{
			Watermark copy = new Watermark();
			
			copy.Enabled = Enabled;
			copy.File = File;
			copy.FitToPage = FitToPage;
			copy.Opacity = Opacity;
			copy.Repetition = Repetition;
			return copy;
		}
		
		public void ReplaceWith(Watermark source)
		{
			if(Enabled != source.Enabled)
				Enabled = source.Enabled;
				
			if(File != source.File)
				File = source.File;
				
			if(FitToPage != source.FitToPage)
				FitToPage = source.FitToPage;
				
			if(Opacity != source.Opacity)
				Opacity = source.Opacity;
				
			if(Repetition != source.Repetition)
				Repetition = source.Repetition;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is Watermark)) return false;
			Watermark v = o as Watermark;
			
			if (!Enabled.Equals(v.Enabled)) return false;
			if (!File.Equals(v.File)) return false;
			if (!FitToPage.Equals(v.FitToPage)) return false;
			if (!Opacity.Equals(v.Opacity)) return false;
			if (!Repetition.Equals(v.Repetition)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
