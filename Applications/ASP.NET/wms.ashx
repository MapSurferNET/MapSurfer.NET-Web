<%@ WebHandler Language="C#" Class="WmsHandler" %>

using System;
using System.Web;

using MapSurfer.Web.Hosting.AspNet;
using MapSurfer.Web.Interfaces.AspNet;

public class WmsHandler : IHttpAsyncHandler 
{
  public void ProcessRequest(HttpContext context)
  {
    throw new NotImplementedException();
  }

  public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback callback, object state)
  {
    IAsyncResult result = null;
    AspNetHostMapService mapService = (AspNetHostMapService)context.Application["MapSurfer.MapServiceInstance"];
    if (mapService != null)
    {
        AspNetHttpContext httpContext = new AspNetHttpContext(context);
        result = mapService.ProcessRequest(httpContext, callback, state, 1);
    }

    return result;
  }

  public void EndProcessRequest(IAsyncResult result)
  {
    //throw new NotImplementedException();
  }

  public bool IsReusable 
  {
    get 
    {
      return false;
    }
  }
}