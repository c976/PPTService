using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PPTService
{
	public static class ReleaseMemoryImageHelper
	{
		private static string ImageName = "RleaseMemoryImage.png";

		public static string GetImageFilePath()
		{
			string text = Path.Combine(AppPath.DocumentPath(), ImageName);
			if (File.Exists(text))
			{
				return text;
			}
			byte[] pixels = new byte[4];
			BitmapSource.Create(1, 1, 96.0, 96.0, PixelFormats.Bgra32, null, pixels, 4).SaveToFile(text);
			return text;
		}
	}
}
