using System.Runtime.CompilerServices;
using System.Windows;

namespace Cvte.Paint.Framework
{
	internal static class ModeFilterControllerExtensions
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass0_0
		{
			public Slide slide;

			public Filter filter;

			internal void _003CSwitchOnWhenLoaded_003Eg__CurrentSlide_Loaded_007C0(object sender, RoutedEventArgs e)
			{
				slide.Loaded -= _003CSwitchOnWhenLoaded_003Eg__CurrentSlide_Loaded_007C0;
				filter.SwitchOn();
			}

			internal void _003CSwitchOnWhenLoaded_003Eg__Board_CurrentSlideChanging_007C1(object sender, RoutedPropertyChangedEventArgs<Slide> e)
			{
				slide.Loaded -= _003CSwitchOnWhenLoaded_003Eg__CurrentSlide_Loaded_007C0;
				Board board = slide.Parent as Board;
				if (board != null)
				{
					board.CurrentSlideChanging -= _003CSwitchOnWhenLoaded_003Eg__Board_CurrentSlideChanging_007C1;
				}
			}
		}

		internal static void SwitchOnWhenLoaded(this Filter filter)
		{
			_003C_003Ec__DisplayClass0_0 obj = new _003C_003Ec__DisplayClass0_0();
			obj.filter = filter;
			obj.slide = obj.filter.CurrentSlide;
			if (obj.slide == null)
			{
				return;
			}
			if (obj.slide.IsLoaded)
			{
				obj.filter.SwitchOn();
				return;
			}
			obj.slide.Loaded += obj._003CSwitchOnWhenLoaded_003Eg__CurrentSlide_Loaded_007C0;
			Board board = obj.slide.Parent as Board;
			if (board != null)
			{
				board.CurrentSlideChanging += obj._003CSwitchOnWhenLoaded_003Eg__Board_CurrentSlideChanging_007C1;
			}
		}
	}
}
