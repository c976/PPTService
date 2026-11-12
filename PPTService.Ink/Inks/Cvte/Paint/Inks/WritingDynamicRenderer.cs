using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Media;

namespace Cvte.Paint.Inks
{
	internal class WritingDynamicRenderer : System.Windows.Input.StylusPlugIns.DynamicRenderer
	{
		private bool _isMouseDown;

		private readonly WritingMode _writingMode;

		private readonly Dictionary<int, int> _lastProcessedTimestamps = new Dictionary<int, int>();

		private bool _isValidSession = true;

		private readonly Dictionary<int, bool> _isWaitingForDownToActivate = new Dictionary<int, bool>();

		public WritingContext WritingContext { get; private set; }

		internal Matrix TransformFromBoard { get; set; }

		public WritingDynamicRenderer(WritingMode writingMode)
		{
			_writingMode = writingMode;
		}

		public void Reset()
		{
			WritingContext = null;
			_isValidSession = true;
		}

		protected override void OnStylusDown(RawStylusInput rawStylusInput)
		{
			if (HasAlreadySeen(rawStylusInput.Timestamp))
			{
				return;
			}
			_isMouseDown = rawStylusInput.StylusDeviceId == 0;
			if (WritingContext == null || WritingContext.DeviceId == rawStylusInput.StylusDeviceId)
			{
				WritingContext writingContext = _writingMode.GetWritingContext(rawStylusInput.StylusDeviceId);
				if (writingContext != null && writingContext.WritingDynamicRenderer == this)
				{
					Reset();
					writingContext = null;
				}
				if ((writingContext == null || writingContext.WritingDynamicRenderer == this) && writingContext == null && WritingContext == null && _isValidSession)
				{
					UpdateLastSeenTimestamp(rawStylusInput);
					SetIsWaitingForDownToActivate(rawStylusInput.StylusDeviceId, false);
					StartWriting(rawStylusInput);
				}
			}
		}

		protected override void OnStylusMove(RawStylusInput rawStylusInput)
		{
			if (HasAlreadySeen(rawStylusInput.Timestamp) || (WritingContext != null && WritingContext.DeviceId != rawStylusInput.StylusDeviceId))
			{
				return;
			}
			WritingContext writingContext = _writingMode.GetWritingContext(rawStylusInput.StylusDeviceId);
			if (writingContext != null && writingContext.WritingDynamicRenderer != this)
			{
				return;
			}
			if (writingContext == null && WritingContext == null && _isValidSession)
			{
				lock (_lastProcessedTimestamps)
				{
					int timestamp = rawStylusInput.Timestamp - 1000;
					if (!HasAlreadySeenStrict(timestamp, rawStylusInput.StylusDeviceId) && !IsWaitingForDownToActivate(rawStylusInput.StylusDeviceId) && Enabled)
					{
						StartWriting(rawStylusInput);
					}
					return;
				}
			}
			if (writingContext?.WritingDynamicRenderer == this && _isValidSession)
			{
				writingContext.LatestStylusInput = rawStylusInput;
				writingContext.LatestTimestamp = rawStylusInput.Timestamp;
				CollectPoints(rawStylusInput, writingContext);
				base.OnStylusMove(rawStylusInput);
				UpdateLastSeenTimestamp(rawStylusInput);
				_writingMode.OnWritingContinued(rawStylusInput, writingContext);
			}
		}

		protected override void OnStylusUp(RawStylusInput rawStylusInput)
		{
			UpdateLastSeenTimestamp(rawStylusInput);
			if (WritingContext != null && WritingContext.DeviceId != rawStylusInput.StylusDeviceId)
			{
				base.OnStylusUp(rawStylusInput);
				return;
			}
			WritingContext writingContext = _writingMode.GetWritingContext(rawStylusInput.StylusDeviceId);
			if (writingContext?.WritingDynamicRenderer == this)
			{
				writingContext.LatestStylusInput = rawStylusInput;
				writingContext.LatestTimestamp = rawStylusInput.Timestamp;
				writingContext.IsDynamicUpTriggered = true;
				CollectPoints(rawStylusInput, writingContext);
				if (!_isValidSession || !writingContext.IsProcessedByUI)
				{
					rawStylusInput.NotifyWhenProcessed(writingContext);
				}
				_writingMode.OnWritingCompleting(rawStylusInput, writingContext);
			}
			base.OnStylusUp(rawStylusInput);
		}

		protected override void OnStylusDownProcessed(object callbackData, bool targetVerified)
		{
			base.OnStylusDownProcessed(callbackData, targetVerified);
			if (!targetVerified)
			{
				RaiseWritingCanceled(callbackData);
			}
		}

		protected override void OnStylusMoveProcessed(object callbackData, bool targetVerified)
		{
			base.OnStylusMoveProcessed(callbackData, targetVerified);
			if (!targetVerified)
			{
				RaiseWritingCanceled(callbackData);
			}
		}

