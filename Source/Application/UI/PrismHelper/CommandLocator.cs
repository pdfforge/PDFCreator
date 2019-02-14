using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using SimpleInjector;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.PrismHelper
{
    public class CommandLocator : ICommandLocator
    {
        private readonly Container _container;

        public CommandLocator(Container container)
        {
            _container = container;
        }

        public ICommand GetCommand<T>() where T : class, ICommand
        {
            var instance = _container.GetInstance<T>();
            return instance;
        }

        public ICommand GetInitializedCommand<TCommand, TParameter>(TParameter parameter) where TCommand : class, IInitializedCommand<TParameter>
        {
            if (parameter == null)
                return null;
            var instance = _container.GetInstance<TCommand>();
            instance?.Init(parameter);
            return instance;
        }

        public IMacroCommandBuilder CreateMacroCommand()
        {
            return new MacroCommandBuilder(this);
        }
    }
}