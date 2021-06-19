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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public partial class MainShell : MetroWindow, IWhitelisted
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IDispatcher _dispatcher;
        private bool _skipSettingsCheck;
        public IUpdateHelper UpdateHelper { get; }

        public MainShellViewModel ViewModel => (MainShellViewModel)DataContext;

        public MainShell(MainShellViewModel vm, IHightlightColorRegistration hightlightColorRegistration, IUpdateHelper updateHelper, IEventAggregator eventAggregator, IDispatcher dispatcher)
        {
            _eventAggregator = eventAggregator;
            _dispatcher = dispatcher;
            DataContext = vm;
            UpdateHelper = updateHelper;
            InitializeComponent();
            vm.Init(Close);
            hightlightColorRegistration.RegisterHighlightColorResource(this);
            TransposerHelper.Register(this, vm);
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
            var desktopScaling = Screen.PrimaryScreen.Bounds.Width / SystemParameters.PrimaryScreenWidth;
            var desktopSize = Screen.FromHandle(new WindowInteropHelper(this).Handle).WorkingArea;
            var scaledWidth = desktopSize.Width / desktopScaling;
            var scaledHeight = desktopSize.Height / desktopScaling;

            if (scaledWidth < Width || scaledHeight < Height)
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
