using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	public class StorageNodeFormatException : StorageNoteFormatException
	{
		
		public StorageNodeFormatException(Type type, string value)
			: base(type, value)
		{
		}
	}
}
