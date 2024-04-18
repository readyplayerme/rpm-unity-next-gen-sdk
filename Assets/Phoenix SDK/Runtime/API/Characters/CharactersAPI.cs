using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ReadyPlayerMe
{
    public class CharactersAPI: ApiBase<CharactersResponse>
    {
        private const string API = "v1/characters";

        public async override Task<CharactersResponse> Post(IRequest request)
        {
            return await WebRequest.Dispatch<CharactersResponse>(new WebRequestData()
            {
                Url = $"{Constants.BASE_URL}/{API}",
                Method = HttpMethod.POST,
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", Constants.TOKEN },
                    { "Content-Type", "application/json" }
                },
                Payload = JsonConvert.SerializeObject(new PayloadData
                {
                    data = request
                })
            });
        }
        
        public async Task<CharactersResponse> Preview(string characterId, string query)
        {
            return await WebRequest.Dispatch<CharactersResponse>(new WebRequestData()
                {
                    Url = $"{Constants.BASE_URL}/{API}/{characterId}/preview?{query}",
                    Method = HttpMethod.GET,
                    Headers = new Dictionary<string, string>
                    {
                        { "Authorization", Constants.TOKEN },
                        { "Content-Type", "application/json" }
                    }
                }
            );
        }
        
        public async override Task<CharactersResponse> Patch(string characterId, IRequest request)
        {
            return await WebRequest.Dispatch<CharactersResponse>(new WebRequestData()
                {
                    Url = $"{Constants.BASE_URL}/{API}/{characterId}",
                    Method = HttpMethod.PATCH,
                    Headers = new Dictionary<string, string>
                    {
                        { "Authorization", Constants.TOKEN },
                        { "Content-Type", "application/json" }
                    },
                    Payload = JsonConvert.SerializeObject(new PayloadData
                    {
                        data = request
                    })
                }
            );
        }
    }
}
