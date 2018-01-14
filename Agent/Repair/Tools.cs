using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repair
{
    public class Tools
    {
        public static string GetMd5(string file)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(file))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public static class StorageModel
        {
            static string applicationFolder = AppDomain.CurrentDomain.BaseDirectory;

            public static string ExpandRelativeName(string relativeName) =>
                Path.GetFullPath(Path.Combine(applicationFolder, relativeName));
        }

        public class Network
        {
            public static void DownloadFile(Uri uri, string relativePath)
            {
                using (var c = new WebClient())
                {
                    c.DownloadFile(uri, StorageModel.ExpandRelativeName(relativePath));
                }
            }

            public static string ReadText(Uri uri)
            {
                var wc = new WebClient { Encoding = Encoding.UTF8 };
                return wc.DownloadString(uri);
            }

            public static async Task<string> ReadTextAsync(Uri uri, CancellationToken ct = default(CancellationToken))
            {
                var wc = new WebClient { Encoding = Encoding.UTF8 };
                using (ct.Register(wc.CancelAsync))
                    return await wc.DownloadStringTaskAsync(uri);
            }

            public static async Task<string> ReadTextAsync(Uri uri, TimeSpan timeout, CancellationToken ct = default(CancellationToken))
            {
                var wc = new WebClient { Encoding = Encoding.UTF8 };
                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(ct))
                using (cts.Token.Register(wc.CancelAsync))
                {
                    try
                    {
                        cts.CancelAfter(timeout);
                        return await wc.DownloadStringTaskAsync(uri);
                    }
                    catch (OperationCanceledException ex) when (cts.IsCancellationRequested && !ct.IsCancellationRequested)
                    {
                        throw new TimeoutException("Couldn't read text within allotted time frame", ex);
                    }
                }
            }
        }
    }
}
