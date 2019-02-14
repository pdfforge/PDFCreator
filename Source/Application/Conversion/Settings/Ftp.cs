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
	/// Upload the converted documents with FTP
	/// </summary>
	[ImplementPropertyChanged]
	public partial class Ftp : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// ID of the linked account
		/// </summary>
		public string AccountId { get; set; } = "";
		
		/// <summary>
		/// Target directory on the server
		/// </summary>
		public string Directory { get; set; } = "";
		
		/// <summary>
		/// If true, this action will be executed
		/// </summary>
		public bool Enabled { get; set; } = false;
		
		/// <summary>
		/// If true, files with the same name will not be overwritten on the server. A counter will be appended instead (i.e. document_2.pdf)
		/// </summary>
		public bool EnsureUniqueFilenames { get; set; } = false;
		
		
		public void ReadValues(Data data, string path = "")
		{
			try { AccountId = Data.UnescapeString(data.GetValue(@"" + path + @"AccountId")); } catch { AccountId = "";}
			try { Directory = Data.UnescapeString(data.GetValue(@"" + path + @"Directory")); } catch { Directory = "";}
			Enabled = bool.TryParse(data.GetValue(@"" + path + @"Enabled"), out var tmpEnabled) ? tmpEnabled : false;
			EnsureUniqueFilenames = bool.TryParse(data.GetValue(@"" + path + @"EnsureUniqueFilenames"), out var tmpEnsureUniqueFilenames) ? tmpEnsureUniqueFilenames : false;
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"AccountId", Data.EscapeString(AccountId));
			data.SetValue(@"" + path + @"Directory", Data.EscapeString(Directory));
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"EnsureUniqueFilenames", EnsureUniqueFilenames.ToString());
		}
		
		public Ftp Copy()
		{
			Ftp copy = new Ftp();
			
			copy.AccountId = AccountId;
			copy.Directory = Directory;
			copy.Enabled = Enabled;
			copy.EnsureUniqueFilenames = EnsureUniqueFilenames;
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is Ftp)) return false;
			Ftp v = o as Ftp;
			
			if (!AccountId.Equals(v.AccountId)) return false;
			if (!Directory.Equals(v.Directory)) return false;
			if (!Enabled.Equals(v.Enabled)) return false;
			if (!EnsureUniqueFilenames.Equals(v.EnsureUniqueFilenames)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
