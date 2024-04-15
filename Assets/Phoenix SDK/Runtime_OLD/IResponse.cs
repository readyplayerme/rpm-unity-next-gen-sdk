using UnityEngine.Networking;

namespace ReadyPlayerMe.Phoenix
{
    public interface IResponse
    {
        bool IsSuccess { get; set; }
        string Error { get; set; }
        long ResponseCode { get; set; }
        void Parse(UnityWebRequest request);
    }
}