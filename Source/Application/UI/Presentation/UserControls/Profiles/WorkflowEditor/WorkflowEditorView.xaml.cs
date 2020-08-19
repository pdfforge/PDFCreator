using pdfforge.PDFCreator.UI.Presentation.Events;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using Prism.Events;
using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor
{
    public partial class WorkflowEditorView : UserControl
    {
        private readonly IEventAggregator _eventAggregator;

        public WorkflowEditorView(WorkflowEditorViewModel vm, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            DataContext = vm;
            TransposerHelper.Register(this, vm);
            InitializeComponent();

            // dummy reference to force GongSolutions.Wpf.DragDrop to be copied to bin folder
            var t = typeof(GongSolutions.Wpf.DragDrop.DragDrop);
        }

        private void WorkflowEditorView_OnLoaded(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<SetProfileTabHelpTopicEvent>().Publish(HelpTopic.WorkflowEditor);
        }
    }
}
