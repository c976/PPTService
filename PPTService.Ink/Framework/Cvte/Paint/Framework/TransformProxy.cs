using System.Collections.Generic;
using System.Windows;

namespace Cvte.Paint.Framework
{
	public class TransformProxy
	{
		private readonly ElementProxy _proxy;

		internal Vector Offset
		{
			get
			{
				return _proxy.Offset;
			}
			set
			{
				_proxy.Offset = value;
			}
		}

		internal TransformProxy(Slide slide, Dictionary<Element, Rect> elementOriginBoundsDictionary)
		{
			_proxy = new ElementProxy(slide, elementOriginBoundsDictionary);
		}

		internal void Show()
		{
			_proxy.Show();
		}

		internal void Hide()
		{
			_proxy.Hide();
		}
	}
}
