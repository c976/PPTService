using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Storage;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Extensions.SupportedExtChars
{
	public class ImageChar : ExtensionalChar
	{
		private ImageCharSaveInfo _saveInfo;

		private Image _image;

		private PropertyChangedArgs _propertyChangedArgs;

		public override Point LogicalCenter { get; set; }

		public override Size Size { get; set; }

		public override UIElement Render(RunProperty runProperty = null)
		{
			EnsureImage();
			return _image;
		}

		public override void StyleAdapter(PropertyChangedArgs propertyChangedArgs)
		{
			if (IsFontSizeChanged(propertyChangedArgs))
			{
				Size textCharSize = TextEditorExtManager.GetTextCharSize(propertyChangedArgs.NewValue);
				double imageWidthWithExtractHeight = TextHelperInner.GetImageWidthWithExtractHeight(_saveInfo.Source.Value, textCharSize.Height);
				Size = new Size(imageWidthWithExtractHeight, textCharSize.Height);
				LogicalCenter = new Point(0.0, Size.Height / 2.0);
				UpdateImage();
			}
			_propertyChangedArgs = propertyChangedArgs;
		}

		private bool IsFontSizeChanged(PropertyChangedArgs propertyChangedArgs)
		{
			if (propertyChangedArgs.Property == PropertyType.FontSize)
			{
				return true;
			}
			if (propertyChangedArgs.Property == PropertyType.RunProperty)
			{
				return true;
			}
			if (_propertyChangedArgs == null)
			{
				return true;
			}
			if (!Converter.AreEquals(_propertyChangedArgs.NewValue.FontSize, propertyChangedArgs.NewValue.FontSize))
			{
				return true;
			}
			return false;
		}

		public override ExtCharSaveInfo GetSaveInfo()
		{
			return new ImageCharSaveInfo
			{
				Source = _saveInfo.Source,
				LogicalCenter = LogicalCenter,
				Size = Size
			};
		}

		public override void SetSaveInfo(ExtCharSaveInfo extCharSaveInfo)
		{
			_saveInfo = (ImageCharSaveInfo)extCharSaveInfo;
			LogicalCenter = _saveInfo.LogicalCenter;
			Size = _saveInfo.Size;
			UpdateImage();
		}

		public override ExtensionalChar DeepClone()
		{
			ImageChar imageChar = new ImageChar();
			imageChar.SetSaveInfo(GetSaveInfo());
			return imageChar;
		}

		private void EnsureImage()
		{
			if (_image == null && _saveInfo.Source != null)
			{
				UpdateImage();
			}
		}

		private void UpdateImage()
		{
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.UriSource = _saveInfo.Source;
			bitmapImage.EndInit();
			Image image = new Image
			{
				Source = bitmapImage,
				Width = Size.Width,
				Height = Size.Height,
				Stretch = Stretch.Uniform
			};
			_image = image;
		}
	}
}
