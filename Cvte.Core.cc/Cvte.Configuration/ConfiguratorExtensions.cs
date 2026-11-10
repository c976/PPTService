using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Cvte.Configuration
{
	public static class ConfiguratorExtensions
	{
		[StructLayout(LayoutKind.Auto)]
		private struct _003CGetAsync_003Ed__0<SA_T0> : IAsyncStateMachine where SA_T0 : struct
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<SA_T0> _003C_003Et__builder;

			public Configurator config;

			public string key;

			private TaskAwaiter<string> _003C_003Eu__1;

			private void MoveNext()
			{
				int num = _003C_003E1__state;
				SA_T0 result2;
				try
				{
					int num2 = num;
					TaskAwaiter<string> awaiter;
					if (0 == 0)
					{
						if (num2 == 0)
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(TaskAwaiter<string>);
							num = (_003C_003E1__state = -1);
							goto IL_0085;
						}
						awaiter = config.GetAsync(key).GetAwaiter();
						num2 = (awaiter.IsCompleted ? 1 : 0);
					}
					if (num2 == 0)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0085;
					IL_0085:
					string result = awaiter.GetResult();
					awaiter = default(TaskAwaiter<string>);
					string text = result;
					result2 = (SA_T0)TypeDescriptor.GetConverter(typeof(SA_T0)).ConvertFromInvariantString(text);
				}
				catch (Exception exception)
				{
					_003C_003E1__state = -2;
					if (0 == 0)
					{
						_003C_003Et__builder.SetException(exception);
					}
					return;
				}
				if (0 == 0)
				{
					_003C_003E1__state = -2;
				}
				_003C_003Et__builder.SetResult(result2);
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout(LayoutKind.Auto)]
		private struct _003CGetAsync_003Ed__1<SA_T0> : IAsyncStateMachine where SA_T0 : struct
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<SA_T0> _003C_003Et__builder;

			public Configurator config;

			public string key;

			public SA_T0 defaultValue;

			private TaskAwaiter<string> _003C_003Eu__1;

			private void MoveNext()
			{
				int num = _003C_003E1__state;
				SA_T0 result2;
				try
				{
					if (num != 0)
					{
						goto IL_00da;
					}
					TaskAwaiter<string> awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<string>);
					num = (_003C_003E1__state = -1);
					goto IL_0088;
					IL_0050:
					_003C_003Eu__1 = awaiter;
					_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
					return;
					IL_00da:
					awaiter = config.GetAsync(key, string.Empty).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						goto IL_0050;
					}
					goto IL_0088;
					IL_0088:
					if (5 == 0)
					{
						goto IL_0050;
					}
					string result = awaiter.GetResult();
					awaiter = default(TaskAwaiter<string>);
					string text = result;
					TypeConverter converter = default(TypeConverter);
					SA_T0 val;
					if (2 != 0)
					{
						converter = TypeDescriptor.GetConverter(typeof(SA_T0));
						if (8 == 0)
						{
							goto IL_00da;
						}
						if (!converter.IsValid(text))
						{
							val = defaultValue;
							goto IL_00d1;
						}
					}
					val = (SA_T0)converter.ConvertFromInvariantString(text);
					goto IL_00d1;
					IL_00d1:
					result2 = val;
				}
				catch (Exception exception)
				{
					do
					{
						_003C_003E1__state = -2;
					}
					while (false);
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetResult(result2);
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout(LayoutKind.Auto)]
		private struct _003CSetAsync_003Ed__2<SA_T0> : IAsyncStateMachine where SA_T0 : struct
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public SA_T0 value;

			public Configurator config;

			public string key;

			private TaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				int num = _003C_003E1__state;
				try
				{
					TaskAwaiter awaiter;
					if (num != 0)
					{
						string text = TypeDescriptor.GetConverter(typeof(SA_T0)).ConvertToInvariantString(value);
						awaiter = config.SetAsync(key, text).GetAwaiter();
						goto IL_005a;
					}
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter);
					goto IL_009c;
					IL_009c:
					num = (_003C_003E1__state = -1);
					goto IL_00a5;
					IL_00a5:
					awaiter.GetResult();
					if (5 == 0)
					{
						goto IL_005a;
					}
					awaiter = default(TaskAwaiter);
					goto end_IL_000f;
					IL_005a:
					if (!awaiter.IsCompleted)
					{
						if (0 == 0)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_009c;
					}
					goto IL_00a5;
					end_IL_000f:;
				}
				catch (Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetResult();
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		private sealed class _003C_003Ec__DisplayClass3_0<SA_T0> where SA_T0 : struct
		{
			public TypeConverter td;

			internal SA_T0 _003CGetArrayAsync_003Eb__0(string x)
			{
				return (SA_T0)td.ConvertFromInvariantString(x);
			}
		}

		[StructLayout(LayoutKind.Auto)]
		private struct _003CGetArrayAsync_003Ed__3<SA_T0> : IAsyncStateMachine where SA_T0 : struct
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<SA_T0[]> _003C_003Et__builder;

			public Configurator config;

			public string key;

			private _003C_003Ec__DisplayClass3_0<SA_T0> _003C_003E8__1;

			private TaskAwaiter<string[]> _003C_003Eu__1;

			private void MoveNext()
			{
				int num = _003C_003E1__state;
				SA_T0[] result2 = default(SA_T0[]);
				try
				{
					int num2 = num;
					while (true)
					{
						TaskAwaiter<string[]> awaiter;
						if (num2 != 0)
						{
							if (0 == 0)
							{
								if (8 != 0)
								{
									_003C_003E8__1 = new _003C_003Ec__DisplayClass3_0<SA_T0>();
									_003C_003E8__1.td = TypeDescriptor.GetConverter(typeof(SA_T0));
									awaiter = config.GetArrayAsync(key).GetAwaiter();
									goto IL_006e;
								}
								break;
							}
						}
						else
						{
							awaiter = _003C_003Eu__1;
						}
						_003C_003Eu__1 = default(TaskAwaiter<string[]>);
						num = (_003C_003E1__state = -1);
						goto IL_00b9;
						IL_006e:
						num2 = (awaiter.IsCompleted ? 1 : 0);
						if (false)
						{
							continue;
						}
						if (num2 != 0)
						{
							goto IL_00b9;
						}
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						goto IL_014d;
						IL_00b9:
						if (3 != 0)
						{
							string[] result = awaiter.GetResult();
							awaiter = default(TaskAwaiter<string[]>);
							var __self1 = _003C_003E8__1;
						result2 = result.Select((string x) => (SA_T0)__self1.td.ConvertFromInvariantString(x)).ToArray();
							break;
						}
						goto IL_006e;
					}
				}
				catch (Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003Et__builder.SetException(exception);
					goto IL_014d;
				}
				goto IL_0136;
				IL_014d:
				if (3 != 0)
				{
					return;
				}
				goto IL_013e;
				IL_013e:
				if (false)
				{
					goto IL_0136;
				}
				_003C_003Et__builder.SetResult(result2);
				goto IL_014d;
				IL_0136:
				_003C_003E1__state = -2;
				goto IL_013e;
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		private sealed class _003C_003Ec__DisplayClass4_0<SA_T0> where SA_T0 : struct
		{
			public TypeConverter td;

			internal bool _003CGetArrayAsync_003Eb__0(string x)
			{
				return td.IsValid(x);
			}

			internal SA_T0 _003CGetArrayAsync_003Eb__1(string x)
			{
				return (SA_T0)td.ConvertFromInvariantString(x);
			}
		}

		[StructLayout(LayoutKind.Auto)]
		private struct _003CGetArrayAsync_003Ed__4<SA_T0> : IAsyncStateMachine where SA_T0 : struct
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<SA_T0[]> _003C_003Et__builder;

			public Configurator config;

			public string key;

			private string[] _003CdefaultStringArray_003E5__1;

			public SA_T0[] defaultArray;

			private _003C_003Ec__DisplayClass4_0<SA_T0> _003C_003E8__2;

			private TaskAwaiter<string[]> _003C_003Eu__1;

			private void MoveNext()
			{
				int num = _003C_003E1__state;
				SA_T0[] result2 = default(SA_T0[]);
				try
				{
					TaskAwaiter<string[]> awaiter;
					if (num != 0)
					{
						_003C_003E8__2 = new _003C_003Ec__DisplayClass4_0<SA_T0>();
						_003C_003E8__2.td = TypeDescriptor.GetConverter(typeof(SA_T0));
						_003CdefaultStringArray_003E5__1 = new string[0];
						awaiter = config.GetArrayAsync(key, _003CdefaultStringArray_003E5__1).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_00be;
					}
					if (6 != 0)
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(TaskAwaiter<string[]>);
						num = (_003C_003E1__state = -1);
						goto IL_00be;
					}
					goto end_IL_000f;
					IL_00be:
					string[] result = awaiter.GetResult();
					awaiter = default(TaskAwaiter<string[]>);
					string[] array = result;
					var __self2 = _003C_003E8__2;
					result2 = ((_003CdefaultStringArray_003E5__1 != array) ? (from x in array
						where __self2.td.IsValid(x)
						select (SA_T0)__self2.td.ConvertFromInvariantString(x)).ToArray() : defaultArray);
					end_IL_000f:;
				}
				catch (Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetResult(result2);
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		private sealed class _003C_003Ec__DisplayClass5_0<SA_T0> where SA_T0 : struct
		{
			public TypeConverter td;

			internal string _003CSetArrayAsync_003Eb__0(SA_T0 x)
			{
				return td.ConvertToInvariantString(x);
			}
		}

		[StructLayout(LayoutKind.Auto)]
		private struct _003CSetArrayAsync_003Ed__5<SA_T0> : IAsyncStateMachine where SA_T0 : struct
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public Configurator config;

			public string key;

			public IEnumerable<SA_T0> values;

			private TaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				_003C_003Ec__DisplayClass5_0<SA_T0> obj = default(_003C_003Ec__DisplayClass5_0<SA_T0>);
				do
				{
					int num = _003C_003E1__state;
					try
					{
						while (true)
						{
							TaskAwaiter awaiter;
							if (num == 0)
							{
								awaiter = _003C_003Eu__1;
								if (0 == 0)
								{
									_003C_003Eu__1 = default(TaskAwaiter);
									if (false)
									{
										continue;
									}
									num = (_003C_003E1__state = -1);
									goto IL_00c5;
								}
							}
							else
							{
								obj = new _003C_003Ec__DisplayClass5_0<SA_T0>();
								obj.td = TypeDescriptor.GetConverter(typeof(SA_T0));
							}
							goto IL_010c;
							IL_010c:
							awaiter = config.SetArrayAsync(key, values.Select(obj._003CSetArrayAsync_003Eb__0)).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								if (0 == 0)
								{
									num = (_003C_003E1__state = 0);
									_003C_003Eu__1 = awaiter;
									_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
								}
								if (0 == 0)
								{
									return;
								}
							}
							goto IL_00c5;
							IL_00c5:
							awaiter.GetResult();
							awaiter = default(TaskAwaiter);
							if (7 == 0)
							{
								goto IL_010c;
							}
							break;
						}
					}
					catch (Exception exception)
					{
						_003C_003E1__state = -2;
						_003C_003Et__builder.SetException(exception);
						return;
					}
					_003C_003E1__state = -2;
				}
				while (4 == 0);
				_003C_003Et__builder.SetResult();
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[AsyncStateMachine(typeof(_003CGetAsync_003Ed__0<>))]
		public static Task<T> GetAsync<T>(this Configurator config, string key) where T : struct
		{
			_003CGetAsync_003Ed__0<T> stateMachine = default(_003CGetAsync_003Ed__0<T>);
			stateMachine.config = config;
			if (0 == 0)
			{
				stateMachine.key = key;
			}
			AsyncTaskMethodBuilder<T> asyncTaskMethodBuilder;
			do
			{
				stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder<T>.Create();
				stateMachine._003C_003E1__state = -1;
				asyncTaskMethodBuilder = stateMachine._003C_003Et__builder;
			}
			while (false);
			asyncTaskMethodBuilder.Start(ref stateMachine);
			return stateMachine._003C_003Et__builder.Task;
		}

		[AsyncStateMachine(typeof(_003CGetAsync_003Ed__1<>))]
		public static Task<T> GetAsync<T>(this Configurator config, string key, T defaultValue) where T : struct
		{
			_003CGetAsync_003Ed__1<T> stateMachine = default(_003CGetAsync_003Ed__1<T>);
			stateMachine.config = config;
			if (4 != 0)
			{
				stateMachine.key = key;
			}
			stateMachine.defaultValue = defaultValue;
			stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder<T>.Create();
			stateMachine._003C_003E1__state = -1;
			AsyncTaskMethodBuilder<T> asyncTaskMethodBuilder = stateMachine._003C_003Et__builder;
			asyncTaskMethodBuilder.Start(ref stateMachine);
			return stateMachine._003C_003Et__builder.Task;
		}

		[AsyncStateMachine(typeof(_003CSetAsync_003Ed__2<>))]
		public static Task SetAsync<T>(this Configurator config, string key, T value) where T : struct
		{
			_003CSetAsync_003Ed__2<T> stateMachine = default(_003CSetAsync_003Ed__2<T>);
			stateMachine.config = config;
			if (4 != 0)
			{
				stateMachine.key = key;
			}
			stateMachine.value = value;
			stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			stateMachine._003C_003E1__state = -1;
			AsyncTaskMethodBuilder asyncTaskMethodBuilder = stateMachine._003C_003Et__builder;
			asyncTaskMethodBuilder.Start(ref stateMachine);
			return stateMachine._003C_003Et__builder.Task;
		}

		[AsyncStateMachine(typeof(_003CGetArrayAsync_003Ed__3<>))]
		public static Task<T[]> GetArrayAsync<T>(this Configurator config, string key) where T : struct
		{
			_003CGetArrayAsync_003Ed__3<T> stateMachine = default(_003CGetArrayAsync_003Ed__3<T>);
			stateMachine.config = config;
			if (0 == 0)
			{
				stateMachine.key = key;
			}
			AsyncTaskMethodBuilder<T[]> asyncTaskMethodBuilder;
			do
			{
				stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder<T[]>.Create();
				stateMachine._003C_003E1__state = -1;
				asyncTaskMethodBuilder = stateMachine._003C_003Et__builder;
			}
			while (false);
			asyncTaskMethodBuilder.Start(ref stateMachine);
			return stateMachine._003C_003Et__builder.Task;
		}

		[AsyncStateMachine(typeof(_003CGetArrayAsync_003Ed__4<>))]
		public static Task<T[]> GetArrayAsync<T>(this Configurator config, string key, T[] defaultArray) where T : struct
		{
			_003CGetArrayAsync_003Ed__4<T> stateMachine = default(_003CGetArrayAsync_003Ed__4<T>);
			stateMachine.config = config;
			if (4 != 0)
			{
				stateMachine.key = key;
			}
			stateMachine.defaultArray = defaultArray;
			stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder<T[]>.Create();
			stateMachine._003C_003E1__state = -1;
			AsyncTaskMethodBuilder<T[]> asyncTaskMethodBuilder = stateMachine._003C_003Et__builder;
			asyncTaskMethodBuilder.Start(ref stateMachine);
			return stateMachine._003C_003Et__builder.Task;
		}

		[AsyncStateMachine(typeof(_003CSetArrayAsync_003Ed__5<>))]
		public static Task SetArrayAsync<T>(this Configurator config, string key, IEnumerable<T> values) where T : struct
		{
			_003CSetArrayAsync_003Ed__5<T> stateMachine = default(_003CSetArrayAsync_003Ed__5<T>);
			stateMachine.config = config;
			if (4 != 0)
			{
				stateMachine.key = key;
			}
			stateMachine.values = values;
			stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			stateMachine._003C_003E1__state = -1;
			AsyncTaskMethodBuilder asyncTaskMethodBuilder = stateMachine._003C_003Et__builder;
			asyncTaskMethodBuilder.Start(ref stateMachine);
			return stateMachine._003C_003Et__builder.Task;
		}
	}
}
