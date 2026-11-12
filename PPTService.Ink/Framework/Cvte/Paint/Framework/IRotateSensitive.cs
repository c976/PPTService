using System.Collections.Generic;

namespace Cvte.Paint.Framework
{
	public interface IRotateSensitive
	{
		List<double> GetSensitiveAngles();

		AlignResult GetAlignResult(double angle);
	}
}
