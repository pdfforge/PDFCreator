using Prism.Ioc;
using SimpleInjector;
using System;

namespace pdfforge.PDFCreator.Editions.EditionBase.Prism.SimpleInjector
{
    public class SimpleInjectorContainerExtension : IContainerExtension<Container>
    {
        public SimpleInjectorContainerExtension(Container instance)
        {
            Instance = instance;
        }

        public Container Instance { get; }

        public object Resolve(Type type)
        {
            return Instance.GetInstance(type);
        }

        public object Resolve(Type type, string name)
        {
            throw new NotImplementedException("Simple Inject cannot resolve by name");
        }

        public void RegisterInstance(Type type, object instance)
        {
            Instance.Register(type, () => instance);
        }

        public void RegisterSingleton(Type @from, Type to)
        {
            Instance.RegisterSingleton(from, to);
        }

        public void Register(Type @from, Type to)
        {
            Instance.Register(from, to);
        }

        public void Register(Type @from, Type to, string name)
        {
            throw new NotImplementedException("Simple Inject cannot register by name");
        }

        public void FinalizeExtension()
        {
        }

        public object ResolveViewModelForView(object view, Type viewModelType)
        {
            return Instance.GetInstance(viewModelType);
        }

        public bool SupportsModules => false;
    }
}
