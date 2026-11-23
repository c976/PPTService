using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	public abstract class Uri
	{
		public string Value { get; set; }

		public static implicit operator Uri(System.Uri value)
		{
			if (value == null)
			{
				return null;
			}
			Uri uri = Create(value?.OriginalString);
			if (uri != null)
			{
				return uri;
			}
			return new FileUri(value?.OriginalString ?? string.Empty);
		}

		public static implicit operator System.Uri(Uri uri)
		{
			if (uri == null)
			{
				return null;
			}
			return new System.Uri(uri.Value, UriKind.RelativeOrAbsolute);
		}

		public static Uri Create(string value)
		{
			if (value.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
			{
				return new FileUri(value);
			}
			if (value.StartsWith("id://", StringComparison.OrdinalIgnoreCase))
			{
				return new IdUri(value);
			}
			if (value.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
			{
				return new HttpUri(value);
			}
			if (value.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
			{
				return new HttpUri(value);
			}
			if (value.StartsWith("app://", StringComparison.OrdinalIgnoreCase))
			{
				return new AppUri(value);
			}
			if (value.StartsWith("base64://", StringComparison.OrdinalIgnoreCase))
			{
				return new Base64Uri(value);
			}
			return null;
		}

		public abstract string Encode();
	}
}
