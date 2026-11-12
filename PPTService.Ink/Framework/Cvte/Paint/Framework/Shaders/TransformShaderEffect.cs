using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;

namespace Cvte.Paint.Framework.Shaders
{
	public class TransformShaderEffect : ShaderEffect
	{
		public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(TransformShaderEffect), 0);

		private static readonly DependencyPropertyKey MPropertyKey = DependencyProperty.RegisterReadOnly("M", typeof(Point4D), typeof(TransformShaderEffect), new UIPropertyMetadata(new Point4D(1.0, 0.0, 0.0, 1.0), ShaderEffect.PixelShaderConstantCallback(0)));

		private static readonly DependencyPropertyKey OffsetPropertyKey = DependencyProperty.RegisterReadOnly("Offset", typeof(Vector), typeof(TransformShaderEffect), new UIPropertyMetadata(new Vector(0.0, 0.0), ShaderEffect.PixelShaderConstantCallback(1)));

		public static readonly DependencyProperty TransformProperty = DependencyProperty.Register("Transform", typeof(GeneralTransform), typeof(TransformShaderEffect), new PropertyMetadata(System.Windows.Media.Transform.Identity, OnTransformPropertyChanged));

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

		public GeneralTransform Transform
		{
			get
			{
				return (GeneralTransform)GetValue(TransformProperty);
			}
			set
			{
				SetValue(TransformProperty, value);
			}
		}

		public TransformShaderEffect()
		{
			PixelShader = new PixelShader
			{
				UriSource = new Uri("pack://application:,,,/" + Assembly.GetExecutingAssembly().FullName + ";component/utils_/shaders/transformshadereffect.ps", UriKind.Absolute)
			};
			UpdateShaderValue(InputProperty);
			UpdateShaderValue(MPropertyKey.DependencyProperty);
			UpdateShaderValue(OffsetPropertyKey.DependencyProperty);
		}

		private static void OnTransformPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Matrix value = ((Transform)e.NewValue).Value;
			if (!object.Equals(value.M11 * value.M22 - value.M12 * value.M21, 0.0))
			{
				value.Invert();
				d.SetValue(MPropertyKey, new Point4D(value.M11, value.M12, value.M21, value.M22));
				d.SetValue(OffsetPropertyKey, new Vector(value.OffsetX, value.OffsetY));
			}
		}
	}
}
