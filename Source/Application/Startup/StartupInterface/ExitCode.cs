namespace pdfforge.PDFCreator.Core.StartupInterface
{
    public enum ExitCode
    {
        Ok = 0,
        Unknown = 1,
        NoTranslations = 11,
        GhostScriptNotFound = 12,
        PrintersBroken = 13,
        StartupConditionFailed = 14,
        NotValidOnTerminalServer = 21,
        LicenseInvalidAndNotReactivated = 22,
        LicenseInvalidAfterReactivation = 23,
        LicenseInvalidAndHiddenWithGpo = 24,
        TrialExpired = 25,
        SpoolFolderInaccessible = 31,
        SpoolerNotRunning = 32,
        InvalidSettingsFile = 41,
        InvalidSettingsInGivenFile = 42,
        ErrorWhileSavingDefaultSettings = 43,
        ErrorWhileManagingPrintJobs = 51,
        PrintFileParameterHasNoArgument = 61,
        PrintFileDoesNotExist = 62,
        PrintFileNotPrintable = 63,
        PrintFileCouldNotBePrinted = 64,
        MissingActivation = 71,
        NoAccessPrivileges = 72,
        InvalidPdfToolsSecureLicense = 81,
        InvalidPdfToolsPdf2PdfLicense = 82,
        InvalidPdfToolsDocumentLicense = 83,
        InvalidPdfAValidatorLicense = 84
    }
}
