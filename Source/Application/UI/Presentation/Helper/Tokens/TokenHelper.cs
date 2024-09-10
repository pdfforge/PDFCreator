using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Tokens
{
    public interface ITokenHelper
    {
        TokenReplacer TokenReplacerWithPlaceHolders { get; }

        List<string> GetTokenListWithFormatting();

        List<string> GetTokenListForMetadata();

        List<string> GetTokenListForFilename();

        List<string> GetTokenListForDirectory();

        List<string> GetTokenListForExternalFiles();

        List<string> GetTokenListForStamp();

        List<string> GetTokenListForPageNumbers();

        List<string> GetTokenListForEmail();

        List<string> GetTokenListForEmailRecipients();

        /// <summary>
        ///     Detection if UserTokens need to be enabled
        ///     or if string contains insecure tokens, like NumberOfPages, InputFilename or InputFilePath/InputDirectory
        /// </summary>
        TokenWarningCheckResult TokenWarningCheck(string textWithTokens, ConversionProfile profile);
    }

    internal static class TokenListExtension
    {
        public static void RemoveToken(this List<string> tokenList, string tokenName)
        {
            tokenList.Remove("<" + tokenName + ">");
        }
    }

    public class TokenHelper : ITokenHelper
    {
        private TokenPlaceHoldersTranslation _translation;
        private TokenReplacer _tokenReplacer;
        private const string UserTokenKey = "NameDefinedByUser";

        public TokenHelper(ITranslationUpdater translationUpdater)
        {
            translationUpdater.RegisterAndSetTranslation(tf =>
            {
                _translation = tf.UpdateOrCreateTranslation(_translation);
                _tokenReplacer = null;
            });
        }

        public TokenReplacer TokenReplacerWithPlaceHolders
        {
            get { return _tokenReplacer ?? (_tokenReplacer = CreateTokenReplacerWithPlaceHolders()); }
        }

        protected virtual TokenReplacer CreateTokenReplacerWithPlaceHolders()
        {
            var tr = CreateTokenReplacerWithPlaceHoldersBase();

            tr.AddToken(new StringToken("Username", Environment.UserName));
            tr.AddToken(new StringToken("Desktop", Environment.GetFolderPath(Environment.SpecialFolder.Desktop)));
            tr.AddToken(new StringToken("MyDocuments", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)));
            tr.AddToken(new StringToken("MyPictures", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)));

            return tr;
        }

        protected TokenReplacer CreateTokenReplacerWithPlaceHoldersBase()
        {
            var tr = new TokenReplacer();

            tr.AddToken(new StringToken("Author", Environment.UserName));
            tr.AddToken(new StringToken("PrintJobAuthor", Environment.UserName));
            tr.AddToken(new StringToken("ClientComputer", Environment.MachineName));
            tr.AddToken(new StringToken("ComputerName", Environment.MachineName));
            tr.AddToken(new NumberToken("Counter", 1234));
            tr.AddToken(new DateToken("DateTime", DateTime.Now));
            tr.AddToken(new StringToken("InputFilename", _translation.InputFilename));
            tr.AddToken(new StringToken("InputDirectory", @"C:\Temp"));
            tr.AddToken(new NumberToken("JobID", 1));
            tr.AddToken(new NumberToken("PageNumber", 1));
            tr.AddToken(new NumberToken("NumberOfCopies", 1));
            tr.AddToken(new NumberToken("NumberOfPages", 1));
            tr.AddToken(new ListToken("OutputFilenames",
                new[]
                {
                    _translation.OutputFilename,
                    _translation.OutputFilename2,
                    _translation.OutputFilename3
                }));
            tr.AddToken(new StringToken("OutputFilePath", @"C:\Temp"));
            tr.AddToken(new StringToken("PrinterName", "PDFCreator"));
            tr.AddToken(new NumberToken("SessionID", 0));
            tr.AddToken(new StringToken("Title", _translation.TitleFromSettings));
            tr.AddToken(new StringToken("PrintJobName", _translation.TitleFromPrintJob));
            tr.AddToken(new StringToken("Subject", _translation.SubjectFromSettings));
            tr.AddToken(new StringToken("Keywords", _translation.KeywordsFromSettings));
            tr.AddToken(new StringToken(TokenNames.DropboxHtmlLinks, "<a href=\"https://dropbox.com/link1\">File.pdf</a>"));
            tr.AddToken(new StringToken(TokenNames.DropboxFullLinks, "File.pdf (https://dropbox.com/link1)"));
            tr.AddToken(new StringToken(TokenNames.OneDriveShareLink, "https://1drv.ms/link"));
            tr.AddToken(new StringToken(TokenNames.OneDriveShareLinkHtml, "<a href=\"https://1drv.ms/link\">https://1drv.ms/link1</a>"));
            tr.AddToken(new EnvironmentToken());
            tr.AddToken(new ParameterPreviewToken("User", _translation.FormatTokenPreviewText));

            return tr;
        }

        public List<string> GetTokenListWithFormatting()
        {
            var tokenList = new List<string>();
            tokenList.AddRange(TokenReplacerWithPlaceHolders.GetTokenNames());

            tokenList.Sort();

            tokenList.Insert(tokenList.IndexOf("<DateTime>") + 1, "<DateTime:yyyyMMddHHmmss>");
            tokenList.Insert(tokenList.IndexOf("<DateTime>"), "<DateTime: >");
            tokenList.Remove("<DateTime>");
            tokenList.Insert(tokenList.IndexOf("<Environment>") + 1, "<Environment:UserName>");
            tokenList.Insert(tokenList.IndexOf("<Environment>"), "<Environment: >");
            tokenList.Remove("<Environment>");
            tokenList.Insert(tokenList.IndexOf("<User>") + 1, $"<User:{UserTokenKey}>");
            tokenList.Insert(tokenList.IndexOf("<User>"), "<User: >");
            tokenList.Remove("<User>");
            tokenList.Remove("<PageNumber>");

            return tokenList;
        }

        public List<string> GetTokenListForMetadata()
        {
            var tokenList = GetTokenListWithFormatting();

            tokenList.Remove("<Author>");
            tokenList.Remove("<Title>");
            tokenList.Remove("<OutputFilenames>");
            tokenList.Remove("<InputDirectory>");
            tokenList.Remove("<OutputFilePath>");
            tokenList.Remove("<OutputFilenames>");
            tokenList.Remove("<Subject>");
            tokenList.Remove("<Keywords>");
            tokenList.RemoveToken(TokenNames.DropboxHtmlLinks);
            tokenList.RemoveToken(TokenNames.DropboxFullLinks);
            tokenList.RemoveToken(TokenNames.OneDriveShareLink);
            tokenList.RemoveToken(TokenNames.OneDriveShareLinkHtml);
            tokenList.Remove("<PageNumber>");

            return tokenList;
        }

        public List<string> GetTokenListForFilename()
        {
            var tokenList = GetTokenListWithFormatting();

            tokenList.Remove("<OutputFilenames>");
            tokenList.Remove("<InputDirectory>");
            tokenList.Remove("<OutputFilePath>");
            tokenList.RemoveToken(TokenNames.DropboxHtmlLinks);
            tokenList.RemoveToken(TokenNames.DropboxFullLinks);
            tokenList.RemoveToken(TokenNames.OneDriveShareLink);
            tokenList.RemoveToken(TokenNames.OneDriveShareLinkHtml);
            tokenList.Remove("<PageNumber>");

            return tokenList;
        }

        public List<string> GetTokenListForDirectory()

        {
            var tokenList = GetTokenListWithFormatting();
            tokenList.Remove("<OutputFilePath>");
            tokenList.RemoveToken(TokenNames.DropboxHtmlLinks);
            tokenList.RemoveToken(TokenNames.DropboxFullLinks);
            tokenList.RemoveToken(TokenNames.OneDriveShareLink);
            tokenList.RemoveToken(TokenNames.OneDriveShareLinkHtml);
            tokenList.Remove("<PageNumber>");

            return tokenList;
        }

        public List<string> GetTokenListForExternalFiles()

        {
            var tokenList = GetTokenListWithFormatting();
            tokenList.Remove("<Author>");
            tokenList.Remove("<OutputFilePath>");
            tokenList.RemoveToken(TokenNames.DropboxHtmlLinks);
            tokenList.RemoveToken(TokenNames.DropboxFullLinks);
            tokenList.RemoveToken(TokenNames.OneDriveShareLink);
            tokenList.RemoveToken(TokenNames.OneDriveShareLinkHtml);
            tokenList.Remove("<Counter>");
            tokenList.Remove("<JobID>");
            tokenList.Remove("<Keywords>");
            tokenList.Remove("<NumberOfCopies>");
            tokenList.Remove("<NumberOfPages>");
            tokenList.Remove("<OutputFilePath>");
            tokenList.Remove("<OutputFilenames>");
            tokenList.Remove("<PrinterName>");
            tokenList.Remove("<PrintJobAuthor>");
            tokenList.Remove("<SessionID>");
            tokenList.Remove("<Title>");
            tokenList.Remove("<PrintJobName>");
            tokenList.Remove("<Subject>");
            tokenList.Remove("<PageNumber>");

            return tokenList;
        }

        public List<string> GetTokenListForStamp()

        {
            var tokenList = GetTokenListWithFormatting();
            tokenList.RemoveToken(TokenNames.DropboxHtmlLinks);
            tokenList.RemoveToken(TokenNames.DropboxFullLinks);
            tokenList.RemoveToken(TokenNames.OneDriveShareLink);
            tokenList.RemoveToken(TokenNames.OneDriveShareLinkHtml);
            tokenList.Remove("<PageNumber>");
            return tokenList;
        }

        public List<string> GetTokenListForPageNumbers()
        {
            var tokenList = new List<string>
            {
                "<Author>",
                "<DateTime: >",
                "<DateTime:yyyyMMddHHmmss>",
                "<NumberOfPages>",
                "<PrinterName>",
                "<PrintJobAuthor>",
                "<PrintJobName>",
                "<PageNumber>",
                "<PageNumber>/<NumberOfPages>",
                "<Title>",
                "<User: >",
                $"<User:{UserTokenKey}>"
            };
            tokenList.Sort();
            return tokenList;
        }

        public List<string> GetTokenListForEmail()
        {
            var tokenList = GetTokenListWithFormatting();

            tokenList.Insert(tokenList.IndexOf("<OutputFilePath>") + 1, "<OutputFilenames:, >");
            tokenList.Insert(tokenList.IndexOf("<OutputFilePath>") + 2, "<OutputFilenames:\\r\\n>");

            tokenList.Remove("<OutputFilePath>");
            tokenList.Remove("<PageNumber>");
            return tokenList;
        }

        public List<string> GetTokenListForEmailRecipients()
        {
            var tokenList = new List<string>();
            tokenList.Add("<Author>");
            tokenList.Add("<ClientComputer>");
            tokenList.Add("<ComputerName>");
            tokenList.Add("<Environment: >");
            tokenList.Add("<Environment:UserName>");
            tokenList.Add("<PrintJobAuthor>");
            tokenList.Add("<User: >");
            tokenList.Add($"<User:{UserTokenKey}>");
            tokenList.Add("<Username>");
            return tokenList;
        }

        /// <summary>
        ///     Detection if string contains insecure tokens, like NumberOfPages, InputFilename, InputFilePath/InputDirectory
        /// </summary>
        private bool ContainsInsecureTokens(string textWithTokens)
        {
            if (Contains_IgnoreCase(textWithTokens, "<NumberOfPages>"))
                return true;
            if (Contains_IgnoreCase(textWithTokens, "<InputFilename>"))
                return true;
            if (Contains_IgnoreCase(textWithTokens, "<InputFilePath>"))
                return true;
            if (Contains_IgnoreCase(textWithTokens, "<InputDirectory>"))
                return true;
            return false;
        }

        public TokenWarningCheckResult TokenWarningCheck(string textWithTokens, ConversionProfile profile)
        {
            if (!profile.UserTokens.Enabled && TokenIdentifier.ContainsUserToken(textWithTokens))
                return TokenWarningCheckResult.RequiresEnablingUserTokens;

            if (ContainsInsecureTokens(textWithTokens))
                return TokenWarningCheckResult.ContainsInsecureTokens;

            return TokenWarningCheckResult.NoWarning;
        }

        private bool Contains_IgnoreCase(string source, string value)
        {
            return source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }

    public enum TokenWarningCheckResult
    {
        NoWarning,
        ContainsInsecureTokens,
        RequiresEnablingUserTokens
    }
}
