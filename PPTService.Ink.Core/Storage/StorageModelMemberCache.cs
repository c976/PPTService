using System.Reflection;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	internal class StorageModelMemberCache
	{
		public string Name { get; }

		public string Path { get; }

		public string NameRegex { get; }

		public bool IsRequired { get; }

		public StorageModelMemberCache(MemberInfo source)
		{
			StorageInfoMemberAttribute customAttribute = source.GetCustomAttribute<StorageInfoMemberAttribute>(false);
			Name = customAttribute?.Name ?? source.GetType().Name.Replace("StorageInfo", string.Empty);
			Path = customAttribute?.Path ?? string.Empty;
			NameRegex = customAttribute?.NameRegex ?? string.Empty;
			IsRequired = customAttribute?.IsRequired ?? true;
		}

		public static implicit operator StorageModelMemberCache(MemberInfo source)
		{
			return new StorageModelMemberCache(source);
		}
	}
}
