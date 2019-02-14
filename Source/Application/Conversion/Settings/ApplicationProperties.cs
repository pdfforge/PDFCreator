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
	[ImplementPropertyChanged]
	public partial class ApplicationProperties : INotifyPropertyChanged {
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
		
		
		public void ReadValues(Data data, string path = "")
		{
			try { AskSwitchDefaultPrinter = bool.Parse(data.GetValue(@"" + path + @"AskSwitchDefaultPrinter")); } catch { AskSwitchDefaultPrinter = true;}
			try { LastLoginVersion = Data.UnescapeString(data.GetValue(@"" + path + @"LastLoginVersion")); } catch { LastLoginVersion = "";}
			try { LastSaveDirectory = Data.UnescapeString(data.GetValue(@"" + path + @"LastSaveDirectory")); } catch { LastSaveDirectory = "";}
			try { LastUsedProfileGuid = Data.UnescapeString(data.GetValue(@"" + path + @"LastUsedProfileGuid")); } catch { LastUsedProfileGuid = "DefaultGuid";}
			try { PrimaryPrinter = Data.UnescapeString(data.GetValue(@"" + path + @"PrimaryPrinter")); } catch { PrimaryPrinter = "PDFCreator";}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"AskSwitchDefaultPrinter", AskSwitchDefaultPrinter.ToString());
			data.SetValue(@"" + path + @"LastLoginVersion", Data.EscapeString(LastLoginVersion));
			data.SetValue(@"" + path + @"LastSaveDirectory", Data.EscapeString(LastSaveDirectory));
			data.SetValue(@"" + path + @"LastUsedProfileGuid", Data.EscapeString(LastUsedProfileGuid));
			data.SetValue(@"" + path + @"PrimaryPrinter", Data.EscapeString(PrimaryPrinter));
		}
		
		public ApplicationProperties Copy()
		{
			ApplicationProperties copy = new ApplicationProperties();
			
			copy.AskSwitchDefaultPrinter = AskSwitchDefaultPrinter;
			copy.LastLoginVersion = LastLoginVersion;
			copy.LastSaveDirectory = LastSaveDirectory;
			copy.LastUsedProfileGuid = LastUsedProfileGuid;
			copy.PrimaryPrinter = PrimaryPrinter;
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is ApplicationProperties)) return false;
			ApplicationProperties v = o as ApplicationProperties;
			
			if (!AskSwitchDefaultPrinter.Equals(v.AskSwitchDefaultPrinter)) return false;
			if (!LastLoginVersion.Equals(v.LastLoginVersion)) return false;
			if (!LastSaveDirectory.Equals(v.LastSaveDirectory)) return false;
			if (!LastUsedProfileGuid.Equals(v.LastUsedProfileGuid)) return false;
			if (!PrimaryPrinter.Equals(v.PrimaryPrinter)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
