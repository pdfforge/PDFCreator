using pdfforge.PDFCreator.Core.Services.Macros;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeCommandBuilder : IMacroCommandBuilder
    {
        public DesignTimeCommandBuilder()
        {
        }

        public IMacroCommand Build()
        {
            return new MacroCommand(new List<ICommand>());
        }

        public IMacroCommandBuilder AddCommand(ICommand command)
        {
            return this;
        }

        public IMacroCommandBuilder AddCommand<T>() where T : class, ICommand
        {
            return this;
        }

        public IMacroCommandBuilder AddInitializedCommand<T>(Action<T> initAction) where T : class, ICommand
        {
            return this;
        }
    }
}
