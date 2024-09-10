using System;
using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.Windows.Feedback
{
    public partial class FeedbackWindowView
    {
        public FeedbackWindowView(FeedbackWindowViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        private void FeedbackWindowView_OnContentRender(object sender, EventArgs e)
        {
            MinWidth = ActualHeight;
            MinHeight = ActualHeight;
        }

        private void UploadedFiles_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (FeedbackWindowViewModel) DataContext;
            if (viewModel == null) 
                return;

            var grid = sender as Grid;
            var window = grid?.FindName("WindowName") as Window;
            if(window == null)
                return;
            // 95 is the height of the ListBox
            if (!viewModel.FileIsAttached)
            {
                window.MinHeight = ActualHeight - 105;
                window.Height = window.MinHeight;
            }
            else
            {
                window.MinHeight = ActualHeight + 105;
            }
        }
    }
}
