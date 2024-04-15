using UnityEngine.Networking;

namespace ReadyPlayerMe.Phoenix
{
    public class Response : IResponse
    {
        public string Text;
        public byte[] Data;

        public bool IsSuccess { get; set; }
        public string Error { get; set; }
        public long ResponseCode { get; set; }

        public void Parse(UnityWebRequest request)
        {
            if (request.downloadHandler is DownloadHandlerFile)
            {
                return;
            }

            Text = request.downloadHandler.text;
            Data = request.downloadHandler.data;
        }
    }
}
