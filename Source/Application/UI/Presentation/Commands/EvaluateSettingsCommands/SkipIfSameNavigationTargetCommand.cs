using pdfforge.PDFCreator.Core.Services.Macros;
using Prism.Regions;
using System;
using System.Linq;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.EvaluateSettingsCommands
{
    public class SkipIfSameNavigationTargetCommand : IWaitableCommand
    {
        private readonly IRegionManager _regionManager;

        public SkipIfSameNavigationTargetCommand(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        //public set to overwrite MainRegionName for PDFCreator Server
        public string MainRegionName { get; set; } = RegionNames.MainRegion;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var target = parameter as string;
            if (target == null || !_regionManager.Regions.Any(r => r.Name == MainRegionName))
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
                return;
            }

            var region = _regionManager.Regions[MainRegionName];
            var activeViewInMainRegion = region.ActiveViews.FirstOrDefault();
            if (activeViewInMainRegion?.GetType().Name == target)
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
                return;
            }

            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
