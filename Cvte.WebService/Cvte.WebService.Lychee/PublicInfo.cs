namespace Cvte.WebService.Lychee
{
	public class PublicInfo
	{
		// 原版 Title / Scale 是两个独立字段，反编译被合并成一个 dynamic，
		// 导致先赋 Title 再赋 Scale 后 Title 读出 double，请求构造时抛绑定异常。
		private string _title;
		private string _author;
		private double _scale;
		private string _device;

		public string Title
		{
			get { return _title; }
			set { _title = value; }
		}

		public string Author
		{
			get { return _author; }
			set { _author = value; }
		}

		public double Scale
		{
			get { return _scale; }
			set { _scale = value; }
		}

		public string Device
		{
			get { return _device; }
			set { _device = value; }
		}
	}
}
