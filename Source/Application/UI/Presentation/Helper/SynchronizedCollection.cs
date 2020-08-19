using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    /// <summary>
    ///     SynchronizedCollection is a wrapper that binds an IList and an ObserverableCollection. All changes that happen
    ///     inside the ObservableCollection will be replicated to the base collection.
    ///     Note: Changes in the base collection can't be detected. Do not modify it after starting the sync.
    /// </summary>
    /// <typeparam name="T">Type of the items in the List and the bound ObservableCollection</typeparam>
    public class SynchronizedCollection<T>
    {
        /// <summary>
        ///     Create a new SynchronizedCollection that reflects changes that take place in the ObservableCollection.
        /// </summary>
        /// <param name="collection">The collection that will be kept in sync when changes to the ObservableCollection take place</param>
        public SynchronizedCollection(IList<T> collection)
        {
            Collection = collection;
            ObservableCollection = new ObservableCollection<T>(collection);
            ResumeUpdates();
        }

        /// <summary>
        ///     The Collection that is kept in sync. Do not modify it! Use the ObservableCollection instead.
        /// </summary>
        public IList<T> Collection { get; }

        /// <summary>
        ///     All changes to this ObservableCollection will be reflected to the base collection
        /// </summary>
        public ObservableCollection<T> ObservableCollection { get; }

        private void OnObservableCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Collection.Insert(e.NewStartingIndex, (T)e.NewItems[0]);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    Collection.RemoveAt(e.OldStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Move:
                    Collection.RemoveAt(e.OldStartingIndex);
                    Collection.Insert(e.NewStartingIndex, (T)e.OldItems[0]);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    Collection.Clear();
                    foreach (var itm in ObservableCollection)
                        Collection.Add(itm);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Do not promote updates to the ObservableCollection to the source collection.
        /// This can be used to react to changes in the source collection that need to be applied to the ObservableCollection.
        /// </summary>
        public void SuspendUpdates()
        {
            ObservableCollection.CollectionChanged -= OnObservableCollectionChanged;
        }

        /// <summary>
        /// Start promoting updates to the ObservableCollection to the source collection after updates were suspended.
        /// </summary>
        public void ResumeUpdates()
        {
            ObservableCollection.CollectionChanged += OnObservableCollectionChanged;
        }
    }
}
