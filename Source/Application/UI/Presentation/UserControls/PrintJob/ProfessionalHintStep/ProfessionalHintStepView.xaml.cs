using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.ProfessionalHintStep
{
    /// <summary>
    /// Interaction logic for PlusHintView.xaml
    /// </summary>
    public partial class ProfessionalHintStepView : UserControl
    {
        public ProfessionalHintStepView(ProfessionalHintStepViewModel stepViewModel)
        {
            DataContext = stepViewModel;

            //Todo: Reuired?
            //Loaded += (sender, e) =>
            //    MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            InitializeComponent();
        }
    }
}
