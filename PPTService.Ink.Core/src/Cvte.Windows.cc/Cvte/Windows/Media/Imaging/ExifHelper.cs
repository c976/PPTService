using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media.Imaging;

namespace PPTService.Ink.Core.Windows.Media.Imaging
{
	public class ExifHelper : IDisposable
	{
		public enum TagNames
		{
			ExifIFD = 34665,
			GpsIFD = 34853,
			NewSubfileType = 254,
			SubfileType = 255,
			ImageWidth = 256,
			ImageHeight = 257,
			BitsPerSample = 258,
			Compression = 259,
			PhotometricInterp = 262,
			ThreshHolding = 263,
			CellWidth = 264,
			CellHeight = 265,
			FillOrder = 266,
			DocumentName = 269,
			ImageDescription = 270,
			EquipMake = 271,
			EquipModel = 272,
			StripOffsets = 273,
			Orientation = 274,
			SamplesPerPixel = 277,
			RowsPerStrip = 278,
			StripBytesCount = 279,
			MinSampleValue = 280,
			MaxSampleValue = 281,
			XResolution = 282,
			YResolution = 283,
			PlanarConfig = 284,
			PageName = 285,
			XPosition = 286,
			YPosition = 287,
			FreeOffset = 288,
			FreeByteCounts = 289,
			GrayResponseUnit = 290,
			GrayResponseCurve = 291,
			T4Option = 292,
			T6Option = 293,
			ResolutionUnit = 296,
			PageNumber = 297,
			TransferFuncition = 301,
			SoftwareUsed = 305,
			DateTime = 306,
			Artist = 315,
			HostComputer = 316,
			Predictor = 317,
			WhitePoint = 318,
			PrimaryChromaticities = 319,
			ColorMap = 320,
			HalftoneHints = 321,
			TileWidth = 322,
			TileLength = 323,
			TileOffset = 324,
			TileByteCounts = 325,
			InkSet = 332,
			InkNames = 333,
			NumberOfInks = 334,
			DotRange = 336,
			TargetPrinter = 337,
			ExtraSamples = 338,
			SampleFormat = 339,
			SMinSampleValue = 340,
			SMaxSampleValue = 341,
			TransferRange = 342,
			JPEGProc = 512,
			JPEGInterFormat = 513,
			JPEGInterLength = 514,
			JPEGRestartInterval = 515,
			JPEGLosslessPredictors = 517,
			JPEGPointTransforms = 518,
			JPEGQTables = 519,
			JPEGDCTables = 520,
			JPEGACTables = 521,
			YCbCrCoefficients = 529,
			YCbCrSubsampling = 530,
			YCbCrPositioning = 531,
			REFBlackWhite = 532,
			ICCProfile = 34675,
			Gamma = 769,
			ICCProfileDescriptor = 770,
			SRGBRenderingIntent = 771,
			ImageTitle = 800,
			Copyright = 33432,
			ResolutionXUnit = 20481,
			ResolutionYUnit = 20482,
			ResolutionXLengthUnit = 20483,
			ResolutionYLengthUnit = 20484,
			PrintFlags = 20485,
			PrintFlagsVersion = 20486,
			PrintFlagsCrop = 20487,
			PrintFlagsBleedWidth = 20488,
			PrintFlagsBleedWidthScale = 20489,
			HalftoneLPI = 20490,
			HalftoneLPIUnit = 20491,
			HalftoneDegree = 20492,
			HalftoneShape = 20493,
			HalftoneMisc = 20494,
			HalftoneScreen = 20495,
			JPEGQuality = 20496,
			GridSize = 20497,
			ThumbnailFormat = 20498,
			ThumbnailWidth = 20499,
			ThumbnailHeight = 20500,
			ThumbnailColorDepth = 20501,
			ThumbnailPlanes = 20502,
			ThumbnailRawBytes = 20503,
			ThumbnailSize = 20504,
			ThumbnailCompressedSize = 20505,
			ColorTransferFunction = 20506,
			ThumbnailData = 20507,
			ThumbnailImageWidth = 20512,
			ThumbnailImageHeight = 1282,
			ThumbnailBitsPerSample = 20514,
			ThumbnailCompression = 20515,
			ThumbnailPhotometricInterp = 20516,
			ThumbnailImageDescription = 20517,
			ThumbnailEquipMake = 20518,
			ThumbnailEquipModel = 20519,
			ThumbnailStripOffsets = 20520,
			ThumbnailOrientation = 20521,
			ThumbnailSamplesPerPixel = 20522,
			ThumbnailRowsPerStrip = 20523,
			ThumbnailStripBytesCount = 20524,
			ThumbnailResolutionX = 20525,
			ThumbnailResolutionY = 20526,
			ThumbnailPlanarConfig = 20527,
			ThumbnailResolutionUnit = 20528,
			ThumbnailTransferFunction = 20529,
			ThumbnailSoftwareUsed = 20530,
			ThumbnailDateTime = 20531,
			ThumbnailArtist = 20532,
			ThumbnailWhitePoint = 20533,
			ThumbnailPrimaryChromaticities = 20534,
			ThumbnailYCbCrCoefficients = 20535,
			ThumbnailYCbCrSubsampling = 20536,
			ThumbnailYCbCrPositioning = 20537,
			ThumbnailRefBlackWhite = 20538,
			ThumbnailCopyRight = 20539,
			LuminanceTable = 20624,
			ChrominanceTable = 20625,
			FrameDelay = 20736,
			LoopCount = 20737,
			PixelUnit = 20752,
			PixelPerUnitX = 20753,
			PixelPerUnitY = 20754,
			PaletteHistogram = 20755,
			ExifExposureTime = 33434,
			ExifFNumber = 33437,
			ExifExposureProg = 34850,
			ExifSpectralSense = 34852,
			ExifISOSpeed = 34855,
			ExifOECF = 34856,
			ExifVer = 36864,
			ExifDTOrig = 36867,
			ExifDTDigitized = 36868,
			ExifCompConfig = 37121,
			ExifCompBPP = 37122,
			ExifShutterSpeed = 37377,
			ExifAperture = 37378,
			ExifBrightness = 37379,
			ExifExposureBias = 37380,
			ExifMaxAperture = 37381,
			ExifSubjectDist = 37382,
			ExifMeteringMode = 37383,
			ExifLightSource = 37384,
			ExifFlash = 37385,
			ExifFocalLength = 37386,
			ExifMakerNote = 37500,
			ExifUserComment = 37510,
			ExifDTSubsec = 37520,
			ExifDTOrigSS = 37521,
			ExifDTDigSS = 37522,
			ExifFPXVer = 40960,
			ExifColorSpace = 40961,
			ExifPixXDim = 40962,
			ExifPixYDim = 40963,
			ExifRelatedWav = 40964,
			ExifInterop = 40965,
			ExifFlashEnergy = 41483,
			ExifSpatialFR = 41484,
			ExifFocalXRes = 41486,
			ExifFocalYRes = 41487,
			ExifFocalResUnit = 41488,
			ExifSubjectLoc = 41492,
			ExifExposureIndex = 41493,
			ExifSensingMethod = 41495,
			ExifFileSource = 41728,
			ExifSceneType = 41729,
			ExifCfaPattern = 41730,
			GpsVer = 0,
			GpsLatitudeRef = 1,
			GpsLatitude = 2,
			GpsLongitudeRef = 3,
			GpsLongitude = 4,
			GpsAltitudeRef = 5,
			GpsAltitude = 6,
			GpsGpsTime = 7,
			GpsGpsSatellites = 8,
			GpsGpsStatus = 9,
			GpsGpsMeasureMode = 10,
			GpsGpsDop = 11,
			GpsSpeedRef = 12,
			GpsSpeed = 13,
			GpsTrackRef = 14,
			GpsTrack = 15,
			GpsImgDirRef = 16,
			GpsImgDir = 17,
			GpsMapDatum = 18,
			GpsDestLatRef = 19,
			GpsDestLat = 20,
			GpsDestLongRef = 21,
			GpsDestLong = 22,
			GpsDestBearRef = 23,
			GpsDestBear = 24,
			GpsDestDistRef = 25,
			GpsDestDist = 26
		}

