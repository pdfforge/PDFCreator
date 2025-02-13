using pdfforge.DataStorage.Storage;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using PropertyChanged;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System;

// ! This file is generated automatically.
// ! Do not edit it outside the sections for custom code.
// ! These changes will be deleted during the next generation run

namespace pdfforge.PDFCreator.Conversion.Settings
{
	public partial class ConversionProfile : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// Appends one or more pages at the end of the converted document
		/// </summary>
		public AttachmentPage AttachmentPage { get; set; } = new AttachmentPage();
		
		/// <summary>
		/// AutoSave allows to create PDF files without user interaction
		/// </summary>
		public AutoSave AutoSave { get; set; } = new AutoSave();
		
		/// <summary>
		/// Adds a page background to the resulting document
		/// </summary>
		public BackgroundPage BackgroundPage { get; set; } = new BackgroundPage();
		
		/// <summary>
		/// Inserts one or more pages at the beginning of the converted document
		/// </summary>
		public CoverPage CoverPage { get; set; } = new CoverPage();
		
		/// <summary>
		/// Pre- and post-conversion actions calling functions from a custom script
		/// </summary>
		public CustomScript CustomScript { get; set; } = new CustomScript();
		
		/// <summary>
		/// Dropbox settings for currently logged user
		/// </summary>
		public DropboxSettings DropboxSettings { get; set; } = new DropboxSettings();
		
		/// <summary>
		/// Opens the default email client with the converted document as attachment
		/// </summary>
		public EmailClientSettings EmailClientSettings { get; set; } = new EmailClientSettings();
		
		/// <summary>
		/// Sends a mail without user interaction through SMTP
		/// </summary>
		public EmailSmtpSettings EmailSmtpSettings { get; set; } = new EmailSmtpSettings();
		
		/// <summary>
		/// Opens the default outlook 365 website with the converted document as attachment
		/// </summary>
		public EmailWebSettings EmailWebSettings { get; set; } = new EmailWebSettings();
		
		public ForwardToFurtherProfile ForwardToFurtherProfile { get; set; } = new ForwardToFurtherProfile();
		
		/// <summary>
		/// Upload the converted documents with FTP
		/// </summary>
		public Ftp Ftp { get; set; } = new Ftp();
		
		/// <summary>
		/// Ghostscript settings
		/// </summary>
		public Ghostscript Ghostscript { get; set; } = new Ghostscript();
		
		/// <summary>
		/// Action to upload files to a HTTP server
		/// </summary>
		public HttpSettings HttpSettings { get; set; } = new HttpSettings();
		
		/// <summary>
		/// Settings for the JPEG output format
		/// </summary>
		public JpegSettings JpegSettings { get; set; } = new JpegSettings();
		
		/// <summary>
		/// OneDrive settings for the current user
		/// </summary>
		public OneDriveSettings OneDriveSettings { get; set; } = new OneDriveSettings();
		
		/// <summary>
		/// Opens the printed file in a viewer
		/// </summary>
		public OpenViewer OpenViewer { get; set; } = new OpenViewer();
		
		/// <summary>
		/// Settings for page numbers.
		/// </summary>
		public PageNumbers PageNumbers { get; set; } = new PageNumbers();
		
		/// <summary>
		/// Settings for the PDF output format
		/// </summary>
		public PdfSettings PdfSettings { get; set; } = new PdfSettings();
		
		/// <summary>
		/// Settings for the PNG output format
		/// </summary>
		public PngSettings PngSettings { get; set; } = new PngSettings();
		
		/// <summary>
		/// Print the document to a physical printer
		/// </summary>
		public Printing Printing { get; set; } = new Printing();
		
		/// <summary>
		/// Properties of the profile
		/// </summary>
		public Properties Properties { get; set; } = new Properties();
		
		/// <summary>
		/// The scripting action allows to run a script after the conversion
		/// </summary>
		public Scripting Scripting { get; set; } = new Scripting();
		
