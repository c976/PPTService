using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Cvte.ComponentModel;
using Cvte.Diagnostics.Annotations;
using Cvte.Diagnostics.Trace;
using Cvte.Paint.Framework.Level;

namespace Cvte.Paint.Framework
{
	internal class LevelSortedElementCollection : IList<Element>, ICollection<Element>, IEnumerable<Element>, IEnumerable
	{
		private List<Element> _layerSortedElementList = new List<Element>();

		private readonly List<Element> _elements = new List<Element>();

		private ReadOnlyCollection<Element> _elementCollection;

		private Slide Slide { get; }

		public int Count => _layerSortedElementList.Count;

		public bool IsReadOnly => false;

		Element IList<Element>.this[int index]
		{
			get
			{
				return _layerSortedElementList[index];
			}
			set
			{
				throw new NotSupportedException("有序集合无法设置指定序号处的元素。");
			}
		}

		public ReadOnlyCollection<Element> Origin
		{
			get
			{
				if (_elementCollection == null)
				{
					_elementCollection = new ReadOnlyCollection<Element>(_elements);
				}
				return _elementCollection;
			}
		}

		public LevelSortedElementCollection([NotNull] Slide slide)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			Slide = slide;
		}

		public void Add(Element item)
		{
			if (item == null)
			{
				if (DebuggingProperties.IsDebug)
				{
					try
					{
						throw new ArgumentException("添加按层级排序的元素列表时，传入元素为空 LevelSortedElementCollection.Add(IEnumerable<Element>.element = null) 仅调试抛出", "item");
					}
					catch (Exception exception)
					{
						Tracer.Warning(exception, "Paint");
						return;
					}
				}
				return;
			}
			_elements.Add(item);
			Slide slide = Slide.GetSlide(item);
			if (slide == null)
			{
				FixCannotFindSlideFromElement(out slide, item);
			}
			if (slide != Slide)
			{
				try
				{
					throw new ArgumentException("传入的元素不在对应页面上，元素所在页面=" + slide.Id + " 期望加入页面=" + Slide.Id + " 这个异常只有调试下抛出");
				}
				catch (ArgumentException exception2)
				{
					if (DebuggingProperties.IsDebug)
					{
						throw;
					}
					Tracer.Error(exception2);
				}
			}
			_layerSortedElementList.Add(item);
			_layerSortedElementList = slide.GetLevelSortedElements(_layerSortedElementList);
		}

		private void FixCannotFindSlideFromElement(out Slide slide, Element element)
		{
			slide = Slide;
			if (DebuggingProperties.IsDebug)
			{
				if (slide.Elements.Contains(element))
				{
					throw new ArgumentException("添加选择的元素在页面里面找到，但从附加属性没有找到对应的页面，请检查日志是否存在模式没有正确关闭，这个异常只有调试下抛出");
				}
				throw new ArgumentException("添加选择的元素没有在页面里面找到，请检查是否给元素添加选择时元素已经加入到页面，这个异常只有调试下抛出");
			}
		}

		public bool Remove(Element item)
		{
			_elements.Remove(item);
			return _layerSortedElementList.Remove(item);
		}

		public void Clear()
		{
			_elements.Clear();
			_layerSortedElementList.Clear();
		}

		public bool Contains(Element item)
		{
			if (item == null)
			{
				return false;
			}
			return _layerSortedElementList.Contains(item);
		}

		void ICollection<Element>.CopyTo(Element[] array, int arrayIndex)
		{
			_layerSortedElementList.ToArray().CopyTo(array, arrayIndex);
		}

		public IEnumerator<Element> GetEnumerator()
		{
			return _layerSortedElementList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public int IndexOf(Element item)
		{
			if (item == null)
			{
				return -1;
			}
			return _layerSortedElementList.IndexOf(item);
		}

		public void Insert(int index, Element item)
		{
			throw new NotSupportedException("有序集合无法在指定序号处插入元素。");
		}

		public void RemoveAt(int index)
		{
			if (index < _layerSortedElementList.Count)
			{
				Element item = _layerSortedElementList[index];
				_elements.Remove(item);
				_layerSortedElementList.RemoveAt(index);
			}
		}

		public void AddRange(IEnumerable<Element> collection)
		{
			foreach (Element item in collection)
			{
				if (item == null)
				{
					if (DebuggingProperties.IsDebug)
					{
						try
						{
							throw new ArgumentException("添加按层级排序的元素列表时，传入元素为空 LevelSortedElementCollection.AddRange(IEnumerable<Element>.element = null) 仅调试抛出", "collection");
						}
						catch (Exception exception)
						{
							Tracer.Warning(exception, "Paint");
						}
					}
				}
				else
				{
					_elements.Add(item);
				}
			}
			if (!_elements.Any())
			{
				return;
			}
			Slide slide = Slide;
			if (_elements.Count < 10)
			{
				for (int i = 0; i < _elements.Count; i++)
				{
					Element element = _elements[i];
					if (!slide.Contains(element))
					{
						_elements.RemoveAt(i);
						i--;
					}
				}
			}
			_layerSortedElementList = slide.GetLevelSortedElements(_elements);
		}

		internal int RemoveAll(Predicate<Element> match)
		{
			_elements.RemoveAll(match);
			return _layerSortedElementList.RemoveAll(match);
		}
	}
}
