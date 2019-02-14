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
	/// Print the document to a physical printer
	/// </summary>
	[ImplementPropertyChanged]
	public partial class Printing : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// Defines the duplex printing mode.
		/// </summary>
		public DuplexPrint Duplex { get; set; } = DuplexPrint.Disable;
		
		/// <summary>
		/// If enabled, the document will be printed to a physical printer
		/// </summary>
		public bool Enabled { get; set; } = false;
		
		/// <summary>
		/// Name of the printer that will be used, if SelectedPrinter is set.
		/// </summary>
		public string PrinterName { get; set; } = "PDFCreator";
		
		/// <summary>
		/// Method to select the printer.
		/// </summary>
		public SelectPrinter SelectPrinter { get; set; } = SelectPrinter.ShowDialog;
		
		
		public void ReadValues(Data data, string path = "")
		{
			Duplex = Enum.TryParse<DuplexPrint>(data.GetValue(@"" + path + @"Duplex"), out var tmpDuplex) ? tmpDuplex : DuplexPrint.Disable;
			Enabled = bool.TryParse(data.GetValue(@"" + path + @"Enabled"), out var tmpEnabled) ? tmpEnabled : false;
			try { PrinterName = Data.UnescapeString(data.GetValue(@"" + path + @"PrinterName")); } catch { PrinterName = "PDFCreator";}
			SelectPrinter = Enum.TryParse<SelectPrinter>(data.GetValue(@"" + path + @"SelectPrinter"), out var tmpSelectPrinter) ? tmpSelectPrinter : SelectPrinter.ShowDialog;
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Duplex", Duplex.ToString());
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"PrinterName", Data.EscapeString(PrinterName));
			data.SetValue(@"" + path + @"SelectPrinter", SelectPrinter.ToString());
		}
		
		public Printing Copy()
		{
			Printing copy = new Printing();
			
			copy.Duplex = Duplex;
			copy.Enabled = Enabled;
			copy.PrinterName = PrinterName;
			copy.SelectPrinter = SelectPrinter;
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is Printing)) return false;
			Printing v = o as Printing;
			
			if (!Duplex.Equals(v.Duplex)) return false;
			if (!Enabled.Equals(v.Enabled)) return false;
			if (!PrinterName.Equals(v.PrinterName)) return false;
			if (!SelectPrinter.Equals(v.SelectPrinter)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
