using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Cvte.Composition
{
	public class TypeDetector : ITypeDetector
	{
		private class MappingTypeCollection : ITypeCollection
		{
			private readonly IDictionary<Type, Lazy<object>> _mapping;

			public MappingTypeCollection(IDictionary<Type, Lazy<object>> mapping)
			{
				_mapping = mapping;
			}

			public IEnumerable<T> OfType<T>(Predicate<Type> typeFilter)
			{
				return GetInstances<T>(_mapping.Where((KeyValuePair<Type, Lazy<object>> x) => typeFilter(x.Key)).ToDictionary((KeyValuePair<Type, Lazy<object>> x) => x.Key, (KeyValuePair<Type, Lazy<object>> x) => x.Value));
			}
		}

		[CompilerGenerated]
		private EventHandler<NewTypesDetectedEventArgs> m_NewTypesDetected;

		private readonly HashSet<Assembly> _processedAssemblies = new HashSet<Assembly>();

		private readonly IDictionary<Type, Lazy<object>> _mapping = new Dictionary<Type, Lazy<object>>();

		public event EventHandler<NewTypesDetectedEventArgs> NewTypesDetected
		{
			[CompilerGenerated]
			add
			{
				EventHandler<NewTypesDetectedEventArgs> eventHandler;
				if (0 == 0)
				{
					if (0 == 0)
					{
						eventHandler = this.m_NewTypesDetected;
					}
					goto IL_000f;
				}
				goto IL_0038;
				IL_0038:
				if (false)
				{
					goto IL_0048;
				}
				return;
				IL_0048:
				EventHandler<NewTypesDetectedEventArgs> eventHandler2 = default(EventHandler<NewTypesDetectedEventArgs>);
				EventHandler<NewTypesDetectedEventArgs> value2 = (EventHandler<NewTypesDetectedEventArgs>)Delegate.Combine(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref this.m_NewTypesDetected, value2, eventHandler2);
				goto IL_0034;
				IL_000f:
				if (7 == 0)
				{
					goto IL_0034;
				}
				EventHandler<NewTypesDetectedEventArgs> eventHandler3 = eventHandler;
				if (6u != 0)
				{
					eventHandler2 = eventHandler3;
				}
				goto IL_0048;
				IL_0034:
				if ((object)eventHandler != eventHandler2)
				{
					goto IL_000f;
				}
				goto IL_0038;
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<NewTypesDetectedEventArgs> eventHandler;
				if (0 == 0)
				{
					if (0 == 0)
					{
						eventHandler = this.m_NewTypesDetected;
					}
					goto IL_000f;
				}
				goto IL_0038;
				IL_0038:
				if (false)
				{
					goto IL_0048;
				}
				return;
				IL_0048:
				EventHandler<NewTypesDetectedEventArgs> eventHandler2 = default(EventHandler<NewTypesDetectedEventArgs>);
				EventHandler<NewTypesDetectedEventArgs> value2 = (EventHandler<NewTypesDetectedEventArgs>)Delegate.Remove(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref this.m_NewTypesDetected, value2, eventHandler2);
				goto IL_0034;
				IL_000f:
				if (7 == 0)
				{
					goto IL_0034;
				}
				EventHandler<NewTypesDetectedEventArgs> eventHandler3 = eventHandler;
				if (6u != 0)
				{
					eventHandler2 = eventHandler3;
				}
				goto IL_0048;
				IL_0034:
				if ((object)eventHandler != eventHandler2)
				{
					goto IL_000f;
				}
				goto IL_0038;
			}
		}

		public void Detect(Assembly assembly)
		{
			int num = (_processedAssemblies.Contains(assembly) ? 1 : 0);
			int num2;
			Dictionary<Type, Lazy<object>> dictionary = default(Dictionary<Type, Lazy<object>>);
			Type[] types = default(Type[]);
			int num3;
			int num4 = default(int);
			if (3 != 0)
			{
				if (num != 0)
				{
					return;
				}
				num2 = (_processedAssemblies.Add(assembly) ? 1 : 0);
				if (7 == 0)
				{
					goto IL_00bb;
				}
				dictionary = new Dictionary<Type, Lazy<object>>();
				types = assembly.GetTypes();
				num3 = 0;
				if (num3 != 0)
				{
					goto IL_00bd;
				}
				num4 = num3;
				goto IL_00be;
			}
			goto IL_00bf;
			IL_0054:
			Type type = types[num4];
			goto IL_0058;
			IL_0058:
			if (3 == 0)
			{
				goto IL_0054;
			}
			num = (_mapping.ContainsKey(type) ? 1 : 0);
			goto IL_0067;
			IL_00bd:
			num4 = num3;
			goto IL_00be;
			IL_0067:
			if (num == 0)
			{
				Type thisType = type;
				if (3 == 0)
				{
					goto IL_0058;
				}
				Lazy<object> value = new Lazy<object>(() => Create(thisType));
				_mapping.Add(thisType, value);
				dictionary.Add(thisType, value);
			}
			num2 = num4;
			goto IL_00bb;
			IL_00be:
			num = num4;
			goto IL_00bf;
			IL_00bf:
			if (0 == 0)
			{
				if (num >= types.Length)
				{
					OnNewTypesDetected(new NewTypesDetectedEventArgs(new MappingTypeCollection(dictionary), assembly));
					return;
				}
				goto IL_0054;
			}
			goto IL_0067;
			IL_00bb:
			num3 = num2 + 1;
			goto IL_00bd;
		}

		public ReadOnlyCollection<T> GetDetectedObjects<T>()
		{
			return GetInstances<T>(_mapping);
		}

		private static ReadOnlyCollection<T> GetInstances<T>(IDictionary<Type, Lazy<object>> mapping)
		{
			return new ReadOnlyCollection<T>((from x in (from x in mapping.Where((KeyValuePair<Type, Lazy<object>> x) =>
					{
						Type key = x.Key;
						int num;
						while (true)
						{
							if (4 != 0)
							{
								num = (typeof(T).IsAssignableFrom(x.Key) ? 1 : 0);
								goto IL_0016;
							}
							goto IL_0066;
							IL_0042:
							num = 0;
							if (num == 0)
							{
								break;
							}
							goto IL_0016;
							IL_0066:
							bool flag = key.IsAbstract;
							if (3 != 0)
							{
								if (flag)
								{
									goto IL_0042;
								}
								if (7 != 0)
								{
									if (1 == 0)
									{
										continue;
									}
									if (!(key.GetConstructor(Type.EmptyTypes) != null))
									{
										goto IL_0042;
									}
								}
								flag = key.ContainsGenericParameters;
							}
							return !flag;
							IL_0016:
							if (num == 0)
							{
								goto IL_0042;
							}
							goto IL_0066;
						}
						return (byte)num != 0;
					})
					select x.Value.Value).OfType<T>()
				where x != null
				select x).ToArray());
		}

		protected virtual object Create(Type type)
		{
			try
			{
				return Activator.CreateInstance(type);
			}
			catch
			{
				return null;
			}
		}

		protected virtual void OnNewTypesDetected(NewTypesDetectedEventArgs e)
		{
			this.m_NewTypesDetected?.Invoke(this, e);
		}
	}
}
