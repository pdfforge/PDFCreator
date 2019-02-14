using NLog;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Presentation.Commands.QuickActions;
using pdfforge.PDFCreator.Utilities.Threading;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using Translatable;

namespace pdfforge.PDFCreator.UI.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ITranslationFactory _translationFactory;
        private readonly IThreadManager _threadManager;
        private Notifier _toastNotifier;

        private readonly TimeSpan _notificationDisplayTimeSpan = TimeSpan.FromSeconds(5);

        private bool ToastNotifierIsNull => _toastNotifier == null;

        private readonly ICommand _openDocumentInExplorerCommand;

        public NotificationService(ITranslationFactory translationFactory, IThreadManager threadManager, ICommandLocator commandLocator)
        {
            _translationFactory = translationFactory;
            _threadManager = threadManager;

            _openDocumentInExplorerCommand = commandLocator.GetCommand<QuickActionOpenExplorerLocationCommand>();
        }

        private void StartBackgroundWindow()
        {
            var backgroundwinThread = new Thread(() =>
                {
                    _logger.Debug("Creating thread of StartBackgroundWindow()");
                    var dispatcher = Dispatcher.CurrentDispatcher;
                    _toastNotifier = new Notifier(cfg =>
                    {
                        cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(_notificationDisplayTimeSpan, MaximumNotificationCount.FromCount(15));
                        cfg.PositionProvider = new PrimaryScreenPositionProvider(Corner.BottomRight, 40, 80);
                        cfg.DisplayOptions.Width = 350;
                        cfg.Dispatcher = dispatcher;
                    });

                    // a new Window has to be created in the background, otherwise the notification cannot be shown
                    var w = new Window();

                    w.Loaded += Window_Loaded;

                    w.WindowStyle = WindowStyle.None;
                    w.Height = 0;
                    w.Width = 0;
                    w.ShowInTaskbar = false;
                    w.Visibility = Visibility.Collapsed;
                    w.AllowsTransparency = true;
                    w.Opacity = 0;
                    w.Focusable = false;
                    w.ResizeMode = ResizeMode.NoResize;
                    w.ShowDialog();
                }
            );

            backgroundwinThread.SetApartmentState(ApartmentState.STA);
            backgroundwinThread.Start();
        }

        private void Window_Loaded(object sender, EventArgs args)
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper((Window)sender);

            int exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);

            exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
        }

        public void ShowInfoNotification(string documentName, string documentPath)
        {
            if (ToastNotifierIsNull)
                StartBackgroundWindow();

            var synchedThread = new ThreadStart(() =>
                {
                    _logger.Debug("Creating thread of ShowInfoNotification()");
                    var translation = _translationFactory.CreateTranslation<NotificationTranslation>();
                    _toastNotifier?.Notify<NotificationViewModel>(() => new NotificationViewModel(translation.SuccessTitle, translation.FormatSuccessNotificationMessage(documentName)
                        , NotificationType.Info, _openDocumentInExplorerCommand, documentPath));

                    Thread.Sleep(_notificationDisplayTimeSpan);
                }
            );

            _threadManager.StartSynchronizedUiThread(synchedThread, "SingleNotificationCall");
        }

        public void ShowErrorNotification(string documentName)
        {
            if (ToastNotifierIsNull)
                StartBackgroundWindow();

            var synchedThread = new ThreadStart(() =>
                {
                    _logger.Debug("Creating thread of ShowErrorNotification()");
                    var translation = _translationFactory.CreateTranslation<NotificationTranslation>();
                    _toastNotifier.Notify<NotificationViewModel>(() => new NotificationViewModel(translation.ErrorTitle, translation.FormatErrorNotificationMessage(documentName), NotificationType.Error));

                    Thread.Sleep(_notificationDisplayTimeSpan);
                }
            );

            _threadManager.StartSynchronizedUiThread(synchedThread, "SingleErrorNotificationCall");
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            int error = 0;
            IntPtr result = IntPtr.Zero;
            // Win32 SetWindowLong doesn't clear error on success
            SetLastError(0);

            if (IntPtr.Size == 4)
            {
                Int32 tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
                error = Marshal.GetLastWin32Error();
                result = new IntPtr(tempResult);
            }
            else
            {
                result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
                error = Marshal.GetLastWin32Error();
            }

            if ((result == IntPtr.Zero) && (error != 0))
            {
                throw new System.ComponentModel.Win32Exception(error);
            }

            return result;
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern Int32 IntSetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

        private static int IntPtrToInt32(IntPtr intPtr)
        {
            return unchecked((int)intPtr.ToInt64());
        }

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(int dwErrorCode);
    }

    [Flags]
    public enum ExtendedWindowStyles
    {
        WS_EX_TOOLWINDOW = 0x00000080
    }

    public enum GetWindowLongFields
    {
        GWL_EXSTYLE = -20
    }
}
