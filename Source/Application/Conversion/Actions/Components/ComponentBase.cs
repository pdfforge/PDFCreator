using System.Collections.Generic;
using System.Linq;

namespace pdfforge.PDFCreator.Conversion.Actions.Components
{
    public class ComponentBase : IComponentBase
    {
        private readonly List<IActionComponent> _components = new List<IActionComponent>();

        public ComponentBase AddComponent(IActionComponent component)
        {
            _components.Add(component);
            return this;
        }

        public List<T> GetComponents<T>() where T : IActionComponent
        {
            return _components.OfType<T>().ToList();
        }

        public T GetComponent<T>() where T : IActionComponent
        {
            return _components.OfType<T>().FirstOrDefault();
        }
    }
}
