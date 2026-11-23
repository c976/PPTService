namespace PPTServiceUltra.Ink.Core.Windows.Input
{
	public static class DeviceButtonExtensions
	{
		public static string GetName(this DeviceButton button, DeviceType type)
		{
			switch (type)
			{
			case DeviceType.Mouse:
				if (button != DeviceButton.LeftButton)
				{
					return "RightButton";
				}
				return "LeftButton";
			case DeviceType.Touch:
				return "Touch";
			case DeviceType.Stylus:
				switch (button)
				{
				case DeviceButton.LeftButton:
					return "Stylus";
				case DeviceButton.Context:
					return "Barrel";
				case DeviceButton.Eraser:
					return "Eraser";
				}
				break;
			}
			return "Unknown";
		}
	}
}
