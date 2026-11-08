using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using PPTService.Collection;

namespace PPTService
{
	public partial class RecommendationWindow : Window, IComponentConnector
	{
		private static readonly string MoreInfoWebsite = "http://www.seewoedu.com/";

		private static readonly string EducationInfoWebsite = "http://bbs.seewo.com/";

		private static readonly string DownloadWebsiteEasiNote5 = "http://e.seewo.com/download/file?code=EasiNote5";

		public RecommendationWindow()
		{
			InitializeComponent();
			VersionTextBlock.Text = App.Version;
			PptOperator.Instance.ShowEnd += Instance_ShowEnd;
		}

		private void Instance_ShowEnd(object sender, EventArgs e)
		{
			SyntacticSugar.CurrentDispatcherBeginInvoke(Close);
		}

		private void CloseButton_OnClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void MoreInfoButton_OnClick(object sender, RoutedEventArgs e)
		{
			FridayReporter.ReportEventImmediately(EventID.RecommendationWindowMoreInfo);
			ExploreHelper.OpenLink(MoreInfoWebsite);
			Close();
		}

		private void EducationInfoButton_OnClick(object sender, RoutedEventArgs e)
		{
			FridayReporter.ReportEventImmediately(EventID.RecommendationWindowEducationInfo);
			ExploreHelper.OpenLink(EducationInfoWebsite);
			Close();
		}

		private void DownloadButton_OnClick(object sender, RoutedEventArgs e)
		{
			FridayReporter.ReportEventImmediately(EventID.RecommendationWindowDownload);
			ExploreHelper.OpenLink(DownloadWebsiteEasiNote5);
			Close();
		}

		private void BackgroundButton_OnClick(object sender, RoutedEventArgs e)
		{
			(Resources["Storyboard.Shake"] as Storyboard)?.Begin();
		}
	}
}