		/// <summary>
		/// Place a stamp text on all pages of the document
		/// </summary>
		public Stamping Stamping { get; set; } = new Stamping();
		
		public TextSettings TextSettings { get; set; } = new TextSettings();
		
		/// <summary>
		/// Settings for the TIFF output format
		/// </summary>
		public TiffSettings TiffSettings { get; set; } = new TiffSettings();
		
		/// <summary>
		/// Parse ps files for user defined tokens
		/// </summary>
		public UserTokens UserTokens { get; set; } = new UserTokens();
		
		/// <summary>
		/// Adds a watermark to the resulting document
		/// </summary>
		public Watermark Watermark { get; set; } = new Watermark();
		
		/// <summary>
		///  Order in which actions are processed by an executing job 
		/// </summary>
		public List<string> ActionOrder { get; set; } = new List<string>();
		
		/// <summary>
		/// Template for the Author field. This may contain tokens.
		/// </summary>
		public string AuthorTemplate { get; set; } = "<PrintJobAuthor>";
		
		/// <summary>
		/// Template of which the filename will be created. This may contain Tokens.
		/// </summary>
		public string FileNameTemplate { get; set; } = "<Title>";
		
		/// <summary>
		/// GUID of the profile
		/// </summary>
		public string Guid { get; set; } = "";
		
		/// <summary>
		/// Template for the Keyword field. This may contain tokens.
		/// </summary>
		public string KeywordTemplate { get; set; } = "";
		
		/// <summary>
		/// Name of the profile
		/// </summary>
		public string Name { get; set; } = "NewProfile";
		
		/// <summary>
		/// Default format for this print job.
		/// </summary>
		public OutputFormat OutputFormat { get; set; } = OutputFormat.Pdf;
		
		/// <summary>
		/// Enable to save files only in a temp directory
		/// </summary>
		public bool SaveFileTemporary { get; set; } = false;
		
		/// <summary>
		/// Show a notification after converting the document
		/// </summary>
		public bool ShowAllNotifications { get; set; } = true;
		
		/// <summary>
		/// Only show notification for error
		/// </summary>
		public bool ShowOnlyErrorNotifications { get; set; } = false;
		
		/// <summary>
		/// If true, a progress window will be shown during conversion
		/// </summary>
		public bool ShowProgress { get; set; } = true;
		
		/// <summary>
		/// Show quick actions page after converting the document
		/// </summary>
		public bool ShowQuickActions { get; set; } = true;
		
		/// <summary>
		/// Allows to skip the print dialog (where metadata are set) and directly proceed to the save dialog
		/// </summary>
		public bool SkipPrintDialog { get; set; } = false;
		
		/// <summary>
		/// Allow to proceed with further send actions if a single send action fails
		/// </summary>
		public bool SkipSendFailures { get; set; } = false;
		
		/// <summary>
		/// Template for the Subject field. This may contain tokens.
		/// </summary>
		public string SubjectTemplate { get; set; } = "";
		
		/// <summary>
		/// Directory in which the files will be saved (in interactive mode, this is the default location that is presented to the user)
		/// </summary>
		public string TargetDirectory { get; set; } = "";
		
		/// <summary>
		/// Template for the Title field. This may contain tokens.
		/// </summary>
		public string TitleTemplate { get; set; } = "<PrintJobName>";
		
