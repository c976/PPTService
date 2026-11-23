using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Ink.Core.Windows.Media
{
	public static class CursorExtensions
	{
		private static readonly string[] ImageTypeInts = new string[4] { "6677", "255216", "7173", "13780" };

		public static bool IsImage(string curPath)
		{
			FileStream fileStream = new FileStream(curPath, FileMode.Open, FileAccess.Read);
			BinaryReader binaryReader = new BinaryReader(fileStream);
			string text = binaryReader.ReadByte().ToString();
			text += binaryReader.ReadByte();
			binaryReader.Close();
			fileStream.Close();
			if (ImageTypeInts.Contains(text))
			{
				return true;
			}
			return false;
		}

		[CanBeNull]
		public static string CurToPng(string curPath, string pngPath)
		{
			Bitmap bitmap = null;
			try
			{
				byte[] array = File.ReadAllBytes(curPath);
				CursorHead cursorHead = new CursorHead(array);
				cursorHead.ImageWidth = (byte)BitConverter.ToInt16(array, (int)(cursorHead.ImageRva + 4));
				cursorHead.ImageHeight = (byte)(BitConverter.ToInt16(array, (int)(cursorHead.ImageRva + 8)) / 2);
				short piex = BitConverter.ToInt16(array, (int)(cursorHead.ImageRva + 14));
				bitmap = LoadImgae(cursorHead.ImageWidth, cursorHead.ImageHeight, piex, array, (int)(cursorHead.ImageRva + 40));
				bitmap.Save(pngPath, ImageFormat.Png);
				byte[] array2 = new byte[4];
				array2[3] = array[37];
				array2[2] = array[36];
				array2[1] = array[35];
				array2[0] = array[34];
				cursorHead.ImageRva = BitConverter.ToUInt32(array2, 0);
				return pngPath;
			}
			catch (Exception)
			{
				return null;
			}
			finally
			{
				bitmap?.Dispose();
				bitmap = null;
			}
		}

		private static Bitmap LoadImgae(byte cursorWidth, byte cursorheight, short piex, byte[] fileBytes, int starIndex)
		{
			Bitmap bitmap = new Bitmap(cursorWidth, cursorheight, PixelFormat.Format32bppArgb);
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, cursorWidth, cursorheight), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
			byte[] array = new byte[bitmapData.Stride * bitmapData.Height];
			switch (piex)
			{
			case 1:
				Load1(fileBytes, array, starIndex, cursorWidth, cursorheight, bitmapData);
				break;
			case 4:
				Load4(fileBytes, array, starIndex, cursorWidth, cursorheight, bitmapData);
				break;
			case 8:
				Load8(fileBytes, array, starIndex, cursorWidth, cursorheight, bitmapData);
				break;
			case 24:
				Load24(fileBytes, array, starIndex, cursorWidth, cursorheight, bitmapData);
				break;
			case 32:
				Load32(fileBytes, array, starIndex, bitmapData);
				break;
			default:
				throw new Exception("Unsupported file format");
			}
			Marshal.Copy(array, 0, bitmapData.Scan0, array.Length);
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}

		private static void Load32(byte[] pFileBytes, byte[] pNewData, int pReadIndex, BitmapData pNewBitmapData)
		{
			for (int num = pNewBitmapData.Height - 1; num != -1; num--)
			{
				int num2 = num * pNewBitmapData.Stride;
				for (int i = 0; i != pNewBitmapData.Width; i++)
				{
					pNewData[num2 + i * 4] = pFileBytes[pReadIndex];
					pNewData[num2 + i * 4 + 1] = pFileBytes[pReadIndex + 1];
					pNewData[num2 + i * 4 + 2] = pFileBytes[pReadIndex + 2];
					pNewData[num2 + i * 4 + 3] = pFileBytes[pReadIndex + 3];
					pReadIndex += 4;
				}
			}
		}

		private static void Load24(byte[] pFileBytes, byte[] pNewData, int pReadIndex, int pWidth, int pHeight, BitmapData pNewBitmapData)
		{
			int sourceIndex = pWidth * pHeight * 3 + pReadIndex;
			int num = pWidth / 8;
			if (pWidth % 8 != 0)
			{
				num++;
			}
			if (num % 4 != 0)
			{
				num += num % 4;
			}
			byte[] array = new byte[num * pHeight];
			Array.Copy(pFileBytes, sourceIndex, array, 0, array.Length);
			for (int i = 0; i != array.Length; i++)
			{
				array[i] = ReverseByte(array[i]);
			}
			BitArray bitArray = new BitArray(array);
			for (int num2 = pNewBitmapData.Height - 1; num2 != -1; num2--)
			{
				sourceIndex = (pHeight - 1 - num2) * (num * 8);
				int num3 = num2 * pNewBitmapData.Stride;
				for (int j = 0; j != pNewBitmapData.Width; j++)
				{
					pNewData[num3 + j * 4] = pFileBytes[pReadIndex];
					pNewData[num3 + j * 4 + 1] = pFileBytes[pReadIndex + 1];
					pNewData[num3 + j * 4 + 2] = pFileBytes[pReadIndex + 2];
					pNewData[num3 + j * 4 + 3] = byte.MaxValue;
					if (bitArray[sourceIndex])
					{
						pNewData[num3 + j * 4 + 3] = 0;
					}
					pReadIndex += 3;
					sourceIndex++;
				}
			}
		}

		private static void Load8(byte[] pFileBytes, byte[] pNewData, int pReadIndex, int pWidth, int pHeight, BitmapData pNewBitmapData)
		{
			Hashtable hashtable = new Hashtable();
			for (int i = 0; i != 256; i++)
			{
				hashtable.Add(i.ToString(CultureInfo.InvariantCulture), Color.FromArgb(pFileBytes[pReadIndex + 3], pFileBytes[pReadIndex + 2], pFileBytes[pReadIndex + 1], pFileBytes[pReadIndex]));
				pReadIndex += 4;
			}
			int sourceIndex = pWidth * pHeight + pReadIndex;
			int num = pWidth / 8;
			if (pWidth % 8 != 0)
			{
				num++;
			}
			if (num % 4 != 0)
			{
				num += num % 4;
			}
			byte[] array = new byte[num * pHeight];
			Array.Copy(pFileBytes, sourceIndex, array, 0, array.Length);
			for (int j = 0; j != array.Length; j++)
			{
				array[j] = ReverseByte(array[j]);
			}
			BitArray bitArray = new BitArray(array);
			for (int num2 = pNewBitmapData.Height - 1; num2 != -1; num2--)
			{
				sourceIndex = (pHeight - 1 - num2) * (num * 8);
				int num3 = num2 * pNewBitmapData.Stride;
				for (int k = 0; k != pNewBitmapData.Width; k++)
				{
					byte b = pFileBytes[pReadIndex];
					Color color = (Color)hashtable[b.ToString(CultureInfo.InvariantCulture)];
					pNewData[num3 + k * 4] = color.B;
					pNewData[num3 + k * 4 + 1] = color.G;
					pNewData[num3 + k * 4 + 2] = color.R;
					pNewData[num3 + k * 4 + 3] = byte.MaxValue;
					if (bitArray[sourceIndex])
					{
						pNewData[num3 + k * 4 + 3] = 0;
					}
					pReadIndex++;
					sourceIndex++;
				}
			}
		}

		private static void Load4(byte[] pFileBytes, byte[] pNewData, int pReadIndex, int pWidth, int pHeight, BitmapData pNewBitmapData)
		{
			Hashtable hashtable = new Hashtable();
			for (int i = 0; i != 16; i++)
			{
				hashtable.Add(i.ToString(CultureInfo.InvariantCulture), Color.FromArgb(pFileBytes[pReadIndex + 3], pFileBytes[pReadIndex + 2], pFileBytes[pReadIndex + 1], pFileBytes[pReadIndex]));
				pReadIndex += 4;
			}
			int sourceIndex = pWidth * pHeight / 2 + pReadIndex;
			int num = pWidth / 8;
			if (pWidth % 8 != 0)
			{
				num++;
			}
			if (num % 4 != 0)
			{
				num += num % 4;
			}
			byte[] array = new byte[num * pHeight];
			Array.Copy(pFileBytes, sourceIndex, array, 0, array.Length);
			for (int j = 0; j != array.Length; j++)
			{
				array[j] = ReverseByte(array[j]);
			}
			BitArray bitArray = new BitArray(array);
			bool flag = true;
			for (int num2 = pNewBitmapData.Height - 1; num2 != -1; num2--)
			{
				sourceIndex = (pHeight - 1 - num2) * (num * 8);
				int num3 = num2 * pNewBitmapData.Stride;
				for (int k = 0; k != pNewBitmapData.Width; k++)
				{
					byte b = pFileBytes[pReadIndex];
					if (flag)
					{
						b = (byte)((b & 0xF0) >> 4);
						flag = false;
					}
					else
					{
						b &= 0xF;
						flag = true;
						pReadIndex++;
					}
					Color color = (Color)hashtable[b.ToString(CultureInfo.InvariantCulture)];
					pNewData[num3 + k * 4] = color.B;
					pNewData[num3 + k * 4 + 1] = color.G;
					pNewData[num3 + k * 4 + 2] = color.R;
					pNewData[num3 + k * 4 + 3] = byte.MaxValue;
					if (bitArray[sourceIndex])
					{
						pNewData[num3 + k * 4 + 3] = 0;
					}
					sourceIndex++;
				}
			}
		}

		private static void Load1(byte[] pFileBytes, byte[] pNewData, int pReadIndex, int pWidth, int pHeight, BitmapData pNewBitmapData)
		{
			Hashtable hashtable = new Hashtable();
			for (int i = 0; i != 2; i++)
			{
				hashtable.Add(i.ToString(CultureInfo.InvariantCulture), Color.FromArgb(pFileBytes[pReadIndex + 3], pFileBytes[pReadIndex + 2], pFileBytes[pReadIndex + 1], pFileBytes[pReadIndex]));
				pReadIndex += 4;
			}
			int sourceIndex = pWidth * pHeight / 8 + pReadIndex;
			int num = pWidth / 8;
			if (pWidth % 8 != 0)
			{
				num++;
			}
			if (num % 4 != 0)
			{
				num += num % 4;
			}
			byte[] array = new byte[num * pHeight];
			Array.Copy(pFileBytes, sourceIndex, array, 0, array.Length);
			for (int j = 0; j != array.Length; j++)
			{
				array[j] = ReverseByte(array[j]);
			}
			BitArray bitArray = new BitArray(array);
			int num2 = 7;
			for (int num3 = pNewBitmapData.Height - 1; num3 != -1; num3--)
			{
				sourceIndex = (pHeight - 1 - num3) * (num * 8);
				int num4 = num3 * pNewBitmapData.Stride;
				for (int k = 0; k != pNewBitmapData.Width; k++)
				{
					byte b = pFileBytes[pReadIndex];
					b = (byte)(new BitArray(new byte[1] { b })[num2] ? 1u : 0u);
					if (num2 == 0)
					{
						pReadIndex++;
						num2 = 7;
					}
					else
					{
						num2--;
					}
					Color color = (Color)hashtable[b.ToString(CultureInfo.InvariantCulture)];
					pNewData[num4 + k * 4] = color.B;
					pNewData[num4 + k * 4 + 1] = color.G;
					pNewData[num4 + k * 4 + 2] = color.R;
					pNewData[num4 + k * 4 + 3] = byte.MaxValue;
					if (bitArray[sourceIndex])
					{
						pNewData[num4 + k * 4 + 3] = 0;
					}
					sourceIndex++;
				}
			}
		}

		public static byte ReverseByte(byte bytetoReverse)
		{
			return (byte)((byte)((byte)((byte)((byte)((byte)((byte)((byte)(0 | (byte)((bytetoReverse & 1) << 7)) | (byte)(((bytetoReverse >> 1) & 1) << 6)) | (byte)(((bytetoReverse >> 2) & 1) << 5)) | (byte)(((bytetoReverse >> 3) & 1) << 4)) | (byte)(((bytetoReverse >> 4) & 1) << 3)) | (byte)(((bytetoReverse >> 5) & 1) << 2)) | (byte)(((bytetoReverse >> 6) & 1) << 1)) | (byte)((bytetoReverse >> 7) & 1));
		}
	}
}
