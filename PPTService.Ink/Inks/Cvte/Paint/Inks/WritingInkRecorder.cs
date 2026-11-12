using System.Collections.Generic;
using System.Windows.Input;

namespace Cvte.Paint.Inks
{
	internal class WritingInkRecorder
	{
		private Dictionary<int, StrokePointCollection> StrokePointDetailsList { get; } = new Dictionary<int, StrokePointCollection>();

		private StrokePointCollection MouseStrokePointDetailsList { get; set; }

		public StrokePointCollection PopStrokePointDetails(int id)
		{
			if (IsMouse(id))
			{
				StrokePointCollection mouseStrokePointDetailsList = MouseStrokePointDetailsList;
				MouseStrokePointDetailsList = null;
				return mouseStrokePointDetailsList;
			}
			StrokePointCollection value;
			if (StrokePointDetailsList.TryGetValue(id, out value))
			{
				StrokePointDetailsList.Remove(id);
			}
			return value;
		}

		public void CollectStrokePointDetails(DynamicRenderer.StylusPointCollectionInfo stylusPointCollectionInfo)
		{
			CollectStrokePointDetails(stylusPointCollectionInfo.StrokeInfo.StylusDeviceId, stylusPointCollectionInfo.StylusPlugInCollection);
		}

		public void CollectStrokePointDetails(int id, StylusPointCollection stylusPlugInCollection)
		{
			if (IsMouse(id))
			{
				if (MouseStrokePointDetailsList == null)
				{
					MouseStrokePointDetailsList = new StrokePointCollection();
				}
				AddStrokePointCollection(MouseStrokePointDetailsList, stylusPlugInCollection);
				return;
			}
			StrokePointCollection value;
			if (!StrokePointDetailsList.TryGetValue(id, out value))
			{
				value = new StrokePointCollection();
				StrokePointDetailsList[id] = value;
			}
			AddStrokePointCollection(value, stylusPlugInCollection);
		}

		public void RemoveCollectStrokePointDetail(int strokeInfoStylusDeviceId)
		{
			if (IsMouse(strokeInfoStylusDeviceId))
			{
				MouseStrokePointDetailsList = null;
			}
			else
			{
				StrokePointDetailsList.Remove(strokeInfoStylusDeviceId);
			}
		}

		private static bool IsMouse(int id)
		{
			return id == -1;
		}

		private static void AddStrokePointCollection(StrokePointCollection strokePointCollection, StylusPointCollection stylusPointCollection)
		{
			foreach (StylusPoint item in stylusPointCollection)
			{
				strokePointCollection.Add(item);
			}
		}
	}
}
