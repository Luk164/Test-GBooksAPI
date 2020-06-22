using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Books.v1;
using Google.Apis.Discovery.v1.Data;
using Google.Apis.Services;
using Google.Apis.Storage.v1;
using Google.Apis.Util.Store;
using RestSharp;

namespace Test_GBooksAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Books API Sample: List MyLibrary");
            Console.WriteLine("================================");
            try
            {
                new Program().Run().Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("ERROR: " + e.Message);
                }
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private async Task Run()
        {
            UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = "289545511724-6dquu1ilvodbskkep1t8lpsa58cc6v1c.apps.googleusercontent.com",
                    ClientSecret = "YDXnTH5Tv2uqB9F4LM70qBHG"
                },
                new[] { BooksService.Scope.Books, },
                "user",
                CancellationToken.None,
                new FileDataStore("Books.ListMyLibrary"));
            var baseClientInit = new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Test CLI app",
                ApiKey = "AIzaSyDP_XISzAyHy5I0t_VRTz6ajBHWi0v9J9Q"
            };

            // Create the service.
            var booksService = new BooksService(baseClientInit);

            var listMyBooks = await booksService.Volumes.Useruploaded.List().ExecuteAsync();

            foreach (var (book, index) in listMyBooks.Items.WithIndex())
            {
                Console.WriteLine(index + " " + book.VolumeInfo.Title);
                if (book.AccessInfo.Epub != null)
                {
                    Console.WriteLine("EPUB:");
                    Console.WriteLine(book.AccessInfo.Epub.DownloadLink);
                }

                if (book.AccessInfo.Pdf != null)
                {
                    Console.WriteLine("PDF:");
                    Console.WriteLine(book.AccessInfo.Pdf.DownloadLink);
                }
            }

            var selection = int.Parse(Console.ReadLine());
            var test = new RestDescription.AuthData.Oauth2Data();
            
            var token = credential.Token;


            var client = new RestClient("http://books.google.sk/books/download/Light_Overlord_Vol_14_The_Witch_of_the_F?id=FS5wJwAAAEAJ&hl=&output=uploaded_content&source=gbs_api");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer ya29.a0AfH6SMDTUUFrF3ToJZPmAXGefBNvTWmHo30eiqqCxlZ5N1r6RrIr4x46uMenwm4LPqomU4XV9hMVXZCWru19hHT48SKMZZKSFOgRryCaVURbl8N_Zn7N0UeamNE3qkU1mZepv23jLOp0JhrhTQj-jx5Fv2YWX8_T32E2");
            IRestResponse response = client.Execute(request);
            var stream = new FileStream("book.epub", FileMode.CreateNew);
            stream.Write(response.RawBytes);

            //using (var client = new WebClient())
            //{
            //    client.Headers.Add("Authorization:" + token.AccessToken);
            //    client.DownloadFileCompleted += (sender, args) => Console.WriteLine("Download complete");
            //    var book = listMyBooks.Items[selection];
            //    var accessInfo = book.AccessInfo;
            //    client.DownloadFile(book.AccessInfo.Pdf.DownloadLink, "book.epub"); //There is a bug in API so pdf links are actually epubs
            //}
        }
    }
}
