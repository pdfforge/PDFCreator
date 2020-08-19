using pdfforge.DataStorage.Storage;
using pdfforge.DataStorage;
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
	/// Place a stamp text on all pages of the document
	/// </summary>
	public partial class Stamping : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// Color of the text
		/// </summary>
		public Color Color { get; set; } = ColorTranslator.FromHtml("#CCCCCC");
		
		/// <summary>
		/// If true, the document all pages will be stamped with a text
		/// </summary>
		public bool Enabled { get; set; } = false;
		
		/// <summary>
		/// If true, the text will be rendered as outline. If false, it will be filled.
		/// </summary>
		public bool FontAsOutline { get; set; } = true;
		
		/// <summary>
		/// PostScript name of the stamp font.
		/// </summary>
		public string FontFile { get; set; } = "arial.ttf";
		
		/// <summary>
		/// Name of the font. (this is only used as a hint, the PostScriptFontName contains the real name)
		/// </summary>
		public string FontName { get; set; } = "Arial";
		
		/// <summary>
		/// Width of the outline
		/// </summary>
		public int FontOutlineWidth { get; set; } = 2;
		
		/// <summary>
		/// Size of the font
		/// </summary>
		public float FontSize { get; set; } = 48;
		
		/// <summary>
		/// Text that will be stamped
		/// </summary>
		public string StampText { get; set; } = "Confidential";
		
		
		public void ReadValues(Data data, string path = "")
		{
			try
			{
				string value = data.GetValue(@"" + path + @"Color").Trim();
				if (value.Length == 0) Color = ColorTranslator.FromHtml("#CCCCCC"); else Color = ColorTranslator.FromHtml(value);
			}
			catch { Color =  ColorTranslator.FromHtml("#CCCCCC");}
			Enabled = bool.TryParse(data.GetValue(@"" + path + @"Enabled"), out var tmpEnabled) ? tmpEnabled : false;
			FontAsOutline = bool.TryParse(data.GetValue(@"" + path + @"FontAsOutline"), out var tmpFontAsOutline) ? tmpFontAsOutline : true;
			try { FontFile = Data.UnescapeString(data.GetValue(@"" + path + @"FontFile")); } catch { FontFile = "arial.ttf";}
			try { FontName = Data.UnescapeString(data.GetValue(@"" + path + @"FontName")); } catch { FontName = "Arial";}
			FontOutlineWidth = int.TryParse(data.GetValue(@"" + path + @"FontOutlineWidth"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpFontOutlineWidth) ? tmpFontOutlineWidth : 2;
			FontSize = float.TryParse(data.GetValue(@"" + path + @"FontSize"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpFontSize) ? tmpFontSize : 48;
			try { StampText = Data.UnescapeString(data.GetValue(@"" + path + @"StampText")); } catch { StampText = "Confidential";}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Color", ColorTranslator.ToHtml(Color));
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"FontAsOutline", FontAsOutline.ToString());
			data.SetValue(@"" + path + @"FontFile", Data.EscapeString(FontFile));
			data.SetValue(@"" + path + @"FontName", Data.EscapeString(FontName));
			data.SetValue(@"" + path + @"FontOutlineWidth", FontOutlineWidth.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"FontSize", FontSize.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"StampText", Data.EscapeString(StampText));
		}
		
		public Stamping Copy()
		{
			Stamping copy = new Stamping();
			
			copy.Color = Color;
			copy.Enabled = Enabled;
			copy.FontAsOutline = FontAsOutline;
			copy.FontFile = FontFile;
			copy.FontName = FontName;
			copy.FontOutlineWidth = FontOutlineWidth;
			copy.FontSize = FontSize;
			copy.StampText = StampText;
			return copy;
		}
		
		public void ReplaceWith(Stamping source)
		{
			if(Color != source.Color)
				Color = source.Color;
				
			if(Enabled != source.Enabled)
				Enabled = source.Enabled;
				
			if(FontAsOutline != source.FontAsOutline)
				FontAsOutline = source.FontAsOutline;
				
			if(FontFile != source.FontFile)
				FontFile = source.FontFile;
				
			if(FontName != source.FontName)
				FontName = source.FontName;
				
			if(FontOutlineWidth != source.FontOutlineWidth)
				FontOutlineWidth = source.FontOutlineWidth;
				
			if(FontSize != source.FontSize)
				FontSize = source.FontSize;
				
			if(StampText != source.StampText)
				StampText = source.StampText;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is Stamping)) return false;
			Stamping v = o as Stamping;
			
			if (!Color.Equals(v.Color)) return false;
			if (!Enabled.Equals(v.Enabled)) return false;
			if (!FontAsOutline.Equals(v.FontAsOutline)) return false;
			if (!FontFile.Equals(v.FontFile)) return false;
			if (!FontName.Equals(v.FontName)) return false;
			if (!FontOutlineWidth.Equals(v.FontOutlineWidth)) return false;
			if (!FontSize.Equals(v.FontSize)) return false;
			if (!StampText.Equals(v.StampText)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