		public enum Orientations
		{
			TopLeft = 1,
			TopRight,
			BottomRight,
			BottomLeft,
			LeftTop,
			RightTop,
			RightBottom,
			LeftBottom
		}

		public enum ExposurePrograms
		{
			Manual = 1,
			Normal,
			AperturePriority,
			ShutterPriority,
			Creative,
			Action,
			Portrait,
			Landscape
		}

		public enum ExposureMeteringModes
		{
			Unknown = 0,
			Average = 1,
			CenterWeightedAverage = 2,
			Spot = 3,
			MultiSpot = 4,
			MultiSegment = 5,
			Partial = 6,
			Other = 255
		}

		public enum FlashModes
		{
			NotFired = 0,
			Fired = 1,
			FiredButNoStrobeReturned = 5,
			FiredAndStrobeReturned = 7
		}

		public enum LightSources
		{
			Unknown = 0,
			Daylight = 1,
			Fluorescent = 2,
			Tungsten = 3,
			Flash = 10,
			StandardLightA = 17,
			StandardLightB = 18,
			StandardLightC = 19,
			D55 = 20,
			D65 = 21,
			D75 = 22,
			Other = 255
		}

		public enum ExifDataTypes : short
		{
			UnsignedByte = 1,
			AsciiString,
			UnsignedShort,
			UnsignedLong,
			UnsignedRational,
			SignedByte,
			Undefined,
			SignedShort,
			SignedLong,
			SignedRational,
			SingleFloat,
			DoubleFloat
		}

