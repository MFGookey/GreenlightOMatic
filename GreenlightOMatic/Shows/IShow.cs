using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GreenlightOMatic.Utility;

namespace GreenlightOMatic.Shows
{
  public interface IShow
  {
    Task MakeShow(CancellationToken ct);

    ShowOptions Title
    {
      get;
    }
  }
}
