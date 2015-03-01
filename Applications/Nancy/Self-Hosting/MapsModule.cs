//==========================================================================================
//
//		MapSurfer.Web.Hosting.Nancy
//		Copyright (c) 2008-2015, MapSurfer.NET
//
//    Authors: Maxim Rylov
//
//==========================================================================================
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

using Nancy;

using MapSurfer.Web.Interfaces.Nancy;

namespace MapSurfer.Web.Hosting.Nancy
{
  public class MapsModule : NancyModule
  {
    private ConcurrentStack<ManualResetEventSlim> m_manualResetEvents;
    private string[] m_mapUrls = null;

    public MapsModule(WebApplicationService webApplicationService)
    {
      m_manualResetEvents = new ConcurrentStack<ManualResetEventSlim>();
      NancyHostMapService mapService = webApplicationService.GetServiceInstance();

      if (m_mapUrls == null)
      {
        string[] mapUrls = mapService.GetMapUrls();

        for (int i = 0; i < mapUrls.Length; i++)
        {
          Uri uri = new Uri(mapUrls[i]);
          mapUrls[i] = uri.AbsolutePath;
        }

        m_mapUrls = mapUrls;
      }

      if (m_mapUrls.Length == 0)
        return;

      for (int i = 0; i < m_mapUrls.Length; i++)
      {
        Get[m_mapUrls[i], true] = async (x, ct) =>
        {
          Response resp = this.Context.Response;
          Stream stream = new MemoryStream();

          if (resp == null)
          {
            resp = new StreamResponse(() => stream);
            this.Context.Response = resp;
          }

          ManualResetEventSlim mres = null;
          if (!m_manualResetEvents.TryPop(out mres))
            mres = new ManualResetEventSlim();

          IHttpContext httpContext = new NancyHttpContext(this.Context, mres);

          Action<IAsyncResult> TileCompletedCallback = (result) =>
          {
            resp.Contents.Invoke(stream);
            stream.Position = 0;
          };

          IAsyncResult ar = mapService.ProcessRequest(httpContext, new AsyncCallback(TileCompletedCallback), 0, 0);

          if (!ar.IsCompleted)
          {
            mres.Wait();
            mres.Reset();
          }

          m_manualResetEvents.Push(mres);

          httpContext.Response.Flush();

          return this.Context.Response;
        };
      }
    }
  }
}
