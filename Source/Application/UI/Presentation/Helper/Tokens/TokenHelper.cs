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

            tr.AddToken(new StringToken(TokenNames.Username, Environment.UserName));
            tr.AddToken(new StringToken(TokenNames.Desktop, Environment.GetFolderPath(Environment.SpecialFolder.Desktop)));
            tr.AddToken(new StringToken(TokenNames.MyDocuments, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)));
            tr.AddToken(new StringToken(TokenNames.MyPictures, Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)));

            return tr;
        }

        protected TokenReplacer CreateTokenReplacerWithPlaceHoldersBase()
        {
            var tr = new TokenReplacer();

            tr.AddToken(new StringToken(TokenNames.Author, Environment.UserName));
            tr.AddToken(new StringToken(TokenNames.PrintJobAuthor, Environment.UserName));
            tr.AddToken(new StringToken(TokenNames.ClientComputer, Environment.MachineName));
            tr.AddToken(new StringToken(TokenNames.ComputerName, Environment.MachineName));
            tr.AddToken(new NumberToken(TokenNames.Counter, 1234));
            tr.AddToken(new DateToken(TokenNames.DateTime, DateTime.Now));
            tr.AddToken(new StringToken(TokenNames.InputFilename, _translation.InputFilename));
            tr.AddToken(new StringToken(TokenNames.InputDirectory, @"C:\Temp"));
            tr.AddToken(new NumberToken(TokenNames.JobId, 1));
            tr.AddToken(new NumberToken(TokenNames.PageNumber, 1));
            tr.AddToken(new NumberToken(TokenNames.NumberOfCopies, 1));
            tr.AddToken(new NumberToken(TokenNames.NumberOfPages, 1));
            tr.AddToken(new ListToken(TokenNames.OutputFilenames,
                new[]
                {
                    _translation.OutputFilename,
                    _translation.OutputFilename2,
                    _translation.OutputFilename3
                }));
            tr.AddToken(new StringToken(TokenNames.OutputFilePath, @"C:\Temp"));
            tr.AddToken(new StringToken(TokenNames.PrinterName, "PDFCreator"));
            tr.AddToken(new NumberToken(TokenNames.SessionId, 0));
            tr.AddToken(new StringToken(TokenNames.Title, _translation.TitleFromSettings));
            tr.AddToken(new StringToken(TokenNames.PrintJobName, _translation.TitleFromPrintJob));
            tr.AddToken(new StringToken(TokenNames.Subject, _translation.SubjectFromSettings));
            tr.AddToken(new StringToken(TokenNames.Keywords, _translation.KeywordsFromSettings));
            tr.AddToken(new StringToken(TokenNames.DropboxHtmlLinks, "<a href=\"https://dropbox.com/link1\">File.pdf</a>"));
            tr.AddToken(new StringToken(TokenNames.DropboxFullLinks, "File.pdf (https://dropbox.com/link1)"));
            tr.AddToken(new StringToken(TokenNames.OneDriveShareLink, "https://1drv.ms/link"));
            tr.AddToken(new StringToken(TokenNames.OneDriveShareLinkHtml, "<a href=\"https://1drv.ms/link\">https://1drv.ms/link1</a>"));
            tr.AddToken(new EnvironmentToken());
            tr.AddToken(new ParameterPreviewToken(TokenNames.User, _translation.FormatTokenPreviewText));

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

            tokenList.RemoveToken("<" + TokenNames.Author + ">");
            tokenList.RemoveToken("<" + TokenNames.Title + ">");
            tokenList.RemoveToken("<" + TokenNames.OutputFilenames + ">");
            tokenList.RemoveToken("<" + TokenNames.InputDirectory + ">");
            tokenList.RemoveToken("<" + TokenNames.OutputFilePath + ">");
            tokenList.RemoveToken("<" + TokenNames.OutputFilenames + ">");
            tokenList.RemoveToken("<" + TokenNames.Subject + ">");
            tokenList.RemoveToken("<" + TokenNames.Keywords + ">");
            tokenList.RemoveToken("<" + TokenNames.DropboxHtmlLinks + ">");
            tokenList.RemoveToken("<" + TokenNames.DropboxFullLinks + ">");
            tokenList.RemoveToken("<" + TokenNames.OneDriveShareLink + ">");
            tokenList.RemoveToken("<" + TokenNames.OneDriveShareLinkHtml + ">");
            tokenList.RemoveToken("<" + TokenNames.PageNumber + ">");

            return tokenList;
        }

        public List<string> GetTokenListForFilename()
        {
            var tokenList = GetTokenListWithFormatting();

            tokenList.RemoveToken("<" + TokenNames.OutputFilenames + ">");
            tokenList.RemoveToken("<" + TokenNames.InputDirectory + ">");
            tokenList.RemoveToken("<" + TokenNames.OutputFilePath + ">");
            tokenList.RemoveToken("<" + TokenNames.DropboxHtmlLinks + ">");
            tokenList.RemoveToken("<" + TokenNames.DropboxFullLinks + ">");
            tokenList.RemoveToken("<" + TokenNames.OneDriveShareLink + ">");
            tokenList.RemoveToken("<" + TokenNames.OneDriveShareLinkHtml + ">");
            tokenList.RemoveToken("<" + TokenNames.PageNumber + ">");

            return tokenList;
        }

        public List<string> GetTokenListForDirectory()

        {
            var tokenList = GetTokenListWithFormatting();
            tokenList.RemoveToken("<" + TokenNames.OutputFilePath + ">");
            tokenList.RemoveToken("<" + TokenNames.DropboxHtmlLinks + ">");
            tokenList.RemoveToken("<" + TokenNames.DropboxFullLinks + ">");
            tokenList.RemoveToken("<" + TokenNames.OneDriveShareLink + ">");
            tokenList.RemoveToken("<" + TokenNames.OneDriveShareLinkHtml + ">");
            tokenList.RemoveToken("<" + TokenNames.PageNumber + ">");

            return tokenList;
        }

        public List<string> GetTokenListForExternalFiles()

        {
            var tokenList = GetTokenListWithFormatting();
            tokenList.RemoveToken("<" + TokenNames.Author + ">");
            tokenList.RemoveToken("<" + TokenNames.OutputFilePath + ">");
            tokenList.RemoveToken("<" + TokenNames.DropboxHtmlLinks + ">");
            tokenList.RemoveToken("<" + TokenNames.DropboxFullLinks + ">");
            tokenList.RemoveToken("<" + TokenNames.OneDriveShareLink + ">");
            tokenList.RemoveToken("<" + TokenNames.OneDriveShareLinkHtml + ">");
            tokenList.RemoveToken("<" + TokenNames.Counter + ">");
            tokenList.RemoveToken("<" + TokenNames.JobId + ">");
            tokenList.RemoveToken("<" + TokenNames.Keywords + ">");
            tokenList.RemoveToken("<" + TokenNames.NumberOfCopies + ">");
            tokenList.RemoveToken("<" + TokenNames.NumberOfPages + ">");
            tokenList.RemoveToken("<" + TokenNames.OutputFilePath + ">");
            tokenList.RemoveToken("<" + TokenNames.OutputFilenames + ">");
            tokenList.RemoveToken("<" + TokenNames.PrinterName + ">");
            tokenList.RemoveToken("<" + TokenNames.PrintJobAuthor + ">");
            tokenList.RemoveToken("<" + TokenNames.SessionId + ">");
            tokenList.RemoveToken("<" + TokenNames.Title + ">");
            tokenList.RemoveToken("<" + TokenNames.PrintJobName + ">");
            tokenList.RemoveToken("<" + TokenNames.Subject + ">");
            tokenList.RemoveToken("<" + TokenNames.PageNumber + ">");

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
