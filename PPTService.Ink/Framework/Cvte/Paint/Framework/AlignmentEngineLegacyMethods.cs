using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Cvte.Paint.Framework
{
	public static class AlignmentEngineLegacyMethods
	{
		[Obsolete("此方法单词拼写错误，请修改为 Prepare。")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void Prepair(this IAlignmentEngine engine, Slide slide, IEnumerable<Element> staticElements, IEnumerable<Element> operatingElements)
		{
			engine.Prepare(slide, staticElements, operatingElements);
		}
	}
}
