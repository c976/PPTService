using System.Runtime.InteropServices;

namespace PPTServiceUltra.Ink.Core.Windows.CoreAudioAPI
{
	[Guid("886d8eeb-8cf2-4446-8d02-cdba1dbdcf99")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IPropertyStore
	{
		[PreserveSig]
		int GetCount([MarshalAs(UnmanagedType.U4)] out uint propertyCount);

		[PreserveSig]
		int GetAt([In][MarshalAs(UnmanagedType.U4)] uint propertyIndex, out PROPERTYKEY propertyKey);

		[PreserveSig]
		int GetValue([In] ref PROPERTYKEY propertyKey, out PROPVARIANT value);

		[PreserveSig]
		int SetValue([In] ref PROPERTYKEY propertyKey, [In] ref PROPVARIANT value);

		[PreserveSig]
		int Commit();
	}
}
