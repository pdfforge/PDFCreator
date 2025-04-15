using System.Windows.Controls;
namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.EmailCollectionHintStep;
/// <summary>
/// Interaction logic for EmailCollectionHintStepView.xaml
/// </summary>
public partial class EmailCollectionHintStepView : UserControl
{
    public EmailCollectionHintStepView(EmailCollectionHintStepViewModel stepViewModel)
    {
        DataContext = stepViewModel;
        InitializeComponent();
    }
}
