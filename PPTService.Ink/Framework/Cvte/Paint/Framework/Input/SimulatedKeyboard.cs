using System;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Cvte.Paint.Framework.Input
{
	public static class SimulatedKeyboard
	{
		private static class KeyboardFlag
		{
			public const int GWL_EXSTYLE = -20;

			public const int WS_EX_NOACTIVATE = 134217728;

			public const uint KEYEVENTF_KEYUP = 2u;

			public const int WM_CLOSE = 16;

			public const byte VK_ESC = 27;

			public const byte VK_Wave = 192;

			public const byte VK_1 = 49;

			public const byte VK_2 = 50;

			public const byte VK_3 = 51;

			public const byte VK_4 = 52;

			public const byte VK_5 = 53;

			public const byte VK_6 = 54;

			public const byte VK_7 = 55;

			public const byte VK_8 = 56;

			public const byte VK_9 = 57;

			public const byte VK_0 = 48;

			public const byte VK_OEM_MINUS = 189;

			public const byte VK_OEM_PLUS = 187;

			public const byte VK_BACK = 8;

			public const byte VK_TAB = 9;

			public const byte VK_Q = 81;

			public const byte VK_W = 87;

			public const byte VK_E = 69;

			public const byte VK_R = 82;

			public const byte VK_T = 84;

			public const byte VK_Y = 89;

			public const byte VK_U = 85;

			public const byte VK_I = 73;

			public const byte VK_O = 79;

			public const byte VK_P = 80;

			public const byte VK_LBracket = 219;

			public const byte VK_RBracket = 221;

			public const byte VK_Path = 220;

			public const byte VK_DELETE = 46;

			public const byte VK_Cap = 20;

			public const byte VK_A = 65;

			public const byte VK_S = 83;

			public const byte VK_D = 68;

			public const byte VK_F = 70;

			public const byte VK_G = 71;

			public const byte VK_H = 72;

			public const byte VK_J = 74;

			public const byte VK_K = 75;

			public const byte VK_L = 76;

			public const byte VK_Sem = 186;

			public const byte VK_Qute = 222;

			public const byte VK_Enter = 13;

			public const byte VK_Shift = 16;

			public const byte VK_Z = 90;

			public const byte VK_X = 88;

			public const byte VK_C = 67;

			public const byte VK_V = 86;

			public const byte VK_B = 66;

			public const byte VK_N = 78;

			public const byte VK_M = 77;

			public const byte VK_Comma = 188;

			public const byte VK_FullStop = 190;

			public const byte VK_Divide = 191;

			public const byte VK_Up = 38;

			public const byte VK_Ctrl = 17;

			public const byte VK_Win = 91;

			public const byte VK_Alt = 18;

			public const byte VK_Space = 32;

			public const byte VK_Left = 37;

			public const byte VK_Down = 40;

			public const byte VK_Right = 39;

			public const byte VK_Menu = 93;
		}

		public static void KeyDown(byte keyCode)
		{
			keybd_event(keyCode, 0, 0u, (UIntPtr)0uL);
		}

		public static void KeyDown(Key key)
		{
			keybd_event((byte)KeyInterop.VirtualKeyFromKey(key), 0, 0u, (UIntPtr)0uL);
		}

		public static void KeyUp(byte keyCode)
		{
			keybd_event(keyCode, 0, 2u, (UIntPtr)0uL);
		}

		public static void KeyUp(Key key)
		{
			keybd_event((byte)KeyInterop.VirtualKeyFromKey(key), 0, 2u, (UIntPtr)0uL);
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
	}
}
