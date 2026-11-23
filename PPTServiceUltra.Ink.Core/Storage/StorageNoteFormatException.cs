using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	[Obsolete("拼写错误，请使用 StorageNodeFormatException。")]
	public class StorageNoteFormatException : StorageNodeException
	{
		public override string Message => Type.Name + "的格式不正确，值为" + Value + "。";

		public Type Type { get; }

		public string Value { get; }

		public StorageNoteFormatException(Type type, string value)
		{
			Type = type;
			Value = value;
		}
	}
}
