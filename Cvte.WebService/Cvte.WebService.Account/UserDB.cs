using System.Runtime.CompilerServices;
using _0002;
using LinqToDB;
using LinqToDB.Data;

namespace Cvte.WebService.Account
{
	internal class UserDB : DataConnection
	{
		[SpecialName]
		public ITable<global::_0002._0002> _0001()
		{
			return GetTable<global::_0002._0002>();
		}
	}
}
