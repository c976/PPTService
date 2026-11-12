using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Cvte.ComponentModel;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework
{
	public sealed class SlideCollection : IList<Slide>, ICollection<Slide>, IEnumerable<Slide>, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
	{
		private readonly List<Slide> _items;

		private readonly Board _board;

		private readonly Action<Slide> _onAdded;

		private readonly Action<Slide> _onRemoved;

		public int Count => _items.Count;

		public int CurrentIndex => IndexOf(_board.CurrentSlide);

		public bool IsReadOnly => false;

		public Slide this[int index]
		{
			get
			{
				return _items[index];
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public event PropertyChangedEventHandler PropertyChanged;

		internal SlideCollection(Board board, Action<Slide> onAdded, Action<Slide> onRemoved)
		{
			_board = board;
			_items = new List<Slide>();
			_onAdded = onAdded;
			_onRemoved = onRemoved;
		}

		internal void Reset(IEnumerable<Slide> slides)
		{
			Delete(_items, false);
			foreach (Slide slide in slides)
			{
				if (ShouldChangeSize(slide))
				{
					slide.Width = _board.SlideSize.Width;
					slide.Height = _board.SlideSize.Height;
				}
				_onAdded(slide);
				_items.Add(slide);
			}
			OnPropertyChanged("Count");
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		private static bool ShouldChangeSize(Slide slide)
		{
			if (!double.IsNaN(slide.Width) && !double.IsNaN(slide.Height))
			{
				return !SpecialSlide.GetKeepIndependentSize(slide);
			}
			return true;
		}

		internal void AddRange(IEnumerable<Slide> slides)
		{
			Insert(slides.ToList());
		}

		internal void Insert(Slide slide)
		{
			InsertAt(new Slide[1] { slide }, new int[1] { _items.Count });
		}

		internal void Insert(IList<Slide> slides)
		{
			List<int> list = new List<int>(slides.Count);
			for (int i = 0; i < slides.Count; i++)
			{
				list.Add(_items.Count + i);
			}
			InsertAt(slides, list);
		}

		internal void InsertAt(Slide slide, int index)
		{
			InsertAt(new Slide[1] { slide }, new int[1] { index });
		}

		internal void InsertAt(IList<Slide> slides, IList<int> indexes)
		{
			if (slides == null)
			{
				throw new ArgumentNullException("slides");
			}
			if (indexes == null)
			{
				throw new ArgumentNullException("indexes");
			}
			if (slides.Count != indexes.Count)
			{
				throw new InvalidOperationException("插入的集合数与位置数不一致");
			}
			int currentIndex = CurrentIndex;
			List<ValueTuple<Slide, int>> list = slides.Select((Slide t, int index) => new ValueTuple<Slide, int>(t, indexes[index])).ToList();
			list.Sort((ValueTuple<Slide, int> s1, ValueTuple<Slide, int> s2) => s1.Item2.CompareTo(s2.Item2));
			foreach (var item3 in list)
			{
				Slide item = item3.Item1;
				int item2 = item3.Item2;
				if (ShouldChangeSize(item))
				{
					item.Width = _board.SlideSize.Width;
					item.Height = _board.SlideSize.Height;
				}
				_onAdded(item);
				_items.Insert(item2, item);
			}
			int currentIndex2 = CurrentIndex;
			OnPropertyChanged("Count");
			if (currentIndex != currentIndex2)
			{
				OnPropertyChanged("CurrentIndex");
			}
			RaiseCollectionChanged(new SlideCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, slides)
			{
				ChangeIndex = indexes
			});
		}

		internal void Delete(Slide slide)
		{
			Delete(new Slide[1] { slide });
		}

		internal void Delete(IList<Slide> slides, bool raiseEvent = true)
		{
			slides = slides.ToList();
			foreach (Slide slide in slides)
			{
				if (DebuggingProperties.IsDebug && !_items.Contains(slide))
				{
					throw new ArgumentException("要删除的 Slide 不在 Board，不能删除不存在的Slide，id=" + slide.Id);
				}
				_items.Remove(slide);
				_onRemoved(slide);
				if (ShouldChangeSize(slide))
				{
					slide.ClearValue(FrameworkElement.WidthProperty);
					slide.ClearValue(FrameworkElement.HeightProperty);
				}
			}
			if (raiseEvent)
			{
				OnPropertyChanged("Count");
				OnPropertyChanged("CurrentIndex");
				RaiseCollectionChanged(new SlideCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, slides));
			}
		}

		internal void DeleteAt(int index)
		{
			Delete(new Slide[1] { _items[index] });
		}

		internal void DeleteAt(IList<int> indexes)
		{
			List<Slide> list = indexes.Select((int index) => _items[index]).ToList();
			Delete(list, false);
			OnPropertyChanged("Count");
			OnPropertyChanged("CurrentIndex");
			RaiseCollectionChanged(new SlideCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, list)
			{
				ChangeIndex = indexes
			});
		}

		internal void Move(int moveFrom, int moveTo)
		{
			if (moveFrom != moveTo)
			{
				List<int> list = new List<int>(_items.Count);
				for (int i = 0; i < _items.Count; i++)
				{
					list.Add(i);
				}
				list[moveFrom] = moveTo;
				list[moveTo] = moveFrom;
				Sort(list);
			}
		}

		internal void Sort(IList<int> moveTos)
		{
			if (moveTos.Count != _items.Count)
			{
				throw new ArgumentException();
			}
			List<string> oldIds = _items.Select((Slide x) => x.Id).ToList();
			List<string> list = moveTos.Select((int x) => oldIds[x]).ToList();
			for (int num = 0; num < list.Count; num++)
			{
				int oldIndex = oldIds.IndexOf(list[num]);
				if (oldIndex != num)
				{
					Slide slide = _items.FirstOrDefault((Slide x) => x.Id == oldIds[oldIndex]);
					if (slide != null)
					{
						_items.Remove(slide);
						_items.Insert(num, slide);
					}
				}
			}
			OnPropertyChanged("CurrentIndex");
			RaiseCollectionChanged(new SlideCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, moveTos, 0, 0));
		}

		public IEnumerator<Slide> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		void ICollection<Slide>.Add(Slide item)
		{
			throw new NotImplementedException();
		}

		internal void Clear()
		{
			foreach (Slide item in _items)
			{
				if (ShouldChangeSize(item))
				{
					item.ClearValue(FrameworkElement.WidthProperty);
					item.ClearValue(FrameworkElement.HeightProperty);
				}
				_onRemoved(item);
			}
			_items.Clear();
			OnPropertyChanged("Count");
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		void ICollection<Slide>.Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(Slide item)
		{
			return _items.Contains(item);
		}

		void ICollection<Slide>.CopyTo(Slide[] array, int arrayIndex)
		{
			for (int i = 0; i < _items.Count; i++)
			{
				array[arrayIndex + i] = _items[i];
			}
		}

		internal bool Remove(Slide slide)
		{
			try
			{
				int index = _items.IndexOf(slide);
				_items.Remove(slide);
				_onRemoved(slide);
				if (ShouldChangeSize(slide))
				{
					slide.ClearValue(FrameworkElement.WidthProperty);
					slide.ClearValue(FrameworkElement.HeightProperty);
				}
				OnPropertyChanged("Count");
				RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, slide, index));
				return true;
			}
			catch (InvalidOperationException)
			{
				return false;
			}
		}

		bool ICollection<Slide>.Remove(Slide item)
		{
			throw new NotImplementedException();
		}

		void IList<Slide>.Insert(int index, Slide slide)
		{
			throw new NotImplementedException();
		}

		internal void RemoveAt(int index)
		{
			Remove(_items[index]);
		}

		void IList<Slide>.RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		public int IndexOf(Slide slide)
		{
			return _items.IndexOf(slide);
		}

		public void Move(Slide slide, int index, bool raiseEvent = true)
		{
			int num = _items.IndexOf(slide);
			if (num != index)
			{
				_items.Remove(slide);
				_items.Insert(index, slide);
				if (raiseEvent)
				{
					OnPropertyChanged("Count");
					RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, new Slide[1] { slide }, index, num));
				}
			}
		}

		private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			CollectionChanged?.Invoke(this, e);
		}

		[NotifyPropertyChangedInvocator]
		internal void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
