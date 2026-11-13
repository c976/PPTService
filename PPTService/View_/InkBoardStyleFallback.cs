using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Cvte.Paint.Framework;

namespace PPTService
{
	/// <summary>
	/// 白板默认样式兜底。
	/// PPTService.Ink 的 themes/*.xaml 是 Page（BAML），只有 VS2015/MSBuild 14 的
	/// XAML 标记编译才会嵌入 .g.resources；用 dotnet build / MSBuild 17 编译老式 csproj
	/// 时不会执行标记编译，Board 与 Slide 就没有默认模板，输入层 BoardEventSource
	/// 也就不存在，表现为「怎么都画不出笔迹」。
	/// 这里用代码复刻 themes/board.xaml、themes/slide.xaml 的最小模板，仅在检测不到
	/// 隐式样式时应用，保证任何编译方式下白板输入层都可用。
	/// </summary>
	internal static class InkBoardStyleFallback
	{
		public static void Apply(Board board)
		{
			if (board == null)
			{
				return;
			}
			Style slideStyle = CreateSlideStyle();
			if (Application.Current != null)
			{
				Application.Current.Resources[typeof(Slide)] = slideStyle;
			}
			board.Resources[typeof(Slide)] = slideStyle;
			board.Style = CreateBoardStyle();
			board.Background = Brushes.Transparent;
			board.ClipToBounds = true;
			board.Focusable = true;
			Stylus.SetIsPressAndHoldEnabled(board, false);
			Stylus.SetIsFlicksEnabled(board, false);
			Stylus.SetIsTapFeedbackEnabled(board, false);
		}

		private static Style CreateBoardStyle()
		{
			FrameworkElementFactory source = new FrameworkElementFactory(typeof(BoardEventSource), "PART_DecoratorSource");
			source.SetBinding(Panel.BackgroundProperty, TemplatedParentBinding("Background"));
			source.SetValue(UIElement.SnapsToDevicePixelsProperty,
				new TemplateBindingExtension(UIElement.SnapsToDevicePixelsProperty));

			FrameworkElementFactory content = new FrameworkElementFactory(typeof(ContentPresenter), "PART_ContentHost");
			content.SetBinding(ContentPresenter.ContentProperty, TemplatedParentBinding("CurrentSlide"));

			FrameworkElementFactory viewbox = new FrameworkElementFactory(typeof(SlideViewbox), "PART_SlideViewBox");
			viewbox.AppendChild(content);

			FrameworkElementFactory grid = new FrameworkElementFactory(typeof(Grid));
			grid.SetValue(FrameworkElement.MarginProperty, new TemplateBindingExtension(Control.PaddingProperty));
			grid.AppendChild(viewbox);
			source.AppendChild(grid);
			source.AppendChild(new FrameworkElementFactory(typeof(Grid), "PART_UtilitiesGrid"));

			FrameworkElementFactory adorner = new FrameworkElementFactory(typeof(AdornerDecorator));
			adorner.AppendChild(source);

			ControlTemplate template = new ControlTemplate(typeof(Board)) { VisualTree = adorner };

			Style style = new Style(typeof(Board));
			style.Setters.Add(new Setter(Control.TemplateProperty, template));
			style.Setters.Add(new Setter(Control.BackgroundProperty, Brushes.Transparent));
			return style;
		}

		private static Style CreateSlideStyle()
		{
			FrameworkElementFactory grid = new FrameworkElementFactory(typeof(Grid));
			grid.SetValue(Panel.BackgroundProperty, new TemplateBindingExtension(Control.BackgroundProperty));
			grid.AppendChild(new FrameworkElementFactory(typeof(ContentPresenter), "PART_ContentHost"));

			FrameworkElementFactory utilities = new FrameworkElementFactory(typeof(ContentPresenter), "PART_UtilitiesHost");
			utilities.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
			utilities.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Stretch);
			utilities.SetValue(UIElement.IsManipulationEnabledProperty, false);
			FrameworkElementFactory canvas = new FrameworkElementFactory(typeof(Canvas));
			canvas.AppendChild(utilities);
			grid.AppendChild(canvas);

			FrameworkElementFactory adorner = new FrameworkElementFactory(typeof(AdornerDecorator));
			adorner.AppendChild(grid);

			FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
			border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Control.BackgroundProperty));
			border.AppendChild(adorner);

			ControlTemplate template = new ControlTemplate(typeof(Slide)) { VisualTree = border };

			Style style = new Style(typeof(Slide));
			style.Setters.Add(new Setter(Control.TemplateProperty, template));
			return style;
		}

		private static Binding TemplatedParentBinding(string path)
		{
			return new Binding(path)
			{
				RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent),
				Mode = BindingMode.OneWay
			};
		}
	}
}
