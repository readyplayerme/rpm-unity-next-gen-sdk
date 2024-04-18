using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ReadyPlayerMe
{
    /*
    public class RefittedAssetEndpoint: EndpointBase
    {
        private const string ENDPOINT = "v1/refitted-assets";
        
        public async Task<IResponse> Post(IRequest request)
        {
            return await WebRequest.Dispatch<RefittedAssetsResponse>(new WebRequestData()
                {
                    Url = $"{Constants.BASE_URL}/{ENDPOINT}",
                    Method = HttpMethod.POST,
                    Headers = new Dictionary<string, string>()
                    {
                        { "Authorization", Constants.TOKEN },
                        { "Content-Type", "application/json" }
                    },
                    Payload = JsonConvert.SerializeObject(request)
                }
            );
        }
        
        public async Task<IResponse> Get()
        {
            return await WebRequest.Dispatch<RefittedAssetsResponse>(new WebRequestData()
                {
                    Url = $"{Constants.BASE_URL}/{ENDPOINT}",
                    Method = HttpMethod.GET,
                    Headers = new Dictionary<string, string>()
                    {
                        { "Authorization", Constants.TOKEN },
                        { "Content-Type", "application/json" }
                    }
                }
            );
        }

        public async Task<IResponse> Get(string parameter)
        {
            return await WebRequest.Dispatch<RefittedAssetsResponse>(new WebRequestData()
                {
                    Url = $"{Constants.BASE_URL}/{ENDPOINT}/{parameter}",
                    Method = HttpMethod.GET,
                    Headers = new Dictionary<string, string>()
                    {
                        { "Authorization", Constants.TOKEN },
                        { "Content-Type", "application/json" }
                    }
                }
            );
        }

        public async Task<IResponse> Put(string parameter, IRequest request)
        {
            return await WebRequest.Dispatch<RefittedAssetsResponse>(new WebRequestData()
                {
                    Url = $"{Constants.BASE_URL}/{ENDPOINT}/{parameter}",
                    Method = HttpMethod.PUT,
                    Headers = new Dictionary<string, string>()
                    {
                        { "Authorization", Constants.TOKEN },
                        { "Content-Type", "application/json" }
                    },
                    Payload = JsonConvert.SerializeObject(request)
                }
            );
        }

        public async Task<IResponse> Delete(string parameter, IRequest request)
        {
            return await WebRequest.Dispatch<RefittedAssetsResponse>(new WebRequestData()
                {
                    Url = $"{Constants.BASE_URL}/{ENDPOINT}/{parameter}",
                    Method = HttpMethod.DELETE,
                    Headers = new Dictionary<string, string>()
                    {
                        { "Authorization", Constants.TOKEN },
                        { "Content-Type", "application/json" }
                    },
                    Payload = JsonConvert.SerializeObject(request)
                }
            );
        }

        public async Task<IResponse> Patch(string parameter, IRequest request)
        {
            return await WebRequest.Dispatch<RefittedAssetsResponse>(new WebRequestData()
                {
                    Url = $"{Constants.BASE_URL}/{ENDPOINT}/{parameter}",
                    Method = HttpMethod.PATCH,
                    Headers = new Dictionary<string, string>()
                    {
                        { "Authorization", Constants.TOKEN },
                        { "Content-Type", "application/json" }
                    },
                    Payload = JsonConvert.SerializeObject(request)
                }
            );
        }
    }
    */
}
