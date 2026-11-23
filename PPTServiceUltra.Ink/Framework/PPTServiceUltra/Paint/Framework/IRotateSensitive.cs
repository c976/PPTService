using System.Collections.Generic;

namespace PPTServiceUltra.Paint.Framework
{
	public interface IRotateSensitive
	{
		List<double> GetSensitiveAngles();

		AlignResult GetAlignResult(double angle);
	}
}
