using pdfforge.DataStorage.Storage;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using PropertyChanged;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System;

// ! This file is generated automatically.
// ! Do not edit it outside the sections for custom code.
// ! These changes will be deleted during the next generation run

namespace pdfforge.PDFCreator.Conversion.Settings
{
	/// <summary>
	/// Settings for page numbers.
	/// </summary>
	public partial class PageNumbers : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// Whether to switch left/right corner every page.
		/// </summary>
		public bool AlternateCorner { get; set; } = false;
		
		/// <summary>
		/// The page to begin numbering on.
		/// </summary>
		public int BeginOn { get; set; } = 1;
		
		/// <summary>
		/// The number to start counting at.
		/// </summary>
		public int BeginWith { get; set; } = 1;
		
		/// <summary>
		/// If true, this action will be executed
		/// </summary>
		public bool Enabled { get; set; } = false;
		
		/// <summary>
		/// The color of the page numbers.
		/// </summary>
		public Color FontColor { get; set; } = ColorTranslator.FromHtml("#000000");
		
		/// <summary>
		/// The font of the page numbers.
		/// </summary>
		public string FontFile { get; set; } = "C:/Windows/Fonts/Arial.ttf";
		
		/// <summary>
		/// The name of the font of the page numbers.
		/// </summary>
		public string FontName { get; set; } = "Arial";
		
		/// <summary>
		/// The size of the page numbers.
		/// </summary>
		public float FontSize { get; set; } = 10f;
		
		/// <summary>
		/// Format of the page numbers.
		/// </summary>
		public string Format { get; set; } = "<PageNumber>/<NumberOfPages>";
		
		/// <summary>
		/// The horizontalOffset offset from the page border of the page numbers in points.
		/// </summary>
		public float HorizontalOffset { get; set; } = 72f;
		
		/// <summary>
		/// Where to put the page numbers.
		/// </summary>
		public PageNumberPosition Position { get; set; } = PageNumberPosition.BottomRight;
		
		/// <summary>
		/// Defines the unit of measurement for the page number position.
		/// </summary>
		public UnitOfMeasurement UnitOfMeasurement { get; set; } = UnitOfMeasurement.Centimeter;
		
		/// <summary>
		/// Print the numbers as roman numerals.
		/// </summary>
		public bool UseRomanNumerals { get; set; } = false;
		
