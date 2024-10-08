using pdfforge.DataStorage.Storage;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
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
	public partial class HttpAccount : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		public string AccountId { get; set; } = "";
		
		/// <summary>
		/// If true, basic authentication with user-.name and password is used
		/// </summary>
		public bool IsBasicAuthentication { get; set; } = false;
		
		/// <summary>
		/// Password that is used to authenticate at the server
		/// </summary>
		private string _password = "";
		public string Password { get { try { return Data.Decrypt(_password); } catch { return ""; } } set { _password = Data.Encrypt(value); } }
		
		public HttpSendMode SendMode { get; set; } = HttpSendMode.HttpPost;
		
		/// <summary>
		/// Timeout in seconds for upload
		/// </summary>
		public int Timeout { get; set; } = 60;
		
		/// <summary>
		/// HTTP upload Url
		/// </summary>
		public string Url { get; set; } = "https://";
		
		/// <summary>
		/// User-name that is used to authenticate at the server
		/// </summary>
		public string UserName { get; set; } = "";
		
		
		public void ReadValues(Data data, string path) {
			try { AccountId = Data.UnescapeString(data.GetValue(@"" + path + @"AccountId")); } catch { AccountId = "";}
			IsBasicAuthentication = bool.TryParse(data.GetValue(@"" + path + @"IsBasicAuthentication"), out var tmpIsBasicAuthentication) ? tmpIsBasicAuthentication : false;
			_password = data.GetValue(@"" + path + @"Password");
			SendMode = Enum.TryParse<HttpSendMode>(data.GetValue(@"" + path + @"SendMode"), out var tmpSendMode) ? tmpSendMode : HttpSendMode.HttpPost;
			Timeout = int.TryParse(data.GetValue(@"" + path + @"Timeout"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpTimeout) ? tmpTimeout : 60;
			try { Url = Data.UnescapeString(data.GetValue(@"" + path + @"Url")); } catch { Url = "https://";}
			try { UserName = Data.UnescapeString(data.GetValue(@"" + path + @"UserName")); } catch { UserName = "";}
		}
		
		public void StoreValues(Data data, string path) {
			data.SetValue(@"" + path + @"AccountId", Data.EscapeString(AccountId));
			data.SetValue(@"" + path + @"IsBasicAuthentication", IsBasicAuthentication.ToString());
			data.SetValue(@"" + path + @"Password", _password);
			data.SetValue(@"" + path + @"SendMode", SendMode.ToString());
			data.SetValue(@"" + path + @"Timeout", Timeout.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"Url", Data.EscapeString(Url));
			data.SetValue(@"" + path + @"UserName", Data.EscapeString(UserName));
		}
		public HttpAccount Copy()
		{
			HttpAccount copy = new HttpAccount();
			
			copy.AccountId = AccountId;
			copy.IsBasicAuthentication = IsBasicAuthentication;
			copy.Password = Password;
			copy.SendMode = SendMode;
			copy.Timeout = Timeout;
			copy.Url = Url;
			copy.UserName = UserName;
			return copy;
		}
		
		public void ReplaceWith(HttpAccount source)
		{
			if(AccountId != source.AccountId)
				AccountId = source.AccountId;
				
			if(IsBasicAuthentication != source.IsBasicAuthentication)
				IsBasicAuthentication = source.IsBasicAuthentication;
				
			Password = source.Password;
			if(SendMode != source.SendMode)
				SendMode = source.SendMode;
				
			if(Timeout != source.Timeout)
				Timeout = source.Timeout;
				
			if(Url != source.Url)
				Url = source.Url;
				
			if(UserName != source.UserName)
				UserName = source.UserName;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is HttpAccount)) return false;
			HttpAccount v = o as HttpAccount;
			
			if (!Object.Equals(AccountId, v.AccountId)) return false;
			if (!Object.Equals(IsBasicAuthentication, v.IsBasicAuthentication)) return false;
			if (!Object.Equals(Password, v.Password)) return false;
			if (!Object.Equals(SendMode, v.SendMode)) return false;
			if (!Object.Equals(Timeout, v.Timeout)) return false;
			if (!Object.Equals(Url, v.Url)) return false;
			if (!Object.Equals(UserName, v.UserName)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
