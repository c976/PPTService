using System.Collections.Generic;
using System.Windows.Input;

namespace Cvte.Paint.Framework.Controls.Texts
{
	public static class ValidKeys
	{
		private static readonly HashSet<Key> ValidList = new HashSet<Key>
		{
			Key.A,
			Key.B,
			Key.C,
			Key.D,
			Key.E,
			Key.F,
			Key.G,
			Key.H,
			Key.I,
			Key.J,
			Key.K,
			Key.L,
			Key.M,
			Key.N,
			Key.O,
			Key.P,
			Key.Q,
			Key.R,
			Key.S,
			Key.T,
			Key.U,
			Key.V,
			Key.W,
			Key.X,
			Key.Y,
			Key.Z,
			Key.NumPad0,
			Key.NumPad1,
			Key.NumPad2,
			Key.NumPad3,
			Key.NumPad4,
			Key.NumPad5,
			Key.NumPad6,
			Key.NumPad7,
			Key.NumPad8,
			Key.NumPad9,
			Key.D0,
			Key.D1,
			Key.D2,
			Key.D3,
			Key.D4,
			Key.D5,
			Key.D6,
			Key.D7,
			Key.D8,
			Key.D9,
			Key.Multiply,
			Key.Add,
			Key.Subtract,
			Key.Divide,
			Key.Space,
			Key.ImeProcessed
		};

		public static bool Contains(Key e)
		{
			return ValidList.Contains(e);
		}
	}
}
