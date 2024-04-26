using System;

namespace ReadyPlayerMe.Runtime.Utils
{
    public static class UrlUtils
    {
        /// <summary>
        ///     Get Avatar id from url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetIdFromUrl(string url)
        {
            Uri.TryCreate(url, UriKind.Absolute, out Uri uri);
            string fileName = uri.Segments[^1];
            return fileName.Split('-')[0];
        }
    }
}
