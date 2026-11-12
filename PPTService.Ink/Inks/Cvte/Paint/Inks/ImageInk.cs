using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework;
using Cvte.Paint.Inks.Storage;
using Cvte.Storage;

namespace Cvte.Paint.Inks
{
	[Transform(PermittedTransforms.RenderTransform | PermittedTransforms.Translate)]
	public sealed class ImageInk : Element, ISavable<ImageInkSaveInfo>, ISavable, IInk, IPermittedTransform
	{
		private class ImageInkRenderer : FrameworkElement
		{
			private readonly ImageStroke _imageStroke;

			public ImageInkRenderer([NotNull] ImageStroke imageStroke)
			{
				_imageStroke = imageStroke;
			}

			protected override void OnRender(DrawingContext dc)
			{
				_imageStroke.Draw(dc);
			}

			protected override Size MeasureOverride(Size availableSize)
			{
				return _imageStroke.GetBounds().Size;
			}
		}

		private ImageStroke _innerStroke;

		private ImageStroke InnerStroke
		{
			get
			{
				return _innerStroke;
			}
			set
			{
				if (_innerStroke != value)
				{
					_innerStroke = value;
					Children.Clear();
					Children.Add(new ImageInkRenderer(value));
				}
			}
		}

		PermittedTransforms IPermittedTransform.PermittedTransform => PermittedTransforms.RenderTransform | PermittedTransforms.Translate;

		[UsedImplicitly]
		private ImageInk()
		{
			IsErasable = true;
		}

		public ImageInk(ImageStroke imageStroke)
		{
			InnerStroke = imageStroke;
			IsErasable = true;
			PathGeometry pathGeometry = new PathGeometry();
			pathGeometry.AddGeometry(new RectangleGeometry(Bounds));
			ClipGeometry = pathGeometry;
		}

		public ImageInkSaveInfo GetSaveInfo()
		{
			return new ImageInkSaveInfo
			{
				Points = InnerStroke.StylusPoints,
				Scale = InnerStroke.BitmapScale,
				Source = InnerStroke.InkImage.UriSource,
				ClipGeometry = ClipGeometry?.ToString()
			};
		}

		public void SetSaveInfo(ImageInkSaveInfo saveInfo)
		{
			InnerStroke = new ImageStroke(saveInfo.Points, new BitmapImage(saveInfo.Source), saveInfo.Scale);
			PathGeometry pathGeometry = new PathGeometry();
			pathGeometry.AddGeometry(Geometry.Parse(saveInfo.ClipGeometry));
			ClipGeometry = pathGeometry;
		}
	}
}
