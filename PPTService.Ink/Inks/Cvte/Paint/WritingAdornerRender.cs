using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Input;
using Cvte.Paint.Framework;
using Cvte.Paint.Inks;

namespace Cvte.Paint
{
	public class WritingAdornerRender : IAdornerRender
	{
		private readonly Dictionary<string, DynamicInk> _dynamicInks = new Dictionary<string, DynamicInk>();

		public Slide CurrentSlide { get; set; }

		private AdornerLayer DynamicInkLayer => AdornerLayer.GetAdornerLayer(CurrentSlide);

		public void AddDynamicInk(DynamicInk newInk, int devideId, StylusPoint stylusPoint, string inkGuid = "")
		{
			AddDynamicInk(newInk, devideId.ToString(), stylusPoint, inkGuid);
		}

		public void AddDynamicInk(DynamicInk newInk, string devideId, StylusPoint stylusPoint, string inkGuid = "")
		{
			newInk.DeviceId = devideId;
			_dynamicInks.Add(devideId, newInk);
			DynamicInkLayer?.Add(newInk);
			newInk.AppendInkPoint(stylusPoint);
		}

		public void DeleteDynamicInk(int deviceId, bool deleteVisual = true)
		{
			DeleteDynamicInk(deviceId.ToString(), deleteVisual);
		}

		public void DeleteDynamicInk(string deviceId, bool deleteVisual = true)
		{
			if (deleteVisual)
			{
				DynamicInkLayer?.Remove(_dynamicInks[deviceId]);
			}
			_dynamicInks.Remove(deviceId);
		}

		public void DeleteDynamicInk(DynamicInk dynamicInk)
		{
			DeleteDynamicInk(dynamicInk.DeviceId);
		}

		public void DeleteAllDynamicInks()
		{
			string[] array = _dynamicInks.Keys.ToArray();
			foreach (string deviceId in array)
			{
				DeleteDynamicInk(deviceId);
			}
		}

		public void DeleteAllLocalDynamicInks()
		{
			KeyValuePair<string, DynamicInk>[] array = _dynamicInks.Where((KeyValuePair<string, DynamicInk> k) => k.Value.IsLocalInk).ToArray();
			foreach (KeyValuePair<string, DynamicInk> keyValuePair in array)
			{
				DeleteDynamicInk(keyValuePair.Key);
			}
		}

		public DynamicInk[] GetAllDynamicInks()
		{
			return _dynamicInks.Values.ToArray();
		}

		public DynamicInk[] GetAllLocalDynamicInks()
		{
			return _dynamicInks.Values.Where((DynamicInk d) => d.IsLocalInk).ToArray();
		}

		public DynamicInk GetDynamicInk(int id)
		{
			return GetDynamicInk(id.ToString());
		}

		public DynamicInk GetDynamicInk(string id)
		{
			DynamicInk value;
			if (_dynamicInks.TryGetValue(id, out value))
			{
				return value;
			}
			return null;
		}

		public void InterruptRender()
		{
			if (CurrentSlide != null && _dynamicInks.Any())
			{
				DeleteAllDynamicInks();
			}
		}
	}
}
