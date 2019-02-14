using MahApps.Metro.Controls;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Events;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using Prism.Events;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public partial class MainShell : MetroWindow, IWhitelisted
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IDispatcher _dispatcher;
        private bool _skipSettingsCheck;
        public IUpdateAssistant UpdateAssistant { get; }

        public MainShellViewModel ViewModel => (MainShellViewModel)DataContext;

        public MainShell(MainShellViewModel vm, IHightlightColorRegistration hightlightColorRegistration, IUpdateAssistant updateAssistant, IEventAggregator eventAggregator, IDispatcher dispatcher)
        {
            _eventAggregator = eventAggregator;
            _dispatcher = dispatcher;
            DataContext = vm;
            UpdateAssistant = updateAssistant;
            InitializeComponent();
            hightlightColorRegistration.RegisterHighlightColorResource(this);
        }

        private void OnTryCloseApplicationEvent()
        {
            _dispatcher.BeginInvoke(Close);
        }

        protected override void OnClosed(EventArgs e)
        {
            ViewModel.OnClosed();
            _eventAggregator.GetEvent<TryCloseApplicationEvent>().Unsubscribe(OnTryCloseApplicationEvent);
            base.OnClosed(e);
        }

        public override void EndInit()
        {
            base.EndInit();
            (DataContext as MainShellViewModel)?.PublishMainShellDone();
        }

        private void MainShell_OnLoaded(object sender, RoutedEventArgs e)
        {
            var desktopSize = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(this).Handle).WorkingArea;
            if (desktopSize.Width < Width || desktopSize.Height < Height)
                WindowState = WindowState.Maximized;

            _eventAggregator.GetEvent<TryCloseApplicationEvent>().Subscribe(OnTryCloseApplicationEvent);
            FocusManager.SetFocusedElement(this, HomeButton);
        }

        private async void MainShell_OnClosing(object sender, CancelEventArgs e)
        {
            if (_skipSettingsCheck)
            {
                return;
            }

            e.Cancel = true;
            _skipSettingsCheck = false;
            var result = await ViewModel.CloseCommand.ExecuteAsync(null);

            if (result == ResponseStatus.Success)
            {
                _skipSettingsCheck = true;
                // Invoke required because we can't call Close during the closing event
                await Dispatcher.BeginInvoke(new Action(Close));
            }
        }
    }
}
