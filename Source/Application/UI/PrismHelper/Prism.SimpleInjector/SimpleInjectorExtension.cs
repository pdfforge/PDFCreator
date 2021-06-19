using SimpleInjector;
using System;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.PrismHelper.Prism.SimpleInjector
{
    public static class SimpleInjectorExtension
    {
        private static readonly Dictionary<string, Type> NavigationViewRegistry = new Dictionary<string, Type>();

        public static void RegisterTypeForNavigation<T>(this Container container, string name = null)
        {
            var type = typeof(T);

            RegisterTypeForNavigation(container, type, name);
        }

        public static void RegisterTypeForNavigation(this Container container, Type type, string name = null)
        {
            if (name == null)
                name = type.Name;

            NavigationViewRegistry[name] = type;
            container.Register(type);
        }

        public static object ResolveNavigationType(this Container container, string name)
        {
            var type = NavigationViewRegistry[name];
            return container.GetInstance(type);
        }

        public static IEnumerable<Type> GetRegisteredTypes()
        {
            return NavigationViewRegistry.Values;
        }
    }
}
