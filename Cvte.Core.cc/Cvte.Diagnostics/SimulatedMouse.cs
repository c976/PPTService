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

		public unsafe static void MoveTo([NotNull] params double[] xAndYValues)
		{
			while (true)
			{
				IntPtr intPtr;
				if (0 == 0)
				{
					if (xAndYValues == null)
					{
						throw new ArgumentNullException("xAndYValues");
					}
					intPtr = (IntPtr)(void*)xAndYValues.LongLength;
					while (intPtr != (IntPtr)0)
					{
						intPtr = (IntPtr)(void*)xAndYValues.LongLength;
						if (7 == 0)
						{
							continue;
						}
						goto IL_001a;
					}
					goto IL_0022;
				}
				goto IL_004c;
				IL_004c:
				if (5 != 0)
				{
					break;
				}
				goto IL_0022;
				IL_001a:
				if (0 == 0)
				{
					intPtr = (IntPtr)((int)(long)intPtr % 2);
				}
				if (intPtr != (IntPtr)0)
				{
					goto IL_0022;
				}
				for (int i = 0; i < xAndYValues.Length; i += 2)
				{
					MoveTo(xAndYValues[i], xAndYValues[i + 1]);
				}
				goto IL_004c;
				IL_0022:
				if (5 != 0)
				{
					throw new ArgumentException("xAndYValues 的个数必须是大于等于2的偶数");
				}
			}
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
			if (-1 == 0)
			{
				goto IL_0026;
			}
			System.Drawing.Point position = Cursor.Position;
			double x2 = x;
			if (false)
			{
				goto IL_002b;
			}
			double y2 = y;
			goto IL_004f;
			IL_002b:
			y2 = position.Y;
			if (5 == 0)
			{
				goto IL_004f;
			}
			MoveTo(x2, y2);
			goto IL_003b;
			IL_0026:
			x2 = position.X;
			goto IL_002b;
			IL_004f:
			MoveTo(x2, y2);
			goto IL_0056;
			IL_0056:
			UIntPtr zero = UIntPtr.Zero;
			if (0 == 0)
			{
				mouse_event(flags, 0, 0, 0u, zero);
			}
			if (moveMouseToOriginalPosition)
			{
				goto IL_0026;
			}
			goto IL_003b;
			IL_003b:
			if (false)
			{
				goto IL_0056;
			}
		}
	}
}
