using System;
using System.Collections.Generic;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Core
{
	internal sealed class LazyInitialzer
	{
		private readonly List<ILazyInitialize> _lazyParts = new List<ILazyInitialize>();

		private readonly List<Action> _notEmptyActions = new List<Action>();

		private readonly List<Action> _editableActions = new List<Action>();

		private bool _hasBeenEditable;

		private bool _hasBeenNotEmpty;

		private bool AllFinish
		{
			get
			{
				if (_hasBeenEditable)
				{
					return _hasBeenNotEmpty;
				}
				return false;
			}
		}

		internal void Add(ILazyInitialize part)
		{
			if (_hasBeenEditable || _hasBeenNotEmpty)
			{
				throw new InvalidOperationException("禁止在初始化过后，加入初始化");
			}
			_lazyParts.Add(part);
		}

		public void UntilEditable(Action initializer)
		{
			if (_hasBeenEditable)
			{
				initializer();
			}
			_editableActions.Add(initializer);
		}

		internal void UntilNotEmpty(Action initializer)
		{
			if (_hasBeenNotEmpty)
			{
				initializer();
			}
			else
			{
				_notEmptyActions.Add(initializer);
			}
		}

		internal void NowItIsEditable()
		{
			NowItIsReady(ref _hasBeenEditable, (ILazyInitialize i) =>
			{
				i.InitializeUntilEditable();
			}, _editableActions);
		}

		internal void NowItIsNotEmpty()
		{
			NowItIsReady(ref _hasBeenNotEmpty, (ILazyInitialize i) =>
			{
				i.InitializeUntilNotEmpty();
			}, _notEmptyActions);
		}

		private void NowItIsReady(ref bool field, Action<ILazyInitialize> initializer, IEnumerable<Action> actions = null)
		{
			if (field || _lazyParts.Count == 0)
			{
				return;
			}
			try
			{
				foreach (ILazyInitialize lazyPart in _lazyParts)
				{
					initializer(lazyPart);
				}
				if (actions != null)
				{
					foreach (Action action in actions)
					{
						action();
					}
				}
			}
			finally
			{
				field = true;
			}
			if (AllFinish)
			{
				_lazyParts.Clear();
				_lazyParts.Capacity = 0;
				_notEmptyActions.Clear();
				_notEmptyActions.Capacity = 0;
				_editableActions.Clear();
				_editableActions.Capacity = 0;
			}
		}
	}
}
