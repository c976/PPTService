using System;
using System.Collections.Generic;
using System.Linq;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Diagnostics
{
	public static class InstanceWatcher
	{
		private static readonly Dictionary<string, HashSet<WeakReference>> WeakReferences = new Dictionary<string, HashSet<WeakReference>>();

		public static void Watch([NotNull] object obj)
		{
			WathcerInternal("", obj);
		}

		public static void Watch([NotNull] object obj, [NotNull] string tag)
		{
			WathcerInternal(tag, obj);
		}

		public static int CountSurvivedInstances()
		{
			return WeakReferences.Sum((KeyValuePair<string, HashSet<WeakReference>> pair) => pair.Value.Count((WeakReference x) => x.Target != null));
		}

		public static int CountSurvivedInstances([NotNull] string key)
		{
			HashSet<WeakReference> value;
			if (0 == 0)
			{
				WeakReferences.TryGetValue(key, out value);
			}
			while (true)
			{
				if (value == null)
				{
					break;
				}
				int result = value.Count((WeakReference x) => x.Target != null);
				while (0 == 0)
				{
					if (0 == 0)
					{
						return result;
					}
					int num;
					if (num == 0)
					{
						goto end_IL_004c;
					}
				}
				continue;
				end_IL_004c:
				break;
			}
			return 0;
		}

		private static void WathcerInternal([NotNull] string tag, [NotNull] object obj)
		{
			do
			{
				if (tag == null)
				{
					throw new ArgumentNullException("tag");
				}
				if (obj == null)
				{
					throw new ArgumentNullException("obj");
				}
			}
			while (4 == 0);
			if (obj is ValueType)
			{
				throw new ArgumentException("obj 必须是一个引用类型。");
			}
			if (!WeakReferences.ContainsKey(tag))
			{
				WeakReferences[tag] = new HashSet<WeakReference>();
			}
			else if (WeakReferences[tag].Any((WeakReference weakReference) => weakReference.Target == obj))
			{
				throw new InvalidOperationException($"{obj}已被监视，不能重复监视。");
			}
			WeakReferences[tag].Add(new WeakReference(obj));
		}

		public static void Reset()
		{
			WeakReferences.Clear();
		}
	}
}
