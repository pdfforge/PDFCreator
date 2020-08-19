using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Controller.Routing;
using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.Editions.EditionBase.CreatorTab;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace pdfforge.PDFCreator.Editions.EditionBase
{
    public class ShellManager : IShellManager
    {
        private readonly IRegionManager _regionManager;
        private readonly IWhitelistedServiceLocator _serviceLocator;
        private readonly IWpfTopMostHelper _topMostHelper;
        private readonly IProfileSettingsTabs _profileSettingsTabs;
        private readonly IApplicationSettingsTabs _applicationSettingsTabs;
        private readonly IStartupRoutine _startupRoutine;
        private readonly IEventAggregator _eventAggregator;
        private DateTime _lastOpenWindowTime;
        private bool wasStarted = false;
        private bool _registeredMainShellViews;
        private bool _registeredPrintJobShellViews;
        private List<(string, Type)> _regionToViewRegister;
        private List<(string, Type)> _printJobRegionToViewRegister;

        public ShellManager(IWhitelistedServiceLocator serviceLocator, IRegionManager regionManager, IWpfTopMostHelper topMostHelper, IProfileSettingsTabs profileSettingsTabs, IApplicationSettingsTabs applicationSettingsTabs, IStartupRoutine startupActions, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _serviceLocator = serviceLocator;
            _topMostHelper = topMostHelper;
            _profileSettingsTabs = profileSettingsTabs;
            _applicationSettingsTabs = applicationSettingsTabs;
            _startupRoutine = startupActions;
            _eventAggregator = eventAggregator;
        }

        private void RunStartup()
        {
            var startupActions = _startupRoutine.GetAllActions().Where(action => action is IDataStartupAction);
            foreach (var startupAction in startupActions)
            {
                startupAction.Execute();
            }

            wasStarted = true;
        }

        private MainShell MainShell { get; set; }
        private PrintJobShell PrintJobShell { get; set; }

        public void ShowMainShell()
        {
            lock (this)
            {
                if (!wasStarted)
                    RunStartup();

                if (MainShell == null)
                {
                    MainShell = _serviceLocator.GetInstance<MainShell>();
                    RegionManager.SetRegionManager(MainShell, _regionManager);
                    RegionManager.UpdateRegions();
                    RegisterStartingViewsInMainShellRegions();
                }
            }

            AvoidWpfDesignerDeadlock();

            try
            {
                _topMostHelper.ShowDialogTopMost(MainShell, true);
            }
            finally
            {
                UnregisterRegions(new RegionNames());
                MainShell = null;
            }
        }

        private void AvoidWpfDesignerDeadlock()
        {
            var timeSinceLastWindow = DateTime.Now - _lastOpenWindowTime;

            //TODO This is required to avoid a deadlock in Visual Studio when XAML Diagnostics is enabled
            if (timeSinceLastWindow.TotalMilliseconds < 1500 && Debugger.IsAttached)
                Thread.Sleep(2500);

            _lastOpenWindowTime = DateTime.Now;
        }

        public void ShowPrintJobShell(Job job)
        {
            lock (this)
            {
                if (!wasStarted)
                    RunStartup();

                if (PrintJobShell == null)
                {
                    PrintJobShell = _serviceLocator.GetInstance<PrintJobShell>();
                    RegionManager.SetRegionManager(PrintJobShell, _regionManager);
                    RegionManager.UpdateRegions();
                    RegisterViewsInPrintJobShell();
                }
            }

            AvoidWpfDesignerDeadlock();

            PrintJobShell.InteractiveWorkflowManager.Job = job;
            try
            {
                _topMostHelper.ShowDialogTopMost(PrintJobShell, true);
            }
            finally
            {
                PrintJobShell?.Close();
                UnregisterRegions(new PrintJobRegionNames());
                PrintJobShell = null;
            }
        }

        public void MainShellToFront()
        {
            if (MainShell != null)
            {
                MainShell.Dispatcher.BeginInvoke(new Action(() => _topMostHelper.MakeTopMostWindow(MainShell, true)));
            }
        }

        private void UnregisterRegions(RegionNameCollection regionNameCollection)
        {
            var regions = regionNameCollection.GetRegionNames();
            foreach (var region in regions)
            {
                _regionManager.Regions.Remove(region);
            }
        }

        public void SetPrintJobShellRegionToViewRegister(List<(string, Type)> regionToViewRegister)
        {
            _printJobRegionToViewRegister = regionToViewRegister;
        }

        public void SetMainShellRegionToViewRegister(List<(string, Type)> regionToViewRegister)
        {
            _regionToViewRegister = regionToViewRegister;
        }

        private void RegisterViewsInPrintJobShell()
        {
            if (_registeredPrintJobShellViews)
                return;

            _registeredPrintJobShellViews = true;
            _printJobRegionToViewRegister.ForEach(action: x =>
                _regionManager.RegisterViewWithRegion(x.Item1, x.Item2));
        }

        private void RegisterStartingViewsInMainShellRegions()
        {
            if (_registeredMainShellViews)
                return;

            _registeredMainShellViews = true;
            _regionToViewRegister.ForEach(action: x =>
                _regionManager.RegisterViewWithRegion(x.Item1, x.Item2));

            _profileSettingsTabs.RegisterTabs(_regionManager, _serviceLocator, _eventAggregator);
            _applicationSettingsTabs.RegisterTabs(_regionManager, _serviceLocator, _eventAggregator);
        }
    }
}
