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
			if (!_keyCallbackMapping.TryGetValue(key, out value))
			{
				return;
			}
			foreach (EventHandler item in value)
			{
				item(publisher, EventArgs.Empty);
			}
		}

		public void Publish<T>(T args) where T : EventArgs
		{
			Publish(null, args);
		}

		public void Publish<T>(object publisher, T args) where T : EventArgs
		{
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
				throw new ArgumentNullException("handler can not be null");
			}
			IList<EventHandler> value;
			if (!_keyCallbackMapping.TryGetValue(key, out value))
			{
				_keyCallbackMapping.Add(key, new List<EventHandler> { handler });
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
			if (callback != null)
			{
				Type parameterType = callback.Method.GetParameters()[1].ParameterType;
				IList<Delegate> value;
				if (!_typeCallbackMapping.TryGetValue(parameterType, out value))
				{
					_typeCallbackMapping.Add(parameterType, new List<Delegate> { callback });
				}
				else
				{
					value.Add(callback);
				}
			}
		}

		public void Unsubscribe<T>(EventHandler<T> callback) where T : EventArgs
		{
			if (callback != null)
			{
				Type parameterType = callback.Method.GetParameters()[1].ParameterType;
				IList<Delegate> value;
				if (_typeCallbackMapping.TryGetValue(parameterType, out value))
				{
					value?.Remove(callback);
				}
			}
		}
	}
}
