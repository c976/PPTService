using System;
using System.Runtime.InteropServices;
using System.Security;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Editing
{
	internal static class ImeNative
	{
		internal struct TF_LANGUAGEPROFILE
		{
			internal Guid clsid;

			internal short langid;

			internal Guid catid;

			[MarshalAs(UnmanagedType.Bool)]
			internal bool fActive;

			internal Guid guidProfile;
		}

		internal struct POINT
		{
			public int x;

			public int y;

			public override string ToString()
			{
				return x + " " + y;
			}
		}

		internal struct RECT
		{
			public int left;

			public int top;

			public int right;

			public int bottom;

			public override string ToString()
			{
				return left + " " + top + " " + right + " " + bottom;
			}
		}

		internal struct CompositionForm
		{
			public int dwStyle;

			public POINT ptCurrentPos;

			public RECT rcArea;
		}

		internal struct CandidateForm
		{
			public int dwIndex;

			public int dwStyle;

			public POINT ptCurrentPos;

			public RECT rcArea;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct LOGFONT
		{
			public int lfHeight;

			public int lfWidth;

			public int lfEscapement;

			public int lfOrientation;

			public int lfWeight;

			public byte lfItalic;

			public byte lfUnderline;

			public byte lfStrikeOut;

			public byte lfCharSet;

			public byte lfOutPrecision;

			public byte lfClipPrecision;

			public byte lfQuality;

			public byte lfPitchAndFamily;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string lfFaceName;
		}

		internal static class TSF_NativeAPI
		{
			public static readonly Guid GUID_TFCAT_TIP_KEYBOARD;

			static TSF_NativeAPI()
			{
				GUID_TFCAT_TIP_KEYBOARD = new Guid(880041059u, 45808, 18308, 139, 103, 94, 18, 200, 112, 26, 49);
			}

			[DllImport("msctf.dll")]
			[SecurityCritical]
			[SuppressUnmanagedCodeSecurity]
			public static extern int TF_CreateInputProcessorProfiles(out ITfInputProcessorProfiles profiles);
		}

		[ComImport]
		[Guid("aa80e801-2021-11d2-93e0-0060b067b86e")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface ITfThreadMgr
		{
			void Activate(out int clientId);

			void Deactivate();

			void CreateDocumentMgr(out IntPtr docMgr);

			void EnumDocumentMgrs(out IntPtr enumDocMgrs);

			void GetFocus(out IntPtr docMgr);

			void SetFocus(IntPtr docMgr);

			void AssociateFocus(IntPtr hwnd, IntPtr newDocMgr, out IntPtr prevDocMgr);

			void IsThreadFocus([MarshalAs(UnmanagedType.Bool)] out bool isFocus);

			void GetFunctionProvider(ref Guid classId, out IntPtr funcProvider);

			void EnumFunctionProviders(out IntPtr enumProviders);

			void GetGlobalCompartment(out IntPtr compartmentMgr);
		}

		[ComImport]
		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[Guid("1F02B6C5-7842-4EE6-8A0B-9A24183A95CA")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface ITfInputProcessorProfiles
		{
			[SecurityCritical]
			void Register();

			[SecurityCritical]
			void Unregister();

			[SecurityCritical]
			void AddLanguageProfile();

			[SecurityCritical]
			void RemoveLanguageProfile();

			[SecurityCritical]
			void EnumInputProcessorInfo();

			[SecurityCritical]
			int GetDefaultLanguageProfile(short langid, ref Guid catid, out Guid clsid, out Guid profile);

			[SecurityCritical]
			void SetDefaultLanguageProfile();

			[SecurityCritical]
			int ActivateLanguageProfile(ref Guid clsid, short langid, ref Guid guidProfile);

			[PreserveSig]
			[SecurityCritical]
			int GetActiveLanguageProfile(ref Guid clsid, out short langid, out Guid profile);

			[PreserveSig]
			[SecurityCritical]
			int GetLanguageProfileDescription(ref Guid clsid, short langid, ref Guid profile, out IntPtr desc);

			[SecurityCritical]
			void GetCurrentLanguage(out short langid);

			[PreserveSig]
			[SecurityCritical]
			int ChangeCurrentLanguage(short langid);

			[PreserveSig]
			[SecurityCritical]
			int GetLanguageList(out IntPtr langids, out int count);

			[SecurityCritical]
			int EnumLanguageProfiles(short langid, out IEnumTfLanguageProfiles enumIPP);

			[SecurityCritical]
			int EnableLanguageProfile();

			[SecurityCritical]
			int IsEnabledLanguageProfile(ref Guid clsid, short langid, ref Guid profile, out bool enabled);

			[SecurityCritical]
			void EnableLanguageProfileByDefault();

			[SecurityCritical]
			void SubstituteKeyboardLayout();
		}

		[ComImport]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[Guid("3d61bf11-ac5f-42c8-a4cb-931bcc28c744")]
		internal interface IEnumTfLanguageProfiles
		{
			void Clone(out IEnumTfLanguageProfiles enumIPP);

			[PreserveSig]
			int Next(int count, [Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] TF_LANGUAGEPROFILE[] profiles, out int fetched);

			void Reset();

			void Skip(int count);
		}

		internal const int CPS_CANCEL = 4;

		internal const int NI_COMPOSITIONSTR = 21;

		internal const int GCS_COMPSTR = 8;

		internal const int WM_IME_COMPOSITION = 271;

		internal const int WM_IME_SETCONTEXT = 641;

		internal const int WM_INPUTLANGCHANGE = 81;

		internal const int IMC_SETCANDIDATEPOS = 153;

		[ThreadStatic]
		internal static bool TextFrameworkThreadMgrInitialized;

		[ThreadStatic]
		internal static ITfThreadMgr TextFrameworkThreadMgr;

		[DllImport("imm32.dll")]
		internal static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);

		[DllImport("imm32.dll")]
		internal static extern IntPtr ImmGetContext(IntPtr hWnd);

		[DllImport("imm32.dll")]
		internal static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hWnd);

		[DllImport("imm32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);

		[DllImport("imm32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool ImmNotifyIME(IntPtr hIMC, int dwAction, int dwIndex, int dwValue = 0);

		[DllImport("imm32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool ImmSetCompositionWindow(IntPtr hIMC, ref CompositionForm form);

		[DllImport("imm32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool ImmGetCompositionWindow(IntPtr hIMC, out CompositionForm form);

		[DllImport("imm32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool ImmGetCandidateWindow(IntPtr hIMC, int dwIndex, out CandidateForm form);

		[DllImport("imm32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool ImmSetCandidateWindow(IntPtr hIMC, ref CandidateForm form);

		[DllImport("imm32.dll", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool ImmSetCompositionFont(IntPtr hIMC, ref LOGFONT font);

		[DllImport("imm32.dll")]
		internal static extern int ImmGetCompositionString(IntPtr hIMC, int dwIndex, byte[] data, int bufLen);

		[DllImport("msctf.dll")]
		internal static extern int TF_CreateThreadMgr(out ITfThreadMgr threadMgr);

		internal static ITfThreadMgr GetTextFrameworkThreadManager()
		{
			if (TextFrameworkThreadMgrInitialized)
			{
				return TextFrameworkThreadMgr;
			}
			TextFrameworkThreadMgrInitialized = true;
			TF_CreateThreadMgr(out TextFrameworkThreadMgr);
			return TextFrameworkThreadMgr;
		}
	}
}
