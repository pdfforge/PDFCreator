using System.Collections.Generic;
using System.Linq;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using System.Windows;
using System.Windows.Controls;
using pdfforge.PDFCreator.Core.Workflow;

namespace pdfforge.PDFCreator.UI.Presentation.Controls
{
    /// <summary>
    /// Interaction logic for PDFViewerControl.xaml
    /// </summary>
    public partial class PreviewControl : UserControl
    {
        public static readonly DependencyProperty PreviewManagerProperty = DependencyProperty.Register(
            nameof(PreviewManager),
            typeof(PreviewManager),
            typeof(PreviewControl),
            new PropertyMetadata());

        public PreviewManager PreviewManager
        {
            get
            { 
                return (PreviewManager)GetValue(PreviewManagerProperty);
            }

            set { SetValue(PreviewManagerProperty, value); }
        }

        public static readonly DependencyProperty IsPreviewLoadingProperty = DependencyProperty.Register(
            nameof(IsPreviewLoading),
            typeof(bool),
            typeof(PreviewControl),
            new PropertyMetadata(true));

        public bool IsPreviewLoading
        {
            get { return (bool)GetValue(IsPreviewLoadingProperty); }

            set { SetValue(IsPreviewLoadingProperty, value); }
        }

        public static readonly DependencyProperty PreviewPageListProperty = DependencyProperty.Register(
            nameof(PreviewPageList),
            typeof(List<PreviewPage>),
            typeof(PreviewControl),
            new PropertyMetadata());

        public List<PreviewPage> PreviewPageList
        {
            get { return (List<PreviewPage>)GetValue(PreviewPageListProperty); }

            set { SetValue(PreviewPageListProperty, value); }
        }

        public static readonly DependencyProperty IsPreviewEnabledProperty = DependencyProperty.Register(
            nameof(IsPreviewEnabled),
            typeof(bool),
            typeof(PreviewControl),
            new PropertyMetadata(defaultValue:true));

        public bool IsPreviewEnabled
        {
            get { return (bool)GetValue(IsPreviewEnabledProperty); }

            set { SetValue(IsPreviewEnabledProperty, value); }
        }

        public static readonly DependencyProperty JobInfoProperty = DependencyProperty.Register(
            nameof(JobInfo),
            typeof(JobInfo),
            typeof(PreviewControl),
            new PropertyMetadata(null, JobInfoPropertyChangedCallback));

        private static async void JobInfoPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PreviewControl previewControl)
            {
                await previewControl.Dispatcher.Invoke(async () => 
                {
                    if (previewControl.JobInfo == null || previewControl.PreviewManager == null || !previewControl.IsPreviewEnabled)
                        return;

                    previewControl.PreviewPageList = [];
                    previewControl.IsPreviewLoading = true;
                    previewControl.PreviewPageList = (await previewControl.PreviewManager.GetTotalPreviewPages(previewControl.JobInfo)).ToList();
                    previewControl.IsPreviewLoading = false;
                });
            }
        }
        public JobInfo JobInfo
        {
            get { return (JobInfo)GetValue(JobInfoProperty); }

            set { SetValue(JobInfoProperty, value); }
        }

        public PreviewControl()
        {
            InitializeComponent();
        }
    }
}
