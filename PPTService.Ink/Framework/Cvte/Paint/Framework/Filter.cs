using System;
using Cvte.Paint.Framework.Settings;

namespace Cvte.Paint.Framework
{
	public abstract class Filter : InputProcessor, IActiveModeFilter
	{
		private int _priority;

		private bool _canSetPriority = true;

		private bool _isEnabled = true;

		public Mode CurrentMode { get; internal set; }

		public IActiveModeController ActiveModeController { get; set; }

		Mode IActiveModeFilter.CurrentMode
		{
			get
			{
				return CurrentMode;
			}
			set
			{
				CurrentMode = value;
			}
		}

		IActiveModeController IActiveModeFilter.ActiveModeController
		{
			get
			{
				return ActiveModeController;
			}
			set
			{
				ActiveModeController = value;
			}
		}

		public virtual bool IsEnabled
		{
			get
			{
				return (this as IAttachedSetting<BoardFilterSettings>)?.Settings.IsEnabled ?? _isEnabled;
			}
			set
			{
				IAttachedSetting<BoardFilterSettings> attachedSetting = this as IAttachedSetting<BoardFilterSettings>;
				if (attachedSetting != null)
				{
					attachedSetting.Settings.IsEnabled = value;
				}
				_isEnabled = value;
			}
		}

		internal int Priority => _priority;

		internal void RaiseWholeProgressCompleted()
		{
			OnWholeProgressCompleted();
		}

		protected virtual void OnWholeProgressCompleted()
		{
		}

		public sealed override void SwitchOn()
		{
			base.SwitchOn();
		}

		public sealed override void SwitchOff()
		{
			base.SwitchOff();
		}

		internal void Attach()
		{
			_canSetPriority = false;
		}

		internal void Detach()
		{
			_canSetPriority = true;
		}

		public bool SetPriority(int priority)
		{
			if (_canSetPriority)
			{
				_priority = priority;
				return true;
			}
			return false;
		}

		protected T GetSetting<T>() where T : BoardFilterSettings, new()
		{
			if (ActiveModeController == null)
			{
				throw new ArgumentException("此时 filter 还没添加进入控制，不应在此时对Filter进行设置。如果是在构造添加 Filter 请使用 AddFilter");
			}
			Board board = GetBoard();
			if (board == null)
			{
				throw new ArgumentException("此时的Filter的当前的Slide应该是在 Board 上");
			}
			T val = board.GetSettings<T>();
			if (val == null)
			{
				val = new T();
				board.ConfigFrom(val);
			}
			return val;
		}
	}
}
