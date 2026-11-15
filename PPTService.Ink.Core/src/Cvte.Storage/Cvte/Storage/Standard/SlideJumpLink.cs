using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	[ActionType("SlideJump")]
	public class SlideJumpLink : Hyperlink
	{
		[ActionParameter("slideId")]
		public string SlideId { get; set; }

		public SlideJumpLink()
		{
		}

		public SlideJumpLink(string slideId)
		{
			SlideId = slideId;
		}
	}
}
