using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	public class Color : EncodedType
	{
		public byte A { get; set; }

		public byte R { get; set; }

		public byte G { get; set; }

		public byte B { get; set; }

		public static implicit operator Color(System.Windows.Media.Color color)
		{
			return new Color
			{
				A = color.A,
				R = color.R,
				G = color.G,
				B = color.B
			};
		}

		public static implicit operator System.Windows.Media.Color(Color color)
		{
			if (color == null)
			{
				return default(System.Windows.Media.Color);
			}
			return new System.Windows.Media.Color
			{
				A = color.A,
				R = color.R,
				G = color.G,
				B = color.B
			};
		}

		public override string Encode()
		{
			return "#" + A.ToString("X2", CultureInfo.InvariantCulture) + R.ToString("X2", CultureInfo.InvariantCulture) + G.ToString("X2", CultureInfo.InvariantCulture) + B.ToString("X2", CultureInfo.InvariantCulture);
		}

		public override bool TryDecode(string value)
		{
			if (!value.StartsWith("#"))
			{
				return false;
			}
			value = value.Substring(1);
			try
			{
				switch (value.Length)
				{
				case 6:
					A = 0;
					R = byte.Parse(value.Substring(0, 2), NumberStyles.AllowHexSpecifier);
					G = byte.Parse(value.Substring(2, 2), NumberStyles.AllowHexSpecifier);
					B = byte.Parse(value.Substring(4, 2), NumberStyles.AllowHexSpecifier);
					return true;
				case 8:
					A = byte.Parse(value.Substring(0, 2), NumberStyles.AllowHexSpecifier);
					R = byte.Parse(value.Substring(2, 2), NumberStyles.AllowHexSpecifier);
					G = byte.Parse(value.Substring(4, 2), NumberStyles.AllowHexSpecifier);
					B = byte.Parse(value.Substring(6, 2), NumberStyles.AllowHexSpecifier);
					return true;
				}
			}
			catch (FormatException)
			{
				return false;
			}
			return false;
		}

		public static Color Parse(string value)
		{
			Color color = new Color();
			color.TryDecode(value);
			return color;
		}
	}
}
