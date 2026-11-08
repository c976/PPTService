using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Cvte.Net.WebServices.EventReport.Models;
using Cvte.Net.WebServices.EventReport.Utils;

namespace Cvte.Net.WebServices.EventReport
{
	
	
	internal class PostEventWebService : WebService
	{
		public string FridayAppId { get; }

		private IEventCommonPropertiesProvider CommonPropertiesProvider { get; }

		private PostEventRequestBuilder RequestBuilder { get; }

		public bool IncludeDefaultProperties { get; set; }

		public PostEventWebService(string fridayAppId, IEventCommonPropertiesProvider commonPropertiesProvider)
		{
			if (string.IsNullOrWhiteSpace(fridayAppId))
			{
				throw new ArgumentNullException("fridayAppId");
			}
			FridayAppId = fridayAppId;
			if (commonPropertiesProvider == null)
			{
				throw new ArgumentNullException("commonPropertiesProvider");
			}
			CommonPropertiesProvider = commonPropertiesProvider;
			RequestBuilder = new PostEventRequestBuilder(fridayAppId, PostEventHelper.SessionId);
		}

		public async Task<PostEventResponse> PostEvent(IList<Event> events, IDictionary<string, object> commonProperties)
		{
			IDictionary<string, object> commonProperties2 = CommonPropertiesProvider.FilterCommonProperties(commonProperties);
			RequestBuilder.IncludeDefaultProperties = IncludeDefaultProperties;
			PostEventRequest request = RequestBuilder.Build(events, commonProperties2);
			return await PostAsync<PostEventResponse>(request).ConfigureAwait(false);
		}

		public async Task<PostEventResponse> PostEvent(PostEventRequestWrapper wrapper)
		{
			wrapper.Request.Init(wrapper.AppId, wrapper.SessionId, wrapper.TimeSpan);
			return await PostAsync<PostEventResponse>(wrapper.Request).ConfigureAwait(false);
		}
	}
}
