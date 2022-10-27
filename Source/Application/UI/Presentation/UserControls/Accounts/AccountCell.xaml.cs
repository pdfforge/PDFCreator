using pdfforge.PDFCreator.Conversion.Settings;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts
{
    /// <summary>
    /// Interaction logic for AccountCell.xaml
    /// </summary>
    public partial class AccountCell : UserControl
    {
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(object),
                typeof(AccountCell), new FrameworkPropertyMetadata());

        public object Icon
        {
            get => (object)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty AccountNameStringProperty =
            DependencyProperty.Register(nameof(AccountNameString), typeof(string),
                typeof(AccountCell), new FrameworkPropertyMetadata());

        public string AccountNameString
        {
            get => (string)GetValue(AccountNameStringProperty);
            set => SetValue(AccountNameStringProperty, value);
        }

        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register(nameof(EditCommand), typeof(ICommand),
                typeof(AccountCell), new FrameworkPropertyMetadata());

        public ICommand EditCommand
        {
            get => (ICommand)GetValue(EditCommandProperty);
            set => SetValue(EditCommandProperty, value);
        }

        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register(nameof(RemoveCommand), typeof(ICommand),
                typeof(AccountCell), new FrameworkPropertyMetadata());

        public ICommand RemoveCommand
        {
            get => (ICommand)GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        public AccountCell()
        {
            InitializeComponent();
        }
    }

    //Inherit from any account...
    public class DesignTimeAccountCellDataContext : SmtpAccount
    { }
}
