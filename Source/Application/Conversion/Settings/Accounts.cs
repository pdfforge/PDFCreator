using pdfforge.DataStorage.Storage;
using pdfforge.DataStorage;
using PropertyChanged;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System;

// ! This file is generated automatically.
// ! Do not edit it outside the sections for custom code.
// ! These changes will be deleted during the next generation run

namespace pdfforge.PDFCreator.Conversion.Settings
{
	public partial class Accounts : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		public ObservableCollection<DropboxAccount> DropboxAccounts { get; set; } = new ObservableCollection<DropboxAccount>();
		public ObservableCollection<FtpAccount> FtpAccounts { get; set; } = new ObservableCollection<FtpAccount>();
		public ObservableCollection<HttpAccount> HttpAccounts { get; set; } = new ObservableCollection<HttpAccount>();
		public ObservableCollection<SmtpAccount> SmtpAccounts { get; set; } = new ObservableCollection<SmtpAccount>();
		public ObservableCollection<TimeServerAccount> TimeServerAccounts { get; set; } = new ObservableCollection<TimeServerAccount>();
		
		public void ReadValues(Data data, string path = "")
		{
			
			try
			{
				int numClasses = int.Parse(data.GetValue(@"" + path + @"DropboxAccounts\numClasses"));
				for (int i = 0; i < numClasses; i++)
				{
					DropboxAccount tmp = new DropboxAccount();
					tmp.ReadValues(data, @"" + path + @"DropboxAccounts\" + i + @"\");
					DropboxAccounts.Add(tmp);
				}
			} catch {}
			
			
			try
			{
				int numClasses = int.Parse(data.GetValue(@"" + path + @"FtpAccounts\numClasses"));
				for (int i = 0; i < numClasses; i++)
				{
					FtpAccount tmp = new FtpAccount();
					tmp.ReadValues(data, @"" + path + @"FtpAccounts\" + i + @"\");
					FtpAccounts.Add(tmp);
				}
			} catch {}
			
			
			try
			{
				int numClasses = int.Parse(data.GetValue(@"" + path + @"HttpAccounts\numClasses"));
				for (int i = 0; i < numClasses; i++)
				{
					HttpAccount tmp = new HttpAccount();
					tmp.ReadValues(data, @"" + path + @"HttpAccounts\" + i + @"\");
					HttpAccounts.Add(tmp);
				}
			} catch {}
			
			
			try
			{
				int numClasses = int.Parse(data.GetValue(@"" + path + @"SmtpAccounts\numClasses"));
				for (int i = 0; i < numClasses; i++)
				{
					SmtpAccount tmp = new SmtpAccount();
					tmp.ReadValues(data, @"" + path + @"SmtpAccounts\" + i + @"\");
					SmtpAccounts.Add(tmp);
				}
			} catch {}
			
			
			try
			{
				int numClasses = int.Parse(data.GetValue(@"" + path + @"TimeServerAccounts\numClasses"));
				for (int i = 0; i < numClasses; i++)
				{
					TimeServerAccount tmp = new TimeServerAccount();
					tmp.ReadValues(data, @"" + path + @"TimeServerAccounts\" + i + @"\");
					TimeServerAccounts.Add(tmp);
				}
			} catch {}
			
		}
		
		public void StoreValues(Data data, string path)
		{
			
			for (int i = 0; i < DropboxAccounts.Count; i++)
			{
				DropboxAccount tmp = DropboxAccounts[i];
				tmp.StoreValues(data, @"" + path + @"DropboxAccounts\" + i + @"\");
			}
			data.SetValue(@"" + path + @"DropboxAccounts\numClasses", DropboxAccounts.Count.ToString());
			
			
			for (int i = 0; i < FtpAccounts.Count; i++)
			{
				FtpAccount tmp = FtpAccounts[i];
				tmp.StoreValues(data, @"" + path + @"FtpAccounts\" + i + @"\");
			}
			data.SetValue(@"" + path + @"FtpAccounts\numClasses", FtpAccounts.Count.ToString());
			
			
			for (int i = 0; i < HttpAccounts.Count; i++)
			{
				HttpAccount tmp = HttpAccounts[i];
				tmp.StoreValues(data, @"" + path + @"HttpAccounts\" + i + @"\");
			}
			data.SetValue(@"" + path + @"HttpAccounts\numClasses", HttpAccounts.Count.ToString());
			
			
			for (int i = 0; i < SmtpAccounts.Count; i++)
			{
				SmtpAccount tmp = SmtpAccounts[i];
				tmp.StoreValues(data, @"" + path + @"SmtpAccounts\" + i + @"\");
			}
			data.SetValue(@"" + path + @"SmtpAccounts\numClasses", SmtpAccounts.Count.ToString());
			
			
			for (int i = 0; i < TimeServerAccounts.Count; i++)
			{
				TimeServerAccount tmp = TimeServerAccounts[i];
				tmp.StoreValues(data, @"" + path + @"TimeServerAccounts\" + i + @"\");
			}
			data.SetValue(@"" + path + @"TimeServerAccounts\numClasses", TimeServerAccounts.Count.ToString());
			
		}
		
		public Accounts Copy()
		{
			Accounts copy = new Accounts();
			
			
			copy.DropboxAccounts = new ObservableCollection<DropboxAccount>();
			for (int i = 0; i < DropboxAccounts.Count; i++)
			{
				copy.DropboxAccounts.Add(DropboxAccounts[i].Copy());
			}
			
			
			copy.FtpAccounts = new ObservableCollection<FtpAccount>();
			for (int i = 0; i < FtpAccounts.Count; i++)
			{
				copy.FtpAccounts.Add(FtpAccounts[i].Copy());
			}
			
			
			copy.HttpAccounts = new ObservableCollection<HttpAccount>();
			for (int i = 0; i < HttpAccounts.Count; i++)
			{
				copy.HttpAccounts.Add(HttpAccounts[i].Copy());
			}
			
			
			copy.SmtpAccounts = new ObservableCollection<SmtpAccount>();
			for (int i = 0; i < SmtpAccounts.Count; i++)
			{
				copy.SmtpAccounts.Add(SmtpAccounts[i].Copy());
			}
			
			
			copy.TimeServerAccounts = new ObservableCollection<TimeServerAccount>();
			for (int i = 0; i < TimeServerAccounts.Count; i++)
			{
				copy.TimeServerAccounts.Add(TimeServerAccounts[i].Copy());
			}
			
			return copy;
		}
		
		public void ReplaceWith(Accounts source)
		{
			
			DropboxAccounts.Clear();
			for (int i = 0; i < source.DropboxAccounts.Count; i++)
			{
				DropboxAccounts.Add(source.DropboxAccounts[i].Copy());
			}
			
			
			FtpAccounts.Clear();
			for (int i = 0; i < source.FtpAccounts.Count; i++)
			{
				FtpAccounts.Add(source.FtpAccounts[i].Copy());
			}
			
			
			HttpAccounts.Clear();
			for (int i = 0; i < source.HttpAccounts.Count; i++)
			{
				HttpAccounts.Add(source.HttpAccounts[i].Copy());
			}
			
			
			SmtpAccounts.Clear();
			for (int i = 0; i < source.SmtpAccounts.Count; i++)
			{
				SmtpAccounts.Add(source.SmtpAccounts[i].Copy());
			}
			
			
			TimeServerAccounts.Clear();
			for (int i = 0; i < source.TimeServerAccounts.Count; i++)
			{
				TimeServerAccounts.Add(source.TimeServerAccounts[i].Copy());
			}
			
		}
		
		public override bool Equals(object o)
		{
			if (!(o is Accounts)) return false;
			Accounts v = o as Accounts;
			
			
			if (DropboxAccounts.Count != v.DropboxAccounts.Count) return false;
			for (int i = 0; i < DropboxAccounts.Count; i++)
			{
				if (!DropboxAccounts[i].Equals(v.DropboxAccounts[i])) return false;
			}
			
			
			if (FtpAccounts.Count != v.FtpAccounts.Count) return false;
			for (int i = 0; i < FtpAccounts.Count; i++)
			{
				if (!FtpAccounts[i].Equals(v.FtpAccounts[i])) return false;
			}
			
			
			if (HttpAccounts.Count != v.HttpAccounts.Count) return false;
			for (int i = 0; i < HttpAccounts.Count; i++)
			{
				if (!HttpAccounts[i].Equals(v.HttpAccounts[i])) return false;
			}
			
			
			if (SmtpAccounts.Count != v.SmtpAccounts.Count) return false;
			for (int i = 0; i < SmtpAccounts.Count; i++)
			{
				if (!SmtpAccounts[i].Equals(v.SmtpAccounts[i])) return false;
			}
			
			
			if (TimeServerAccounts.Count != v.TimeServerAccounts.Count) return false;
			for (int i = 0; i < TimeServerAccounts.Count; i++)
			{
				if (!TimeServerAccounts[i].Equals(v.TimeServerAccounts[i])) return false;
			}
			
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
