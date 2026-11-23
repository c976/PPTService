using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	internal static class ImageFix
	{
		internal static bool TryFixImage(IOException ioException, System.Uri uri, ref System.Windows.Media.ImageBrush systemImageBrush)
		{
			try
			{
				COMException ex = ioException.InnerException as COMException;
				if (ex != null && ex.ErrorCode == -2003292302)
				{
					BitmapImage bitmapImage = new BitmapImage();
					bitmapImage.BeginInit();
					bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
					bitmapImage.UriSource = uri;
					bitmapImage.EndInit();
					string text = CreateNewFileName(uri.OriginalString);
					bitmapImage.SaveBitmapImageIntoFile(text);
					uri = new System.Uri(text, UriKind.RelativeOrAbsolute);
					systemImageBrush = new System.Windows.Media.ImageBrush(new BitmapImage(uri));
					return true;
				}
				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal static string CreateNewFileName(string filePath)
		{
			if (File.Exists(filePath))
			{
				return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filePath), Guid.NewGuid().ToString("D") + System.IO.Path.GetExtension(filePath));
			}
			return filePath;
		}

		internal static void SaveBitmapImageIntoFile([NotNull] this BitmapImage bitmapImage, string filePath)
		{
			BitmapEncoder bitmapEncoder = new PngBitmapEncoder();
			bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapImage));
			using (FileStream stream = new FileStream(filePath, FileMode.Create))
			{
				bitmapEncoder.Save(stream);
			}
		}
	}
}
