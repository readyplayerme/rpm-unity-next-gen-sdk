using ReadyPlayerMe.Phoenix;
using System.Threading.Tasks;
using ReadyPlayerMe.Phoenix.Data;
using System.Collections.Generic;

public class RefittedAssetEndpoint
{
    private const string ENDPOINT = "v1/refitted-assets";
    
    public async Task<RefittedAssetResponse> ListEquipableAssets(string organizationId = null)
    {
        return await WebRequest.Dispatch<RefittedAssetResponse>(new RequestData()
            {
                Url = $"{Constants.BASE_URL}/{ENDPOINT}" + (organizationId != null ? $"?organizationId={organizationId}" : ""),
                Method = HttpMethod.GET,
                Headers = new Dictionary<string, string>()
                {
                    { "Authorization", Constants.TOKEN }
                }
            }
        );
    }
}
