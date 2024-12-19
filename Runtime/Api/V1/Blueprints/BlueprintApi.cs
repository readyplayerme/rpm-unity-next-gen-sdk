using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace ReadyPlayerMe.Api.V1
{
    public class BlueprintApi : WebApi
    {
        public virtual async Task<BlueprintListResponse> ListAsync(BlueprintListRequest request, CancellationToken cancellationToken = default)
        {
            var apiRequest = new ApiRequest<string>()
            {
                Url = $"{Settings.ApiBaseUrl}/v1/public/blueprints?applicationId={request.ApplicationId}&archived={request.Archived.ToString().ToLower()}",
                Method = UnityWebRequest.kHttpVerbGET,
                Headers = new Dictionary<string, string>()
                {
                    { "Content-Type", "application/json" },
                }
            };
            return await Dispatch<BlueprintListResponse>(apiRequest, cancellationToken);
        }
    }
}