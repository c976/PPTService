using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Diagnostics.Annotations;
using PPTService.Ink.Core.Diagnostics.Trace;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils
{
	public static class TextTracer
	{
		public static class DebugTag
		{
			public const string Cursor = "Cursor";

			public const string TextRenderSize = "文本渲染大小";

			public const string ClearSelection = "文本选择";

			public const string RenderText = "文本渲染";
		}

		private static Lazy<ILogTracer> _logTracerLazy;

		private static ILogTracer _innerTracer;

		private static readonly object LockTracerObject;

		public const string TextEditor = "[TextEditor]";

		[CanBeNull]
		private static ILogTracer InnerTracer
		{
			get
			{
				if (_innerTracer == null)
				{
					lock (LockTracerObject)
					{
						if (_innerTracer == null && _logTracerLazy != null)
						{
							_innerTracer = _logTracerLazy.Value;
							_logTracerLazy = null;
						}
					}
				}
				return _innerTracer;
			}
			set
			{
				if (_innerTracer != null)
				{
					_innerTracer.Debug( () => "切换LogTracer从" + _innerTracer.GetType().FullName + "切换为" + (value?.GetType().FullName ?? "null"));
				}
				_innerTracer = value;
			}
		}

		public static DebugTags EnabledDebugTags
		{
			get
			{
				return InnerTracer?.EnabledDebugTags;
			}
			set
			{
				if (InnerTracer != null)
				{
					InnerTracer.EnabledDebugTags = value;
				}
			}
		}

		public static TraceLevel TraceLevel
		{
			get
			{
				if (InnerTracer != null)
				{
					return InnerTracer.TraceLevel;
				}
				return TraceLevel.Off;
			}
			set
			{
				if (InnerTracer != null)
				{
					InnerTracer.TraceLevel = value;
				}
			}
		}

		public static IReadOnlyCollection<string> TracerTags
		{
			get
			{
				if (InnerTracer != null)
				{
					return InnerTracer.TracerTags;
				}
				return new string[0];
			}
		}

		static TextTracer()
		{
			LockTracerObject = new object();
			_logTracerLazy = new Lazy<ILogTracer>(() => Tracer.BuildLogTracer("[TextEditor]"));
		}

		public static void SetOutputTracer(ILogTracer tracer)
		{
			InnerTracer = tracer;
		}

		internal static void LogCallMemberName([CallerMemberName] string name = "")
		{
			InnerTracer?.Debug(name);
		}

		internal static void Debug(string message, params string[] tags)
		{
			InnerTracer?.Debug(message, tags);
		}

		internal static void Debug(Func<string> messageFactory, params string[] tags)
		{
			InnerTracer?.Debug(messageFactory, tags);
		}

		internal static void Info(string message, params string[] tags)
		{
			InnerTracer?.Info(message, tags);
		}

		internal static void Info(Func<string> messageFactory, params string[] tags)
		{
			InnerTracer?.Info(messageFactory, tags);
		}

		internal static void Warning(string message, params string[] tags)
		{
			InnerTracer?.Warning(message, tags);
		}

		internal static void Warning(Func<string> messageFactory, params string[] tags)
		{
			InnerTracer?.Warning(messageFactory, tags);
		}

		internal static void Warning(Exception exception, params string[] tags)
		{
			InnerTracer?.Warning(exception, tags);
		}

		internal static void Error(string message, params string[] tags)
		{
			InnerTracer?.Error(message, tags);
		}

		internal static void Error(Exception exception, params string[] tags)
		{
			InnerTracer?.Error(exception, tags);
		}

		internal static ILogTracer BuildLogTracer(params string[] tags)
		{
			return InnerTracer?.BuildLogTracer(tags);
		}

		internal static TextTracerConditional CreateConditional(string debugTag)
		{
			return new TextTracerConditional(InnerTracer != null && InnerTracer.TraceLevel > TraceLevel.Debug && InnerTracer.EnabledDebugTags.IsTagEnable(new string[1] { debugTag }));
		}

		internal static TextTracerConditional CreateConditional(string[] debugTagList)
		{
			return new TextTracerConditional(InnerTracer != null && InnerTracer.TraceLevel > TraceLevel.Debug && InnerTracer.EnabledDebugTags.IsTagEnable(debugTagList));
		}
	}
}
