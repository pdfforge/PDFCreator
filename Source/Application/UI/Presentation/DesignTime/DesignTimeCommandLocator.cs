﻿using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeCommandLocator : ICommandLocator
    {
        public ICommand GetInitializedCommand<TCommand, TParameter>(TParameter parameter) where TCommand : class, IInitializedCommand<TParameter>
        {
            return new DelegateCommand(o => { });
        }

        public IMacroCommandBuilder CreateMacroCommand()
        {
            return new DesignTimeCommandBuilder();
        }

        public ICommand GetCommand<T>() where T : class, ICommand
        {
            return new DelegateCommand(o => { });
        }
    }
}
