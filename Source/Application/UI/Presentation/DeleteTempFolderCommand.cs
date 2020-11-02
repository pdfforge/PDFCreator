using pdfforge.PDFCreator.Utilities;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public interface IDeleteTempFolderCommand : ICommand
    {
    }

    public class DeleteTempFolderCommand : IDeleteTempFolderCommand
    {
        private readonly ITempDirectoryHelper _tempDirectory;

        public DeleteTempFolderCommand(ITempDirectoryHelper tempDirectory)
        {
            _tempDirectory = tempDirectory;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _tempDirectory.CleanUp();
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
