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
	/// Parse ps files for user defined tokens
	/// </summary>
	public partial class UserTokens : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// Activate parsing ps files for user tokens (Only available in the PDFCreator business editions)
		/// </summary>
		public bool Enabled { get; set; } = false;
		
		/// <summary>
		/// UserToken separator in the document
		/// </summary>
		public UserTokenSeperator Seperator { get; set; } = UserTokenSeperator.SquareBrackets;
		
		
		public void ReadValues(Data data, string path = "")
		{
			Enabled = bool.TryParse(data.GetValue(@"" + path + @"Enabled"), out var tmpEnabled) ? tmpEnabled : false;
			Seperator = Enum.TryParse<UserTokenSeperator>(data.GetValue(@"" + path + @"Seperator"), out var tmpSeperator) ? tmpSeperator : UserTokenSeperator.SquareBrackets;
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"Seperator", Seperator.ToString());
		}
		
		public UserTokens Copy()
		{
			UserTokens copy = new UserTokens();
			
			copy.Enabled = Enabled;
			copy.Seperator = Seperator;
			return copy;
		}
		
		public void ReplaceWith(UserTokens source)
		{
			if(Enabled != source.Enabled)
				Enabled = source.Enabled;
				
			if(Seperator != source.Seperator)
				Seperator = source.Seperator;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is UserTokens)) return false;
			UserTokens v = o as UserTokens;
			
			if (!Enabled.Equals(v.Enabled)) return false;
			if (!Seperator.Equals(v.Seperator)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
