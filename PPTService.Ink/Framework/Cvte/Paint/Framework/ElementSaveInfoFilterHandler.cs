using System;
using System.Collections.Generic;
using Cvte.Linq;

namespace Cvte.Paint.Framework
{
	internal static class ElementSaveInfoFilterHandler
	{
		public static void OnBeforeGetElementSaveInfo(Element element)
		{
			element.Extensions.ForEach((IExtension<Element> extension) =>
			{
				IElementSaveInfoFilter elementSaveInfoFilter = extension as IElementSaveInfoFilter;
				if (elementSaveInfoFilter != null)
				{
					try
					{
						elementSaveInfoFilter.BeforeGetElementSaveInfo(element);
					}
					catch (Exception innerException)
					{
						throw new SaveInfoException($"调用元素 [{element}] 的扩展 [{extension}] 的 BeforeGetElementSaveInfo 时出错", innerException, extension.GetType());
					}
				}
			});
		}

		public static void OnAfterGetElementSaveInfo(Element element, ElementSaveInfo elementSaveInfo)
		{
			element.Extensions.ForEach((IExtension<Element> extension) =>
			{
				IElementSaveInfoFilter elementSaveInfoFilter = extension as IElementSaveInfoFilter;
				if (elementSaveInfoFilter != null)
				{
					try
					{
						elementSaveInfoFilter.AfterGetElementSaveInfo(element, ref elementSaveInfo);
					}
					catch (Exception innerException)
					{
						throw new SaveInfoException($"调用元素 [{element}] 的扩展 [{extension}] 的 AfterGetElementSaveInfo 时出错", innerException, extension.GetType());
					}
				}
			});
		}

		public static void OnBeforeSetElementSaveInfo(Element element, IList<IExtension<Element>> extensions, ElementSaveInfo elementSaveInfo)
		{
			extensions.ForEach((IExtension<Element> extension) =>
			{
				IElementSaveInfoFilter elementSaveInfoFilter = extension as IElementSaveInfoFilter;
				if (elementSaveInfoFilter != null)
				{
					try
					{
						elementSaveInfoFilter.BeforeSetElementSaveInfo(element, ref elementSaveInfo);
					}
					catch (Exception innerException)
					{
						throw new SaveInfoException($"调用元素 [{element}] 的扩展 [{extension}] 的 BeforeSetElementSaveInfo 时出错", innerException, extension.GetType());
					}
				}
			});
		}

		public static void OnAfterSetElementSaveInfo(Element element, ElementSaveInfo elementSaveInfo)
		{
			element.Extensions.ForEach((IExtension<Element> extension) =>
			{
				IElementSaveInfoFilter elementSaveInfoFilter = extension as IElementSaveInfoFilter;
				if (elementSaveInfoFilter != null)
				{
					try
					{
						elementSaveInfoFilter.AfterSetElementSaveInfo(element, elementSaveInfo);
					}
					catch (Exception innerException)
					{
						throw new SaveInfoException($"调用元素 [{element}] 的扩展 [{extension}] 的 AfterSetElementSaveInfo 时出错", innerException, extension.GetType());
					}
				}
			});
		}
	}
}
