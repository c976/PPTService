using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Exceptions
{
	
	
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("请使用 UnknownStorageNodeException 代替，本类型命名拼写错误。")]
	public class UnknowStorageNodeException : StorageNodeException
	{
		public override string Message => "无法创建类型" + TypeName + "，请确保该类型已实现并被加载，并检查协议名是否正确。";

		public Type Type { get; private set; }

		public string TypeName { get; }

		public UnknowStorageNodeException(Type type)
		{
			Type = type;
			TypeName = type.Name;
		}

		public UnknowStorageNodeException(Type type, string typeName)
		{
			Type = type;
			TypeName = typeName;
		}

		public UnknowStorageNodeException(string typeName)
		{
			TypeName = typeName;
		}
	}
}