		public struct Rational
		{
			public int Numerator;

			public int Denominator;

			public override string ToString()
			{
				return ToString("/");
			}

			public string ToString(string Delimiter)
			{
				return Numerator + "/" + Denominator;
			}

			public double ToDouble()
			{
				return (double)Numerator / (double)Denominator;
			}
		}

		private readonly string _desDir;

		private readonly string _filePath;

		private readonly Bitmap _image;

		private Encoding _encoding = Encoding.UTF8;

		public Encoding Encoding
		{
			get
			{
				return _encoding;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				_encoding = value;
			}
		}

		public string EquipmentMaker => GetPropertyString(271);

		public string EquipmentModel => GetPropertyString(272);

		public string Software => GetPropertyString(305);

		public Orientations Orientation
		{
			get
			{
				int propertyInt = GetPropertyInt16(274);
				if (!Enum.IsDefined(typeof(Orientations), propertyInt))
				{
					return Orientations.TopLeft;
				}
				return (Orientations)Enum.Parse(typeof(Orientations), Enum.GetName(typeof(Orientations), propertyInt));
			}
		}

		public DateTime DateTimeLastModified
		{
			get
			{
				try
				{
					return DateTime.ParseExact(GetPropertyString(306), "yyyy\\:MM\\:dd HH\\:mm\\:ss", null);
				}
				catch
				{
					return DateTime.MinValue;
				}
			}
			set
			{
				try
				{
					SetPropertyString(306, value.ToString("yyyy\\:MM\\:dd HH\\:mm\\:ss"));
				}
				catch
				{
				}
			}
		}

		public DateTime DateTimeOriginal
		{
			get
			{
				try
				{
					return DateTime.ParseExact(GetPropertyString(36867), "yyyy\\:MM\\:dd HH\\:mm\\:ss", null);
				}
				catch
				{
					return DateTime.MinValue;
				}
			}
			set
			{
				try
				{
					SetPropertyString(36867, value.ToString("yyyy\\:MM\\:dd HH\\:mm\\:ss"));
				}
				catch
				{
				}
			}
		}

		public DateTime DateTimeDigitized
		{
			get
			{
				try
				{
					return DateTime.ParseExact(GetPropertyString(36868), "yyyy\\:MM\\:dd HH\\:mm\\:ss", null);
				}
				catch
				{
					return DateTime.MinValue;
				}
			}
			set
			{
				try
				{
					SetPropertyString(36868, value.ToString("yyyy\\:MM\\:dd HH\\:mm\\:ss"));
				}
				catch
				{
				}
			}
		}

		public int Width => _image.Width;

		public int Height => _image.Height;

		public double ResolutionX
		{
			get
			{
				double num = GetPropertyRational(282).ToDouble();
				if (GetPropertyInt16(296) == 3)
				{
					return num * 2.54;
				}
				return num;
			}
		}

		public double ResolutionY
		{
			get
			{
				double num = GetPropertyRational(283).ToDouble();
				if (GetPropertyInt16(296) == 3)
				{
					return num * 2.54;
				}
				return num;
			}
		}

		public string Title
		{
			get
			{
				return GetPropertyString(800);
			}
			set
			{
				try
				{
					SetPropertyString(800, value);
				}
				catch
				{
				}
			}
		}

		public string UserComment
		{
			get
			{
				return GetPropertyString(37510);
			}
			set
			{
				try
				{
					SetPropertyString(37510, value);
				}
				catch
				{
				}
			}
		}

		public string Artist
		{
			get
			{
				return GetPropertyString(315);
			}
			set
			{
				try
				{
					SetPropertyString(315, value);
				}
				catch
				{
				}
			}
		}

