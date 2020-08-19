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
	public partial class ParameterSettings : INotifyPropertyChanged, ISettings {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		public Parameters Parameters { get; set; } = new Parameters();
		
		public bool LoadData(IStorage storage)
		{
			try {
				var data = Data.CreateDataStorage();
				storage.ReadData(data);
				ReadValues(data);
				return true;
			} catch { return false; }
			
		}
		
		public bool SaveData(IStorage storage)
		{
			try {
				var data = StoreValues();
				storage.WriteData(data);
				return true;
			} catch { return false; }
			
		}
		
		public void ReadValues(Data data, string path = "")
		{
			Parameters.ReadValues(data, path + @"Parameters\");
		}
		
		public void StoreValues(Data data, string path)
		{
			Parameters.StoreValues(data, path + @"Parameters\");
		}
		
		public Data StoreValues(string path = "")
		{
			var data = Data.CreateDataStorage();
			StoreValues(data, "");
			return data;
		}
		
		public ParameterSettings Copy()
		{
			ParameterSettings copy = new ParameterSettings();
			
			copy.Parameters = Parameters.Copy();
			return copy;
		}
		
		public void ReplaceWith(ParameterSettings source)
		{
			Parameters.ReplaceWith(source.Parameters);
		}
		
		public override bool Equals(object o)
		{
			if (!(o is ParameterSettings)) return false;
			ParameterSettings v = o as ParameterSettings;
			
			if (!Parameters.Equals(v.Parameters)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
