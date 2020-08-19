using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using System;
using System.Linq.Expressions;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public abstract class CurrentSettingsBase<TSource, TSetting> : ICurrentSettings<TSetting>
    {
        private readonly Func<TSource, TSetting> _getter;
        private readonly Action<TSource, TSetting> _setter;

        public event EventHandler SettingsChanged;

        protected abstract TSource GetCurrentSettings();

        public TSetting Settings
        {
            get => _getter(GetCurrentSettings());
            set => _setter(GetCurrentSettings(), value);
        }

        public CurrentSettingsBase(Expression<Func<TSource, TSetting>> expression, ICurrentSettingsProvider currentSettingsProvider)
        {
            var generator = new CustomMethodGenerator();
            _getter = expression.Compile();
            _setter = generator.GenerateSetterFromGetter<TSource, TSetting, TSetting>(expression);

            currentSettingsProvider.SettingsChanged += (sender, args) => SettingsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class CreatorCurrentSettings<TSetting> : CurrentSettingsBase<PdfCreatorSettings, TSetting>
    {
        private readonly CurrentSettingsProvider _currentSettingsProvider;

        public CreatorCurrentSettings(Expression<Func<PdfCreatorSettings, TSetting>> expression, CurrentSettingsProvider currentSettingsProvider)
            : base(expression, currentSettingsProvider)
        {
            _currentSettingsProvider = currentSettingsProvider;
        }

        protected override PdfCreatorSettings GetCurrentSettings()
        {
            return _currentSettingsProvider.Settings;
        }
    }
}
