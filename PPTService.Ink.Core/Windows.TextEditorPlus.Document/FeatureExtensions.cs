namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	internal static class FeatureExtensions
	{
		public static Feature Clone(this IFeature feature)
		{
			Feature feature2 = feature as Feature;
			if (feature2 == null)
			{
				SingleFeature singleFeature = feature as SingleFeature;
				if (singleFeature == null)
				{
					ReadOnlyFeature readOnlyFeature = feature as ReadOnlyFeature;
					if (readOnlyFeature != null)
					{
						return readOnlyFeature;
					}
					return new Feature(feature);
				}
				return singleFeature.ToFeature();
			}
			return feature2.Clone();
		}
	}
}
