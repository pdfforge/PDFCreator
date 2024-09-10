using pdfforge.DataStorage.Storage;
using pdfforge.DataStorage;
using PropertyChanged;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System;

// ! This file is generated automatically.
// ! Do not edit it outside the sections for custom code.
// ! These changes will be deleted during the next generation run

namespace pdfforge.PDFCreator.Conversion.Settings
{
	public partial class MicrosoftAccount : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		private string _accessToken = "";
		public string AccessToken { get { try { return Data.Decrypt(_accessToken); } catch { return ""; } } set { _accessToken = Data.Encrypt(value); } }
		
		public string AccountId { get; set; } = "";
		
		public string AccountInfo { get; set; } = "";
		
		public long ExpirationDate { get; set; } = 0;
		
		/// <summary>
		/// Json Containing all info including refresh token
		/// </summary>
		private string _microsoftJson = "";
		public string MicrosoftJson { get { try { return Data.Decrypt(_microsoftJson); } catch { return ""; } } set { _microsoftJson = Data.Encrypt(value); } }
		
		public string PermissionScopes { get; set; } = "";
		
		
		public void ReadValues(Data data, string path) {
			_accessToken = data.GetValue(@"" + path + @"AccessToken");
			try { AccountId = Data.UnescapeString(data.GetValue(@"" + path + @"AccountId")); } catch { AccountId = "";}
			try { AccountInfo = Data.UnescapeString(data.GetValue(@"" + path + @"AccountInfo")); } catch { AccountInfo = "";}
			ExpirationDate = long.TryParse(data.GetValue(@"" + path + @"ExpirationDate"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpExpirationDate) ? tmpExpirationDate : 0;
			_microsoftJson = data.GetValue(@"" + path + @"MicrosoftJson");
			try { PermissionScopes = Data.UnescapeString(data.GetValue(@"" + path + @"PermissionScopes")); } catch { PermissionScopes = "";}
		}
		
		public void StoreValues(Data data, string path) {
			data.SetValue(@"" + path + @"AccessToken", _accessToken);
			data.SetValue(@"" + path + @"AccountId", Data.EscapeString(AccountId));
			data.SetValue(@"" + path + @"AccountInfo", Data.EscapeString(AccountInfo));
			data.SetValue(@"" + path + @"ExpirationDate", ExpirationDate.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"MicrosoftJson", _microsoftJson);
			data.SetValue(@"" + path + @"PermissionScopes", Data.EscapeString(PermissionScopes));
		}
		public MicrosoftAccount Copy()
		{
			MicrosoftAccount copy = new MicrosoftAccount();
			
			copy.AccessToken = AccessToken;
			copy.AccountId = AccountId;
			copy.AccountInfo = AccountInfo;
			copy.ExpirationDate = ExpirationDate;
			copy.MicrosoftJson = MicrosoftJson;
			copy.PermissionScopes = PermissionScopes;
			return copy;
		}
		
		public void ReplaceWith(MicrosoftAccount source)
		{
			AccessToken = source.AccessToken;
			if(AccountId != source.AccountId)
				AccountId = source.AccountId;
				
			if(AccountInfo != source.AccountInfo)
				AccountInfo = source.AccountInfo;
				
			if(ExpirationDate != source.ExpirationDate)
				ExpirationDate = source.ExpirationDate;
				
			MicrosoftJson = source.MicrosoftJson;
			if(PermissionScopes != source.PermissionScopes)
				PermissionScopes = source.PermissionScopes;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is MicrosoftAccount)) return false;
			MicrosoftAccount v = o as MicrosoftAccount;
			
			if (!Object.Equals(AccessToken, v.AccessToken)) return false;
			if (!Object.Equals(AccountId, v.AccountId)) return false;
			if (!Object.Equals(AccountInfo, v.AccountInfo)) return false;
			if (!Object.Equals(ExpirationDate, v.ExpirationDate)) return false;
			if (!Object.Equals(MicrosoftJson, v.MicrosoftJson)) return false;
			if (!Object.Equals(PermissionScopes, v.PermissionScopes)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
