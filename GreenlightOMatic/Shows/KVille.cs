using System;
using System.Threading;
using System.Threading.Tasks;
using GreenlightOMatic.Utility;
using Microsoft.Extensions.Logging;

namespace GreenlightOMatic.Shows
{
  public class KVille : IShow
  {
    public ShowOptions Title { get; private set;}
    private ILogger<KVille> _logger;

    public KVille(ILoggerFactory loggerFactory)
    {
      _logger = loggerFactory.CreateLogger<KVille>();
      Title = ShowOptions.KVille;
    }

    public async Task MakeShow(CancellationToken ct)
    {
      await Task.Delay(200); // Let other loggers catch up
      int episode = 1,
          season = 1,
          episodesPerSeason = 10;

      var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(ct);
      var token = linkedSource.Token;

      linkedSource.CancelAfter(TimeSpan.FromSeconds(4));
      
      while (token.IsCancellationRequested == false)
      {
        _logger.LogInformation($"Making {Title.ToString()} season {season} episode {episode}");
        season += episode / episodesPerSeason;
        episode = (episode % episodesPerSeason) + 1;
        try
        {
          await Task.Delay(1000, token);
        }
        catch (OperationCanceledException) {
          if (ct.IsCancellationRequested == false)
          {

            _logger.LogWarning("Oh no a writer's strike has happened!");
          }
          //throw;
        }
      }
      //throw new OperationCanceledException();
    }
  }
}
