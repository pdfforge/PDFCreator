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
	/// Digitally sign the PDF document
	/// </summary>
	public partial class Signature : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// If true, the PDF file may be signed by additional persons
		/// </summary>
		public bool AllowMultiSigning { get; set; } = false;
		
		/// <summary>
		/// The Path of the background image to use.
		/// </summary>
		public string BackgroundImageFile { get; set; } = "";
		
		/// <summary>
		/// Path to the certificate
		/// </summary>
		public string CertificateFile { get; set; } = "";
		
		/// <summary>
		/// Defines the display level of the signature in the document.
		/// </summary>
		public DisplaySignature DisplaySignature { get; set; } = DisplaySignature.NoDisplay;
		
		/// <summary>
		/// If true, the signature will be displayed in the PDF document
		/// </summary>
		public bool Enabled { get; set; } = false;
		
		/// <summary>
		/// Resize signature text to fit into displayed signature
		/// </summary>
		public bool FitTextToSignatureSize { get; set; } = true;
		
		/// <summary>
		/// Font color of the signature text
		/// </summary>
		public Color FontColor { get; set; } = ColorTranslator.FromHtml("#000000");
		
		/// <summary>
		/// PostScript name of the signature font
		/// </summary>
		public string FontFile { get; set; } = "arial.ttf";
		
		/// <summary>
		/// Font name of the signature text  (this is only used as a hint, FontFile contains the real name)
		/// </summary>
		public string FontName { get; set; } = "Arial";
		
		/// <summary>
		/// Size of the signature font
		/// </summary>
		public float FontSize { get; set; } = 12f;
		
		/// <summary>
		/// Signature location: Top left corner (X part)
		/// </summary>
		public float LeftX { get; set; } = 100f;
		
		/// <summary>
		/// Signature location: Top left corner (Y part)
		/// </summary>
		public float LeftY { get; set; } = 100f;
		
		/// <summary>
		/// Signature location: Bottom right corner (X part)
		/// </summary>
		public float RightX { get; set; } = 200f;
		
		/// <summary>
		/// Signature location: Bottom right corner (Y part)
		/// </summary>
		public float RightY { get; set; } = 200f;
		
		/// <summary>
		/// Contact name of the signature
		/// </summary>
		public string SignContact { get; set; } = "";
		
		/// <summary>
		/// Signature location
		/// </summary>
		public string SignLocation { get; set; } = "";
		
		/// <summary>
		/// Reason for the signature
		/// </summary>
		public string SignReason { get; set; } = "";
		
		/// <summary>
		/// If the signature page is set to custom, this property defines the page where the signature will be displayed
		/// </summary>
		public int SignatureCustomPage { get; set; } = 1;
		
		/// <summary>
		/// Defines the page on which the signature will be displayed.
		/// </summary>
		public SignaturePage SignaturePage { get; set; } = SignaturePage.FirstPage;
		
		/// <summary>
		/// Password for the certificate file
		/// </summary>
		private string _signaturePassword = "";
		public string SignaturePassword { get { try { return Data.Decrypt(_signaturePassword); } catch { return ""; } } set { _signaturePassword = Data.Encrypt(value); } }
		
		/// <summary>
		/// ID of the linked account for the timeserver
		/// </summary>
		public string TimeServerAccountId { get; set; } = "";
		
		
		public void ReadValues(Data data, string path = "")
		{
			AllowMultiSigning = bool.TryParse(data.GetValue(@"" + path + @"AllowMultiSigning"), out var tmpAllowMultiSigning) ? tmpAllowMultiSigning : false;
			try { BackgroundImageFile = Data.UnescapeString(data.GetValue(@"" + path + @"BackgroundImageFile")); } catch { BackgroundImageFile = "";}
			try { CertificateFile = Data.UnescapeString(data.GetValue(@"" + path + @"CertificateFile")); } catch { CertificateFile = "";}
			DisplaySignature = Enum.TryParse<DisplaySignature>(data.GetValue(@"" + path + @"DisplaySignature"), out var tmpDisplaySignature) ? tmpDisplaySignature : DisplaySignature.NoDisplay;
			Enabled = bool.TryParse(data.GetValue(@"" + path + @"Enabled"), out var tmpEnabled) ? tmpEnabled : false;
			FitTextToSignatureSize = bool.TryParse(data.GetValue(@"" + path + @"FitTextToSignatureSize"), out var tmpFitTextToSignatureSize) ? tmpFitTextToSignatureSize : true;
			try
			{
				string value = data.GetValue(@"" + path + @"FontColor").Trim();
				if (value.Length == 0) FontColor = ColorTranslator.FromHtml("#000000"); else FontColor = ColorTranslator.FromHtml(value);
			}
			catch { FontColor =  ColorTranslator.FromHtml("#000000");}
			try { FontFile = Data.UnescapeString(data.GetValue(@"" + path + @"FontFile")); } catch { FontFile = "arial.ttf";}
			try { FontName = Data.UnescapeString(data.GetValue(@"" + path + @"FontName")); } catch { FontName = "Arial";}
			FontSize = float.TryParse(data.GetValue(@"" + path + @"FontSize"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpFontSize) ? tmpFontSize : 12f;
			LeftX = float.TryParse(data.GetValue(@"" + path + @"LeftX"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpLeftX) ? tmpLeftX : 100f;
			LeftY = float.TryParse(data.GetValue(@"" + path + @"LeftY"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpLeftY) ? tmpLeftY : 100f;
			RightX = float.TryParse(data.GetValue(@"" + path + @"RightX"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpRightX) ? tmpRightX : 200f;
			RightY = float.TryParse(data.GetValue(@"" + path + @"RightY"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpRightY) ? tmpRightY : 200f;
			try { SignContact = Data.UnescapeString(data.GetValue(@"" + path + @"SignContact")); } catch { SignContact = "";}
			try { SignLocation = Data.UnescapeString(data.GetValue(@"" + path + @"SignLocation")); } catch { SignLocation = "";}
			try { SignReason = Data.UnescapeString(data.GetValue(@"" + path + @"SignReason")); } catch { SignReason = "";}
			SignatureCustomPage = int.TryParse(data.GetValue(@"" + path + @"SignatureCustomPage"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpSignatureCustomPage) ? tmpSignatureCustomPage : 1;
			SignaturePage = Enum.TryParse<SignaturePage>(data.GetValue(@"" + path + @"SignaturePage"), out var tmpSignaturePage) ? tmpSignaturePage : SignaturePage.FirstPage;
			_signaturePassword = data.GetValue(@"" + path + @"SignaturePassword");
			try { TimeServerAccountId = Data.UnescapeString(data.GetValue(@"" + path + @"TimeServerAccountId")); } catch { TimeServerAccountId = "";}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"AllowMultiSigning", AllowMultiSigning.ToString());
			data.SetValue(@"" + path + @"BackgroundImageFile", Data.EscapeString(BackgroundImageFile));
			data.SetValue(@"" + path + @"CertificateFile", Data.EscapeString(CertificateFile));
			data.SetValue(@"" + path + @"DisplaySignature", DisplaySignature.ToString());
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"FitTextToSignatureSize", FitTextToSignatureSize.ToString());
			data.SetValue(@"" + path + @"FontColor", ColorTranslator.ToHtml(FontColor));
			data.SetValue(@"" + path + @"FontFile", Data.EscapeString(FontFile));
			data.SetValue(@"" + path + @"FontName", Data.EscapeString(FontName));
			data.SetValue(@"" + path + @"FontSize", FontSize.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"LeftX", LeftX.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"LeftY", LeftY.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"RightX", RightX.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"RightY", RightY.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"SignContact", Data.EscapeString(SignContact));
			data.SetValue(@"" + path + @"SignLocation", Data.EscapeString(SignLocation));
			data.SetValue(@"" + path + @"SignReason", Data.EscapeString(SignReason));
			data.SetValue(@"" + path + @"SignatureCustomPage", SignatureCustomPage.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"SignaturePage", SignaturePage.ToString());
			data.SetValue(@"" + path + @"SignaturePassword", _signaturePassword);
			data.SetValue(@"" + path + @"TimeServerAccountId", Data.EscapeString(TimeServerAccountId));
		}
		
		public Signature Copy()
		{
			Signature copy = new Signature();
			
			copy.AllowMultiSigning = AllowMultiSigning;
			copy.BackgroundImageFile = BackgroundImageFile;
			copy.CertificateFile = CertificateFile;
			copy.DisplaySignature = DisplaySignature;
			copy.Enabled = Enabled;
			copy.FitTextToSignatureSize = FitTextToSignatureSize;
			copy.FontColor = FontColor;
			copy.FontFile = FontFile;
			copy.FontName = FontName;
			copy.FontSize = FontSize;
			copy.LeftX = LeftX;
			copy.LeftY = LeftY;
			copy.RightX = RightX;
			copy.RightY = RightY;
			copy.SignContact = SignContact;
			copy.SignLocation = SignLocation;
			copy.SignReason = SignReason;
			copy.SignatureCustomPage = SignatureCustomPage;
			copy.SignaturePage = SignaturePage;
			copy.SignaturePassword = SignaturePassword;
			copy.TimeServerAccountId = TimeServerAccountId;
			return copy;
		}
		
		public void ReplaceWith(Signature source)
		{
			if(AllowMultiSigning != source.AllowMultiSigning)
				AllowMultiSigning = source.AllowMultiSigning;
				
			if(BackgroundImageFile != source.BackgroundImageFile)
				BackgroundImageFile = source.BackgroundImageFile;
				
			if(CertificateFile != source.CertificateFile)
				CertificateFile = source.CertificateFile;
				
			if(DisplaySignature != source.DisplaySignature)
				DisplaySignature = source.DisplaySignature;
				
			if(Enabled != source.Enabled)
				Enabled = source.Enabled;
				
			if(FitTextToSignatureSize != source.FitTextToSignatureSize)
				FitTextToSignatureSize = source.FitTextToSignatureSize;
				
			if(FontColor != source.FontColor)
				FontColor = source.FontColor;
				
			if(FontFile != source.FontFile)
				FontFile = source.FontFile;
				
			if(FontName != source.FontName)
				FontName = source.FontName;
				
			if(FontSize != source.FontSize)
				FontSize = source.FontSize;
				
			if(LeftX != source.LeftX)
				LeftX = source.LeftX;
				
			if(LeftY != source.LeftY)
				LeftY = source.LeftY;
				
			if(RightX != source.RightX)
				RightX = source.RightX;
				
			if(RightY != source.RightY)
				RightY = source.RightY;
				
			if(SignContact != source.SignContact)
				SignContact = source.SignContact;
				
			if(SignLocation != source.SignLocation)
				SignLocation = source.SignLocation;
				
			if(SignReason != source.SignReason)
				SignReason = source.SignReason;
				
			if(SignatureCustomPage != source.SignatureCustomPage)
				SignatureCustomPage = source.SignatureCustomPage;
				
			if(SignaturePage != source.SignaturePage)
				SignaturePage = source.SignaturePage;
				
			SignaturePassword = source.SignaturePassword;
			if(TimeServerAccountId != source.TimeServerAccountId)
				TimeServerAccountId = source.TimeServerAccountId;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is Signature)) return false;
			Signature v = o as Signature;
			
			if (!Object.Equals(AllowMultiSigning, v.AllowMultiSigning)) return false;
			if (!Object.Equals(BackgroundImageFile, v.BackgroundImageFile)) return false;
			if (!Object.Equals(CertificateFile, v.CertificateFile)) return false;
			if (!Object.Equals(DisplaySignature, v.DisplaySignature)) return false;
			if (!Object.Equals(Enabled, v.Enabled)) return false;
			if (!Object.Equals(FitTextToSignatureSize, v.FitTextToSignatureSize)) return false;
			if (!Object.Equals(FontColor, v.FontColor)) return false;
			if (!Object.Equals(FontFile, v.FontFile)) return false;
			if (!Object.Equals(FontName, v.FontName)) return false;
			if (!Object.Equals(FontSize, v.FontSize)) return false;
			if (!Object.Equals(LeftX, v.LeftX)) return false;
			if (!Object.Equals(LeftY, v.LeftY)) return false;
			if (!Object.Equals(RightX, v.RightX)) return false;
			if (!Object.Equals(RightY, v.RightY)) return false;
			if (!Object.Equals(SignContact, v.SignContact)) return false;
			if (!Object.Equals(SignLocation, v.SignLocation)) return false;
			if (!Object.Equals(SignReason, v.SignReason)) return false;
			if (!Object.Equals(SignatureCustomPage, v.SignatureCustomPage)) return false;
			if (!Object.Equals(SignaturePage, v.SignaturePage)) return false;
			if (!Object.Equals(SignaturePassword, v.SignaturePassword)) return false;
			if (!Object.Equals(TimeServerAccountId, v.TimeServerAccountId)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
