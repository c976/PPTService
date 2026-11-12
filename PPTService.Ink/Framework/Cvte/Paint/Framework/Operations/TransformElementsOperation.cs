using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Cvte.Diagnostics.Annotations;
using Cvte.Documents;
using Cvte.Linq;
using Cvte.Paint.Framework.Elements;

namespace Cvte.Paint.Framework.Operations
{
	public class TransformElementsOperation : ElementOperation, IOperation
	{
		internal class ElementData
		{
			public string ElementId { get; }

			public Rect Bounds { get; }

			public double Rotation { get; }

			public Matrix Transform { get; }

			public Point LogicalOriginPoint { get; }

			public ReadOnlyCollection<ElementData> GroupSubElements { get; private set; }

			public ElementData(string elementId, Rect bounds, double rotation, Matrix transform, Point logicalOriginPoint, List<ElementData> subElementDatas = null)
			{
				ElementId = elementId;
				Bounds = bounds;
				Rotation = rotation;
				Transform = transform;
				LogicalOriginPoint = logicalOriginPoint;
				if (subElementDatas == null)
				{
					subElementDatas = new List<ElementData>();
				}
				GroupSubElements = new ReadOnlyCollection<ElementData>(subElementDatas);
			}
		}

		internal class ElementTransformData
		{
			public string ElementId { get; }

			public ElementData Source { get; }

			public ElementData Target { get; }

			public ElementTransformData(string elementId, Rect sourceBounds, Rect targetBounds, double sourceRotation, double targetRotation, Matrix sourceTransform, Matrix targetTransform, Point logicalOriginPoint)
			{
				ElementId = elementId;
				Source = new ElementData(elementId, sourceBounds, sourceRotation, sourceTransform, logicalOriginPoint);
				Target = new ElementData(elementId, targetBounds, targetRotation, targetTransform, logicalOriginPoint);
			}

			public ElementTransformData(string elementId, ElementData source, ElementData target)
			{
				ElementId = elementId;
				Source = source;
				Target = target;
			}
		}

		public class Builder
		{
			private readonly Slide _slide;

			private readonly List<Element> _elements = new List<Element>();

			private readonly List<ElementData> _sourceDataList = new List<ElementData>();

			private readonly List<ElementData> _targetDataList = new List<ElementData>();

			public bool AnyElementsChanged => _elements.Any();

			public Builder(Slide slide)
			{
				if (slide == null)
				{
					throw new ArgumentNullException("slide");
				}
				_slide = slide;
			}

			public void RecordSource([NotNull] IEnumerable<Element> elements)
			{
				if (elements == null)
				{
					throw new ArgumentNullException("elements");
				}
				if (_elements.Any())
				{
					throw new InvalidOperationException("不能重复记录 Bounds，必须取消或结束之前的记录。");
				}
				AppendRecordingSource(elements);
			}

			public void AppendRecordingSource([NotNull] IEnumerable<Element> elements)
			{
				if (elements == null)
				{
					throw new ArgumentNullException("elements");
				}
				elements = (elements as ICollection<Element>) ?? elements.ToArray();
				_elements.AddRange(elements);
				_sourceDataList.AddRange(elements.Select((Element x) => GetElementData(x)));
			}

			public void IgnoreRecordingSource([NotNull] IEnumerable<Element> elements)
			{
				if (elements == null)
				{
					throw new ArgumentNullException("elements");
				}
				foreach (Element item in elements.Where(_elements.Contains))
				{
					int index = _elements.IndexOf(item);
					_elements.RemoveAt(index);
					_sourceDataList.RemoveAt(index);
				}
			}

			public TransformElementsOperation RecordTarget()
			{
				List<ElementTransformData> list = new List<ElementTransformData>(_elements.Count);
				for (int i = 0; i < _elements.Count; i++)
				{
					ElementTransformData elementTransformData = GetElementTransformData(_elements[i], _sourceDataList[i]);
					_targetDataList.Add(elementTransformData.Target);
					list.Add(elementTransformData);
				}
				TransformElementsOperation result = new TransformElementsOperation(_slide, list);
				Reset();
				return result;
			}

			private ElementTransformData GetElementTransformData(Element element, ElementData sourceData)
			{
				ElementData elementData = GetElementData(element);
				return new ElementTransformData(element.Id, sourceData, elementData);
			}

			private ElementData GetElementData(Element element)
			{
				IGroupElement groupElement = element as IGroupElement;
				if (groupElement == null)
				{
					return new ElementData(element.Id, element.Bounds, element.Rotation, element.Transform, element.LogicalOriginPoint);
				}
				List<ElementData> list = new List<ElementData>();
				foreach (Element element2 in groupElement.Elements)
				{
					list.Add(GetElementData(element2));
				}
				return new ElementData(element.Id, element.Bounds, element.Rotation, element.Transform, element.LogicalOriginPoint, list);
			}

			public void CancelRecording()
			{
				Reset();
			}

			public void CancelAndRestore()
			{
				_elements.ForEach((Element x, int i) =>
				{
					CancelAndRestore(x, _sourceDataList[i]);
				});
				Reset();
			}

