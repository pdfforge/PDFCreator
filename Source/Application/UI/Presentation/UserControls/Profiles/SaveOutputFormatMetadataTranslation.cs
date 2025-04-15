﻿using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class SaveOutputFormatMetadataTranslation : ITranslatable
    {
        public string Save { get; set; } = "Save";

        public string Interactive { get; private set; } = "Interactive";
        public string Autosave { get; private set; } = "Automatic";
        public string FilenameLabel { get; private set; } = "Filename:";
        public string TargetDirectoryLabel { get; private set; } = "Target directory:";

        public string LastUsedDirectory { get; protected set; } = "Last used directory";
        public string SaveOnlyTemporary { get; private set; } = "Only save file temporarily";
        public string MissingDirectory { get; protected set; } = "Missing directory";
        public string ShowQuickActions { get; private set; } = "Show quick actions";
        public string SkipPrintDialog { get; private set; } = "Skip job dialog";
        public string SkipSendFailures { get; private set; } = "Skip send failures";
        public string ShowTrayNotification { get; private set; } = "Show tray notification";

        public string ExistingFileLabel { get; private set; } = "Behavior for existing file:";
        public string BehaviorMerge { get; private set; } = "Merge";
        public string BehaviorOverwrite { get; private set; } = "Overwrite";
        public string BehaviorUnique { get; private set; } = "Ensure unique filenames";
        public string BehaviorMergeBeforeModifyActions { get; private set; } = "Merge before the actions";

        public string NonPdfAutoMerge { get; private set; } = "Non-pdf auto-merge";

        public string OutputFormat { get; private set; } = "Output format";
        public string ColorsLabel { get; private set; } = "Colors:";
        public string ResolutionLabel { get; private set; } = "Resolution:";
        public string CompressionLabel { get; private set; } = "Compression:";

        public string MetaData { get; set; } = "Metadata";
        public string TitleLabel { get; protected set; } = "Title:";
        public string AuthorLabel { get; protected set; } = "Author:";
        public string SubjectLabel { get; protected set; } = "Subject:";
        public string KeywordsLabel { get; protected set; } = "Keywords:";

        public EnumTranslation<ColorModel>[] PdfColorValues { get; set; } = EnumTranslation<ColorModel>.CreateDefaultEnumTranslation();
        public EnumTranslation<JpegColor>[] JpegColorValues { get; set; } = EnumTranslation<JpegColor>.CreateDefaultEnumTranslation();
        public EnumTranslation<PngColor>[] PngColorValues { get; set; } = EnumTranslation<PngColor>.CreateDefaultEnumTranslation();
        public EnumTranslation<TiffColor>[] TiffColorValues { get; set; } = EnumTranslation<TiffColor>.CreateDefaultEnumTranslation();

        public EnumTranslation<CompressionColorAndGray>[] CompressionValues { get; set; } = EnumTranslation<CompressionColorAndGray>.CreateDefaultEnumTranslation();
    }
}
