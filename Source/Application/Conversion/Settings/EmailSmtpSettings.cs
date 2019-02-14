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
	/// Sends a mail without user interaction through SMTP
	/// </summary>
	[ImplementPropertyChanged]
	public partial class EmailSmtpSettings : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// ID of linked account
		/// </summary>
		public string AccountId { get; set; } = "";
		
		/// <summary>
		/// Add the PDFCreator signature to the mail
		/// </summary>
		public bool AddSignature { get; set; } = true;
		
		/// <summary>
		/// Body text of the mail
		/// </summary>
		public string Content { get; set; } = "";
		
		/// <summary>
		/// If true, this action will be executed
		/// </summary>
		public bool Enabled { get; set; } = false;
		
		/// <summary>
		/// Use html for e-mail body
		/// </summary>
		public bool Html { get; set; } = false;
		
		/// <summary>
		/// The list of receipients of the e-mail, i.e. info@someone.com; me@mywebsite.org
		/// </summary>
		public string Recipients { get; set; } = "";
		
		/// <summary>
		/// The list of receipients of the e-mail in the 'BCC' field, i.e. info@someone.com; me@mywebsite.org
		/// </summary>
		public string RecipientsBcc { get; set; } = "";
		
		/// <summary>
		/// The list of receipients of the e-mail in the 'CC' field, i.e. info@someone.com; me@mywebsite.org
		/// </summary>
		public string RecipientsCc { get; set; } = "";
		
		/// <summary>
		/// Subject line of the e-mail
		/// </summary>
		public string Subject { get; set; } = "";
		
		
		public void ReadValues(Data data, string path = "")
		{
			try { AccountId = Data.UnescapeString(data.GetValue(@"" + path + @"AccountId")); } catch { AccountId = "";}
			AddSignature = bool.TryParse(data.GetValue(@"" + path + @"AddSignature"), out var tmpAddSignature) ? tmpAddSignature : true;
			try { Content = Data.UnescapeString(data.GetValue(@"" + path + @"Content")); } catch { Content = "";}
			Enabled = bool.TryParse(data.GetValue(@"" + path + @"Enabled"), out var tmpEnabled) ? tmpEnabled : false;
			Html = bool.TryParse(data.GetValue(@"" + path + @"Html"), out var tmpHtml) ? tmpHtml : false;
			try { Recipients = Data.UnescapeString(data.GetValue(@"" + path + @"Recipients")); } catch { Recipients = "";}
			try { RecipientsBcc = Data.UnescapeString(data.GetValue(@"" + path + @"RecipientsBcc")); } catch { RecipientsBcc = "";}
			try { RecipientsCc = Data.UnescapeString(data.GetValue(@"" + path + @"RecipientsCc")); } catch { RecipientsCc = "";}
			try { Subject = Data.UnescapeString(data.GetValue(@"" + path + @"Subject")); } catch { Subject = "";}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"AccountId", Data.EscapeString(AccountId));
			data.SetValue(@"" + path + @"AddSignature", AddSignature.ToString());
			data.SetValue(@"" + path + @"Content", Data.EscapeString(Content));
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"Html", Html.ToString());
			data.SetValue(@"" + path + @"Recipients", Data.EscapeString(Recipients));
			data.SetValue(@"" + path + @"RecipientsBcc", Data.EscapeString(RecipientsBcc));
			data.SetValue(@"" + path + @"RecipientsCc", Data.EscapeString(RecipientsCc));
			data.SetValue(@"" + path + @"Subject", Data.EscapeString(Subject));
		}
		
		public EmailSmtpSettings Copy()
		{
			EmailSmtpSettings copy = new EmailSmtpSettings();
			
			copy.AccountId = AccountId;
			copy.AddSignature = AddSignature;
			copy.Content = Content;
			copy.Enabled = Enabled;
			copy.Html = Html;
			copy.Recipients = Recipients;
			copy.RecipientsBcc = RecipientsBcc;
			copy.RecipientsCc = RecipientsCc;
			copy.Subject = Subject;
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is EmailSmtpSettings)) return false;
			EmailSmtpSettings v = o as EmailSmtpSettings;
			
			if (!AccountId.Equals(v.AccountId)) return false;
			if (!AddSignature.Equals(v.AddSignature)) return false;
			if (!Content.Equals(v.Content)) return false;
			if (!Enabled.Equals(v.Enabled)) return false;
			if (!Html.Equals(v.Html)) return false;
			if (!Recipients.Equals(v.Recipients)) return false;
			if (!RecipientsBcc.Equals(v.RecipientsBcc)) return false;
			if (!RecipientsCc.Equals(v.RecipientsCc)) return false;
			if (!Subject.Equals(v.Subject)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
