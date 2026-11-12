using System;
using System.Collections.Generic;
using System.Linq;
using Cvte.Diagnostics.Annotations;
using Cvte.Linq;

namespace Cvte.Paint.Framework
{
	public class PaintAggregateException : PaintException
	{
		public Exception[] InnerExceptions { get; }

		public PaintAggregateException([NotNull] string message, [NotNull] IEnumerable<Exception> innerExceptions)
			: this(message, (innerExceptions as Exception[]) ?? innerExceptions.ToArray())
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			if (innerExceptions == null)
			{
				throw new ArgumentNullException("innerExceptions");
			}
		}

		public PaintAggregateException([NotNull] string message, [NotNull] Exception innerException, [NotNull] params Exception[] innerExceptions)
			: base(message, innerException)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			if (innerException == null)
			{
				throw new ArgumentNullException("innerException");
			}
			if (innerExceptions == null)
			{
				throw new ArgumentNullException("innerExceptions");
			}
			InnerExceptions = new Exception[innerExceptions.Length + 1];
			InnerExceptions[0] = innerException;
			innerExceptions.ForEach((Exception e, int i) =>
			{
				InnerExceptions[i + 1] = e;
			});
		}

		private PaintAggregateException([NotNull] string message, [NotNull] params Exception[] innerExceptions)
			: base(message)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			if (innerExceptions == null)
			{
				throw new ArgumentNullException("innerExceptions");
			}
			InnerExceptions = innerExceptions;
		}

		public override string ToString()
		{
			return base.ToString() + Environment.NewLine + Environment.NewLine + "The inner exception" + ((InnerExceptions.Length > 1) ? "s are" : " is") + ":" + Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine + Environment.NewLine, InnerExceptions.Select((Exception e) => e.ToString()));
		}
	}
}
