using System.Windows.Input;

namespace PPTService.Ink.Core.Hardware
{
	/// <summary>输入设备信息（WPF TabletDevice 数据封装，通用）。</summary>
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
