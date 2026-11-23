using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace PPTServiceUltra.Ink.Core.Windows.Media
{
	public static class BrushCreator
	{
		private static Dictionary<string, WeakReference<Brush>> BrushCacheList { get; } = new Dictionary<string, WeakReference<Brush>>();

		public static SolidColorBrush GetOrCreate(string color)
		{
			if (!color.StartsWith("#"))
			{
				throw new ArgumentException("输入的color不是有效颜色，需要使用 # 开始。 Color=" + color);
			}
			Brush brush;
			if (TryGetBrush(color, out brush))
			{
				return (SolidColorBrush)brush;
			}
			object obj;
			try
			{
				obj = ColorConverter.ConvertFromString(color);
			}
			catch (FormatException)
			{
				throw new ArgumentException("输入的color不是有效颜色。 Color=" + color);
			}
			if (obj == null)
			{
				throw new ArgumentException("输入的color不是有效颜色。 Color=" + color);
			}
			SolidColorBrush solidColorBrush = new SolidColorBrush((Color)obj);
			if (TryFreeze(solidColorBrush))
			{
				BrushCacheList.Add(color, new WeakReference<Brush>(solidColorBrush));
			}
			return solidColorBrush;
		}

		[Obsolete("请使用 GetOrCreateBrush 方法代替，此方法没有说明创建的时候会被缓存")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static SolidColorBrush Create(string color)
		{
			return GetOrCreate(color);
		}

		private static bool TryGetBrush(string key, out Brush brush)
		{
			WeakReference<Brush> value;
			if (BrushCacheList.TryGetValue(key, out value))
			{
				if (value.TryGetTarget(out brush))
				{
					return true;
				}
				BrushCacheList.Remove(key);
			}
			brush = null;
			return false;
		}

		private static bool TryFreeze(Freezable freezable)
		{
			if (freezable.CanFreeze)
			{
				freezable.Freeze();
				return true;
			}
			return false;
		}
	}
}
