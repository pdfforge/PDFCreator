namespace pdfforge.PDFCreator.Conversion.Settings.GroupPolicies
{
    public interface IGpoSettings
    {
        bool DisableApplicationSettings { get; }

        bool DisableDebugTab { get; }

        bool DisablePrinterTab { get; }

        bool DisableProfileManagement { get; }

        bool DisableTitleTab { get; }

        bool DisableHistory { get; }

        bool DisableAccountsTab { get; }
        bool DisableUsageStatistics { get; }

        bool DisableRssFeed { get; }

        bool DisableTips { get; }

        bool HideLicenseTab { get; }

        bool HidePdfArchitectInfo { get; }

        string Language { get; }

        string UpdateInterval { get; }

        bool LoadSharedAppSettings { get; }
        bool LoadSharedProfiles { get; }
        bool AllowUserDefinedProfiles { get; }

        bool DisableLicenseExpirationReminder { get; }
    };
}

