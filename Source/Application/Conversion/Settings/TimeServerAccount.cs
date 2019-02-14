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
	public partial class TimeServerAccount : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		public string AccountId { get; set; } = "";
		
		/// <summary>
		/// Set to true, if the time server needs authentication
		/// </summary>
		public bool IsSecured { get; set; } = false;
		
		/// <summary>
		/// Password for the time server
		/// </summary>
		private string _password = "";
		public string Password { get { try { return Data.Decrypt(_password); } catch { return ""; } } set { _password = Data.Encrypt(value); } }
		
		/// <summary>
		/// URL of a time server that provides a signed timestamp
		/// </summary>
		public string Url { get; set; } = "https://freetsa.org/tsr";
		
		/// <summary>
		/// Login name for the time server
		/// </summary>
		public string UserName { get; set; } = "";
		
		
		public void ReadValues(Data data, string path) {
			try { AccountId = Data.UnescapeString(data.GetValue(@"" + path + @"AccountId")); } catch { AccountId = "";}
			IsSecured = bool.TryParse(data.GetValue(@"" + path + @"IsSecured"), out var tmpIsSecured) ? tmpIsSecured : false;
			_password = data.GetValue(@"" + path + @"Password");
			try { Url = Data.UnescapeString(data.GetValue(@"" + path + @"Url")); } catch { Url = "https://freetsa.org/tsr";}
			try { UserName = Data.UnescapeString(data.GetValue(@"" + path + @"UserName")); } catch { UserName = "";}
		}
		
		public void StoreValues(Data data, string path) {
			data.SetValue(@"" + path + @"AccountId", Data.EscapeString(AccountId));
			data.SetValue(@"" + path + @"IsSecured", IsSecured.ToString());
			data.SetValue(@"" + path + @"Password", _password);
			data.SetValue(@"" + path + @"Url", Data.EscapeString(Url));
			data.SetValue(@"" + path + @"UserName", Data.EscapeString(UserName));
		}
		public TimeServerAccount Copy()
		{
			TimeServerAccount copy = new TimeServerAccount();
			
			copy.AccountId = AccountId;
			copy.IsSecured = IsSecured;
			copy.Password = Password;
			copy.Url = Url;
			copy.UserName = UserName;
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is TimeServerAccount)) return false;
			TimeServerAccount v = o as TimeServerAccount;
			
			if (!AccountId.Equals(v.AccountId)) return false;
			if (!IsSecured.Equals(v.IsSecured)) return false;
			if (!Password.Equals(v.Password)) return false;
			if (!Url.Equals(v.Url)) return false;
			if (!UserName.Equals(v.UserName)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
