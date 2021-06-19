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
	public partial class DropboxAccount : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		private string _accessToken = "";
		public string AccessToken { get { try { return Data.Decrypt(_accessToken); } catch { return ""; } } set { _accessToken = Data.Encrypt(value); } }
		
		public string AccountId { get; set; } = "";
		
		public string AccountInfo { get; set; } = "";
		
		/// <summary>
		/// Token to refresh the login token
		/// </summary>
		private string _refreshToken = "";
		public string RefreshToken { get { try { return Data.Decrypt(_refreshToken); } catch { return ""; } } set { _refreshToken = Data.Encrypt(value); } }
		
		
		public void ReadValues(Data data, string path) {
			_accessToken = data.GetValue(@"" + path + @"AccessToken");
			try { AccountId = Data.UnescapeString(data.GetValue(@"" + path + @"AccountId")); } catch { AccountId = "";}
			try { AccountInfo = Data.UnescapeString(data.GetValue(@"" + path + @"AccountInfo")); } catch { AccountInfo = "";}
			_refreshToken = data.GetValue(@"" + path + @"RefreshToken");
		}
		
		public void StoreValues(Data data, string path) {
			data.SetValue(@"" + path + @"AccessToken", _accessToken);
			data.SetValue(@"" + path + @"AccountId", Data.EscapeString(AccountId));
			data.SetValue(@"" + path + @"AccountInfo", Data.EscapeString(AccountInfo));
			data.SetValue(@"" + path + @"RefreshToken", _refreshToken);
		}
		public DropboxAccount Copy()
		{
			DropboxAccount copy = new DropboxAccount();
			
			copy.AccessToken = AccessToken;
			copy.AccountId = AccountId;
			copy.AccountInfo = AccountInfo;
			copy.RefreshToken = RefreshToken;
			return copy;
		}
		
		public void ReplaceWith(DropboxAccount source)
		{
			AccessToken = source.AccessToken;
			if(AccountId != source.AccountId)
				AccountId = source.AccountId;
				
			if(AccountInfo != source.AccountInfo)
				AccountInfo = source.AccountInfo;
				
			RefreshToken = source.RefreshToken;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is DropboxAccount)) return false;
			DropboxAccount v = o as DropboxAccount;
			
			if (!AccessToken.Equals(v.AccessToken)) return false;
			if (!AccountId.Equals(v.AccountId)) return false;
			if (!AccountInfo.Equals(v.AccountInfo)) return false;
			if (!RefreshToken.Equals(v.RefreshToken)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
