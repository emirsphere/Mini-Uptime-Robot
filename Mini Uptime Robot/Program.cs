using Polly;
using Polly.Extensions.Http;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Mini_Uptime_Robot.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
namespace Mini_Uptime_Robot
{
    public class Program
    {
        public static void Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("C:\\Logs\\uptime-log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Servis baþlatýlýyor...");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Servis beklenmedik þekilde durdu!");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHttpClient("UptimeClient")
                            .AddPolicyHandler(GetRetryPolicy());

                    services.AddTransient<IUptimeCheckerService, UptimeCheckerService>();

                    services.AddHostedService<Worker>();
                });

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry:(outcome, timespan, retryAttempt, context) =>
                {
                    Log.Warning("Hata alýndý. {Delay} saniye bekleniyor... (Deneme {RetryCount})", timespan.TotalSeconds, retryAttempt);

                });
                
        }


    }
}