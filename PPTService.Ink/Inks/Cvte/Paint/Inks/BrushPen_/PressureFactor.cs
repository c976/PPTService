using System;
using System.Collections.Generic;
using System.Linq;

namespace Cvte.Paint.Inks.BrushPen_
{
	public static class PressureFactor
	{
		private static readonly Queue<float> PressureFactors;

		private static DateTime _lastPressureFactorRecievedTime;

		public static float Default { get; set; }

		public static float MinValue { get; set; }

		public static float Current
		{
			get
			{
				UpdatePressureFactorQueue();
				if (PressureFactors.Count == 0)
				{
					return Default;
				}
				return PressureFactors.Average();
			}
		}

		public static int ReservedPressureFacotrCount { get; set; }

		public static TimeSpan ClearInterval { get; set; }

		static PressureFactor()
		{
			PressureFactors = new Queue<float>();
			Default = 0.5f;
			MinValue = 0.01f;
			ReservedPressureFacotrCount = 5;
			ClearInterval = TimeSpan.FromMilliseconds(300.0);
		}

		public static void SetPressure(float pressureFactor)
		{
			if (pressureFactor > 1f)
			{
				pressureFactor = 1f;
			}
			if (pressureFactor <= MinValue)
			{
				pressureFactor = MinValue;
			}
			UpdatePressureFactorQueue();
			PressureFactors.Enqueue(pressureFactor);
			while (PressureFactors.Count > ReservedPressureFacotrCount)
			{
				PressureFactors.Dequeue();
			}
			_lastPressureFactorRecievedTime = DateTime.Now;
		}

		private static void UpdatePressureFactorQueue()
		{
			if (DateTime.Now - _lastPressureFactorRecievedTime > ClearInterval)
			{
				PressureFactors.Clear();
			}
		}
	}
}
