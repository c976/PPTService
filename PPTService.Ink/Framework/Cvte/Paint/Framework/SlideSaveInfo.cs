using System;
using System.Collections.Generic;
using Cvte.Diagnostics.Annotations;
using Cvte.Storage;
using Cvte.Storage.Standard;

namespace Cvte.Paint.Framework
{
	[SaveInfoContract("Slide", UniqueIdentifyMemberName = "Id", Description = "页面")]
	public class SlideSaveInfo : SaveInfo, ICloneable<SlideSaveInfo>
	{
		[NotNull]
		private IList<ElementSaveInfo> _elements;

		[SaveInfoMember("Id", IsRequired = true, Description = "唯一标识")]
		public string Id { get; set; }

		[DoubleRangeChecker(Maximun = 30720.0, Minimum = 15.0)]
		[SaveInfoMember("Width", IsRequired = true, Description = "页面宽度")]
		public double Width { get; set; }

		[DoubleRangeChecker(Maximun = 17280.0, Minimum = 8.4375)]
		[SaveInfoMember("Height", IsRequired = true, Description = "页面高度")]
		public double Height { get; set; }

		[SaveInfoMember("KeepIndependentSize", IsRequired = false, Description = "大小是否独立")]
		public bool KeepIndependentSize { get; set; }

		[SaveInfoMember("Background", Description = "背景")]
		public Brush Background { get; set; }

		[SaveInfoMember("Elements", Description = "元素集合", IsFallbackArray = true)]
		[NotNull]
		public IList<ElementSaveInfo> Elements
		{
			get
			{
				return _elements;
			}
			set
			{
				_elements = value;
			}
		}

		[SaveInfoMember("TransitionKey", Description = "切页动画")]
		public string TransitionKey { get; set; }

		[SaveInfoMember("Duration", Description = "切页动画时长")]
		public Cvte.Storage.Standard.TimeSpan Duration { get; set; }

		[SaveInfoMember("AnimationOrders", Description = "动画顺序")]
		public IList<string> AnimationOrders { get; set; }

		public SlideSaveInfo()
		{
			_elements = new FallbackArray<ElementSaveInfo>();
		}

		public SlideSaveInfo Clone()
		{
			Id = Guid.NewGuid().ToString();
			return this;
		}
	}
}
