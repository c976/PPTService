using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	public class StorageNodeFormatException : StorageNoteFormatException
	{
		
		public StorageNodeFormatException(Type type, string value)
			: base(type, value)
		{
		}
	}
}
