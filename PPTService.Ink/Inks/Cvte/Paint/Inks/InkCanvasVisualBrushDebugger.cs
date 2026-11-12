using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Cvte.Paint.Framework;

namespace Cvte.Paint.Inks
{
	internal class InkCanvasVisualBrushDebugger
	{
		private bool _isStopping;

		private readonly Board _board;

		private static readonly DependencyProperty DebuggerProperty = DependencyProperty.RegisterAttached("Debugger", typeof(InkCanvasVisualBrushDebugger), typeof(InkCanvasVisualBrushDebugger), new PropertyMetadata((object)null));

		private InkCanvasVisualBrushDebugger(Board board)
		{
			_board = board;
		}

		public static async void AttachDebugInfo(InkCanvas inkCanvas)
		{
			for (int i = 0; i < 10; i++)
			{
				for (DependencyObject parent = VisualTreeHelper.GetParent(inkCanvas); parent != null; parent = VisualTreeHelper.GetParent(parent))
				{
					Board board = parent as Board;
					if (board != null)
					{
						SetDebugger(board);
						return;
					}
				}
				await Task.Delay(TimeSpan.FromSeconds(1.0));
			}
		}

		private static void SetDebugger(Board board)
		{
			if (GetDebugger(board) == null)
			{
				InkCanvasVisualBrushDebugger inkCanvasVisualBrushDebugger = new InkCanvasVisualBrushDebugger(board);
				inkCanvasVisualBrushDebugger.Start();
				SetDebugger(board, inkCanvasVisualBrushDebugger);
			}
		}

		private async void Start()
		{
			_board.Unloaded += delegate
			{
				Stop();
			};
			while (!_isStopping)
			{
				await Task.Delay(TimeSpan.FromSeconds(1.0)).ConfigureAwait(false);
				if (await CheckDispatcherHangAsync(_board.Dispatcher))
				{
					CheckAttachedVisualBrush();
				}
				else
				{
					await _board.Dispatcher.InvokeAsync(CheckAttachedVisualBrush);
				}
			}
		}

		private void CheckAttachedVisualBrush()
		{
			CheckAttachedVisualBrush(_board);
			foreach (Slide slide in _board.Slides)
			{
				CheckAttachedVisualBrush(slide);
			}
		}

		private void CheckAttachedVisualBrush(Visual visual)
		{
			List<VisualBrush> visualBrushes = GetVisualBrushes(visual);
			if (visualBrushes.Count > 0)
			{
				throw new InvalidVisualBrushOperationException("在开启高性能笔迹的时候，禁止给 Board 或 Slide 设置 VisualBrush 以防止锁主线程。仅调试下抛出，请找德熙。可以在此异常里面的属性了解有哪个 VisualBrush 引用了", visualBrushes);
			}
		}

		private static async Task<bool> CheckDispatcherHangAsync(Dispatcher dispatcher)
		{
			TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
			dispatcher.InvokeAsync(() => taskCompletionSource.TrySetResult(true));
			await Task.WhenAny(taskCompletionSource.Task, Task.Delay(TimeSpan.FromSeconds(2.0)));
			return !taskCompletionSource.Task.IsCompleted;
		}

		private void Stop()
		{
			_isStopping = true;
			_board.ClearValue(DebuggerProperty);
		}

		private List<VisualBrush> GetVisualBrushes(Visual visual)
		{
			object value = typeof(Visual).GetField("CyclicBrushToChannelsMapField", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
			return (value.GetType().GetMethod("GetValue").Invoke(value, new object[1] { visual }) as IDictionary)?.Keys.OfType<VisualBrush>().ToList() ?? new List<VisualBrush>(0);
		}

		private static void SetDebugger(DependencyObject element, InkCanvasVisualBrushDebugger value)
		{
			element.SetValue(DebuggerProperty, value);
		}

		private static InkCanvasVisualBrushDebugger GetDebugger(DependencyObject element)
		{
			return (InkCanvasVisualBrushDebugger)element.GetValue(DebuggerProperty);
		}
	}
}
