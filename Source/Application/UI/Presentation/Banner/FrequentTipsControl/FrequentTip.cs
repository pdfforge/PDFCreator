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
            if (!(obj is FrequentTip tip))
                return false;

            return (Title == tip.Title)
                   && (Text == tip.Text)
                   && (Command == tip.Command)
                ;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // Choose large primes to avoid hashing collisions
                const int hashingBase = (int)2166136261;
                const int hashingMultiplier = 16777619;

                int hash = hashingBase;
                // ReSharper disable NonReadonlyMemberInGetHashCode
                hash = (hash * hashingMultiplier) ^ Title?.GetHashCode() ?? 0;
                hash = (hash * hashingMultiplier) ^ Text?.GetHashCode() ?? 0;
                return hash;
            }
        }
    }
}
