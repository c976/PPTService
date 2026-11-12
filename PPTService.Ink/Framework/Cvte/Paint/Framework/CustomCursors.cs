using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Resources;
using Cvte.Diagnostics.Trace;

namespace Cvte.Paint.Framework
{
	public static class CustomCursors
	{
		private static readonly Dictionary<CursorStyle, Cursor> CustomCursorDictionary = new Dictionary<CursorStyle, Cursor>
		{
			{
				CursorStyle.Default,
				Cursors.Arrow
			},
			{
				CursorStyle.None,
				Cursors.None
			}
		};

		private static readonly Dictionary<CursorStyle, string> CursorNames = new Dictionary<CursorStyle, string>
		{
			{
				CursorStyle.NS,
				"NS.cur"
			},
			{
				CursorStyle.NESW,
				"NESW.cur"
			},
			{
				CursorStyle.WE,
				"WE.cur"
			},
			{
				CursorStyle.NWSE,
				"NWSE.cur"
			},
			{
				CursorStyle.Rotate,
				"Rotation.cur"
			},
			{
				CursorStyle.Move,
				"Move.cur"
			},
			{
				CursorStyle.Text,
				"TextCorner.cur"
			},
			{
				CursorStyle.Origin,
				"ChangeOriginCur.cur"
			},
			{
				CursorStyle.AddNode,
				"AddNode.cur"
			},
			{
				CursorStyle.DragNode,
				"DragNode.cur"
			},
			{
				CursorStyle.DrawFree,
				"DrawFreeCur.cur"
			},
			{
				CursorStyle.Forbid,
				"Forbid.cur"
			},
			{
				CursorStyle.Selection,
				"Selection.cur"
			},
			{
				CursorStyle.Cross,
				"Cross.cur"
			},
			{
				CursorStyle.NS_S,
				"NS_S.cur"
			},
			{
				CursorStyle.WE_S,
				"WE_S.cur"
			},
			{
				CursorStyle.Grasp,
				"Grasp.cur"
			},
			{
				CursorStyle.Grasp_Pressed,
				"Grasp_Pressed.cur"
			}
		};

		public static ITracer CurrentTracer { get; set; }

		private static Cursor GetCursor(string name, ITracer tracer)
		{
			try
			{
				StreamResourceInfo resourceStream = Application.GetResourceStream(new Uri("/PPTService.Ink;component/resources/cursor/" + name, UriKind.Relative));
				return (resourceStream != null) ? new Cursor(resourceStream.Stream) : Cursors.Arrow;
			}
			catch (IOException exception)
			{
				tracer?.Trace(exception, null);
				return Cursors.Arrow;
			}
			catch (Exception exception2)
			{
				tracer?.Trace(exception2, null);
				return Cursors.Arrow;
			}
		}

		public static Cursor GetCursor(CursorStyle curStyle)
		{
			Cursor value;
			if (CustomCursorDictionary.TryGetValue(curStyle, out value))
			{
				return value;
			}
			string value2;
			if (!CursorNames.TryGetValue(curStyle, out value2))
			{
				return Cursors.Arrow;
			}
			Cursor cursor = GetCursor(value2, CurrentTracer);
			CustomCursorDictionary.Add(curStyle, cursor);
			return CustomCursorDictionary[curStyle];
		}
	}
}
