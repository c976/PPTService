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
				ConfiguredTaskAwaitable.ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					awaiter = Task.Run(() =>
					{
						if (!CS_0024_003C_003E8__locals8._fileInfo.Exists)
						{
							CS_0024_003C_003E8__locals8.CreateNewXmlFile();
						}
						if (CS_0024_003C_003E8__locals8._isExceptionHandled)
						{
							try
							{
								CS_0024_003C_003E8__locals8.LoadConfigFile();
								return;
							}
							catch (Exception)
							{
								CS_0024_003C_003E8__locals8._fileInfo.Delete();
								CS_0024_003C_003E8__locals8.CreateNewXmlFile();
								return;
							}
						}
						CS_0024_003C_003E8__locals8.LoadConfigFile();
					}).ConfigureAwait(false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						_003C_003E1__state = 0;
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaitable.ConfiguredTaskAwaiter);
					num = -1;
					_003C_003E1__state = -1;
				}
				awaiter.GetResult();
				awaiter = default(ConfiguredTaskAwaitable.ConfiguredTaskAwaiter);
				CS_0024_003C_003E8__locals8.Root._hasRead = true;
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
		private struct _003CSaveConfigsAsync_003Ed__29 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public Configurator _003C_003E4__this;

			private TaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				int num = _003C_003E1__state;
				Configurator configurator = _003C_003E4__this;
				TaskAwaiter awaiter;
				if (num != 0)
				{
					Configurator configurator2 = configurator;
					Configurator root = ((configurator == configurator.Root) ? configurator : FindRoot(configurator));
					awaiter = Task.Run(() =>
					{
						root._contextDocument.Save(configurator2._fileInfo.FullName);
					}).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
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
					num = -1;
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

		[StructLayout(LayoutKind.Auto)]
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
				TaskAwaiter awaiter;
				if (num != 0)
				{
					if (configurator.Root._hasRead)
					{
						goto IL_0089;
					}
					awaiter = configurator.Root.ReadConfigsAsync().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
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
					num = -1;
					_003C_003E1__state = -1;
				}
				awaiter.GetResult();
				awaiter = default(TaskAwaiter);
				goto IL_0089;
				IL_0089:
				XmlNode xmlNode = configurator._contextDocument.SelectSingleNode(configurator.CombineNodePath(configurator._nodePath, key));
				string result;
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
				int num = _003C_003E1__state;
				Configurator configurator = _003C_003E4__this;
				TaskAwaiter awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(TaskAwaiter);
						num = -1;
						_003C_003E1__state = -1;
						goto IL_011f;
					}
					if (configurator.Root._hasRead)
					{
						goto IL_00ae;
					}
					awaiter = configurator.Root.ReadConfigsAsync().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
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
					num = -1;
					_003C_003E1__state = -1;
				}
				awaiter.GetResult();
				awaiter = default(TaskAwaiter);
				goto IL_00ae;
				IL_00ae:
				key = configurator.CombineNodePath(configurator._nodePath, key);
				configurator.CreateAndSelectNode(key).InnerText = value;
				if (configurator.IsAutoSave)
				{
					awaiter = configurator.Root.SaveConfigsAsync().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 1;
						_003C_003E1__state = 1;
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_011f;
				}
				goto IL_0130;
				IL_011f:
				awaiter.GetResult();
				awaiter = default(TaskAwaiter);
				goto IL_0130;
				IL_0130:
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
				TaskAwaiter awaiter;
				if (num != 0)
				{
					if (configurator.Root._hasRead)
					{
						goto IL_0089;
					}
					awaiter = configurator.Root.ReadConfigsAsync().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
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
					num = -1;
					_003C_003E1__state = -1;
				}
				awaiter.GetResult();
				awaiter = default(TaskAwaiter);
				goto IL_0089;
				IL_0089:
				XmlNode xmlNode = configurator._contextDocument.SelectSingleNode(configurator.CombineNodePath(configurator._nodePath, key));
				string[] result;
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
				TaskAwaiter awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(TaskAwaiter);
						num = -1;
						_003C_003E1__state = -1;
						goto IL_0175;
					}
					if (configurator.Root._hasRead)
					{
						goto IL_00ae;
					}
					awaiter = configurator.Root.ReadConfigsAsync().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
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
					num = -1;
					_003C_003E1__state = -1;
				}
				awaiter.GetResult();
				awaiter = default(TaskAwaiter);
				goto IL_00ae;
				IL_00ae:
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
					if (num < 0)
					{
						enumerator?.Dispose();
					}
				}
				if (configurator.IsAutoSave)
				{
					awaiter = configurator.Root.SaveConfigsAsync().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 1;
						_003C_003E1__state = 1;
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0175;
				}
				goto IL_0186;
				IL_0175:
				awaiter.GetResult();
				awaiter = default(TaskAwaiter);
				goto IL_0186;
				IL_0186:
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

		private static readonly char[] NodeSeparatorChars = new char[2] { '/', '\\' };

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
			if (this == Root)
			{
				_contextDocument.Load(_fileInfo.FullName);
				return;
			}
			Configurator configurator = FindRoot(this);
			configurator._contextDocument.Load(configurator._fileInfo.FullName);
		}

		[AsyncStateMachine(typeof(_003CReadConfigsAsync_003Ed__28))]
		private Task ReadConfigsAsync()
		{
			_003CReadConfigsAsync_003Ed__28 stateMachine = default(_003CReadConfigsAsync_003Ed__28);
			stateMachine._003C_003E4__this = this;
			stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			stateMachine._003C_003E1__state = -1;
			AsyncTaskMethodBuilder asyncTaskMethodBuilder = stateMachine._003C_003Et__builder;
			asyncTaskMethodBuilder.Start(ref stateMachine);
			return stateMachine._003C_003Et__builder.Task;
		}

		[AsyncStateMachine(typeof(_003CSaveConfigsAsync_003Ed__29))]
		private Task SaveConfigsAsync()
		{
			_003CSaveConfigsAsync_003Ed__29 stateMachine = default(_003CSaveConfigsAsync_003Ed__29);
			stateMachine._003C_003E4__this = this;
			stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			stateMachine._003C_003E1__state = -1;
			AsyncTaskMethodBuilder asyncTaskMethodBuilder = stateMachine._003C_003Et__builder;
			asyncTaskMethodBuilder.Start(ref stateMachine);
			return stateMachine._003C_003Et__builder.Task;
		}

		[AsyncStateMachine(typeof(_003CGetAsync_003Ed__30))]
		public Task<string> GetAsync(string key, string defaultValue = null)
		{
			_003CGetAsync_003Ed__30 stateMachine = default(_003CGetAsync_003Ed__30);
			stateMachine._003C_003E4__this = this;
			stateMachine.key = key;
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
			stateMachine.key = key;
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
			stateMachine.key = key;
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
			stateMachine.key = key;
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
			using (XmlTextWriter xmlTextWriter = new XmlTextWriter(FileName, Encoding.UTF8))
			{
				xmlTextWriter.Formatting = Formatting.Indented;
				xmlTextWriter.WriteStartDocument(false);
				xmlTextWriter.WriteStartElement(Root._nodeName);
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.Flush();
				xmlTextWriter.Close();
			}
			_fileInfo = new FileInfo(FileName);
		}

		private XmlNode CreateAndSelectNode(string key)
		{
			XmlNode xmlNode = _contextDocument.SelectSingleNode(key);
			if (xmlNode == null)
			{
				string[] array = key.Split(NodeSeparatorChars);
				xmlNode = _contextDocument.SelectSingleNode(Root._nodeName);
				for (int i = 1; i < array.Length; i++)
				{
					XmlNode xmlNode2 = xmlNode.SelectSingleNode(array[i]);
					if (xmlNode2 != null)
					{
						xmlNode = xmlNode2;
						continue;
					}
					xmlNode.AppendChild(_contextDocument.CreateNode(XmlNodeType.Element, array[i], null));
					xmlNode = xmlNode.SelectSingleNode(array[i]);
				}
			}
			return xmlNode;
		}

		[Obsolete("同步读写配置的方法将于 2017年6月18日 删除；请在方法名称后加上 Async 替代。")]
		private void ReadConfigs()
		{
			if (!_fileInfo.Exists)
			{
				CreateNewXmlFile();
			}
			if (_isExceptionHandled)
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
			if (this == Root)
			{
				_contextDocument.Save(_fileInfo.FullName);
			}
			else
			{
				FindRoot(this)._contextDocument.Save(_fileInfo.FullName);
			}
		}

		[Obsolete("同步读写配置的方法将于 2017年6月18日 删除；请在方法名称后加上 Async 替代。")]
		public string Get(string key, string defaultValue = null)
		{
			if (!Root._hasRead)
			{
				Root.ReadConfigs();
			}
			XmlNode xmlNode = _contextDocument.SelectSingleNode(CombineNodePath(_nodePath, key));
			if (xmlNode != null)
			{
				return xmlNode.InnerText;
			}
			if (defaultValue == null)
			{
				throw new KeyNotFoundException($"Key {key} not found.");
			}
			return defaultValue;
		}

		[Obsolete("同步读写配置的方法将于 2017年6月18日 删除；请在方法名称后加上 Async 替代。")]
		public void Set(string key, string value)
		{
			if (!Root._hasRead)
			{
				Root.ReadConfigs();
			}
			key = CombineNodePath(_nodePath, key);
			CreateAndSelectNode(key).InnerText = value;
			if (IsAutoSave)
			{
				Root.SaveConfigs();
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
			if (defaultValue == null)
			{
				throw new KeyNotFoundException($"Key {key} not found.");
			}
			return defaultValue;
		}

		[Obsolete("同步读写配置的方法将于 2017年6月18日 删除；请在方法名称后加上 Async 替代。")]
		public void SetArray(string key, IEnumerable<string> values)
		{
			if (!Root._hasRead)
			{
				Root.ReadConfigs();
			}
			key = CombineNodePath(_nodePath, key);
			XmlNode xmlNode = CreateAndSelectNode(key);
			xmlNode.RemoveAll();
			foreach (string value in values)
			{
				XmlNode xmlNode2 = _contextDocument.CreateNode(XmlNodeType.Element, "value", null);
				xmlNode2.InnerText = value;
				xmlNode.AppendChild(xmlNode2);
			}
			if (IsAutoSave)
			{
				Root.SaveConfigs();
			}
		}
	}
}