		/// <summary>
		/// Show a warning for failing send actions (only if SkipSendFailures is active)
		/// </summary>
		public bool WarnSendFailures { get; set; } = false;
		
		
		public void ReadValues(Data data, string path) {
			AttachmentPage.ReadValues(data, path + @"AttachmentPage\");
			AutoSave.ReadValues(data, path + @"AutoSave\");
			BackgroundPage.ReadValues(data, path + @"BackgroundPage\");
			CoverPage.ReadValues(data, path + @"CoverPage\");
			CustomScript.ReadValues(data, path + @"CustomScript\");
			DropboxSettings.ReadValues(data, path + @"DropboxSettings\");
			EmailClientSettings.ReadValues(data, path + @"EmailClientSettings\");
			EmailSmtpSettings.ReadValues(data, path + @"EmailSmtpSettings\");
			EmailWebSettings.ReadValues(data, path + @"EmailWebSettings\");
			ForwardToFurtherProfile.ReadValues(data, path + @"ForwardToFurtherProfile\");
			Ftp.ReadValues(data, path + @"Ftp\");
			Ghostscript.ReadValues(data, path + @"Ghostscript\");
			HttpSettings.ReadValues(data, path + @"HttpSettings\");
			JpegSettings.ReadValues(data, path + @"JpegSettings\");
			OneDriveSettings.ReadValues(data, path + @"OneDriveSettings\");
			OpenViewer.ReadValues(data, path + @"OpenViewer\");
			PageNumbers.ReadValues(data, path + @"PageNumbers\");
			PdfSettings.ReadValues(data, path + @"PdfSettings\");
			PngSettings.ReadValues(data, path + @"PngSettings\");
			Printing.ReadValues(data, path + @"Printing\");
			Properties.ReadValues(data, path + @"Properties\");
			Scripting.ReadValues(data, path + @"Scripting\");
			Stamping.ReadValues(data, path + @"Stamping\");
			TextSettings.ReadValues(data, path + @"TextSettings\");
			TiffSettings.ReadValues(data, path + @"TiffSettings\");
			UserTokens.ReadValues(data, path + @"UserTokens\");
			Watermark.ReadValues(data, path + @"Watermark\");
			try{
				int numClasses = int.Parse(data.GetValue(@"" + path + @"ActionOrder\numClasses"));
				for (int i = 0; i < numClasses; i++){
					try{
						var value = Data.UnescapeString(data.GetValue(path + @"ActionOrder\" + i + @"\ActionOrder"));
						ActionOrder.Add(value);
					}catch{}
				}
			}catch{}
			try { AuthorTemplate = Data.UnescapeString(data.GetValue(@"" + path + @"AuthorTemplate")); } catch { AuthorTemplate = "<PrintJobAuthor>";}
			try { FileNameTemplate = Data.UnescapeString(data.GetValue(@"" + path + @"FileNameTemplate")); } catch { FileNameTemplate = "<Title>";}
			try { Guid = Data.UnescapeString(data.GetValue(@"" + path + @"Guid")); } catch { Guid = "";}
			try { KeywordTemplate = Data.UnescapeString(data.GetValue(@"" + path + @"KeywordTemplate")); } catch { KeywordTemplate = "";}
			try { Name = Data.UnescapeString(data.GetValue(@"" + path + @"Name")); } catch { Name = "NewProfile";}
			OutputFormat = Enum.TryParse<OutputFormat>(data.GetValue(@"" + path + @"OutputFormat"), out var tmpOutputFormat) ? tmpOutputFormat : OutputFormat.Pdf;
			SaveFileTemporary = bool.TryParse(data.GetValue(@"" + path + @"SaveFileTemporary"), out var tmpSaveFileTemporary) ? tmpSaveFileTemporary : false;
			ShowAllNotifications = bool.TryParse(data.GetValue(@"" + path + @"ShowAllNotifications"), out var tmpShowAllNotifications) ? tmpShowAllNotifications : true;
			ShowOnlyErrorNotifications = bool.TryParse(data.GetValue(@"" + path + @"ShowOnlyErrorNotifications"), out var tmpShowOnlyErrorNotifications) ? tmpShowOnlyErrorNotifications : false;
			ShowProgress = bool.TryParse(data.GetValue(@"" + path + @"ShowProgress"), out var tmpShowProgress) ? tmpShowProgress : true;
			ShowQuickActions = bool.TryParse(data.GetValue(@"" + path + @"ShowQuickActions"), out var tmpShowQuickActions) ? tmpShowQuickActions : true;
			SkipPrintDialog = bool.TryParse(data.GetValue(@"" + path + @"SkipPrintDialog"), out var tmpSkipPrintDialog) ? tmpSkipPrintDialog : false;
			SkipSendFailures = bool.TryParse(data.GetValue(@"" + path + @"SkipSendFailures"), out var tmpSkipSendFailures) ? tmpSkipSendFailures : false;
			try { SubjectTemplate = Data.UnescapeString(data.GetValue(@"" + path + @"SubjectTemplate")); } catch { SubjectTemplate = "";}
			try { TargetDirectory = Data.UnescapeString(data.GetValue(@"" + path + @"TargetDirectory")); } catch { TargetDirectory = "";}
			try { TitleTemplate = Data.UnescapeString(data.GetValue(@"" + path + @"TitleTemplate")); } catch { TitleTemplate = "<PrintJobName>";}
			WarnSendFailures = bool.TryParse(data.GetValue(@"" + path + @"WarnSendFailures"), out var tmpWarnSendFailures) ? tmpWarnSendFailures : false;
		}
		
		public void StoreValues(Data data, string path) {
			AttachmentPage.StoreValues(data, path + @"AttachmentPage\");
			AutoSave.StoreValues(data, path + @"AutoSave\");
			BackgroundPage.StoreValues(data, path + @"BackgroundPage\");
			CoverPage.StoreValues(data, path + @"CoverPage\");
			CustomScript.StoreValues(data, path + @"CustomScript\");
			DropboxSettings.StoreValues(data, path + @"DropboxSettings\");
			EmailClientSettings.StoreValues(data, path + @"EmailClientSettings\");
			EmailSmtpSettings.StoreValues(data, path + @"EmailSmtpSettings\");
			EmailWebSettings.StoreValues(data, path + @"EmailWebSettings\");
			ForwardToFurtherProfile.StoreValues(data, path + @"ForwardToFurtherProfile\");
			Ftp.StoreValues(data, path + @"Ftp\");
			Ghostscript.StoreValues(data, path + @"Ghostscript\");
			HttpSettings.StoreValues(data, path + @"HttpSettings\");
			JpegSettings.StoreValues(data, path + @"JpegSettings\");
			OneDriveSettings.StoreValues(data, path + @"OneDriveSettings\");
			OpenViewer.StoreValues(data, path + @"OpenViewer\");
			PageNumbers.StoreValues(data, path + @"PageNumbers\");
			PdfSettings.StoreValues(data, path + @"PdfSettings\");
			PngSettings.StoreValues(data, path + @"PngSettings\");
			Printing.StoreValues(data, path + @"Printing\");
			Properties.StoreValues(data, path + @"Properties\");
			Scripting.StoreValues(data, path + @"Scripting\");
			Stamping.StoreValues(data, path + @"Stamping\");
			TextSettings.StoreValues(data, path + @"TextSettings\");
			TiffSettings.StoreValues(data, path + @"TiffSettings\");
			UserTokens.StoreValues(data, path + @"UserTokens\");
			Watermark.StoreValues(data, path + @"Watermark\");
			for (int i = 0; i < ActionOrder.Count; i++){
				data.SetValue(path + @"ActionOrder\" + i + @"\ActionOrder", Data.EscapeString(ActionOrder[i]));
			}
			data.SetValue(path + @"ActionOrder\numClasses", ActionOrder.Count.ToString());
			data.SetValue(@"" + path + @"AuthorTemplate", Data.EscapeString(AuthorTemplate));
			data.SetValue(@"" + path + @"FileNameTemplate", Data.EscapeString(FileNameTemplate));
			data.SetValue(@"" + path + @"Guid", Data.EscapeString(Guid));
			data.SetValue(@"" + path + @"KeywordTemplate", Data.EscapeString(KeywordTemplate));
			data.SetValue(@"" + path + @"Name", Data.EscapeString(Name));
			data.SetValue(@"" + path + @"OutputFormat", OutputFormat.ToString());
			data.SetValue(@"" + path + @"SaveFileTemporary", SaveFileTemporary.ToString());
			data.SetValue(@"" + path + @"ShowAllNotifications", ShowAllNotifications.ToString());
			data.SetValue(@"" + path + @"ShowOnlyErrorNotifications", ShowOnlyErrorNotifications.ToString());
			data.SetValue(@"" + path + @"ShowProgress", ShowProgress.ToString());
			data.SetValue(@"" + path + @"ShowQuickActions", ShowQuickActions.ToString());
			data.SetValue(@"" + path + @"SkipPrintDialog", SkipPrintDialog.ToString());
			data.SetValue(@"" + path + @"SkipSendFailures", SkipSendFailures.ToString());
			data.SetValue(@"" + path + @"SubjectTemplate", Data.EscapeString(SubjectTemplate));
			data.SetValue(@"" + path + @"TargetDirectory", Data.EscapeString(TargetDirectory));
			data.SetValue(@"" + path + @"TitleTemplate", Data.EscapeString(TitleTemplate));
			data.SetValue(@"" + path + @"WarnSendFailures", WarnSendFailures.ToString());
		}
		public ConversionProfile Copy()
		{
			ConversionProfile copy = new ConversionProfile();
			
			copy.AttachmentPage = AttachmentPage.Copy();
			copy.AutoSave = AutoSave.Copy();
			copy.BackgroundPage = BackgroundPage.Copy();
			copy.CoverPage = CoverPage.Copy();
			copy.CustomScript = CustomScript.Copy();
			copy.DropboxSettings = DropboxSettings.Copy();
			copy.EmailClientSettings = EmailClientSettings.Copy();
			copy.EmailSmtpSettings = EmailSmtpSettings.Copy();
			copy.EmailWebSettings = EmailWebSettings.Copy();
			copy.ForwardToFurtherProfile = ForwardToFurtherProfile.Copy();
			copy.Ftp = Ftp.Copy();
			copy.Ghostscript = Ghostscript.Copy();
			copy.HttpSettings = HttpSettings.Copy();
			copy.JpegSettings = JpegSettings.Copy();
			copy.OneDriveSettings = OneDriveSettings.Copy();
			copy.OpenViewer = OpenViewer.Copy();
			copy.PageNumbers = PageNumbers.Copy();
			copy.PdfSettings = PdfSettings.Copy();
			copy.PngSettings = PngSettings.Copy();
			copy.Printing = Printing.Copy();
			copy.Properties = Properties.Copy();
			copy.Scripting = Scripting.Copy();
			copy.Stamping = Stamping.Copy();
			copy.TextSettings = TextSettings.Copy();
			copy.TiffSettings = TiffSettings.Copy();
			copy.UserTokens = UserTokens.Copy();
			copy.Watermark = Watermark.Copy();
			copy.ActionOrder = new List<string>(ActionOrder);
			copy.AuthorTemplate = AuthorTemplate;
			copy.FileNameTemplate = FileNameTemplate;
			copy.Guid = Guid;
			copy.KeywordTemplate = KeywordTemplate;
			copy.Name = Name;
			copy.OutputFormat = OutputFormat;
			copy.SaveFileTemporary = SaveFileTemporary;
			copy.ShowAllNotifications = ShowAllNotifications;
			copy.ShowOnlyErrorNotifications = ShowOnlyErrorNotifications;
			copy.ShowProgress = ShowProgress;
			copy.ShowQuickActions = ShowQuickActions;
			copy.SkipPrintDialog = SkipPrintDialog;
			copy.SkipSendFailures = SkipSendFailures;
			copy.SubjectTemplate = SubjectTemplate;
			copy.TargetDirectory = TargetDirectory;
			copy.TitleTemplate = TitleTemplate;
			copy.WarnSendFailures = WarnSendFailures;
			return copy;
		}
		
		public void ReplaceWith(ConversionProfile source)
		{
			AttachmentPage.ReplaceWith(source.AttachmentPage);
			AutoSave.ReplaceWith(source.AutoSave);
			BackgroundPage.ReplaceWith(source.BackgroundPage);
			CoverPage.ReplaceWith(source.CoverPage);
			CustomScript.ReplaceWith(source.CustomScript);
			DropboxSettings.ReplaceWith(source.DropboxSettings);
			EmailClientSettings.ReplaceWith(source.EmailClientSettings);
			EmailSmtpSettings.ReplaceWith(source.EmailSmtpSettings);
			EmailWebSettings.ReplaceWith(source.EmailWebSettings);
			ForwardToFurtherProfile.ReplaceWith(source.ForwardToFurtherProfile);
			Ftp.ReplaceWith(source.Ftp);
			Ghostscript.ReplaceWith(source.Ghostscript);
			HttpSettings.ReplaceWith(source.HttpSettings);
			JpegSettings.ReplaceWith(source.JpegSettings);
			OneDriveSettings.ReplaceWith(source.OneDriveSettings);
			OpenViewer.ReplaceWith(source.OpenViewer);
			PageNumbers.ReplaceWith(source.PageNumbers);
			PdfSettings.ReplaceWith(source.PdfSettings);
			PngSettings.ReplaceWith(source.PngSettings);
			Printing.ReplaceWith(source.Printing);
			Properties.ReplaceWith(source.Properties);
			Scripting.ReplaceWith(source.Scripting);
			Stamping.ReplaceWith(source.Stamping);
			TextSettings.ReplaceWith(source.TextSettings);
			TiffSettings.ReplaceWith(source.TiffSettings);
			UserTokens.ReplaceWith(source.UserTokens);
			Watermark.ReplaceWith(source.Watermark);
			ActionOrder.Clear();
			for (int i = 0; i < source.ActionOrder.Count; i++)
			{
				ActionOrder.Add(source.ActionOrder[i]);
			}
			
			if(AuthorTemplate != source.AuthorTemplate)
				AuthorTemplate = source.AuthorTemplate;
				
			if(FileNameTemplate != source.FileNameTemplate)
				FileNameTemplate = source.FileNameTemplate;
				
			if(Guid != source.Guid)
				Guid = source.Guid;
				
			if(KeywordTemplate != source.KeywordTemplate)
				KeywordTemplate = source.KeywordTemplate;
				
			if(Name != source.Name)
				Name = source.Name;
				
			if(OutputFormat != source.OutputFormat)
				OutputFormat = source.OutputFormat;
				
			if(SaveFileTemporary != source.SaveFileTemporary)
				SaveFileTemporary = source.SaveFileTemporary;
				
			if(ShowAllNotifications != source.ShowAllNotifications)
				ShowAllNotifications = source.ShowAllNotifications;
				
			if(ShowOnlyErrorNotifications != source.ShowOnlyErrorNotifications)
				ShowOnlyErrorNotifications = source.ShowOnlyErrorNotifications;
				
			if(ShowProgress != source.ShowProgress)
				ShowProgress = source.ShowProgress;
				
			if(ShowQuickActions != source.ShowQuickActions)
				ShowQuickActions = source.ShowQuickActions;
				
			if(SkipPrintDialog != source.SkipPrintDialog)
				SkipPrintDialog = source.SkipPrintDialog;
				
			if(SkipSendFailures != source.SkipSendFailures)
				SkipSendFailures = source.SkipSendFailures;
				
			if(SubjectTemplate != source.SubjectTemplate)
				SubjectTemplate = source.SubjectTemplate;
				
			if(TargetDirectory != source.TargetDirectory)
				TargetDirectory = source.TargetDirectory;
				
			if(TitleTemplate != source.TitleTemplate)
				TitleTemplate = source.TitleTemplate;
				
			if(WarnSendFailures != source.WarnSendFailures)
				WarnSendFailures = source.WarnSendFailures;
				
		}
		
		public override bool Equals(object o)
		{
			if (!(o is ConversionProfile)) return false;
			ConversionProfile v = o as ConversionProfile;
			
			if (!Object.Equals(AttachmentPage, v.AttachmentPage)) return false;
			if (!Object.Equals(AutoSave, v.AutoSave)) return false;
			if (!Object.Equals(BackgroundPage, v.BackgroundPage)) return false;
			if (!Object.Equals(CoverPage, v.CoverPage)) return false;
			if (!Object.Equals(CustomScript, v.CustomScript)) return false;
			if (!Object.Equals(DropboxSettings, v.DropboxSettings)) return false;
			if (!Object.Equals(EmailClientSettings, v.EmailClientSettings)) return false;
			if (!Object.Equals(EmailSmtpSettings, v.EmailSmtpSettings)) return false;
			if (!Object.Equals(EmailWebSettings, v.EmailWebSettings)) return false;
			if (!Object.Equals(ForwardToFurtherProfile, v.ForwardToFurtherProfile)) return false;
			if (!Object.Equals(Ftp, v.Ftp)) return false;
			if (!Object.Equals(Ghostscript, v.Ghostscript)) return false;
			if (!Object.Equals(HttpSettings, v.HttpSettings)) return false;
			if (!Object.Equals(JpegSettings, v.JpegSettings)) return false;
			if (!Object.Equals(OneDriveSettings, v.OneDriveSettings)) return false;
			if (!Object.Equals(OpenViewer, v.OpenViewer)) return false;
			if (!Object.Equals(PageNumbers, v.PageNumbers)) return false;
			if (!Object.Equals(PdfSettings, v.PdfSettings)) return false;
			if (!Object.Equals(PngSettings, v.PngSettings)) return false;
			if (!Object.Equals(Printing, v.Printing)) return false;
			if (!Object.Equals(Properties, v.Properties)) return false;
			if (!Object.Equals(Scripting, v.Scripting)) return false;
			if (!Object.Equals(Stamping, v.Stamping)) return false;
			if (!Object.Equals(TextSettings, v.TextSettings)) return false;
			if (!Object.Equals(TiffSettings, v.TiffSettings)) return false;
			if (!Object.Equals(UserTokens, v.UserTokens)) return false;
			if (!Object.Equals(Watermark, v.Watermark)) return false;
			if (!ActionOrder.SequenceEqual(v.ActionOrder)) return false;
			if (!Object.Equals(AuthorTemplate, v.AuthorTemplate)) return false;
			if (!Object.Equals(FileNameTemplate, v.FileNameTemplate)) return false;
			if (!Object.Equals(Guid, v.Guid)) return false;
			if (!Object.Equals(KeywordTemplate, v.KeywordTemplate)) return false;
			if (!Object.Equals(Name, v.Name)) return false;
			if (!Object.Equals(OutputFormat, v.OutputFormat)) return false;
			if (!Object.Equals(SaveFileTemporary, v.SaveFileTemporary)) return false;
			if (!Object.Equals(ShowAllNotifications, v.ShowAllNotifications)) return false;
			if (!Object.Equals(ShowOnlyErrorNotifications, v.ShowOnlyErrorNotifications)) return false;
			if (!Object.Equals(ShowProgress, v.ShowProgress)) return false;
			if (!Object.Equals(ShowQuickActions, v.ShowQuickActions)) return false;
			if (!Object.Equals(SkipPrintDialog, v.SkipPrintDialog)) return false;
			if (!Object.Equals(SkipSendFailures, v.SkipSendFailures)) return false;
			if (!Object.Equals(SubjectTemplate, v.SubjectTemplate)) return false;
			if (!Object.Equals(TargetDirectory, v.TargetDirectory)) return false;
			if (!Object.Equals(TitleTemplate, v.TitleTemplate)) return false;
			if (!Object.Equals(WarnSendFailures, v.WarnSendFailures)) return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
