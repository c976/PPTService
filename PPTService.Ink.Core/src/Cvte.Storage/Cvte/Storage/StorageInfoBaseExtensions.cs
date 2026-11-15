using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	public static class StorageInfoBaseExtensions
	{
		internal const string StorageInfoExtName = "StorageInfo";

		public static string GetStorageInfoMemberName(this StorageModel storageModel)
		{
			StorageInfoMemberAttribute customAttribute = storageModel.GetType().GetCustomAttribute<StorageInfoMemberAttribute>(false);
			if (customAttribute != null)
			{
				return customAttribute.Name ?? storageModel.GetType().Name.Replace("StorageInfo", string.Empty);
			}
			return string.Empty;
		}

		public static PropertyInfo[] GetContentMembers(this StorageModel storageModel)
		{
			return storageModel.GetType().GetProperties().Where( (PropertyInfo p) =>
			{
				StorageInfoMemberAttribute customAttribute = p.GetCustomAttribute<StorageInfoMemberAttribute>();
				return customAttribute != null && customAttribute.Type == StorageFileType.Content;
			})
				.ToArray();
		}

		public static string GetThumbnailName(this StorageModel storageModel)
		{
			using (IEnumerator<StorageInfoMemberAttribute> enumerator = (from property in storageModel.GetType().GetProperties()
				select property.GetCustomAttribute<StorageInfoMemberAttribute>() into attr
				where attr != null && attr.Type == StorageFileType.Thumbnail
				select attr).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current.Name;
				}
			}
			return string.Empty;
		}
	}
}
