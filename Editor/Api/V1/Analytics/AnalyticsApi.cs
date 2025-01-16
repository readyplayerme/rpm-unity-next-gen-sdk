using System.Collections.Generic;
using System.Threading.Tasks;
using PlayerZero.Api;
using PlayerZero.Editor.Api.V1.Analytics.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace PlayerZero.Editor.Api.V1.Analytics
{
    public sealed class AnalyticsApi : WebApi
    {
        private const string Resource = "analytics-events";
        
        private async Task<AnalyticsEventResponse> SendEventAsync(AnalyticsEventRequest request)
        {
            return await Dispatch<AnalyticsEventResponse, AnalyticsEventRequestBody>(
                new ApiRequest<AnalyticsEventRequestBody>()
                {
                    Url = $"{Settings.ApiBaseUrl}/v1/{Resource}",
                    Method = UnityWebRequest.kHttpVerbPOST,
                    Headers = new Dictionary<string, string>()
                    {
                        { "Content-Type", "application/json" },
                    },
                    Payload = request.Payload
                }
            );
        }

        public void SendEvent(AnalyticsEventRequest request)
        {
            if (Settings != null)
                request?.Payload?.Properties?.Add("applicationId", Settings.ApplicationId);

#pragma warning disable CS4014
            SendEventAsync(request);
#pragma warning restore CS4014
        }
    }
}