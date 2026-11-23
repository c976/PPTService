using System;
using System.Runtime.InteropServices;

namespace PPTServiceUltra.Ink.Core.Windows.CoreAudioAPI
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct VariantData
	{
		[FieldOffset(0)]
		public bool AsBoolean;

		[FieldOffset(0)]
		public uint AsUInt32;

		[FieldOffset(0)]
		public IntPtr AsStringPtr;

		[FieldOffset(4)]
		public IntPtr AsFormatPtr;
	}
}
