using System;
using System.Threading;
using System.Threading.Tasks;
using GreenlightOMatic.Shows;
using GreenlightOMatic.Utility;
using Microsoft.Extensions.Logging;

namespace GreenlightOMatic
{
  public class GreenlightOMatic
  {

    private ILogger _logger;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS1998:Await.Warning")]
    public async Task Run(ILoggerFactory loggerFactory)
    {
      _logger = loggerFactory.CreateLogger<GreenlightOMatic>();
      var shows = new ShowOptions[]
        {
          ShowOptions.Simpsons,
          ShowOptions.Firefly,
          ShowOptions.Dollhouse,
          ShowOptions.KVille,
          ShowOptions.Quit
        };

      while (true)
      {
        DisplayMenu(shows);
        var choice = GetSelection(shows);
        Console.WriteLine($"You have selected {choice.ToString()}");

        if (choice != ShowOptions.Quit)
        {
          _logger.LogInformation($"Beginning pre-production of {choice.ToString()}");
        }

        Task showToRun = Task.CompletedTask;

        var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token;
        int timeout = Timeout.Infinite;

        Func<Task> taskToRun = () => Task.CompletedTask;
        switch (choice)
        {
          case ShowOptions.Simpsons:
           taskToRun = async () =>
            {
              var show = new Simpsons(loggerFactory);
              showToRun = show.MakeShow(token);
            };
            break;
          case ShowOptions.Firefly:
            taskToRun = async () =>
            {
              var show = new Firefly(loggerFactory);
              showToRun = show.MakeShow(token);
            };
            break;
          case ShowOptions.Dollhouse:
            taskToRun = async () =>
            {
              var show = new Dollhouse(loggerFactory);
              showToRun = show.MakeShow(token);
            };
            break;
          case ShowOptions.KVille:
            taskToRun = async () =>
            {
              var show = new KVille(loggerFactory);
              showToRun = show.MakeShow(token);
            };

            timeout = 100;
            break;
          case ShowOptions.Quit:
            return;
        }

        _logger.LogInformation("Pre-production complete.  Let's make some television, people!");

        await Task.Factory.StartNew(taskToRun);

        GetCancel(showToRun, timeout);
        if (showToRun.IsCanceled || showToRun.IsCompleted || showToRun.IsFaulted)
        {
          _logger.LogCritical($"Cannot cancel {choice.ToString()} as something else cancelled it");
        }
        else
        {
          tokenSource.Cancel();
          _logger.LogCritical($"Awaiting cancelled show {choice.ToString()} to cancel.");
          if (showToRun.IsCanceled || showToRun.IsCompleted || showToRun.IsFaulted)
          {
            await showToRun;
          }
          _logger.LogCritical($"Show {choice.ToString()} has been cancelled.");
        }
        await Task.Delay(1000); // Delay for a second to let the loggers finish up
      }
    }

    private ShowOptions GetSelection(ShowOptions[] shows)
    {
      while (true)
      {
        Console.Write("Enter option number:\t");
        var choice = Reader.ReadKey(Timeout.Infinite).KeyChar.ToString();
        Console.WriteLine();

        if (int.TryParse(choice, out int choiceOption))
        {
          if (choiceOption >= 0 && choiceOption < shows.Length)
          {
            return shows[choiceOption];
          }
        }
        Console.WriteLine("Please select a VALID option");
      }
    }

    private void DisplayMenu(ShowOptions[] shows)
    {
      Console.WriteLine("Please greenlight a show:");
      for (var i = 0; i < shows.Length; i++)
      {
        Console.WriteLine($"{i}:\t{shows[i].ToString()}");
      }
    }

    private void GetCancel(Task runningTask, int timeout)
    {
      Console.Write("Enter \"c\" or \"C\" to cancel the show");
      Console.WriteLine();
      while (true)
      {
        try
        {

          var keyStroke = Reader.ReadKey(timeout);
          if (keyStroke.KeyChar.ToString().ToLowerInvariant().Equals("c"))
          {
            return;
          }
          Console.WriteLine();
        }
        catch (TimeoutException)
        {
          _logger.LogDebug("Timeout Exception caught");
          if (runningTask.IsCanceled || runningTask.IsCompleted || runningTask.IsFaulted)
          {
            return;
          }
        }
      }
    }
  }
}
