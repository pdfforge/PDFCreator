using Optional;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Misc;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Tokens
{
    public class TokenViewModel<T> : ObservableObject, ITranslatableViewModel<TokenHintPanelTranslation>, IMountable
    {
        private readonly Func<string, string> _generatePreview;
        private string _preview;
        private T _currentValue;
        private readonly Func<T, string> _getter;
        private readonly Action<T, string> _setter;

        public TokenViewModel(Expression<Func<T, string>> selector, T initialValue, IList<string> tokens, Func<string, string> generatePreview, IList<Func<string, Option<string>>> buttonCommandFunctions)
        {
            if (RestrictedServiceLocator.IsLocationProviderSet)
            {
                var translationUpdater = RestrictedServiceLocator.Current.GetInstance<ITranslationUpdater>();
                translationUpdater.RegisterAndSetTranslation(this);
            }

            _getter = selector.Compile();
            var newValue = Expression.Parameter(selector.Body.Type);
            _setter = Expression.Lambda<Action<T, string>>(
                Expression.Assign(selector.Body, newValue),
                selector.Parameters[0], newValue).Compile();

            foreach (var command in buttonCommandFunctions)
            {
                ButtonCommands.Add(new DelegateCommand(_ => ExecuteButtonCommand(command)));
            }

            _generatePreview = generatePreview;

            SetTokens(tokens);

            CurrentValue = initialValue;
        }

        public void SetTokens(IList<string> tokens)
        {
            Tokens = new ObservableCollection<TokenWithCommand>();
            foreach (var token in tokens)
                Tokens.Add(new TokenWithCommand(token, ExecuteMethod));
        }

        public T CurrentValue
        {
            get { return _currentValue; }
            set { _currentValue = value; RaiseTextChanged(); }
        }

        public ICommand ButtonCommand => ButtonCommands.Any() ? ButtonCommands[0] : null;
        public IList<ICommand> ButtonCommands { get; set; } = new List<ICommand>();

        public bool ShowButton => ButtonCommand != null;

        public string Preview
        {
            get { return _preview; }
            set
            {
                _preview = value;
                RaisePropertyChanged(nameof(Preview));
            }
        }

        public ObservableCollection<TokenWithCommand> Tokens { get; set; }

        public int CurrentCursorPos { get; set; }

        public string Text
        {
            get
            {
                if (CurrentValue == null)
                    return "";
                return _getter(CurrentValue);
            }
            set
            {
                _setter(CurrentValue, value);
                TextChanged?.Invoke(this, EventArgs.Empty);
                GeneratePreviewText();
            }
        }

        private void ExecuteButtonCommand(Func<string, Option<string>> func)
        {
            var result = func(Text);
            result.MatchSome(s => Text = s);
            CurrentCursorPos = Text.Length;
            RaiseTextChanged();
            RaisePropertyChanged(nameof(CurrentCursorPos));
        }

        private void GeneratePreviewText()
        {
            if (CurrentValue == null || Text == null)
                Preview = "";
            else
                Preview = _generatePreview(Text);
        }

        private void ExecuteMethod(string text)
        {
            var newSelectionStart = CurrentCursorPos + text.Length;
            Text = Text.Insert(CurrentCursorPos, text);

            RaisePropertyChanged(nameof(Text));

            CurrentCursorPos = newSelectionStart;
            RaisePropertyChanged(nameof(CurrentCursorPos));
        }

        public event EventHandler TextChanged;

        public void RaiseTextChanged()
        {
            TextChanged?.Invoke(this, EventArgs.Empty);
            RaisePropertyChanged(nameof(Text));
            GeneratePreviewText();
        }

        public TokenHintPanelTranslation Translation { get; set; }

        public virtual void MountView()
        {
            GeneratePreviewText();
            RaisePropertyChanged(nameof(Text));
            RaisePropertyChanged(nameof(Preview));
        }

        public virtual void UnmountView()
        {
        }
    }

    public class TokenWithCommand
    {
        public TokenWithCommand(string name, Action<string> execute)
        {
            Name = name;
            MyCommand = new DelegateCommand(o => execute(Name));
        }

        public string Name { get; set; }
        public DelegateCommand MyCommand { get; set; }
    }
}
