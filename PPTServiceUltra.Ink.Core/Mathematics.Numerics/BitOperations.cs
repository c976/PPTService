using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Mathematics.Numerics
{
	internal static class BitOperations
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CLSCompliant(false)]
		public static uint RotateLeft(uint value, int offset)
		{
			return (value << offset) | (value >> 32 - offset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CLSCompliant(false)]
		public static ulong RotateLeft(ulong value, int offset)
		{
			return (value << offset) | (value >> 64 - offset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CLSCompliant(false)]
		public static uint RotateRight(uint value, int offset)
		{
			return (value >> offset) | (value << 32 - offset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CLSCompliant(false)]
		public static ulong RotateRight(ulong value, int offset)
		{
			return (value >> offset) | (value << 64 - offset);
		}
	}
}
