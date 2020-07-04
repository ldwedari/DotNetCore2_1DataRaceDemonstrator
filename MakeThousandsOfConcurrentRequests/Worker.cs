using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace MakeThousandsOfConcurrentRequests
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
            await Task.Delay(5000, stoppingToken); //Wait for the Server to start
            while (!stoppingToken.IsCancellationRequested)
            {
                var tasks = Enumerable.Range(0, 100).Select(i =>
                {
                    var httpClient = _clientFactory.CreateClient($"test");
                    return httpClient.GetAsync("https://localhost:5001/api/values", stoppingToken);
                });

                await Task.WhenAll(tasks);
            }
        }
    }
}
