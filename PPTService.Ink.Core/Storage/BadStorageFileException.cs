using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	public class BadStorageFileException : StorageException
	{
		public override string Message => "Cannot Read Content:\"" + SubString(Content) + "\"";

		public string Content { get; set; }

		public override string ToString()
		{
			return "无法识别的文档内容：" + (Content ?? string.Empty) + "\n" + base.ToString();
		}

		private string SubString(string content)
		{
			if (string.IsNullOrWhiteSpace(content))
			{
				if (content != null)
				{
					return "\" \"";
				}
				return "null";
			}
			if (content.Length <= 11)
			{
				return content;
			}
			return content.Substring(0, 8) + "...";
		}
	}
}
