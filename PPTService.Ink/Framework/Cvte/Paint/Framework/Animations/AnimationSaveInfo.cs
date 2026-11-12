using System;
using System.Collections.Generic;
using Cvte.Storage;
using Cvte.Storage.Standard;

namespace Cvte.Paint.Framework.Animations
{
	[SaveInfoContract("Animation", Description = "动画", UniqueIdentifyMemberName = "Id")]
	public class AnimationSaveInfo : SaveInfo, ICloneable<AnimationSaveInfo>
	{
		[SaveInfoMember("Type", Description = "类型")]
		public string Type { get; set; }

		[SaveInfoMember("Id", Description = "标识")]
		public string Id { get; set; }

		[SaveInfoMember("Number", Description = "编号")]
		public int Number { get; set; }

		[SaveInfoMember("Category", Description = "类别")]
		public AnimationCategory Category { get; set; }

		[SaveInfoMember("Trigger", Description = "触发时机")]
		public AnimationTrigger Trigger { get; set; }

		[SaveInfoMember("TriggerSource", Description = "触发源")]
		public string TriggerSource { get; set; }

		[SaveInfoMember("Duration", Description = "时长")]
		public Cvte.Storage.Standard.TimeSpan Duration { get; set; }

		[SaveInfoMember("Delay", Description = "延时")]
		public Cvte.Storage.Standard.TimeSpan Delay { get; set; }

		[SaveInfoMember("AccelerationRatio", Description = "加速比")]
		public double AccelerationRatio { get; set; }

		[SaveInfoMember("DecelerationRatio", Description = "减速比")]
		public double DecelerationRatio { get; set; }

		[SaveInfoMember("RepeatBehavior", Description = "播放次数", DefaultValue = 1)]
		public double RepeatBehavior { get; set; }

		[SaveInfoMember("Center", Description = "旋转或缩放中心")]
		public Point Center { get; set; }

		[SaveInfoMember("Start", Description = "起始值（Double型）")]
		public double? Start { get; set; }

		[SaveInfoMember("End", Description = "结束值（Double型）")]
		public double? End { get; set; }

		[SaveInfoMember("StartSize", Description = "起始值（Size型）")]
		public Size StartSize { get; set; }

		[SaveInfoMember("EndSize", Description = "结束值（Size型）")]
		public Size EndSize { get; set; }

		[SaveInfoMember("Orientation", Description = "方向（八种）")]
		public AnimationOrientation? Orientation { get; set; }

		[SaveInfoMember("Direction", Description = "方向（两种）")]
		public AnimationDirection? Direction { get; set; }

		[SaveInfoMember("Effect", Description = "效果（“无到有”或“有到无”）")]
		public AnimationEffect? Effect { get; set; }

		[SaveInfoMember("Path", Description = "动画路径")]
		public Path Path { get; set; }

		[SaveInfoMember("AnimationAudio", Description = "动画音效")]
		public AnimationAudioSaveInfo AnimationAudio { get; set; }

		[SaveInfoMember("Magnitude", Description = "级别")]
		public AnimationMagnitude? Magnitude { get; set; }

		[SaveInfoMember("Frequency", Description = "频率")]
		public double? Frequency { get; set; }

		[SaveInfoMember("Children", Description = "动画分量集合")]
		public List<AnimationSaveInfo> Children { get; set; }

		[SaveInfoMember("AnimationSequence", Description = "动画序列")]
		public string AnimationSequence { get; set; }

		public AnimationSaveInfo Clone()
		{
			Id = Guid.NewGuid().ToString();
			return this;
		}
	}
}