		protected override void OnStylusUpProcessed(object callbackData, bool targetVerified)
		{
			base.OnStylusUpProcessed(callbackData, targetVerified);
			WritingContext writingContext = callbackData as WritingContext;
			if (writingContext != null)
			{
				if (!_isValidSession)
				{
					Reset();
				}
				else if (!writingContext.IsProcessedByUI)
				{
					Reset();
					_writingMode.OnWritingCanceled(writingContext.DeviceId, null);
				}
			}
		}

		private void RaiseWritingCanceled(object callbackData)
		{
			int? num = callbackData as int?;
			if (num.HasValue && _isValidSession)
			{
				_isValidSession = false;
				_writingMode.OnWritingCanceled(num.Value, null);
			}
		}

		private void StartWriting(RawStylusInput rawStylusInput)
		{
			WritingContext writingContext = (WritingContext = new WritingContext(rawStylusInput, this));
			writingContext.LatestStylusInput = rawStylusInput;
			writingContext.LatestTimestamp = rawStylusInput.Timestamp;
			_writingMode.OnWritingStarting(rawStylusInput, writingContext);
			if (writingContext.IsCanceled)
			{
				_writingMode.OnWritingCanceled(rawStylusInput.StylusDeviceId, null);
				SetIsWaitingForDownToActivate(rawStylusInput.StylusDeviceId, true);
				return;
			}
			rawStylusInput.NotifyWhenProcessed(rawStylusInput.StylusDeviceId);
			CollectPoints(rawStylusInput, writingContext);
			base.OnStylusDown(rawStylusInput);
			_writingMode.OnWritingStarted(rawStylusInput, writingContext);
		}

		private void UpdateLastSeenTimestamp(RawStylusInput rawStylusInput)
		{
			lock (_lastProcessedTimestamps)
			{
				int value;
				if (!_lastProcessedTimestamps.TryGetValue(rawStylusInput.StylusDeviceId, out value) || value < rawStylusInput.Timestamp)
				{
					_lastProcessedTimestamps[rawStylusInput.StylusDeviceId] = rawStylusInput.Timestamp;
				}
			}
		}

		private bool HasAlreadySeen(int timestamp)
		{
			lock (_lastProcessedTimestamps)
			{
				return _lastProcessedTimestamps.Values.Any((int x) => x >= timestamp);
			}
		}

		private bool HasAlreadySeenStrict(int timestamp, int id)
		{
			lock (_lastProcessedTimestamps)
			{
				if (!_lastProcessedTimestamps.ContainsKey(id))
				{
					return false;
				}
				return _lastProcessedTimestamps[id] >= timestamp;
			}
		}

		protected override void OnDraw(DrawingContext drawingContext, StylusPointCollection stylusPoints, Geometry geometry, Brush fillBrush)
		{
			if (_isMouseDown && _writingMode.ShouldIgnoreOnce)
			{
				_writingMode.ShouldIgnoreOnce = false;
			}
			else if (WritingContext?.WritingDynamicRenderer == this && WritingContext != null)
			{
				_writingMode.OnDynamicRenderering(WritingContext, drawingContext, stylusPoints, geometry, fillBrush);
			}
		}

		private void TransformPoints(StylusPointCollection points, Matrix matrix)
		{
			for (int i = 0; i < points.Count; i++)
			{
				StylusPoint value = points[i];
				Point point = value.ToPoint();
				point = matrix.Transform(point);
				value.X = point.X;
				value.Y = point.Y;
				points[i] = value;
			}
		}

		private void CollectPoints(RawStylusInput rawStylusInput, WritingContext context)
		{
			StylusPointCollection points = rawStylusInput.GetStylusPoints().Clone();
			TransformPoints(points, TransformFromBoard);
			StylusPointCollection stylusPointCollection = _writingMode.FilterStylusPoints(points, context);
			if (stylusPointCollection != null && stylusPointCollection.Any())
			{
				Matrix transformFromBoard = TransformFromBoard;
				transformFromBoard.Invert();
				StylusPointCollection stylusPointCollection2 = stylusPointCollection.Clone();
				TransformPoints(stylusPointCollection2, transformFromBoard);
				rawStylusInput.SetStylusPoints(stylusPointCollection2);
				context.CollectedStylusPoints.AddRange(stylusPointCollection);
			}
		}

		internal bool IsWaitingForDownToActivate(int deviceId)
		{
			lock (_isWaitingForDownToActivate)
			{
				if (!_isWaitingForDownToActivate.ContainsKey(deviceId))
				{
					return false;
				}
				return _isWaitingForDownToActivate[deviceId];
			}
		}

		internal void SetIsWaitingForDownToActivate(int deviceId, bool value)
		{
			lock (_isWaitingForDownToActivate)
			{
				_isWaitingForDownToActivate[deviceId] = value;
			}
		}
	}
}
