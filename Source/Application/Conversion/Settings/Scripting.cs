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
	/// The scripting action allows to run a script after the conversion
	/// </summary>
	public partial class Scripting : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// If true, the given script or application will be executed
		/// </summary>
		public bool Enabled { get; set; } = false;
		
		/// <summary>
		/// Parameters that will be passed to the script in addition to the output files
		/// </summary>
		public string ParameterString { get; set; } = "";
		
		/// <summary>
		/// Path to the script or application
		/// </summary>
		public string ScriptFile { get; set; } = "";
		
		/// <summary>
		/// If false, the given script or application will be executed in a hidden window
		/// </summary>
		public bool Visible { get; set; } = true;
		
		/// <summary>
		/// Wait until the script execution has ended
		/// </summary>
		public bool WaitForScript { get; set; } = true;
		
		
		public void ReadValues(Data data, string path = "")
		{
			Enabled = bool.TryParse(data.GetValue(@"" + path + @"Enabled"), out var tmpEnabled) ? tmpEnabled : false;
			try { ParameterString = Data.UnescapeString(data.GetValue(@"" + path + @"ParameterString")); } catch { ParameterString = "";}
			try { ScriptFile = Data.UnescapeString(data.GetValue(@"" + path + @"ScriptFile")); } catch { ScriptFile = "";}
			Visible = bool.TryParse(data.GetValue(@"" + path + @"Visible"), out var tmpVisible) ? tmpVisible : true;
			WaitForScript = bool.TryParse(data.GetValue(@"" + path + @"WaitForScript"), out var tmpWaitForScript) ? tmpWaitForScript : true;
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"ParameterString", Data.EscapeString(ParameterString));
			data.SetValue(@"" + path + @"ScriptFile", Data.EscapeString(ScriptFile));
			data.SetValue(@"" + path + @"Visible", Visible.ToString());
			data.SetValue(@"" + path + @"WaitForScript", WaitForScript.ToString());
		}
		
		public Scripting Copy()
		{
			Scripting copy = new Scripting();
			
			copy.Enabled = Enabled;
			copy.ParameterString = ParameterString;
			copy.ScriptFile = ScriptFile;
			copy.Visible = Visible;
			copy.WaitForScript = WaitForScript;
			return copy;
		}
		
		public void ReplaceWith(Scripting source)
		{
			if(Enabled != source.Enabled)
				Enabled = source.Enabled;
				
			if(ParameterString != source.ParameterString)
				ParameterString = source.ParameterString;
				
			if(ScriptFile != source.ScriptFile)
				ScriptFile = source.ScriptFile;
				
			if(Visible != source.Visible)
				Visible = source.Visible;
				
			if(WaitForScript != source.WaitForScript)
				WaitForScript = source.WaitForScript;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is Scripting)) return false;
			Scripting v = o as Scripting;
			
			if (!Object.Equals(Enabled, v.Enabled)) return false;
			if (!Object.Equals(ParameterString, v.ParameterString)) return false;
			if (!Object.Equals(ScriptFile, v.ScriptFile)) return false;
			if (!Object.Equals(Visible, v.Visible)) return false;
			if (!Object.Equals(WaitForScript, v.WaitForScript)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
