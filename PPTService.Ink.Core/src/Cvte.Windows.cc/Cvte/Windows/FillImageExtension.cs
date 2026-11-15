using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using PPTService.Ink.Core.Windows.Mathematics;

namespace PPTService.Ink.Core.Windows
{
	[MarkupExtensionReturnType(typeof(Brush))]
	public class FillImageExtension : MarkupExtension
	{
		private class GridImage : FrameworkElement
		{
			private readonly ImageSource _source;

			private readonly double _imageWidth;

			private readonly double _imageHeight;

			private readonly Thickness _edge;

			internal Brush Background { get; set; }

			public GridImage(ImageSource source, Thickness edge)
			{
				if (source == null)
				{
					throw new ArgumentNullException("source");
				}
				if (edge.Left < 0.0 || edge.Top < 0.0 || edge.Right < 0.0 || edge.Bottom < 0.0)
				{
					throw new ArgumentException("图像上的边缘必须大于或等于 0。", "Thickness");
				}
				_source = source;
				_imageWidth = source.Width;
				_imageHeight = source.Height;
				_edge = edge;
			}

			protected override void OnRender(DrawingContext dc)
			{
				if (ActualWidth < _edge.Left + _edge.Right || ActualHeight < _edge.Top + _edge.Bottom)
				{
					return;
				}
				dc.DrawRectangle(Brushes.Transparent, null, new Rect(0.0, 0.0, ActualWidth, ActualHeight));
				dc.DrawRectangle(Background, null, new Rect(0.0, 0.0, ActualWidth, ActualHeight));
				double[] lengths = GetLengths(_imageWidth, _edge.Left, _edge.Right);
				double[] lengths2 = GetLengths(_imageHeight, _edge.Top, _edge.Bottom);
				double[] lengths3 = GetLengths(ActualWidth, _edge.Left, _edge.Right);
				double[] lengths4 = GetLengths(ActualHeight, _edge.Top, _edge.Bottom);
				dc.PushGuidelineSet(new GuidelineSet(lengths3.Take(3).ToArray(), lengths4.Take(3).ToArray()));
				for (int i = 0; i < 3; i++)
				{
					for (int j = 0; j < 3; j++)
					{
						if (!Nearly.Equals(lengths[j + 3], 0.0) && !Nearly.Equals(lengths2[i + 3], 0.0))
						{
							Brush brush = new ImageBrush(_source)
							{
								ViewboxUnits = BrushMappingMode.Absolute,
								Viewbox = new Rect(lengths[j], lengths2[i], lengths[j + 3], lengths2[i + 3])
							};
							brush.Freeze();
							dc.DrawRectangle(brush, null, new Rect(lengths3[j] + Margin.Left, lengths4[i] + Margin.Top, lengths3[j + 3], lengths4[i + 3]));
						}
					}
				}
				dc.Pop();
			}

			private static double[] GetLengths(double total, double head, double tail)
			{
				return new double[6]
				{
					0.0,
					head,
					total - tail,
					head,
					total - head - tail,
					tail
				};
			}
		}

		[ConstructorArgument("source")]
		[DefaultValue(null)]
		public ImageSource Source { get; set; }

		public Thickness Edge { get; set; }

		public Brush Background { get; set; }

		public FillImageExtension()
		{
		}

		public FillImageExtension(ImageSource source)
		{
			Source = source;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			IProvideValueTarget provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			if (provideValueTarget == null)
			{
				return null;
			}
			if (provideValueTarget.TargetObject.ToString().EndsWith("SharedDp"))
			{
				return this;
			}
			FrameworkElement frameworkElement = provideValueTarget.TargetObject as FrameworkElement;
			if (frameworkElement == null)
			{
				return this;
			}
			return CreateBrush(frameworkElement);
		}

		private Brush CreateBrush(FrameworkElement target)
		{
			return CreateAttachedBrush(target, Source, Edge, Background);
		}

		public static Brush CreateAttachedBrush(FrameworkElement target, ImageSource source, Thickness edge, Brush background)
		{
			GridImage gridImage = new GridImage(source, edge)
			{
				Background = background
			};
			gridImage.SetBinding(FrameworkElement.WidthProperty, new Binding
			{
				Source = target,
				Path = new PropertyPath("ActualWidth")
			});
			gridImage.SetBinding(FrameworkElement.HeightProperty, new Binding
			{
				Source = target,
				Path = new PropertyPath("ActualHeight")
			});
			return new VisualBrush(gridImage);
		}
	}
}
