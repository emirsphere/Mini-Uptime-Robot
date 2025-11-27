using System.Diagnostics;

namespace Mini_Uptime_Robot
{
    public class WebsiteResult
    {
        public int Latency { get; set; }
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class Worker : BackgroundService
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            client.Timeout = TimeSpan.FromSeconds(10);
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                string url = _configuration.GetValue<string>("RobotSettings:TargetUrl") ?? "https://www.google.com";
                int delaySeconds = _configuration.GetValue<int>("RobotSettings:IntervalSeconds");
                try
                {
                    WebsiteResult result = await GetResultAsync(url);
                    if (result.IsSuccess)
                    {
                        _logger.LogInformation("Siteye eriþildi. Kod: {code}, Süre: {ms} ms",
                            result.StatusCode, result.Latency);
                    }
                    else
                    {
                        _logger.LogWarning("Sitede sorun var! Kod: {code}, Hata: {err}",
                            result.StatusCode, result.ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Kritik Hata: Siteye hiç ulaþýlamadý!");
                }

                await Task.Delay(delaySeconds * 1000, stoppingToken);
            }
        }

        private async Task<WebsiteResult> GetResultAsync(string url)
        {
            var result = new WebsiteResult();
            var stopwatch = StartStopWatch();

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Head, url);

                using (var response = await client.SendAsync(request))
                {
                    // Saati durdur ve kaydet
                    result.Latency = StopStopWatch(stopwatch);
                    result.StatusCode = (int)response.StatusCode;
                    result.IsSuccess = response.IsSuccessStatusCode;

                    if (!result.IsSuccess)
                    {
                        result.ErrorMessage = "Sunucu hata kodu döndürdü.";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Latency = StopStopWatch(stopwatch);
                result.IsSuccess = false;
                result.StatusCode = 0; 
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private Stopwatch StartStopWatch()
        {
            return Stopwatch.StartNew();
        }

        private int StopStopWatch(Stopwatch stopwatch)
        {
            stopwatch.Stop();
            return (int)stopwatch.ElapsedMilliseconds;
        }
    }
}