using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Cli.Core;
using PPTServiceUltra.Ink.Core.Cli.Utils;

namespace PPTServiceUltra.Ink.Core.Cli.Parsers
{
	
	
	internal class RuntimeOptionParser< T> : RuntimeCommandLineOptionParser<T>
	{
		private readonly T _options;

		
		private readonly SortedList<int, ValueTupleSlim<int, PropertyInfo>> _indexedValueDictionary = new SortedList<int, ValueTupleSlim<int, PropertyInfo>>();

		private readonly Dictionary<char, PropertyInfo> _shortNameDictionary = new Dictionary<char, PropertyInfo>();

		private readonly Dictionary<string, PropertyInfo> _longNameDictionary = new Dictionary<string, PropertyInfo>();

		public RuntimeOptionParser( string verb, IReadOnlyList<PropertyInfo> attributedProperties)
			: base(verb)
		{
			_options = Activator.CreateInstance<T>();
			foreach (PropertyInfo attributedProperty in attributedProperties)
			{
				if (attributedProperty.IsDefined(typeof(OptionAttribute)))
				{
					OptionAttribute customAttribute = attributedProperty.GetCustomAttribute<OptionAttribute>();
					if (customAttribute.ShortName.HasValue)
					{
						_shortNameDictionary[customAttribute.ShortName.Value] = attributedProperty;
					}
					if (customAttribute.LongName == null || string.IsNullOrWhiteSpace(customAttribute.LongName))
					{
						_longNameDictionary[attributedProperty.Name] = attributedProperty;
					}
					else
					{
						_longNameDictionary[customAttribute.LongName] = attributedProperty;
					}
				}
				if (attributedProperty.IsDefined(typeof(ValueAttribute)))
				{
					ValueAttribute customAttribute2 = attributedProperty.GetCustomAttribute<ValueAttribute>();
					_indexedValueDictionary[customAttribute2.Index] = new ValueTupleSlim<int, PropertyInfo>(customAttribute2.Length, attributedProperty);
				}
			}
		}

		public override void SetValue(IReadOnlyList<string> values)
		{
			int num = 0;
			foreach (KeyValuePair<int, ValueTupleSlim<int, PropertyInfo>> item3 in _indexedValueDictionary)
			{
				int num2 = item3.Key + num;
				int item;
				PropertyInfo item2;
				item3.Value.Deconstruct(out item, out item2);
				int num3 = item;
				PropertyInfo property = item2;
				num += num3 - 1;
				if (num3 == 1)
				{
					SetValueCore(property, values[num2]);
				}
				else
				{
					SetValueCore(property, values.Skip(num2).Take(num3).ToList());
				}
			}
		}

		public override void SetValue(char shortName, bool value)
		{
			PropertyInfo value2;
			if (_shortNameDictionary.TryGetValue(shortName, out value2))
			{
				SetValueCore(value2, "");
			}
		}

		public override void SetValue(char shortName, string value)
		{
			PropertyInfo value2;
			if (_shortNameDictionary.TryGetValue(shortName, out value2))
			{
				SetValueCore(value2, value);
			}
		}

		public override void SetValue(char shortName, IReadOnlyList<string> values)
		{
			PropertyInfo value;
			if (_shortNameDictionary.TryGetValue(shortName, out value))
			{
				SetValueCore(value, values);
			}
		}

		public override void SetValue(string longName, bool value)
		{
			PropertyInfo value2;
			if (_longNameDictionary.TryGetValue(longName, out value2) && value2.PropertyType == typeof(bool))
			{
				value2.SetValue(_options, value);
			}
		}

		public override void SetValue(string longName, string value)
		{
			PropertyInfo value2;
			if (_longNameDictionary.TryGetValue(longName, out value2))
			{
				SetValueCore(value2, value);
			}
		}

		public override void SetValue(string longName, IReadOnlyList<string> values)
		{
			PropertyInfo value;
			if (_longNameDictionary.TryGetValue(longName, out value))
			{
				SetValueCore(value, values);
			}
		}

		
		public override void SetValue(char shortName, SingleOptimizedStrings values)
		{
			PropertyInfo value;
			if (_shortNameDictionary.TryGetValue(shortName, out value))
			{
				SetValueCore(value, values);
			}
		}

		public override void SetValue(string longName,  SingleOptimizedStrings values)
		{
			PropertyInfo value;
			if (_longNameDictionary.TryGetValue(longName, out value))
			{
				SetValueCore(value, values);
			}
		}

		private void SetValueCore(PropertyInfo property, string value)
		{
			SetValueCore(property, string.IsNullOrEmpty(value) ? null : new string[1] { value });
		}

		private void SetValueCore(PropertyInfo property,  IReadOnlyList<string> values)
		{
			Type propertyType = property.PropertyType;
			property.SetValue(_options, values.ToAssignableValue(propertyType));
		}

		public override T Commit()
		{
			return _options;
		}
	}
}
