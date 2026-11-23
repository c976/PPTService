using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PPTServiceUltra.Ink.Core.Windows
{
	public class AdaptiveTriggerCollection : Collection<AdaptiveCondition>, IList, ICollection, IEnumerable
	{
		internal SortedList<double, AdaptiveCondition> WidthSortedList { get; } = new SortedList<double, AdaptiveCondition>();

		internal SortedList<double, AdaptiveCondition> HeightSortedList { get; } = new SortedList<double, AdaptiveCondition>();

		
		
		internal SortedList<double, ValueTuple<Size, AdaptiveCondition>> SizeSortedList
		{
			
			get;
		} = new SortedList<double, ValueTuple<Size, AdaptiveCondition>>();

		public AdaptiveTriggerCollection()
		{
		}

		public AdaptiveTriggerCollection(IList<AdaptiveCondition> items)
			: base(items)
		{
			foreach (AdaptiveCondition item in items)
			{
				if (!double.IsNaN(item.MinWindowWidth))
				{
					WidthSortedList.Add(item.MinWindowWidth, item);
				}
				if (!double.IsNaN(item.MinWindowHeight))
				{
					HeightSortedList.Add(item.MinWindowHeight, item);
				}
				if (Size.Empty != item.MinWindowSize)
				{
					SizeSortedList.Add(item.MinWindowSize.Width + item.MinWindowSize.Height, new ValueTuple<Size, AdaptiveCondition>(item.MinWindowSize, item));
				}
			}
		}

		protected override void InsertItem(int index, AdaptiveCondition item)
		{
			base.InsertItem(index, item);
			if (!double.IsNaN(item.MinWindowWidth))
			{
				WidthSortedList.Add(item.MinWindowWidth, item);
			}
			if (!double.IsNaN(item.MinWindowHeight))
			{
				HeightSortedList.Add(item.MinWindowHeight, item);
			}
			if (Size.Empty != item.MinWindowSize)
			{
				SizeSortedList.Add(item.MinWindowSize.Width + item.MinWindowSize.Height, new ValueTuple<Size, AdaptiveCondition>(item.MinWindowSize, item));
			}
		}

		protected override void RemoveItem(int index)
		{
			AdaptiveCondition adaptiveCondition = base[index];
			base.RemoveItem(index);
			if (!double.IsNaN(adaptiveCondition.MinWindowWidth))
			{
				WidthSortedList.Remove(adaptiveCondition.MinWindowWidth);
			}
			if (!double.IsNaN(adaptiveCondition.MinWindowHeight))
			{
				HeightSortedList.Remove(adaptiveCondition.MinWindowHeight);
			}
			if (Size.Empty != adaptiveCondition.MinWindowSize)
			{
				SizeSortedList.Remove(adaptiveCondition.MinWindowSize.Width + adaptiveCondition.MinWindowSize.Height);
			}
		}

		protected override void SetItem(int index, AdaptiveCondition item)
		{
			AdaptiveCondition adaptiveCondition = base[index];
			if (!double.IsNaN(adaptiveCondition.MinWindowWidth))
			{
				WidthSortedList.Remove(adaptiveCondition.MinWindowWidth);
			}
			if (!double.IsNaN(adaptiveCondition.MinWindowHeight))
			{
				HeightSortedList.Remove(adaptiveCondition.MinWindowHeight);
			}
			if (Size.Empty != adaptiveCondition.MinWindowSize)
			{
				SizeSortedList.Remove(adaptiveCondition.MinWindowSize.Width + adaptiveCondition.MinWindowSize.Height);
			}
			if (!double.IsNaN(item.MinWindowWidth))
			{
				WidthSortedList.Add(item.MinWindowWidth, item);
			}
			if (!double.IsNaN(item.MinWindowHeight))
			{
				HeightSortedList.Add(item.MinWindowHeight, item);
			}
			if (Size.Empty != item.MinWindowSize)
			{
				SizeSortedList.Add(adaptiveCondition.MinWindowSize.Width + adaptiveCondition.MinWindowSize.Height, new ValueTuple<Size, AdaptiveCondition>(item.MinWindowSize, item));
			}
		}

		protected override void ClearItems()
		{
			base.ClearItems();
			WidthSortedList.Clear();
			HeightSortedList.Clear();
			SizeSortedList.Clear();
		}
	}
}
