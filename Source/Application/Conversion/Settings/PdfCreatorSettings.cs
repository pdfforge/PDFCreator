using pdfforge.DataStorage.Storage;
using pdfforge.DataStorage;
using PropertyChanged;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	/// Container class for PDFCreator settings and profiles
	/// </summary>
	public partial class PdfCreatorSettings : INotifyPropertyChanged, ISettings {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// PDFCreator application settings
		/// </summary>
		public ApplicationSettings ApplicationSettings { get; set; } = new ApplicationSettings();
		
		public ObservableCollection<ConversionProfile> ConversionProfiles { get; set; } = new ObservableCollection<ConversionProfile>();
		public CreatorAppSettings CreatorAppSettings { get; set; } = new CreatorAppSettings();
		
		public ObservableCollection<DefaultViewer> DefaultViewers { get; set; } = new ObservableCollection<DefaultViewer>();
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
			ApplicationSettings.ReadValues(data, path + @"ApplicationSettings\");
			
			try
			{
				int numClasses = int.Parse(data.GetValue(@"" + path + @"ConversionProfiles\numClasses"));
				for (int i = 0; i < numClasses; i++)
				{
					ConversionProfile tmp = new ConversionProfile();
					tmp.ReadValues(data, @"" + path + @"ConversionProfiles\" + i + @"\");
					ConversionProfiles.Add(tmp);
				}
			} catch {}
			
			CreatorAppSettings.ReadValues(data, path + @"CreatorAppSettings\");
			
			try
			{
				int numClasses = int.Parse(data.GetValue(@"" + path + @"DefaultViewers\numClasses"));
				for (int i = 0; i < numClasses; i++)
				{
					DefaultViewer tmp = new DefaultViewer();
					tmp.ReadValues(data, @"" + path + @"DefaultViewers\" + i + @"\");
					DefaultViewers.Add(tmp);
				}
			} catch {}
			
		}
		
		public void StoreValues(Data data, string path)
		{
			ApplicationSettings.StoreValues(data, path + @"ApplicationSettings\");
			
			for (int i = 0; i < ConversionProfiles.Count; i++)
			{
				ConversionProfile tmp = ConversionProfiles[i];
				tmp.StoreValues(data, @"" + path + @"ConversionProfiles\" + i + @"\");
			}
			data.SetValue(@"" + path + @"ConversionProfiles\numClasses", ConversionProfiles.Count.ToString());
			
			CreatorAppSettings.StoreValues(data, path + @"CreatorAppSettings\");
			
			for (int i = 0; i < DefaultViewers.Count; i++)
			{
				DefaultViewer tmp = DefaultViewers[i];
				tmp.StoreValues(data, @"" + path + @"DefaultViewers\" + i + @"\");
			}
			data.SetValue(@"" + path + @"DefaultViewers\numClasses", DefaultViewers.Count.ToString());
			
		}
		
		public Data StoreValues(string path = "")
		{
			var data = Data.CreateDataStorage();
			StoreValues(data, "");
			return data;
		}
		
		public PdfCreatorSettings Copy()
		{
			PdfCreatorSettings copy = new PdfCreatorSettings();
			
			copy.ApplicationSettings = ApplicationSettings.Copy();
			
			copy.ConversionProfiles = new ObservableCollection<ConversionProfile>();
			for (int i = 0; i < ConversionProfiles.Count; i++)
			{
				copy.ConversionProfiles.Add(ConversionProfiles[i].Copy());
			}
			
			copy.CreatorAppSettings = CreatorAppSettings.Copy();
			
			copy.DefaultViewers = new ObservableCollection<DefaultViewer>();
			for (int i = 0; i < DefaultViewers.Count; i++)
			{
				copy.DefaultViewers.Add(DefaultViewers[i].Copy());
			}
			
			return copy;
		}
		
		public void ReplaceWith(PdfCreatorSettings source)
		{
			ApplicationSettings.ReplaceWith(source.ApplicationSettings);
			
			ConversionProfiles.Clear();
			for (int i = 0; i < source.ConversionProfiles.Count; i++)
			{
				ConversionProfiles.Add(source.ConversionProfiles[i].Copy());
			}
			
			CreatorAppSettings.ReplaceWith(source.CreatorAppSettings);
			
			DefaultViewers.Clear();
			for (int i = 0; i < source.DefaultViewers.Count; i++)
			{
				DefaultViewers.Add(source.DefaultViewers[i].Copy());
			}
			
		}
		
		public override bool Equals(object o)
		{
			if (!(o is PdfCreatorSettings)) return false;
			PdfCreatorSettings v = o as PdfCreatorSettings;
			
			if (!ApplicationSettings.Equals(v.ApplicationSettings)) return false;
			
			if (ConversionProfiles.Count != v.ConversionProfiles.Count) return false;
			for (int i = 0; i < ConversionProfiles.Count; i++)
			{
				if (!ConversionProfiles[i].Equals(v.ConversionProfiles[i])) return false;
			}
			
			if (!CreatorAppSettings.Equals(v.CreatorAppSettings)) return false;
			
			if (DefaultViewers.Count != v.DefaultViewers.Count) return false;
			for (int i = 0; i < DefaultViewers.Count; i++)
			{
				if (!DefaultViewers[i].Equals(v.DefaultViewers[i])) return false;
			}
			
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
