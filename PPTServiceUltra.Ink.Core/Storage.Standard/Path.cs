using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	public class Path : EncodedType
	{
		public string Value { get; set; }

		public static implicit operator Path(System.Windows.Media.Geometry geometry)
		{
			if (geometry == null)
			{
				return null;
			}
			return new Path
			{
				Value = geometry.ToString(CultureInfo.InvariantCulture)
			};
		}

		public static implicit operator System.Windows.Media.Geometry(Path path)
		{
			if (path == null)
			{
				return null;
			}
			return System.Windows.Media.Geometry.Parse(path.Value);
		}

		public override string Encode()
		{
			return Value;
		}

		public override bool TryDecode(string value)
		{
			Value = value;
			return true;
		}
	}
}
