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
	/// AutoSave allows to create PDF files without user interaction
	/// </summary>
	public partial class AutoSave : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		public bool Enabled { get; set; } = false;
		
		/// <summary>
		/// Defines the behaviour if a file already exists e.g. overwrite, merge or ensure unique filename
		/// </summary>
		public AutoSaveExistingFileBehaviour ExistingFileBehaviour { get; set; } = AutoSaveExistingFileBehaviour.EnsureUniqueFilenames;
		
		
		public void ReadValues(Data data, string path = "")
		{
			Enabled = bool.TryParse(data.GetValue(@"" + path + @"Enabled"), out var tmpEnabled) ? tmpEnabled : false;
			ExistingFileBehaviour = Enum.TryParse<AutoSaveExistingFileBehaviour>(data.GetValue(@"" + path + @"ExistingFileBehaviour"), out var tmpExistingFileBehaviour) ? tmpExistingFileBehaviour : AutoSaveExistingFileBehaviour.EnsureUniqueFilenames;
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"ExistingFileBehaviour", ExistingFileBehaviour.ToString());
		}
		
		public AutoSave Copy()
		{
			AutoSave copy = new AutoSave();
			
			copy.Enabled = Enabled;
			copy.ExistingFileBehaviour = ExistingFileBehaviour;
			return copy;
		}
		
		public void ReplaceWith(AutoSave source)
		{
			if(Enabled != source.Enabled)
				Enabled = source.Enabled;
				
			if(ExistingFileBehaviour != source.ExistingFileBehaviour)
				ExistingFileBehaviour = source.ExistingFileBehaviour;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is AutoSave)) return false;
			AutoSave v = o as AutoSave;
			
			if (!Object.Equals(Enabled, v.Enabled)) return false;
			if (!Object.Equals(ExistingFileBehaviour, v.ExistingFileBehaviour)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
