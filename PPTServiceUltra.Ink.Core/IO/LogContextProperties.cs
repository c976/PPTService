using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.IO
{
	
	
	[Obsolete("此类毫无作用，请默默删除")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class LogContextProperties : IEnumerable<KeyValuePair<string, string>>, IEnumerable
	{
		[Obsolete("此方法毫无作用，请默默删除")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string this[string name]
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}

		internal LogContextProperties()
		{
		}

		[Obsolete("此方法毫无作用，请默默删除")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		
		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
