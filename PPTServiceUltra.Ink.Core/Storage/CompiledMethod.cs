using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Ink.Core.Storage
{
	internal static class CompiledMethod
	{
		
		public static Action<object, object> CreatePropertySetter([NotNull] PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentNullException("propertyInfo");
			}
			Type declaringType = propertyInfo.DeclaringType;
			if ((object)declaringType == null)
			{
				throw new ArgumentException("It can not be a runtime property.", "propertyInfo");
			}
			Type type = declaringType;
			Type propertyType = propertyInfo.PropertyType;
			DynamicMethod dynamicMethod = new DynamicMethod("<set_Property>", typeof(void), new Type[2]
			{
				typeof(object),
				typeof(object)
			});
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			dynamicMethod.DefineParameter(1, ParameterAttributes.None, "this");
			dynamicMethod.DefineParameter(2, ParameterAttributes.None, "value");
			iLGenerator.Emit(OpCodes.Nop);
			iLGenerator.Emit(OpCodes.Ldarg_0);
			iLGenerator.Emit(OpCodes.Castclass, type);
			iLGenerator.Emit(OpCodes.Ldarg_1);
			OpCode opcode = (propertyInfo.PropertyType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass);
			iLGenerator.Emit(opcode, propertyType);
			MethodInfo setMethod = propertyInfo.GetSetMethod();
			if ((object)setMethod != null)
			{
				iLGenerator.Emit(OpCodes.Callvirt, setMethod);
				iLGenerator.Emit(OpCodes.Nop);
				iLGenerator.Emit(OpCodes.Ret);
				return (Action<object, object>)dynamicMethod.CreateDelegate(typeof(Action<object, object>));
			}
			if (propertyInfo.GetSetMethod(true) == null)
			{
				throw new NotSupportedException($"SaveInfo 中的属性 {type}.{propertyInfo.Name} 必须有公共可见的 set 方法。");
			}
			throw new NotSupportedException($"SaveInfo 中的属性 {type}.{propertyInfo.Name} 不能使用私有的 set 方法，请修改为 public set。");
		}
	}
}
