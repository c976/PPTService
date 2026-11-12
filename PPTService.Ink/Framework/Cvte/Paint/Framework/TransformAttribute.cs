using System;

namespace Cvte.Paint.Framework
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	[Obsolete("通过特性的方式性能比较差，请使用 IPermittedTransform 接口代替")]
	public class TransformAttribute : Attribute
	{
		public PermittedTransforms Permitted { get; set; }

		public TransformAttribute(PermittedTransforms permitted)
		{
			Permitted = permitted;
		}
	}
}
