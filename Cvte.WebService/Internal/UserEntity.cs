using System.Runtime.CompilerServices;
using LinqToDB.Mapping;

namespace Cvte.WebService.Internal
{
	// LinqToDB 默认以类型名作为表名。重命名前该实体名为 Class26，
	// 这里显式锁定表名，避免类名改为 UserEntity 后 User.db 的表映射随之变化。
	[Table("Class26")]
	internal class UserEntity
	{
		private string string_0;

		private string string_1;

		private string string_2;

		private string string_3;

		private string string_4;

		private string string_5;

		private long? nullable_0;

		[SpecialName]
		public string method_0()
		{
			return string_0;
		}

		[SpecialName]
		public void method_1(string string_6)
		{
			string_0 = string_6;
		}

		[SpecialName]
		public string method_2()
		{
			return string_1;
		}

		[SpecialName]
		public void method_3(string string_6)
		{
			string_1 = string_6;
		}

		[SpecialName]
		public string method_4()
		{
			return string_2;
		}

		[SpecialName]
		public void method_5(string string_6)
		{
			string_2 = string_6;
		}

		[SpecialName]
		public string method_6()
		{
			return string_3;
		}

		[SpecialName]
		public void method_7(string string_6)
		{
			string_3 = string_6;
		}

		[SpecialName]
		public string method_8()
		{
			return string_4;
		}

		[SpecialName]
		public void method_9(string string_6)
		{
			string_4 = string_6;
		}

		[SpecialName]
		public string method_10()
		{
			return string_5;
		}

		[SpecialName]
		public void method_11(string string_6)
		{
			string_5 = string_6;
		}

		[SpecialName]
		public long? method_12()
		{
			return nullable_0;
		}

		[SpecialName]
		public void method_13(long? nullable_1)
		{
			nullable_0 = nullable_1;
		}
	}
}
