using System;
using System.Threading;

namespace MapSurfer.Web
{
  public interface IHttpContext
  {
    IHttpRequest Request { get; }
    IHttpResponse Response { get; }

    ManualResetEventSlim ManualResetEvent { get; }

   // void WriteToTrace(string message);
  }
}
