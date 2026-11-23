using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Ink.Core.Diagnostics.Trace
{
	
	
	public class DebugTags
	{
		private HashSet<string> EnabledDebugTags { get; }

		public DebugTags()
		{
			EnabledDebugTags = new HashSet<string>();
		}

		public DebugTags([NotNull] HashSet<string> enabledDebugTags)
		{
			if (enabledDebugTags == null)
			{
				throw new ArgumentNullException("enabledDebugTags");
			}
			EnabledDebugTags = enabledDebugTags;
		}

		public bool IsTagEnable(string[] tags)
		{
			if (tags == null || tags.Length == 0)
			{
				return true;
			}
			if (EnabledDebugTags.Count != 0)
			{
				return tags.Any( (string temp) => EnabledDebugTags.Contains(temp));
			}
			return true;
		}
	}
}
