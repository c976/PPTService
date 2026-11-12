using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cvte.ComponentModel;
using Cvte.Diagnostics.Annotations;
using Cvte.Diagnostics.Trace;
using Cvte.IO;
using Cvte.Linq;
using Cvte.Storage;

namespace Cvte.Paint.Framework
{
	public static class SaveInfoExtension
	{
		[CanBeNull]
		public static Element ToElement(this ElementSaveInfo saveInfo)
		{
			if (saveInfo == null)
			{
				return null;
			}
			ISaveInfoProvider interfaceInstance = InterfaceCreator.GetInterfaceInstance<ISaveInfoProvider>(new object[0]);
			Element value;
			if (!interfaceInstance.TryCreate<Element, ElementSaveInfo>(saveInfo, out value))
			{
				try
				{
					throw new ArgumentException($"无法从 {saveInfo.GetType()} 创建对应的元素");
				}
				catch (Exception arg)
				{
					if (DebuggingProperties.IsDebug)
					{
						throw;
					}
					Tracer.Warning($"SaveInfoExtension:{arg}", "Paint", "SaveInfoExtension");
				}
				return null;
			}
			try
			{
				IDynamicSavableElement dynamicSavableElement = value as IDynamicSavableElement;
				if (dynamicSavableElement != null)
				{
					dynamicSavableElement.SetSaveInfo(saveInfo);
					return value;
				}
				ICreateExtensionsSaveInfoProvider createExtensionsSaveInfoProvider = interfaceInstance as ICreateExtensionsSaveInfoProvider;
				if (createExtensionsSaveInfoProvider != null)
				{
					IList<IExtension<Element>> list = createExtensionsSaveInfoProvider.CreateExtensions<Element>(saveInfo);
					foreach (IExtension<Element> item in list)
					{
						value.Extensions.AddWithoutAttached(item);
					}
					ElementSaveInfoFilterHandler.OnBeforeSetElementSaveInfo(value, list, saveInfo);
					SetElementSaveInfoWithoutExtension(value, saveInfo);
					foreach (IExtension<Element> item2 in list)
					{
						interfaceInstance.SetSaveInfo(item2, saveInfo);
						item2.Attached(value);
					}
				}
				else
				{
					ElementSaveInfoFilterHandler.OnBeforeSetElementSaveInfo(value, value.Extensions, saveInfo);
					SetElementSaveInfoWithoutExtension(value, saveInfo);
					InterfaceCreator.GetInterfaceInstance<ISaveInfoProvider>(new object[0]).CreateExtensions(saveInfo, value.Extensions);
				}
			}
			catch (FatalSaveInfoException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				if (Debugger.IsAttached)
				{
					Debugger.Break();
				}
				Log.Error("Error when ToElement", ex2);
				SaveInfoExceptionPackage.CurrentExceptionPackage.Add(ex2);
				return null;
			}
			ElementSaveInfoFilterHandler.OnAfterSetElementSaveInfo(value, saveInfo);
			return value;
		}

		private static void SetElementSaveInfoWithoutExtension(Element element, ElementSaveInfo saveInfo)
		{
			SetSaveInfoInner(saveInfo, element);
			IElementSaveInfo elementSaveInfo = element as IElementSaveInfo;
			if (elementSaveInfo != null)
			{
				elementSaveInfo.ElementSaveInfoProvider.SetSaveInfo(saveInfo);
			}
			else
			{
				InterfaceCreator.GetInterfaceInstance<ISaveInfoProvider>(new object[0]).SetSaveInfo(element, saveInfo);
			}
		}

		private static void SetSaveInfoInner(ElementSaveInfo saveInfo, Element element)
		{
			CommonElementSaveInfoProvider.SetCommonElementSaveInfo(saveInfo, element);
		}

		[CanBeNull]
		public static ElementSaveInfo GetElementSaveInfo(this Element element)
		{
			IDynamicSavableElement dynamicSavableElement = element as IDynamicSavableElement;
			if (dynamicSavableElement != null)
			{
				return dynamicSavableElement.GetSaveInfo();
			}
			ElementSaveInfoFilterHandler.OnBeforeGetElementSaveInfo(element);
			IElementSaveInfo elementSaveInfo = element as IElementSaveInfo;
			ElementSaveInfo elementSaveInfo2;
			if (elementSaveInfo != null)
			{
				elementSaveInfo2 = elementSaveInfo.ElementSaveInfoProvider.GetSaveInfo();
			}
			else if (!InterfaceCreator.GetInterfaceInstance<ISaveInfoProvider>(new object[0]).TryGetSaveInfo<Element, ElementSaveInfo>(element, out elementSaveInfo2) || elementSaveInfo2 == null)
			{
				return null;
			}
			try
			{
				CommonElementSaveInfoProvider.GetCommonElementSaveInfo(elementSaveInfo2, element);
			}
			catch (Exception innerException)
			{
				throw new SaveInfoException($"设置元素[{element}]通用属性时出错", innerException, element.GetType());
			}
			element.Extensions.ForEach((IExtension<Element> extension) =>
			{
				IExtensionSavable<ElementSaveInfo> extensionSavable = extension as IExtensionSavable<ElementSaveInfo>;
				try
				{
					extensionSavable?.GetSaveInfo(ref elementSaveInfo2);
				}
				catch (Exception innerException2)
				{
					throw new SaveInfoException($"获取元素[{element}]的扩展[{extension}]的SaveInfo时出错", innerException2, extension.GetType());
				}
			});
			ElementSaveInfoFilterHandler.OnAfterGetElementSaveInfo(element, elementSaveInfo2);
			return elementSaveInfo2;
		}

		public static void SetElementSaveInfo(this Element element, ElementSaveInfo saveInfo)
		{
			if (saveInfo == null)
			{
				return;
			}
			IDynamicSavableElement dynamicSavableElement = element as IDynamicSavableElement;
			if (dynamicSavableElement != null)
			{
				dynamicSavableElement.SetSaveInfo(saveInfo);
				return;
			}
			ElementSaveInfoFilterHandler.OnBeforeSetElementSaveInfo(element, element.Extensions, saveInfo);
			SetSaveInfoInner(saveInfo, element);
			IElementSaveInfo elementSaveInfo = element as IElementSaveInfo;
			if (elementSaveInfo != null)
			{
				elementSaveInfo.ElementSaveInfoProvider.SetSaveInfo(saveInfo);
			}
			else
			{
				InterfaceCreator.GetInterfaceInstance<ISaveInfoProvider>(new object[0]).SetSaveInfo(element, saveInfo);
			}
			InterfaceCreator.GetInterfaceInstance<ISaveInfoProvider>(new object[0]).SetExtensions(saveInfo, element.Extensions);
			foreach (IExtension<Element> extension in element.Extensions)
			{
				extension.Attached(element);
			}
			ElementSaveInfoFilterHandler.OnAfterSetElementSaveInfo(element, saveInfo);
		}

		public static T GetExtension<T>(this SaveInfo saveInfo) where T : SaveInfo
		{
			return saveInfo.Extensions.OfType<T>().FirstOrDefault();
		}
	}
}
