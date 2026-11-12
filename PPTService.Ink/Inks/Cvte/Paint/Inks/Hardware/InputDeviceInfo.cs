using System.Windows.Input;

namespace Cvte.Paint.Inks.Hardware
{
	public class InputDeviceInfo
	{
		public int Id { get; }

		public string Name { get; }

		public string ProductId { get; }

		public TabletDeviceType Type { get; }

		public InputDeviceInfo()
		{
		}

		public InputDeviceInfo(int id, string name, string productId, TabletDeviceType type)
		{
			Id = id;
			Name = name;
			ProductId = productId;
			Type = type;
		}
	}
}
