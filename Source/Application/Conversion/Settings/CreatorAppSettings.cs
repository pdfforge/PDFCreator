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
	public partial class CreatorAppSettings : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		public bool AskSwitchDefaultPrinter { get; set; } = true;
		
		public string LastLoginVersion { get; set; } = "";
		
		/// <summary>
		/// The last directory the user during interactive job (if no target directory was set in profile)
		/// </summary>
		public string LastSaveDirectory { get; set; } = "";
		
		public string LastUsedProfileGuid { get; set; } = "DefaultGuid";
		
		public string PrimaryPrinter { get; set; } = "PDFCreator";
		
		/// <summary>
		/// Version of the settings classes. This is used for internal purposes, i.e. to match properties when they were renamed
		/// </summary>
		public int SettingsVersion { get; set; } = 9;
		
		
		public void ReadValues(Data data, string path = "")
		{
			AskSwitchDefaultPrinter = bool.TryParse(data.GetValue(@"" + path + @"AskSwitchDefaultPrinter"), out var tmpAskSwitchDefaultPrinter) ? tmpAskSwitchDefaultPrinter : true;
			try { LastLoginVersion = Data.UnescapeString(data.GetValue(@"" + path + @"LastLoginVersion")); } catch { LastLoginVersion = "";}
			try { LastSaveDirectory = Data.UnescapeString(data.GetValue(@"" + path + @"LastSaveDirectory")); } catch { LastSaveDirectory = "";}
			try { LastUsedProfileGuid = Data.UnescapeString(data.GetValue(@"" + path + @"LastUsedProfileGuid")); } catch { LastUsedProfileGuid = "DefaultGuid";}
			try { PrimaryPrinter = Data.UnescapeString(data.GetValue(@"" + path + @"PrimaryPrinter")); } catch { PrimaryPrinter = "PDFCreator";}
			SettingsVersion = int.TryParse(data.GetValue(@"" + path + @"SettingsVersion"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpSettingsVersion) ? tmpSettingsVersion : 9;
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"AskSwitchDefaultPrinter", AskSwitchDefaultPrinter.ToString());
			data.SetValue(@"" + path + @"LastLoginVersion", Data.EscapeString(LastLoginVersion));
			data.SetValue(@"" + path + @"LastSaveDirectory", Data.EscapeString(LastSaveDirectory));
			data.SetValue(@"" + path + @"LastUsedProfileGuid", Data.EscapeString(LastUsedProfileGuid));
			data.SetValue(@"" + path + @"PrimaryPrinter", Data.EscapeString(PrimaryPrinter));
			data.SetValue(@"" + path + @"SettingsVersion", SettingsVersion.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}
		
		public CreatorAppSettings Copy()
		{
			CreatorAppSettings copy = new CreatorAppSettings();
			
			copy.AskSwitchDefaultPrinter = AskSwitchDefaultPrinter;
			copy.LastLoginVersion = LastLoginVersion;
			copy.LastSaveDirectory = LastSaveDirectory;
			copy.LastUsedProfileGuid = LastUsedProfileGuid;
			copy.PrimaryPrinter = PrimaryPrinter;
			copy.SettingsVersion = SettingsVersion;
			return copy;
		}
		
		public void ReplaceWith(CreatorAppSettings source)
		{
			if(AskSwitchDefaultPrinter != source.AskSwitchDefaultPrinter)
				AskSwitchDefaultPrinter = source.AskSwitchDefaultPrinter;
				
			if(LastLoginVersion != source.LastLoginVersion)
				LastLoginVersion = source.LastLoginVersion;
				
			if(LastSaveDirectory != source.LastSaveDirectory)
				LastSaveDirectory = source.LastSaveDirectory;
				
			if(LastUsedProfileGuid != source.LastUsedProfileGuid)
				LastUsedProfileGuid = source.LastUsedProfileGuid;
				
			if(PrimaryPrinter != source.PrimaryPrinter)
				PrimaryPrinter = source.PrimaryPrinter;
				
			if(SettingsVersion != source.SettingsVersion)
				SettingsVersion = source.SettingsVersion;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is CreatorAppSettings)) return false;
			CreatorAppSettings v = o as CreatorAppSettings;
			
			if (!AskSwitchDefaultPrinter.Equals(v.AskSwitchDefaultPrinter)) return false;
			if (!LastLoginVersion.Equals(v.LastLoginVersion)) return false;
			if (!LastSaveDirectory.Equals(v.LastSaveDirectory)) return false;
			if (!LastUsedProfileGuid.Equals(v.LastUsedProfileGuid)) return false;
			if (!PrimaryPrinter.Equals(v.PrimaryPrinter)) return false;
			if (!SettingsVersion.Equals(v.SettingsVersion)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
