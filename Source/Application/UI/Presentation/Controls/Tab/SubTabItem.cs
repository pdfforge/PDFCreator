using Prism;
using System;
using System.Windows;
using System.Windows.Controls;
using Prism.Regions;

namespace pdfforge.PDFCreator.UI.Presentation.Controls.Tab
{
    public class SubTabItem : TabItem, IActiveAware, IRegionMemberLifetime
    {
        private bool _isActive;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                IsActiveChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler IsActiveChanged;
        public bool KeepAlive { get; } = true;
    }
}
