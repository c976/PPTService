using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Diagnostics.Annotations;

namespace PPTService.Ink.Core.Diagnostics.Trace
{
	
	public interface ITracer
	{
		void Trace([NotNull] string eventId, [CanBeNull] string eventValue = null, params string[] tag);

		void Trace([NotNull] string eventId, [NotNull] IEnumerable<KeyValuePair<string, string>> eventParams, params string[] tag);

		void Trace([NotNull] Exception exception, [CanBeNull] string type = null, params string[] tag);
	}
}
