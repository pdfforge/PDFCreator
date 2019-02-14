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
	/// <summary>
	/// Dropbox settings for currently logged user
	/// </summary>
	[ImplementPropertyChanged]
	public partial class DropboxSettings : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// ID of the linked account
		/// </summary>
		public string AccountId { get; set; } = "";
		
		public bool CreateShareLink { get; set; } = false;
		
		public bool Enabled { get; set; } = false;
		
		/// <summary>
		/// If true, files with the same name will not be overwritten on the server. A counter will be appended instead (i.e. document_2.pdf)
		/// </summary>
		public bool EnsureUniqueFilenames { get; set; } = false;
		
		public string SharedFolder { get; set; } = "PDFCreator";
		
		
		public void ReadValues(Data data, string path = "")
		{
			try { AccountId = Data.UnescapeString(data.GetValue(@"" + path + @"AccountId")); } catch { AccountId = "";}
			CreateShareLink = bool.TryParse(data.GetValue(@"" + path + @"CreateShareLink"), out var tmpCreateShareLink) ? tmpCreateShareLink : false;
			Enabled = bool.TryParse(data.GetValue(@"" + path + @"Enabled"), out var tmpEnabled) ? tmpEnabled : false;
			EnsureUniqueFilenames = bool.TryParse(data.GetValue(@"" + path + @"EnsureUniqueFilenames"), out var tmpEnsureUniqueFilenames) ? tmpEnsureUniqueFilenames : false;
			try { SharedFolder = Data.UnescapeString(data.GetValue(@"" + path + @"SharedFolder")); } catch { SharedFolder = "PDFCreator";}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"AccountId", Data.EscapeString(AccountId));
			data.SetValue(@"" + path + @"CreateShareLink", CreateShareLink.ToString());
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"EnsureUniqueFilenames", EnsureUniqueFilenames.ToString());
			data.SetValue(@"" + path + @"SharedFolder", Data.EscapeString(SharedFolder));
		}
		
		public DropboxSettings Copy()
		{
			DropboxSettings copy = new DropboxSettings();
			
			copy.AccountId = AccountId;
			copy.CreateShareLink = CreateShareLink;
			copy.Enabled = Enabled;
			copy.EnsureUniqueFilenames = EnsureUniqueFilenames;
			copy.SharedFolder = SharedFolder;
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is DropboxSettings)) return false;
			DropboxSettings v = o as DropboxSettings;
			
			if (!AccountId.Equals(v.AccountId)) return false;
			if (!CreateShareLink.Equals(v.CreateShareLink)) return false;
			if (!Enabled.Equals(v.Enabled)) return false;
			if (!EnsureUniqueFilenames.Equals(v.EnsureUniqueFilenames)) return false;
			if (!SharedFolder.Equals(v.SharedFolder)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
