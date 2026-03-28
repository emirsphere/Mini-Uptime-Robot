using Mini_Uptime_Robot.Services;

namespace Mini_Uptime_Robot
{
    
    public class Worker : BackgroundService
    {
        private readonly IUptimeCheckerService _uptimeCheckerService;
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        public Worker(ILogger<Worker> logger, IConfiguration configuration, IUptimeCheckerService uptimeCheckerService)
        {
            _logger = logger;
            _configuration = configuration;
            _uptimeCheckerService = uptimeCheckerService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var urls = _configuration.GetSection("RobotSettings:TargetUrls").Get<string[]>()
                           ?? new[] { "https://www.google.com" };
                int delaySeconds = _configuration.GetValue<int>("RobotSettings:IntervalSeconds");
                var parallelOptions = new ParallelOptions
                {
                    MaxDegreeOfParallelism = 10,
                    CancellationToken = stoppingToken
                };
                _logger.LogInformation("--- Yeni Kontrol Döngüsü Baþladý ({count} Site) ---", urls.Length);

                await Parallel.ForEachAsync(urls, parallelOptions, async (url, token) =>
                {
                    var result = await _uptimeCheckerService.CheckAsync(url, token);

                    if (result.IsSuccess)
                    {
                        _logger.LogInformation("Siteye eriþildi [{url}]. Kod: {code}, Süre: {ms} ms",
                            url, result.StatusCode, result.Latency);
                    }
                    else
                    {
                        _logger.LogWarning("Sitede sorun var [{url}]! Kod: {code}, Hata: {err}",
                            url, result.StatusCode, result.ErrorMessage);
                    }
                });
                _logger.LogInformation("--- Kontrol Döngüsü Bitti. {delay} sn bekleniyor... ---", delaySeconds);
                await Task.Delay(delaySeconds * 1000, stoppingToken);
            }
        }
    }
}