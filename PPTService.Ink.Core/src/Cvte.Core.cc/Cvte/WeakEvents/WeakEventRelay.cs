using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.WeakEvents
{
	
	
	internal abstract class WeakEventRelay<TEventSource> where TEventSource : class
	{
		private readonly TEventSource _eventSource;

		private readonly ConcurrentDictionary<string, string> _events = new ConcurrentDictionary<string, string>();

		protected WeakEventRelay(TEventSource eventSource)
		{
			if (eventSource == null)
			{
				throw new ArgumentNullException("eventSource");
			}
			_eventSource = eventSource;
		}

		protected void Subscribe(Action<TEventSource> sourceEventAdder, Action relayEventAdder, [CallerMemberName] string eventName = null)
		{
			if (eventName == null)
			{
				throw new ArgumentNullException("eventName");
			}
			if (_events.TryAdd(eventName, eventName))
			{
				sourceEventAdder(_eventSource);
			}
			relayEventAdder();
		}

		protected void TryInvoke< TSender,  TArgs>(WeakEvent<TSender, TArgs> weakEvent, TSender sender, TArgs e)
		{
			if (!weakEvent.Invoke(sender, e))
			{
				OnReferenceLost(_eventSource);
			}
		}

		protected abstract void OnReferenceLost(TEventSource source);
	}
}
