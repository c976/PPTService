#define TRACE
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Threading;

namespace Cvte.Paint.Inks
{
	public class DynamicRendererThreadManager : IDisposable
	{
		private class MediaContextReflection
		{
			private class Reflector : Binder
			{
				private class BinderState
				{
					public object[] args;
				}

				public override FieldInfo BindToField(BindingFlags bindingAttr, FieldInfo[] match, object value, CultureInfo culture)
				{
					if (match == null)
					{
						throw new ArgumentNullException("match");
					}
					for (int i = 0; i < match.Length; i++)
					{
						if (ChangeType(value, match[i].FieldType, culture) != null)
						{
							return match[i];
						}
					}
					return null;
				}

				public override MethodBase BindToMethod(BindingFlags bindingAttr, MethodBase[] match, ref object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] names, out object state)
				{
					BinderState binderState = new BinderState();
					object[] array = new object[args.Length];
					args.CopyTo(array, 0);
					binderState.args = array;
					state = binderState;
					if (match == null)
					{
						throw new ArgumentNullException();
					}
					for (int i = 0; i < match.Length; i++)
					{
						int num = 0;
						ParameterInfo[] parameters = match[i].GetParameters();
						if (args.Length != parameters.Length)
						{
							continue;
						}
						for (int j = 0; j < args.Length; j++)
						{
							if (names != null)
							{
								if (names.Length != args.Length)
								{
									throw new ArgumentException("names and args must have the same number of elements.");
								}
								for (int k = 0; k < names.Length; k++)
								{
									if (string.Compare(parameters[j].Name, names[k].ToString()) == 0)
									{
										args[j] = binderState.args[k];
									}
								}
							}
							if (ChangeType(args[j], parameters[j].ParameterType, culture) == null)
							{
								break;
							}
							num++;
						}
						if (num == args.Length)
						{
							return match[i];
						}
					}
					return null;
				}

				public override object ChangeType(object value, Type myChangeType, CultureInfo culture)
				{
					if (CanConvertFrom(value.GetType(), myChangeType))
					{
						return Convert.ChangeType(value, myChangeType);
					}
					return null;
				}

				public override void ReorderArgumentArray(ref object[] args, object state)
				{
					((BinderState)state).args.CopyTo(args, 0);
				}

				public override MethodBase SelectMethod(BindingFlags bindingAttr, MethodBase[] match, Type[] types, ParameterModifier[] modifiers)
				{
					if (match == null)
					{
						throw new ArgumentNullException("match");
					}
					for (int i = 0; i < match.Length; i++)
					{
						int num = 0;
						ParameterInfo[] parameters = match[i].GetParameters();
						if (types.Length == parameters.Length)
						{
							for (int j = 0; j < types.Length && CanConvertFrom(types[j], parameters[j].ParameterType); j++)
							{
								num++;
							}
							if (num == types.Length)
							{
								return match[i];
							}
						}
					}
					return null;
				}

				public override PropertyInfo SelectProperty(BindingFlags bindingAttr, PropertyInfo[] match, Type returnType, Type[] indexes, ParameterModifier[] modifiers)
				{
					if (match == null)
					{
						throw new ArgumentNullException("match");
					}
					for (int i = 0; i < match.Length; i++)
					{
						int num = 0;
						ParameterInfo[] indexParameters = match[i].GetIndexParameters();
						if (indexes.Length == indexParameters.Length)
						{
							for (int j = 0; j < indexes.Length && CanConvertFrom(indexes[j], indexParameters[j].ParameterType); j++)
							{
								num++;
							}
							if (num == indexes.Length && CanConvertFrom(returnType, match[i].PropertyType))
							{
								return match[i];
							}
						}
					}
					return null;
				}

				private bool CanConvertFrom(Type type1, Type type2)
				{
					if (type1.IsPrimitive && type2.IsPrimitive)
					{
						TypeCode typeCode = Type.GetTypeCode(type1);
						TypeCode typeCode2 = Type.GetTypeCode(type2);
						if (typeCode == typeCode2)
						{
							return true;
						}
						switch (typeCode)
						{
						case TypeCode.Char:
							switch (typeCode2)
							{
							case TypeCode.UInt16:
								return true;
							case TypeCode.UInt32:
								return true;
							case TypeCode.Int32:
								return true;
							case TypeCode.UInt64:
								return true;
							case TypeCode.Int64:
								return true;
							case TypeCode.Single:
								return true;
							case TypeCode.Double:
								return true;
							default:
								return false;
							}
						case TypeCode.Byte:
							switch (typeCode2)
							{
							case TypeCode.Char:
								return true;
							case TypeCode.UInt16:
								return true;
							case TypeCode.Int16:
								return true;
							case TypeCode.UInt32:
								return true;
							case TypeCode.Int32:
								return true;
							case TypeCode.UInt64:
								return true;
							case TypeCode.Int64:
								return true;
							case TypeCode.Single:
								return true;
							case TypeCode.Double:
								return true;
							default:
								return false;
							}
						case TypeCode.SByte:
							switch (typeCode2)
							{
							case TypeCode.Int16:
								return true;
							case TypeCode.Int32:
								return true;
							case TypeCode.Int64:
								return true;
							case TypeCode.Single:
								return true;
							case TypeCode.Double:
								return true;
							default:
								return false;
							}
						case TypeCode.UInt16:
							switch (typeCode2)
							{
							case TypeCode.UInt32:
								return true;
							case TypeCode.Int32:
								return true;
							case TypeCode.UInt64:
								return true;
							case TypeCode.Int64:
								return true;
							case TypeCode.Single:
								return true;
							case TypeCode.Double:
								return true;
							default:
								return false;
							}
						case TypeCode.Int16:
							switch (typeCode2)
							{
							case TypeCode.Int32:
								return true;
							case TypeCode.Int64:
								return true;
							case TypeCode.Single:
								return true;
							case TypeCode.Double:
								return true;
							default:
								return false;
							}
						case TypeCode.UInt32:
							switch (typeCode2)
							{
							case TypeCode.UInt64:
								return true;
							case TypeCode.Int64:
								return true;
							case TypeCode.Single:
								return true;
							case TypeCode.Double:
								return true;
							default:
								return false;
							}
						case TypeCode.Int32:
							switch (typeCode2)
							{
							case TypeCode.Int64:
								return true;
							case TypeCode.Single:
								return true;
							case TypeCode.Double:
								return true;
							default:
								return false;
							}
						case TypeCode.UInt64:
							switch (typeCode2)
							{
							case TypeCode.Single:
								return true;
							case TypeCode.Double:
								return true;
							default:
								return false;
							}
						case TypeCode.Int64:
							switch (typeCode2)
							{
							case TypeCode.Single:
								return true;
							case TypeCode.Double:
								return true;
							default:
								return false;
							}
						case TypeCode.Single:
							if (typeCode2 == TypeCode.Double)
							{
								return true;
							}
							return false;
						}
					}
					return false;
				}

