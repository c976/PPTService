using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using Cvte.Diagnostics.Annotations;
using Cvte.Linq;

namespace Cvte.Paint.Framework.Level
{
	public class LevelStorage : ILevelStorage, ILevelManager, IComparer<ILevelElement>
	{
		private class LevelArray : IReadOnlyList<LevelInfo>, IReadOnlyCollection<LevelInfo>, IEnumerable<LevelInfo>, IEnumerable
		{
			public LevelInfo this[int index]
			{
				get
				{
					return ((IList<LevelInfo>)LevelList)[index];
				}
				set
				{
					((IList<LevelInfo>)LevelList)[index] = value;
				}
			}

			public int Count => ((ICollection<LevelInfo>)LevelList).Count;

			private List<LevelInfo> LevelList { get; } = new List<LevelInfo>();

			public void Add(LevelInfo item)
			{
				((ICollection<LevelInfo>)LevelList).Add(item);
			}

			public IEnumerator<LevelInfo> GetEnumerator()
			{
				return ((IEnumerable<LevelInfo>)LevelList).GetEnumerator();
			}

			public int IndexOf(LevelInfo item)
			{
				return ((IList<LevelInfo>)LevelList).IndexOf(item);
			}

			public void Insert(int index, LevelInfo item)
			{
				((IList<LevelInfo>)LevelList).Insert(index, item);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return ((IEnumerable<LevelInfo>)LevelList).GetEnumerator();
			}
		}

		private class LevelElementConnect
		{
			public LevelInfo this[ILevelElement element]
			{
				get
				{
					return LevelElementDictionary[element.Id];
				}
				set
				{
					LevelElementDictionary[element.Id] = value;
				}
			}

			public LevelInfo this[string element]
			{
				get
				{
					return LevelElementDictionary[element];
				}
				set
				{
					LevelElementDictionary[element] = value;
				}
			}

			private Dictionary<string, LevelInfo> LevelElementDictionary { get; } = new Dictionary<string, LevelInfo>();

			public bool ContainsKey(ILevelElement element)
			{
				return LevelElementDictionary.ContainsKey(element.Id);
			}
		}

		private class LevelErratic : ILevelErratic
		{
			private LevelStorage LevelStorage { get; }

			public LevelErratic(LevelStorage levelStorage)
			{
				LevelStorage = levelStorage;
			}

			public void MoveTo(string element, LevelInfo level)
			{
				LevelStorage.LevelElementDictionary[element] = level;
			}

			public void EndMove()
			{
				LevelStorage.ApplyLevelToAllElement();
			}
		}

		[StructLayout(LayoutKind.Auto)]
		[CompilerGenerated]
		private struct _003C_003Ec__DisplayClass8_0
		{
			public LevelStorage _003C_003E4__this;

			public ILevelElement element;
		}

		private LevelErratic _levelErratic;

		public ILevelContainer LevelContainer { get; }

		public ILevelOperator LevelOperator { get; set; } = new WPFLevelOperator();

		private LevelArray LevelList { get; } = new LevelArray();

		private LevelElementConnect LevelElementDictionary { get; } = new LevelElementConnect();

		public LevelStorage([NotNull] ILevelContainer levelContainer)
		{
			if (levelContainer == null)
			{
				throw new ArgumentNullException("levelContainer");
			}
			LevelContainer = levelContainer;
		}

		private void ThrowNotExistsLevelNodeException(string levelId)
		{
			throw new InvalidOperationException("未能找到被插入的目标层级:" + levelId);
		}

		private void VerifyElement(ILevelElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			if (!LevelElementDictionary.ContainsKey(element))
			{
				throw new ArgumentException("不能传入不在容器的元素 " + element.Id);
			}
		}