		/// <summary>
		/// The vertical offset from the page border of the page numbers in points.
		/// </summary>
		public float VerticalOffset { get; set; } = 72f;
		
		
		public void ReadValues(Data data, string path = "")
		{
			AlternateCorner = bool.TryParse(data.GetValue(@"" + path + @"AlternateCorner"), out var tmpAlternateCorner) ? tmpAlternateCorner : false;
			BeginOn = int.TryParse(data.GetValue(@"" + path + @"BeginOn"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpBeginOn) ? tmpBeginOn : 1;
			BeginWith = int.TryParse(data.GetValue(@"" + path + @"BeginWith"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpBeginWith) ? tmpBeginWith : 1;
			Enabled = bool.TryParse(data.GetValue(@"" + path + @"Enabled"), out var tmpEnabled) ? tmpEnabled : false;
			try
			{
				string value = data.GetValue(@"" + path + @"FontColor").Trim();
				if (value.Length == 0) FontColor = ColorTranslator.FromHtml("#000000"); else FontColor = ColorTranslator.FromHtml(value);
			}
			catch { FontColor =  ColorTranslator.FromHtml("#000000");}
			try { FontFile = Data.UnescapeString(data.GetValue(@"" + path + @"FontFile")); } catch { FontFile = "C:/Windows/Fonts/Arial.ttf";}
			try { FontName = Data.UnescapeString(data.GetValue(@"" + path + @"FontName")); } catch { FontName = "Arial";}
			FontSize = float.TryParse(data.GetValue(@"" + path + @"FontSize"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpFontSize) ? tmpFontSize : 10f;
			try { Format = Data.UnescapeString(data.GetValue(@"" + path + @"Format")); } catch { Format = "<PageNumber>/<NumberOfPages>";}
			HorizontalOffset = float.TryParse(data.GetValue(@"" + path + @"HorizontalOffset"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpHorizontalOffset) ? tmpHorizontalOffset : 72f;
			Position = Enum.TryParse<PageNumberPosition>(data.GetValue(@"" + path + @"Position"), out var tmpPosition) ? tmpPosition : PageNumberPosition.BottomRight;
			UnitOfMeasurement = Enum.TryParse<UnitOfMeasurement>(data.GetValue(@"" + path + @"UnitOfMeasurement"), out var tmpUnitOfMeasurement) ? tmpUnitOfMeasurement : UnitOfMeasurement.Centimeter;
			UseRomanNumerals = bool.TryParse(data.GetValue(@"" + path + @"UseRomanNumerals"), out var tmpUseRomanNumerals) ? tmpUseRomanNumerals : false;
			VerticalOffset = float.TryParse(data.GetValue(@"" + path + @"VerticalOffset"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpVerticalOffset) ? tmpVerticalOffset : 72f;
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"AlternateCorner", AlternateCorner.ToString());
			data.SetValue(@"" + path + @"BeginOn", BeginOn.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"BeginWith", BeginWith.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"FontColor", ColorTranslator.ToHtml(FontColor));
			data.SetValue(@"" + path + @"FontFile", Data.EscapeString(FontFile));
			data.SetValue(@"" + path + @"FontName", Data.EscapeString(FontName));
			data.SetValue(@"" + path + @"FontSize", FontSize.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"Format", Data.EscapeString(Format));
			data.SetValue(@"" + path + @"HorizontalOffset", HorizontalOffset.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"Position", Position.ToString());
			data.SetValue(@"" + path + @"UnitOfMeasurement", UnitOfMeasurement.ToString());
			data.SetValue(@"" + path + @"UseRomanNumerals", UseRomanNumerals.ToString());
			data.SetValue(@"" + path + @"VerticalOffset", VerticalOffset.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}
		
		public PageNumbers Copy()
		{
			PageNumbers copy = new PageNumbers();
			
			copy.AlternateCorner = AlternateCorner;
			copy.BeginOn = BeginOn;
			copy.BeginWith = BeginWith;
			copy.Enabled = Enabled;
			copy.FontColor = FontColor;
			copy.FontFile = FontFile;
			copy.FontName = FontName;
			copy.FontSize = FontSize;
			copy.Format = Format;
			copy.HorizontalOffset = HorizontalOffset;
			copy.Position = Position;
			copy.UnitOfMeasurement = UnitOfMeasurement;
			copy.UseRomanNumerals = UseRomanNumerals;
			copy.VerticalOffset = VerticalOffset;
			return copy;
		}
		
		public void ReplaceWith(PageNumbers source)
		{
			if(AlternateCorner != source.AlternateCorner)
				AlternateCorner = source.AlternateCorner;
				
			if(BeginOn != source.BeginOn)
				BeginOn = source.BeginOn;
				
			if(BeginWith != source.BeginWith)
				BeginWith = source.BeginWith;
				
			if(Enabled != source.Enabled)
				Enabled = source.Enabled;
				
			if(FontColor != source.FontColor)
				FontColor = source.FontColor;
				
			if(FontFile != source.FontFile)
				FontFile = source.FontFile;
				
			if(FontName != source.FontName)
				FontName = source.FontName;
				
			if(FontSize != source.FontSize)
				FontSize = source.FontSize;
				
			if(Format != source.Format)
				Format = source.Format;
				
			if(HorizontalOffset != source.HorizontalOffset)
				HorizontalOffset = source.HorizontalOffset;
				
			if(Position != source.Position)
				Position = source.Position;
				
			if(UnitOfMeasurement != source.UnitOfMeasurement)
				UnitOfMeasurement = source.UnitOfMeasurement;
				
			if(UseRomanNumerals != source.UseRomanNumerals)
				UseRomanNumerals = source.UseRomanNumerals;
				
			if(VerticalOffset != source.VerticalOffset)
				VerticalOffset = source.VerticalOffset;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is PageNumbers)) return false;
			PageNumbers v = o as PageNumbers;
			
			if (!Object.Equals(AlternateCorner, v.AlternateCorner)) return false;
			if (!Object.Equals(BeginOn, v.BeginOn)) return false;
			if (!Object.Equals(BeginWith, v.BeginWith)) return false;
			if (!Object.Equals(Enabled, v.Enabled)) return false;
			if (!Object.Equals(FontColor, v.FontColor)) return false;
			if (!Object.Equals(FontFile, v.FontFile)) return false;
			if (!Object.Equals(FontName, v.FontName)) return false;
			if (!Object.Equals(FontSize, v.FontSize)) return false;
			if (!Object.Equals(Format, v.Format)) return false;
			if (!Object.Equals(HorizontalOffset, v.HorizontalOffset)) return false;
			if (!Object.Equals(Position, v.Position)) return false;
			if (!Object.Equals(UnitOfMeasurement, v.UnitOfMeasurement)) return false;
			if (!Object.Equals(UseRomanNumerals, v.UseRomanNumerals)) return false;
			if (!Object.Equals(VerticalOffset, v.VerticalOffset)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
