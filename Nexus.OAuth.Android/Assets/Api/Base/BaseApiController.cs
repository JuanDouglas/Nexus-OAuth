using Android.Content;
using Android.Content.PM;
using Android.OS;
using Java.Net;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Nexus.OAuth.Android.Assets.Api.Base
{
    internal abstract class BaseApiController : IDisposable
    {
        private const string StorageClientKey = "Client-Key";
        private const string HeaderUserAgent = "User-Agent";
        private const string HeaderAcceptLanguage = "Accept-Language";
        public const string Host =
#if DEBUG
            "https://nexus-oauth-app.azurewebsites.net/api";
#else
            "https://nexus-oauth-app.azurewebsites.net/api";
#endif
        public abstract string ControllerHost { get; }
        public static string ClientKey
            => GetClientKey();

        public Context Context { get; set; }
        public HttpClient HttpClient { get => _httpClient; }
        private readonly HttpClient _httpClient;
        private readonly string version;
        public virtual HttpRequestMessage BaseRequest
        {
            get
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get
                };

                request.Headers.Add(HeaderAcceptLanguage, Thread.CurrentThread.CurrentCulture.Name);
                request.Headers.Add(StorageClientKey, ClientKey);
                request.Headers.Add(HeaderUserAgent, $"Mobile.OAuth/{version} (Android {Build.VERSION.ReleaseOrCodename};)");
                return request;
            }
        }
        public BaseApiController(Context context)
        {
            _httpClient = new HttpClient();
            Context = context ?? throw new NullReferenceException(nameof(context));

            PackageManager manager = Context.PackageManager;
            PackageInfo pckInfo = manager.GetPackageInfo(Context.PackageName, PackageInfoFlags.Activities);

            version = pckInfo.VersionName;
        }

        private static string GetClientKey()
        {
            Task<string> taskGetKey = SecureStorage.GetAsync(StorageClientKey);
            taskGetKey.Wait();

            string clientKey = taskGetKey.Result;

            if (string.IsNullOrEmpty(clientKey))
            {
                clientKey = GenerateToken(96);

                Task save = SecureStorage.SetAsync(StorageClientKey, clientKey);
                save.Wait();
            }

            return clientKey;
        }

        private protected virtual async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            var response = await HttpClient.SendAsync(request);

            return response;
        }

        private protected static async Task<T> CastJsonResponse<T>(HttpResponseMessage response)
        {
            string json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Generate Tokens with specific length
        /// </summary>
        /// <param name="size">Token Size</param>
        /// <param name="lower">Use lowercase characters.</param>
        /// <param name="upper">Use uppercase characters.</param>
        /// <returns>New token with size value.</returns>
        public static string GenerateToken(int size, bool upper = true, bool lower = true)
        {
            // ASCII characters rangers
            byte[] lowers = new byte[] { 97, 123 };
            // Upercase latters
            byte[] uppers = new byte[] { 65, 91 };
            // ASCII numbers
            byte[] numbers = new byte[] { 48, 58 };

            var random = new Random();
            string result = string.Empty;

            for (int i = 0; i < size; i++)
            {
                int type = random.Next(0, lower ? 3 : 2);

                byte[] possibles = type switch
                {
                    1 => upper ? uppers : numbers,
                    2 => lowers,
                    _ => numbers
                };

                int selected = random.Next(possibles[0], possibles[1]);
                char character = (char)selected;

                result += character;
            }

            return result;
        }

        public static string EncodeString(string str)
            => URLEncoder.Encode(str, Encoding.Default.HeaderName);
        public void Dispose()
        {
            HttpClient.Dispose();
            GC.Collect();
        }
    }
}