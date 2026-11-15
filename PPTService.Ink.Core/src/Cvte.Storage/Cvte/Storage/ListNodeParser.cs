using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Storage.AdvancedSaveInfos;
using PPTService.Ink.Core.Storage.Standard;

namespace PPTService.Ink.Core.Storage
{
	
	
	internal class ListNodeParser : IStorableNodeParser
	{
		private readonly UriNodeParser _uriNodeParser;

		private readonly Func<StorableNode, SaveInfo> _saveInfoParseFunc;

		private readonly ISaveInfoManager _saveInfoManager;

		private readonly IStorageLogger _storageLogger;

		private readonly Func<IReferenceManager> _referenceManager;

		private readonly Func<StorableNode, Type, SaveInfo> _givenTypeSaveInfoParseFunc;

		private readonly Func<SaveInfo, StorableNode> _saveInfoDeparseFunc;

		public ListNodeParser(Func<StorableNode, SaveInfo> saveInfoParseFunc, Func<SaveInfo, StorableNode> saveInfoDeparseFunc, UriNodeParser uriNodeParser)
		{
			_saveInfoParseFunc = saveInfoParseFunc;
			_saveInfoDeparseFunc = saveInfoDeparseFunc;
			_uriNodeParser = uriNodeParser;
		}

		public ListNodeParser(ISaveInfoManager saveInfoManager, IStorageLogger storageLogger, Func<IReferenceManager> getReferenceManager, Func<StorableNode, SaveInfo> saveInfoParseFunc, Func<StorableNode, Type, SaveInfo> givenTypeSaveInfoParseFunc, Func<SaveInfo, StorableNode> saveInfoDeparseFunc, UriNodeParser uriNodeParser)
			: this(saveInfoParseFunc, saveInfoDeparseFunc, uriNodeParser)
		{
			_saveInfoManager = saveInfoManager;
			_storageLogger = storageLogger;
			_referenceManager = getReferenceManager;
			_givenTypeSaveInfoParseFunc = givenTypeSaveInfoParseFunc;
		}

		public bool CanParse(Type type)
		{
			if (typeof(IEnumerable).IsAssignableFrom(type))
			{
				return type.GenericTypeArguments.Any();
			}
			return false;
		}

		public bool CanDeparse(object value)
		{
			return value is IList;
		}

		public object Parse(StorableNode node, Type type, PropertyInfo property)
		{
			if (!node.HasChild)
			{
				return null;
			}
			Type type2 = type.GenericTypeArguments[0];
			IList list = (IList)Activator.CreateInstance(GetArrayType(type, type2, property));
			foreach (StorableNode child in node.Children)
			{
				if (typeof(SaveInfo).IsAssignableFrom(type2))
				{
					IFallbackArray fallbackArray = list as IFallbackArray;
					SaveInfo saveInfo;
					if (fallbackArray != null && !_saveInfoManager.TryGet(child.Name, out saveInfo) && child.IsSupportFallback(_saveInfoParseFunc))
					{
						fallbackArray.UnknownNode.Add(child);
						_storageLogger.Warning("未能在列表 " + type.Name + " 中加载类型为 " + child.Name + " 的元素；加入未知节点中。");
					}
					SaveInfo saveInfo2 = _saveInfoParseFunc(child);
					if (saveInfo2 != null)
					{
						list.Add(saveInfo2);
					}
				}
				else if (typeof(PPTService.Ink.Core.Storage.Standard.Uri).IsAssignableFrom(type2))
				{
					object obj = _uriNodeParser.Parse(child, null, null);
					if (obj != null)
					{
						list.Add(obj);
					}
				}
				else if (typeof(IStorableType).IsAssignableFrom(type2) || typeof(IList).IsAssignableFrom(type2))
				{
					StorableNode arg = new StorableNode("StorableTypeItem", new List<StorableNode> { child });
					SaveInfo saveInfo3 = _givenTypeSaveInfoParseFunc(arg, type2);
					if (saveInfo3 != null)
					{
						list.Add(((ItemSaveInfo)saveInfo3).Item);
					}
				}
				else
				{
					list.Add(Convert.ChangeType(child.Value, type2, CultureInfo.InvariantCulture));
				}
			}
			return list;
		}

		public void Deparse(string name, object value, StorableNode node, PropertyInfo property)
		{
			IList list = value as IList;
			if (list == null)
			{
				return;
			}
			StorableNode storableNode = new StorableNode
			{
				Name = name,
				IsArray = true,
				IsDeltaSupported = (property == null || property.GetMemberIsDeltaSupported())
			};
			for (int i = 0; i < list.Count; i++)
			{
				object obj = list[i];
				if (obj is SaveInfo)
				{
					StorableNode storableNode2 = _saveInfoDeparseFunc(obj as SaveInfo);
					if (storableNode2 != null)
					{
						storableNode.AddChild(storableNode2);
					}
				}
				else if (obj is PPTService.Ink.Core.Storage.Standard.Uri)
				{
					_uriNodeParser.Deparse("Item", obj, storableNode, null);
				}
				else if (obj is IStorableType || obj is IList)
				{
					ItemSaveInfo arg = new ItemSaveInfo
					{
						Item = obj
					};
					StorableNode storableNode3 = _saveInfoDeparseFunc(arg)?.FirstNode;
					if (storableNode3 != null)
					{
						storableNode.AddChild(storableNode3);
					}
				}
				else
				{
					storableNode.AddChild(new StorableNode("Item")
					{
						Value = Convert.ToString(obj, CultureInfo.InvariantCulture)
					});
				}
			}
			IFallbackArray fallbackArray = value as IFallbackArray;
			if (fallbackArray != null)
			{
				foreach (StorableNode item in fallbackArray.UnknownNode)
				{
					if (item.IsSupportFallback(_saveInfoParseFunc))
					{
						UnknownNodeReferenceHelper.RecordReference(_referenceManager?.Invoke(), item);
						storableNode.AddChild(item);
					}
				}
			}
			node.AddChild(storableNode);
		}

		private Type GetArrayType(Type listType, Type itemType,  PropertyInfo listTypeProperty)
		{
			if ((listTypeProperty?.GetCustomAttribute<SaveInfoMemberAttribute>())?.IsFallbackArray ?? false)
			{
				return typeof(FallbackArray<>).MakeGenericType(itemType);
			}
			if (!typeof(IFallbackArray).IsAssignableFrom(listType))
			{
				return typeof(List<>).MakeGenericType(itemType);
			}
			return typeof(FallbackArray<>).MakeGenericType(itemType);
		}
	}
}
