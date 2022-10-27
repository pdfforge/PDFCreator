using MahApps.Metro.Controls;
using pdfforge.Obsidian.Trigger;
using System.Windows;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public partial class InteractionHostWindow : MetroWindow
    {
        public IInteractionRequest InteractionRequest { get; }

        public InteractionHostWindow(IInteractionRequest interactionRequest)
        {
            InteractionRequest = interactionRequest;
            InitializeComponent();
        }

        private void InteractionHostWindow_OnUnloaded(object sender, RoutedEventArgs e)
        {
            OverlayActionTrigger.Actions.Clear();
        }
    }
}
