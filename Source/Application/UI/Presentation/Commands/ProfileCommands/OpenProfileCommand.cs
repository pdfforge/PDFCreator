using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Controller.Routing;
using pdfforge.PDFCreator.UI.Presentation.Routing;
using System;
using System.Linq;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.ProfileCommands
{
    public class OpenProfileCommand : ICommand
    {
        private readonly IMainWindowThreadLauncher _mainWindowThreadLauncher;
        private readonly IStartupRoutine _startupRoutine;

        public OpenProfileCommand(IMainWindowThreadLauncher mainWindowThreadLauncher, IStartupRoutine startupRoutine)
        {
            _mainWindowThreadLauncher = mainWindowThreadLauncher;
            _startupRoutine = startupRoutine;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var navigationAction = _startupRoutine.GetActionByType<StartupNavigationAction>().FirstOrDefault();
            navigationAction.Target = RegionNames.ProfilesView;
            _mainWindowThreadLauncher.LaunchMainWindow();
        }

#pragma warning disable 67

        public event EventHandler CanExecuteChanged;

#pragma warning restore 67
    }
}
