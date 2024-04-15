/*
 
Structure of the response from the organization-base-models endpoint is as follows:
 
data: [{
    "_id": "65deedf94edcb3debbd61c51",
    "organizationId": "65deed404227732687e58565",
    "baseModelId": "65dc6f8c6934c255d54ede58",
    "assets": {},
    "glbUrl": "https://files.readyplayer.me/organization-base-model/glb/65deedf94edcb3debbd61c51/65deedf94edcb3debbd61c51-1709212203587.glb",
    "iconUrl": "https://renderapi.s3.amazonaws.com/klUPSS1kT_000.png",
    "metadata": [],
    "createdAt": "2024-02-28T08:25:29.200Z",
    "updatedAt": "2024-02-29T13:10:06.857Z",
    "__v": 0,
    "id": "65deedf94edcb3debbd61c51",
    "pendingJobs": 0,
    "failedJobs": 0
}],
"pagination": {
    "totalDocs": 84,
    "limit": 10,
    "totalPages": 9,
    "page": 1,
    "pagingCounter": 1,
    "hasPrevPage": false,
    "hasNextPage": true,
    "prevPage": 0,
    "nextPage": 2
}
    
*/

namespace ReadyPlayerMe.Phoenix.Data
{
    public struct BaseModelsResponse
    {
        public BaseModelsResponseData[] data;
        public Pagination pagination;
    }
    
    public struct BaseModelsResponseData
    {
        public string id;
        public string organizationId;
        public string baseModelId;
        public string glbUrl;
        public string iconUrl;
    }

    public struct Pagination
    {
        public int totalDocs;
        public int limit;
        public int totalPages;
        public int page;
        public int pagingCounter;
        public bool hasPrevPage;
        public bool hasNextPage;
        public int prevPage;
        public int nextPage;
    }
}