using System.Collections.Generic;

namespace PPTService.Paint.Framework
{
	public interface IRotateSensitive
	{
		List<double> GetSensitiveAngles();

		AlignResult GetAlignResult(double angle);
	}
}
