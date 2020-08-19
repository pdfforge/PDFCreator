using NLog;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    public class DropboxAccountViewModel : AccountViewModelBase<DropboxAccountInteraction, DropboxTranslation>
    {
        private readonly IDropboxService _dropboxService;
        private readonly DropboxAppData _dropboxData;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public DropboxAccountViewModel(IDropboxService dropboxService, DropboxAppData dropboxAppData, ITranslationUpdater translationUpdater)
            : base(translationUpdater)
        {
            _dropboxService = dropboxService;
            _dropboxData = dropboxAppData;
        }

        public override string Title => Translation.AddDropboxAccount;

        protected override void SaveExecute()
        {
            //not required here
        }

        protected override bool SaveCanExecute()
        {
            //not required here
            return false;
        }

        public void SetAccessTokenAndUserInfo(Uri eUri)
        {
            // if url doesnt start with redirecturl that means that current url for recieveing token isn't reach yet
            if (eUri.ToString().StartsWith(_dropboxData.RedirectUri, StringComparison.OrdinalIgnoreCase) == false)
                return;

            try
            {
                var accessToken = _dropboxService.ParseAccessToken(eUri);
                var currentUserInfo = _dropboxService.GetDropUserInfo(accessToken);
                var newDropboxAccount = new DropboxAccount
                {
                    AccessToken = currentUserInfo.AccessToken,
                    AccountInfo = currentUserInfo.AccountInfo,
                    AccountId = currentUserInfo.AccountId
                };
                Interaction.DropboxAccount = newDropboxAccount;
                Interaction.Result = DropboxAccountInteractionResult.Success;
            }
            catch (ArgumentException)
            {
                Interaction.Result = DropboxAccountInteractionResult.AccesTokenParsingError;
                _logger.Warn("Cannot parse dropbox access token. New Account can't be created.");
            }
            catch (Exception e)
            {
                Interaction.Result = DropboxAccountInteractionResult.Error;
                _logger.Warn($"Unexpected exception during determination of dropbox token.{Environment.NewLine}{e}");
            }

            FinishInteraction();
        }

        public Uri GetAuthorizeUri()
        {
            return _dropboxService.GetAuthorizeUri(_dropboxData.AppKey, _dropboxData.RedirectUri);
        }

        protected override void ClearPassword()
        {
        }
    }
}
