using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.UI.Presentation.Events;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using Prism.Events;
using System;
using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public partial class ProfilesView : UserControl
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IDispatcher _dispatcher;

        public ProfilesView(ProfilesViewModel vm, IEventAggregator eventAggregator, IDispatcher dispatcher)
        {
            _eventAggregator = eventAggregator;
            _dispatcher = dispatcher;
            DataContext = vm;
            TransposerHelper.Register(this, vm);
            InitializeComponent();
        }

        private void ProfilesView_OnLoaded(object sender, RoutedEventArgs eventArgs)
        {
            var setHelpTopicEvent = _eventAggregator.GetEvent<SetProfileTabHelpTopicEvent>();
            setHelpTopicEvent.Subscribe(OnSetTopic, true);
        }

        private void ProfilesView_OnUnloaded(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<SetProfileTabHelpTopicEvent>().Unsubscribe(OnSetTopic);
        }

        private void OnSetTopic(HelpTopic helpTopic)
        {
            try
            {
                _dispatcher.BeginInvoke(() => this.SetValue(HelpProvider.HelpTopicProperty, helpTopic));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
