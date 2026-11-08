using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cvte
{
	public class EventAggregator
	{
		private readonly IDictionary<Type, IList<Delegate>> _typeCallbackMapping;

		private readonly IDictionary<string, IList<EventHandler>> _keyCallbackMapping;

		public EventAggregator()
		{
			_typeCallbackMapping = new Dictionary<Type, IList<Delegate>>();
			_keyCallbackMapping = new Dictionary<string, IList<EventHandler>>();
		}

		public void Publish(string key)
		{
			Publish(null, key);
		}

		public void Publish(object publisher, string key)
		{
			IList<EventHandler> value;
			while (_keyCallbackMapping.TryGetValue(key, out value))
			{
				if (false)
				{
					continue;
				}
				IEnumerator<EventHandler> enumerator = value.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						enumerator.Current(publisher, EventArgs.Empty);
					}
				}
				finally
				{
					while (enumerator != null)
					{
						if (4 != 0)
						{
							enumerator.Dispose();
							break;
						}
					}
				}
				if (false)
				{
					continue;
				}
				break;
			}
		}

		public void Publish<T>(T args) where T : EventArgs
		{
			Publish(null, args);
		}

		public void Publish<T>(object publisher, T args) where T : EventArgs
		{
			IList<Delegate> value;
			while (true)
			{
				Type type = args.GetType();
				if (!_typeCallbackMapping.TryGetValue(type, out value))
				{
					if (false)
					{
						continue;
					}
					return;
				}
				break;
			}
			IEnumerator<Delegate> enumerator = value.GetEnumerator();
			try
			{
				while (6 != 0 && enumerator.MoveNext())
				{
					Delegate current = enumerator.Current;
					try
					{
						current.DynamicInvoke(publisher, args);
					}
					catch (TargetInvocationException)
					{
					}
				}
			}
			finally
			{
				if (false || 5 == 0 || false || enumerator != null)
				{
					enumerator.Dispose();
				}
			}
		}

		public void Subscribe(string key, EventHandler handler)
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler can not be null");
			}
			IList<EventHandler> value;
			if (!_keyCallbackMapping.TryGetValue(key, out value) || -1 == 0)
			{
				if (0 == 0)
				{
					IDictionary<string, IList<EventHandler>> keyCallbackMapping = _keyCallbackMapping;
					List<EventHandler> list = new List<EventHandler>();
					if (0 == 0)
					{
						list.Add(handler);
					}
					keyCallbackMapping.Add(key, list);
				}
			}
			else
			{
				value.Add(handler);
			}
		}

		public void UnsubscribeAll(string key)
		{
			_keyCallbackMapping.Remove(key);
		}

		public void Subscribe<T>(EventHandler<T> callback) where T : EventArgs
		{
			if (callback == null)
			{
				return;
			}
			Type parameterType = default(Type);
			while (true)
			{
				IL_0005:
				if (2 != 0)
				{
					parameterType = callback.Method.GetParameters()[1].ParameterType;
					IList<Delegate> value;
					while (_typeCallbackMapping.TryGetValue(parameterType, out value))
					{
						if (false)
						{
							goto IL_0005;
						}
						if (1 == 0)
						{
							return;
						}
						value.Add(callback);
						if (5 == 0 || 6 == 0)
						{
							continue;
						}
						return;
					}
				}
				_typeCallbackMapping.Add(parameterType, new List<Delegate> { callback });
				break;
			}
		}

		public void Unsubscribe<T>(EventHandler<T> callback) where T : EventArgs
		{
			Type key = default(Type);
			while (true)
			{
				bool flag;
				IList<Delegate> value;
				if (0 == 0)
				{
					if (callback == null || -1 == 0)
					{
						break;
					}
					Type parameterType = callback.Method.GetParameters()[1].ParameterType;
					if (0 == 0)
					{
						key = parameterType;
					}
					flag = _typeCallbackMapping.TryGetValue(key, out value);
					goto IL_0026;
				}
				goto IL_002f;
				IL_002f:
				flag = value.Remove(callback);
				if (true && 0 == 0)
				{
					break;
				}
				goto IL_0026;
				IL_0026:
				if (!flag)
				{
					if (5 != 0)
					{
						break;
					}
					continue;
				}
				if (value == null)
				{
					break;
				}
				goto IL_002f;
			}
		}
	}
}
