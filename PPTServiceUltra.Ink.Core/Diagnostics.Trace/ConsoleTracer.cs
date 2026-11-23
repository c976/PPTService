#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace PPTServiceUltra.Ink.Core.Diagnostics.Trace
{
	
	
	internal class ConsoleTracer : ITracerListener, ITracer
	{
		private static readonly Lazy<ConsoleTracer> TracerLazy = new Lazy<ConsoleTracer>(() => new ConsoleTracer(), LazyThreadSafetyMode.PublicationOnly);

		internal static ConsoleTracer Instance => TracerLazy.Value;

		private ConsoleTracer()
		{
		}

		public void Trace(string eventId, string eventValue = null, params string[] tag)
		{
			if (string.IsNullOrWhiteSpace(eventId))
			{
				throw new ArgumentNullException("eventId");
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(eventId);
			stringBuilder.Append(" ");
			stringBuilder.Append(eventValue);
			stringBuilder.Append(" ");
			TagToString(tag, stringBuilder);
			System.Diagnostics.Trace.WriteLine(stringBuilder.ToString());
		}

		public void Trace(string eventId,  IEnumerable<KeyValuePair<string, string>> eventParams, params string[] tag)
		{
			if (string.IsNullOrWhiteSpace(eventId))
			{
				throw new ArgumentNullException("eventId");
			}
			if (eventParams == null)
			{
				throw new ArgumentNullException("eventParams");
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(eventId);
			TagToString(tag, stringBuilder);
			stringBuilder.AppendLine();
			foreach (KeyValuePair<string, string> eventParam in eventParams)
			{
				stringBuilder.Append(eventParam.Key);
				stringBuilder.Append(":");
				stringBuilder.Append(eventParam.Value);
				stringBuilder.AppendLine();
			}
			System.Diagnostics.Trace.WriteLine(stringBuilder.ToString());
		}

		public void Trace(Exception exception, string type = null, params string[] tag)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(type))
			{
				stringBuilder.Append("模块:");
				stringBuilder.Append(type);
				stringBuilder.AppendLine("发生异常");
			}
			TagToString(tag, stringBuilder);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(exception.ToString());
			System.Diagnostics.Trace.WriteLine(stringBuilder.ToString());
		}

		private static void TagToString(string[] tag, StringBuilder str)
		{
			if (tag != null && tag.Length != 0)
			{
				str.Append("[");
				str.Append(string.Join(",", tag));
				str.Append("]");
			}
		}
	}
}
