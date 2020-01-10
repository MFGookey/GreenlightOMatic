using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GreenlightOMatic.Utility;
using Microsoft.Extensions.Logging;

namespace GreenlightOMatic.Shows
{
  public class Firefly : IShow
  {
    public ShowOptions Title { get; private set;}
    private ILogger<Firefly> _logger;

    public Firefly(ILoggerFactory loggerFactory)
    {
      _logger = loggerFactory.CreateLogger<Firefly>();
      Title = ShowOptions.Firefly;
    }

    public async Task MakeShow(CancellationToken ct)
    {
      int episode = 1,
          season = 1,
          episodesPerSeason = 10;
      while (ct.IsCancellationRequested == false)
      {
        _logger.LogInformation($"Making {Title.ToString()} season {season} episode {episode}");
        season += episode / episodesPerSeason;
        episode = (episode % episodesPerSeason) + 1;
        try
        {
          await Task.Delay(1000, ct);
        }
        catch (OperationCanceledException) {
           // These are expected.  Do nothing.
        }
      }
    }
  }
}
