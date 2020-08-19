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
	/// Properties of the profile
	/// </summary>
	public partial class Properties : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// Can users delete this profile?
		/// </summary>
		public bool Deletable { get; set; } = true;
		
		/// <summary>
		/// True for shared profiles
		/// </summary>
		public bool IsShared { get; set; } = false;
		
		/// <summary>
		/// Can users rename this profile?
		/// </summary>
		public bool Renamable { get; set; } = true;
		
		
		public void ReadValues(Data data, string path = "")
		{
			Deletable = bool.TryParse(data.GetValue(@"" + path + @"Deletable"), out var tmpDeletable) ? tmpDeletable : true;
			IsShared = bool.TryParse(data.GetValue(@"" + path + @"IsShared"), out var tmpIsShared) ? tmpIsShared : false;
			Renamable = bool.TryParse(data.GetValue(@"" + path + @"Renamable"), out var tmpRenamable) ? tmpRenamable : true;
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Deletable", Deletable.ToString());
			data.SetValue(@"" + path + @"IsShared", IsShared.ToString());
			data.SetValue(@"" + path + @"Renamable", Renamable.ToString());
		}
		
		public Properties Copy()
		{
			Properties copy = new Properties();
			
			copy.Deletable = Deletable;
			copy.IsShared = IsShared;
			copy.Renamable = Renamable;
			return copy;
		}
		
		public void ReplaceWith(Properties source)
		{
			if(Deletable != source.Deletable)
				Deletable = source.Deletable;
				
			if(IsShared != source.IsShared)
				IsShared = source.IsShared;
				
			if(Renamable != source.Renamable)
				Renamable = source.Renamable;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is Properties)) return false;
			Properties v = o as Properties;
			
			if (!Deletable.Equals(v.Deletable)) return false;
			if (!IsShared.Equals(v.IsShared)) return false;
			if (!Renamable.Equals(v.Renamable)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
