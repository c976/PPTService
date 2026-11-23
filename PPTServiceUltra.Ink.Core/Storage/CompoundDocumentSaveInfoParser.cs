using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Storage.AdvancedSaveInfos;
using PPTServiceUltra.Ink.Core.Storage.Standard;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	public abstract class CompoundDocumentSaveInfoParser : ISaveInfoMetadataParser, ISaveInfoParser, IMetadataProvider, ISaveInfoNodeParser
	{
		private readonly SaveInfoMetadataManager _metadataManager = new SaveInfoMetadataManager();

		private readonly IList<IStorableNodeParser> _nodeParsers = new List<IStorableNodeParser>();

		private readonly ISaveInfoManager _saveInfoManager;

		private IReferenceManager _referenceManager;

		private IStorageLogger _logger;

		public bool IsRuntime { get; set; }

		protected CompoundDocumentSaveInfoParser(ISaveInfoManager saveInfoManager)
			: this(saveInfoManager, null)
		{
		}

		protected CompoundDocumentSaveInfoParser(ISaveInfoManager saveInfoManager, IReferenceManager referenceManager)
		{
			_saveInfoManager = saveInfoManager;
			_referenceManager = referenceManager;
			_logger = new CompoundDocumentLogger();
			IsRuntime = false;
			_nodeParsers.Add(new ActionNodeParser());
			_nodeParsers.Add(new UriNodeParser(() => _referenceManager));
			_nodeParsers.Add(new ComplexNodeParser(() => _referenceManager));
			_nodeParsers.Add(new EncodedNodeParser());
			_nodeParsers.Add(new EnumNodeParser());
			_nodeParsers.Add(new ArrayNodeParser());
			_nodeParsers.Add(new CompoundNodeParser(() => _referenceManager));
			_nodeParsers.Add(new SaveInfoNodeParser(ParseNode, DeparseNode));
			_nodeParsers.Add(new ListNodeParser(saveInfoManager, _logger, () => _referenceManager, ParseNode, ParseNode, DeparseNode, new UriNodeParser(() => _referenceManager)));
		}

		public void SetReferenceManager(IReferenceManager referenceManager)
		{
			_referenceManager = referenceManager;
		}

		public void SetLogger(IStorageLogger logger)
		{
			_logger = logger;
		}

		public SaveInfo Parse(string text)
		{
			try
			{
				StorableNode currentNode = ParseValue(text);
				return ParseNode(currentNode);
			}
			catch (Exception ex)
			{
				_logger.Error(new BadStorageFileException
				{
					Content = text,
					Info = ex.Message
				});
				return null;
			}
		}

		public void ResetMetadataManager()
		{
			_metadataManager.Reset();
		}

		public List<MetadataSaveInfo> GetMetadata()
		{
			return _metadataManager.GetSaveInfoMetadata();
		}

		public void LoadMetadata(IList<MetadataSaveInfo> metadata)
		{
			_metadataManager.Reset();
			_metadataManager.AddRange(metadata);
		}

		public string Deparse< T>(T saveInfo) where T : SaveInfo
		{
			try
			{
				StorableNode node = DeparseNode(saveInfo);
				return DeparseValue(node);
			}
			catch (Exception ex)
			{
				_logger.Error(new UnknowStorageException(ex.Message, ex));
				return string.Empty;
			}
		}

		public abstract StorableNode ParseValue(string value);

		public abstract string DeparseValue(StorableNode node);

		public SaveInfo ParseNode(StorableNode currentNode)
		{
			return ParseNode(currentNode, null);
		}

		protected SaveInfo ParseNode(StorableNode currentNode, Type givenPropertyType)
		{
			if (currentNode == null)
			{
				if (Debugger.IsAttached)
				{
					Debugger.Break();
				}
				return null;
			}
			SaveInfo saveInfo;
			if (!TryGetSaveInfo(currentNode, out saveInfo))
			{
				return null;
			}
			Dictionary<string, StorableNode> nameNodes = new Dictionary<string, StorableNode>();
			foreach (StorableNode item in currentNode.Children.Where( (StorableNode childNode) => !nameNodes.ContainsKey(childNode.Name)))
			{
				nameNodes.Add(item.Name, item);
			}
			List<StorableNode> list = nameNodes.Values.ToList();
			foreach (SaveInfoPropertyCache item2 in from x in saveInfo.GetAllMembers()
				select x.GetSaveInfoMemberCache())
			{
				string name = item2.Name;
				StorableNode storableNode;
				if (!nameNodes.TryGetValue(name, out storableNode) && !item2.Aliases.Any( (string alias) => nameNodes.TryGetValue(alias, out storableNode)))
				{
					object defaultValue = item2.DefaultValue;
					if (defaultValue != null)
					{
						try
						{
							defaultValue = Convert.ChangeType(defaultValue, item2.PropertyType);
							item2.SetValue(saveInfo, defaultValue);
						}
						catch (Exception ex)
						{
							_logger.Warning(ex.Message);
						}
					}
					continue;
				}
				list.Remove(storableNode);
				bool flag = false;
				Type propertyType = givenPropertyType ?? item2.PropertyType;
				using (IEnumerator<IStorableNodeParser> enumerator3 = _nodeParsers.Where( (IStorableNodeParser nodeParser) => nodeParser.CanParse(propertyType)).GetEnumerator())
				{
					if (enumerator3.MoveNext())
					{
						IStorableNodeParser current3 = enumerator3.Current;
						flag = true;
						object obj = null;
						try
						{
							obj = current3.Parse(storableNode, propertyType, item2);
							if (obj == null)
							{
								saveInfo.UnknownProperties.Add(storableNode);
								_logger.Warning("解析失败：" + saveInfo.GetType().Name + "." + item2.Name + ", 返回值为空");
							}
							else
							{
								_logger.Info("解析成功：" + saveInfo.GetType().Name + "." + item2.Name);
							}
						}
						catch (UnknownStorageNodeException ex2)
						{
							_logger.Warning(ex2.Message);
						}
						catch (StorageNoteFormatException ex3)
						{
							bool flag2 = false;
							foreach (ISaveInfoMemberConverter converter in GetConverters(item2, _logger))
							{
								try
								{
									obj = converter.Convert(storableNode);
									if (obj != null && item2.PropertyType == obj.GetType())
									{
										flag2 = true;
										break;
									}
								}
								catch (Exception)
								{
									_logger.Warning(ex3.Message);
									throw;
								}
							}
							if (!flag2)
							{
								_logger.Warning(ex3.Message);
							}
						}
						catch (ReferenceMissingException exception)
						{
							_logger.Error(exception);
						}
						catch (Exception ex5)
						{
							_logger.Error(new UnknowStorageException($"解析出错：{saveInfo.GetType().Name}.{item2.Name}，错误为{ex5}", ex5));
							if (Debugger.IsAttached)
							{
								Debugger.Break();
							}
						}
						finally
						{
							if (obj != null)
							{
								if (!IsRuntime)
								{
									if (!item2.IsRuntime)
									{
										item2.SetValue(saveInfo, obj);
									}
									else
									{
										_logger.Info("非 Runtime 工作模式下，不读取 Runtime 的属性 " + item2.Name);
									}
								}
								else
								{
									item2.SetValue(saveInfo, obj);
								}
							}
						}
					}
				}
				if (flag)
				{
					continue;
				}
				try
				{
					bool flag3 = propertyType.IsNullable();
					if (flag3 && string.IsNullOrWhiteSpace(storableNode.StringValue))
					{
						item2.SetValue(saveInfo, null);
						continue;
					}
					Type type = (flag3 ? propertyType.GetGenericArguments()[0] : propertyType);
					object value = (type.IsEnum ? Enum.Parse(type, storableNode.StringValue) : Convert.ChangeType(storableNode.Value, type, CultureInfo.InvariantCulture));
					value = item2.EnsureInRange(value);
					item2.SetValue(saveInfo, value);
				}
				catch (InvalidCastException arg)
				{
					if (!saveInfo.IsFallbackSaveInfo)
					{
						saveInfo.UnknownValues.Add(storableNode);
						_logger.Warning($"格式转换错误：{saveInfo.GetType().Name}.{item2.Name}，错误为{arg}");
						if (Debugger.IsAttached)
						{
							Debugger.Break();
						}
					}
				}
				catch (Exception ex6)
				{
					_logger.Error(new UnknowStorageException($"解析错误：{saveInfo.GetType().Name}.{item2.Name}，错误为{ex6}", ex6));
				}
			}
			if (!saveInfo.IsFallbackSaveInfo)
			{
				foreach (StorableNode item3 in list)
				{
					SaveInfo saveInfo2;
					if (!_saveInfoManager.TryGet(item3.Name, out saveInfo2))
					{
						saveInfo.UnknownProperties.Add(item3);
						_logger.Warning(new UnknownStorageNodeException(item3.Name, item3)
						{
							SaveInfoMetadata = item3.GetAdvancedSaveInfoMetadata(this)
						});
						continue;
					}
					SaveInfo saveInfo3 = ParseNode(item3);
					if (saveInfo3 != null)
					{
						saveInfo.Extensions.Add(saveInfo3);
					}
				}
			}
			currentNode.DeltaSaveTerminalNode = !saveInfo.GetSaveInfoSubPropertyDeltaSaveEnable();
			string saveInfoUniqueIdentifyMemberName = saveInfo.GetSaveInfoUniqueIdentifyMemberName();
			if (!string.IsNullOrWhiteSpace(saveInfoUniqueIdentifyMemberName))
			{
				string childValue = currentNode.GetChildValue(saveInfoUniqueIdentifyMemberName);
				if (!string.IsNullOrWhiteSpace(childValue))
				{
					currentNode.InternalKey = childValue;
				}
			}
			return saveInfo;
		}

		private bool TryGetSaveInfo(StorableNode currentNode, out SaveInfo saveInfo)
		{
			if (!_saveInfoManager.TryGet(currentNode.Name, out saveInfo))
			{
				string text = "未能加载类型为" + currentNode.Name + "的元素,请确保元素已经设置了SaveInfoContract并且已经被加载";
				_logger.Error(new UnknownStorageNodeException(currentNode.Name, currentNode)
				{
					SaveInfoMetadata = currentNode.GetAdvancedSaveInfoMetadata(this)
				});
				MetadataSaveInfo saveInfo2;
				if (_metadataManager.TryGetMetadataSaveInfo(currentNode.Name, out saveInfo2) && !string.IsNullOrWhiteSpace(saveInfo2.FallbackType) && _saveInfoManager.TryGet(saveInfo2.FallbackType, out saveInfo))
				{
					saveInfo.SaveInfoMetadataName = saveInfo2.Name;
					saveInfo.SaveInfoMetadataFriendlyName = saveInfo2.FriendlyName;
					saveInfo.SaveInfoMetadataType = saveInfo2.Type;
					saveInfo.OriginMetadata = saveInfo2;
					saveInfo.IsFallbackSaveInfo = true;
					return true;
				}
				return false;
			}
			return true;
		}

		public StorableNode DeparseNode(SaveInfo saveInfo)
		{
			if (saveInfo.IsFallbackSaveInfo)
			{
				MetadataSaveInfo metadataSaveInfo = saveInfo.OriginMetadata as MetadataSaveInfo;
				if (metadataSaveInfo != null)
				{
					_metadataManager.Add(metadataSaveInfo);
				}
				return null;
			}
			StorableNode storableNode = new StorableNode
			{
				Name = saveInfo.GetSaveInfoContractName()
			};
			_metadataManager.Add(new MetadataSaveInfo(storableNode.Name)
			{
				Type = saveInfo.GetSaveInfoType(),
				FallbackType = saveInfo.GetSaveInfoFallbackType(),
				FriendlyName = saveInfo.GetSaveInfoFriendlyName()
			});
			PropertyInfo[] members = saveInfo.GetMembers(IsRuntime);
			foreach (PropertyInfo propertyInfo in members)
			{
				string saveInfoMemberName = propertyInfo.GetSaveInfoMemberName();
				object propertyValue = propertyInfo.GetValue(saveInfo, BindingFlags.Default, null, null, CultureInfo.InvariantCulture);
				if (propertyValue == null || !((!FallbackArrayExtension.IsEmptyArray(propertyValue)) ?? true))
				{
					continue;
				}
				bool flag = false;
				using (IEnumerator<IStorableNodeParser> enumerator = _nodeParsers.Where( (IStorableNodeParser nodeParser) => nodeParser.CanDeparse(propertyValue)).GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						IStorableNodeParser current = enumerator.Current;
						flag = true;
						try
						{
							current.Deparse(saveInfoMemberName, propertyValue, storableNode, propertyInfo);
							_logger.Info("解析成功：" + saveInfo.GetType().Name + "." + propertyInfo.Name);
						}
						catch (UnknownStorageNodeException ex)
						{
							_logger.Warning(ex.Message);
						}
						catch (StorageNoteFormatException ex2)
						{
							_logger.Warning(ex2.Message);
						}
						catch (ReferenceMissingException exception)
						{
							_logger.Error(exception);
						}
						catch (Exception arg)
						{
							_logger.Warning($"解析出错：{saveInfo.GetType().Name}.{propertyInfo.Name}，错误为{arg}");
							if (Debugger.IsAttached)
							{
								Debugger.Break();
							}
						}
					}
				}
				if (!flag)
				{
					Type type = (propertyInfo.PropertyType.IsNullable() ? propertyInfo.PropertyType.GetGenericArguments()[0] : propertyInfo.PropertyType);
					StorableNode node = new StorableNode(saveInfoMemberName)
					{
						Value = (type.IsEnum ? propertyValue.ToString() : propertyValue)
					};
					storableNode.AddChild(node);
				}
			}
			foreach (SaveInfo extension in saveInfo.Extensions)
			{
				if (extension == null)
				{
					continue;
				}
				StorableNode storableNode2 = DeparseNode(extension);
				if (storableNode2 != null)
				{
					string text = extension.GetType().GetCustomAttributes<SaveInfoExtensionAttribute>().FirstOrDefault( (SaveInfoExtensionAttribute attr) => attr.TargetType.IsInstanceOfType(saveInfo))?.Name;
					if (!string.IsNullOrEmpty(text))
					{
						storableNode2.Name = text;
					}
					storableNode.AddChild(storableNode2);
				}
			}
			storableNode.AppendUnknownNode(saveInfo, this, _referenceManager);
			storableNode.DeltaSaveTerminalNode = !saveInfo.GetSaveInfoSubPropertyDeltaSaveEnable();
			string saveInfoUniqueIdentifyMemberName = saveInfo.GetSaveInfoUniqueIdentifyMemberName();
			if (!string.IsNullOrWhiteSpace(saveInfoUniqueIdentifyMemberName))
			{
				string childValue = storableNode.GetChildValue(saveInfoUniqueIdentifyMemberName);
				if (!string.IsNullOrWhiteSpace(childValue))
				{
					storableNode.InternalKey = childValue;
				}
			}
			return storableNode;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static IEnumerable<ISaveInfoMemberConverter> GetConverters(SaveInfoPropertyCache saveInfoMember, IStorageLogger logger)
		{
			try
			{
				return saveInfoMember.GetConverters();
			}
			catch (AggregateException ex)
			{
				foreach (Exception innerException in ex.InnerExceptions)
				{
					logger.Warning(innerException.GetType().FullName + ", " + innerException.Message);
				}
			}
			catch (Exception ex2)
			{
				logger.Warning(ex2.GetType().FullName + ", " + ex2.Message);
			}
			return new ISaveInfoMemberConverter[0];
		}
	}
}
