using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PPTService
{
	public class SlideItem : UserControl
	{
		public static readonly DependencyProperty IsPptThumbnailProperty = DependencyProperty.Register("IsPptThumbnail", typeof(string), typeof(SlideItem), new PropertyMetadata((object)null));

		public static readonly DependencyProperty IndexProperty = DependencyProperty.Register("Index", typeof(int), typeof(SlideItem), new PropertyMetadata(0));

		public static readonly DependencyProperty BitmapImageProperty = DependencyProperty.Register("BitmapImage", typeof(BitmapImage), typeof(SlideItem), new PropertyMetadata((object)null));

		public string IsPptThumbnail
		{
			get
			{
				return (string)GetValue(IsPptThumbnailProperty);
			}
			set
			{
				SetValue(IsPptThumbnailProperty, value);
			}
		}

		public int Index
		{
			get
			{
				return (int)GetValue(IndexProperty);
			}
			set
			{
				SetValue(IndexProperty, value);
			}
		}

		public BitmapImage BitmapImage
		{
			get
			{
				return (BitmapImage)GetValue(BitmapImageProperty);
			}
			set
			{
				SetValue(BitmapImageProperty, value);
			}
		}
	}
}
