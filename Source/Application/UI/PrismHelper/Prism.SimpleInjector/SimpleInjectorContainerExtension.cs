using Prism.Ioc;
using SimpleInjector;
using System;

namespace pdfforge.PDFCreator.UI.PrismHelper.Prism.SimpleInjector
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

        public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
        {
            throw new NotImplementedException();
        }

        public object Resolve(Type type, string name)
        {
            throw new NotImplementedException("Simple Inject cannot resolve by name");
        }

        public object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterInstance(Type type, object instance)
        {
            Instance.RegisterSingleton(type, () => instance);
            return this;
        }

        public IContainerRegistry RegisterInstance(Type type, object instance, string name)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterSingleton(Type @from, Type to)
        {
            Instance.RegisterSingleton(from, to);
            return this;
        }

        public IContainerRegistry RegisterSingleton(Type @from, Type to, string name)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry Register(Type @from, Type to)
        {
            Instance.Register(from, to);
            return this;
        }

        public IContainerRegistry Register(Type @from, Type to, string name)
        {
            throw new NotImplementedException("Simple Inject cannot register by name");
        }

        public bool IsRegistered(Type type)
        {
            throw new NotImplementedException();
        }

        public bool IsRegistered(Type type, string name)
        {
            throw new NotImplementedException();
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
