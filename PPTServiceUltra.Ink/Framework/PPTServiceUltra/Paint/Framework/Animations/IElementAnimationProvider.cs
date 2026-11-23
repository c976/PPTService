using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PPTServiceUltra.Paint.Framework.Animations
{
	public interface IElementAnimationProvider
	{
		void AddAnimations(Element element, IEnumerable<IAnimation> animations);

		ObservableCollection<IAnimation> GetAnimations(Element element);
	}
}
