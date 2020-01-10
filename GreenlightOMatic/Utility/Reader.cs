using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GreenlightOMatic.Utility
{
  public static class Reader
  {
    private static Thread inputThread;
    private static AutoResetEvent getInput, gotInput;
    private static ConsoleKeyInfo input;

    static Reader()
    {
      getInput = new AutoResetEvent(false);
      gotInput = new AutoResetEvent(false);
      inputThread = new Thread(reader);
      inputThread.IsBackground = true;
      inputThread.Start();
    }

    private static void reader()
    {
      while (true)
      {
        getInput.WaitOne();
        input = Console.ReadKey();
        gotInput.Set();
      }
    }

    // omit the parameter to read a line without a timeout
    public static ConsoleKeyInfo ReadKey(int timeOutMillisecs = Timeout.Infinite)
    {
      getInput.Set();
      bool success = gotInput.WaitOne(timeOutMillisecs);
      if (success)
        return input;
      else
        throw new TimeoutException("User did not provide input within the timelimit.");
    }
  }
}
