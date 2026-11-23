using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using PPTServiceUltra.Ink.Core.Storage;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Extensions.SupportedExtChars;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Storage;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Extensions
{
	
	
	public static class TextEditorExtManager
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass3_0
		{
			
			public Type extCharType;

			internal ExtensionalChar _003CAddExtMapping_003Eg__Creator_007C0()
			{
				return (ExtensionalChar)Activator.CreateInstance(extCharType);
			}
		}

		
		
		private static readonly Dictionary<Type, ValueTuple<Type, Func<ExtensionalChar>>> ExtMapping;

		static TextEditorExtManager()
		{
			ExtMapping = new Dictionary<Type, ValueTuple<Type, Func<ExtensionalChar>>>();
			AddExtensionalMapping(typeof(ImageCharSaveInfo), typeof(ImageChar), () => new ImageChar());
		}

		public static void LoadSaveInfoAssembly(Assembly assembly)
		{
			TextContext.SaveInfoManager.Load(assembly);
		}

		public static void AddExtMapping(Type saveInfoType, Type extCharType)
		{
			_003C_003Ec__DisplayClass3_0 obj = new _003C_003Ec__DisplayClass3_0();
			obj.extCharType = extCharType;
			if (!ExtMapping.ContainsKey(saveInfoType))
			{
				ExtMapping.Add(saveInfoType, new ValueTuple<Type, Func<ExtensionalChar>>(obj.extCharType, obj._003CAddExtMapping_003Eg__Creator_007C0));
			}
		}

		public static void AddExtensionalMapping(Type saveInfoType, Type extCharType, Func<ExtensionalChar> creator)
		{
			if (!ExtMapping.ContainsKey(saveInfoType))
			{
				ExtMapping.Add(saveInfoType, new ValueTuple<Type, Func<ExtensionalChar>>(extCharType, creator));
			}
		}

		internal static ExtensionalChar CreateExtChar(SaveInfo saveInfo)
		{
			if (!ExtMapping.ContainsKey(saveInfo.GetType()))
			{
				throw new InvalidOperationException("Call AddExtensionalMapping Method Before Please");
			}
			ExtensionalChar extensionalChar = ExtMapping[saveInfo.GetType()].Item2();
			extensionalChar.SetSaveInfo((ExtCharSaveInfo)saveInfo);
			return extensionalChar;
		}

		public static Size GetTextCharSize(RunProperty runProperty)
		{
			return TextHelperInner.BuildDefaultCharSize(runProperty);
		}

		internal static void StyleAdapter(ExtensionalChar extensionalChar, PropertyChangedArgs args, ArrangingType arrangingType)
		{
			Size size = extensionalChar.Size;
			extensionalChar.StyleAdapter(args);
			if (!(extensionalChar is ImageChar))
			{
				Size size2 = extensionalChar.Size;
				if (arrangingType != ArrangingType.Horizontal && !size.Equals(size2))
				{
					extensionalChar.Size = new Size(extensionalChar.Size.Height, extensionalChar.Size.Width);
				}
			}
		}
	}
}
