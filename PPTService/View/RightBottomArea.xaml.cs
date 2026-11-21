using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using PPTService.Collection;

namespace PPTService
{
	public partial class RightBottomArea : UserControl, IComponentConnector
	{
		public RightBottomArea()
		{
			InitializeComponent();
		}

		private void Previous_Click(object sender, RoutedEventArgs e)
		{
			PptOperator.Instance.PreviousPage();
			PlayHiddenStoryBoard();
			WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			FridayReporter.ReportEventImmediately(EventID.RightRegionPreviousPage);
		}

		private void Thumbnail_Click(object sender, RoutedEventArgs e)
		{
			if (PptVersionManager.IsWps)
			{
				return;
			}
			FridayReporter.ReportEventImmediately(EventID.RightRegionThumbnail);
			int count = ViewHelper.MainWindow.RightSlideListBox.Items.Count;
			if (count <= 0 || count != PptOperator.Instance.SlidesCount)
			{
				WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
				Log.WriteError($"ItemsCount  {count};  SlidesCount  {PptOperator.Instance.SlidesCount}");
				return;
			}
			ViewHelper.MainWindow.PlayHiddenLeftSlideManagerViewStoryboard();
			ViewHelper.MainWindow.PlayHiddenRightCenterButtonGridStoryboard();
			if (!ViewHelper.MainWindow.IsRightSlideManagerViewShowed)
			{
				ViewHelper.MainWindow.PlayStoryboard("ShowRightBottomSlideManagerViewStoryboard");
			}
			else
			{
				ViewHelper.MainWindow.PlayHiddenRightSlideManagerViewStoryboard();
			}
		}

		public void Next_Click(object sender, RoutedEventArgs e)
		{
			PptOperator.Instance.NextPage();
			PlayHiddenStoryBoard();
			WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			FridayReporter.ReportEventImmediately(EventID.RightRegionNextPage);
		}

		private void PlayHiddenStoryBoard()
		{
			ViewHelper.MainWindow.PlayHiddenLeftSlideManagerViewStoryboard();
			ViewHelper.MainWindow.PlayHiddenRightSlideManagerViewStoryboard();
			ViewHelper.MainWindow.PlayHiddenRightCenterButtonGridStoryboard();
		}

		public void PlayHiddenOrShowLeftAreaStoryBoard(bool isHidden)
		{
			double value = ActualHeight + ViewHelper.MainWindow.RootGrid.Margin.Bottom + 3.0;
			if (!isHidden)
			{
				Visibility = Visibility.Visible;
				value = 0.0;
			}
			DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
			DoubleKeyFrame keyFrame = new EasingDoubleKeyFrame(value, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(300.0)));
			doubleAnimationUsingKeyFrames.KeyFrames.Add(keyFrame);
			Storyboard.SetTarget(doubleAnimationUsingKeyFrames, RootGrid);
			Storyboard.SetTargetProperty(doubleAnimationUsingKeyFrames, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
			Storyboard storyboard = new Storyboard();
			storyboard.Children = new TimelineCollection { doubleAnimationUsingKeyFrames };
			storyboard.Completed += (object s, EventArgs e) =>
			{
				if (isHidden)
				{
					Visibility = Visibility.Collapsed;
				}
			};
			storyboard.Begin();
		}

	}
}
