using ReadyPlayerMe.Api.V1;

namespace ReadyPlayerMe.Samples.AvatarCreator
{
    public static class AssetExtension
    {
        public static bool IsStyleAsset(this Asset asset)
        {
            return asset.Type == "baseModel";
        }
    }
}