using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace zero_allow.webapi.loadtest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var subOptimalUrl = new Uri("http://localhost:5000/contentlogging/suboptimal");

            var totalRequests = 1_000_000;

            var concurrency = 1000;

            using (var sem = new SemaphoreSlim(concurrency, concurrency))
                using(var httpClient = new HttpClient())
            {
                var requestsIssued = 0;

                var requestBytes = Encoding.UTF8.GetBytes(string.Join(Environment.NewLine,
                    Enumerable.Repeat("012345678", 1024 * 1024)));
                
                var bytesContent = new ByteArrayContent(requestBytes);
                var responseBuffer = new Memory<byte>(new byte[1024 * 1024]);
                
                for (; requestsIssued < totalRequests; requestsIssued++)
                {
                    if (requestsIssued % 1000 == 0)
                    {
                        Console.WriteLine($"Issued: {requestsIssued}");
                    }
                    await sem.WaitAsync();
                    async Task RunOne(HttpClient client, Uri uri, ByteArrayContent content, Memory<byte> buffer)
                    {
                        try
                        {
                            var response = await client.PostAsync(uri, content);
                            //Console.WriteLine("issued request");
                            var stream = await response.Content.ReadAsStreamAsync();
                            await stream.ReadAsync(buffer);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        finally
                        {
                            sem.Release();
                        }
                    }
                    _ = RunOne(httpClient, subOptimalUrl, bytesContent, responseBuffer);
                }

                for (var i = 0; i < concurrency; i++)
                {
                    sem.Wait();
                }
                Console.WriteLine($"Issued: {requestsIssued} requests total");
            }
        }
    }
}