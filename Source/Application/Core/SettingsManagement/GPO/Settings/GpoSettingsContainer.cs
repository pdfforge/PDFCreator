using pdfforge.DataStorage.Storage;
using pdfforge.DataStorage;
using System.Collections.Generic;
using System.Text;
using System;

// ! This file is generated automatically.
// ! Do not edit it outside the sections for custom code.
// ! These changes will be deleted during the next generation run

namespace pdfforge.PDFCreator.Core.SettingsManagement.GPO.Settings
{
	public partial class GpoSettingsContainer : ISettings {
		
		public GpoSettings GpoSettings { get; set; } = new GpoSettings();
		
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
			GpoSettings.ReadValues(data, path + @"GpoSettings\");
		}
		
		public void StoreValues(Data data, string path)
		{
			GpoSettings.StoreValues(data, path + @"GpoSettings\");
		}
		
		public Data StoreValues(string path = "")
		{
			var data = Data.CreateDataStorage();
			StoreValues(data, "");
			return data;
		}
		
		public GpoSettingsContainer Copy()
		{
			GpoSettingsContainer copy = new GpoSettingsContainer();
			
			copy.GpoSettings = GpoSettings.Copy();
			return copy;
		}
		
		public void ReplaceWith(GpoSettingsContainer source)
		{
			GpoSettings.ReplaceWith(source.GpoSettings);
		}
		
		public override bool Equals(object o)
		{
			if (!(o is GpoSettingsContainer)) return false;
			GpoSettingsContainer v = o as GpoSettingsContainer;
			
			if (!GpoSettings.Equals(v.GpoSettings)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