		public void AddElement([NotNull] ILevelElement element)
		{
			_003C_003Ec__DisplayClass8_0 obj = default(_003C_003Ec__DisplayClass8_0);
			obj._003C_003E4__this = this;
			obj.element = element;
			if (obj.element == null)
			{
				throw new ArgumentNullException("element");
			}
			DependencyObject dependencyObject = obj.element as DependencyObject;
			if (dependencyObject != null)
			{
				InitializedLevelProperties initializedLevelProperties = dependencyObject.GetInitializedLevelProperties();
				if (initializedLevelProperties != null)
				{
					switch (initializedLevelProperties.InsertAction)
					{
					case InsertAction.After:
					{
						LevelInfo targetLevel2 = GetTargetLevel(initializedLevelProperties.TargetElement);
						LevelInfo levelInfo2 = new LevelInfo();
						int num2 = LevelList.IndexOf(targetLevel2);
						if (num2 == -1)
						{
							ThrowNotExistsLevelNodeException(targetLevel2.Id);
						}
						LevelList.Insert(num2 + 1, levelInfo2);
						AddElement(obj.element, levelInfo2);
						ApplyLevelToAllElement();
						break;
					}
					case InsertAction.Before:
					{
						LevelInfo targetLevel = GetTargetLevel(initializedLevelProperties.TargetElement);
						LevelInfo levelInfo = new LevelInfo();
						int num = LevelList.IndexOf(targetLevel);
						if (num == -1)
						{
							ThrowNotExistsLevelNodeException(targetLevel.Id);
						}
						LevelList.Insert(num, levelInfo);
						AddElement(obj.element, levelInfo);
						ApplyLevelToAllElement();
						break;
					}
					default:
						throw new ArgumentOutOfRangeException();
					}
					LevelManager.ClearInitializedLevelProperties(dependencyObject);
				}
				else
				{
					_003CAddElement_003Eg__AddLast_007C8_0(ref obj);
				}
			}
			else
			{
				_003CAddElement_003Eg__AddLast_007C8_0(ref obj);
			}
		}

		private LevelInfo GetTargetLevel(ILevelElement element)
		{
			return LevelElementDictionary[element];
		}

		private void AddElement(ILevelElement element, LevelInfo level)
		{
			LevelElementDictionary[element] = level;
		}

		public LevelMoveResultCollection MoveToTop([NotNull] params ILevelElement[] elementList)
		{
			if (elementList == null)
			{
				throw new ArgumentNullException("elementList");
			}
			if (!elementList.Any() || IsAllElement(elementList) || IsAllTop(elementList))
			{
				return LevelMoveResultCollection.Empty;
			}
			List<LevelMoveResult> list = new List<LevelMoveResult>();
			foreach (ILevelElement temp in GetLevelSortedElements(elementList))
			{
				LevelElementDictionary.ContainsKey(temp);
				list.Add(GetLevelMoveResult(temp, () =>
				{
					LevelInfo levelInfo = new LevelInfo();
					LevelList.Add(levelInfo);
					LevelElementDictionary[temp] = levelInfo;
				}));
			}
			ApplyLevelToAllElement();
			return new LevelMoveResultCollection(list, GetLevelErratic());
		}

		private void ApplyLevelToAllElement()
		{
			LevelOperator?.ApplyLevel(this);
		}

		public LevelMoveResultCollection MoveToBottom([NotNull] params ILevelElement[] elementList)
		{
			if (elementList == null)
			{
				throw new ArgumentNullException("elementList");
			}
			if (!elementList.Any() || IsAllElement(elementList) || IsAllBottom(elementList))
			{
				return LevelMoveResultCollection.Empty;
			}
			List<LevelMoveResult> list = new List<LevelMoveResult>();
			foreach (ILevelElement temp in GetLevelSortedElements(elementList).ToArray().Reverse())
			{
				list.Add(GetLevelMoveResult(temp, () =>
				{
					LevelInfo levelInfo = new LevelInfo();
					LevelList.Insert(0, levelInfo);
					LevelElementDictionary[temp] = levelInfo;
				}));
			}
			ApplyLevelToAllElement();
			return new LevelMoveResultCollection(list, GetLevelErratic());
		}

