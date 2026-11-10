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
		private struct _003CGetAsync_003Ed__0<T> : IAsyncStateMachine where T : struct
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<T> _003C_003Et__builder;

			public Configurator config;

			public string key;

			private TaskAwaiter<string> _003C_003Eu__1;

			private void MoveNext()
			{
				TaskAwaiter<string> awaiter;
				if (_003C_003E1__state != 0)
				{
					awaiter = config.GetAsync(key).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						int num = 0;
						_003C_003E1__state = 0;
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<string>);
					int num = -1;
					_003C_003E1__state = -1;
				}
				string result = awaiter.GetResult();
				awaiter = default(TaskAwaiter<string>);
				string text = result;
				T result2 = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(text);
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
		private struct _003CGetAsync_003Ed__1<T> : IAsyncStateMachine where T : struct
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<T> _003C_003Et__builder;

			public Configurator config;

			public string key;

			public T defaultValue;

			private TaskAwaiter<string> _003C_003Eu__1;

			private void MoveNext()
			{
				TaskAwaiter<string> awaiter;
				if (_003C_003E1__state != 0)
				{
					awaiter = config.GetAsync(key, string.Empty).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						int num = 0;
						_003C_003E1__state = 0;
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<string>);
					int num = -1;
					_003C_003E1__state = -1;
				}
				string result = awaiter.GetResult();
				awaiter = default(TaskAwaiter<string>);
				string text = result;
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
				T result2 = (converter.IsValid(text) ? ((T)converter.ConvertFromInvariantString(text)) : defaultValue);
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
		private struct _003CSetAsync_003Ed__2<T> : IAsyncStateMachine where T : struct
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public T value;

			public Configurator config;

			public string key;

			private TaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				TaskAwaiter awaiter;
				if (_003C_003E1__state != 0)
				{
					string text = TypeDescriptor.GetConverter(typeof(T)).ConvertToInvariantString(value);
					awaiter = config.SetAsync(key, text).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						int num = 0;
						_003C_003E1__state = 0;
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter);
					int num = -1;
					_003C_003E1__state = -1;
				}
				awaiter.GetResult();
				awaiter = default(TaskAwaiter);
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

		private sealed class _003C_003Ec__DisplayClass3_0<T> where T : struct
		{
			public TypeConverter td;

			internal T _003CGetArrayAsync_003Eb__0(string x)
			{
				return (T)td.ConvertFromInvariantString(x);
			}
		}

		[StructLayout(LayoutKind.Auto)]
		private struct _003CGetArrayAsync_003Ed__3<T> : IAsyncStateMachine where T : struct
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<T[]> _003C_003Et__builder;

			public Configurator config;

			public string key;

			private _003C_003Ec__DisplayClass3_0<T> _003C_003E8__1;

			private TaskAwaiter<string[]> _003C_003Eu__1;

			private void MoveNext()
			{
				TaskAwaiter<string[]> awaiter;
				if (_003C_003E1__state != 0)
				{
					_003C_003E8__1 = new _003C_003Ec__DisplayClass3_0<T>();
					_003C_003E8__1.td = TypeDescriptor.GetConverter(typeof(T));
					awaiter = config.GetArrayAsync(key).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						int num = 0;
						_003C_003E1__state = 0;
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<string[]>);
					int num = -1;
					_003C_003E1__state = -1;
				}
				string[] result = awaiter.GetResult();
				awaiter = default(TaskAwaiter<string[]>);
				// struct 内的 lambda 不能捕获 this 成员（CS1673），先复制到局部变量
				_003C_003Ec__DisplayClass3_0<T> displayClass = this._003C_003E8__1;
				T[] result2 = result.Select((string x) => (T)displayClass.td.ConvertFromInvariantString(x)).ToArray();
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

		private sealed class _003C_003Ec__DisplayClass4_0<T> where T : struct
		{
			public TypeConverter td;

			internal bool _003CGetArrayAsync_003Eb__0(string x)
			{
				return td.IsValid(x);
			}

			internal T _003CGetArrayAsync_003Eb__1(string x)
			{
				return (T)td.ConvertFromInvariantString(x);
			}
		}

		[StructLayout(LayoutKind.Auto)]
		private struct _003CGetArrayAsync_003Ed__4<T> : IAsyncStateMachine where T : struct
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<T[]> _003C_003Et__builder;

			public Configurator config;

			public string key;

			private string[] _003CdefaultStringArray_003E5__1;

			public T[] defaultArray;

			private _003C_003Ec__DisplayClass4_0<T> _003C_003E8__2;

			private TaskAwaiter<string[]> _003C_003Eu__1;

			private void MoveNext()
			{
				TaskAwaiter<string[]> awaiter;
				if (_003C_003E1__state != 0)
				{
					_003C_003E8__2 = new _003C_003Ec__DisplayClass4_0<T>();
					_003C_003E8__2.td = TypeDescriptor.GetConverter(typeof(T));
					_003CdefaultStringArray_003E5__1 = new string[0];
					awaiter = config.GetArrayAsync(key, _003CdefaultStringArray_003E5__1).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						int num = 0;
						_003C_003E1__state = 0;
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<string[]>);
					int num = -1;
					_003C_003E1__state = -1;
				}
				string[] result = awaiter.GetResult();
				awaiter = default(TaskAwaiter<string[]>);
				string[] array = result;
				// struct 内的 lambda 不能捕获 this 成员（CS1673），先复制到局部变量
				_003C_003Ec__DisplayClass4_0<T> displayClass = this._003C_003E8__2;
				T[] result2 = ((_003CdefaultStringArray_003E5__1 != array) ? (from x in array
					where displayClass.td.IsValid(x)
					select (T)displayClass.td.ConvertFromInvariantString(x)).ToArray() : defaultArray);
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
		private struct _003CSetArrayAsync_003Ed__5<T> : IAsyncStateMachine where T : struct
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public Configurator config;

			public string key;

			public IEnumerable<T> values;

			private TaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				TaskAwaiter awaiter;
				if (_003C_003E1__state != 0)
				{
					TypeConverter td = TypeDescriptor.GetConverter(typeof(T));
					awaiter = config.SetArrayAsync(key, values.Select((T x) => td.ConvertToInvariantString(x))).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						int num = 0;
						_003C_003E1__state = 0;
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter);
					int num = -1;
					_003C_003E1__state = -1;
				}
				awaiter.GetResult();
				awaiter = default(TaskAwaiter);
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

		[AsyncStateMachine(typeof(_003CGetAsync_003Ed__0<>))]
		public static Task<T> GetAsync<T>(this Configurator config, string key) where T : struct
		{
			_003CGetAsync_003Ed__0<T> stateMachine = default(_003CGetAsync_003Ed__0<T>);
			stateMachine.config = config;
			stateMachine.key = key;
			stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder<T>.Create();
			stateMachine._003C_003E1__state = -1;
			AsyncTaskMethodBuilder<T> asyncTaskMethodBuilder = stateMachine._003C_003Et__builder;
			asyncTaskMethodBuilder.Start(ref stateMachine);
			return stateMachine._003C_003Et__builder.Task;
		}

		[AsyncStateMachine(typeof(_003CGetAsync_003Ed__1<>))]
		public static Task<T> GetAsync<T>(this Configurator config, string key, T defaultValue) where T : struct
		{
			_003CGetAsync_003Ed__1<T> stateMachine = default(_003CGetAsync_003Ed__1<T>);
			stateMachine.config = config;
			stateMachine.key = key;
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
			stateMachine.key = key;
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
			stateMachine.key = key;
			stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder<T[]>.Create();
			stateMachine._003C_003E1__state = -1;
			AsyncTaskMethodBuilder<T[]> asyncTaskMethodBuilder = stateMachine._003C_003Et__builder;
			asyncTaskMethodBuilder.Start(ref stateMachine);
			return stateMachine._003C_003Et__builder.Task;
		}

		[AsyncStateMachine(typeof(_003CGetArrayAsync_003Ed__4<>))]
		public static Task<T[]> GetArrayAsync<T>(this Configurator config, string key, T[] defaultArray) where T : struct
		{
			_003CGetArrayAsync_003Ed__4<T> stateMachine = default(_003CGetArrayAsync_003Ed__4<T>);
			stateMachine.config = config;
			stateMachine.key = key;
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
			stateMachine.key = key;
			stateMachine.values = values;
			stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			stateMachine._003C_003E1__state = -1;
			AsyncTaskMethodBuilder asyncTaskMethodBuilder = stateMachine._003C_003Et__builder;
			asyncTaskMethodBuilder.Start(ref stateMachine);
			return stateMachine._003C_003Et__builder.Task;
		}
	}
}
