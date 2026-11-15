using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using PPTService.Ink.Core.ComponentModel;
using Newtonsoft.Json;

namespace PPTService.Ink.Core.Storage
{
	
	
	[JsonObject]
	public abstract class SaveInfo
	{
		private IList<SaveInfo> _extensions = new List<SaveInfo>();

		public IList<StorableNode> UnknownProperties { get; } = new List<StorableNode>();

		public IList<StorableNode> UnknownValues { get; } = new List<StorableNode>();

		public string SaveInfoMetadataName { get; internal set; }

		public string SaveInfoMetadataFriendlyName { get; internal set; }

		public SaveInfoType SaveInfoMetadataType { get; internal set; }

		public IList<SaveInfo> Extensions
		{
			get
			{
				if (DebuggingProperties.IsDebug && !IsFallbackSaveInfo)
				{
					VerifyExtensionAttribute();
				}
				return _extensions;
			}
			set
			{
				_extensions = value ?? new List<SaveInfo>();
			}
		}

		internal bool IsFallbackSaveInfo { get; set; }

		internal SaveInfo OriginMetadata { get; set; }

		private void VerifyExtensionAttribute()
		{
			foreach (SaveInfo extension in _extensions)
			{
				List<SaveInfoExtensionAttribute> list = extension.GetType().GetCustomAttributes<SaveInfoExtensionAttribute>().ToList();
				if (list.Count == 0)
				{
					throw new InvalidOperationException(extension.GetType().FullName + " 作为 " + GetType().Name + " 的扩展在使用，但没有为 " + extension.GetType().Name + " 标记 " + typeof(SaveInfoExtensionAttribute).Name + "。");
				}
				Type type = GetType();
				if (list.All( (SaveInfoExtensionAttribute att) => att.TargetType != type && !att.TargetType.IsAssignableFrom(type)))
				{
					throw new InvalidOperationException(extension.GetType().FullName + " 作为 " + GetType().Name + " 的扩展在使用，但没有为 " + extension.GetType().Name + " 标记 " + typeof(SaveInfoExtensionAttribute).Name + "。");
				}
			}
		}
	}
}
