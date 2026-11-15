using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core
{
	
	
	public class ConcurrentEventAggregator
	{
		private readonly ConcurrentDictionary<Type, IList<Delegate>> _typeCallbackMapping;

		private readonly ConcurrentDictionary<string, IList<EventHandler>> _keyCallbackMapping;

		private object _locker = new object();

		public ConcurrentEventAggregator()
		{
			_typeCallbackMapping = new ConcurrentDictionary<Type, IList<Delegate>>();
			_keyCallbackMapping = new ConcurrentDictionary<string, IList<EventHandler>>();
		}

		public void Publish(string key)
		{
			Publish(null, key);
		}

		public void Publish(object publisher, string key)
		{
			IList<EventHandler> value;
			if (!_keyCallbackMapping.TryGetValue(key, out value))
			{
				return;
			}
			foreach (EventHandler item in value)
			{
				item(publisher, EventArgs.Empty);
			}
		}

		public void Publish< T>(T args) where T : EventArgs
		{
			Publish(null, args);
		}

		public void Publish< T>(object publisher, T args) where T : EventArgs
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			Type type = args.GetType();
			IList<Delegate> value;
			if (!_typeCallbackMapping.TryGetValue(type, out value))
			{
				return;
			}
			foreach (Delegate item in value)
			{
				try
				{
					item.DynamicInvoke(publisher, args);
				}
				catch (TargetInvocationException)
				{
				}
			}
		}

		public void Subscribe(string key, EventHandler handler)
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			lock (_locker)
			{
				IList<EventHandler> value;
				if (!_keyCallbackMapping.TryGetValue(key, out value))
				{
					_keyCallbackMapping.TryAdd(key, new List<EventHandler> { handler });
				}
				else
				{
					value.Add(handler);
				}
			}
		}

		[Obsolete("不要用这个取消全局", false)]
		public void UnsubscribeAll(string key)
		{
			IList<EventHandler> value;
			_keyCallbackMapping.TryRemove(key, out value);
		}

		[Obsolete("使用Subscribe(string key, EventHandler handler)代替", false)]
		public void Subscribe< T>(EventHandler<T> callback) where T : EventArgs
		{
			if (callback == null)
			{
				return;
			}
			Type parameterType = callback.Method.GetParameters()[1].ParameterType;
			lock (_locker)
			{
				IList<Delegate> value;
				if (!_typeCallbackMapping.TryGetValue(parameterType, out value))
				{
					_typeCallbackMapping.TryAdd(parameterType, new List<Delegate> { callback });
				}
				else
				{
					value.Add(callback);
				}
			}
		}

		[Obsolete("使用Unsubscribe(string key, EventHandler handler)代替", false)]
		public void Unsubscribe< T>(EventHandler<T> callback) where T : EventArgs
		{
			if (callback == null)
			{
				return;
			}
			Type parameterType = callback.Method.GetParameters()[1].ParameterType;
			IList<Delegate> value;
			if (!_typeCallbackMapping.TryGetValue(parameterType, out value))
			{
				return;
			}
			lock (_locker)
			{
				value.Remove(callback);
			}
		}

		public void Unsubscribe(string key, EventHandler handler)
		{
			IList<EventHandler> value;
			if (handler == null || !_keyCallbackMapping.TryGetValue(key, out value))
			{
				return;
			}
			lock (_locker)
			{
				value.Remove(handler);
			}
		}
	}
}
