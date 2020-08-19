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
	/// <summary>
	/// PDF Security options
	/// </summary>
	public partial class Security : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// Allow to user to print the document
		/// </summary>
		public bool AllowPrinting { get; set; } = true;
		
		/// <summary>
		/// Allow to user to use a screen reader
		/// </summary>
		public bool AllowScreenReader { get; set; } = true;
		
		/// <summary>
		/// Allow to user to copy content from the PDF
		/// </summary>
		public bool AllowToCopyContent { get; set; } = false;
		
		/// <summary>
		/// Allow to user to make changes to the assembly
		/// </summary>
		public bool AllowToEditAssembly { get; set; } = false;
		
		/// <summary>
		/// Allow to user to edit comments
		/// </summary>
		public bool AllowToEditComments { get; set; } = false;
		
		/// <summary>
		/// Allow to user to edit the document
		/// </summary>
		public bool AllowToEditTheDocument { get; set; } = false;
		
		/// <summary>
		/// Allow to user to fill in forms
		/// </summary>
		public bool AllowToFillForms { get; set; } = true;
		
		/// <summary>
		/// If true, the PDF file will be password protected
		/// </summary>
		public bool Enabled { get; set; } = false;
		
		/// <summary>
		/// Defines the encryption level.
		/// </summary>
		public EncryptionLevel EncryptionLevel { get; set; } = EncryptionLevel.Aes256Bit;
		
		/// <summary>
		/// Password that can be used to modify the document
		/// </summary>
		private string _ownerPassword = "";
		public string OwnerPassword { get { try { return Data.Decrypt(_ownerPassword); } catch { return ""; } } set { _ownerPassword = Data.Encrypt(value); } }
		
		/// <summary>
		/// If true, a password is required to open the document.
		/// </summary>
		public bool RequireUserPassword { get; set; } = false;
		
		/// <summary>
		/// If true, only printing in low resolution will be supported
		/// </summary>
		public bool RestrictPrintingToLowQuality { get; set; } = false;
		
		/// <summary>
		/// Password that must be used to open the document (if set)
		/// </summary>
		private string _userPassword = "";
		public string UserPassword { get { try { return Data.Decrypt(_userPassword); } catch { return ""; } } set { _userPassword = Data.Encrypt(value); } }
		
		
		public void ReadValues(Data data, string path = "")
		{
			AllowPrinting = bool.TryParse(data.GetValue(@"" + path + @"AllowPrinting"), out var tmpAllowPrinting) ? tmpAllowPrinting : true;
			AllowScreenReader = bool.TryParse(data.GetValue(@"" + path + @"AllowScreenReader"), out var tmpAllowScreenReader) ? tmpAllowScreenReader : true;
			AllowToCopyContent = bool.TryParse(data.GetValue(@"" + path + @"AllowToCopyContent"), out var tmpAllowToCopyContent) ? tmpAllowToCopyContent : false;
			AllowToEditAssembly = bool.TryParse(data.GetValue(@"" + path + @"AllowToEditAssembly"), out var tmpAllowToEditAssembly) ? tmpAllowToEditAssembly : false;
			AllowToEditComments = bool.TryParse(data.GetValue(@"" + path + @"AllowToEditComments"), out var tmpAllowToEditComments) ? tmpAllowToEditComments : false;
			AllowToEditTheDocument = bool.TryParse(data.GetValue(@"" + path + @"AllowToEditTheDocument"), out var tmpAllowToEditTheDocument) ? tmpAllowToEditTheDocument : false;
			AllowToFillForms = bool.TryParse(data.GetValue(@"" + path + @"AllowToFillForms"), out var tmpAllowToFillForms) ? tmpAllowToFillForms : true;
			Enabled = bool.TryParse(data.GetValue(@"" + path + @"Enabled"), out var tmpEnabled) ? tmpEnabled : false;
			EncryptionLevel = Enum.TryParse<EncryptionLevel>(data.GetValue(@"" + path + @"EncryptionLevel"), out var tmpEncryptionLevel) ? tmpEncryptionLevel : EncryptionLevel.Aes256Bit;
			_ownerPassword = data.GetValue(@"" + path + @"OwnerPassword");
			RequireUserPassword = bool.TryParse(data.GetValue(@"" + path + @"RequireUserPassword"), out var tmpRequireUserPassword) ? tmpRequireUserPassword : false;
			RestrictPrintingToLowQuality = bool.TryParse(data.GetValue(@"" + path + @"RestrictPrintingToLowQuality"), out var tmpRestrictPrintingToLowQuality) ? tmpRestrictPrintingToLowQuality : false;
			_userPassword = data.GetValue(@"" + path + @"UserPassword");
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"AllowPrinting", AllowPrinting.ToString());
			data.SetValue(@"" + path + @"AllowScreenReader", AllowScreenReader.ToString());
			data.SetValue(@"" + path + @"AllowToCopyContent", AllowToCopyContent.ToString());
			data.SetValue(@"" + path + @"AllowToEditAssembly", AllowToEditAssembly.ToString());
			data.SetValue(@"" + path + @"AllowToEditComments", AllowToEditComments.ToString());
			data.SetValue(@"" + path + @"AllowToEditTheDocument", AllowToEditTheDocument.ToString());
			data.SetValue(@"" + path + @"AllowToFillForms", AllowToFillForms.ToString());
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"EncryptionLevel", EncryptionLevel.ToString());
			data.SetValue(@"" + path + @"OwnerPassword", _ownerPassword);
			data.SetValue(@"" + path + @"RequireUserPassword", RequireUserPassword.ToString());
			data.SetValue(@"" + path + @"RestrictPrintingToLowQuality", RestrictPrintingToLowQuality.ToString());
			data.SetValue(@"" + path + @"UserPassword", _userPassword);
		}
		
		public Security Copy()
		{
			Security copy = new Security();
			
			copy.AllowPrinting = AllowPrinting;
			copy.AllowScreenReader = AllowScreenReader;
			copy.AllowToCopyContent = AllowToCopyContent;
			copy.AllowToEditAssembly = AllowToEditAssembly;
			copy.AllowToEditComments = AllowToEditComments;
			copy.AllowToEditTheDocument = AllowToEditTheDocument;
			copy.AllowToFillForms = AllowToFillForms;
			copy.Enabled = Enabled;
			copy.EncryptionLevel = EncryptionLevel;
			copy.OwnerPassword = OwnerPassword;
			copy.RequireUserPassword = RequireUserPassword;
			copy.RestrictPrintingToLowQuality = RestrictPrintingToLowQuality;
			copy.UserPassword = UserPassword;
			return copy;
		}
		
		public void ReplaceWith(Security source)
		{
			if(AllowPrinting != source.AllowPrinting)
				AllowPrinting = source.AllowPrinting;
				
			if(AllowScreenReader != source.AllowScreenReader)
				AllowScreenReader = source.AllowScreenReader;
				
			if(AllowToCopyContent != source.AllowToCopyContent)
				AllowToCopyContent = source.AllowToCopyContent;
				
			if(AllowToEditAssembly != source.AllowToEditAssembly)
				AllowToEditAssembly = source.AllowToEditAssembly;
				
			if(AllowToEditComments != source.AllowToEditComments)
				AllowToEditComments = source.AllowToEditComments;
				
			if(AllowToEditTheDocument != source.AllowToEditTheDocument)
				AllowToEditTheDocument = source.AllowToEditTheDocument;
				
			if(AllowToFillForms != source.AllowToFillForms)
				AllowToFillForms = source.AllowToFillForms;
				
			if(Enabled != source.Enabled)
				Enabled = source.Enabled;
				
			if(EncryptionLevel != source.EncryptionLevel)
				EncryptionLevel = source.EncryptionLevel;
				
			OwnerPassword = source.OwnerPassword;
			if(RequireUserPassword != source.RequireUserPassword)
				RequireUserPassword = source.RequireUserPassword;
				
			if(RestrictPrintingToLowQuality != source.RestrictPrintingToLowQuality)
				RestrictPrintingToLowQuality = source.RestrictPrintingToLowQuality;
				
			UserPassword = source.UserPassword;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is Security)) return false;
			Security v = o as Security;
			
			if (!AllowPrinting.Equals(v.AllowPrinting)) return false;
			if (!AllowScreenReader.Equals(v.AllowScreenReader)) return false;
			if (!AllowToCopyContent.Equals(v.AllowToCopyContent)) return false;
			if (!AllowToEditAssembly.Equals(v.AllowToEditAssembly)) return false;
			if (!AllowToEditComments.Equals(v.AllowToEditComments)) return false;
			if (!AllowToEditTheDocument.Equals(v.AllowToEditTheDocument)) return false;
			if (!AllowToFillForms.Equals(v.AllowToFillForms)) return false;
			if (!Enabled.Equals(v.Enabled)) return false;
			if (!EncryptionLevel.Equals(v.EncryptionLevel)) return false;
			if (!OwnerPassword.Equals(v.OwnerPassword)) return false;
			if (!RequireUserPassword.Equals(v.RequireUserPassword)) return false;
			if (!RestrictPrintingToLowQuality.Equals(v.RestrictPrintingToLowQuality)) return false;
			if (!UserPassword.Equals(v.UserPassword)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
