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
  public class Simpsons : IShow
  {
    public ShowOptions Title { get; private set;}
    private ILogger<Simpsons> _logger;

    public Simpsons(ILoggerFactory loggerFactory)
    {
      _logger = loggerFactory.CreateLogger<Simpsons>();
      Title = ShowOptions.Simpsons;
    }

    public async Task MakeShow(CancellationToken ct)
    {
      Task.Delay(200); // Let other loggers catch up
      int episode = 1,
          season = 1,
          episodesPerSeason = 10;

      while (true)
      {
        _logger.LogInformation($"Making {Title.ToString()} season {season} episode {episode}");
        season += episode / episodesPerSeason;
        episode = (episode % episodesPerSeason) + 1;
        await Task.Delay(1000);
      }
    }
  }
}
