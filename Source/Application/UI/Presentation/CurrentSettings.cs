using pdfforge.PDFCreator.Conversion.Settings;
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
            _getter = expression.Compile();

            currentSettingsProvider.SettingsChanged += (sender, args) => SettingsChanged?.Invoke(this, EventArgs.Empty);

            ParameterExpression newValue = Expression.Parameter(typeof(TSetting));

            // Define own Setter
            _setter = Expression.Lambda<Action<TSource, TSetting>>(
                Expression.Assign(expression.Body, newValue),
                expression.Parameters[0], newValue
            ).Compile();
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
