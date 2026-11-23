using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Editing
{
	internal static class ImeHelper
	{
		internal static Rect GetBounds(FrameworkElement textarea, PresentationSource source)
		{
			if (source.RootVisual == null || !source.RootVisual.IsAncestorOf(textarea))
			{
				return Rect.Empty;
			}
			Rect rect = new Rect(0.0, 0.0, textarea.ActualWidth, textarea.ActualHeight);
			return TransformToDevice(textarea.TransformToAncestor(source.RootVisual).TransformBounds(rect), source.RootVisual);
		}

		internal static Rect TransformToDevice(Rect rect, Visual visual)
		{
			Matrix transformToDevice = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
			return Rect.Transform(rect, transformToDevice);
		}

		public static string GetCurrentInputMethod()
		{
			ImeNative.ITfInputProcessorProfiles profiles;
			if (ImeNative.TSF_NativeAPI.TF_CreateInputProcessorProfiles(out profiles) == 0)
			{
				try
				{
					short langid;
					profiles.GetCurrentLanguage(out langid);
					Guid catid = ImeNative.TSF_NativeAPI.GUID_TFCAT_TIP_KEYBOARD;
					Guid clsid;
					Guid profile;
					profiles.GetDefaultLanguageProfile(langid, ref catid, out clsid, out profile);
					ImeNative.ITfInputProcessorProfiles tfInputProcessorProfiles = profiles;
					Guid clsid2 = clsid;
					tfInputProcessorProfiles.GetActiveLanguageProfile(ref clsid2, out langid, out profile);
					ImeNative.ITfInputProcessorProfiles tfInputProcessorProfiles2 = profiles;
					clsid2 = clsid;
					short langid2 = langid;
					Guid profile2 = profile;
					IntPtr desc;
					if (tfInputProcessorProfiles2.GetLanguageProfileDescription(ref clsid2, langid2, ref profile2, out desc) == 0 && desc != IntPtr.Zero)
					{
						string result = Marshal.PtrToStringBSTR(desc);
						Marshal.FreeBSTR(desc);
						return result;
					}
				}
				catch (Exception)
				{
				}
			}
			return string.Empty;
		}
	}
}
