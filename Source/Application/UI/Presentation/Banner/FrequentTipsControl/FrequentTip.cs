using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Banner
{
    public class FrequentTip
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public ICommand Command { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            FrequentTip ft = obj as FrequentTip;
            if (ft == null)
                return false;

            return (Title == ft.Title)
                   && (Text == ft.Text)
                   && (Command == ft.Command)
                ;
        }
    }
}
