using System;

namespace pdfforge.PDFCreator.UI.ComWrapper
{
	public class Printers
	{
		private readonly dynamic _printers;
		
		internal Printers(dynamic printers)
		{
			_printers = printers;
		}
		public int Count
		{
			 get { return _printers.Count; }
		}
		
		public string GetPrinterByIndex(int index)
		{
			return _printers.GetPrinterByIndex(index);
		}
		
	}
}
