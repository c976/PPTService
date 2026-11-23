using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Ink.Core.Diagnostics.Trace
{
	
	
	public class LogTracer : ILogTracer, ITracer
	{
		
		public static class Context
		{
			public const string Debug = "[Debug]";

			public const string Info = "[Info]";

			public const string Warning = "[Warning]";

			public const string Error = "[Error]";
		}

		private readonly string[] _tracerTags;

		private DebugTags _enabledDebugTags;

		private bool? _isCurrentTracerAlwaysEnableDebugTag;

		public ITracer OutputTracer { get; }

		public DebugTags EnabledDebugTags
		{
			get
			{
				return _enabledDebugTags;
			}
			set
			{
				_enabledDebugTags = value;
				if (_isCurrentTracerAlwaysEnableDebugTag.HasValue)
				{
					_isCurrentTracerAlwaysEnableDebugTag = EnabledDebugTags.IsTagEnable(_tracerTags);
				}
			}
		}

		public TraceLevel TraceLevel { get; set; }

		public IReadOnlyCollection<string> TracerTags => _tracerTags;

		private bool IsCurrentTracerAlwaysEnableDebugTag
		{
			get
			{
				bool? isCurrentTracerAlwaysEnableDebugTag = _isCurrentTracerAlwaysEnableDebugTag;
				if (!isCurrentTracerAlwaysEnableDebugTag.HasValue)
				{
					string[] tracerTags = _tracerTags;
					if (tracerTags == null || tracerTags.Length == 0)
					{
						_isCurrentTracerAlwaysEnableDebugTag = false;
					}
					else
					{
						_isCurrentTracerAlwaysEnableDebugTag = EnabledDebugTags.IsTagEnable(tracerTags);
					}
				}
				return _isCurrentTracerAlwaysEnableDebugTag.Value;
			}
		}

		internal LogTracer(ITracer outputTracer, string[] tracerTags, TraceLevel traceLevel, DebugTags enabledDebugTags)
		{
			_tracerTags = tracerTags;
			OutputTracer = outputTracer;
			TraceLevel = traceLevel;
			EnabledDebugTags = enabledDebugTags;
		}

		public void Debug(string message, params string[] tags)
		{
			if (tags == null)
			{
				throw new ArgumentNullException("tags");
			}
			if (TraceLevel >= TraceLevel.Debug && (IsCurrentTracerAlwaysEnableDebugTag || EnabledDebugTags.IsTagEnable(tags)))
			{
				string text = string.Empty;
				if (tags.Length != 0)
				{
					text = " " + string.Join(",", tags);
				}
				Trace("[Debug]" + text, message, BuildTags(tags, "[Debug]"));
			}
		}

		public void Debug([NotNull] Func<string> getMessage, params string[] tags)
		{
			if (getMessage == null)
			{
				throw new ArgumentNullException("getMessage");
			}
			if (tags == null)
			{
				throw new ArgumentNullException("tags");
			}
			if (TraceLevel >= TraceLevel.Debug && (IsCurrentTracerAlwaysEnableDebugTag || EnabledDebugTags.IsTagEnable(tags)))
			{
				string text = string.Empty;
				if (tags.Length != 0)
				{
					text = " " + string.Join(",", tags);
				}
				Trace("[Debug]" + text, getMessage(), BuildTags(tags, "[Debug]"));
			}
		}

		public void Info(string message, params string[] tags)
		{
			if (TraceLevel >= TraceLevel.Info)
			{
				Trace("[Info]", message, BuildTags(tags, "[Info]"));
			}
		}

		public void Info([NotNull] Func<string> getMessage, params string[] tags)
		{
			if (getMessage == null)
			{
				throw new ArgumentNullException("getMessage");
			}
			if (TraceLevel >= TraceLevel.Info)
			{
				Trace("[Info]", getMessage(), BuildTags(tags, "[Info]"));
			}
		}

		public void Warning(string message, params string[] tags)
		{
			if (TraceLevel >= TraceLevel.Warning)
			{
				Trace("[Warning]", message, BuildTags(tags, "[Warning]"));
			}
		}

		public void Warning([NotNull] Func<string> messageFactory, params string[] tags)
		{
			if (messageFactory == null)
			{
				throw new ArgumentNullException("messageFactory");
			}
			if (TraceLevel >= TraceLevel.Warning)
			{
				Trace("[Warning]", messageFactory(), BuildTags(tags, "[Warning]"));
			}
		}

		public void Warning([NotNull] Exception exception, params string[] tags)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			if (TraceLevel >= TraceLevel.Warning)
			{
				Trace("[Warning]", exception.ToString(), tags);
			}
		}

		public void Error(string message, params string[] tags)
		{
			if (TraceLevel >= TraceLevel.Error)
			{
				Trace("[Error]", message, BuildTags(tags, "[Error]"));
			}
		}

		public void Error([NotNull] Exception exception, params string[] tags)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			if (TraceLevel >= TraceLevel.Error)
			{
				Trace("[Error]", exception.ToString(), BuildTags(tags, "[Error]"));
			}
		}

		public ILogTracer BuildLogTracer(params string[] tags)
		{
			return new LogTracer(OutputTracer, MergeArrays(new string[2][] { _tracerTags, tags }), TraceLevel, EnabledDebugTags);
		}

		public void Trace(string eventId, string eventValue = null, params string[] tag)
		{
			OutputTracer.Trace(eventId, eventValue, tag);
		}

		public void Trace(string eventId,  IEnumerable<KeyValuePair<string, string>> eventParams, params string[] tag)
		{
			OutputTracer.Trace(eventId, eventParams, tag);
		}

		public void Trace(Exception exception, string type = null, params string[] tag)
		{
			OutputTracer.Trace(exception, type, tag);
		}

		private string[] BuildTags(string[] tags, params string[] addTags)
		{
			return MergeArrays(new string[3][] { _tracerTags, addTags, tags });
		}

		private static T[] MergeArrays< T>(T[][] mergeList)
		{
			T[] array = new T[mergeList.Sum( (T[] temp) => temp.Length)];
			int num = 0;
			foreach (T[] array2 in mergeList)
			{
				Array.Copy(array2, 0, array, num, array2.Length);
				num += array2.Length;
			}
			return array;
		}
	}
}
