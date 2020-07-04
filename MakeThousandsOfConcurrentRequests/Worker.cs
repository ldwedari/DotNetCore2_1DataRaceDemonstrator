using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MakeThousandsOfConcurrentOfRequests
{
    public class Worker : BackgroundService
    {
        private readonly IHttpClientFactory _clientFactory;

        public Worker(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //await Task.Delay(5000, stoppingToken); //Wait for the Server to start
            var tasks = Enumerable.Range(0, 10000).Select(i =>
            {
                var httpClient = _clientFactory.CreateClient($"test");
                return httpClient.GetAsync("https://localhost:44394/api/values", stoppingToken);
            });

            await Task.WhenAll(tasks);
        }
    }
}
