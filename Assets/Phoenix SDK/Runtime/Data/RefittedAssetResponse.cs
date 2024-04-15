namespace ReadyPlayerMe.Phoenix.Data
{
    public struct RefittedAssetResponse
    {
        public RefittedAssetResponseData[] data;
        public Pagination pagination;
    }
    
    public struct RefittedAssetResponseData
    {
        public string id;
        public string organizationId;
        public string baseModelId;
        public string glbUrl;
        public string iconUrl;
    }
}