			private void ElementDataFillToElement(Element element, ElementData data)
			{
				element.Bounds = data.Bounds;
				element.Rotation = data.Rotation;
				element.Transform = data.Transform;
			}

			private void CancelAndRestore(Element element, ElementData sourceData)
			{
				IGroupElement groupElement = element as IGroupElement;
				if (groupElement == null)
				{
					ElementDataFillToElement(element, sourceData);
					return;
				}
				ElementDataFillToElement(element, sourceData);
				for (int i = 0; i < groupElement.Elements.Count; i++)
				{
					List<Element> list = groupElement.Elements.ToList();
					CancelAndRestore(list[i], sourceData.GroupSubElements[i]);
				}
			}

			private void Reset()
			{
				_elements.Clear();
				_sourceDataList.Clear();
				_targetDataList.Clear();
			}

			internal ElementData GetSourceTransformData(Element element)
			{
				int num = _elements.IndexOf(element);
				if (num < 0)
				{
					throw new ArgumentException("未找到查询原始变换数据的 Element 实例，以至于无法生成其初始变换量。");
				}
				return _sourceDataList[num];
			}
		}

		private readonly ElementTransformData[] _elementsData;

		internal TransformElementsOperation([NotNull] Slide slide, [NotNull] IEnumerable<ElementTransformData> elementsData)
			: base(slide)
		{
			if (elementsData == null)
			{
				throw new ArgumentNullException("elementsData");
			}
			_elementsData = (elementsData as ElementTransformData[]) ?? elementsData.ToArray();
		}

		private void UndoOneElement(Element element, ElementData data)
		{
			if (element == null)
			{
				return;
			}
			Action<Element, ElementData> action = (Element e, ElementData d) =>
			{
				e.Bounds = d.Bounds;
				e.Rotation = d.Rotation;
				e.Transform = d.Transform;
				e.LogicalOriginPoint = d.LogicalOriginPoint;
			};
			GroupElementBase groupElementBase = element as GroupElementBase;
			if (groupElementBase == null)
			{
				action(element, data);
				return;
			}
			action(element, data);
			groupElementBase.Elements.ForEach((Element e) =>
			{
				if (e.IsGroupInnerSelected())
				{
					e.SetIsGroupInnerSelected(false);
					e.SetIsGroupInnerSelected(true);
				}
			});
			foreach (Element innerElement in groupElementBase.Elements)
			{
				ElementData elementData = data.GroupSubElements.FirstOrDefault((ElementData subElement) => subElement.ElementId == innerElement.Id);
				if (elementData == null)
				{
					throw new InvalidOperationException("撤销重做栈中找不到Id为" + innerElement.Id + "的元素");
				}
				UndoOneElement(innerElement, elementData);
			}
		}

		protected override void OnRedo()
		{
			Slide slide = GetSlide();
			ElementTransformData[] elementsData = _elementsData;
			foreach (ElementTransformData elementTransformData in elementsData)
			{
				Element elementById = slide.GetElementById(elementTransformData.ElementId);
				_003COnRedo_003Eg__action_007C3_0(elementById, elementTransformData.Target);
				GroupElementBase groupElementBase = elementById as GroupElementBase;
				if (groupElementBase == null)
				{
					continue;
				}
				foreach (Element innerElement in groupElementBase.Elements)
				{
					ElementData elementData = elementTransformData.Target.GroupSubElements.FirstOrDefault((ElementData subElement) => subElement.ElementId == innerElement.Id);
					if (elementData == null)
					{
						throw new InvalidOperationException("撤销重做栈中找不到Id为" + innerElement.Id + "的元素");
					}
					_003COnRedo_003Eg__action_007C3_0(innerElement, elementData);
				}
			}
			slide.RefreshSelectionBoxes();
		}

		protected override void OnUndo()
		{
			Slide slide = GetSlide();
			List<ElementTransformData> list = new List<ElementTransformData>();
			ElementTransformData[] elementsData = _elementsData;
			foreach (ElementTransformData elementTransformData in elementsData)
			{
				Element elementById = slide.GetElementById(elementTransformData.ElementId);
				if (elementById == null)
				{
					list.Add(elementTransformData);
				}
				else
				{
					UndoOneElement(elementById, elementTransformData.Source);
				}
			}
			slide.RefreshSelectionBoxes();
			if (list.Any())
			{
				bool flag = list.Count > 1;
				string text = (flag ? "Some" : "An");
				string text2 = (flag ? "s" : "");
				string text3 = (flag ? "are" : "is");
				throw new InvalidOperationException(text + " element" + text2 + " " + text3 + " lost while Undo executing. " + $"Total count: {_elementsData.Length}, Lost count: {list.Count}.");
			}
		}

		[CompilerGenerated]
		internal static void _003COnRedo_003Eg__action_007C3_0(Element e, ElementData d)
		{
			e.Bounds = d.Bounds;
			e.Rotation = d.Rotation;
			e.LogicalOriginPoint = d.LogicalOriginPoint;
			if (!object.Equals(e.Transform, d.Transform))
			{
				e.Transform = d.Transform;
			}
		}
	}
}
