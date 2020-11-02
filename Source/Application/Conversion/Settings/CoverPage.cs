using pdfforge.DataStorage.Storage;
using pdfforge.DataStorage;
using PropertyChanged;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System;

// ! This file is generated automatically.
// ! Do not edit it outside the sections for custom code.
// ! These changes will be deleted during the next generation run

namespace pdfforge.PDFCreator.Conversion.Settings
{
	/// <summary>
	/// Inserts one or more pages at the beginning of the converted document
	/// </summary>
	public partial class CoverPage : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// Enables the CoverPage action
		/// </summary>
		public bool Enabled { get; set; } = false;
		
		/// <summary>
		/// Filename of the PDF that will be inserted
		/// </summary>
		public List<string> Files { get; set; } = new List<string>();
		
		
		public void ReadValues(Data data, string path = "")
		{
			Enabled = bool.TryParse(data.GetValue(@"" + path + @"Enabled"), out var tmpEnabled) ? tmpEnabled : false;
			try{
				int numClasses = int.Parse(data.GetValue(@"" + path + @"Files\numClasses"));
				for (int i = 0; i < numClasses; i++){
					try{
						var value = Data.UnescapeString(data.GetValue(path + @"Files\" + i + @"\Files"));
						Files.Add(value);
					}catch{}
				}
			}catch{}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			for (int i = 0; i < Files.Count; i++){
				data.SetValue(path + @"Files\" + i + @"\Files", Data.EscapeString(Files[i]));
			}
			data.SetValue(path + @"Files\numClasses", Files.Count.ToString());
		}
		
		public CoverPage Copy()
		{
			CoverPage copy = new CoverPage();
			
			copy.Enabled = Enabled;
			copy.Files = new List<string>(Files);
			return copy;
		}
		
		public void ReplaceWith(CoverPage source)
		{
			if(Enabled != source.Enabled)
				Enabled = source.Enabled;
				
			Files.Clear();
			for (int i = 0; i < source.Files.Count; i++)
			{
				Files.Add(source.Files[i]);
			}
			
		}
		
		public override bool Equals(object o)
		{
			if (!(o is CoverPage)) return false;
			CoverPage v = o as CoverPage;
			
			if (!Enabled.Equals(v.Enabled)) return false;
			if (!Files.SequenceEqual(v.Files)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
