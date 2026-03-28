using System.Diagnostics;
using Mini_Uptime_Robot.Models;


namespace Mini_Uptime_Robot.Services
{
    public class UptimeCheckerService : IUptimeCheckerService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UptimeCheckerService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<WebsiteResult> CheckAsync(string url, CancellationToken cancellationToken)
        {
            var result = new WebsiteResult();
            var stopwatch = StartStopWatch();
            using (var client = _httpClientFactory.CreateClient())
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Head, url);
                    client.Timeout = TimeSpan.FromSeconds(10);
                    using (var response = await client.SendAsync(request, cancellationToken))
                    {
                        result.Latency = StopStopWatch(stopwatch);
                        result.StatusCode = (int)response.StatusCode;
                        result.IsSuccess = response.IsSuccessStatusCode;
                        if (!result.IsSuccess)
                        {
                            result.ErrorMessage = $"Received status code {(int)response.StatusCode}";
                        }
                    }

                }

                catch (TaskCanceledException)
                {
                    result.Latency = StopStopWatch(stopwatch);
                    result.IsSuccess = false;
                    result.StatusCode = 0;
                    result.ErrorMessage = "Request timed out";
                }
                catch (Exception ex)
                {
                    result.Latency = StopStopWatch(stopwatch);
                    result.IsSuccess = false;
                    result.StatusCode = 0;
                    result.ErrorMessage = ex.Message;
                }
            }
            return result;

        }

        private Stopwatch StartStopWatch()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            return stopwatch;
        }

        private int StopStopWatch(Stopwatch stopwatch)
        {
            stopwatch.Stop();
            return (int)stopwatch.ElapsedMilliseconds;
        }
    }
}
