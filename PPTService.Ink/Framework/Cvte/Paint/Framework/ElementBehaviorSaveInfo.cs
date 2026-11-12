using Cvte.Storage;

namespace Cvte.Paint.Framework
{
	[SaveInfoContract("ElementBehavior", Description = "音视频播放行为")]
	public class ElementBehaviorSaveInfo : SaveInfo
	{
		[SaveInfoMember("IsLoopPlay", Description = "是否循环播放")]
		public bool IsLoopPlay { get; set; }

		[SaveInfoMember("IsAutoPlay", Description = "是否自动播放")]
		public bool IsAutoPlay { get; set; }

		[SaveInfoMember("IsCrossSlidePlay", Description = "是否可以跨页面播放")]
		public bool IsCrossSlidePlay { get; set; }

		[SaveInfoMember("StopPlayPageNumber", DefaultValue = 0, Description = "跨页面播放的时候到第多少页之后停止，此属性仅在 IsCrossSlidePlay 为 true 时生效")]
		public int StopPlayPageNumber { get; set; }

		public ElementBehaviorSaveInfo Clone()
		{
			return new ElementBehaviorSaveInfo
			{
				IsLoopPlay = IsLoopPlay,
				IsAutoPlay = IsAutoPlay,
				IsCrossSlidePlay = IsCrossSlidePlay,
				StopPlayPageNumber = StopPlayPageNumber
			};
		}
	}
}
