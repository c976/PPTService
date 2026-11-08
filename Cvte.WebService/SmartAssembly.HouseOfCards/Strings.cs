// SA_COMPAT_ORIGINAL - original ILSpy output preserved for reference.
// Superseded by SA_Compat.cs; not compiled.
#if SA_ORIGINAL_DECOMPILED
using System;
using System.Reflection;
using System.Reflection.Emit;
using _0001;
using _0003;
using SmartAssembly.Delegates;

namespace SmartAssembly.HouseOfCards
{
	public static class Strings
	{
		[global::_0001._0001]
		public unsafe static void CreateGetStringDelegate(Type ownerType)
		{
			FieldInfo[] fields = ownerType.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetField);
			foreach (FieldInfo fieldInfo in fields)
			{
				try
				{
					if ((object)fieldInfo.FieldType != typeof(GetString))
					{
						continue;
					}
					DynamicMethod dynamicMethod;
					while (true)
					{
						dynamicMethod = new DynamicMethod(string.Empty, typeof(string), new Type[1] { typeof(int) }, ownerType.Module, true);
						ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
						iLGenerator.Emit(OpCodes.Ldarg_0);
						if (false)
						{
							break;
						}
						MethodInfo[] methods = typeof(_0003._0002).GetMethods(BindingFlags.Static | BindingFlags.Public);
						int num = 0;
						if (4 == 0)
						{
							continue;
						}
						while (true)
						{
							int num2 = num;
							IntPtr intPtr = (IntPtr)num2;
							if (0 == 0)
							{
								IntPtr intPtr2 = (IntPtr)(void*)methods.LongLength;
								while (true)
								{
									if (false)
									{
										goto IL_0100;
									}
									intPtr2 = (IntPtr)(int)(long)intPtr2;
									goto IL_0115;
									IL_0115:
									if ((long)num2 < (long)intPtr2)
									{
										MethodInfo methodInfo = methods[num];
										if ((object)methodInfo.ReturnType != typeof(string))
										{
											num2 = num;
											intPtr2 = (IntPtr)1;
											goto IL_0100;
										}
										iLGenerator.Emit(OpCodes.Ldc_I4, fieldInfo.MetadataToken & 0xFFFFFF);
										iLGenerator.Emit(OpCodes.Sub);
										if (2 == 0)
										{
											goto end_IL_0048;
										}
										iLGenerator.Emit(OpCodes.Call, methodInfo);
									}
									iLGenerator.Emit(OpCodes.Ret);
									goto end_IL_0048;
									IL_0100:
									if (intPtr2 != (IntPtr)0)
									{
										if (intPtr2 != (IntPtr)0)
										{
											break;
										}
										continue;
									}
									goto IL_0115;
								}
								intPtr = (IntPtr)(void*)((long)num2 + (long)intPtr2);
							}
							num = (int)(long)intPtr;
						}
						continue;
						end_IL_0048:
						break;
					}
					fieldInfo.SetValue(null, dynamicMethod.CreateDelegate(typeof(GetString)));
					break;
				}
				catch
				{
				}
			}
		}
	}
}

#endif
