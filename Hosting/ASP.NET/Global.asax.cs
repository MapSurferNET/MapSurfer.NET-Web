using System;

namespace MapSurfer.Web.Hosting.AspNet
{
  public class Global : System.Web.HttpApplication
  {
    private const string MAP_SERVICE_OBJECT_NAME = "MapSurfer.MapServiceInstance";

    /// <summary>
    /// Creates mapping service instance.
    /// </summary>
    private void CreateMapService()
    {
      if (Application[MAP_SERVICE_OBJECT_NAME] == null)
      {
        AspNetHostMapService mapService = new AspNetHostMapService();
        mapService.Load();
        mapService.Start();

        this.Application[MAP_SERVICE_OBJECT_NAME] = mapService;
      }
    }

    /// <summary>
    /// Destroyes mapping sevice instance.
    /// </summary>
    private void DestroyMapService()
    {
      try
      {
        if (Application[MAP_SERVICE_OBJECT_NAME] != null)
        {
          AspNetHostMapService mapService = (AspNetHostMapService)Application[MAP_SERVICE_OBJECT_NAME];
          mapService.Dispose();
          Application[MAP_SERVICE_OBJECT_NAME] = null;
        }
      }
      catch
      { }
    }


    protected void Application_Start(object sender, EventArgs e)
    {
      CreateMapService();
    }

    protected void Session_Start(object sender, EventArgs e)
    {

    }

    protected void Application_BeginRequest(object sender, EventArgs e)
    {

    }

    protected void Application_AuthenticateRequest(object sender, EventArgs e)
    {

    }

    protected void Application_Error(object sender, EventArgs e)
    {

    }

    protected void Session_End(object sender, EventArgs e)
    {

    }

    protected void Application_End(object sender, EventArgs e)
    {
      DestroyMapService();
    }
  }
}