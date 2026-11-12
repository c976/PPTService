using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Operations;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Settings;
using Cvte.Windows.Input;
using Cvte.Windows.Threading;

namespace Cvte.Paint.Inks
{
	public abstract class PenMode : WritingPenMode, IPenMode
	{
		private readonly List<DynamicInk> _inksToAddIntoSlide = new List<DynamicInk>();

		private readonly Dictionary<int, DynamicInk> _dynamicInks = new Dictionary<int, DynamicInk>();

		public bool DelayInserting { get; set; } = true;

		public bool IsBezierInterpolationEnabled { get; set; }

		public bool IsTouchWritingEnabled { get; set; } = true;

		public double MinDrawingTimeInMilliseconds { get; set; } = 300.0;

		protected InkBoardSettings InkBoardSettings => CurrentSlide.GetSettings<InkBoardSettings>();

		protected RecordWritingModeSettings RecordWritingModeSettings => CurrentSlide.GetSettings<RecordWritingModeSettings>();

		private AdornerLayer DynamicInkLayer
		{
			get
			{
				if (CurrentSlide == null)
				{
					return null;
				}
				return AdornerLayer.GetAdornerLayer(CurrentSlide);
			}
		}

		protected PenMode()
			: base(new ModeSettings
			{
				IsElementInteractionEnabled = false,
				IsTouchCursorVisible = false
			})
		{
		}

		protected PenMode(ModeSettings modeSettings)
			: base(modeSettings)
		{
		}

		protected override void OnDeviceDown(ModeInputEventArgs e)
		{
			if (e.Type != DeviceType.Touch || IsTouchWritingEnabled)
			{
				if (GetDynamicInk(e.Id) != null)
				{
					DeleteDynamicInk(e.Id);
				}
				DynamicInk dynamicInk = AddDynamicInk(e.Id);
				dynamicInk.AppendPoints(e.GetStylusPoints().ToArray());
				OnWritingStarted(e, dynamicInk.InkId);
			}
		}

		protected virtual void OnWritingStarted(ModeInputEventArgs e, string dynamicInkId)
		{
		}

		protected virtual void OnWritingContinued(ModeInputEventArgs e, string dynamicInkInkId)
		{
		}

		protected virtual void OnWritingCompleted(ModeInputEventArgs e, string dynamicInkInkId, Ink dynamicInkInk)
		{
		}

		protected override void OnDeviceMove(ModeInputEventArgs e)
		{
			if (e.Type != DeviceType.Touch || IsTouchWritingEnabled)
			{
				DynamicInk dynamicInk = GetDynamicInk(e.Id);
				if (dynamicInk == null)
				{
					dynamicInk = AddDynamicInk(e.Id);
				}
				dynamicInk.AppendPoints(e.GetStylusPoints().ToArray());
				OnWritingContinued(e, dynamicInk.InkId);
			}
		}

		protected override void OnDeviceUp(ModeInputEventArgs e)
		{
			if (e.Type == DeviceType.Touch && !IsTouchWritingEnabled)
			{
				return;
			}
			DynamicInk dynamicInk = GetDynamicInk(e.Id);
			if (dynamicInk == null)
			{
				return;
			}
			dynamicInk.AppendPoints(e.GetStylusPoints().ToArray());
			double totalMilliseconds = (DateTime.Now - dynamicInk.CreatedTime).TotalMilliseconds;
			if (totalMilliseconds > MinDrawingTimeInMilliseconds || !DelayInserting)
			{
				DeleteDynamicInk(e.Id);
				AddInkIntoSlide(dynamicInk);
				OnWritingCompleted(e, dynamicInk.InkId, dynamicInk.Ink);
				return;
			}
			DeleteDynamicInk(e.Id, false);
			_inksToAddIntoSlide.Add(dynamicInk);
			DispatcherTask.Start(MinDrawingTimeInMilliseconds - totalMilliseconds, () =>
			{
				if (CurrentSlide != null)
				{
					DeleteDynamicInk(dynamicInk, false);
					if (_inksToAddIntoSlide.Contains(dynamicInk))
					{
						_inksToAddIntoSlide.Remove(dynamicInk);
						AddInkIntoSlide(dynamicInk);
						OnWritingCompleted(e, dynamicInk.InkId, dynamicInk.Ink);
					}
				}
			});
		}

		protected override void OnSwitchOn()
		{
			base.OnSwitchOn();
		}

		protected override void OnSwitchOff()
		{
			base.OnSwitchOff();
			DeleteAllDynamicInks();
			foreach (DynamicInk item in _inksToAddIntoSlide)
			{
				DeleteDynamicInk(item);
				if (!item.CreatedTime.IsWithin(MinDrawingTimeInMilliseconds))
				{
					AddInkIntoSlide(item);
				}
			}
			_inksToAddIntoSlide.Clear();
		}

		protected abstract DynamicInk CreateDynamicInk();

		protected internal virtual void AddInkIntoSlide(DynamicInk dynamicInk)
		{
		}

		internal DynamicInk AddDynamicInk(int deviceId)
		{
			DynamicInk dynamicInk = CreateDynamicInk();
			dynamicInk.IsBezierInterpolationEnabled = IsBezierInterpolationEnabled;
			dynamicInk.NeedCollectStrokeDetails = RecordWritingModeSettings.NeedCollectStrokeDetails;
			_dynamicInks.Add(deviceId, dynamicInk);
			DynamicInkLayer?.Add(dynamicInk);
			return dynamicInk;
		}

		internal void DeleteDynamicInk(int deviceId, bool deleteVisial = true)
		{
			if (deleteVisial)
			{
				DynamicInkLayer?.Remove(_dynamicInks[deviceId]);
			}
			_dynamicInks.Remove(deviceId);
		}

		internal void DeleteDynamicInk(DynamicInk dynamicInk, bool immediately = true)
		{
			if (CurrentSlide == null)
			{
				return;
			}
			AdornerLayer inkLayer = DynamicInkLayer;
			if (inkLayer == null)
			{
				return;
			}
			if (immediately)
			{
				inkLayer.Remove(dynamicInk);
				return;
			}
			inkLayer.Dispatcher.InvokeAsync(() =>
			{
				inkLayer.Remove(dynamicInk);
			});
		}

		internal DynamicInk GetDynamicInk(int id)
		{
			DynamicInk value;
			if (_dynamicInks.TryGetValue(id, out value))
			{
				return value;
			}
			return null;
		}

		internal void DeleteAllDynamicInks()
		{
			int[] array = _dynamicInks.Keys.ToArray();
			foreach (int deviceId in array)
			{
				DeleteDynamicInk(deviceId);
			}
		}

		internal DynamicInk[] GetAllDynamicInks()
		{
			return _dynamicInks.Values.ToArray();
		}

		protected void InsertAddingOperation(Element element)
		{
			Board.GetUndoRedoProvider(CurrentSlide)?.Insert(new AddElementsOperation(CurrentSlide, element));
		}
	}
}
