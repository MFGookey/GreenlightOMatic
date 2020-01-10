using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace GreenlightOMatic
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var gom = new GreenlightOMatic();
      var options = new ConsoleLoggerOptions();
      options.Format = ConsoleLoggerFormat.Default;
      var loggerFactory = LoggerFactory.Create(builder =>
      {
        builder
            .AddFilter("Microsoft", LogLevel.Warning)
            .AddFilter("System", LogLevel.Warning)
            .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
            .AddConsole(o =>
              {
                o.Format = ConsoleLoggerFormat.Default;
              }
            );
      });
      
      try
      {
        gom.Run(loggerFactory).GetAwaiter().GetResult();
      }
      catch(Exception e)
      {
        Console.WriteLine(e.Message);
        Console.WriteLine(e.StackTrace);
      }

      loggerFactory.Dispose();
      Console.WriteLine("Press any key to continue...");
      Console.ReadKey();
    }
  }
}
