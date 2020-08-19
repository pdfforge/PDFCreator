using pdfforge.DataStorage.Storage;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using PropertyChanged;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Text;
using System;

// ! This file is generated automatically.
// ! Do not edit it outside the sections for custom code.
// ! These changes will be deleted during the next generation run

namespace pdfforge.PDFCreator.Conversion.Settings
{
	/// <summary>
	/// 
	/// TitleReplacements are used to automatically replace text in the print job name for the final document's title.
	/// i.e. Word prints are named "Document.docx - Microsoft Word", where the replacement can remove the ".docx - Microsoft Word" part.
	/// 
	/// </summary>
	public partial class TitleReplacement : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		public string Replace { get; set; } = "";
		
		public ReplacementType ReplacementType { get; set; } = ReplacementType.Replace;
		
		public string Search { get; set; } = "";
		
		
		public void ReadValues(Data data, string path) {
			try { Replace = Data.UnescapeString(data.GetValue(@"" + path + @"Replace")); } catch { Replace = "";}
			ReplacementType = Enum.TryParse<ReplacementType>(data.GetValue(@"" + path + @"ReplacementType"), out var tmpReplacementType) ? tmpReplacementType : ReplacementType.Replace;
			try { Search = Data.UnescapeString(data.GetValue(@"" + path + @"Search")); } catch { Search = "";}
		}
		
		public void StoreValues(Data data, string path) {
			data.SetValue(@"" + path + @"Replace", Data.EscapeString(Replace));
			data.SetValue(@"" + path + @"ReplacementType", ReplacementType.ToString());
			data.SetValue(@"" + path + @"Search", Data.EscapeString(Search));
		}
		public TitleReplacement Copy()
		{
			TitleReplacement copy = new TitleReplacement();
			
			copy.Replace = Replace;
			copy.ReplacementType = ReplacementType;
			copy.Search = Search;
			return copy;
		}
		
		public void ReplaceWith(TitleReplacement source)
		{
			if(Replace != source.Replace)
				Replace = source.Replace;
				
			if(ReplacementType != source.ReplacementType)
				ReplacementType = source.ReplacementType;
				
			if(Search != source.Search)
				Search = source.Search;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is TitleReplacement)) return false;
			TitleReplacement v = o as TitleReplacement;
			
			if (!Replace.Equals(v.Replace)) return false;
			if (!ReplacementType.Equals(v.ReplacementType)) return false;
			if (!Search.Equals(v.Search)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
