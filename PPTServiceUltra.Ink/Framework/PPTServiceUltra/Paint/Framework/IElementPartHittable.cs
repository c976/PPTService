using System.Collections.Generic;

namespace PPTServiceUltra.Paint.Framework
{
	public interface IElementPartHittable
	{
		object HittedObject { get; set; }

		void HitPart(List<object> hittedObjects);
	}
}
