using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using PPTService.Core.Net.WebServices.EventReport;

namespace PPTService.Collection
{
	public static class FridayReporter
	{
		public static bool IsInitialize;

		public static readonly object InitializeLocker = new object();

		private static Lazy<EventReporter> _theEventReporter;

		private static readonly ConcurrentDictionary<string, string> EventCache = new ConcurrentDictionary<string, string>();

		public static bool IsReportEnabled
		{
			get
			{
				if (IsInitialize)
				{
					return _theEventReporter != null;
				}
				return false;
			}
		}

		public static void Initialize(string appName, string appVersion)
		{
			lock (InitializeLocker)
			{
				if (IsInitialize)
				{
					return;
				}
				IsInitialize = true;
			}
			_theEventReporter = new Lazy<EventReporter>(() =>
			{
				EventReporter eventReporter = new EventReporter(CollectionConfig.FridayKey, new CommonPropertiesProvider(appName, appVersion));
				foreach (KeyValuePair<string, string> item in EventCache)
				{
					if (item.Value == null)
					{
						eventReporter.ReportEventImmediately(item.Key.Trim());
					}
					else
					{
						eventReporter.ReportEventImmediately(item.Key.Trim(), item.Value);
					}
				}
				return eventReporter;
			});
		}

		public static void ReportEventImmediately(string eventId)
		{
			if (!IsReportEnabled)
			{
				Console.WriteLine(eventId.Trim());
			}
			else
			{
				_theEventReporter.Value.ReportEventImmediately(eventId.Trim());
			}
		}

		public static void ReportEventImmediately(string eventId, string eventValue)
		{
			if (!IsReportEnabled)
			{
				Console.WriteLine(eventId.Trim() + " : " + eventValue);
			}
			else
			{
				_theEventReporter.Value.ReportEventImmediately(eventId.Trim(), eventValue);
			}
		}

		public static void ReportEventDelayed(string eventId)
		{
			if (!IsReportEnabled)
			{
				Console.WriteLine(eventId.Trim());
			}
			else if (_theEventReporter.IsValueCreated)
			{
				ReportEventImmediately(eventId);
			}
			else
			{
				EventCache.TryAdd(eventId, null);
			}
		}

		public static void ReportEventDelayed(string eventId, string eventValue)
		{
			if (!IsReportEnabled)
			{
				Console.WriteLine(eventId.Trim());
			}
			else if (_theEventReporter.IsValueCreated)
			{
				ReportEventImmediately(eventId, eventValue);
			}
			else
			{
				EventCache.TryAdd(eventId, eventValue);
			}
		}
	}
}
