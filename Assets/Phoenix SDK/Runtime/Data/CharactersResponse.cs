/**
    "data": {
        "_id":"66168a4ca1fac599b83f531f",
        "organizationId":"65deed404227732687e58565",
        "organizationBaseModelId":"65deedf94edcb3debbd61c51",
        "assets":{},
        "glbUrl":"https://files.readyplayer.me/character/glbUrl/66168a4ca1fac599b83f531f/66168a4ca1fac599b83f531f-1712753229281.glb",
        "iconUrl":"https://renderapi.s3.amazonaws.com/bXeDv8rUL_000.png",
        "createdAt":"2024-04-10T12:47:08.786Z",
        "updatedAt":"2024-04-10T12:47:12.178Z",
        "__v":0,
        "id":"66168a4ca1fac599b83f531f"
    }
 */

namespace ReadyPlayerMe.Phoenix.Data
{
    public struct CharactersRequest
    {
        public CharactersRequestData data;
    }
    
    public struct CharactersRequestData
    {
        public string organizationBaseModelId;
        public string organizationId;
    }
    
    public struct CharactersResponse
    {
        public CharactersResponseData data;
    }
    
    public struct CharactersResponseData
    {
        public string id;
        public string organizationId;
        public string organizationBaseModelId;
        public string glbUrl;
        public string iconUrl;
    }
}
