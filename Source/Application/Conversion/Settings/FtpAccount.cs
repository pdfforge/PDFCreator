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
	public partial class FtpAccount : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		public string AccountId { get; set; } = "";
		
		/// <summary>
		/// Password that is used to authenticate at the server
		/// </summary>
		private string _password = "";
		public string Password { get { try { return Data.Decrypt(_password); } catch { return ""; } } set { _password = Data.Encrypt(value); } }
		
		/// <summary>
		/// Hostname or IP address of the FTP server
		/// </summary>
		public string Server { get; set; } = "";
		
		/// <summary>
		/// User name that is used to authenticate at the server
		/// </summary>
		public string UserName { get; set; } = "";
		
		
		public void ReadValues(Data data, string path) {
			try { AccountId = Data.UnescapeString(data.GetValue(@"" + path + @"AccountId")); } catch { AccountId = "";}
			_password = data.GetValue(@"" + path + @"Password");
			try { Server = Data.UnescapeString(data.GetValue(@"" + path + @"Server")); } catch { Server = "";}
			try { UserName = Data.UnescapeString(data.GetValue(@"" + path + @"UserName")); } catch { UserName = "";}
		}
		
		public void StoreValues(Data data, string path) {
			data.SetValue(@"" + path + @"AccountId", Data.EscapeString(AccountId));
			data.SetValue(@"" + path + @"Password", _password);
			data.SetValue(@"" + path + @"Server", Data.EscapeString(Server));
			data.SetValue(@"" + path + @"UserName", Data.EscapeString(UserName));
		}
		public FtpAccount Copy()
		{
			FtpAccount copy = new FtpAccount();
			
			copy.AccountId = AccountId;
			copy.Password = Password;
			copy.Server = Server;
			copy.UserName = UserName;
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is FtpAccount)) return false;
			FtpAccount v = o as FtpAccount;
			
			if (!AccountId.Equals(v.AccountId)) return false;
			if (!Password.Equals(v.Password)) return false;
			if (!Server.Equals(v.Server)) return false;
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
