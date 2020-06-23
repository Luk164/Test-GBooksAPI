using System;
using System.IO;
using Google.Apis.Books.v1.Data;
using RestSharp;

namespace Downloader
{
    public static class MyDownloader
    {
        public static IRestResponse MyDownload(Volume selectedVolume, string token)
        {
            var client =
                new RestClient(selectedVolume.AccessInfo.Pdf.DownloadLink)
                {
                    Timeout = -1
                };
            var getRequest = new RestRequest(Method.GET);
            getRequest.AddHeader("Authorization", "Bearer " + token);
            var test = client.Execute(getRequest);
            return test;
            //var stream = new FileStream(selectedVolume.VolumeInfo.Title + ".epub", FileMode.CreateNew);
            //stream.Write(response.RawBytes);
        }
    }
}
