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
		
		
		public void ReadValues(Data data, string path = "")
		{
			Capacity = int.TryParse(data.GetValue(@"" + path + @"Capacity"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tmpCapacity) ? tmpCapacity : 30;
			Column = Enum.TryParse<TableColumns>(data.GetValue(@"" + path + @"Column"), out var tmpColumn) ? tmpColumn : TableColumns.Author;
			Enabled = bool.TryParse(data.GetValue(@"" + path + @"Enabled"), out var tmpEnabled) ? tmpEnabled : true;
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"Capacity", Capacity.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"Column", Column.ToString());
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
		}
		
		public JobHistory Copy()
		{
			JobHistory copy = new JobHistory();
			
			copy.Capacity = Capacity;
			copy.Column = Column;
			copy.Enabled = Enabled;
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
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is JobHistory)) return false;
			JobHistory v = o as JobHistory;
			
			if (!Object.Equals(Capacity, v.Capacity)) return false;
			if (!Object.Equals(Column, v.Column)) return false;
			if (!Object.Equals(Enabled, v.Enabled)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
