using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	[AttributeUsage(AttributeTargets.Property)]
	public class StorageInfoMemberAttribute : Attribute
	{
		public string Name { get; set; }

		public string NameRegex { get; set; }

		public string Path { get; set; }

		public StorageFileType Type { get; set; }

		public bool IsRequired { get; set; } = true;
	}
}
