using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Cache;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	public static class SaveInfoBaseExtensions
	{
		private static readonly CachePool<MemberInfo, SaveInfoContractAttribute> ContractCache = new CachePool<MemberInfo, SaveInfoContractAttribute>((MemberInfo x) => x.GetCustomAttribute<SaveInfoContractAttribute>(false), true);

		private static readonly CachePool<MemberInfo, SaveInfoMemberAttribute> MemberCache = new CachePool<MemberInfo, SaveInfoMemberAttribute>((MemberInfo x) => x.GetCustomAttribute<SaveInfoMemberAttribute>(false), true);

		private static readonly CachePool<Type, PropertyInfo[]> PropertiesCache = new CachePool<Type, PropertyInfo[]>((Type x) => x.GetProperties(), true);

		internal const string SaveInfoExtName = "SaveInfo";

		public static bool IsFallbackSaveInfo(this SaveInfo saveInfo)
		{
			if (saveInfo.OriginMetadata != null && saveInfo.OriginMetadata is MetadataSaveInfo)
			{
				return true;
			}
			return GetSaveInfoContractIsFallback(saveInfo.GetType());
		}

		private static bool GetSaveInfoContractIsFallback(Type type)
		{
			SaveInfoContractAttribute saveInfoContractAttribute = ContractCache[type];
			if (saveInfoContractAttribute != null)
			{
				if (!saveInfoContractAttribute.IsFallbackElement)
				{
					return GetSaveInfoContractIsFallback(type.BaseType);
				}
				return true;
			}
			return false;
		}

		public static string GetSaveInfoContractName(this SaveInfo saveInfo)
		{
			return GetSaveInfoContractName(saveInfo.GetType());
		}

		private static string GetSaveInfoContractName(Type type)
		{
			SaveInfoContractAttribute saveInfoContractAttribute = ContractCache[type];
			if (saveInfoContractAttribute != null)
			{
				return saveInfoContractAttribute.Name ?? type.Name.Replace("SaveInfo", string.Empty);
			}
			return string.Empty;
		}

		public static string GetSaveInfoUniqueIdentifyMemberName(this SaveInfo saveInfo)
		{
			return GetSaveInfoUniqueIdentifyMemberName(saveInfo.GetType());
		}

		private static string GetSaveInfoUniqueIdentifyMemberName(Type type)
		{
			SaveInfoContractAttribute saveInfoContractAttribute = ContractCache[type];
			if (saveInfoContractAttribute == null)
			{
				return string.Empty;
			}
			string text = saveInfoContractAttribute.UniqueIdentifyMemberName ?? string.Empty;
			if (string.IsNullOrWhiteSpace(text) && type.BaseType != null)
			{
				GetSaveInfoUniqueIdentifyMemberName(type.BaseType);
			}
			return text;
		}

		public static bool GetSaveInfoSubPropertyDeltaSaveEnable(this SaveInfo saveInfo)
		{
			return GetSaveInfoSubPropertyDeltaSaveEnable(saveInfo.GetType());
		}

		private static bool GetSaveInfoSubPropertyDeltaSaveEnable(Type type)
		{
			SaveInfoContractAttribute saveInfoContractAttribute = ContractCache[type];
			if (saveInfoContractAttribute != null)
			{
				if (saveInfoContractAttribute.IsSubPropertyDeltaSaveEnabled)
				{
					return GetSaveInfoSubPropertyDeltaSaveEnable(type.BaseType);
				}
				return false;
			}
			return true;
		}

		public static string GetSaveInfoFallbackType(this SaveInfo saveInfo)
		{
			return GetSaveInfoFallbackType(saveInfo.GetType());
		}

		private static string GetSaveInfoFallbackType(Type type)
		{
			SaveInfoContractAttribute saveInfoContractAttribute = ContractCache[type];
			if (saveInfoContractAttribute != null)
			{
				if ((object)saveInfoContractAttribute.FallbackSaveInfoType == null && type.BaseType != null)
				{
					return GetSaveInfoFallbackType(type.BaseType);
				}
				if (saveInfoContractAttribute.FallbackSaveInfoType != null)
				{
					return GetSaveInfoContractName(saveInfoContractAttribute.FallbackSaveInfoType);
				}
			}
			return string.Empty;
		}

		public static SaveInfoType GetSaveInfoType(this SaveInfo saveInfo)
		{
			return GetSaveInfoType(saveInfo.GetType());
		}

		private static SaveInfoType GetSaveInfoType(Type type)
		{
			SaveInfoContractAttribute saveInfoContractAttribute = ContractCache[type];
			if (saveInfoContractAttribute == null)
			{
				return SaveInfoType.Unset;
			}
			if (saveInfoContractAttribute.SaveInfoType == SaveInfoType.Unset)
			{
				if (type.BaseType != null)
				{
					return GetSaveInfoType(type.BaseType);
				}
				return SaveInfoType.Unset;
			}
			return saveInfoContractAttribute.SaveInfoType;
		}

		public static string GetSaveInfoFriendlyName(this SaveInfo saveInfo)
		{
			return GetSaveInfoFriendlyName(saveInfo.GetType());
		}

		private static string GetSaveInfoFriendlyName(Type type)
		{
			SaveInfoContractAttribute saveInfoContractAttribute = ContractCache[type];
			if (saveInfoContractAttribute != null)
			{
				return saveInfoContractAttribute.FriendlyName ?? string.Empty;
			}
			return string.Empty;
		}

		public static string GetSaveInfoMemberName(this SaveInfo saveInfo)
		{
			SaveInfoMemberAttribute saveInfoMemberAttribute = MemberCache[saveInfo.GetType()];
			if (saveInfoMemberAttribute != null)
			{
				string name = saveInfoMemberAttribute.Name;
				if (string.IsNullOrWhiteSpace(name))
				{
					string name2 = saveInfo.GetType().Name;
					if (name2.EndsWith("SaveInfo", StringComparison.OrdinalIgnoreCase))
					{
						return name2.Replace("SaveInfo", string.Empty);
					}
					return name2;
				}
				return name;
			}
			return string.Empty;
		}

		public static bool GetMemberIsDeltaSupported(this PropertyInfo propertyInfo)
		{
			return MemberCache[propertyInfo]?.IsDeltaSupported ?? true;
		}

		public static PropertyInfo[] GetMembers(this SaveInfo saveInfo, bool isRuntime = false)
		{
			return PropertiesCache[saveInfo.GetType()].Where( (PropertyInfo p) =>
			{
				SaveInfoMemberAttribute saveInfoMemberAttribute = MemberCache[p];
				if (saveInfoMemberAttribute == null)
				{
					return false;
				}
				return isRuntime || !saveInfoMemberAttribute.IsRuntime;
			}).ToArray();
		}

		public static PropertyInfo[] GetAllMembers(this SaveInfo saveInfo)
		{
			return saveInfo.GetMembers(true);
		}
	}
}
