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
	public partial class JobHistory : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// Max number of jobs tracked in history
		/// </summary>
		public int Capacity { get; set; } = 30;
		
		/// <summary>
		/// Selected column in search options
		/// </summary>
		public TableColumns Column { get; set; } = TableColumns.Author;
		
		/// <summary>
		/// If enabled PDFCreator tracks latest converted files in history
		/// </summary>
		public bool Enabled { get; set; } = true;
		
		/// <summary>
		/// Last DateFrom in search options
		/// </summary>
		public DateTime LastDateFrom { get; set; } = DateTime.Now;
		
		/// <summary>
		/// Last DateTo in search options
		/// </summary>
		public DateTime LastDateTo { get; set; } = DateTime.Now;
		
		/// <summary>
		/// Last searched text in search options
		/// </summary>
		public string LastSearchText { get; set; } = "";
		
		
		public void ReadValues(Data data, string path = "")
		{
			Capacity = int.TryParse(data.GetValue(@"" + path + @"Capacity"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpCapacity) ? tmpCapacity : 30;
			Column = Enum.TryParse<TableColumns>(data.GetValue(@"" + path + @"Column"), out var tmpColumn) ? tmpColumn : TableColumns.Author;
			Enabled = bool.TryParse(data.GetValue(@"" + path + @"Enabled"), out var tmpEnabled) ? tmpEnabled : true;
			LastDateFrom = DateTime.TryParse(data.GetValue(@"" + path + @"LastDateFrom"), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var tmpLastDateFrom) ? tmpLastDateFrom : DateTime.Now;
			LastDateTo = DateTime.TryParse(data.GetValue(@"" + path + @"LastDateTo"), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var tmpLastDateTo) ? tmpLastDateTo : DateTime.Now;
			try { LastSearchText = Data.UnescapeString(data.GetValue(@"" + path + @"LastSearchText")); } catch { LastSearchText = "";}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Capacity", Capacity.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"Column", Column.ToString());
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"LastDateFrom", LastDateFrom.ToString("yyyy-MM-dd HH:mm:ss"));
			data.SetValue(@"" + path + @"LastDateTo", LastDateTo.ToString("yyyy-MM-dd HH:mm:ss"));
			data.SetValue(@"" + path + @"LastSearchText", Data.EscapeString(LastSearchText));
		}
		
		public JobHistory Copy()
		{
			JobHistory copy = new JobHistory();
			
			copy.Capacity = Capacity;
			copy.Column = Column;
			copy.Enabled = Enabled;
			copy.LastDateFrom = LastDateFrom;
			copy.LastDateTo = LastDateTo;
			copy.LastSearchText = LastSearchText;
			return copy;
		}
		
		public void ReplaceWith(JobHistory source)
		{
			if(Capacity != source.Capacity)
				Capacity = source.Capacity;
				
			if(Column != source.Column)
				Column = source.Column;
				
			if(Enabled != source.Enabled)
				Enabled = source.Enabled;
				
			if(LastDateFrom != source.LastDateFrom)
				LastDateFrom = source.LastDateFrom;
				
			if(LastDateTo != source.LastDateTo)
				LastDateTo = source.LastDateTo;
				
			if(LastSearchText != source.LastSearchText)
				LastSearchText = source.LastSearchText;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is JobHistory)) return false;
			JobHistory v = o as JobHistory;
			
			if (!Capacity.Equals(v.Capacity)) return false;
			if (!Column.Equals(v.Column)) return false;
			if (!Enabled.Equals(v.Enabled)) return false;
			if (!LastDateFrom.Equals(v.LastDateFrom)) return false;
			if (!LastDateTo.Equals(v.LastDateTo)) return false;
			if (!LastSearchText.Equals(v.LastSearchText)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
