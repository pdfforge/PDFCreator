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
	/// Opens the printed file in a viewer
	/// </summary>
	public partial class OpenViewer : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// If true, this action will be executed
		/// </summary>
		public bool Enabled { get; set; } = false;
		
		/// <summary>
		/// If the output is a PDF, use PDF Architect instead of the default PDF viewer
		/// </summary>
		public bool OpenWithPdfArchitect { get; set; } = true;
		
		
		public void ReadValues(Data data, string path = "")
		{
			Enabled = bool.TryParse(data.GetValue(@"" + path + @"Enabled"), out var tmpEnabled) ? tmpEnabled : false;
			OpenWithPdfArchitect = bool.TryParse(data.GetValue(@"" + path + @"OpenWithPdfArchitect"), out var tmpOpenWithPdfArchitect) ? tmpOpenWithPdfArchitect : true;
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"OpenWithPdfArchitect", OpenWithPdfArchitect.ToString());
		}
		
		public OpenViewer Copy()
		{
			OpenViewer copy = new OpenViewer();
			
			copy.Enabled = Enabled;
			copy.OpenWithPdfArchitect = OpenWithPdfArchitect;
			return copy;
		}
		
		public void ReplaceWith(OpenViewer source)
		{
			if(Enabled != source.Enabled)
				Enabled = source.Enabled;
				
			if(OpenWithPdfArchitect != source.OpenWithPdfArchitect)
				OpenWithPdfArchitect = source.OpenWithPdfArchitect;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is OpenViewer)) return false;
			OpenViewer v = o as OpenViewer;
			
			if (!Enabled.Equals(v.Enabled)) return false;
			if (!OpenWithPdfArchitect.Equals(v.OpenWithPdfArchitect)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
