using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Translation
{
    public interface ITranslationUpdater : IWhitelisted
    {
        void RegisterAndSetTranslation<T>(ITranslatableViewModel<T> model) where T : ITranslatable, new();

        void RegisterAndSetTranslation(Action<ITranslationFactory> setTranslationAction);

        void CleanUp(object sender, ThreadFinishedEventArgs e);

        void Clear();
    }

    public class TranslationUpdater : ITranslationUpdater
    {
        private readonly Dictionary<Action<ITranslationFactory>, Thread> _threadLookup = new Dictionary<Action<ITranslationFactory>, Thread>();
        private readonly ITranslationFactory _translationFactory;

        public TranslationUpdater(ITranslationFactory translationFactory, IThreadManager threadManager)
        {
            _translationFactory = translationFactory;
            translationFactory.TranslationChanged += (sender, args) => UpdateTranslations();
            threadManager.CleanUpAfterThreadClosed += CleanUp;
        }

        public void RegisterAndSetTranslation<T>(ITranslatableViewModel<T> model) where T : ITranslatable, new()
        {
            lock (this)
            {
                UpdateTranslation(_translationFactory, model);
                _threadLookup.Add(tf => UpdateTranslation(tf, model), Thread.CurrentThread);
            }
        }

        public void RegisterAndSetTranslation(Action<ITranslationFactory> setTranslationAction)
        {
            lock (this)
            {
                setTranslationAction(_translationFactory);
                _threadLookup.Add(setTranslationAction, Thread.CurrentThread);
            }
        }

        public void CleanUp(object sender, ThreadFinishedEventArgs e)
        {
            lock (this)
            {
                var keyValuePairs = _threadLookup.Where(pair => pair.Value == e.SynchronizedThread.Thread).ToList();

                foreach (var result in keyValuePairs)
                {
                    _threadLookup.Remove(result.Key);
                }
            }
        }

        private void UpdateTranslations()
        {
            lock (this)
            {
                // Convert to array to prevent race condition
                var updateTranslationActions = _threadLookup.Keys.ToArray();

                foreach (var updateTranslationAction in updateTranslationActions)
                {
                    updateTranslationAction(_translationFactory);
                }
            }
        }

        public void Clear()
        {
            lock (this)
            {
                _threadLookup.Clear();
            }
        }

        private void UpdateTranslation<T>(ITranslationFactory translationFactory, ITranslatableViewModel<T> viewModel) where T : ITranslatable, new()
        {
            viewModel.Translation = translationFactory.UpdateOrCreateTranslation(viewModel.Translation);
        }
    }
}
