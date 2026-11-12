using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PPTService.Core.Net.WebServices.EventReport.Models;

namespace PPTService.Core.Net.WebServices.EventReport
{
	
	
	internal class InnerEventReporter
	{
		private readonly ConcurrentQueue<Event> _events = new ConcurrentQueue<Event>();

		
		private readonly ConcurrentQueue<ValueTuple<IList<Event>, IDictionary<string, object>>> _reportingEvents = new ConcurrentQueue<ValueTuple<IList<Event>, IDictionary<string, object>>>();

		private volatile bool _isReporting;

		private PostEventWebService WebService { get; }

		private IEventCommonPropertiesProvider CommonPropertiesProvider { get; }

		private PostEventDataCache EventDataCache { get; }

		public InnerEventReporter(string fridayAppId, IEventCommonPropertiesProvider commonPropertiesProvider, bool includeDefaultProperties)
		{
			if (commonPropertiesProvider == null)
			{
				throw new ArgumentNullException("commonPropertiesProvider");
			}
			CommonPropertiesProvider = commonPropertiesProvider;
			WebService = new PostEventWebService(fridayAppId, commonPropertiesProvider)
			{
				IncludeDefaultProperties = includeDefaultProperties
			};
			EventDataCache = new PostEventDataCache(fridayAppId, WebService)
			{
				IncludeDefaultProperties = includeDefaultProperties
			};
		}

		internal async Task ReportEvent(Event theEvent)
		{
			_events.Enqueue(theEvent);
			if (_events.Count >= 10)
			{
				IList<Event> item = DequeueEvents();
				IDictionary<string, object> item2 = CommonPropertiesProvider.BuildCommonProperties();
				_reportingEvents.Enqueue(new ValueTuple<IList<Event>, IDictionary<string, object>>(item, item2));
				await ReportEventByQueue();
			}
		}

		internal async Task ReportEventImmediately(Event theEvent)
		{
			Event[] item = new Event[1] { theEvent };
			IDictionary<string, object> item2 = CommonPropertiesProvider.BuildCommonProperties();
			_reportingEvents.Enqueue(new ValueTuple<IList<Event>, IDictionary<string, object>>(item, item2));
			await ReportEventByQueue();
		}

		public async Task FlushAsync()
		{
			if (_events.Any())
			{
				IList<Event> events = DequeueEvents();
				IDictionary<string, object> properties = CommonPropertiesProvider.BuildCommonProperties();
				await Task.Run( async () =>
				{
					await WebService.PostEvent(events, properties).ConfigureAwait(false);
				}).ConfigureAwait(false);
			}
		}

		public async Task CacheDataAsync()
		{
			IList<Event> events = DequeueEvents();
			IDictionary<string, object> commonProperties = CommonPropertiesProvider.BuildCommonProperties();
			await EventDataCache.CacheDataAsync(events, commonProperties).ConfigureAwait(false);
		}

		public async Task FlushCacheDataAsync()
		{
			await EventDataCache.FlushCacheDataAsync().ConfigureAwait(false);
		}

		private IList<Event> DequeueEvents()
		{
			List<Event> list = new List<Event>();
			Event result;
			while (_events.TryDequeue(out result))
			{
				list.Add(result);
			}
			return list;
		}

		private async Task ReportEventByQueue()
		{
			lock (_reportingEvents)
			{
				if (_isReporting)
				{
					return;
				}
				_isReporting = true;
			}
			await Task.Run( async () =>
			{
				ValueTuple<IList<Event>, IDictionary<string, object>> result;
				while (_reportingEvents.TryDequeue(out result))
				{
					await WebService.PostEvent(result.Item1, result.Item2).ConfigureAwait(false);
					lock (_reportingEvents)
					{
						if (_reportingEvents.IsEmpty)
						{
							_isReporting = false;
							break;
						}
					}
				}
			});
		}
	}
}
