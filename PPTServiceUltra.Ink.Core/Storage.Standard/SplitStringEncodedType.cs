using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	public abstract class SplitStringEncodedType< TParam> : EncodedType
	{
		private const char Separator = ',';

		public override string Encode()
		{
			StringBuilder stringBuilder = new StringBuilder();
			TParam[] array = GetParams();
			foreach (TParam val in array)
			{
				stringBuilder.Append((stringBuilder.Length == 0) ? Convert.ToString(val, CultureInfo.InvariantCulture) : $"{','}{Convert.ToString(val, CultureInfo.InvariantCulture)}");
			}
			return stringBuilder.ToString();
		}

		public override bool TryDecode(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return false;
			}
			string[] source = value.Split(',');
			try
			{
				SetParams(source.Select( (string param) => (TParam)Convert.ChangeType(param, typeof(TParam), CultureInfo.InvariantCulture)).ToArray());
			}
			catch (FormatException)
			{
				return false;
			}
			catch (InvalidCastException)
			{
				return false;
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}
			return true;
		}

		protected abstract TParam[] GetParams();

		protected abstract void SetParams(params TParam[] values);
	}
}
