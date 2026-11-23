using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PPTServiceUltra.Ink.Core.Windows.Media
{
	public class FloodFiller
	{
		private byte _tolerance;

		private byte _preTolerance;

		private readonly double _dpiX;

		private readonly double _dpiY;

		private readonly int _width;

		private readonly int _height;

		private readonly int _stride;

		private byte[] _tempPixels;

		private byte[] _startColor;

		private byte[] _fillColor;

		private bool[] _pixelsChecked;

		private readonly byte _bitmapPixelFormatSize;

		private BitmapSource _temBitmapSource;

		private readonly FloodFillRangeQueue _ranges = new FloodFillRangeQueue();

		private Rect _floodFillRegion;

		public Rect FloodFillRegion
		{
			get
			{
				return _floodFillRegion;
			}
			set
			{
				double num = ((value.Left < 0.0) ? 0.0 : value.Left);
				double num2 = ((value.Top < 0.0) ? 0.0 : value.Top);
				double num3 = ((value.Right > (double)_width) ? ((double)_width) : value.Right);
				double num4 = ((value.Bottom > (double)_height) ? ((double)_height) : value.Bottom);
				_floodFillRegion = new Rect((int)num, (int)num2, (int)num3 - (int)num, (int)num4 - (int)num2);
			}
		}

		public byte[] OriginalPixels { get; set; }

		public byte[] ResultPixels { get; set; }

		public FloodFiller(BitmapSource bitmapSource)
		{
			int bitsPerPixel = bitmapSource.Format.BitsPerPixel;
			_dpiX = bitmapSource.DpiX;
			_dpiY = bitmapSource.DpiY;
			_width = bitmapSource.PixelWidth;
			_height = bitmapSource.PixelHeight;
			_stride = (_width * bitsPerPixel + 7) / 8;
			_bitmapPixelFormatSize = (byte)(bitsPerPixel / 8);
			_pixelsChecked = new bool[_width * _height];
			_tempPixels = new byte[_stride * _height];
			ResultPixels = new byte[_stride * _height];
			OriginalPixels = new byte[_stride * _height];
			bitmapSource.CopyPixels(Int32Rect.Empty, OriginalPixels, _stride, 0);
			bitmapSource.CopyPixels(Int32Rect.Empty, ResultPixels, _stride, 0);
			FloodFillRegion = new Rect(0.0, 0.0, _width, _height);
		}

		public BitmapSource FloodFill(System.Drawing.Point pt, byte tolerance)
		{
			int x = pt.X;
			int y = pt.Y;
			_tolerance = tolerance;
			if (!IsNeedFloodFill())
			{
				return _temBitmapSource;
			}
			_tempPixels = new byte[_stride * _height];
			_pixelsChecked = new bool[_width * _height];
			Array.Copy(OriginalPixels, ResultPixels, OriginalPixels.Length);
			InitColor(PointToByteIndex(ref x, ref y));
			LinearFill(ref x, ref y);
			while (_ranges.Count > 0)
			{
				FloodFillRange floodFillRange = _ranges.Dequeue();
				int y2 = floodFillRange.Y - 1;
				int y3 = floodFillRange.Y + 1;
				int index = _width * y2 + floodFillRange.StartX;
				int index2 = _width * y3 + floodFillRange.StartX;
				for (int i = floodFillRange.StartX; i <= floodFillRange.EndX; i++)
				{
					int px = PointToByteIndex(ref i, ref y2);
					index = MakeIndexInsideOfRange(index, _pixelsChecked.Length);
					if ((double)floodFillRange.Y > FloodFillRegion.Top && !_pixelsChecked[index] && CheckPixel(ref px))
					{
						LinearFill(ref i, ref y2);
					}
					index2 = MakeIndexInsideOfRange(index2, _pixelsChecked.Length);
					px = PointToByteIndex(ref i, ref y3);
					if ((double)floodFillRange.Y < FloodFillRegion.Bottom - 1.0 && !_pixelsChecked[index2] && CheckPixel(ref px))
					{
						LinearFill(ref i, ref y3);
					}
					index++;
					index2++;
				}
			}
			_preTolerance = tolerance;
			_temBitmapSource = BitmapSource.Create(_width, _height, _dpiX, _dpiY, PixelFormats.Bgra32, null, _tempPixels, _stride);
			return _temBitmapSource;
		}

		public BitmapSource FloodFillResult()
		{
			ExpandCorrode();
			return BitmapSource.Create(_width, _height, _dpiX, _dpiY, PixelFormats.Bgra32, null, ResultPixels, _stride);
		}

		private void LinearFill(ref int x, ref int y)
		{
			int num = x;
			int px = PointToByteIndex(ref x, ref y);
			int num2 = _width * y + x;
			do
			{
				_tempPixels[px] = _fillColor[0];
				_tempPixels[px + 1] = _fillColor[1];
				_tempPixels[px + 2] = _fillColor[2];
				_tempPixels[px + 3] = byte.MaxValue;
				ResultPixels[px + 3] = 0;
				_pixelsChecked[num2] = true;
				num--;
				num2--;
				px -= _bitmapPixelFormatSize;
			}
			while (!((double)num < FloodFillRegion.Left) && !_pixelsChecked[num2] && CheckPixel(ref px));
			if (num != 0)
			{
				num++;
			}
			int num3 = x;
			px = PointToByteIndex(ref x, ref y);
			num2 = _width * y + x;
			do
			{
				_tempPixels[px] = _fillColor[0];
				_tempPixels[px + 1] = _fillColor[1];
				_tempPixels[px + 2] = _fillColor[2];
				_tempPixels[px + 3] = byte.MaxValue;
				ResultPixels[px + 3] = 0;
				_pixelsChecked[num2] = true;
				num3++;
				num2++;
				px += _bitmapPixelFormatSize;
			}
			while (!((double)num3 >= FloodFillRegion.Right) && !_pixelsChecked[num2] && CheckPixel(ref px));
			num3--;
			FloodFillRange r = new FloodFillRange(num, num3, y);
			_ranges.Enqueue(ref r);
		}

		protected bool CheckPixel(ref int px)
		{
			if (OriginalPixels[px] >= _startColor[0] - _tolerance && OriginalPixels[px] <= _startColor[0] + _tolerance && OriginalPixels[px + 1] >= _startColor[1] - _tolerance && OriginalPixels[px + 1] <= _startColor[1] + _tolerance && OriginalPixels[px + 2] >= _startColor[2] - _tolerance)
			{
				return OriginalPixels[px + 2] <= _startColor[2] + _tolerance;
			}
			return false;
		}

		protected int PointToByteIndex(ref int x, ref int y)
		{
			if (x < 0)
			{
				x = 0;
			}
			if (x > _width)
			{
				x = _width;
			}
			if (y < 0)
			{
				y = 0;
			}
			if (y > _height)
			{
				y = _height;
			}
			return _stride * y + x * _bitmapPixelFormatSize;
		}

		private bool IsNeedFloodFill()
		{
			if (Math.Abs(_preTolerance - _tolerance) < 1)
			{
				return _tolerance == 1;
			}
			return true;
		}

		private void InitColor(int idx)
		{
			byte b = OriginalPixels[idx];
			byte b2 = OriginalPixels[idx + 1];
			byte b3 = OriginalPixels[idx + 2];
			_startColor = new byte[3] { b, b2, b3 };
			byte b4 = (byte)(255 - b);
			byte b5 = (byte)(255 - b2);
			byte b6 = (byte)(255 - b3);
			_fillColor = new byte[3] { b4, b5, b6 };
		}

		public void ExpandCorrode()
		{
			Array.Copy(ResultPixels, _tempPixels, ResultPixels.Length);
			int fromInclusive = (int)((FloodFillRegion.Left < 1.0) ? 1.0 : FloodFillRegion.Left);
			int startHeight = (int)((FloodFillRegion.Top < 1.0) ? 1.0 : FloodFillRegion.Top);
			int toExclusive = (int)(FloodFillRegion.Right - 1.0);
			int height = (int)(FloodFillRegion.Bottom - 1.0);
			Parallel.For(fromInclusive, toExclusive, (int i) =>
			{
				Parallel.For(startHeight, height, (int j) =>
				{
					int x = i;
					int y = j;
					int num = PointToByteIndex(ref x, ref y);
					if (_tempPixels[num + 3] == byte.MaxValue && GetRoundPixel(ref _tempPixels, i, j).Count((bool t) => t) > 2)
					{
						ResultPixels[PointToByteIndex(ref x, ref y) + 3] = 0;
					}
				});
			});
		}

		public bool[] GetRoundPixel(ref byte[] pixels, int x, int y)
		{
			bool[] array = new bool[8];
			int num = 0;
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					if (i != 0 || j != 0)
					{
						int x2 = x + i;
						int y2 = y + j;
						int num2 = PointToByteIndex(ref x2, ref y2);
						if (pixels[num2 + 3] == 0 && _pixelsChecked[_width * y2 + x2])
						{
							array[num] = true;
							num++;
						}
						else
						{
							array[num] = false;
							num++;
						}
					}
				}
			}
			return array;
		}

		private int MakeIndexInsideOfRange(int index, int arrayLength)
		{
			if (index < 0)
			{
				return 0;
			}
			if (index >= arrayLength)
			{
				return arrayLength - 1;
			}
			return index;
		}
	}
}