				public static MethodInfo GetMethod(Type type, string name, Type[] types, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				{
					return (from mi in type.GetMethods(bindingAttr)
						where mi.Name == name
						select mi).Where((MethodInfo mi) =>
					{
						if (types == null)
						{
							return true;
						}
						ParameterInfo[] p = mi.GetParameters();
						return types.All((Type temp) => p.Any((ParameterInfo pm) => pm.ParameterType == temp));
					}).First();
				}
			}

			private static readonly MethodInfo _from;

			private static readonly MethodInfo _removeRenderComplete;

			private static readonly MethodInfo _addRenderComplete;

			private static Type MediaContext { get; }

			private static EventInfo RenderComplete { get; }

			static MediaContextReflection()
			{
				MediaContext = Assembly.GetAssembly(typeof(StylusPlugIn)).GetType("System.Windows.Media.MediaContext");
				_from = Reflector.GetMethod(MediaContext, "From", null);
				RenderComplete = MediaContext.GetEvent("RenderComplete", BindingFlags.Instance | BindingFlags.NonPublic);
				_addRenderComplete = RenderComplete.GetAddMethod(true);
				_removeRenderComplete = RenderComplete.GetRemoveMethod(true);
			}

			public static void AddRenderComplete(object mediaContext, EventHandler onRenderComplete)
			{
				_addRenderComplete.Invoke(mediaContext, new object[1] { onRenderComplete });
			}

			public static void RemoveRenderComplete(object mediaContext, EventHandler onRenderComplete)
			{
				_removeRenderComplete.Invoke(mediaContext, new object[1] { onRenderComplete });
			}

			public static object From(Dispatcher dispatcher)
			{
				return _from.Invoke(null, new object[1] { dispatcher });
			}
		}

		private AutoResetEvent _startupCompleted;

		private Dispatcher _mainDispatcher;

		public Dispatcher InkingDispatcher { get; private set; }

		private object InkingMediaContext { get; set; }

		private object MainMediaContext { get; set; }

		public event EventHandler InkingMediaContextRenderComplete
		{
			add
			{
				InkingDispatcher.VerifyAccess();
				MediaContextReflection.AddRenderComplete(InkingMediaContext, value);
			}
			remove
			{
				InkingDispatcher.VerifyAccess();
				MediaContextReflection.RemoveRenderComplete(InkingMediaContext, value);
			}
		}

		public event EventHandler MainMediaContextRenderComplete
		{
			add
			{
				_mainDispatcher.VerifyAccess();
				MediaContextReflection.AddRenderComplete(MainMediaContext, value);
			}
			remove
			{
				_mainDispatcher.VerifyAccess();
				MediaContextReflection.RemoveRenderComplete(MainMediaContext, value);
			}
		}

		public void StartUp()
		{
			_startupCompleted = new AutoResetEvent(false);
			Thread thread = new Thread(InkingThreadProc);
			thread.Priority = ThreadPriority.Highest;
			thread.SetApartmentState(ApartmentState.STA);
			thread.IsBackground = true;
			thread.Start();
			WaitForStartupCompleted();
			_mainDispatcher = Dispatcher.CurrentDispatcher;
			MainMediaContext = MediaContextReflection.From(_mainDispatcher);
			_startupCompleted.Close();
			_startupCompleted = null;
		}

		public void Close()
		{
			InkingDispatcher.InvokeShutdown();
		}

		private void WaitForStartupCompleted()
		{
			_startupCompleted.WaitOne();
		}

		[SecurityCritical]
		private void InkingThreadProc()
		{
			Thread.CurrentThread.Name = "DynamicRenderer";
			InkingDispatcher = Dispatcher.CurrentDispatcher;
			InkingMediaContext = MediaContextReflection.From(InkingDispatcher);
			InkingDispatcher.UnhandledException += InkingDispatcher_UnhandledException;
			_startupCompleted.Set();
			Dispatcher.Run();
		}

		private void InkingDispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			e.Handled = true;
			Trace.WriteLine(e);
			if (Debugger.IsAttached)
			{
				Debugger.Break();
			}
		}

		public void Dispose()
		{
			Close();
		}
	}
}
