using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Diagnostics
{
	public static class SimulatedMouse
	{
		[Flags]
		private enum MouseEventFlag : uint
		{
			Move = 1u,
			LeftDown = 2u,
			LeftUp = 4u,
			RightDown = 8u,
			RightUp = 0x10u,
			MiddleDown = 0x20u,
			MiddleUp = 0x40u,
			XDown = 0x80u,
			XUp = 0x100u,
			Wheel = 0x800u,
			VirtualDesk = 0x4000u,
			Absolute = 0x8000u
		}

		public static void Click(double x, double y, bool moveMouseToOriginalPosition = false)
		{
			PressLeftButton(x, y, moveMouseToOriginalPosition);
			ReleaseLeftButton(x, y, moveMouseToOriginalPosition);
		}

		public static void Click()
		{
			PressLeftButton();
			ReleaseLeftButton();
		}

		public static void PressLeftButton(double x, double y, bool moveMouseToOriginalPosition = false)
		{
			Simulate(MouseEventFlag.LeftDown, x, y);
		}

		public static void PressLeftButton()
		{
			System.Windows.Point currentPosition = GetCurrentPosition();
			PressLeftButton(currentPosition.X, currentPosition.Y);
		}

		public static void ReleaseLeftButton(double x, double y, bool moveMouseToOriginalPosition = false)
		{
			Simulate(MouseEventFlag.LeftUp, x, y, moveMouseToOriginalPosition);
		}

		public static void ReleaseLeftButton()
		{
			System.Windows.Point currentPosition = GetCurrentPosition();
			ReleaseLeftButton(currentPosition.X, currentPosition.Y);
		}

		public static void RightClick(double x, double y, bool moveMouseToOriginalPosition = false)
		{
			PressRightButton(x, y, moveMouseToOriginalPosition);
			ReleaseRightButton(x, y, moveMouseToOriginalPosition);
		}

		public static void RightClick()
		{
			PressRightButton();
			ReleaseRightButton();
		}

		public static void PressRightButton(double x, double y, bool moveMouseToOriginalPosition = false)
		{
			Simulate(MouseEventFlag.RightDown, x, y);
		}

		public static void PressRightButton()
		{
			System.Windows.Point currentPosition = GetCurrentPosition();
			PressRightButton(currentPosition.X, currentPosition.Y);
		}

		public static void ReleaseRightButton(double x, double y, bool moveMouseToOriginalPosition = false)
		{
			Simulate(MouseEventFlag.RightUp, x, y, moveMouseToOriginalPosition);
		}

		public static void ReleaseRightButton()
		{
			System.Windows.Point currentPosition = GetCurrentPosition();
			ReleaseRightButton(currentPosition.X, currentPosition.Y);
		}

		public static void MoveTo(double x, double y)
		{
			SetCursorPos((int)x, (int)y);
		}

		public static void MoveTo([NotNull] params double[] xAndYValues)
		{
			if (xAndYValues == null)
			{
				throw new ArgumentNullException("xAndYValues");
			}
			if (xAndYValues.Length != 0 && xAndYValues.Length % 2 == 0)
			{
				for (int i = 0; i < xAndYValues.Length; i += 2)
				{
					MoveTo(xAndYValues[i], xAndYValues[i + 1]);
				}
				return;
			}
			throw new ArgumentException("xAndYValues 的个数必须是大于等于2的偶数");
		}

		public static System.Windows.Point GetCurrentPosition()
		{
			System.Drawing.Point position = Cursor.Position;
			return new System.Windows.Point(position.X, position.Y);
		}

		[DllImport("user32.dll")]
		private static extern void mouse_event(MouseEventFlag flags, int dx, int dy, uint data, UIntPtr extraInfo);

		[DllImport("User32")]
		private static extern void SetCursorPos(int x, int y);

		private static void Simulate(MouseEventFlag flags, double x, double y)
		{
			Simulate(flags, x, y, true);
		}

		private static void Simulate(MouseEventFlag flags, double x, double y, bool moveMouseToOriginalPosition)
		{
			System.Drawing.Point position = Cursor.Position;
			MoveTo(x, y);
			mouse_event(flags, 0, 0, 0u, UIntPtr.Zero);
			if (moveMouseToOriginalPosition)
			{
				MoveTo(position.X, position.Y);
			}
		}
	}
}
