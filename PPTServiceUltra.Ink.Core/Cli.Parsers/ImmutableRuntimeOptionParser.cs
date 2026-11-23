using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Cli.Core;
using PPTServiceUltra.Ink.Core.Cli.Utils;

namespace PPTServiceUltra.Ink.Core.Cli.Parsers
{
	
	
	internal class ImmutableRuntimeOptionParser< T> : RuntimeCommandLineOptionParser<T>, IRawCommandLineOptionParser<T>
	{
		private readonly ConstructorInfo _constructor;

		private readonly Type[] _propertyTypes;

		private readonly object[] _values;

		private readonly Dictionary<PropertyInfo, int> _indexDictionary = new Dictionary<PropertyInfo, int>();

		
		private readonly Dictionary<int, ValueTupleSlim<int, PropertyInfo>> _indexedValueDictionary = new Dictionary<int, ValueTupleSlim<int, PropertyInfo>>();

		private readonly Dictionary<char, PropertyInfo> _shortNameDictionary = new Dictionary<char, PropertyInfo>();

		private readonly Dictionary<string, PropertyInfo> _longNameDictionary = new Dictionary<string, PropertyInfo>();

		public ImmutableRuntimeOptionParser( string verb, IReadOnlyList<PropertyInfo> attributedProperties)
			: base(verb)
		{
			_values = new object[attributedProperties.Count];
			_propertyTypes = attributedProperties.Select( (PropertyInfo x) => x.PropertyType).ToArray();
			ConstructorInfo constructor = typeof(T).GetConstructor(_propertyTypes);
			if ((object)constructor == null)
			{
				throw new InvalidOperationException("Option 中必须为每个可能的命令行参数添加构造函数参数。");
			}
			_constructor = constructor;
			for (int num = 0; num < attributedProperties.Count; num++)
			{
				PropertyInfo propertyInfo = attributedProperties[num];
				_indexDictionary[propertyInfo] = num;
				if (propertyInfo.IsDefined(typeof(OptionAttribute)))
				{
					OptionAttribute customAttribute = propertyInfo.GetCustomAttribute<OptionAttribute>();
					if (customAttribute.ShortName.HasValue)
					{
						_shortNameDictionary[customAttribute.ShortName.Value] = propertyInfo;
					}
					if (customAttribute.LongName == null || string.IsNullOrWhiteSpace(customAttribute.LongName))
					{
						_longNameDictionary[propertyInfo.Name] = propertyInfo;
					}
					else
					{
						_longNameDictionary[customAttribute.LongName] = propertyInfo;
					}
				}
				if (propertyInfo.IsDefined(typeof(ValueAttribute)))
				{
					ValueAttribute customAttribute2 = propertyInfo.GetCustomAttribute<ValueAttribute>();
					_indexedValueDictionary[customAttribute2.Index] = new ValueTupleSlim<int, PropertyInfo>(customAttribute2.Length, propertyInfo);
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
				num += num3;
				_indexedValueDictionary[num2].Deconstruct(out item, out item2);
				PropertyInfo key = item2;
				int index = _indexDictionary[key];
				if (num3 == 1)
				{
					SetValueCore(index, values[num2]);
				}
				else
				{
					SetValueCore(index, values.Skip(num2).Take(num3).ToList());
				}
			}
		}

		public override void SetValue(char shortName, bool value)
		{
			_values[_indexDictionary[_shortNameDictionary[shortName]]] = value;
		}

		public override void SetValue(char shortName, string value)
		{
			SetValueCore(_indexDictionary[_shortNameDictionary[shortName]], value);
		}

		public override void SetValue(char shortName, IReadOnlyList<string> values)
		{
			SetValueCore(_indexDictionary[_shortNameDictionary[shortName]], values);
		}

		public override void SetValue(string longName, bool value)
		{
			_values[_indexDictionary[_longNameDictionary[longName]]] = value;
		}

		public override void SetValue(string longName, string value)
		{
			SetValueCore(_indexDictionary[_longNameDictionary[longName]], value);
		}

		public override void SetValue(string longName, IReadOnlyList<string> values)
		{
			SetValueCore(_indexDictionary[_longNameDictionary[longName]], values);
		}

		
		public override void SetValue(char shortName, SingleOptimizedStrings values)
		{
			SetValueCore(_indexDictionary[_shortNameDictionary[shortName]], values);
		}

		public override void SetValue(string longName,  SingleOptimizedStrings values)
		{
			SetValueCore(_indexDictionary[_longNameDictionary[longName]], values);
		}

		private void SetValueCore(int index, string value)
		{
			SetValueCore(index, string.IsNullOrEmpty(value) ? null : new string[1] { value });
		}

		private void SetValueCore(int index,  IReadOnlyList<string> values)
		{
			Type assignableType = _propertyTypes[index];
			_values[index] = values.ToAssignableValue(assignableType);
		}

		public override T Commit()
		{
			return (T)_constructor.Invoke(_values);
		}
	}
}
