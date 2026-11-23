using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.ComponentModel;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	public class FileUri : Uri
	{
		public FileUri()
		{
		}

		public FileUri(string value)
		{
			Value = UriUtils.RemovePrefix(value, "file://");
			VerifyFileUriValue(Value);
		}

		public override string Encode()
		{
			return "file://" + Value;
		}

		private static void VerifyFileUriValue(string value)
		{
			if (!string.IsNullOrWhiteSpace(value) && !File.Exists(value))
			{
				string message = "[CoreLib.Storage] FileUri Is Not Absulote File Path. Or file is not exists. " + value;
				if (DebuggingProperties.IsDebug)
				{
					throw new InvalidOperationException(message);
				}
				if (Debugger.IsAttached)
				{
					Debugger.Log(100, "CoreLib.Storage", message);
					Debugger.Break();
				}
			}
		}
	}
}
