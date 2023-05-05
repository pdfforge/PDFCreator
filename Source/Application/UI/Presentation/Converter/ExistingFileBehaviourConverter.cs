using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Newtonsoft.Json.Linq;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.UI.Presentation.Converter
{
    internal class ExistingFileBehaviourConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var requestedExistingFileBehaviour = (AutoSaveExistingFileBehaviour)parameter;
            var currentExistingFileBehaviour = (AutoSaveExistingFileBehaviour)values[0];
            var outputFormat = (OutputFormat)values[1];

            if (!outputFormat.IsPdf())
            {
                switch (requestedExistingFileBehaviour)
                {
                    case AutoSaveExistingFileBehaviour.Merge:
                    case AutoSaveExistingFileBehaviour.MergeBeforeModifyActions:
                        return false;
                    case AutoSaveExistingFileBehaviour.EnsureUniqueFilenames:
                        return currentExistingFileBehaviour is AutoSaveExistingFileBehaviour.EnsureUniqueFilenames 
                                                            or AutoSaveExistingFileBehaviour.Merge 
                                                            or AutoSaveExistingFileBehaviour.MergeBeforeModifyActions;
                    case AutoSaveExistingFileBehaviour.Overwrite:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return currentExistingFileBehaviour.Equals(parameter);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
