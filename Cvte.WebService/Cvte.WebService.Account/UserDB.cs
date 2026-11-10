using System.Runtime.CompilerServices;
using LinqToDB;
using LinqToDB.Data;
using Cvte.WebService.Internal;

namespace Cvte.WebService.Account
{
	internal class UserDB : DataConnection
	{
		[SpecialName]
		public ITable<UserEntity> method_0()
		{
			return ((DataConnection)this).GetTable<UserEntity>();
		}
	}
}
