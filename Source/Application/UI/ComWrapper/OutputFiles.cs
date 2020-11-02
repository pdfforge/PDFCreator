using System;

namespace pdfforge.PDFCreator.UI.ComWrapper
{
	public class OutputFiles
	{
		private readonly dynamic _outputFiles;
		
		internal OutputFiles(dynamic outputFiles)
		{
			_outputFiles = outputFiles;
		}
		public int Count
		{
			 get { return _outputFiles.Count; }
		}
		
		public string GetFilename(int index)
		{
			return _outputFiles.GetFilename(index);
		}
		
	}
}
