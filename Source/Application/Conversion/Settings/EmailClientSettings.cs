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
	/// <summary>
	/// Opens the default e-mail client with the converted document as attachment
	/// </summary>
	public partial class EmailClientSettings : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// Add the PDFCreator signature to the mail
		/// </summary>
		public bool AddSignature { get; set; } = true;
		
		/// <summary>
		/// The list of additional attachments for the e-mail
		/// </summary>
		public List<string> AdditionalAttachments { get; set; } = new List<string>();
		
		/// <summary>
		/// Body text of the e-mail
		/// </summary>
		public string Content { get; set; } = "";
		
		/// <summary>
		/// Enables the EmailClient action
		/// </summary>
		public bool Enabled { get; set; } = false;
		
		/// <summary>
		/// Use html for e-mail body
		/// </summary>
		public bool Html { get; set; } = false;
		
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
		/// Subject line of the e-mail
		/// </summary>
		public string Subject { get; set; } = "";
		
		
		public void ReadValues(Data data, string path = "")
		{
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
			Enabled = bool.TryParse(data.GetValue(@"" + path + @"Enabled"), out var tmpEnabled) ? tmpEnabled : false;
			Html = bool.TryParse(data.GetValue(@"" + path + @"Html"), out var tmpHtml) ? tmpHtml : false;
			try { Recipients = Data.UnescapeString(data.GetValue(@"" + path + @"Recipients")); } catch { Recipients = "";}
			try { RecipientsBcc = Data.UnescapeString(data.GetValue(@"" + path + @"RecipientsBcc")); } catch { RecipientsBcc = "";}
			try { RecipientsCc = Data.UnescapeString(data.GetValue(@"" + path + @"RecipientsCc")); } catch { RecipientsCc = "";}
			try { Subject = Data.UnescapeString(data.GetValue(@"" + path + @"Subject")); } catch { Subject = "";}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"AddSignature", AddSignature.ToString());
			for (int i = 0; i < AdditionalAttachments.Count; i++){
				data.SetValue(path + @"AdditionalAttachments\" + i + @"\AdditionalAttachments", Data.EscapeString(AdditionalAttachments[i]));
			}
			data.SetValue(path + @"AdditionalAttachments\numClasses", AdditionalAttachments.Count.ToString());
			data.SetValue(@"" + path + @"Content", Data.EscapeString(Content));
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"Html", Html.ToString());
			data.SetValue(@"" + path + @"Recipients", Data.EscapeString(Recipients));
			data.SetValue(@"" + path + @"RecipientsBcc", Data.EscapeString(RecipientsBcc));
			data.SetValue(@"" + path + @"RecipientsCc", Data.EscapeString(RecipientsCc));
			data.SetValue(@"" + path + @"Subject", Data.EscapeString(Subject));
		}
		
		public EmailClientSettings Copy()
		{
			EmailClientSettings copy = new EmailClientSettings();
			
			copy.AddSignature = AddSignature;
			copy.AdditionalAttachments = new List<string>(AdditionalAttachments);
			copy.Content = Content;
			copy.Enabled = Enabled;
			copy.Html = Html;
			copy.Recipients = Recipients;
			copy.RecipientsBcc = RecipientsBcc;
			copy.RecipientsCc = RecipientsCc;
			copy.Subject = Subject;
			return copy;
		}
		
		public void ReplaceWith(EmailClientSettings source)
		{
			if(AddSignature != source.AddSignature)
				AddSignature = source.AddSignature;
				
			AdditionalAttachments.Clear();
			for (int i = 0; i < source.AdditionalAttachments.Count; i++)
			{
				AdditionalAttachments.Add(source.AdditionalAttachments[i]);
			}
			
			if(Content != source.Content)
				Content = source.Content;
				
			if(Enabled != source.Enabled)
				Enabled = source.Enabled;
				
			if(Html != source.Html)
				Html = source.Html;
				
			if(Recipients != source.Recipients)
				Recipients = source.Recipients;
				
			if(RecipientsBcc != source.RecipientsBcc)
				RecipientsBcc = source.RecipientsBcc;
				
			if(RecipientsCc != source.RecipientsCc)
				RecipientsCc = source.RecipientsCc;
				
			if(Subject != source.Subject)
				Subject = source.Subject;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is EmailClientSettings)) return false;
			EmailClientSettings v = o as EmailClientSettings;
			
			if (!AddSignature.Equals(v.AddSignature)) return false;
			if (!AdditionalAttachments.SequenceEqual(v.AdditionalAttachments)) return false;
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
