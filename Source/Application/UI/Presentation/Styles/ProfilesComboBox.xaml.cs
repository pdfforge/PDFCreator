using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using NaturalSort.Extension;
using Prism.Events;
using pdfforge.PDFCreator.UI.Presentation.Events;

namespace pdfforge.PDFCreator.UI.Presentation.Styles
{
    public partial class ProfilesComboBox : UserControl
    {
        public ObservableCollection<ConversionProfileWrapper> Profiles
        {
            get => (ObservableCollection<ConversionProfileWrapper>)GetValue(ProfilesProperty);
            set => SetValue(ProfilesProperty, value);
        }

        public static readonly DependencyProperty ProfilesProperty =
            DependencyProperty.Register(nameof(Profiles), typeof(ObservableCollection<ConversionProfileWrapper>),
                typeof(ProfilesComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, Profiles_PropertyChangedCallback));

        public ICollectionView ProfilesView
        {
            get => (ICollectionView)GetValue(ProfilesViewProperty);
            set => SetValue(ProfilesViewProperty, value);
        }

        public static readonly DependencyProperty ProfilesViewProperty =
            DependencyProperty.Register(nameof(ProfilesView), typeof(ICollectionView), typeof(ProfilesComboBox));
        
        public IEventAggregator EventAggregator
        {
            get => (IEventAggregator)GetValue(EventAggregatorProperty);
            set => SetValue(EventAggregatorProperty, value);
        }

        public static readonly DependencyProperty EventAggregatorProperty =
            DependencyProperty.Register(nameof(EventAggregator), typeof(IEventAggregator), typeof(ProfilesComboBox));

        private static void Profiles_PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not ObservableCollection<ConversionProfileWrapper> profiles)
                return;

            var collectionViewSource = new CollectionViewSource { Source = profiles };

            d.SetValue(ProfilesViewProperty, collectionViewSource.View);
        }

        public ConversionProfileWrapper SelectedProfile
        {
            get => (ConversionProfileWrapper)GetValue(SelectedProfileProperty);
            set => SetValue(SelectedProfileProperty, value);
        }

        public static readonly DependencyProperty SelectedProfileProperty =
            DependencyProperty.Register(nameof(SelectedProfile), typeof(ConversionProfileWrapper),
                typeof(ProfilesComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedProfileChanged));

        private static void OnSelectedProfileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                ((ProfilesComboBox)d).SelectedProfile = e.NewValue as ConversionProfileWrapper;

            if (d.GetValue(ProfilesProperty) is ObservableCollection<ConversionProfileWrapper> profiles)
            {
                // This should not be required, but the ObservableCollection does not seem to update the CollectionView sometimes...
                var view = (ICollectionView)d.GetValue(ProfilesViewProperty);
                view.Refresh();
                view.MoveCurrentTo(e.NewValue);
            }
        }

        public ProfilesComboBox()
        {
            InitializeComponent();
        }

        private readonly NaturalSortComparer _naturalSortComparer = new NaturalSortComparer(StringComparison.CurrentCulture);

        private SubscriptionToken _profileRenamedSubscriptionToken;

        private void ProfilesComboBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            var profilesCollectionViewSource = (ListCollectionView)ProfilesView;

            Comparison<ConversionProfileWrapper> conversionProfileComparison = (pmX, pmY)
                => _naturalSortComparer.Compare(pmX.Name, pmY.Name);

            var conversionProfileComparer = Comparer<ConversionProfileWrapper>.Create(conversionProfileComparison);
            profilesCollectionViewSource.CustomSort = conversionProfileComparer;

            _profileRenamedSubscriptionToken = EventAggregator?.GetEvent<ProfileRenamedEvent>().Subscribe(profilesCollectionViewSource.Refresh);
        }

        private void ProfilesComboBox_OnUnloaded(object sender, RoutedEventArgs e)
        {
            _profileRenamedSubscriptionToken?.Dispose();
        }
    }
}
