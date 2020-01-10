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
    private CancellationTokenSource _tokenSource;
    private ShowOptions _choice;

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
      
      Console.CancelKeyPress += (sender, eventArgs) =>
      {
        _logger.LogInformation($"Awaiting cancelled show {_choice.ToString()} to cancel.");
        eventArgs.Cancel = true;
        _tokenSource.Cancel();
      };

      while (true)
      {
        DisplayMenu(shows);
        _choice = GetSelection(shows);
        Console.WriteLine($"You have selected {_choice.ToString()}");

        if (_choice != ShowOptions.Quit)
        {
          _logger.LogInformation($"Beginning pre-production of {_choice.ToString()}");
        }

        _tokenSource = new CancellationTokenSource();
        var token = _tokenSource.Token;
        
        IShow show = null;
        
        switch (_choice)
        {
          case ShowOptions.Simpsons:
            show = new Simpsons(loggerFactory);
            break;
          case ShowOptions.Firefly:
            show = new Firefly(loggerFactory);
            break;
          case ShowOptions.Dollhouse:
            show = new Dollhouse(loggerFactory);
            break;
          case ShowOptions.KVille:
            show = new KVille(loggerFactory);
            break;
          case ShowOptions.Quit:
            return;
        }
        
        _logger.LogInformation("Pre-production complete.  Let's make some television, people!");
        
        PromptCancel();

        if (null != show)
        {
          await show.MakeShow(token).ContinueWith((t) =>
          {
            _logger.LogCritical($"Show {_choice.ToString()} has been cancelled.");
          });
        }

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

    private void PromptCancel()
    {
      Console.Write("Enter ctrl+\"c\" to cancel the show");
      Console.WriteLine();
    }
  }
}
