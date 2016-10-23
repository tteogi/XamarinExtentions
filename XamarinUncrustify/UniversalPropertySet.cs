using System;
using MonoDevelop.Core;
using MonoDevelop.Projects;

namespace GMonoDevelop
{
	public class UniversalPropertySet
	{
		private IPropertySet _propertySet;
		private PropertyBag _propertyBag;
		public UniversalPropertySet(IPropertySet property)
		{
			_propertySet = property;
		}

		public UniversalPropertySet(PropertyBag property)
		{
			_propertyBag = property;
		}

		public T GetValue<T>(string name)
		{
			return _propertySet != null ? _propertySet.GetValue<T>(name) : _propertyBag.GetValue<T>(name);
		}

		public void SetValue<T>(string name, T value)
		{
			if (_propertySet != null)
				_propertySet.SetValue(name, value);
			else
				_propertyBag.SetValue(name, value);
		}

		public bool HasValue(string name)
		{
			return _propertySet != null ? _propertySet.HasProperty(name) : _propertyBag.HasValue(name);
		}
	}
}
