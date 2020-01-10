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
  public class Dollhouse : IShow
  {
    public ShowOptions Title { get; private set;}
    private ILogger<Dollhouse> _logger;

    public Dollhouse(ILoggerFactory loggerFactory)
    {
      _logger = loggerFactory.CreateLogger<Dollhouse>();
      Title = ShowOptions.Dollhouse;
    }

    public async Task MakeShow(CancellationToken ct)
    {
      await Task.Delay(200); // Let other loggers catch up
      int season = 1,
          episodesPerSeason = 10;

      while (ct.IsCancellationRequested == false)
      {
        for (var episode = 1; episode <= episodesPerSeason; episode++)
        {
          _logger.LogInformation($"Making {Title.ToString()} season {season} episode {episode}");
          season += episode / episodesPerSeason;
          await Task.Delay(1000);
        }
      }
    }
  }
}
