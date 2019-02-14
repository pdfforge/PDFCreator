using CommonServiceLocator;
using SimpleInjector;
using System;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Editions.EditionBase.Prism.SimpleInjector
{
    public class SimpleInjectorServiceLocator : ServiceLocatorImplBase
    {
        private readonly Container _container;

        public SimpleInjectorServiceLocator(Container container)
        {
            _container = container;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (key != null && serviceType == typeof(object))
                return _container.ResolveNavigationType(key);

            return _container.GetInstance(serviceType);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return _container.GetAllInstances(serviceType);
        }
    }
}
