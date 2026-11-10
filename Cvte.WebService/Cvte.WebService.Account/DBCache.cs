using System;
using System.Runtime.CompilerServices;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;

namespace Cvte.WebService.Account
{
	public class DBCache<TDB> where TDB : DataConnection, new()
	{
		private readonly string string_0;

		private readonly Func<string> func_0;

		[SpecialName]
		private string method_0()
		{
			if (!string.IsNullOrEmpty(string_0))
			{
				return string_0;
			}
			return func_0();
		}

		public DBCache(string connectionString)
		{
			string_0 = connectionString;
		}

		public DBCache(Func<string> getConnectionStringFunc)
		{
			func_0 = getConnectionStringFunc;
		}

		public void Execute(Action<TDB> execute)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected Obj, but got Unknown
			TDB val = default(TDB);
			try
			{
				val = (TDB)Activator.CreateInstance(typeof(TDB), (object)new SQLiteDataProvider("SQLite.Classic"), method_0());
				execute(val);
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
			finally
			{
				object obj = val;
				if (obj != null)
				{
					((DataConnection)obj).Dispose();
				}
			}
		}

		public bool ExecuteTransaction(Action<TDB> execute)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected Obj, but got Unknown
			TDB val = default(TDB);
			try
			{
				val = (TDB)Activator.CreateInstance(typeof(TDB), (object)new SQLiteDataProvider("SQLite.Classic"), method_0());
				DataConnectionTransaction val2 = ((DataConnection)val).BeginTransaction();
				try
				{
					try
					{
						execute(val);
					}
					catch (Exception)
					{
						val2.Rollback();
						return false;
					}
					val2.Commit();
					return true;
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			finally
			{
				object obj = val;
				if (obj != null)
				{
					((DataConnection)obj).Dispose();
				}
			}
		}

		public TReturn Execute<TReturn>(Func<TDB, TReturn> execute)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected Obj, but got Unknown
			TDB val = default(TDB);
			try
			{
				val = (TDB)Activator.CreateInstance(typeof(TDB), (object)new SQLiteDataProvider("SQLite.Classic"), method_0());
				return execute(val);
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
				return default(TReturn);
			}
			finally
			{
				object obj = val;
				if (obj != null)
				{
					((DataConnection)obj).Dispose();
				}
			}
		}
	}
}
