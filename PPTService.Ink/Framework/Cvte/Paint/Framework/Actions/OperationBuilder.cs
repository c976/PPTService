using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Cvte.ComponentModel;
using Cvte.Diagnostics.Annotations;
using Cvte.Documents;
using Cvte.Paint.Framework.Operations;

namespace Cvte.Paint.Framework.Actions
{
	public static class OperationBuilder
	{
		private class ChangeElementsActionOperation : ElementOperation, IOperation
		{
			[NotNull]
			private readonly Slide _slide;

			private readonly bool _isReselectNeeded;

			private readonly Dictionary<string, ValueTuple<ElementSaveInfo, ElementSaveInfo>> _elementDeltaDictionary;

			internal ChangeElementsActionOperation([NotNull] Slide slide, Dictionary<string, ValueTuple<ElementSaveInfo, ElementSaveInfo>> deltaDictionary)
				: base(slide)
			{
				_slide = slide;
				_elementDeltaDictionary = deltaDictionary;
				_isReselectNeeded = _elementDeltaDictionary.Any((KeyValuePair<string, ValueTuple<ElementSaveInfo, ElementSaveInfo>> x) => x.Value.Item1.IsLocked != x.Value.Item2.IsLocked);
			}

			protected override void OnRedo()
			{
				Update((ValueTuple<ElementSaveInfo, ElementSaveInfo> tuple) => tuple.Item2);
			}

			protected override void OnUndo()
			{
				Update((ValueTuple<ElementSaveInfo, ElementSaveInfo> tuple) => tuple.Item1);
			}

			private void Update(Func<ValueTuple<ElementSaveInfo, ElementSaveInfo>, ElementSaveInfo> getDataToApply)
			{
				List<Element> list = new List<Element>();
				foreach (KeyValuePair<string, ValueTuple<ElementSaveInfo, ElementSaveInfo>> item in _elementDeltaDictionary)
				{
					Element elementById = _slide.GetElementById(item.Key);
					if (DebuggingProperties.IsDebug && elementById == null)
					{
						throw new ArgumentNullException("指定的id没有在Slide找到元素")
						{
							Data = 
							{
								{
									(object)"slide",
									(object)_slide
								},
								{
									(object)"id",
									(object)item.Key
								}
							}
						};
					}
					list.Add(elementById);
					elementById?.Extensions.OfType<ElementActionExtension>().FirstOrDefault()?.SetSaveInfo(getDataToApply(item.Value));
				}
				if (_isReselectNeeded && list.All((Element x) => x.GetIsSelected()))
				{
					_slide.Reselect(list);
				}
			}
		}

		public static IOperation DoOperation([NotNull] Slide slide, [NotNull] IList<Element> elements, Action<Element> action)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			if (elements == null)
			{
				throw new ArgumentNullException("elements");
			}
			Dictionary<string, ValueTuple<ElementSaveInfo, ElementSaveInfo>> dictionary = new Dictionary<string, ValueTuple<ElementSaveInfo, ElementSaveInfo>>();
			foreach (Element element in elements)
			{
				ElementSaveInfo info = new ElementSaveInfo();
				element.Extensions.OfType<ElementActionExtension>().FirstOrDefault()?.GetSaveInfo(ref info);
				action?.Invoke(element);
				ElementSaveInfo info2 = new ElementSaveInfo();
				element.Extensions.OfType<ElementActionExtension>().FirstOrDefault()?.GetSaveInfo(ref info2);
				dictionary.Add(element.Id, new ValueTuple<ElementSaveInfo, ElementSaveInfo>(info, info2));
			}
			slide.Reselect(elements);
			IOperation operation = new ChangeElementsActionOperation(slide, dictionary);
			Board.GetUndoRedoProvider(slide).Insert(operation);
			return operation;
		}
	}
}
