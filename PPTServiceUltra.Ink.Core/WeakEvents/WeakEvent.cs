using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.WeakEvents
{
	
	
	internal class WeakEvent< TSender,  TArgs>
	{
		
		private sealed class WeakEventHandler
		{
			
			
			internal object Target
			{
				
				get;
				
				private set;
			}

			private Dictionary<MethodInfo, List<Action<TSender, TArgs>>> MethodHandlers { get; } = new Dictionary<MethodInfo, List<Action<TSender, TArgs>>>();

			internal void Add(MulticastDelegate handler, Action<TSender, TArgs> castedHandler)
			{
				if ((object)handler == null)
				{
					throw new ArgumentNullException("handler");
				}
				if (Target != null && Target != handler.Target)
				{
					throw new ArgumentException("如果代码没有写错，不可能在这里传入不一致的 Target。", "handler");
				}
				Target = handler.Target;
				List<Action<TSender, TArgs>> value;
				if (MethodHandlers.TryGetValue(handler.Method, out value))
				{
					value.Add(castedHandler);
					return;
				}
				value = new List<Action<TSender, TArgs>> { castedHandler };
				MethodHandlers[handler.Method] = value;
			}

			internal void Remove(MulticastDelegate handler)
			{
				if ((object)handler == null)
				{
					throw new ArgumentNullException("handler");
				}
				if (Target != null && Target != handler.Target)
				{
					throw new ArgumentException("如果代码没有写错，不可能在这里传入不一致的 Target。", "handler");
				}
				Target = handler.Target;
				List<Action<TSender, TArgs>> value;
				if (MethodHandlers.TryGetValue(handler.Method, out value))
				{
					value.RemoveAt(value.Count - 1);
					if (value.Count == 0)
					{
						MethodHandlers.Remove(handler.Method);
					}
				}
			}

			internal IReadOnlyList<Action<TSender, TArgs>> GetInvokingHandlers()
			{
				return MethodHandlers.SelectMany( (KeyValuePair<MethodInfo, List<Action<TSender, TArgs>>> x) => x.Value).ToList();
			}
		}

		private readonly object _locker = new object();

		private readonly List<WeakReference<object>> _relatedInstances = new List<WeakReference<object>>();

		
		private readonly ConditionalWeakTable<object, WeakEventHandler> _handlers = new ConditionalWeakTable<object, WeakEventHandler>();

		public void Add(MulticastDelegate originalHandler, Action<TSender, TArgs> castedHandler)
		{
			lock (_locker)
			{
				object target = originalHandler.Target;
				if (target == null)
				{
					throw new ArgumentException("无法将弱事件应用于没有对象的纯函数中。", "originalHandler");
				}
				object target2 = target;
				MethodInfo method = originalHandler.Method;
				WeakReference<object> weakReference = _relatedInstances.Find((WeakReference<object> x) =>
				{
					object target3;
					return x.TryGetTarget(out target3) && target2 == target3;
				});
				if (weakReference == null)
				{
					weakReference = new WeakReference<object>(target2);
					_relatedInstances.Add(weakReference);
					WeakEventHandler weakEventHandler = new WeakEventHandler();
					weakEventHandler.Add(originalHandler, castedHandler);
					_handlers.Add(target2, weakEventHandler);
				}
				else
				{
					WeakEventHandler value;
					if (!_handlers.TryGetValue(target2, out value))
					{
						throw new InvalidOperationException("有一个已经被 GC 掉的对象正在试图注册事件处理函数，可能代码写错了。");
					}
					value.Add(originalHandler, castedHandler);
				}
			}
		}

		public void Remove(MulticastDelegate originalHandler)
		{
			lock (_locker)
			{
				object target = originalHandler.Target;
				if (target == null)
				{
					throw new ArgumentException("无法将弱事件应用于没有方法的纯对象中。", "originalHandler");
				}
				object target2 = target;
				if (_relatedInstances.Find((WeakReference<object> x) =>
				{
					object target3;
					return x.TryGetTarget(out target3) && target2 == target3;
				}) != null)
				{
					WeakEventHandler value;
					if (!_handlers.TryGetValue(target2, out value))
					{
						throw new InvalidOperationException("有一个已经被 GC 掉的对象正在试图注销事件处理函数，可能代码写错了。");
					}
					value.Remove(originalHandler);
				}
			}
		}

		public bool Invoke(TSender sender, TArgs e)
		{
			List<Action<TSender, TArgs>> list = null;
			lock (_locker)
			{
				List<WeakEventHandler> list2 = _relatedInstances.ConvertAll( ( WeakReference<object> x) =>
				{
					object target;
					WeakEventHandler value;
					return (!x.TryGetTarget(out target) || !_handlers.TryGetValue(target, out value)) ? null : value;
				});
				if (list2.Exists( (WeakEventHandler x) => x != null))
				{
					list = list2.OfType<WeakEventHandler>().SelectMany( (WeakEventHandler x) => x.GetInvokingHandlers()).ToList();
				}
				else
				{
					list = null;
					_relatedInstances.Clear();
				}
			}
			if (list != null)
			{
				foreach (Action<TSender, TArgs> item in list)
				{
					item(sender, e);
				}
			}
			return list != null;
		}
	}
	
	
	internal class WeakEvent<TArgs> : WeakEvent<object, TArgs>
	{
	}
}
