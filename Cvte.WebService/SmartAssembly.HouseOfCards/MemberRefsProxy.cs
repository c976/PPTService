// SA_COMPAT_ORIGINAL - original ILSpy output preserved for reference.
// Superseded by SA_Compat.cs; not compiled.
#if SA_ORIGINAL_DECOMPILED
using System;
using System.Reflection;
using System.Reflection.Emit;
using _0001;

namespace SmartAssembly.HouseOfCards
{
	public static class MemberRefsProxy
	{
		private static ModuleHandle _0001;

		private static char[] _0001;

		[global::_0001._0001]
		public unsafe static void CreateMemberRefsDelegates(int typeID)
		{
			Type type = default(Type);
			try
			{
				Type typeFromHandle = Type.GetTypeFromHandle(MemberRefsProxy._0001.ResolveTypeHandle(33554433 + typeID));
				if (0 == 0)
				{
					type = typeFromHandle;
				}
			}
			catch
			{
				return;
			}
			if (false)
			{
				goto IL_01d5;
			}
			FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetField);
			int num = 0;
			goto IL_024b;
			IL_00a5:
			IntPtr intPtr2;
			int num2;
			IntPtr intPtr = (IntPtr)(void*)((long)intPtr2 + (long)num2);
			if (false)
			{
				goto IL_0067;
			}
			int num3 = (int)(long)intPtr;
			goto IL_00bb;
			IL_01f5:
			ILGenerator iLGenerator;
			iLGenerator.Emit(OpCodes.Tailcall);
			bool flag;
			MethodInfo methodInfo;
			iLGenerator.Emit(flag ? OpCodes.Callvirt : OpCodes.Call, methodInfo);
			iLGenerator.Emit(OpCodes.Ret);
			Delegate value;
			DynamicMethod dynamicMethod;
			try
			{
				value = dynamicMethod.CreateDelegate(type);
			}
			catch
			{
				goto IL_0247;
			}
			goto IL_0236;
			IL_024b:
			FieldInfo fieldInfo;
			string name;
			if (num < fields.Length)
			{
				fieldInfo = fields[num];
				name = fieldInfo.Name;
				intPtr = (IntPtr)0;
				goto IL_0067;
			}
			return;
			IL_01d1:
			int num5;
			int num4 = num5;
			goto IL_01e9;
			IL_0067:
			flag = (byte)(long)intPtr != 0;
			num3 = 0;
			int num6 = name.Length - 1;
			goto IL_00c4;
			IL_00c4:
			char c;
			if (num6 >= 0)
			{
				c = name[num6];
				goto IL_0084;
			}
			goto IL_00ca;
			IL_0084:
			if (c == '~')
			{
				flag = true;
				goto IL_00ca;
			}
			num2 = 0;
			goto IL_00b5;
			IL_00ca:
			try
			{
				methodInfo = (MethodInfo)MethodBase.GetMethodFromHandle(MemberRefsProxy._0001.ResolveMethodHandle(num3 + 167772161));
			}
			catch
			{
				goto IL_0247;
			}
			num5 = (methodInfo.IsStatic ? 1 : 0);
			int num7;
			int i = default(int);
			while (num5 == 0)
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
				intPtr2 = (IntPtr)(void*)parameters.LongLength;
				if (false)
				{
					goto IL_00a5;
				}
				num7 = (int)(long)intPtr2 + 1;
				Type[] array = new Type[num7];
				array[0] = typeof(object);
				if (4 != 0)
				{
					i = 1;
				}
				for (; i < num7; i++)
				{
					array[i] = parameters[i - 1].ParameterType;
				}
				dynamicMethod = new DynamicMethod(string.Empty, methodInfo.ReturnType, array, type, true);
				iLGenerator = dynamicMethod.GetILGenerator();
				iLGenerator.Emit(OpCodes.Ldarg_0);
				if (num7 > 1)
				{
					iLGenerator.Emit(OpCodes.Ldarg_1);
				}
				if (num7 > 2)
				{
					iLGenerator.Emit(OpCodes.Ldarg_2);
				}
				if (num7 > 3)
				{
					iLGenerator.Emit(OpCodes.Ldarg_3);
				}
				if (num7 > 4)
				{
					num5 = 4;
					if (num5 == 0)
					{
						continue;
					}
					goto IL_01d1;
				}
				goto IL_01f5;
			}
			try
			{
				value = Delegate.CreateDelegate(fieldInfo.FieldType, methodInfo);
			}
			catch (Exception)
			{
				goto IL_0247;
			}
			goto IL_0236;
			IL_01d5:
			iLGenerator.Emit(OpCodes.Ldarg_S, num4);
			num4++;
			goto IL_01e9;
			IL_00b5:
			int num8;
			if (num2 < 58)
			{
				num8 = _0001[num2];
				goto IL_009c;
			}
			goto IL_00bb;
			IL_0236:
			try
			{
				if (0 == 0)
				{
					fieldInfo.SetValue(null, value);
				}
			}
			catch
			{
			}
			goto IL_0247;
			IL_01e9:
			if (false)
			{
				goto IL_0084;
			}
			if (num4 < num7)
			{
				goto IL_01d5;
			}
			goto IL_01f5;
			IL_00bb:
			num8 = num6 - 1;
			if (false)
			{
				goto IL_009c;
			}
			num6 = num8;
			goto IL_00c4;
			IL_009c:
			if (num8 == c)
			{
				intPtr2 = (IntPtr)(num3 * 58);
				goto IL_00a5;
			}
			num2++;
			goto IL_00b5;
			IL_0247:
			num++;
			goto IL_024b;
		}

		static MemberRefsProxy()
		{
			while (true)
			{
				if (-1 != 0)
				{
					_0001 = new char[58]
					{
						'\u0001', '\u0002', '\u0003', '\u0004', '\u0005', '\u0006', '\a', '\b', '\u000e', '\u000f',
						'\u0010', '\u0011', '\u0012', '\u0013', '\u0014', '\u0015', '\u0016', '\u0017', '\u0018', '\u0019',
						'\u001a', '\u001b', '\u001c', '\u001d', '\u001e', '\u001f', '\u007f', '\u0080', '\u0081', '\u0082',
						'\u0083', '\u0084', '\u0086', '\u0087', '\u0088', '\u0089', '\u008a', '\u008b', '\u008c', '\u008d',
						'\u008e', '\u008f', '\u0090', '\u0091', '\u0092', '\u0093', '\u0094', '\u0095', '\u0096', '\u0097',
						'\u0098', '\u0099', '\u009a', '\u009b', '\u009c', '\u009d', '\u009e', '\u009f'
					};
					if ((object)typeof(MulticastDelegate) == null)
					{
						goto IL_002d;
					}
				}
				goto IL_0045;
				IL_002d:
				if (0 == 0)
				{
					if (false)
					{
						continue;
					}
					if (5 != 0)
					{
						break;
					}
				}
				goto IL_0045;
				IL_0045:
				MemberRefsProxy._0001 = Assembly.GetExecutingAssembly().GetModules()[0].ModuleHandle;
				goto IL_002d;
			}
		}
	}
}

#endif
