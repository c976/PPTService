using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Cvte.Configuration
{
	public class Configurator
	{
		[StructLayout(LayoutKind.Auto)]
		[CompilerGenerated]
		private struct _003CReadConfigsAsync_003Ed__28 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public Configurator _003C_003E4__this;

			private ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				int num = _003C_003E1__state;
				Configurator CS_0024_003C_003E8__locals8 = _003C_003E4__this;
				try
				{
					while (true)
					{
						ConfiguredTaskAwaitable.ConfiguredTaskAwaiter awaiter;
						if (num == 0)
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaitable.ConfiguredTaskAwaiter);
							goto IL_0092;
						}
						ConfiguredTaskAwaitable configuredTaskAwaitable = Task.Run(() =>
						{
							bool flag = CS_0024_003C_003E8__locals8._fileInfo.Exists;
							while (true)
							{
								if (4 != 0)
								{
									if (!flag && 0 == 0)
									{
										CS_0024_003C_003E8__locals8.CreateNewXmlFile();
									}
									if (2 == 0)
									{
										break;
									}
									flag = CS_0024_003C_003E8__locals8._isExceptionHandled;
								}
								if (6 != 0 && 0 == 0)
								{
									if (!flag)
									{
										CS_0024_003C_003E8__locals8.LoadConfigFile();
										break;
									}
									try
									{
										CS_0024_003C_003E8__locals8.LoadConfigFile();
										break;
									}
									catch (Exception)
									{
										CS_0024_003C_003E8__locals8._fileInfo.Delete();
										CS_0024_003C_003E8__locals8.CreateNewXmlFile();
										break;
									}
								}
							}
						}).ConfigureAwait(false);
						goto IL_0045;
						IL_0062:
						if (0 == 0)
						{
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_0092;
						IL_0045:
						awaiter = configuredTaskAwaitable.GetAwaiter();
						if (5 != 0)
						{
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								goto IL_0062;
							}
							goto IL_009b;
						}
						break;
						IL_0092:
						num = (_003C_003E1__state = -1);
						goto IL_009b;
						IL_009b:
						if (2 != 0)
						{
							awaiter.GetResult();
							if (0 == 0)
							{
								awaiter = default(ConfiguredTaskAwaitable.ConfiguredTaskAwaiter);
								if (4 == 0)
								{
									continue;
								}
								CS_0024_003C_003E8__locals8.Root._hasRead = true;
								break;
							}
							goto IL_0045;
						}
						goto IL_0062;
					}
				}
				catch (Exception exception)
				{
					do
					{
						_003C_003E1__state = -2;
						_003C_003Et__builder.SetException(exception);
					}
					while (false);
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

		[StructLayout(LayoutKind.Auto)]
		[CompilerGenerated]
		private struct _003CSaveConfigsAsync_003Ed__29 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public Configurator _003C_003E4__this;

			private TaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				int num;
				if (0 == 0)
				{
					num = _003C_003E1__state;
				}
				Configurator configurator = _003C_003E4__this;
				try
				{
					TaskAwaiter awaiter = default(TaskAwaiter);
					if ((num != 0) ? true : false)
					{
						if (4 == 0)
						{
							goto IL_00cb;
						}
						if (8 != 0)
						{
							Configurator configurator2 = configurator;
							Configurator root = ((configurator != configurator.Root) ? FindRoot(configurator) : configurator);
							awaiter = Task.Run(() =>
							{
								root._contextDocument.Save(configurator2._fileInfo.FullName);
							}).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
								return;
							}
							goto IL_00c4;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(TaskAwaiter);
					}
					num = (_003C_003E1__state = -1);
					goto IL_00c4;
					IL_00cb:
					awaiter = default(TaskAwaiter);
					if (false)
					{
						return;
					}
					goto end_IL_0021;
					IL_00c4:
					awaiter.GetResult();
					goto IL_00cb;
					end_IL_0021:;
				}
				catch (Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				do
				{
					_003C_003E1__state = -2;
				}
				while (false);
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

		[StructLayout(LayoutKind.Auto)]
		[CompilerGenerated]
		private struct _003CGetAsync_003Ed__30 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<string> _003C_003Et__builder;

			public Configurator _003C_003E4__this;

			public string key;

			public string defaultValue;

			private TaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				int num = _003C_003E1__state;
				Configurator configurator = _003C_003E4__this;
				string result;
				try
				{
					TaskAwaiter awaiter;
					if (num != 0)
					{
						if (configurator.Root._hasRead)
						{
							goto IL_00a8;
						}
						awaiter = configurator.Root.ReadConfigsAsync().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							if (0 == 0)
							{
								return;
							}
							goto IL_00c6;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(TaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					awaiter.GetResult();
					awaiter = default(TaskAwaiter);
					goto IL_00a8;
					IL_00a8:
					XmlNode xmlNode = configurator._contextDocument.SelectSingleNode(configurator.CombineNodePath(configurator._nodePath, key));
					goto IL_00c6;
					IL_00c6:
					if (xmlNode != null)
					{
						result = xmlNode.InnerText;
					}
					else
					{
						if (defaultValue == null)
						{
							throw new KeyNotFoundException($"Key {key} not found.");
						}
						result = defaultValue;
					}
				}
				catch (Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetResult(result);
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
		[CompilerGenerated]
		private struct _003CSetAsync_003Ed__31 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public Configurator _003C_003E4__this;

			public string key;

			public string value;

			private TaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				int num;
				do
				{
					num = _003C_003E1__state;
				}
				while (4 == 0);
				Configurator configurator = _003C_003E4__this;
				Configurator configurator2;
				if (2u != 0)
				{
					configurator2 = configurator;
				}
				try
				{
					TaskAwaiter awaiter;
					if (num != 0)
					{
						if (num == 1)
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(TaskAwaiter);
							num = (_003C_003E1__state = -1);
							goto IL_013f;
						}
						if (configurator2.Root._hasRead)
						{
							goto IL_00b2;
						}
						awaiter = configurator2.Root.ReadConfigsAsync().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							goto IL_019c;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(TaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					awaiter.GetResult();
					awaiter = default(TaskAwaiter);
					goto IL_00b2;
					IL_013f:
					awaiter.GetResult();
					awaiter = default(TaskAwaiter);
					goto end_IL_0023;
					IL_00b2:
					key = configurator2.CombineNodePath(configurator2._nodePath, key);
					configurator2.CreateAndSelectNode(key).InnerText = value;
					if (configurator2.IsAutoSave)
					{
						awaiter = configurator2.Root.SaveConfigsAsync().GetAwaiter();
						if (awaiter.IsCompleted)
						{
							goto IL_013f;
						}
						num = (_003C_003E1__state = 1);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						goto IL_019c;
					}
					end_IL_0023:;
				}
				catch (Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003Et__builder.SetException(exception);
					goto IL_019c;
				}
				goto IL_0189;
				IL_0189:
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetResult();
				goto IL_019c;
				IL_019c:
				if (0 == 0)
				{
					return;
				}
				goto IL_0189;
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
		[CompilerGenerated]
		private struct _003CGetArrayAsync_003Ed__32 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<string[]> _003C_003Et__builder;

			public Configurator _003C_003E4__this;

			public string key;

			public string[] defaultValue;

			private TaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				int num = _003C_003E1__state;
				Configurator configurator = _003C_003E4__this;
				Configurator configurator2;
				if (true)
				{
					configurator2 = configurator;
				}
				string[] result;
				try
				{
					TaskAwaiter awaiter;
					if (num != 0)
					{
						if (configurator2.Root._hasRead)
						{
							goto IL_00a7;
						}
						awaiter = configurator2.Root.ReadConfigsAsync().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(TaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					awaiter.GetResult();
					awaiter = default(TaskAwaiter);
					goto IL_00a7;
					IL_00a7:
					XmlNode xmlNode = configurator2._contextDocument.SelectSingleNode(configurator2.CombineNodePath(configurator2._nodePath, key));
					if (-1 == 0)
					{
						return;
					}
					if (xmlNode != null)
					{
						result = (from XmlNode child in xmlNode.ChildNodes
							select child.InnerText).ToArray();
					}
					else
					{
						if (defaultValue == null)
						{
							throw new KeyNotFoundException($"Key {key} not found.");
						}
						result = defaultValue;
					}
				}
				catch (Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetResult(result);
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
		[CompilerGenerated]
		private struct _003CSetArrayAsync_003Ed__33 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public Configurator _003C_003E4__this;

			public string key;

			public IEnumerable<string> values;

			private TaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				int num = _003C_003E1__state;
				Configurator configurator = _003C_003E4__this;
				try
				{
					TaskAwaiter awaiter;
					if (num == 0)
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(TaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_00ad;
					}
					int num2 = num;
					goto IL_002a;
					IL_00ad:
					awaiter.GetResult();
					awaiter = default(TaskAwaiter);
					goto IL_00bc;
					IL_00bc:
					key = configurator.CombineNodePath(configurator._nodePath, key);
					XmlNode xmlNode = configurator.CreateAndSelectNode(key);
					xmlNode.RemoveAll();
					IEnumerator<string> enumerator = values.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							string current = enumerator.Current;
							XmlNode xmlNode2 = configurator._contextDocument.CreateNode(XmlNodeType.Element, "value", null);
							xmlNode2.InnerText = current;
							xmlNode.AppendChild(xmlNode2);
						}
					}
					finally
					{
						if (num < 0 && 0 == 0 && enumerator != null)
						{
							goto IL_013b;
						}
						goto IL_0142;
						IL_013b:
						enumerator.Dispose();
						goto IL_0142;
						IL_0142:
						if (4 == 0)
						{
							goto IL_013b;
						}
					}
					if (configurator.IsAutoSave)
					{
						awaiter = configurator.Root.SaveConfigsAsync().GetAwaiter();
						if (false)
						{
							goto IL_0191;
						}
						num2 = (awaiter.IsCompleted ? 1 : 0);
						if (8 == 0)
						{
							goto IL_002a;
						}
						if (num2 == 0)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_01ad;
					}
					goto end_IL_001e;
					IL_002a:
					if (num2 != 1 && 7 != 0)
					{
						if (!configurator.Root._hasRead)
						{
							awaiter = configurator.Root.ReadConfigsAsync().GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								if (true)
								{
									num = (_003C_003E1__state = 0);
									if (0 == 0)
									{
										_003C_003Eu__1 = awaiter;
									}
									_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
								}
								return;
							}
							goto IL_00ad;
						}
						goto IL_00bc;
					}
					goto IL_0191;
					IL_0191:
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_01ad;
					IL_01ad:
					awaiter.GetResult();
					awaiter = default(TaskAwaiter);
					end_IL_001e:;
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

		private readonly bool _isExceptionHandled;

		private bool _hasRead;

		private readonly string _nodeName;

		private readonly string _nodePath;

		private const char NodeSeparatorChar = '/';

		private static readonly char[] NodeSeparatorChars;

		private FileInfo _fileInfo;

		private readonly FileSystemWatcher _fileWatcher;

		private readonly ConfigXmlDocument _contextDocument;

		internal const string SyncObsoleteString = "同步读写配置的方法将于 2017年6月18日 删除；请在方法名称后加上 Async 替代。";

		internal const string SyncGetObsoleteString = "同步读写配置值的方法将于 2017年6月18日 删除；请使用 GetAsync<T> 替代。";

		internal const string SyncSetObsoleteString = "同步读写配置值的方法将于 2017年6月18日 删除；请使用 SetAsync<T> 替代。";

		private Configurator Root { get; }

		private Configurator Parent { get; }

		public bool IsAutoSave { get; set; } = true;

		public bool AutoReload
		{
			get
			{
				return _fileWatcher.EnableRaisingEvents;
			}
			set
			{
				_fileWatcher.EnableRaisingEvents = value;
				Root._hasRead = false;
			}
		}

		protected string FileName => _fileInfo.FullName;

		public Configurator(string fileName, string rootName = null, bool isExceptionHandled = true)
		{
			_isExceptionHandled = isExceptionHandled;
			Root = this;
			Parent = null;
			_nodeName = rootName ?? "configuration";
			_nodePath = _nodeName;
			_fileInfo = new FileInfo(fileName);
			_fileWatcher = new FileSystemWatcher(_fileInfo.DirectoryName, _fileInfo.Name);
			_fileWatcher.Changed += OnConfigFileChanged;
			_contextDocument = new ConfigXmlDocument();
		}

		public Configurator(Configurator owner, string nodeName = null, bool isExceptionHandled = true)
			: this(FindRoot(owner).FileName, nodeName, isExceptionHandled)
		{
			Root = FindRoot(owner);
			Parent = owner;
			_nodePath = CombineNodePath(Parent._nodePath, _nodeName);
			_fileInfo = Root._fileInfo;
			_contextDocument = Root._contextDocument;
		}

		private void OnConfigFileChanged(object sender, FileSystemEventArgs e)
		{
			Root._hasRead = false;
		}

		private void LoadConfigFile()
		{
			Configurator configurator = default(Configurator);
			while (true)
			{
				if (-1 != 0)
				{
					if (this == Root)
					{
						break;
					}
					configurator = FindRoot(this);
				}
				if (0 == 0 || 1 == 0)
				{
					if (-1 != 0)
					{
						configurator._contextDocument.Load(configurator._fileInfo.FullName);
						return;
					}
					continue;
				}
				return;
			}
			_contextDocument.Load(_fileInfo.FullName);
		}

		[AsyncStateMachine(typeof(_003CReadConfigsAsync_003Ed__28))]
		private Task ReadConfigsAsync()
		{
			_003CReadConfigsAsync_003Ed__28 stateMachine = default(_003CReadConfigsAsync_003Ed__28);
			stateMachine._003C_003E4__this = this;
			AsyncTaskMethodBuilder asyncTaskMethodBuilder;
			do
			{
				if (2 != 0)
				{
					stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
					stateMachine._003C_003E1__state = -1;
				}
				asyncTaskMethodBuilder = stateMachine._003C_003Et__builder;
			}
			while (false);
			asyncTaskMethodBuilder.Start(ref stateMachine);
			return stateMachine._003C_003Et__builder.Task;
		}

		[AsyncStateMachine(typeof(_003CSaveConfigsAsync_003Ed__29))]
		private Task SaveConfigsAsync()
		{
			_003CSaveConfigsAsync_003Ed__29 stateMachine = default(_003CSaveConfigsAsync_003Ed__29);
			stateMachine._003C_003E4__this = this;
			AsyncTaskMethodBuilder asyncTaskMethodBuilder;
			do
			{
				if (2 != 0)
				{
					stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
					stateMachine._003C_003E1__state = -1;
				}
				asyncTaskMethodBuilder = stateMachine._003C_003Et__builder;
			}
			while (false);
			asyncTaskMethodBuilder.Start(ref stateMachine);
			return stateMachine._003C_003Et__builder.Task;
		}

		[AsyncStateMachine(typeof(_003CGetAsync_003Ed__30))]
		public Task<string> GetAsync(string key, string defaultValue = null)
		{
			_003CGetAsync_003Ed__30 stateMachine = default(_003CGetAsync_003Ed__30);
			stateMachine._003C_003E4__this = this;
			if (4 != 0)
			{
				stateMachine.key = key;
			}
			stateMachine.defaultValue = defaultValue;
			stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder<string>.Create();
			stateMachine._003C_003E1__state = -1;
			AsyncTaskMethodBuilder<string> asyncTaskMethodBuilder = stateMachine._003C_003Et__builder;
			asyncTaskMethodBuilder.Start(ref stateMachine);
			return stateMachine._003C_003Et__builder.Task;
		}

		[AsyncStateMachine(typeof(_003CSetAsync_003Ed__31))]
		public Task SetAsync(string key, string value)
		{
			_003CSetAsync_003Ed__31 stateMachine = default(_003CSetAsync_003Ed__31);
			stateMachine._003C_003E4__this = this;
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

		[AsyncStateMachine(typeof(_003CGetArrayAsync_003Ed__32))]
		public Task<string[]> GetArrayAsync(string key, string[] defaultValue = null)
		{
			_003CGetArrayAsync_003Ed__32 stateMachine = default(_003CGetArrayAsync_003Ed__32);
			stateMachine._003C_003E4__this = this;
			if (4 != 0)
			{
				stateMachine.key = key;
			}
			stateMachine.defaultValue = defaultValue;
			stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder<string[]>.Create();
			stateMachine._003C_003E1__state = -1;
			AsyncTaskMethodBuilder<string[]> asyncTaskMethodBuilder = stateMachine._003C_003Et__builder;
			asyncTaskMethodBuilder.Start(ref stateMachine);
			return stateMachine._003C_003Et__builder.Task;
		}

		[AsyncStateMachine(typeof(_003CSetArrayAsync_003Ed__33))]
		public Task SetArrayAsync(string key, IEnumerable<string> values)
		{
			_003CSetArrayAsync_003Ed__33 stateMachine = default(_003CSetArrayAsync_003Ed__33);
			stateMachine._003C_003E4__this = this;
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

		private string CombineNodePath(string path1, string path2)
		{
			return path1 + "/" + path2;
		}

		private static Configurator FindRoot(Configurator node)
		{
			while (node.Parent != null)
			{
				node = node.Parent;
			}
			return node;
		}

		private void CreateNewXmlFile()
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(FileName, Encoding.UTF8);
			try
			{
				if (false)
				{
					goto IL_004c;
				}
				xmlTextWriter.Formatting = Formatting.Indented;
				goto IL_0061;
				IL_0029:
				xmlTextWriter.WriteStartElement(Root._nodeName);
				if (7 != 0)
				{
					xmlTextWriter.WriteEndElement();
					if (false)
					{
						goto IL_0061;
					}
					xmlTextWriter.Flush();
				}
				goto IL_004c;
				IL_0061:
				if (true)
				{
					xmlTextWriter.WriteStartDocument(false);
				}
				goto IL_0029;
				IL_004c:
				if (false)
				{
					goto IL_0029;
				}
				xmlTextWriter.Close();
			}
			finally
			{
				do
				{
					((IDisposable)xmlTextWriter)?.Dispose();
				}
				while (false);
			}
			_fileInfo = new FileInfo(FileName);
		}

		private XmlNode CreateAndSelectNode(string key)
		{
			XmlNode xmlNode = _contextDocument.SelectSingleNode(key);
			string[] array;
			if (0 == 0)
			{
				if (xmlNode != null || 1 == 0)
				{
					goto IL_00a1;
				}
				array = key.Split(NodeSeparatorChars);
			}
			xmlNode = _contextDocument.SelectSingleNode(Root._nodeName);
			int num = default(int);
			while (true)
			{
				IL_0055:
				if (8 != 0)
				{
					num = 1;
				}
				while (num < array.Length)
				{
					if (7 == 0)
					{
						goto IL_0055;
					}
					XmlNode xmlNode2 = xmlNode.SelectSingleNode(array[num]);
					if (xmlNode2 != null)
					{
						xmlNode = xmlNode2;
						if (false)
						{
							continue;
						}
					}
					else
					{
						if (false)
						{
							continue;
						}
						xmlNode.AppendChild(_contextDocument.CreateNode(XmlNodeType.Element, array[num], null));
						xmlNode = xmlNode.SelectSingleNode(array[num]);
					}
					num++;
				}
				break;
			}
			goto IL_00a1;
			IL_00a1:
			return xmlNode;
		}

		[Obsolete("同步读写配置的方法将于 2017年6月18日 删除；请在方法名称后加上 Async 替代。")]
		private void ReadConfigs()
		{
			bool flag = _fileInfo.Exists;
			do
			{
				if (!flag)
				{
					CreateNewXmlFile();
				}
				flag = _isExceptionHandled;
			}
			while (-1 == 0);
			if (flag)
			{
				try
				{
					LoadConfigFile();
				}
				catch (Exception)
				{
					_fileInfo.Delete();
					CreateNewXmlFile();
				}
			}
			else
			{
				LoadConfigFile();
			}
			Root._hasRead = true;
		}

		[Obsolete("同步读写配置的方法将于 2017年6月18日 删除；请在方法名称后加上 Async 替代。")]
		private void SaveConfigs()
		{
			if (1 == 0 || this != Root)
			{
				FindRoot(this)._contextDocument.Save(_fileInfo.FullName);
				if (0 == 0)
				{
					return;
				}
			}
			_contextDocument.Save(_fileInfo.FullName);
		}

		[Obsolete("同步读写配置的方法将于 2017年6月18日 删除；请在方法名称后加上 Async 替代。")]
		public string Get(string key, string defaultValue = null)
		{
			while (true)
			{
				if (!Root._hasRead)
				{
					Root.ReadConfigs();
				}
				while (6 != 0)
				{
					XmlNode xmlNode = _contextDocument.SelectSingleNode(CombineNodePath(_nodePath, key));
					if (xmlNode != null)
					{
						if (6 != 0)
						{
							return xmlNode.InnerText;
						}
					}
					else if (defaultValue == null)
					{
						throw new KeyNotFoundException($"Key {key} not found.");
					}
					if (0 == 0)
					{
						return defaultValue;
					}
				}
			}
		}

		[Obsolete("同步读写配置的方法将于 2017年6月18日 删除；请在方法名称后加上 Async 替代。")]
		public void Set(string key, string value)
		{
			while (true)
			{
				bool flag = Root._hasRead;
				if (4 != 0)
				{
					if (!flag)
					{
						Root.ReadConfigs();
					}
					key = CombineNodePath(_nodePath, key);
					CreateAndSelectNode(key).InnerText = value;
					goto IL_0030;
				}
				goto IL_0036;
				IL_0036:
				if (flag)
				{
					Root.SaveConfigs();
				}
				if (false)
				{
					continue;
				}
				if (0 == 0)
				{
					break;
				}
				goto IL_0030;
				IL_0030:
				flag = IsAutoSave;
				goto IL_0036;
			}
		}

		[Obsolete("同步读写配置的方法将于 2017年6月18日 删除；请在方法名称后加上 Async 替代。")]
		public string[] GetArray(string key, string[] defaultValue = null)
		{
			if (!Root._hasRead)
			{
				Root.ReadConfigs();
			}
			XmlNode xmlNode = _contextDocument.SelectSingleNode(CombineNodePath(_nodePath, key));
			if (xmlNode != null)
			{
				return (from XmlNode child in xmlNode.ChildNodes
					select child.InnerText).ToArray();
			}
			if (defaultValue != null)
			{
				return defaultValue;
			}
			throw new KeyNotFoundException($"Key {key} not found.");
		}

		[Obsolete("同步读写配置的方法将于 2017年6月18日 删除；请在方法名称后加上 Async 替代。")]
		public void SetArray(string key, IEnumerable<string> values)
		{
			if (true && !Root._hasRead)
			{
				Root.ReadConfigs();
			}
			if (false)
			{
				goto IL_0052;
			}
			key = CombineNodePath(_nodePath, key);
			XmlNode xmlNode = default(XmlNode);
			if (4 != 0)
			{
				xmlNode = CreateAndSelectNode(key);
				goto IL_004c;
			}
			goto IL_00b1;
			IL_00b1:
			if (8 != 0)
			{
				return;
			}
			goto IL_004c;
			IL_004c:
			xmlNode.RemoveAll();
			goto IL_0052;
			IL_0052:
			using (IEnumerator<string> enumerator = values.GetEnumerator())
			{
				if (0 == 0)
				{
				}
				while (enumerator.MoveNext())
				{
					string current;
					XmlNode xmlNode2;
					do
					{
						current = enumerator.Current;
						xmlNode2 = _contextDocument.CreateNode(XmlNodeType.Element, "value", null);
					}
					while (-1 == 0);
					xmlNode2.InnerText = current;
					xmlNode.AppendChild(xmlNode2);
				}
			}
			if (IsAutoSave)
			{
				Root.SaveConfigs();
			}
			goto IL_00b1;
		}

		static Configurator()
		{
			do
			{
				if (0 == 0)
				{
					NodeSeparatorChars = new char[2] { '/', '\\' };
				}
			}
			while (false);
		}
	}
}
