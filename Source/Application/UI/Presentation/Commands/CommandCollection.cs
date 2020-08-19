using pdfforge.PDFCreator.Conversion.Jobs.Annotations;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class CommandCollection<T> : INotifyPropertyChanged, IEnumerable<NamedCommand> where T : ITranslatable, new()
    {
        private readonly ITranslationUpdater _updater;

        private readonly IList<NamedCommand> _commands = new List<NamedCommand>();

        private readonly List<Action<T>> _updateTranslationFunctions = new List<Action<T>>();
        private T _translation;

        public CommandCollection(ITranslationUpdater updater)
        {
            _updater = updater;
            updater.RegisterAndSetTranslation(UpdateTranslations);
        }

        private void UpdateTranslations(ITranslationFactory translationFactory)
        {
            _translation = translationFactory.UpdateOrCreateTranslation(_translation);

            foreach (var updateTranslationFunction in _updateTranslationFunctions)
            {
                updateTranslationFunction(_translation);
            }
        }

        public void AddCommand(ICommand command, Func<T, string> getTranslationFunc)
        {
            command.CanExecuteChanged += (sender, args) => OnPropertyChanged(nameof(Enabled));
            var namedCommand = new NamedCommand(command);
            Action<T> action = translation => namedCommand.Name = getTranslationFunc(translation);
            action(_translation);
            _commands.Add(namedCommand);
            _updateTranslationFunctions.Add(action);
        }

        public IEnumerator<NamedCommand> GetEnumerator()
        {
            return _commands.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Enabled
        {
            get
            {
                if (_commands == null || _commands.Count < 1)
                    return false;

                foreach (var command in _commands)
                {
                    if (!command.Command.CanExecute(null))
                        return false;
                }

                return true;
            }
        }

        public void RaiseEnabledChanged()
        {
            OnPropertyChanged(nameof(Enabled));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class NamedCommand : INotifyPropertyChanged
    {
        private string _name = "";

        public NamedCommand(ICommand command)
        {
            Command = command;
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public ICommand Command { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
