using System.Reflection;
using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Cache;

namespace PPTService.Ink.Core.Storage
{
	
	
	public static class PropertyInfoExtensions
	{
		private static readonly CachePool<PropertyInfo, SaveInfoPropertyCache> SaveInfoCache = new CachePool<PropertyInfo, SaveInfoPropertyCache>((PropertyInfo x) => x, true);

		private static readonly CachePool<MemberInfo, StorageModelMemberCache> StorageInfoCache = new CachePool<MemberInfo, StorageModelMemberCache>((MemberInfo x) => x, true);

		internal static SaveInfoPropertyCache GetSaveInfoMemberCache(this PropertyInfo propertyInfo)
		{
			return SaveInfoCache[propertyInfo];
		}

		public static string GetSaveInfoMemberName(this PropertyInfo propertyInfo)
		{
			return SaveInfoCache[propertyInfo].Name;
		}

		public static string GetStorageInfoMemberName(this PropertyInfo propertyInfo)
		{
			return StorageInfoCache[propertyInfo].Name;
		}

		public static string GetStorageInfoMemberPath(this PropertyInfo propertyInfo)
		{
			return StorageInfoCache[propertyInfo].Path;
		}

		public static string GetStorageInfoMemberNameRegex(this PropertyInfo propertyInfo)
		{
			return StorageInfoCache[propertyInfo].NameRegex;
		}

		public static bool GetStorageInfoMemberIsRequired(this PropertyInfo propertyInfo)
		{
			return StorageInfoCache[propertyInfo].IsRequired;
		}
	}
}
