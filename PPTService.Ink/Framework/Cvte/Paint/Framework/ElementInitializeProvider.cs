using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Cvte.Paint.Framework
{
	internal static class ElementInitializeProvider
	{
		private static readonly Dictionary<string, ValueTuple<IList<Action<ElementInitializingContext>>, ElementInitializingContext>> SlideActions = new Dictionary<string, ValueTuple<IList<Action<ElementInitializingContext>>, ElementInitializingContext>>();

		public static void WaitingForLoaded(Slide slide, Action<ElementInitializingContext> action)
		{
			if (slide.IsDataLoaded)
			{
				Board board = slide.Parent as Board;
				if (board != null)
				{
					action(new ElementInitializingContext
					{
						Slide = slide,
						Board = board
					});
					return;
				}
			}
			string id = slide.Id;
			if (!SlideActions.ContainsKey(id))
			{
				SlideActions.Add(id, new ValueTuple<IList<Action<ElementInitializingContext>>, ElementInitializingContext>(new List<Action<ElementInitializingContext>>(), new ElementInitializingContext
				{
					Slide = slide
				}));
			}
			SlideActions[id].Item1.Add(action);
			slide.DataLoaded -= Slide_DataLoaded;
			slide.DataLoaded += Slide_DataLoaded;
		}

		public static void OnInsertSlide(Board board, Slide slide)
		{
			if (slide.IsDataLoaded && SlideActions.ContainsKey(slide.Id))
			{
				SlideActions[slide.Id].Item2.Board = board;
				Do(slide.Id);
			}
		}

		private static void Slide_DataLoaded(object sender, EventArgs e)
		{
			Slide slide = (Slide)sender;
			Board board = slide.Parent as Board;
			if (board != null)
			{
				slide.DataLoaded -= Slide_DataLoaded;
				ValueTuple<IList<Action<ElementInitializingContext>>, ElementInitializingContext> value;
				if (SlideActions.TryGetValue(slide.Id, out value))
				{
					value.Item2.Board = board;
					Do(slide.Id);
				}
			}
		}

		private static void Do(string id)
		{
			List<Exception> list = new List<Exception>();
			ValueTuple<IList<Action<ElementInitializingContext>>, ElementInitializingContext> valueTuple = SlideActions[id];
			IList<Action<ElementInitializingContext>> item = valueTuple.Item1;
			ElementInitializingContext item2 = valueTuple.Item2;
			foreach (Action<ElementInitializingContext> item4 in item)
			{
				try
				{
					item4(item2);
				}
				catch (Exception item3)
				{
					list.Add(item3);
				}
			}
			SlideActions.Remove(id);
			if (list.Any())
			{
				throw new AggregateException("slide加载完成事件出现异常", list);
			}
		}
	}
}
