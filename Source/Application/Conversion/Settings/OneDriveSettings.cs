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
	/// OneDrive settings for the current user
	/// </summary>
	public partial class OneDriveSettings : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// ID of the linked account
		/// </summary>
		public string AccountId { get; set; } = "";
		
		/// <summary>
		/// If true, creates a link that can be used to share the file uploaded to OneDrive
		/// </summary>
		public bool CreateShareLink { get; set; } = false;
		
		public bool Enabled { get; set; } = false;
		
		/// <summary>
		/// If true, files with the same name will not be overwritten on the server. A counter will be appended instead (i.e. document_2.pdf)
		/// </summary>
		public bool EnsureUniqueFilenames { get; set; } = false;
		
		/// <summary>
		/// If true, opens the file after the upload to OneDrive
		/// </summary>
		public bool OpenUploadedFile { get; set; } = false;
		
		public string SharedFolder { get; set; } = "PDFCreator";
		
		public bool ShowShareLink { get; set; } = false;
		
		
		public void ReadValues(Data data, string path = "")
		{
			try { AccountId = Data.UnescapeString(data.GetValue(@"" + path + @"AccountId")); } catch { AccountId = "";}
			CreateShareLink = bool.TryParse(data.GetValue(@"" + path + @"CreateShareLink"), out var tmpCreateShareLink) ? tmpCreateShareLink : false;
			Enabled = bool.TryParse(data.GetValue(@"" + path + @"Enabled"), out var tmpEnabled) ? tmpEnabled : false;
			EnsureUniqueFilenames = bool.TryParse(data.GetValue(@"" + path + @"EnsureUniqueFilenames"), out var tmpEnsureUniqueFilenames) ? tmpEnsureUniqueFilenames : false;
			OpenUploadedFile = bool.TryParse(data.GetValue(@"" + path + @"OpenUploadedFile"), out var tmpOpenUploadedFile) ? tmpOpenUploadedFile : false;
			try { SharedFolder = Data.UnescapeString(data.GetValue(@"" + path + @"SharedFolder")); } catch { SharedFolder = "PDFCreator";}
			ShowShareLink = bool.TryParse(data.GetValue(@"" + path + @"ShowShareLink"), out var tmpShowShareLink) ? tmpShowShareLink : false;
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"AccountId", Data.EscapeString(AccountId));
			data.SetValue(@"" + path + @"CreateShareLink", CreateShareLink.ToString());
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"EnsureUniqueFilenames", EnsureUniqueFilenames.ToString());
			data.SetValue(@"" + path + @"OpenUploadedFile", OpenUploadedFile.ToString());
			data.SetValue(@"" + path + @"SharedFolder", Data.EscapeString(SharedFolder));
			data.SetValue(@"" + path + @"ShowShareLink", ShowShareLink.ToString());
		}
		
		public OneDriveSettings Copy()
		{
			OneDriveSettings copy = new OneDriveSettings();
			
			copy.AccountId = AccountId;
			copy.CreateShareLink = CreateShareLink;
			copy.Enabled = Enabled;
			copy.EnsureUniqueFilenames = EnsureUniqueFilenames;
			copy.OpenUploadedFile = OpenUploadedFile;
			copy.SharedFolder = SharedFolder;
			copy.ShowShareLink = ShowShareLink;
			return copy;
		}
		
		public void ReplaceWith(OneDriveSettings source)
		{
			if(AccountId != source.AccountId)
				AccountId = source.AccountId;
				
			if(CreateShareLink != source.CreateShareLink)
				CreateShareLink = source.CreateShareLink;
				
			if(Enabled != source.Enabled)
				Enabled = source.Enabled;
				
			if(EnsureUniqueFilenames != source.EnsureUniqueFilenames)
				EnsureUniqueFilenames = source.EnsureUniqueFilenames;
				
			if(OpenUploadedFile != source.OpenUploadedFile)
				OpenUploadedFile = source.OpenUploadedFile;
				
			if(SharedFolder != source.SharedFolder)
				SharedFolder = source.SharedFolder;
				
			if(ShowShareLink != source.ShowShareLink)
				ShowShareLink = source.ShowShareLink;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is OneDriveSettings)) return false;
			OneDriveSettings v = o as OneDriveSettings;
			
			if (!Object.Equals(AccountId, v.AccountId)) return false;
			if (!Object.Equals(CreateShareLink, v.CreateShareLink)) return false;
			if (!Object.Equals(Enabled, v.Enabled)) return false;
			if (!Object.Equals(EnsureUniqueFilenames, v.EnsureUniqueFilenames)) return false;
			if (!Object.Equals(OpenUploadedFile, v.OpenUploadedFile)) return false;
			if (!Object.Equals(SharedFolder, v.SharedFolder)) return false;
			if (!Object.Equals(ShowShareLink, v.ShowShareLink)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
