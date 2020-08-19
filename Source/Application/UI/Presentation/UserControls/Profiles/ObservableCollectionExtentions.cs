using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public static class ObservableCollectionExtentions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> from)
        {
            var to = new ObservableCollection<T>();
            foreach (var value in from)
            {
                to.Add(value);
            }
            return to;
        }
    }
}