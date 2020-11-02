using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace pdfforge.PDFCreator.UI.COM
{
    public class ValueReflector
    {
        private const BindingFlags RequiredFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField | BindingFlags.SetProperty;
        private static readonly IFormatProvider InvariantFormatProvider = CultureInfo.InvariantCulture.NumberFormat;

        public bool HasProperty(object parent, string propertyName)
        {
            return FindProperty(parent, propertyName) != null;
        }

        public bool SetPropertyValue(object parent, string propertyName, string value)
        {
            var pi = FindProperty(parent, propertyName);

            if (pi == null)
                throw new ArgumentException($"unknown property {propertyName} for {parent.GetType().Name}, check if property exists and has a getter.", nameof(propertyName));

            SetProperty(pi.Parent, pi.Property, value);
            return true;
        }

        public bool SetListPropertyValue(object parent, string propertyName, IEnumerable<string> value)
        {
            var propertyInfo = FindProperty(parent, propertyName);

            if (propertyInfo == null)
                throw new ArgumentException(nameof(propertyName));

            SetListProperty(propertyInfo.Parent, propertyInfo.Property, value);
            return true;
        }

        private PropertyWrapper FindProperty(object parent, string propertyName)
        {
            var t = parent.GetType();

            string itemRemainder = null;
            var splitPos = propertyName.IndexOf('.');
            if (splitPos >= 0)
            {
                itemRemainder = propertyName.Substring(splitPos + 1);
                propertyName = propertyName.Substring(0, splitPos);
            }

            var mi = t.GetMember(propertyName, RequiredFlags);

            if (mi.Length == 0)
            {
                return null;
            }

            switch (mi[0].MemberType)
            {
                case MemberTypes.Property:
                    var pi = t.GetProperty(propertyName, RequiredFlags);
                    if (!pi.CanRead)
                        return null;

                    if (itemRemainder != null)
                    {
                        var m = pi.GetGetMethod();
                        var newParent = m.Invoke(parent, new object[] { });
                        return FindProperty(newParent, itemRemainder);
                    }
                    return new PropertyWrapper(parent, parent.GetType().GetProperty(propertyName, RequiredFlags));

                default:
                    throw new Exception("MemberType " + mi[0].MemberType + " not supported in " + t + " for Member " + propertyName);
            }
        }

        private void SetListProperty(object parent, PropertyInfo property, IEnumerable<string> value)
        {
            property.SetValue(parent, value.ToList(), null);
        }

        private void SetProperty(object parent, PropertyInfo property, string value)
        {
            object v;

            if (property.PropertyType.IsEnum)
            {
                v = ConvertEnum(value, property.PropertyType);
            }
            else
            {
                v = ConvertValue(value, property.PropertyType);
            }

            property.SetValue(parent, v, null);
        }

        private object ConvertEnum(string value, Type type)
        {
            if (!Enum.IsDefined(type, value))
            {
                throw new ArgumentException($"unknown value {value} for enum {type.Name}", nameof(value));
            }

            return Enum.Parse(type, value);
        }

        private object ConvertValue(string value, Type type)
        {
            if (type == typeof(Color))
            {
                return ColorTranslator.FromHtml(value);
            }
            if (type == typeof(Version))
            {
                return Version.Parse(value);
            }
            return Convert.ChangeType(value, type, InvariantFormatProvider);
        }

        public string GetPropertyValue(object parent, string propertyName)
        {
            var pInfo = FindProperty(parent, propertyName);

            if (pInfo == null)
                throw new ArgumentException($"Property with Name {propertyName} not found in object-type {parent.GetType()}.");

            return pInfo.Property.GetValue(pInfo.Parent, null).ToString();
        }

        public IEnumerable<string> GetPropertyListValue(object parent, string propertyName)
        {
            var pInfo = FindProperty(parent, propertyName);

            if (pInfo == null)
                throw new ArgumentException("Property not found.");

            return (IList<string>)pInfo.Property.GetValue(pInfo.Parent, null);
        }
    }

    internal class PropertyWrapper
    {
        public PropertyWrapper(object parent, PropertyInfo property)
        {
            Parent = parent;
            Property = property;
        }

        public object Parent { get; set; }
        public PropertyInfo Property { get; set; }
    }
}
