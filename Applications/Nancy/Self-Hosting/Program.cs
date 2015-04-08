//==========================================================================================
//
//		MapSurfer.Web.Hosting.Nancy
//		Copyright (c) 2008-2015, MapSurfer.NET
//
//    Authors: Maxim Rylov
//
//==========================================================================================
using System;
using System.IO;
using System.Configuration;

using Nancy;
using Nancy.Hosting.Self;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace MapSurfer.Web.Hosting.Nancy
{
  class Program
  {
    static void Main(string[] args)
    {
      if (args != null && args.Length >= 2)
      {
        if (args[0] == "-conf" && File.Exists(args[1]))
          AppConfig.ChangeAppConfig(args[1]);
      }

      // initialize an instance of NancyHost (found in the Nancy.Hosting.Self package)
      string uri = (string)ConfigurationManager.AppSettings["Nancy.Uri"];
      if (string.IsNullOrEmpty(uri))
        throw new Exception("The parameter 'Nancy.Uri' is not specified in .config file.");

      var host = new NancyHost(new Uri(uri), new CustomBootstrapper());
      host.Start(); // start hosting
      Console.WriteLine();
      Console.Write("Service is ready to receive requests on ");

      ConsoleColor prevColor = Console.ForegroundColor;
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine(uri.ToString());
      Console.ForegroundColor = prevColor;

      Console.ReadKey();
      host.Stop(); // stop hosting 
    }
  }

  public class WebApplicationService
  {
    private NancyHostMapService m_mapService;

    public WebApplicationService()
    {
      m_mapService = new NancyHostMapService();
      Console.WriteLine();
      Console.Write("Loading service...");
      m_mapService.Load();
      Console.WriteLine("   Done.");
      Console.Write("Starting service...");
      m_mapService.Start();
      Console.WriteLine("   Done.");
    }

    public NancyHostMapService GetServiceInstance()
    {
      return m_mapService;
    }
  }

  public class CustomBootstrapper : DefaultNancyBootstrapper
  {
    protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
    {
      base.ApplicationStartup(container, pipelines);
      container.Register<WebApplicationService>().AsSingleton();
    }
  }
}
