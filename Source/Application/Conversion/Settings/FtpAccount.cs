using pdfforge.DataStorage.Storage;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
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
	public partial class FtpAccount : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		public string AccountId { get; set; } = "";
		
		/// <summary>
		/// Normal authentication or with key file.
		/// </summary>
		public AuthenticationType AuthenticationType { get; set; } = AuthenticationType.NormalAuthentication;
		
		/// <summary>
		/// The connection type used for the FTP action.
		/// </summary>
		public FtpConnectionType FtpConnectionType { get; set; } = FtpConnectionType.Ftp;
		
		/// <summary>
		/// True if key file requires a password.
		/// </summary>
		public bool KeyFileRequiresPass { get; set; } = false;
		
		/// <summary>
		/// Password that is used to authenticate at the server
		/// </summary>
		private string _password = "";
		public string Password { get { try { return Data.Decrypt(_password); } catch { return ""; } } set { _password = Data.Encrypt(value); } }
		
		/// <summary>
		/// The path private key file for the sftp connection.
		/// </summary>
		public string PrivateKeyFile { get; set; } = "";
		
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
			AuthenticationType = Enum.TryParse<AuthenticationType>(data.GetValue(@"" + path + @"AuthenticationType"), out var tmpAuthenticationType) ? tmpAuthenticationType : AuthenticationType.NormalAuthentication;
			FtpConnectionType = Enum.TryParse<FtpConnectionType>(data.GetValue(@"" + path + @"FtpConnectionType"), out var tmpFtpConnectionType) ? tmpFtpConnectionType : FtpConnectionType.Ftp;
			KeyFileRequiresPass = bool.TryParse(data.GetValue(@"" + path + @"KeyFileRequiresPass"), out var tmpKeyFileRequiresPass) ? tmpKeyFileRequiresPass : false;
			_password = data.GetValue(@"" + path + @"Password");
			try { PrivateKeyFile = Data.UnescapeString(data.GetValue(@"" + path + @"PrivateKeyFile")); } catch { PrivateKeyFile = "";}
			try { Server = Data.UnescapeString(data.GetValue(@"" + path + @"Server")); } catch { Server = "";}
			try { UserName = Data.UnescapeString(data.GetValue(@"" + path + @"UserName")); } catch { UserName = "";}
		}
		
		public void StoreValues(Data data, string path) {
			data.SetValue(@"" + path + @"AccountId", Data.EscapeString(AccountId));
			data.SetValue(@"" + path + @"AuthenticationType", AuthenticationType.ToString());
			data.SetValue(@"" + path + @"FtpConnectionType", FtpConnectionType.ToString());
			data.SetValue(@"" + path + @"KeyFileRequiresPass", KeyFileRequiresPass.ToString());
			data.SetValue(@"" + path + @"Password", _password);
			data.SetValue(@"" + path + @"PrivateKeyFile", Data.EscapeString(PrivateKeyFile));
			data.SetValue(@"" + path + @"Server", Data.EscapeString(Server));
			data.SetValue(@"" + path + @"UserName", Data.EscapeString(UserName));
		}
		public FtpAccount Copy()
		{
			FtpAccount copy = new FtpAccount();
			
			copy.AccountId = AccountId;
			copy.AuthenticationType = AuthenticationType;
			copy.FtpConnectionType = FtpConnectionType;
			copy.KeyFileRequiresPass = KeyFileRequiresPass;
			copy.Password = Password;
			copy.PrivateKeyFile = PrivateKeyFile;
			copy.Server = Server;
			copy.UserName = UserName;
			return copy;
		}
		
		public void ReplaceWith(FtpAccount source)
		{
			if(AccountId != source.AccountId)
				AccountId = source.AccountId;
				
			if(AuthenticationType != source.AuthenticationType)
				AuthenticationType = source.AuthenticationType;
				
			if(FtpConnectionType != source.FtpConnectionType)
				FtpConnectionType = source.FtpConnectionType;
				
			if(KeyFileRequiresPass != source.KeyFileRequiresPass)
				KeyFileRequiresPass = source.KeyFileRequiresPass;
				
			Password = source.Password;
			if(PrivateKeyFile != source.PrivateKeyFile)
				PrivateKeyFile = source.PrivateKeyFile;
				
			if(Server != source.Server)
				Server = source.Server;
				
			if(UserName != source.UserName)
				UserName = source.UserName;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is FtpAccount)) return false;
			FtpAccount v = o as FtpAccount;
			
			if (!AccountId.Equals(v.AccountId)) return false;
			if (!AuthenticationType.Equals(v.AuthenticationType)) return false;
			if (!FtpConnectionType.Equals(v.FtpConnectionType)) return false;
			if (!KeyFileRequiresPass.Equals(v.KeyFileRequiresPass)) return false;
			if (!Password.Equals(v.Password)) return false;
			if (!PrivateKeyFile.Equals(v.PrivateKeyFile)) return false;
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
