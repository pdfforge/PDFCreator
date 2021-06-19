using pdfforge.DataStorage.Storage;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
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
	/// <summary>
	/// Sends a mail without user interaction through SMTP
	/// </summary>
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
		/// The list of additional attachments for the e-mail
		/// </summary>
		public List<string> AdditionalAttachments { get; set; } = new List<string>();
		
		/// <summary>
		/// Body text of the mail
		/// </summary>
		public string Content { get; set; } = "";
		
		/// <summary>
		/// Display name for e-mail sender
		/// </summary>
		public string DisplayName { get; set; } = "";
		
		/// <summary>
		/// If true, this action will be executed
		/// </summary>
		public bool Enabled { get; set; } = false;
		
		/// <summary>
		/// Set the e-mail body format
		/// </summary>
		public EmailFormatSetting Format { get; set; } = EmailFormatSetting.Text;
		
		/// <summary>
		/// If set it will be used as From and the address from the account will be set as Sender
		/// </summary>
		public string OnBehalfOf { get; set; } = "";
		
		/// <summary>
		/// The list of recipients of the e-mail, i.e. info@someone.com; me@mywebsite.org
		/// </summary>
		public string Recipients { get; set; } = "";
		
		/// <summary>
		/// The list of recipients of the e-mail in the 'BCC' field, i.e. info@someone.com; me@mywebsite.org
		/// </summary>
		public string RecipientsBcc { get; set; } = "";
		
		/// <summary>
		/// The list of recipients of the e-mail in the 'CC' field, i.e. info@someone.com; me@mywebsite.org
		/// </summary>
		public string RecipientsCc { get; set; } = "";
		
		/// <summary>
		/// Specifies an address that should be used to reply to the e-mail
		/// </summary>
		public string ReplyTo { get; set; } = "";
		
		/// <summary>
		/// Subject line of the e-mail
		/// </summary>
		public string Subject { get; set; } = "";
		
		
		public void ReadValues(Data data, string path = "")
		{
			try { AccountId = Data.UnescapeString(data.GetValue(@"" + path + @"AccountId")); } catch { AccountId = "";}
			AddSignature = bool.TryParse(data.GetValue(@"" + path + @"AddSignature"), out var tmpAddSignature) ? tmpAddSignature : true;
			try{
				int numClasses = int.Parse(data.GetValue(@"" + path + @"AdditionalAttachments\numClasses"));
				for (int i = 0; i < numClasses; i++){
					try{
						var value = Data.UnescapeString(data.GetValue(path + @"AdditionalAttachments\" + i + @"\AdditionalAttachments"));
						AdditionalAttachments.Add(value);
					}catch{}
				}
			}catch{}
			try { Content = Data.UnescapeString(data.GetValue(@"" + path + @"Content")); } catch { Content = "";}
			try { DisplayName = Data.UnescapeString(data.GetValue(@"" + path + @"DisplayName")); } catch { DisplayName = "";}
			Enabled = bool.TryParse(data.GetValue(@"" + path + @"Enabled"), out var tmpEnabled) ? tmpEnabled : false;
			Format = Enum.TryParse<EmailFormatSetting>(data.GetValue(@"" + path + @"Format"), out var tmpFormat) ? tmpFormat : EmailFormatSetting.Text;
			try { OnBehalfOf = Data.UnescapeString(data.GetValue(@"" + path + @"OnBehalfOf")); } catch { OnBehalfOf = "";}
			try { Recipients = Data.UnescapeString(data.GetValue(@"" + path + @"Recipients")); } catch { Recipients = "";}
			try { RecipientsBcc = Data.UnescapeString(data.GetValue(@"" + path + @"RecipientsBcc")); } catch { RecipientsBcc = "";}
			try { RecipientsCc = Data.UnescapeString(data.GetValue(@"" + path + @"RecipientsCc")); } catch { RecipientsCc = "";}
			try { ReplyTo = Data.UnescapeString(data.GetValue(@"" + path + @"ReplyTo")); } catch { ReplyTo = "";}
			try { Subject = Data.UnescapeString(data.GetValue(@"" + path + @"Subject")); } catch { Subject = "";}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"AccountId", Data.EscapeString(AccountId));
			data.SetValue(@"" + path + @"AddSignature", AddSignature.ToString());
			for (int i = 0; i < AdditionalAttachments.Count; i++){
				data.SetValue(path + @"AdditionalAttachments\" + i + @"\AdditionalAttachments", Data.EscapeString(AdditionalAttachments[i]));
			}
			data.SetValue(path + @"AdditionalAttachments\numClasses", AdditionalAttachments.Count.ToString());
			data.SetValue(@"" + path + @"Content", Data.EscapeString(Content));
			data.SetValue(@"" + path + @"DisplayName", Data.EscapeString(DisplayName));
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"Format", Format.ToString());
			data.SetValue(@"" + path + @"OnBehalfOf", Data.EscapeString(OnBehalfOf));
			data.SetValue(@"" + path + @"Recipients", Data.EscapeString(Recipients));
			data.SetValue(@"" + path + @"RecipientsBcc", Data.EscapeString(RecipientsBcc));
			data.SetValue(@"" + path + @"RecipientsCc", Data.EscapeString(RecipientsCc));
			data.SetValue(@"" + path + @"ReplyTo", Data.EscapeString(ReplyTo));
			data.SetValue(@"" + path + @"Subject", Data.EscapeString(Subject));
		}
		
		public EmailSmtpSettings Copy()
		{
			EmailSmtpSettings copy = new EmailSmtpSettings();
			
			copy.AccountId = AccountId;
			copy.AddSignature = AddSignature;
			copy.AdditionalAttachments = new List<string>(AdditionalAttachments);
			copy.Content = Content;
			copy.DisplayName = DisplayName;
			copy.Enabled = Enabled;
			copy.Format = Format;
			copy.OnBehalfOf = OnBehalfOf;
			copy.Recipients = Recipients;
			copy.RecipientsBcc = RecipientsBcc;
			copy.RecipientsCc = RecipientsCc;
			copy.ReplyTo = ReplyTo;
			copy.Subject = Subject;
			return copy;
		}
		
		public void ReplaceWith(EmailSmtpSettings source)
		{
			if(AccountId != source.AccountId)
				AccountId = source.AccountId;
				
			if(AddSignature != source.AddSignature)
				AddSignature = source.AddSignature;
				
			AdditionalAttachments.Clear();
			for (int i = 0; i < source.AdditionalAttachments.Count; i++)
			{
				AdditionalAttachments.Add(source.AdditionalAttachments[i]);
			}
			
			if(Content != source.Content)
				Content = source.Content;
				
			if(DisplayName != source.DisplayName)
				DisplayName = source.DisplayName;
				
			if(Enabled != source.Enabled)
				Enabled = source.Enabled;
				
			if(Format != source.Format)
				Format = source.Format;
				
			if(OnBehalfOf != source.OnBehalfOf)
				OnBehalfOf = source.OnBehalfOf;
				
			if(Recipients != source.Recipients)
				Recipients = source.Recipients;
				
			if(RecipientsBcc != source.RecipientsBcc)
				RecipientsBcc = source.RecipientsBcc;
				
			if(RecipientsCc != source.RecipientsCc)
				RecipientsCc = source.RecipientsCc;
				
			if(ReplyTo != source.ReplyTo)
				ReplyTo = source.ReplyTo;
				
			if(Subject != source.Subject)
				Subject = source.Subject;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is EmailSmtpSettings)) return false;
			EmailSmtpSettings v = o as EmailSmtpSettings;
			
			if (!AccountId.Equals(v.AccountId)) return false;
			if (!AddSignature.Equals(v.AddSignature)) return false;
			if (!AdditionalAttachments.SequenceEqual(v.AdditionalAttachments)) return false;
			if (!Content.Equals(v.Content)) return false;
			if (!DisplayName.Equals(v.DisplayName)) return false;
			if (!Enabled.Equals(v.Enabled)) return false;
			if (!Format.Equals(v.Format)) return false;
			if (!OnBehalfOf.Equals(v.OnBehalfOf)) return false;
			if (!Recipients.Equals(v.Recipients)) return false;
			if (!RecipientsBcc.Equals(v.RecipientsBcc)) return false;
			if (!RecipientsCc.Equals(v.RecipientsCc)) return false;
			if (!ReplyTo.Equals(v.ReplyTo)) return false;
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
