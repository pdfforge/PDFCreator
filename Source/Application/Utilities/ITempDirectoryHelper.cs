namespace pdfforge.PDFCreator.Utilities
{
    public interface ITempDirectoryHelper
    {
        void CleanUp();

        string CreateTestFileDirectory();
    }
}
