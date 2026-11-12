using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Windows.Threading;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework
{
	public static class PaintAggregateExceptionBuilder
	{
		[DebuggerStepThrough]
		public static void DoActionsWithException([NotNull] Action action, params Action[] actions)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			new Action[1] { action }.Union(actions).ForEachWithPaintException((Action a) =>
			{
				a();
			});
		}

		[DebuggerStepThrough]
		public static void DoActionsWithException([NotNull] string message, [NotNull] Action action, params Action[] actions)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			new Action[1] { action }.Union(actions).ForEachWithPaintException((Action a) =>
			{
				a();
			}, message);
		}

		[DebuggerStepThrough]
		public static void ForEachWithPaintException<T>(this IEnumerable<T> source, Action<T> action, string exceptionMessage = null)
		{
			Dictionary<T, Exception> dictionary = null;
			foreach (T item in source)
			{
				try
				{
					action(item);
				}
				catch (Exception value)
				{
					dictionary = dictionary ?? new Dictionary<T, Exception>();
					dictionary.Add(item, value);
				}
			}
			if (dictionary == null)
			{
				return;
			}
			if (dictionary.Count == 1)
			{
				ExceptionDispatchInfo.Capture(dictionary.Single().Value).Throw();
				return;
			}
			throw new PaintAggregateException((string.IsNullOrEmpty(exceptionMessage) ? ("Critical exception" + ((dictionary.Count > 1) ? "s are" : " is") + " occurred during paint operations.") : (exceptionMessage ?? "")) + Environment.NewLine + "The source" + ((dictionary.Count > 1) ? "s are" : " is") + ": " + string.Join(", ", dictionary.Keys.Select((T k) => k.GetType().FullName)) + ".", dictionary.Values);
		}

		[DebuggerStepThrough]
		internal static void InvokeThrowException(Dispatcher dispatcher, Exception ex)
		{
			(dispatcher ?? Dispatcher.CurrentDispatcher).BeginInvoke((Action)(() =>
			{
				ExceptionDispatchInfo.Capture(ex).Throw();
			}));
		}
	}
}
