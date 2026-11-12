using System;
using System.Windows;
using System.Windows.Input;

namespace Cvte.Paint.Framework.Input
{
	internal class BoardFocusManager
	{
		private readonly Board _board;

		public BoardFocusManager(Board board)
		{
			if (board == null)
			{
				throw new ArgumentNullException("board");
			}
			_board = board;
			board.CurrentSlideChanging += OnCurrentSlideChanging;
		}

		private void OnCurrentSlideChanging(object sender, RoutedPropertyChangedEventArgs<Slide> e)
		{
			DependencyObject dependencyObject = Keyboard.FocusedElement as DependencyObject;
			if (dependencyObject != null && Slide.GetSlide(dependencyObject) == e.OldValue)
			{
				_board.Focus();
			}
		}

		public static void Manage(Board board)
		{
			new BoardFocusManager(board);
		}
	}
}
