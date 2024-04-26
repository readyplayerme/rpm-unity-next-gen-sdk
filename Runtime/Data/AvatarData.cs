using UnityEngine;
using ReadyPlayerMe.Runtime.Utils;

namespace ReadyPlayerMe.Runtime.Data
{
    public class AvatarData: MonoBehaviour
    {
        public string Id { get; private set; }
        
        public void Initialize(string url)
        {
            Id = UrlUtils.GetIdFromUrl(url);
        }
    }
}
