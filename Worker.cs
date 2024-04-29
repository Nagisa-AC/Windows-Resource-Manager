using System.Collections;
using System.ComponentModel;
using System.Diagnostics;


namespace App.WindowsService;

public sealed class WindowsBackgroundService(
    JokeService jokeService,
    ILogger<WindowsBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                ArrayList list = new ArrayList();
                string machineName = System.Environment.MachineName;
                var categories = PerformanceCounterCategory.GetCategories(machineName);

                //foreach (var category in categories )
                //{
                //    list.Add(category.CategoryName);
                //}

                //list.Sort();


                //PerformanceCounter processorCounter = new PerformanceCounter("Process", "% Processor Time", "Google Chrome");
                //float processorUsage = processorCounter.NextValue();
                //logger.LogInformation($"Processor usage: {processorUsage}%");
                //Thread.Sleep(2000);

                //var category = new PerformanceCounterCategory("Process");
                //var instanceNames = category.GetInstanceNames();
                //Array.Sort(instanceNames);
                //string instance;



                //foreach (var proc in instanceNames)
                //{
                //    try
                //    {
                //        using (PerformanceCounter processorCounter = new PerformanceCounter("Process", "% Processor Time", proc))
                //        {
                //            // Get the CPU usage percentage for the process
                //            float processorUsage = processorCounter.NextValue();
                //            Thread.Sleep(1000); // Wait for a second
                //            processorUsage = processorCounter.NextValue();

                //            // Log the process name and its CPU usage percentage
                //            logger.LogInformation($"{proc} usage: {processorUsage}%");
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        // Handle exceptions such as process not found or permission issues
                //        logger.LogWarning($"Failed to get CPU usage for process {proc}: {ex.Message}");
                //    }
                //}


                string processName = "chrome"; // Replace with the name of the process you want to monitor

                // Create a PerformanceCounter object for CPU usage of the specified process
                PerformanceCounter cpuCounter = new PerformanceCounter("Process", "% Processor Time", processName);
                PerformanceCounter ramCounter = new PerformanceCounter(".NET CLR Memory", "# bytes in all heaps", processName);

                // Set up a timer to periodically retrieve CPU usage
                System.Timers.Timer timer = new System.Timers.Timer();
                timer.Interval = 3000; // Set the interval to 1 second (1000 milliseconds)
                timer.Elapsed += (sender, e) =>
                {

                    // testing chrome mem usage 

                    double memsize = 0;
                    PerformanceCounter PC = new PerformanceCounter();
                    PC.CategoryName = "Process";
                    PC.CounterName = "Working Set - Private";
                    PC.InstanceName = "chrome";
                    memsize = PC.NextValue() / (1024 * 1024);
                    logger.LogInformation($"{PC.InstanceName} mem usage: {memsize} MB");
                    PC.Close();
                    PC.Dispose();

                    // Retrieve CPU usage for the specified process
                    //float cpuUsage = cpuCounter.NextValue();
                    //float ramUsage = ramCounter.NextValue();

                    //logger.LogInformation($"RAM Usage: {ramUsage}");

                    // Output CPU usage to the console
                    //Console.WriteLine($"CPU Usage for process '{processName}': {cpuUsage}%");
                };
                timer.Start();

                // Wait for user input to exit
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();



                Thread.Sleep(200000);

                //if (instanceNames.Contains("chrome"))
                //{
                //    instance = "chrome";
                //    logger.LogInformation($"{instance} is a running process");
                //}





                Thread.Sleep(100000);

                //Process[] localAll = Process.GetProcesses();
                //int counter = 1;
                //foreach (Process process in localAll)
                //{
                //    logger.LogInformation($"Process {counter}: {process.ProcessName}");
                //    counter++;
                //}


                //await Task.Delay(TimeSpan.FromSeconds(10000), stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            // When the stopping token is canceled, for example, a call made from services.msc,
            // we shouldn't exit with a non-zero exit code. In other words, this is expected...
        }
        catch (Exception ex)
        {
            logger.LogInformation(ex, "{Message}", ex.Message);


            // Terminates this process and returns an exit code to the operating system.
            // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
            // performs one of two scenarios:
            // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
            // 2. When set to "StopHost": will cleanly stop the host, and log errors.
            //
            // In order for the Windows Service Management system to leverage configured
            // recovery options, we need to terminate the process with a non-zero exit code.
            Environment.Exit(1);
        }
    }
}