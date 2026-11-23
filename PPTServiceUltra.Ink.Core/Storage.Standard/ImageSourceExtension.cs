using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	public static class ImageSourceExtension
	{
		public static readonly DependencyProperty ImageFilePathProperty = DependencyProperty.RegisterAttached("ImageFilePath", typeof(string), typeof(ImageSource));

		public static void SetImageFilePath([NotNull] this ImageSource source, string value)
		{
			source.SetValue(ImageFilePathProperty, value);
		}

		public static string GetImageFilePath([NotNull] this ImageSource source)
		{
			return (string)source.GetValue(ImageFilePathProperty);
		}
	}
}
