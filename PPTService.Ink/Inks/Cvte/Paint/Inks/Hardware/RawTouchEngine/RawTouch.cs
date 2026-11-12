using System;
using System.Collections.Generic;
using System.Windows;

namespace Cvte.Paint.Inks.Hardware.RawTouchEngine
{
	public static class RawTouch
	{
		private static bool _isInitialized;

		private static readonly List<int> BlocksToRemove = new List<int>();

		private static readonly Dictionary<int, TouchArea> Blocks = new Dictionary<int, TouchArea>();

		public static void Initialize()
		{
			CVTouchInfoProvider.RawTouchDown -= CVTouchInfoProvider_RawTouchDown;
			CVTouchInfoProvider.RawTouchMove -= CVTouchInfoProvider_RawTouchMove;
			CVTouchInfoProvider.RawTouchUp -= CVTouchInfoProvider_RawTouchUp;
			CVTouchInfoProvider.RawTouchDown += CVTouchInfoProvider_RawTouchDown;
			CVTouchInfoProvider.RawTouchMove += CVTouchInfoProvider_RawTouchMove;
			CVTouchInfoProvider.RawTouchUp += CVTouchInfoProvider_RawTouchUp;
			_isInitialized = true;
		}

		private static void CVTouchInfoProvider_RawTouchDown(object sender, RawTouchEventArgs e)
		{
			Record(e);
		}

		private static void CVTouchInfoProvider_RawTouchMove(object sender, RawTouchEventArgs e)
		{
			Record(e);
		}

		private static void CVTouchInfoProvider_RawTouchUp(object sender, RawTouchEventArgs e)
		{
			Record(e);
		}

		private static void Record(RawTouchEventArgs e)
		{
			TouchArea value;
			if (!Blocks.TryGetValue(e.Id, out value))
			{
				Blocks[e.Id] = new TouchArea
				{
					CreatedTime = DateTime.Now,
					Size = new Size(e.Width, e.Height),
					Position = e.Position,
					LastUpdatedTime = DateTime.Now
				};
			}
			else
			{
				value.Size = new Size(e.Width, e.Height);
				value.Position = e.Position;
				value.LastUpdatedTime = DateTime.Now;
			}
			TouchDebugger.Log("RawTouch.Record: " + value);
		}

		public static TouchArea GetTouchArea(Point point)
		{
			if (!_isInitialized)
			{
				throw new InvalidOperationException("必须调用 Initialize 之后才可以调用 GetTouchArea 获取触摸信息");
			}
			BlocksToRemove.Clear();
			TouchArea result = null;
			foreach (KeyValuePair<int, TouchArea> block in Blocks)
			{
				if ((DateTime.Now - block.Value.LastUpdatedTime).TotalMilliseconds > 100.0)
				{
					BlocksToRemove.Add(block.Key);
				}
				else if ((block.Value.Position - point).Length < 10.0)
				{
					result = block.Value;
					break;
				}
			}
			foreach (int item in BlocksToRemove)
			{
				Blocks.Remove(item);
			}
			BlocksToRemove.Clear();
			return result;
		}

		public static List<TouchArea> GetAllTouchArea()
		{
			if (!_isInitialized)
			{
				throw new InvalidOperationException("必须调用 Initialize 之后才可以调用 GetTouchArea 获取触摸信息");
			}
			List<TouchArea> list = new List<TouchArea>();
			foreach (KeyValuePair<int, TouchArea> block in Blocks)
			{
				if ((DateTime.Now - block.Value.LastUpdatedTime).TotalMilliseconds <= 100.0)
				{
					list.Add(block.Value);
				}
			}
			return list;
		}
	}
}
