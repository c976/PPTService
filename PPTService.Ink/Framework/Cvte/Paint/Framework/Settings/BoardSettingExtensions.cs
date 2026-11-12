using System;
using System.Windows;
using Cvte.Paint.Framework.SelectionBoxs;

namespace Cvte.Paint.Framework.Settings
{
	public static class BoardSettingExtensions
	{
		internal static readonly DependencyPropertyKey SettingsPropertyKey = DependencyProperty.RegisterAttachedReadOnly("Settings", typeof(BoardSettingsProvider), typeof(BoardSettingExtensions), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

		private static BoardSettingsProvider GetSettings(this DependencyObject d)
		{
			BoardSettingsProvider boardSettingsProvider = (BoardSettingsProvider)(d?.GetValue(SettingsPropertyKey.DependencyProperty));
			if (boardSettingsProvider != null)
			{
				return boardSettingsProvider;
			}
			Board board = d as Board;
			if (board == null)
			{
				Slide slide = d as Slide;
				if (slide != null)
				{
					Board board2 = slide.Parent as Board;
					if (board2 != null)
					{
						return CreateBoardSettingsProvider(board2);
					}
					throw new ArgumentException("找不到 Board 设置，当前拿到 Slide 且页面的 slide.Parent 不是 Board 类，请检查 Board 的初始化设置是否正确，以及页面是否在画布上");
				}
				throw new ArgumentException("找不到Board设置而且传入的DependencyObject不是Board，当前拿到" + (d?.GetType().Name ?? "空") + "，请检查Board的初始化设置是否正确");
			}
			return CreateBoardSettingsProvider(board);
		}

		private static BoardSettingsProvider CreateBoardSettingsProvider(Board board)
		{
			BoardSettingsProvider boardSettingsProvider = new BoardSettingsProvider
			{
				Board = board
			};
			board.SetSettings(boardSettingsProvider);
			return boardSettingsProvider;
		}

		private static void SetSettings(this DependencyObject d, BoardSettingsProvider settings)
		{
			d.SetValue(SettingsPropertyKey, settings);
		}

		internal static void SetSettings(this Board board, BoardSettingsProvider settings)
		{
			((DependencyObject)board).SetSettings(settings);
		}

		[Obsolete("请用 Board.BoardSettings")]
		public static BoardSettingsProvider Settings(this Board board)
		{
			return board.GetSettings();
		}

		public static BoardSettingsProvider Settings(this Slide slide)
		{
			return slide.GetSettings();
		}

		internal static BoardSettingsProvider Settings(this Element element)
		{
			return element.GetSettings();
		}

		private static T GetSettings<T>(this DependencyObject d) where T : BoardSettings, new()
		{
			BoardSettingsProvider boardSettingsProvider = (BoardSettingsProvider)d.GetValue(SettingsPropertyKey.DependencyProperty);
			if (boardSettingsProvider == null)
			{
				boardSettingsProvider = d.GetSettings();
			}
			return boardSettingsProvider.GetSettings<T>();
		}

		public static T GetSettings<T>(this Board board) where T : BoardSettings, new()
		{
			return ((DependencyObject)board).GetSettings<T>();
		}

		public static T GetSettings<T>(this Slide slide) where T : BoardSettings, new()
		{
			return ((DependencyObject)slide).GetSettings<T>();
		}

		public static T GetSettings<T>(this Element element) where T : BoardSettings, new()
		{
			return ((DependencyObject)element).GetSettings<T>();
		}

		private static void ConfigFrom<T>(this DependencyObject d, T setting) where T : BoardSettings, new()
		{
			((BoardSettingsProvider)d.GetValue(SettingsPropertyKey.DependencyProperty)).ConfigFrom(setting);
		}

		public static void ConfigFrom<T>(this Board d, T setting) where T : BoardSettings, new()
		{
			((DependencyObject)d).ConfigFrom(setting);
		}

		public static void ConfigFrom<T>(this Slide d, T setting) where T : BoardSettings, new()
		{
			((DependencyObject)d).ConfigFrom(setting);
		}

		public static void ConfigFrom<T>(this Element d, T setting) where T : BoardSettings, new()
		{
			((DependencyObject)d).ConfigFrom(setting);
		}

		[Obsolete("现在选择框无法获得")]
		internal static BoardSettingsProvider Settings(this ISelectionBox box)
		{
			return ((DependencyObject)box).GetSettings();
		}
	}
}
