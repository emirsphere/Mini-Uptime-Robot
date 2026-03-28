🚀 Mini Uptime Robot

Mini Uptime Robot is a .NET 8 based, configurable web monitoring tool that runs as a background service (Windows Service compatible) and supports structured logging.

Unlike simple "Ping" tools, it uses HTTP HEAD requests to verify not just if the server is reachable, but if the application is "healthy" (Status Code 200) and measures the response latency.

🌟 Features

Smart Health Check: Checks HTTP Status Codes to ensure application health, not just server connectivity.

Low Resource Consumption: Extremely CPU and RAM friendly thanks to HEAD requests and async/await architecture.

Structured Logging: Uses Serilog infrastructure to generate daily rolling log files (Logs/uptime-log-YYYYMMDD.txt).

Configuration: Target URL and check intervals can be modified via appsettings.json without recompiling the code.

Service Ready: Ready to be installed as a native Windows Service.

⚙️ Installation & Usage

Follow these steps to run this project on your own server.

1. Configuration

Open the appsettings.json file with a text editor and configure it for your target website:

{
  "RobotSettings": {
    "TargetUrl": "[https://www.your-website.com](https://www.your-website.com)",   <-- Change this
    "IntervalSeconds": 60                            <-- Check interval in seconds
  }
}


2. Build

Build the project in "Release" mode using Visual Studio or the CLI:

dotnet publish -c Release -o C:\Robot


3. Install as Windows Service (Optional)

To start the application automatically on boot, open CMD as Administrator and run:

sc create "MiniUptimeRobot" binPath= "C:\Robot\Mini_Uptime_Robot.exe" start= auto
sc start "MiniUptimeRobot"


Note: To uninstall the service, use the sc delete "MiniUptimeRobot" command.

📊 Logs

The application generates detailed reports in C:\Logs (or your configured path) while running:

[14:00:01 INF] Robot Started! Target: [https://google.com](https://google.com), Interval: 10 sec
[14:00:11 INF] ✅ Site is UP. Code: 200, Latency: 45 ms
[14:00:21 WRN] ⚠️ Site issues detected! Code: 500, Error: Internal Server Error


🛠️ Technologies

  .NET 8 Worker Service

  Serilog (File & Console Logging)

  System.Net.HttpClient (Best Practices)

  Dependency Injection
