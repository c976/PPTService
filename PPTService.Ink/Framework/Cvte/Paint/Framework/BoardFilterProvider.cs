using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework
{
	public static class BoardFilterProvider
	{
		private static readonly DependencyProperty BoardAttachedFilterListProperty = DependencyProperty.RegisterAttached("BoardAttachedFilterList", typeof(BoardAttachedFilterDictionary), typeof(Board), new FrameworkPropertyMetadata(null));

		private static void SetBoardAttachedFilterDictionary(Board board, BoardAttachedFilterDictionary filterDictionary)
		{
			board.SetValue(BoardAttachedFilterListProperty, filterDictionary);
		}

		[CanBeNull]
		private static BoardAttachedFilterDictionary GetBoardAttachedFilterDictionary(Board board)
		{
			return (BoardAttachedFilterDictionary)board.GetValue(BoardAttachedFilterListProperty);
		}

		[CanBeNull]
		public static T GetFilter<T>(this Board board) where T : Filter
		{
			BoardAttachedFilterDictionary boardAttachedFilterDictionary = GetBoardAttachedFilterDictionary(board);
			Filter value;
			if (boardAttachedFilterDictionary != null && boardAttachedFilterDictionary.TryGetValue(typeof(T), out value))
			{
				return (T)value;
			}
			return null;
		}

		public static T EnableFilter<T>(this Board board) where T : Filter, new()
		{
			T val = board.GetFilter<T>();
			if (val == null)
			{
				val = new T();
				board.AddFilter(val);
			}
			val.IsEnabled = true;
			return val;
		}

		public static void AddFilter<T>(this Board board, T filter) where T : Filter
		{
			Type typeFromHandle = typeof(T);
			if (!ContainFilter(board, typeFromHandle))
			{
				BoardAttachedFilterDictionary boardAttachedFilterDictionary = GetBoardAttachedFilterDictionary(board);
				if (boardAttachedFilterDictionary == null)
				{
					boardAttachedFilterDictionary = new BoardAttachedFilterDictionary();
					SetBoardAttachedFilterDictionary(board, boardAttachedFilterDictionary);
					board.Loaded += Board_Loaded;
					board.Unloaded += Board_Unloaded;
				}
				boardAttachedFilterDictionary.Add(typeFromHandle, filter);
				if (board.IsLoaded)
				{
					board.Controller.AddFilter(filter);
					filter.IsEnabled = true;
				}
			}
		}

		public static void DisableFilter<T>(this Board board) where T : Filter
		{
			T filter = board.GetFilter<T>();
			if (filter != null)
			{
				filter.IsEnabled = false;
			}
		}

		public static void RemoveFilter<T>(this Board board) where T : Filter
		{
			BoardAttachedFilterDictionary boardAttachedFilterDictionary = GetBoardAttachedFilterDictionary(board);
			Type typeFromHandle = typeof(T);
			boardAttachedFilterDictionary?.Remove(typeFromHandle);
			foreach (T item in board.Controller.GetFilters().OfType<T>().ToList())
			{
				item.IsEnabled = false;
				board.Controller.RemoveFilter(item);
			}
		}

		internal static bool ContainFilter(Board board, Type filterType)
		{
			return GetBoardAttachedFilterDictionary(board)?.ContainsKey(filterType) ?? false;
		}

		private static void Board_Unloaded(object sender, RoutedEventArgs e)
		{
			BoardAttachedFilterDictionary boardAttachedFilterDictionary = GetBoardAttachedFilterDictionary((Board)sender);
			if (boardAttachedFilterDictionary == null)
			{
				return;
			}
			foreach (KeyValuePair<Type, Filter> item in boardAttachedFilterDictionary)
			{
				item.Value.IsEnabled = false;
			}
		}

		private static void Board_Loaded(object sender, RoutedEventArgs e)
		{
			Board board = (Board)sender;
			if (DesignerProperties.GetIsInDesignMode(board))
			{
				return;
			}
			List<Filter> list = board.Controller.GetFilters().ToList();
			BoardAttachedFilterDictionary boardAttachedFilterDictionary = GetBoardAttachedFilterDictionary(board);
			if (boardAttachedFilterDictionary == null)
			{
				return;
			}
			foreach (KeyValuePair<Type, Filter> item in boardAttachedFilterDictionary)
			{
				if (!list.Contains(item.Value))
				{
					board.Controller.AddFilter(item.Value);
				}
				item.Value.IsEnabled = true;
			}
		}
	}
}
