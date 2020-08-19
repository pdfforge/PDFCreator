using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.UserGuide
{
    public class ShowUserGuideCommand : IInitializedCommand<HelpTopic>
    {
        private readonly IUserGuideHelper _userGuideHelper;
        private HelpTopic _helpTopic = HelpTopic.General;

        public ShowUserGuideCommand(IUserGuideHelper userGuideHelper)
        {
            _userGuideHelper = userGuideHelper;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is HelpTopic)
            {
                _userGuideHelper.ShowHelp((HelpTopic)parameter);
            }
            else
            {
                _userGuideHelper.ShowHelp(_helpTopic);
            }
        }

        public void Init(HelpTopic parameter)
        {
            _helpTopic = parameter;
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