		public string Description
		{
			get
			{
				return GetPropertyString(270);
			}
			set
			{
				try
				{
					SetPropertyString(270, value);
				}
				catch
				{
				}
			}
		}

		public string Copyright
		{
			get
			{
				return GetPropertyString(33432);
			}
			set
			{
				try
				{
					SetPropertyString(33432, value);
				}
				catch
				{
				}
			}
		}

		public double ExposureTimeAbs
		{
			get
			{
				if (IsPropertyDefined(33434))
				{
					return GetPropertyRational(33434).ToDouble();
				}
				if (IsPropertyDefined(37377))
				{
					return 1.0 / Math.Pow(2.0, GetPropertyRational(37377).ToDouble());
				}
				return 0.0;
			}
		}

		public Rational ExposureTime
		{
			get
			{
				if (IsPropertyDefined(33434))
				{
					return GetPropertyRational(33434);
				}
				return default(Rational);
			}
		}

		public double Aperture
		{
			get
			{
				if (IsPropertyDefined(33437))
				{
					return GetPropertyRational(33437).ToDouble();
				}
				if (IsPropertyDefined(37378))
				{
					return Math.Pow(Math.Sqrt(2.0), GetPropertyRational(37378).ToDouble());
				}
				return 0.0;
			}
		}

		public ExposurePrograms ExposureProgram
		{
			get
			{
				int propertyInt = GetPropertyInt16(34850);
				if (Enum.IsDefined(typeof(ExposurePrograms), propertyInt))
				{
					return (ExposurePrograms)Enum.Parse(typeof(ExposurePrograms), Enum.GetName(typeof(ExposurePrograms), propertyInt));
				}
				return ExposurePrograms.Normal;
			}
		}

		public short ISO => GetPropertyInt16(34855);

		public double SubjectDistance => GetPropertyRational(37382).ToDouble();

		public ExposureMeteringModes ExposureMeteringMode
		{
			get
			{
				int propertyInt = GetPropertyInt16(37383);
				if (Enum.IsDefined(typeof(ExposureMeteringModes), propertyInt))
				{
					return (ExposureMeteringModes)Enum.Parse(typeof(ExposureMeteringModes), Enum.GetName(typeof(ExposureMeteringModes), propertyInt));
				}
				return ExposureMeteringModes.Unknown;
			}
		}

		public double FocalLength => GetPropertyRational(37386).ToDouble();

		public FlashModes FlashMode
		{
			get
			{
				int propertyInt = GetPropertyInt16(37385);
				if (Enum.IsDefined(typeof(FlashModes), propertyInt))
				{
					return (FlashModes)Enum.Parse(typeof(FlashModes), Enum.GetName(typeof(FlashModes), propertyInt));
				}
				return FlashModes.NotFired;
			}
		}

		public LightSources LightSource
		{
			get
			{
				int propertyInt = GetPropertyInt16(37384);
				if (Enum.IsDefined(typeof(LightSources), propertyInt))
				{
					return (LightSources)Enum.Parse(typeof(LightSources), Enum.GetName(typeof(LightSources), propertyInt));
				}
				return LightSources.Unknown;
			}
		}

		public ExifHelper(Bitmap bitmap)
		{
			if (bitmap == null)
			{
				throw new ArgumentNullException("bitmap");
			}
			_image = bitmap;
		}

		public ExifHelper(string filePath, string dir)
		{
			_desDir = dir;
			_filePath = filePath;
			_image = (Bitmap)Image.FromFile(filePath);
		}

		public Bitmap GetBitmap()
		{
			return (Bitmap)_image.Clone();
		}

		public string GetRotatedImage()
		{
			Orientations orientation = Orientation;
			if (orientation != Orientations.BottomRight && orientation != Orientations.RightTop && orientation != Orientations.LeftBottom)
			{
				return _filePath;
			}
			if (!Directory.Exists(_desDir))
			{
				Directory.CreateDirectory(_desDir);
			}
			string extension = Path.GetExtension(_filePath);
			string text = Path.Combine(_desDir, Guid.NewGuid().ToString() + extension);
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.UriSource = new Uri(_filePath);
			switch (orientation)
			{
			case Orientations.BottomRight:
				bitmapImage.Rotation = Rotation.Rotate180;
				break;
			case Orientations.RightTop:
				bitmapImage.Rotation = Rotation.Rotate90;
				break;
			case Orientations.LeftBottom:
				bitmapImage.Rotation = Rotation.Rotate270;
				break;
			}
			bitmapImage.EndInit();
			bitmapImage.SaveToFile(text, true);
			return text;
		}

