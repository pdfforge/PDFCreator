﻿using System;
using pdfforge.PDFCreator.Core.ServiceLocator;

namespace pdfforge.PDFCreator.Core.SettingsManagement.Customization
{
    public class ViewCustomization : IWhitelisted
    {
        private ViewCustomization()
        {
        }

        public void ApplyCustomization(string mainWindowText, string printJobWindowCaption)
        {
            CustomizationEnabled = true;
            MainWindowText = mainWindowText;
            PrintJobWindowCaption = printJobWindowCaption;
        }

        public void ApplyTrial(string trialExpireDate)
        {
            var trialExpireDateDecrypted = DataStorage.Data.Decrypt(trialExpireDate);
            if (DateTime.TryParse(trialExpireDateDecrypted, out var dateTime))
            {
                TrialEnabled = true;
                TrialExpireDateTime = dateTime;
                TrialExpireDateString = TrialExpireDateTime.ToString("yyyy-MM-dd");
                TrialText = $"(Trial {TrialExpireDateString})";
            }
        }

        public static ViewCustomization DefaultCustomization => new ViewCustomization();

        public bool CustomizationEnabled { get; private set; } = false;

        public string MainWindowText { get; private set; } = "";

        public string PrintJobWindowCaption { get; private set; } = "PDFCreator";

        public bool TrialEnabled { get; private set; } = false;
        public string TrialText { get; private set; } = "";
        public string TrialExpireDateString { get; private set; } = "";
        public DateTime TrialExpireDateTime { get; private set; }
    }
}
