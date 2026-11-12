using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Cvte.Paint.Framework.Shaders
{
	public class ScaleShaderEffect : ShaderEffect
	{
		public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(ScaleShaderEffect), 0);

		public static readonly DependencyProperty VirtualProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Virtual", typeof(ScaleShaderEffect), 1);

		public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(ScaleShaderEffect), new UIPropertyMetadata(1.0, ShaderEffect.PixelShaderConstantCallback(0)));

		public static readonly DependencyProperty CenterProperty = DependencyProperty.Register("Center", typeof(Point), typeof(ScaleShaderEffect), new UIPropertyMetadata(new Point(0.5, 0.5), ShaderEffect.PixelShaderConstantCallback(1)));

		public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register("Offset", typeof(Vector), typeof(ScaleShaderEffect), new UIPropertyMetadata(new Vector(0.0, 0.0), ShaderEffect.PixelShaderConstantCallback(2)));

		private static readonly DependencyProperty TopLeftProperty = DependencyProperty.Register("TopLeft", typeof(Point), typeof(ScaleShaderEffect), new UIPropertyMetadata(new Point(0.0, 0.0), ShaderEffect.PixelShaderConstantCallback(3)));

		private static readonly DependencyProperty BottomRightProperty = DependencyProperty.Register("BottomRight", typeof(Point), typeof(ScaleShaderEffect), new UIPropertyMetadata(new Point(1.0, 1.0), ShaderEffect.PixelShaderConstantCallback(4)));

		public static readonly DependencyProperty VirtualBoundsProperty = DependencyProperty.Register("VirtualBounds", typeof(Rect), typeof(ScaleShaderEffect), new PropertyMetadata(new Rect(0.0, 0.0, 1.0, 1.0), OnVirtualBoundsChanged));

		public Brush Input
		{
			get
			{
				return (Brush)GetValue(InputProperty);
			}
			set
			{
				SetValue(InputProperty, value);
			}
		}

		public Brush Virtual
		{
			get
			{
				return (Brush)GetValue(VirtualProperty);
			}
			set
			{
				SetValue(VirtualProperty, value);
			}
		}

		public double Scale
		{
			get
			{
				return (double)GetValue(ScaleProperty);
			}
			set
			{
				SetValue(ScaleProperty, value);
			}
		}

		public Point Center
		{
			get
			{
				return (Point)GetValue(CenterProperty);
			}
			set
			{
				SetValue(CenterProperty, value);
			}
		}

		public Vector Offset
		{
			get
			{
				return (Vector)GetValue(OffsetProperty);
			}
			set
			{
				SetValue(OffsetProperty, value);
			}
		}

		public Rect VirtualBounds
		{
			get
			{
				return (Rect)GetValue(VirtualBoundsProperty);
			}
			set
			{
				SetValue(VirtualBoundsProperty, value);
			}
		}

		public ScaleShaderEffect()
		{
			PixelShader = new PixelShader
			{
				UriSource = new Uri("pack://application:,,,/" + Assembly.GetExecutingAssembly().FullName + ";component/utils_/shaders/scaleshadereffect.ps", UriKind.Absolute)
			};
			UpdateShaderValue(InputProperty);
			UpdateShaderValue(VirtualProperty);
			UpdateShaderValue(ScaleProperty);
			UpdateShaderValue(CenterProperty);
			UpdateShaderValue(OffsetProperty);
			UpdateShaderValue(TopLeftProperty);
			UpdateShaderValue(BottomRightProperty);
		}

		private static void OnVirtualBoundsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Rect rect = (Rect)e.NewValue;
			d.SetValue(TopLeftProperty, rect.TopLeft);
			d.SetValue(BottomRightProperty, rect.BottomRight);
		}
	}
}