		public LevelMoveResultCollection ExchangeLevel([NotNull] ILevelElement sourceElement, [NotNull] ILevelElement targetElement)
		{
			if (sourceElement == null)
			{
				throw new ArgumentNullException("sourceElement");
			}
			if (targetElement == null)
			{
				throw new ArgumentNullException("targetElement");
			}
			if (!LevelElementDictionary.ContainsKey(sourceElement))
			{
				throw new ArgumentException("不能传入不在容器之内的元素 \r\n 传入元素 sourceElement " + sourceElement.Id + " targetElement " + targetElement.Id + " 其中 sourceElement 找不到");
			}
			if (!LevelElementDictionary.ContainsKey(targetElement))
			{
				throw new ArgumentException("不能传入不在容器之内的元素 \r\n 传入元素 sourceElement " + sourceElement.Id + " targetElement " + targetElement.Id + " 其中 targetElement 找不到");
			}
			if (sourceElement == targetElement)
			{
				return LevelMoveResultCollection.Empty;
			}
			LevelInfo a = LevelElementDictionary[sourceElement];
			LevelInfo b = LevelElementDictionary[targetElement];
			List<LevelMoveResult> list = new List<LevelMoveResult>();
			LevelMoveResult levelMoveResult = GetLevelMoveResult(sourceElement, () =>
			{
				LevelElementDictionary[sourceElement] = b;
			});
			list.Add(levelMoveResult);
			levelMoveResult = GetLevelMoveResult(targetElement, () =>
			{
				LevelElementDictionary[targetElement] = a;
			});
			list.Add(levelMoveResult);
			ApplyLevelToAllElement();
			return GetLevelMoveResultCollection(list);
		}

		public ILevelElement GetUpLevelElement([NotNull] ILevelElement targetElement)
		{
			VerifyElement(targetElement);
			List<ILevelElement> levelSortedElements = GetLevelSortedElements();
			int num = levelSortedElements.IndexOf(targetElement);
			if (num == levelSortedElements.Count - 1)
			{
				return null;
			}
			if (num == -1)
			{
				throw new ArgumentException("传入的targetElement在当前容器找不到");
			}
			return levelSortedElements[num + 1];
		}

		public ILevelElement GetDownLevelElement(ILevelElement targetElement)
		{
			VerifyElement(targetElement);
			List<ILevelElement> levelSortedElements = GetLevelSortedElements();
			int num = levelSortedElements.IndexOf(targetElement);
			switch (num)
			{
			case 0:
				return null;
			case -1:
				throw new ArgumentException("传入的targetElement在当前容器找不到");
			default:
				return levelSortedElements[num - 1];
			}
		}

		public List<ILevelElement> GetLevelSortedElements(IEnumerable<ILevelElement> elements)
		{
			return (from temp in elements.Select((ILevelElement temp) =>
				{
					VerifyElement(temp);
					int index = LevelList.IndexOf(LevelElementDictionary[temp]);
					return new
					{
						Element = temp,
						Index = index
					};
				})
				orderby temp.Index
				select temp.Element).ToList();
		}

		public List<ILevelElement> GetLevelSortedElements()
		{
			return GetLevelSortedElements(LevelContainer.GetLevelElementList());
		}

