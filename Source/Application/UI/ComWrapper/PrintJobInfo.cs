using System;

namespace pdfforge.PDFCreator.UI.ComWrapper
{
	public class PrintJobInfo
	{
		private readonly dynamic _printJobInfo;
		
		internal PrintJobInfo(dynamic printJobInfo)
		{
			_printJobInfo = printJobInfo;
		}
		public string PrintJobName
		{
			 get { return _printJobInfo.PrintJobName; }
		}
		
		public string PrintJobAuthor
		{
			 get { return _printJobInfo.PrintJobAuthor; }
		}
		
		public string Subject
		{
			 get { return _printJobInfo.Subject; }
		}
		
		public string Keywords
		{
			 get { return _printJobInfo.Keywords; }
		}
		
	}
}
