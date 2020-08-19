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
	public partial class DefaultViewer : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		public bool IsActive { get; set; } = false;
		
		public OutputFormat OutputFormat { get; set; } = OutputFormat.Pdf;
		
		public string Parameters { get; set; } = "";
		
		public string Path { get; set; } = "";
		
		
		public void ReadValues(Data data, string path) {
			IsActive = bool.TryParse(data.GetValue(@"" + path + @"IsActive"), out var tmpIsActive) ? tmpIsActive : false;
			OutputFormat = Enum.TryParse<OutputFormat>(data.GetValue(@"" + path + @"OutputFormat"), out var tmpOutputFormat) ? tmpOutputFormat : OutputFormat.Pdf;
			try { Parameters = Data.UnescapeString(data.GetValue(@"" + path + @"Parameters")); } catch { Parameters = "";}
			try { Path = Data.UnescapeString(data.GetValue(@"" + path + @"Path")); } catch { Path = "";}
		}
		
		public void StoreValues(Data data, string path) {
			data.SetValue(@"" + path + @"IsActive", IsActive.ToString());
			data.SetValue(@"" + path + @"OutputFormat", OutputFormat.ToString());
			data.SetValue(@"" + path + @"Parameters", Data.EscapeString(Parameters));
			data.SetValue(@"" + path + @"Path", Data.EscapeString(Path));
		}
		public DefaultViewer Copy()
		{
			DefaultViewer copy = new DefaultViewer();
			
			copy.IsActive = IsActive;
			copy.OutputFormat = OutputFormat;
			copy.Parameters = Parameters;
			copy.Path = Path;
			return copy;
		}
		
		public void ReplaceWith(DefaultViewer source)
		{
			if(IsActive != source.IsActive)
				IsActive = source.IsActive;
				
			if(OutputFormat != source.OutputFormat)
				OutputFormat = source.OutputFormat;
				
			if(Parameters != source.Parameters)
				Parameters = source.Parameters;
				
			if(Path != source.Path)
				Path = source.Path;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is DefaultViewer)) return false;
			DefaultViewer v = o as DefaultViewer;
			
			if (!IsActive.Equals(v.IsActive)) return false;
			if (!OutputFormat.Equals(v.OutputFormat)) return false;
			if (!Parameters.Equals(v.Parameters)) return false;
			if (!Path.Equals(v.Path)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