		public LevelMoveResultCollection MoveUp(params ILevelElement[] elementList)
		{
			if (!elementList.Any() || IsAllElement(elementList) || IsAllTop(elementList))
			{
				return LevelMoveResultCollection.Empty;
			}
			List<ILevelElement> sortedElements = GetLevelSortedElements(LevelContainer.GetLevelElementList());
			Dictionary<int, ILevelElement> dictionary = GetLevelSortedElements(elementList).ToDictionary((ILevelElement x) => sortedElements.IndexOf(x));
			List<ValueTuple<ILevelElement, List<ILevelElement>>> list = new List<ValueTuple<ILevelElement, List<ILevelElement>>>();
			int num = 0;
			foreach (KeyValuePair<int, ILevelElement> item4 in dictionary)
			{
				if (item4.Key > num)
				{
					list.Add(new ValueTuple<ILevelElement, List<ILevelElement>>(null, new List<ILevelElement> { item4.Value }));
				}
				else if (item4.Key == num)
				{
					if (!list.Any())
					{
						list.Add(new ValueTuple<ILevelElement, List<ILevelElement>>(null, new List<ILevelElement>()));
					}
					list.Last().Item2.Add(item4.Value);
				}
				num = item4.Key + 1;
				if (item4.Key == sortedElements.Count - 1)
				{
					list.RemoveAt(list.Count - 1);
				}
				else
				{
					list[list.Count - 1] = new ValueTuple<ILevelElement, List<ILevelElement>>(sortedElements[item4.Key + 1], list.Last().Item2);
				}
			}
			List<LevelMoveResult> levelMoveList = new List<LevelMoveResult>();
			foreach (var item5 in list)
			{
				ILevelElement item = item5.Item1;
				List<ILevelElement> item2 = item5.Item2;
				LevelInfo item3 = LevelElementDictionary[item];
				int n = LevelList.IndexOf(item3);
				n++;
				item2.OfType<ILevelElement>().Reverse().ForEach((ILevelElement x) =>
				{
					levelMoveList.Add(GetLevelMoveResult(x, () =>
					{
						LevelInfo levelInfo = new LevelInfo();
						LevelList.Insert(n, levelInfo);
						LevelElementDictionary[x] = levelInfo;
					}));
				});
			}
			ApplyLevelToAllElement();
			return new LevelMoveResultCollection(levelMoveList, GetLevelErratic());
		}

		public LevelMoveResultCollection MoveDown(params ILevelElement[] elementList)
		{
			if (!elementList.Any() || IsAllElement(elementList) || IsAllBottom(elementList))
			{
				return LevelMoveResultCollection.Empty;
			}
			List<ILevelElement> sortedElements = GetLevelSortedElements(LevelContainer.GetLevelElementList());
			Dictionary<int, ILevelElement> dictionary = GetLevelSortedElements(elementList).ToDictionary((ILevelElement x) => sortedElements.IndexOf(x));
			List<ValueTuple<ILevelElement, List<ILevelElement>>> list = new List<ValueTuple<ILevelElement, List<ILevelElement>>>();
			int num = 0;
			foreach (KeyValuePair<int, ILevelElement> item4 in dictionary)
			{
				if (item4.Key > num)
				{
					list.Add(new ValueTuple<ILevelElement, List<ILevelElement>>(sortedElements[item4.Key - 1], new List<ILevelElement> { item4.Value }));
				}
				else if (item4.Key == num && list.Any())
				{
					list.Last().Item2.Add(item4.Value);
				}
				num = item4.Key + 1;
			}
			List<LevelMoveResult> levelMoveList = new List<LevelMoveResult>();
			foreach (var item5 in list)
			{
				ILevelElement item = item5.Item1;
				List<ILevelElement> item2 = item5.Item2;
				LevelInfo item3 = LevelElementDictionary[item];
				int n = LevelList.IndexOf(item3);
				item2.Reverse();
				item2.ForEach((ILevelElement x) =>
				{
					levelMoveList.Add(GetLevelMoveResult(x, () =>
					{
						LevelInfo levelInfo = new LevelInfo();
						LevelList.Insert(n, levelInfo);
						LevelElementDictionary[x] = levelInfo;
					}));
				});
			}
			ApplyLevelToAllElement();
			return GetLevelMoveResultCollection(levelMoveList);
		}

		public LevelMoveResultCollection MoveUp(ILevelElement moveElement, ILevelElement targetElement)
		{
			VerifyElement(moveElement);
			VerifyElement(targetElement);
			if (moveElement == targetElement)
			{
				return LevelMoveResultCollection.Empty;
			}
			LevelInfo item = LevelElementDictionary[targetElement];
			int n = LevelList.IndexOf(item);
			LevelInfo level = new LevelInfo();
			n++;
			LevelMoveResult levelMoveResult = GetLevelMoveResult(moveElement, () =>
			{
				LevelList.Insert(n, level);
				LevelElementDictionary[moveElement] = level;
			});
			ApplyLevelToAllElement();
			return GetLevelMoveResultCollection(levelMoveResult);
		}

