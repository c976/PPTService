using System;
using System.Collections.Generic;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework.Animations;
using Cvte.Storage;
using Cvte.Storage.AdvancedSaveInfos;
using Cvte.Storage.Standard;

namespace Cvte.Paint.Framework
{
	[SaveInfoContract("Element", Description = "元素", UniqueIdentifyMemberName = "Id", IsSubPropertyDeltaSaveEnabled = false, SaveInfoType = SaveInfoType.Element, FallbackSaveInfoType = typeof(FallbackElementSaveInfo))]
	public class ElementSaveInfo : AdvancedSaveInfo, ICloneable<ElementSaveInfo>
	{
		[SaveInfoMember("Id", Description = "唯一标识", IsRequired = true)]
		public string Id { get; set; }

		[SaveInfoMember("X", Description = "X坐标", IsRequired = true, DefaultValue = 0.0)]
		public double X { get; set; }

		[SaveInfoMember("Y", Description = "Y坐标", IsRequired = true, DefaultValue = 0.0)]
		public double Y { get; set; }

		[DoubleRangeChecker(Maximun = double.MaxValue, Minimum = 0.125)]
		[SaveInfoMember("Width", Description = "宽度", IsRequired = true)]
		public double Width { get; set; }

		[DoubleRangeChecker(Maximun = double.MaxValue, Minimum = 0.125)]
		[SaveInfoMember("Height", Description = "高度", IsRequired = true)]
		public double Height { get; set; }

		[SaveInfoMember("Rotation", Description = "旋转角度", DefaultValue = 0.0)]
		public double Rotation { get; set; }

		[CanBeNull]
		[SaveInfoMember("Transform", Description = "元素变换")]
		public Matrix Transform { get; set; }

		[SaveInfoMember("Level", IsRuntime = true, Description = "层级")]
		public string Level { get; set; }

		[SaveInfoMember("IsLocked", Description = "标记元素是否处于锁定状态", DefaultValue = false)]
		public bool IsLocked { get; set; }

		[SaveInfoMember("CanClone", Description = "标记元素是否处于可被复制的状态", DefaultValue = false)]
		public bool CanClone { get; set; }

		[SaveInfoMember("Animations", Description = "动画集合")]
		public IList<AnimationSaveInfo> Animations { get; set; }

		[SaveInfoMember("Hyperlink", Description = "超链接")]
		public string ElementHyperlink { get; set; }

		[SaveInfoMember("HasMask", Description = "是否具有纹理笔特性", DefaultValue = false, Aliases = new string[] { "IsTexture" })]
		public bool HasMask { get; set; }

		[SaveInfoMember("IsErasable", IsRuntime = true, Description = "是否可被擦除", DefaultValue = false)]
		public bool IsErasable { get; set; }

		[SaveInfoMember("IsErased", IsRuntime = true, Description = "是否已被擦除", DefaultValue = false)]
		public bool IsErased { get; set; }

		[SaveInfoMember("ClipGeometry", IsRuntime = true, Description = "显示范围")]
		public string ClipGeometry { get; set; }

		[SaveInfoMember("ClipMatrix", IsRuntime = true, Description = "显示范围的Matrix")]
		public Matrix ClipMatrix { get; set; }

		[SaveInfoMember("ElementBehavior", Description = "音视频播放行为")]
		public ElementBehaviorSaveInfo ElementBehavior { get; set; }

		[SaveInfoMember("RotateOrigin", Description = "旋转中心")]
		public Point RotateOrigin { get; set; }

		[SaveInfoMember("ShowRotateOrigin", Description = "是否显示旋转中心")]
		public bool? ShowRotateOrigin { get; set; }

		[SaveInfoMember("Thumbnail", Description = "元素缩略图")]
		[CanBeNull]
		public Cvte.Storage.Standard.Uri Thumbnail { get; set; }

		public ElementSaveInfo()
		{
			Metadata = new AdvancedSaveInfoMetadata
			{
				SupportsFallback = true,
				ImportanceLevel = NodeImportanceLevel.Important
			};
		}

		public ElementSaveInfo Clone()
		{
			ElementSaveInfo elementSaveInfo = CloneElement();
			if (elementSaveInfo == null)
			{
				elementSaveInfo = this;
			}
			elementSaveInfo.Id = Guid.NewGuid().ToString();
			IsLocked = false;
			CanClone = false;
			if (Animations != null)
			{
				for (int i = 0; i < Animations.Count; i++)
				{
					Animations[i] = Animations[i].Clone();
				}
			}
			return elementSaveInfo;
		}

		protected virtual ElementSaveInfo CloneElement()
		{
			return null;
		}
	}
}
