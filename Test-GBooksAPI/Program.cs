using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Downloader;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Books.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;

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
            var selectedVolume = listMyBooks.Items[selection];

            var token = await credential.GetAccessTokenForRequestAsync();

            var response = MyDownloader.MyDownload(selectedVolume, token);
            var name = selectedVolume.VolumeInfo.Title.RemoveFilenameInvalidChars() + ".epub";
            File.Delete(name);
            var stream = new FileStream(name, FileMode.CreateNew);
            stream.Write(response.RawBytes);
        }
    }
}