		public bool IsPropertyDefined(int PID)
		{
			return Array.IndexOf(_image.PropertyIdList, PID) > -1;
		}

		public int GetPropertyInt32(int pid, int defaultValue = 0)
		{
			if (IsPropertyDefined(pid))
			{
				PropertyItem propertyItem = _image.GetPropertyItem(pid);
				if (propertyItem != null)
				{
					return GetInt32(propertyItem.Value);
				}
			}
			return defaultValue;
		}

		public short GetPropertyInt16(int PID)
		{
			return GetPropertyInt16(PID, 0);
		}

		public short GetPropertyInt16(int PID, short DefaultValue)
		{
			if (IsPropertyDefined(PID))
			{
				return GetInt16(_image.GetPropertyItem(PID).Value);
			}
			return DefaultValue;
		}

		public string GetPropertyString(int pid, string defaultValue = "")
		{
			if (IsPropertyDefined(pid))
			{
				PropertyItem propertyItem = _image.GetPropertyItem(pid);
				if (propertyItem != null)
				{
					return GetString(propertyItem.Value);
				}
			}
			return defaultValue;
		}

		public byte[] GetProperty(int PID, byte[] DefaultValue)
		{
			if (IsPropertyDefined(PID))
			{
				return _image.GetPropertyItem(PID).Value;
			}
			return DefaultValue;
		}

		public byte[] GetProperty(int PID)
		{
			return GetProperty(PID, null);
		}

		public Rational GetPropertyRational(int pid)
		{
			if (IsPropertyDefined(pid))
			{
				PropertyItem propertyItem = _image.GetPropertyItem(pid);
				if (propertyItem == null)
				{
					return _003CGetPropertyRational_003Eg__GetDefault_007C86_0();
				}
				return GetRational(propertyItem.Value);
			}
			return _003CGetPropertyRational_003Eg__GetDefault_007C86_0();
		}

		public void SetPropertyString(int pid, string value)
		{
			byte[] bytes = _encoding.GetBytes(value + "\0");
			SetProperty(pid, bytes, ExifDataTypes.AsciiString);
		}

		public void SetPropertyInt16(int pid, short value)
		{
			SetProperty(pid, new byte[2]
			{
				(byte)(value & 0xFF),
				(byte)((value & 0xFF00) >> 8)
			}, ExifDataTypes.SignedShort);
		}

		public void SetPropertyInt32(int PID, int Value)
		{
			byte[] array = new byte[4];
			for (int i = 0; i < 4; i++)
			{
				array[i] = (byte)(Value & 0xFF);
				Value >>= 8;
			}
			SetProperty(PID, array, ExifDataTypes.SignedLong);
		}

		public void SetProperty(int PID, byte[] Data, ExifDataTypes Type)
		{
			PropertyItem propertyItem = _image.PropertyItems[0];
			propertyItem.Id = PID;
			propertyItem.Value = Data;
			propertyItem.Type = (short)Type;
			propertyItem.Len = Data.Length;
			_image.SetPropertyItem(propertyItem);
		}

		private int GetInt32(byte[] B)
		{
			if (B.Length < 4)
			{
				throw new ArgumentException("Data too short (4 bytes expected)", "B");
			}
			return (B[3] << 24) | (B[2] << 16) | (B[1] << 8) | B[0];
		}

		private short GetInt16(byte[] B)
		{
			if (B.Length < 2)
			{
				throw new ArgumentException("Data too short (2 bytes expected)", "B");
			}
			return (short)((B[1] << 8) | B[0]);
		}

		private string GetString(byte[] B)
		{
			string text = _encoding.GetString(B);
			if (text.EndsWith("\0"))
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text;
		}

		private Rational GetRational(byte[] B)
		{
			Rational result = default(Rational);
			byte[] array = new byte[4];
			byte[] array2 = new byte[4];
			Array.Copy(B, 0, array, 0, 4);
			Array.Copy(B, 4, array2, 0, 4);
			result.Denominator = GetInt32(array2);
			result.Numerator = GetInt32(array);
			return result;
		}

		public void Dispose()
		{
			_image.Dispose();
		}

		[CompilerGenerated]
		internal static Rational _003CGetPropertyRational_003Eg__GetDefault_007C86_0()
		{
			Rational result = default(Rational);
			result.Numerator = 0;
			result.Denominator = 1;
			return result;
		}
	}
}