		public LevelMoveResultCollection MoveDown(ILevelElement moveElement, ILevelElement targetElement)
		{
			VerifyElement(moveElement);
			VerifyElement(targetElement);
			if (moveElement == targetElement)
			{
				return LevelMoveResultCollection.Empty;
			}
			LevelInfo item = LevelElementDictionary[targetElement];
			int n = LevelList.IndexOf(item);
			LevelInfo level = new LevelInfo();
			LevelMoveResult levelMoveResult = GetLevelMoveResult(moveElement, () =>
			{
				LevelList.Insert(n, level);
				LevelElementDictionary[moveElement] = level;
			});
			ApplyLevelToAllElement();
			return GetLevelMoveResultCollection(levelMoveResult);
		}

		private LevelMoveResult GetLevelMoveResult(ILevelElement element, Action action)
		{
			LevelInfo oldLevel = LevelElementDictionary[element];
			action();
			LevelInfo newLevel = LevelElementDictionary[element];
			return new LevelMoveResult(element, newLevel, oldLevel);
		}

		private bool IsAllElement(ILevelElement[] elementList)
		{
			if (LevelContainer.GetLevelElementList().Count == elementList.Length)
			{
				return true;
			}
			return false;
		}

		private bool IsAllTop(ILevelElement[] elementList)
		{
			if (elementList.Length == 1 && LevelElementDictionary[elementList[0]] == LevelList.Last())
			{
				return true;
			}
			List<ILevelElement> levelSortedElements = GetLevelSortedElements();
			int count = levelSortedElements.Count - elementList.Length;
			int level = GetLevel(levelSortedElements.Skip(count).First());
			return elementList.Select(GetLevel).All((int temp) => temp >= level);
		}

		private bool IsAllBottom(ILevelElement[] elementList)
		{
			if (elementList.Length == 1 && LevelElementDictionary[elementList[0]] == LevelList[0])
			{
				return true;
			}
			List<ILevelElement> levelSortedElements = GetLevelSortedElements();
			int level = GetLevel(levelSortedElements.Take(elementList.Length).Last());
			return elementList.Select(GetLevel).All((int temp) => temp <= level);
		}

		private int GetLevel(ILevelElement element)
		{
			return LevelList.IndexOf(LevelElementDictionary[element]);
		}

		public int Compare(ILevelElement x, ILevelElement y)
		{
			VerifyElement(x);
			VerifyElement(y);
			int num = LevelList.IndexOf(LevelElementDictionary[x]);
			int num2 = LevelList.IndexOf(LevelElementDictionary[y]);
			if (num == -1)
			{
				throw new ArgumentException("元素" + x.Id + "不在容器内");
			}
			if (num2 == -1)
			{
				throw new ArgumentException("元素" + y.Id + "不在容器内");
			}
			return num.CompareTo(num2);
		}

		private LevelErratic GetLevelErratic()
		{
			return _levelErratic ?? (_levelErratic = new LevelErratic(this));
		}

		private LevelMoveResultCollection GetLevelMoveResultCollection(IReadOnlyList<LevelMoveResult> levelMoveResultList)
		{
			return new LevelMoveResultCollection(levelMoveResultList, GetLevelErratic());
		}

		private LevelMoveResultCollection GetLevelMoveResultCollection(LevelMoveResult levelMoveResult)
		{
			return new LevelMoveResultCollection(levelMoveResult, GetLevelErratic());
		}

		[CompilerGenerated]
		private void _003CAddElement_003Eg__AddLast_007C8_0(ref _003C_003Ec__DisplayClass8_0 P_0)
		{
			if (!LevelElementDictionary.ContainsKey(P_0.element))
			{
				LevelInfo levelInfo = new LevelInfo();
				LevelList.Add(levelInfo);
				AddElement(P_0.element, levelInfo);
			}
			ApplyLevelToAllElement();
		}
	}
}
