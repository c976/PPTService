using System;

namespace PPTService.Ink.Core.Windows.Media
{
	internal class CursorHead
	{
		private byte[] _mImageCount = new byte[2];

		private byte[] _mImageLength = new byte[4];

		private byte[] _mImageRva = new byte[4];

		public byte ImageHeight { get; set; }

		public ushort ImageCount
		{
			get
			{
				return BitConverter.ToUInt16(_mImageCount, 0);
			}
			set
			{
				_mImageCount = BitConverter.GetBytes(value);
			}
		}

		public byte ImageWidth { get; set; }

		public byte ColorCount { get; set; }

		public uint ImageLength
		{
			get
			{
				return BitConverter.ToUInt32(_mImageLength, 0);
			}
			set
			{
				_mImageLength = BitConverter.GetBytes(value);
			}
		}

		public uint ImageRva
		{
			get
			{
				return BitConverter.ToUInt32(_mImageRva, 0);
			}
			set
			{
				_mImageRva = BitConverter.GetBytes(value);
			}
		}

		public CursorHead(byte[] cursorData)
		{
			ImageCount = BitConverter.ToUInt16(cursorData, 4);
			ImageHeight = cursorData[7];
			ImageWidth = cursorData[6];
			ColorCount = cursorData[8];
			ImageLength = BitConverter.ToUInt32(cursorData, 14);
			ImageRva = BitConverter.ToUInt32(cursorData, 18);
		}
	}
}
