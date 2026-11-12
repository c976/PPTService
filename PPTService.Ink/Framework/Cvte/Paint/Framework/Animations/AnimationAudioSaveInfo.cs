using Cvte.Storage;
using Cvte.Storage.Standard;

namespace Cvte.Paint.Framework.Animations
{
	[SaveInfoContract("AnimationAudioSaveInfo", Description = "动画音效")]
	public class AnimationAudioSaveInfo : SaveInfo
	{
		[SaveInfoMember("Name", Description = "音效名称")]
		public string Name { get; set; }

		[SaveInfoMember("Source", Description = "音效路径")]
		public Uri Source { get; set; }
	}
}
