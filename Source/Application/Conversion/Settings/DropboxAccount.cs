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
	public partial class DropboxAccount : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		private string _accessToken = "";
		public string AccessToken { get { try { return Data.Decrypt(_accessToken); } catch { return ""; } } set { _accessToken = Data.Encrypt(value); } }
		
		public string AccountId { get; set; } = "";
		
		public string AccountInfo { get; set; } = "";
		
		
		public void ReadValues(Data data, string path) {
			_accessToken = data.GetValue(@"" + path + @"AccessToken");
			try { AccountId = Data.UnescapeString(data.GetValue(@"" + path + @"AccountId")); } catch { AccountId = "";}
			try { AccountInfo = Data.UnescapeString(data.GetValue(@"" + path + @"AccountInfo")); } catch { AccountInfo = "";}
		}
		
		public void StoreValues(Data data, string path) {
			data.SetValue(@"" + path + @"AccessToken", _accessToken);
			data.SetValue(@"" + path + @"AccountId", Data.EscapeString(AccountId));
			data.SetValue(@"" + path + @"AccountInfo", Data.EscapeString(AccountInfo));
		}
		public DropboxAccount Copy()
		{
			DropboxAccount copy = new DropboxAccount();
			
			copy.AccessToken = AccessToken;
			copy.AccountId = AccountId;
			copy.AccountInfo = AccountInfo;
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is DropboxAccount)) return false;
			DropboxAccount v = o as DropboxAccount;
			
			if (!AccessToken.Equals(v.AccessToken)) return false;
			if (!AccountId.Equals(v.AccountId)) return false;
			if (!AccountInfo.Equals(v.AccountInfo)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
