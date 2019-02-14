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
	/// Settings for the PDF output format
	/// </summary>
	[ImplementPropertyChanged]
	public partial class PdfSettings : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// Compression settings for color and greyscale images
		/// </summary>
		public CompressColorAndGray CompressColorAndGray { get; set; } = new CompressColorAndGray();
		
		/// <summary>
		/// Compression settings for monochrome images
		/// </summary>
		public CompressMonochrome CompressMonochrome { get; set; } = new CompressMonochrome();
		
		/// <summary>
		/// PDF Security options
		/// </summary>
		public Security Security { get; set; } = new Security();
		
		/// <summary>
		/// Digitally sign the PDF document
		/// </summary>
		public Signature Signature { get; set; } = new Signature();
		
		/// <summary>
		/// Color model of the PDF (does not apply to images).
		/// </summary>
		public ColorModel ColorModel { get; set; } = ColorModel.Rgb;
		
		/// <summary>
		/// Defines which controls will be opened in the reader.
		/// </summary>
		public DocumentView DocumentView { get; set; } = DocumentView.NoOutLineNoThumbnailImages;
		
		/// <summary>
		/// Enable PDF/A validation
		/// </summary>
		public bool EnablePdfAValidation { get; set; } = false;
		
		/// <summary>
		/// If enabled, no fonts will be embedded into the output.
		/// </summary>
		public bool NoFonts { get; set; } = false;
		
		/// <summary>
		/// Define how pages are automatically rotated.
		/// </summary>
		public PageOrientation PageOrientation { get; set; } = PageOrientation.Automatic;
		
		/// <summary>
		/// Defines how the document will be opened in the reader.
		/// </summary>
		public PageView PageView { get; set; } = PageView.OnePage;
		
		/// <summary>
		/// Defines the page number the viewer will start on when opening the document
		/// </summary>
		public int ViewerStartsOnPage { get; set; } = 1;
		
		
		public void ReadValues(Data data, string path = "")
		{
			CompressColorAndGray.ReadValues(data, path + @"CompressColorAndGray\");
			CompressMonochrome.ReadValues(data, path + @"CompressMonochrome\");
			Security.ReadValues(data, path + @"Security\");
			Signature.ReadValues(data, path + @"Signature\");
			ColorModel = Enum.TryParse<ColorModel>(data.GetValue(@"" + path + @"ColorModel"), out var tmpColorModel) ? tmpColorModel : ColorModel.Rgb;
			DocumentView = Enum.TryParse<DocumentView>(data.GetValue(@"" + path + @"DocumentView"), out var tmpDocumentView) ? tmpDocumentView : DocumentView.NoOutLineNoThumbnailImages;
			EnablePdfAValidation = bool.TryParse(data.GetValue(@"" + path + @"EnablePdfAValidation"), out var tmpEnablePdfAValidation) ? tmpEnablePdfAValidation : false;
			NoFonts = bool.TryParse(data.GetValue(@"" + path + @"NoFonts"), out var tmpNoFonts) ? tmpNoFonts : false;
			PageOrientation = Enum.TryParse<PageOrientation>(data.GetValue(@"" + path + @"PageOrientation"), out var tmpPageOrientation) ? tmpPageOrientation : PageOrientation.Automatic;
			PageView = Enum.TryParse<PageView>(data.GetValue(@"" + path + @"PageView"), out var tmpPageView) ? tmpPageView : PageView.OnePage;
			ViewerStartsOnPage = int.TryParse(data.GetValue(@"" + path + @"ViewerStartsOnPage"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpViewerStartsOnPage) ? tmpViewerStartsOnPage : 1;
		}
		
		public void StoreValues(Data data, string path)
		{
			CompressColorAndGray.StoreValues(data, path + @"CompressColorAndGray\");
			CompressMonochrome.StoreValues(data, path + @"CompressMonochrome\");
			Security.StoreValues(data, path + @"Security\");
			Signature.StoreValues(data, path + @"Signature\");
			data.SetValue(@"" + path + @"ColorModel", ColorModel.ToString());
			data.SetValue(@"" + path + @"DocumentView", DocumentView.ToString());
			data.SetValue(@"" + path + @"EnablePdfAValidation", EnablePdfAValidation.ToString());
			data.SetValue(@"" + path + @"NoFonts", NoFonts.ToString());
			data.SetValue(@"" + path + @"PageOrientation", PageOrientation.ToString());
			data.SetValue(@"" + path + @"PageView", PageView.ToString());
			data.SetValue(@"" + path + @"ViewerStartsOnPage", ViewerStartsOnPage.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}
		
		public PdfSettings Copy()
		{
			PdfSettings copy = new PdfSettings();
			
			copy.CompressColorAndGray = CompressColorAndGray.Copy();
			copy.CompressMonochrome = CompressMonochrome.Copy();
			copy.Security = Security.Copy();
			copy.Signature = Signature.Copy();
			copy.ColorModel = ColorModel;
			copy.DocumentView = DocumentView;
			copy.EnablePdfAValidation = EnablePdfAValidation;
			copy.NoFonts = NoFonts;
			copy.PageOrientation = PageOrientation;
			copy.PageView = PageView;
			copy.ViewerStartsOnPage = ViewerStartsOnPage;
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is PdfSettings)) return false;
			PdfSettings v = o as PdfSettings;
			
			if (!CompressColorAndGray.Equals(v.CompressColorAndGray)) return false;
			if (!CompressMonochrome.Equals(v.CompressMonochrome)) return false;
			if (!Security.Equals(v.Security)) return false;
			if (!Signature.Equals(v.Signature)) return false;
			if (!ColorModel.Equals(v.ColorModel)) return false;
			if (!DocumentView.Equals(v.DocumentView)) return false;
			if (!EnablePdfAValidation.Equals(v.EnablePdfAValidation)) return false;
			if (!NoFonts.Equals(v.NoFonts)) return false;
			if (!PageOrientation.Equals(v.PageOrientation)) return false;
			if (!PageView.Equals(v.PageView)) return false;
			if (!ViewerStartsOnPage.Equals(v.ViewerStartsOnPage)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
