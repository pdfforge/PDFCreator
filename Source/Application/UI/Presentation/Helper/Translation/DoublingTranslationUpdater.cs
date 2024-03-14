using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Translation
{

    public class DoublingTranslationUpdater : TranslationUpdater
    {
        public DoublingTranslationUpdater(ITranslationFactory translationFactory, IThreadManager threadManager) : base(translationFactory, threadManager)
        {
        }

        protected override void UpdateTranslation<T>(ITranslationFactory translationFactory, ITranslatableViewModel<T> viewModel)
        {
            base.UpdateTranslation<T>(translationFactory, viewModel);

            var propertyInfos = viewModel.Translation.GetType().GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                var copyText = "<c>";
                if (propertyInfo.PropertyType.FullName != null && 
                    !propertyInfo.PropertyType.FullName.Equals("System.String")) 
                    continue;

                var value = propertyInfo.GetValue(viewModel.Translation);

                if ((value.ToString()).Contains(copyText))
                    continue;

                propertyInfo.SetValue(viewModel.Translation, $"{value}{copyText}{value}");
            }
        }
    }
}
