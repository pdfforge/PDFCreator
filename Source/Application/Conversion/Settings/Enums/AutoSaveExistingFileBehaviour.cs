namespace pdfforge.PDFCreator.Conversion.Settings.Enums
{
    public enum AutoSaveExistingFileBehaviour
    {
        /// <summary>
        /// Existing files will not be overwritten. Existing files automatically get append with the new files.
        /// </summary>
        Merge,
        /// <summary>
        /// Existing files will not be overwritten. Existing filenames automatically get an appendix.
        /// </summary>
        EnsureUniqueFilenames,
        /// <summary>
        /// Execute the modify actions on the already merged files
        /// </summary>
        MergeBeforeModifyActions,
        Overwrite
    }
}
