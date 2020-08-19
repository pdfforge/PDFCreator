using pdfforge.PDFCreator.Conversion.Jobs.Annotations;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.QuickActionStep
{
    public class DropDownButtonItem : INotifyPropertyChanged
    {
        private readonly Func<string> _getNameFunction;
        private readonly Func<object> _getParameterFunction;
        public string Name => _getNameFunction();
        public object Parameter => _getParameterFunction();
        public ICommand Command { get; }

        public DropDownButtonItem(Func<string> getNameFunction, Func<object> getParameterFunction, ICommand command )
        {
            _getNameFunction = getNameFunction;
            _getParameterFunction = getParameterFunction;
            Command = command;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
