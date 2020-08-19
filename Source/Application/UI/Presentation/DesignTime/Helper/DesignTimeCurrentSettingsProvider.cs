using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper
{
    public class DesignTimeCurrentSettingsProvider : ICurrentSettingsProvider
    {
        public DesignTimeCurrentSettingsProvider()
        {
            Settings = new PdfCreatorSettings();
            Settings.ConversionProfiles.Add(new ConversionProfile());

            Settings.ApplicationSettings.TitleReplacement.AddRange(new[]
            {
                new TitleReplacement(ReplacementType.Start, "start1", ""),
                new TitleReplacement(ReplacementType.Start, "start2", ""),
                new TitleReplacement(ReplacementType.Start, "start3", ""),
                new TitleReplacement(ReplacementType.Start, "start4", ""),
                new TitleReplacement(ReplacementType.Start, "start5", ""),
                new TitleReplacement(ReplacementType.End, "end1", ""),
                new TitleReplacement(ReplacementType.End, "end2", ""),
                new TitleReplacement(ReplacementType.End, "end3", ""),
                new TitleReplacement(ReplacementType.End, "end4", ""),
                new TitleReplacement(ReplacementType.End, "end5", ""),
                new TitleReplacement(ReplacementType.Replace, "all1", ""),
                new TitleReplacement(ReplacementType.Replace, "all2", ""),
                new TitleReplacement(ReplacementType.Replace, "all3", ""),
                new TitleReplacement(ReplacementType.Replace, "all4", ""),
                new TitleReplacement(ReplacementType.Replace, "all5", ""),
                new TitleReplacement(ReplacementType.RegEx, "regex5", ""),
                new TitleReplacement(ReplacementType.RegEx, "regex5", ""),
                new TitleReplacement(ReplacementType.RegEx, "regex5", ""),
                new TitleReplacement(ReplacementType.RegEx, "regex5", ""),
                new TitleReplacement(ReplacementType.RegEx, "regex5", ""),
            });

            SelectedProfile = Settings.ConversionProfiles.First();
            Profiles = new ObservableCollection<ConversionProfile>(Settings.ConversionProfiles);
        }

        public ConversionProfile SelectedProfile { get; set; }
        public ObservableCollection<ConversionProfile> Profiles { get; }

        public ConversionProfile GetProfileByName(string name)
        {
            return SelectedProfile;
        }

        public PdfCreatorSettings Settings { get; }
        public Accounts Accounts => Settings.ApplicationSettings.Accounts;
        public ObservableCollection<TitleReplacement> TitleReplacements => Settings.ApplicationSettings.TitleReplacement;
        public ObservableCollection<DefaultViewer> DefaultViewers => Settings.DefaultViewers;
        public ObservableCollection<PrinterMapping> PrinterMappings => Settings.ApplicationSettings.PrinterMappings;

        public void StoreCurrentSettings()
        {
        }

        public void Reset()
        {
        }

#pragma warning disable CS0067

        public event EventHandler SettingsChanged;
        public int GetRegisteredSelectedProfileChangedListener()
        {
            return 0;
        }

        public event PropertyChangedEventHandler SelectedProfileChanged;

#pragma warning restore CS0067
    }
}
