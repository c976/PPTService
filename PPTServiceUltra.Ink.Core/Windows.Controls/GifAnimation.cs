using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PPTServiceUltra.Ink.Core.Windows.Controls
{
	public class GifAnimation
	{
		private enum GifStatus
		{
			Play,
			Stop,
			Replay
		}

		private int _delay;

		private int _stride;

		private int _dispose;

		private bool _hasError;

		private bool _interlace;

		private byte[] _gifData;

		private int _transIndex;

		private int _frameCount;

		private int _lastDispose;

		private bool _isStaticGif;

		private bool _transparency;

		private int _offset;

		private int _startOffset;

		private int _frameLeft;

		private int _frameTop;

		private int _frameWidth;

		private int _frameHeight;

		private int _lastFrameLeft;

		private int _lastFrameTop;

		private int _lastFrameWidth;

		private int _lastFrameHeight;

		private int _backgroundIndex;

		private int _backgroundColor;

		private int _lastBackgroundColor;

		private int[] _activeColorTable;

		private int[] _globalColorTable;

		private short[] _prefix;

		private int[] _dest;

		private int[] _lastDest;

		private byte[] _suffix;

		private byte[] _pixelStack;

		private byte[] _pixels;

		private readonly byte[] _block = new byte[256];

		private const int MaxStackSize = 4096;

		private Stopwatch _stopWatch;

		private WriteableBitmap _source;

		private const byte GifDescriptor = 44;

		private const byte Terminator = 0;

		private const byte ExtensionIntroducer = 33;

		private const byte GifTrailer = 59;

		private GifStatus _control;

		private BackgroundTask _backgroundTask;

		public int LogicalWidth { get; private set; }

		public int LogicalHeight { get; private set; }

		public bool AutoReplay { get; set; } = true;

		public WriteableBitmap Source
		{
			get
			{
				return _source;
			}
			set
			{
				_source = value;
				SourceChanged?.Invoke(this, _source);
			}
		}

		public event EventHandler PlayOnceFinished;

		public event EventHandler<ImageSource> SourceChanged;

		public GifAnimation(string fileName)
		{
			InitializeData(fileName);
		}

		public void Play()
		{
			if (!_isStaticGif && !_hasError)
			{
				if (_003CPlay_003Eg__IsParsed_007C52_0())
				{
					_control = GifStatus.Replay;
					AddBackgroundTask();
				}
				else
				{
					ParseGif();
				}
			}
		}

		public void Stop()
		{
			_control = GifStatus.Stop;
		}

		public void Init(bool needPlay = false)
		{
			if (_startOffset <= 0)
			{
				_control = ((!needPlay) ? GifStatus.Stop : GifStatus.Play);
				ParseGif();
			}
		}

		private void InitializeData(string filePath)
		{
			_gifData = null;
			_suffix = (_pixelStack = (_pixels = null));
			_prefix = null;
			_activeColorTable = (_globalColorTable = null);
			_dest = null;
			_lastDest = null;
			MemoryStream memoryStream = new MemoryStream();
			using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
			{
				fileStream.CopyTo(memoryStream);
				fileStream.Close();
			}
			_gifData = memoryStream.GetBuffer();
			_backgroundTask = new BackgroundTask(Path.GetFileName(filePath), PlayGif, TimeSpan.Zero);
		}

		public int CheckImageCount()
		{
			int num = 0;
			int offset = _offset;
			_offset = 0;
			ParseHeader();
			ParseLogicalScreen();
			int num2 = _offset;
			_offset = offset;
			while (num2 < _gifData.Length && _gifData[num2] != 59)
			{
				switch (_gifData[num2])
				{
				case 33:
					num2 = ParseExtensionBlock(num2);
					break;
				case 44:
				{
					num++;
					byte b = _gifData[num2 + 9];
					if ((b & 0x80) > 0)
					{
						int num3 = b & 7;
						num3 = (int)Math.Pow(2.0, num3 + 1) * 3;
						num2 += num3;
					}
					num2 += 10;
					num2 = Skip(num2);
					break;
				}
				case 59:
					return num;
				default:
					num2++;
					break;
				}
			}
			return num;
		}

		private void ParseGif()
		{
			ParseHeader();
			ParseLogicalScreen();
			_startOffset = _offset;
			_stopWatch = new Stopwatch();
			if (_control == GifStatus.Play || _control == GifStatus.Replay)
			{
				AddBackgroundTask();
			}
		}

		private void PlayGif()
		{
			CheckControl();
			if (_offset == -1)
			{
				_isStaticGif = _frameCount == 1;
				if (_isStaticGif)
				{
					_stopWatch.Stop();
					RemoveBackgroundTask();
				}
				_frameCount = 0;
				_lastDest = null;
				_offset = _startOffset;
			}
			if (ParseBlock())
			{
				_frameCount++;
				if (_control == GifStatus.Stop)
				{
					_stopWatch.Stop();
					RemoveBackgroundTask();
				}
				Delay();
			}
			CheckControl();
		}

		private void CheckControl()
		{
			if (_control != GifStatus.Play)
			{
				if (_control == GifStatus.Stop)
				{
					RemoveBackgroundTask();
				}
				else if (_control == GifStatus.Replay)
				{
					_control = GifStatus.Play;
					_offset = _startOffset;
				}
			}
		}

		private void Delay()
		{
			_stopWatch.Stop();
			int num = _delay * 10;
			long elapsedMilliseconds = _stopWatch.ElapsedMilliseconds;
			if (elapsedMilliseconds < num)
			{
				num = (int)(num - elapsedMilliseconds);
			}
			_backgroundTask.DelayTime = TimeSpan.FromMilliseconds(num);
		}

		private void ParseHeader()
		{
			if (_gifData.Length < 3)
			{
				throw new Exception("Not a proper GIF file: data is missing");
			}
			if (Encoding.ASCII.GetString(_gifData, 0, 3) != "GIF")
			{
				throw new Exception("Not a proper GIF file: missing GIF header");
			}
			_offset = 6;
		}

		private void ParseLogicalScreen()
		{
			LogicalWidth = BitConverter.ToUInt16(_gifData, _offset);
			LogicalHeight = BitConverter.ToUInt16(_gifData, _offset + 2);
			_stride = 4 * LogicalWidth;
			byte b = _gifData[_offset + 4];
			_backgroundIndex = _gifData[_offset + 5];
			bool flag = (b & 0x80) > 0;
			_offset += 7;
			if (flag)
			{
				_globalColorTable = CreateColorTable(b);
				_backgroundColor = _globalColorTable[_backgroundIndex];
			}
		}

		private bool ParseBlock()
		{
			try
			{
				switch (_gifData[_offset])
				{
				case 33:
					ParseExtension();
					return false;
				case 44:
					ParseGraphicBlock();
					return true;
				case 59:
					if (AutoReplay)
					{
						_offset = -1;
					}
					else if (_control == GifStatus.Play)
					{
						Stop();
					}
					PlayOnceFinished?.Invoke(this, EventArgs.Empty);
					return false;
				default:
					_offset = -1;
					PlayOnceFinished?.Invoke(this, EventArgs.Empty);
					return false;
				}
			}
			catch (ThreadAbortException)
			{
				return true;
			}
			catch (Exception)
			{
				Stop();
				_hasError = true;
				return false;
			}
		}

		private void ParseExtension()
		{
			_stopWatch.Restart();
			if (_gifData[_offset + 1] == 249)
			{
				ParseGraphicControlExtension();
			}
			else
			{
				_offset = ParseExtensionBlock(_offset);
			}
		}

		private void ParseGraphicControlExtension()
		{
			int num = _gifData[_offset + 2];
			int num2 = _offset + num + 2 + 1;
			byte b = _gifData[_offset + 3];
			_dispose = (b & 0x1C) >> 2;
			if (_dispose == 0)
			{
				_dispose = 1;
			}
			_transparency = (b & 1) != 0;
			_delay = BitConverter.ToUInt16(_gifData, _offset + 4);
			if (_delay <= 1)
			{
				_delay = 10;
			}
			_transIndex = _gifData[_offset + 6];
			while (_gifData[num2] != 0)
			{
				num2 = num2 + _gifData[num2] + 1;
			}
			num2++;
			_offset = num2;
		}

		private int ParseExtensionBlock(int offset)
		{
			int num = _gifData[offset + 2];
			int num2 = offset + num + 2 + 1;
			if (_gifData[offset + 1] != byte.MaxValue || num <= 10 || Encoding.ASCII.GetString(_gifData, offset + 3, 8) == "NETSCAPE")
			{
			}
			while (_gifData[num2] != 0)
			{
				num2 = num2 + _gifData[num2] + 1;
			}
			return num2 + 1;
		}

		private void ParseGraphicBlock()
		{
			_frameLeft = BitConverter.ToUInt16(_gifData, _offset + 1);
			_frameTop = BitConverter.ToUInt16(_gifData, _offset + 3);
			_frameWidth = BitConverter.ToUInt16(_gifData, _offset + 5);
			_frameHeight = BitConverter.ToUInt16(_gifData, _offset + 7);
			if (_frameWidth > LogicalWidth)
			{
				LogicalWidth = _frameWidth;
			}
			if (_frameHeight > LogicalHeight)
			{
				LogicalHeight = _frameHeight;
			}
			byte b = _gifData[_offset + 9];
			bool flag = (b & 0x80) > 0;
			_interlace = (b & 0x40) > 0;
			_offset += 10;
			if (flag)
			{
				_activeColorTable = CreateColorTable(b);
			}
			else
			{
				_activeColorTable = _globalColorTable;
				if (_backgroundIndex == _transIndex)
				{
					_backgroundColor = 0;
				}
			}
			int num = 0;
			if (_transparency)
			{
				num = _activeColorTable[_transIndex];
				_activeColorTable[_transIndex] = 0;
			}
			int offset = _offset;
			DecodeImageData();
			try
			{
				PixelToImage();
			}
			catch (OutOfMemoryException)
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();
			}
			if (_transparency)
			{
				_activeColorTable[_transIndex] = num;
			}
			ResetFrame();
			_offset = Skip(offset);
		}

		private int[] CreateColorTable(byte packedField)
		{
			int num = packedField & 7;
			int num2 = (int)Math.Pow(2.0, num + 1);
			num = num2 * 3;
			int[] array = new int[num];
			Array.Copy(_gifData, _offset, array, 0, num);
			int[] array2 = new int[256];
			int num3 = 0;
			int num4 = 0;
			while (num3 < num2)
			{
				int num5 = array[num4++] & 0xFF;
				int num6 = array[num4++] & 0xFF;
				int num7 = array[num4++] & 0xFF;
				array2[num3++] = (int)((num5 << 16) | (num6 << 8) | num7 | 0xFF000000u);
			}
			_offset += num;
			return array2;
		}

		private void DecodeImageData()
		{
			int num = _frameWidth * _frameHeight;
			if (_pixels == null || _pixels.Length < num)
			{
				_pixels = new byte[num];
			}
			if (_prefix == null)
			{
				_prefix = new short[4096];
			}
			if (_suffix == null)
			{
				_suffix = new byte[4096];
			}
			if (_pixelStack == null)
			{
				_pixelStack = new byte[4097];
			}
			int num2 = _gifData[_offset];
			int num3 = 1 << num2;
			int num4 = num3 + 1;
			int num5 = num3 + 2;
			int num6 = -1;
			int num7 = num2 + 1;
			int num8 = (1 << num7) - 1;
			for (int i = 0; i < num3 && i < _prefix.Length && i < _suffix.Length; i++)
			{
				_prefix[i] = 0;
				_suffix[i] = (byte)i;
			}
			int num9 = 0;
			int num10 = 0;
			int num11 = 0;
			int num12 = 0;
			int num13 = 0;
			int num14 = 0;
			int num15 = 0;
			int num16 = 0;
			while (num16 < num)
			{
				if (num11 == 0)
				{
					if (num12 < num7)
					{
						if (num14 == 0)
						{
							num14 = ReadBlock(_gifData, _offset + 1);
							if (num14 <= 0)
							{
								break;
							}
							num10 = 0;
							_offset += num14 + 1;
						}
						num13 += (_block[num10] & 0xFF) << num12;
						num12 += 8;
						num10++;
						num14--;
						continue;
					}
					int num17 = num13 & num8;
					num13 >>= num7;
					num12 -= num7;
					if (num17 > num5 || num17 == num4)
					{
						break;
					}
					if (num17 == num3)
					{
						num7 = num2 + 1;
						num8 = (1 << num7) - 1;
						num5 = num3 + 2;
						num6 = -1;
						continue;
					}
					if (num6 == -1)
					{
						_pixelStack[num11++] = _suffix[num17];
						num6 = num17;
						num15 = num17;
						continue;
					}
					int num18 = num17;
					if (num17 == num5)
					{
						_pixelStack[num11++] = (byte)num15;
						num17 = num6;
					}
					while (num17 > num3)
					{
						_pixelStack[num11++] = _suffix[num17];
						num17 = _prefix[num17];
					}
					num15 = _suffix[num17] & 0xFF;
					if (num5 >= 4096)
					{
						num11++;
						continue;
					}
					_pixelStack[num11++] = (byte)num15;
					_prefix[num5] = (short)num6;
					_suffix[num5] = (byte)num15;
					num5++;
					if ((num5 & num8) == 0 && num5 < 4096)
					{
						num7++;
						num8 += num5;
					}
					num6 = num18;
				}
				num11--;
				_pixels[num9++] = _pixelStack[num11];
				num16++;
			}
			for (int j = num9; j < num; j++)
			{
				_pixels[j] = 0;
			}
			_offset++;
		}

		private int ReadBlock(byte[] gifData, int offset)
		{
			byte b = gifData[offset];
			if (b <= 0)
			{
				return b;
			}
			Array.Copy(gifData, offset + 1, _block, 0, b);
			return b;
		}

		private void PixelToImage()
		{
			UpdateSource();
			if (_lastDispose > 0)
			{
				if (_lastDispose == 3 && _frameCount - 2 <= 0)
				{
					_lastDest = null;
				}
				if (_lastDest != null)
				{
					_dest = _lastDest;
					if (_lastDispose == 2)
					{
						int num = 0;
						if (!_transparency)
						{
							num = _lastBackgroundColor;
						}
						for (int i = 0; i < _lastFrameHeight; i++)
						{
							int num2 = (_lastFrameTop + i) * LogicalWidth + _lastFrameLeft;
							int num3 = num2 + _lastFrameWidth;
							for (int j = num2; j < num3; j++)
							{
								_dest[j] = num;
							}
						}
					}
				}
			}
			_dest = _dest ?? new int[LogicalWidth * LogicalHeight];
			int num4 = 1;
			int num5 = 8;
			int num6 = 0;
			for (int k = 0; k < _frameHeight; k++)
			{
				int num7 = k;
				if (_interlace)
				{
					if (num6 >= _frameHeight)
					{
						num4++;
						switch (num4)
						{
						case 2:
							num6 = 4;
							break;
						case 3:
							num6 = 2;
							num5 = 4;
							break;
						case 4:
							num6 = 1;
							num5 = 2;
							break;
						}
					}
					num7 = num6;
					num6 += num5;
				}
				num7 += _frameTop;
				if (num7 >= LogicalHeight)
				{
					continue;
				}
				int num8 = num7 * LogicalWidth;
				int l = num8 + _frameLeft;
				int num9 = l + _frameWidth;
				if (num8 + LogicalWidth < num9)
				{
					num9 = num8 + LogicalWidth;
				}
				int num10 = k * _frameWidth;
				for (; l < num9; l++)
				{
					int num11 = _pixels[num10++] & 0xFF;
					int num12 = _activeColorTable[num11];
					if (num12 != 0)
					{
						_dest[l] = num12;
					}
				}
			}
			Application.Current?.Dispatcher.InvokeAsync(() =>
			{
				if (LogicalWidth * LogicalHeight == _dest.Length)
				{
					_source.WritePixels(new Int32Rect(0, 0, LogicalWidth, LogicalHeight), _dest, _stride, 0);
					SourceChanged?.Invoke(this, Source);
				}
				else
				{
					SourceChanged?.Invoke(this, BitmapSource.Create(LogicalWidth, LogicalHeight, 96.0, 96.0, PixelFormats.Bgra32, null, _dest, _stride));
				}
			});
		}

		private void UpdateSource()
		{
			Application.Current?.Dispatcher.InvokeAsync(() =>
			{
				if (_source == null || _source.PixelWidth != LogicalWidth || _source.PixelHeight != LogicalHeight)
				{
					_source = new WriteableBitmap(LogicalWidth, LogicalHeight, 96.0, 96.0, PixelFormats.Bgra32, null);
					SourceChanged?.Invoke(this, _source);
				}
			});
		}

		private void ResetFrame()
		{
			_lastDest = _dest;
			_transparency = false;
			_lastDispose = _dispose;
			_lastFrameTop = _frameTop;
			_lastFrameLeft = _frameLeft;
			_lastFrameWidth = _frameWidth;
			_lastFrameHeight = _frameHeight;
			_lastBackgroundColor = _backgroundColor;
		}

		private int Skip(int offset)
		{
			int num = offset;
			num++;
			while (_gifData[num] != 0)
			{
				num += _gifData[num];
				num++;
			}
			return num + 1;
		}

		private void AddBackgroundTask()
		{
			if (!GifThreadManager.Instance.ContainsBackgroundTask(_backgroundTask))
			{
				GifThreadManager.Instance.RunBackgroundTask(_backgroundTask);
			}
		}

		private void RemoveBackgroundTask()
		{
			GifThreadManager.Instance.RemoveBackgroundTask(_backgroundTask);
			_backgroundTask.DelayTime = TimeSpan.Zero;
		}

		[CompilerGenerated]
		private bool _003CPlay_003Eg__IsParsed_007C52_0()
		{
			return _startOffset > 0;
		}
	}
}
