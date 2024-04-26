using System;
using UnityEngine;

namespace ReadyPlayerMe.Runtime.Utils
{
    public class UrlUtils : MonoBehaviour
    {
        public static string GetIdFromUrl(string url)
        {
            Uri.TryCreate(url, UriKind.Absolute, out Uri uri);
            string fileName = uri.Segments[^1];
            return fileName.Split('-')[0];
        }
    }
}
