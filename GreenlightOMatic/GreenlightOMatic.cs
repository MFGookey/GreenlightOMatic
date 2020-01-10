using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GreenlightOMatic.Shows;
using GreenlightOMatic.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace GreenlightOMatic
{
  public class GreenlightOMatic
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS1998:Await.Warning")]
    public async Task Run(ILoggerFactory loggerFactory)
    {
      var logger = loggerFactory.CreateLogger<GreenlightOMatic>();
      var shows = new ShowOptions[]
        {
          ShowOptions.Simpsons,
          ShowOptions.Firefly,
          ShowOptions.Dollhouse,
          ShowOptions.Quit
        };

      while (true)
      {
        DisplayMenu(shows);
        var choice = GetSelection(shows);
        Console.WriteLine($"You have selected {choice.ToString()}");

        if (choice != ShowOptions.Quit)
        {
          logger.LogInformation($"Beginning pre-production of {choice.ToString()}");
        }

        Task showToRun = Task.Delay(0);

        var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token;
        switch (choice)
        {
          case ShowOptions.Simpsons:
            Task.Factory.StartNew(async () =>
            {
              var show = new Simpsons(loggerFactory);
              showToRun = show.MakeShow(token);
            });
            break;
          case ShowOptions.Firefly:
            Task.Factory.StartNew(async () =>
            {
              var show = new Firefly(loggerFactory);
              showToRun = show.MakeShow(token);
            });
            break;
          case ShowOptions.Dollhouse:
            Task.Factory.StartNew(async () =>
            {
              var show = new Dollhouse(loggerFactory);
              showToRun = show.MakeShow(token);
            });
            break;
          case ShowOptions.Quit:
            return;
        }
        logger.LogInformation("Pre-production complete.  Let's make some television, people!");

        GetCancel();
        tokenSource.Cancel();
        logger.LogCritical($"Awaiting cancelled show {choice.ToString()} to cancel.");
        await showToRun;
        logger.LogCritical($"Show {choice.ToString()} has been cancelled.");
        await Task.Delay(1000); // Delay for a second to let the loggers finish up
      }
    }

    private ShowOptions GetSelection(ShowOptions[] shows)
    {
      while (true)
      {
        Console.Write("Enter option number:\t");
        var choice = Console.ReadLine();
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

    private void GetCancel()
    {
      Console.Write("Enter \"c\" or \"C\" to cancel the show");
      while (true)
      {
        Console.WriteLine();
        var keyStroke = Console.ReadKey();
        if (keyStroke.KeyChar.ToString().ToLowerInvariant().Equals("c"))
        {
          Console.WriteLine();
          return;
        }
      }
    }
  }
}
