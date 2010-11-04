using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;

namespace fit.Runner
{
  public class AutoSetThreadCulture : IDisposable
  {
    private CultureInfo previousCulture;

    public AutoSetThreadCulture(CultureInfo culture)
    {
      previousCulture = Thread.CurrentThread.CurrentCulture;
      Thread.CurrentThread.CurrentCulture = culture;
    }

    public void Dispose()
    {
      Thread.CurrentThread.CurrentCulture = previousCulture;
    }
  }
}
