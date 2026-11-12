using System;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework
{
	internal class RotationCenterInfo : IRotationCenterInfo
	{
		private readonly IDefaultRotationInfoHost _defaultHost;

		private bool? _isVisible;

		public bool IsVisible => _isVisible ?? _defaultHost.IsVisibleByDefault;

		internal bool IsSetByUser { get; private set; }

		internal RotationCenterInfo([NotNull] IDefaultRotationInfoHost defaultHost)
		{
			if (defaultHost == null)
			{
				throw new ArgumentNullException("defaultHost");
			}
			_defaultHost = defaultHost;
		}

		public void SetVisible(bool isVisible, bool isSetByUser)
		{
			_isVisible = isVisible;
			IsSetByUser = isSetByUser;
		}
	}
}
