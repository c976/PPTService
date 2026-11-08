using System;
using System.Runtime.CompilerServices;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;

namespace Cvte.WebService.Account
{
	public class DBCache<TDB> where TDB : DataConnection, new()
	{
		private string m__connectionString;

		private Func<string> m__connectionStringFunc;

		private string _0001()
		{
			if (!string.IsNullOrEmpty(m__connectionString))
			{
				return m__connectionString;
			}
			return (m__connectionStringFunc != null) ? m__connectionStringFunc() : null;
		}

		public DBCache(string connectionString)
		{
			m__connectionString = connectionString;
		}

		public DBCache(Func<string> getConnectionStringFunc)
		{
			m__connectionStringFunc = getConnectionStringFunc;
		}

		public void Execute(Action<TDB> execute)
		{
			TDB val = null;
			try
			{
				val = (TDB)Activator.CreateInstance(typeof(TDB), new SQLiteDataProvider("SQLite.Classic"), _0001());
				execute(val);
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
			finally
			{
				val?.Dispose();
			}
		}

		public bool ExecuteTransaction(Action<TDB> execute)
		{
			TDB val = null;
			try
			{
				do
				{
					if (0 == 0)
					{
						val = (TDB)Activator.CreateInstance(typeof(TDB), new SQLiteDataProvider("SQLite.Classic"), _0001());
					}
				}
				while (1 == 0);
				using (DataConnectionTransaction dataConnectionTransaction = val.BeginTransaction())
				{
					try
					{
						execute(val);
					}
					catch (Exception)
					{
						if (0 == 0)
						{
							dataConnectionTransaction.Rollback();
							return false;
						}
						goto end_IL_004b;
					}
					dataConnectionTransaction.Commit();
					return true;
					end_IL_004b:;
				}
			}
			finally
			{
				val?.Dispose();
			}
			bool result;
			return result;
		}

		public TReturn Execute<TReturn>(Func<TDB, TReturn> execute)
		{
			TReturn result = default(TReturn);
			if (6 != 0)
			{
				TDB val = null;
				try
				{
					do
					{
						val = (TDB)Activator.CreateInstance(typeof(TDB), new SQLiteDataProvider("SQLite.Classic"), _0001());
						result = execute(val);
					}
					while (8 == 0);
				}
				catch (Exception value)
				{
					Console.WriteLine(value);
					do
					{
						result = default(TReturn);
					}
					while (false);
				}
				finally
				{
					do
					{
						if (2 != 0)
						{
							val?.Dispose();
						}
					}
					while (6 == 0);
				}
			}
			return result;
		}
	}
}
