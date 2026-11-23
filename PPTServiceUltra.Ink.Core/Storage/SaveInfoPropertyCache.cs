using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	internal class SaveInfoPropertyCache
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass18_0
		{
			
			public SaveInfoPropertyCache _003C_003E4__this;

			
			public List<Exception> exceptions;

			internal List<ISaveInfoMemberConverter> _003CGetConverters_003Eg__GetAttributeConverters_007C0()
			{
				return (from converter in _003C_003E4__this._propertyInfo.GetCustomAttributes<SaveInfoMemberConverterAttribute>(false).Select( (SaveInfoMemberConverterAttribute x) =>
					{
						try
						{
							return Activator.CreateInstance(x.ConverterType) as ISaveInfoMemberConverter;
						}
						catch (Exception item)
						{
							exceptions.Add(item);
							return (ISaveInfoMemberConverter)null;
						}
					})
					where converter != null
					select converter).ToList();
			}

			
			internal ISaveInfoMemberConverter _003CGetConverters_003Eb__1(SaveInfoMemberConverterAttribute x)
			{
				try
				{
					return Activator.CreateInstance(x.ConverterType) as ISaveInfoMemberConverter;
				}
				catch (Exception item)
				{
					exceptions.Add(item);
					return null;
				}
			}
		}

		[NotNull]
		private readonly PropertyInfo _propertyInfo;

		[NotNull]
		private readonly Action<object, object> _setValue;

		[CanBeNull]
		private readonly Func<object, object> _ensureInRange;

		[CanBeNull]
		private IReadOnlyList<ISaveInfoMemberConverter> _converters;

		[NotNull]
		public string Name { get; }

		[NotNull]
		public IReadOnlyList<string> Aliases { get; }

		[CanBeNull]
		public object DefaultValue { get; }

		[NotNull]
		public Type PropertyType { get; }

		public bool IsRuntime { get; }

		public SaveInfoPropertyCache([NotNull] PropertyInfo source)
		{
			if ((object)source == null)
			{
				throw new ArgumentNullException("source");
			}
			_propertyInfo = source;
			PropertyType = source.PropertyType;
			SaveInfoMemberAttribute customAttribute = source.GetCustomAttribute<SaveInfoMemberAttribute>(false);
			Name = customAttribute?.Name ?? source.GetType().Name.Replace("SaveInfo", string.Empty);
			Aliases = customAttribute?.Aliases ?? new string[0];
			DefaultValue = customAttribute?.DefaultValue;
			IsRuntime = customAttribute?.IsRuntime ?? false;
			SaveInfoMemberCheckerAttribute customAttribute2 = source.GetCustomAttribute<SaveInfoMemberCheckerAttribute>();
			_ensureInRange = ((customAttribute2 == null) ? null : new Func<object, object>(customAttribute2.Check));
			_setValue = CompiledMethod.CreatePropertySetter(source);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetValue([NotNull] object saveInfo, object value)
		{
			if (saveInfo == null)
			{
				throw new ArgumentNullException("saveInfo");
			}
			_setValue(saveInfo, value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public object EnsureInRange(object value)
		{
			if (_ensureInRange != null)
			{
				return _ensureInRange(value);
			}
			return value;
		}

		public IReadOnlyList<ISaveInfoMemberConverter> GetConverters()
		{
			_003C_003Ec__DisplayClass18_0 obj = new _003C_003Ec__DisplayClass18_0();
			obj._003C_003E4__this = this;
			if (_converters != null)
			{
				return _converters;
			}
			obj.exceptions = new List<Exception>();
			_converters = obj._003CGetConverters_003Eg__GetAttributeConverters_007C0();
			if (obj.exceptions.Count == 1)
			{
				ExceptionDispatchInfo.Capture(obj.exceptions[0]).Throw();
			}
			else if (obj.exceptions.Count > 1)
			{
				throw new AggregateException(obj.exceptions);
			}
			return _converters;
		}

		public static implicit operator SaveInfoPropertyCache(PropertyInfo source)
		{
			return new SaveInfoPropertyCache(source);
		}

		public static implicit operator PropertyInfo(SaveInfoPropertyCache @this)
		{
			return @this._propertyInfo;
		}
	}
}
