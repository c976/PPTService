using System;
using System.Windows;

namespace Cvte.Paint.Framework
{
	[Obsolete("此段代码本不应该出现在这，后面需要把Alignment的耦合全部解除后删除")]
	public class AlignmentDependencyProperty
	{
		public static readonly DependencyProperty AlignmentEngineProperty = DependencyProperty.RegisterAttached("AlignmentEngine", typeof(IAlignmentEngine), typeof(IAlignmentEngine), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetAlignmentEngine(DependencyObject element, IAlignmentEngine value)
		{
			element.SetValue(AlignmentEngineProperty, value);
		}

		public static IAlignmentEngine GetAlignmentEngine(DependencyObject element)
		{
			return (IAlignmentEngine)element.GetValue(AlignmentEngineProperty);
		}
	}
}
