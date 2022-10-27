namespace pdfforge.PDFCreator.Conversion.Jobs.FolderProvider
{
    public interface ITempFolderProvider
    {
        string TempFolder { get; }

        string CreatePrefixTempFolder(string prefix);
    }
}
