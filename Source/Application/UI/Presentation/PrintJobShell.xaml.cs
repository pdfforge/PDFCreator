using MahApps.Metro.Controls;
using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using Prism.Regions;
using System;
using System.Windows;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Core.SettingsManagement.Customization;
using pdfforge.PDFCreator.UI.Presentation.Events;
using Prism.Events;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public partial class PrintJobShell : MetroWindow, IWhitelisted
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IDispatcher _dispatcher;
        public InteractiveWorkflowManager InteractiveWorkflowManager { get; }

        public PrintJobShell(IRegionManager regionManager, IInteractiveWorkflowManagerFactory interactiveWorkflowManagerFactory, PrintJobShellViewModel viewModel,
            ICurrentSettingsProvider currentSettingsProvider, ViewCustomization viewCustomization, IEventAggregator eventAggregator, IDispatcher dispatcher)
        {
            _eventAggregator = eventAggregator;
            _dispatcher = dispatcher;
            DataContext = viewModel;
            InitializeComponent();
            InteractiveWorkflowManager = interactiveWorkflowManagerFactory.CreateInteractiveWorkflowManager(regionManager, currentSettingsProvider);
            Closing += (sender, args) => InteractiveWorkflowManager.Cancel = true;

            if (viewCustomization.CustomizationEnabled)
            {
                Title = viewCustomization.PrintJobWindowCaption;
            }
        }

        private void OnTryCloseApplication()
        {
            _dispatcher.BeginInvoke(Close);
        }

        private async void PrintJobShell_OnLoaded(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<TryCloseApplicationEvent>().Subscribe(OnTryCloseApplication);
            await InteractiveWorkflowManager.Run();
            await Dispatcher.BeginInvoke(new Action(Close));
        }

        private void PrintJobShell_OnClosed(object sender, EventArgs e)
        {
            _eventAggregator.GetEvent<TryCloseApplicationEvent>().Unsubscribe(OnTryCloseApplication);
        }
    }
}
