using System;
using System.Windows;
using Cvte.Paint.Framework.ElementLockedActions;
using Cvte.Storage;

namespace Cvte.Paint.Framework
{
	public class ElementActionExtension : IExtension<Element>, IExtension, IExtensionSavable<ElementSaveInfo>, ISavable
	{
		public static readonly RoutedEvent ElementIsLockedChangedEvent = EventManager.RegisterRoutedEvent("ElementIsLockedChanged", RoutingStrategy.Bubble, typeof(EventHandler<ElementIsLockedChangedRoutedEventArgs>), typeof(ElementActionExtension));

		private bool _isLocked;

		private Element _owner;

		public bool IsLocked
		{
			get
			{
				return _isLocked;
			}
			set
			{
				bool isLocked = _isLocked;
				_isLocked = value;
				if (_owner != null && isLocked != value)
				{
					ValueChangeEventArgs<bool> e = new ValueChangeEventArgs<bool>(isLocked, value);
					(_owner as IExtensionPropertySensitive)?.IsLockedPropertyChanged(e);
					Slide slide = Slide.GetSlide(_owner);
					_owner.RaiseEvent(new ElementIsLockedChangedRoutedEventArgs(ElementIsLockedChangedEvent, slide, _owner, e));
				}
			}
		}

		public bool CanClone { get; set; }

		public void Attached(Element owner)
		{
			_owner = owner;
		}

		public void Detached(Element owner)
		{
			_owner = null;
		}

		public void GetSaveInfo(ref ElementSaveInfo info)
		{
			info.IsLocked = IsLocked;
			info.CanClone = CanClone;
		}

		public void SetSaveInfo(ElementSaveInfo info)
		{
			IsLocked = info.IsLocked;
			CanClone = info.CanClone;
		}
	}
}
