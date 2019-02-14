using System;

namespace pdfforge.PDFCreator.Core.ServiceLocator
{
    /// <summary>
    /// RestrictedServiceLocator
    /// </summary>
    public static class RestrictedServiceLocator
    {
        private static IWhitelistedServiceLocator _current;

        /// <summary>
        /// Is true if the a locator instance is set
        /// </summary>
        public static bool IsLocationProviderSet => _current != null;

        /// <summary>
        /// Returns the current instance of IWhitelistedServiceLocator if present.
        /// </summary>
        public static IWhitelistedServiceLocator Current
        {
            get
            {
                if (!IsLocationProviderSet)
                    throw new InvalidOperationException("No Service Locator instance ist set!");
                return _current;
            }
            set
            {
                _current = value;
            }
        }
    }
}
