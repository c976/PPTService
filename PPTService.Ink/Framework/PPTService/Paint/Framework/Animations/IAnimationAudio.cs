using System;

namespace PPTService.Paint.Framework.Animations
{
	public interface IAnimationAudio
	{
		string Name { get; set; }

		Uri Source { get; set; }
	}
}
