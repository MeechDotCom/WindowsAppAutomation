using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DLLDevOpsHealthCheck
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Load configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var logFilePath = configuration["LogFilePath"];
            var checkInterval = int.Parse(configuration["CheckIntervalSeconds"]);

            // Set up performance counters for CPU and Memory
            var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            var memoryCounter = new PerformanceCounter("Memory", "Available MBytes");

            Console.WriteLine("DLL DevOps Health Check Application");
            Console.WriteLine("Press Ctrl+C to exit...");

            while (true)
            {
                // Get the current CPU and memory usage
                float cpuUsage = cpuCounter.NextValue();
                float memoryUsage = memoryCounter.NextValue();

                // Log the health check result
                string logEntry = $"{DateTime.Now}: CPU Usage: {cpuUsage}% | Available Memory: {memoryUsage} MB";
                await File.AppendAllTextAsync(logFilePath, logEntry + Environment.NewLine);

                Console.WriteLine(logEntry);

                // Wait for the next check interval
                await Task.Delay(checkInterval * 1000);
            }
        }
    }
}
