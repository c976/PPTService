using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PPTService.Ink.Core.ComponentModel;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	public abstract class Brush : ComplexType
	{
		public double Opacity { get; set; } = 1.0;

		public static implicit operator Brush(System.Windows.Media.Brush brush)
		{
			return Convert(brush);
		}

		public static implicit operator System.Windows.Media.Brush(Brush brush)
		{
			return ConvertBack(brush);
		}

		private static Brush Convert(System.Windows.Media.Brush brush)
		{
			if (brush == null)
			{
				return null;
			}
			SolidColorBrush solidColorBrush = brush as SolidColorBrush;
			if (solidColorBrush != null)
			{
				System.Windows.Media.Color color = solidColorBrush.Color;
				return new ColorBrush
				{
					Color = new Color
					{
						A = (byte)((double)(int)color.A * solidColorBrush.Opacity),
						R = color.R,
						G = color.G,
						B = color.B
					},
					Opacity = solidColorBrush.Opacity
				};
			}
			System.Windows.Media.LinearGradientBrush linearGradientBrush = brush as System.Windows.Media.LinearGradientBrush;
			if (linearGradientBrush != null)
			{
				return new LinearGradientBrush
				{
					StartPoint = linearGradientBrush.StartPoint,
					EndPoint = linearGradientBrush.EndPoint,
					GradientStops = linearGradientBrush.GradientStops.ToString(CultureInfo.InvariantCulture),
					Opacity = linearGradientBrush.Opacity
				};
			}
			System.Windows.Media.ImageBrush imageBrush = brush as System.Windows.Media.ImageBrush;
			BitmapImage bitmapImage = imageBrush?.ImageSource as BitmapImage;
			if (bitmapImage == null)
			{
				return null;
			}
			if (DebuggingProperties.IsDebug && bitmapImage.UriSource != null && bitmapImage.UriSource.AbsoluteUri.StartsWith("pack://application", StringComparison.InvariantCultureIgnoreCase))
			{
				throw new InvalidOperationException("不能将程序集资源作为存储资源保存，请使用 ImageBrush 等将资源转换为文件引用。" + bitmapImage.UriSource.AbsoluteUri);
			}
			Uri source = null;
			if (bitmapImage.UriSource != null)
			{
				string localPath = bitmapImage.UriSource.LocalPath;
				source = ((!File.Exists(localPath)) ? ((Uri)new AppUri(bitmapImage.UriSource.AbsoluteUri)) : ((Uri)new FileUri(localPath)));
			}
			else if (bitmapImage.StreamSource != null)
			{
				string imageFilePath = bitmapImage.GetImageFilePath();
				if (File.Exists(imageFilePath))
				{
					source = new FileUri(imageFilePath);
				}
				else
				{
					string text = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString() + ".png");
					using (FileStream stream = new FileStream(text, FileMode.Create))
					{
						PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
						pngBitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapImage));
						pngBitmapEncoder.Save(stream);
					}
					source = new FileUri(text);
				}
			}
			MatrixTransform matrixTransform = imageBrush.RelativeTransform as MatrixTransform;
			Matrix relativeMatrixTransform = null;
			if (matrixTransform != null)
			{
				relativeMatrixTransform = matrixTransform.Value;
			}
			return new ImageBrush
			{
				Source = source,
				Stretch = imageBrush.Stretch,
				TileMode = imageBrush.TileMode,
				ViewboxUnits = imageBrush.ViewboxUnits,
				ViewportUnits = imageBrush.ViewportUnits,
				Viewbox = imageBrush.Viewbox,
				Viewport = imageBrush.Viewport,
				Opacity = imageBrush.Opacity,
				RelativeMatrixTransform = relativeMatrixTransform
			};
		}

		private static System.Windows.Media.Brush ConvertBack(Brush brush)
		{
			ColorBrush colorBrush = brush as ColorBrush;
			if (colorBrush != null)
			{
				Color color = colorBrush.Color;
				byte a = 0;
				if (colorBrush.Opacity != 0.0)
				{
					a = (byte)((double)(int)color.A / colorBrush.Opacity);
				}
				return new SolidColorBrush(new System.Windows.Media.Color
				{
					A = a,
					R = color.R,
					G = color.G,
					B = color.B
				})
				{
					Opacity = colorBrush.Opacity
				};
			}
			LinearGradientBrush linearGradientBrush = brush as LinearGradientBrush;
			if (linearGradientBrush != null)
			{
				return new System.Windows.Media.LinearGradientBrush
				{
					StartPoint = linearGradientBrush.StartPoint,
					EndPoint = linearGradientBrush.EndPoint,
					GradientStops = GradientStopCollection.Parse(linearGradientBrush.GradientStops),
					Opacity = linearGradientBrush.Opacity
				};
			}
			ImageBrush imageBrush = brush as ImageBrush;
			if (imageBrush == null)
			{
				return null;
			}
			IdUri idUri = imageBrush.Source as IdUri;
			if (idUri != null)
			{
				string absolutePath;
				if (!imageBrush.ReferenceManager.TryGet(idUri.Value, out absolutePath))
				{
					throw new ReferenceMissingException(idUri.Value)
					{
						ExtendMessage = $"Reference Count:{imageBrush.ReferenceManager.References.Count}"
					};
				}
				imageBrush.Source = new FileUri(absolutePath);
			}
			System.Uri uri = new System.Uri(imageBrush.Source.Value, UriKind.RelativeOrAbsolute);
			System.Windows.Media.ImageBrush systemImageBrush = null;
			try
			{
				if (uri.IsFile && !File.Exists(uri.OriginalString))
				{
					return null;
				}
				systemImageBrush = new System.Windows.Media.ImageBrush(new BitmapImage(uri));
			}
			catch (IOException ioException)
			{
				if (!ImageFix.TryFixImage(ioException, uri, ref systemImageBrush))
				{
					return null;
				}
			}
			catch (Exception)
			{
				return null;
			}
			try
			{
				systemImageBrush.Stretch = imageBrush.Stretch;
				systemImageBrush.TileMode = imageBrush.TileMode;
				systemImageBrush.Opacity = imageBrush.Opacity;
				if (imageBrush.ViewboxUnits.HasValue)
				{
					systemImageBrush.ViewboxUnits = imageBrush.ViewboxUnits.Value;
				}
				if (imageBrush.ViewportUnits.HasValue)
				{
					systemImageBrush.ViewportUnits = imageBrush.ViewportUnits.Value;
				}
				if (imageBrush.Viewbox != null)
				{
					systemImageBrush.Viewbox = imageBrush.Viewbox;
				}
				if (imageBrush.Viewport != null)
				{
					systemImageBrush.Viewport = imageBrush.Viewport;
				}
				if (imageBrush.RelativeMatrixTransform != null)
				{
					systemImageBrush.RelativeTransform = new MatrixTransform(imageBrush.RelativeMatrixTransform);
				}
				return systemImageBrush;
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
