//==========================================================================================
//
//		MapSurfer.Web.Hosting
//		Copyright (c) 2008-2015, MapSurfer.NET
//
//    Authors: Maxim Rylov
//
//==========================================================================================
using System;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Web;

using MapSurfer.Web;

namespace MapSurfer.Web.Hosting
{
  public delegate IAsyncResult ProcessRequestDelegate(IHttpContext context, AsyncCallback callback, object state, byte serviceType);
  public delegate string[] GetMapUrlsDelegate();

  /// <summary>
  /// Summary description for AbstractMapService
  /// </summary>
  public abstract class AbstractMapService : IDisposable
  {
    protected ProcessRequestDelegate m_processReqDelegate;
    protected GetMapUrlsDelegate m_getMapUrlsDelegate;
    private object m_mapService;

    public AbstractMapService()
    {

    }

    protected static void CheckInstallPath()
    {
      string strInstallPath = (string)ConfigurationManager.AppSettings["MapSurfer.InstallPath"];

      if (strInstallPath != null)
      {
        if (!Directory.Exists(strInstallPath))
          throw new DirectoryNotFoundException(strInstallPath);

        AssemblyLoader.SetMSNInstallPath(Path.Combine(strInstallPath, "Core"));
      }
    }

    public void Load()
    {
      CheckInstallPath();

      string strVersion = (string)ConfigurationManager.AppSettings["MapSurfer.Version"];

      AssemblyLoader.Register(strVersion);

      Assembly assembly = AssemblyLoader.LoadAssembly("MapSurfer.Web.dll");

      if (assembly == null)
        throw new FileNotFoundException("MapSurfer.Web.dll");

      string typeName = "MapSurfer.Web.Services.MappingService";
      object[] args = new object[] { (string)ConfigurationManager.AppSettings["MapSurfer.ServiceName"] };

      m_mapService = assembly.CreateInstance(typeName, true, BindingFlags.CreateInstance, null, args, System.Globalization.CultureInfo.InvariantCulture, null);
      MethodInfo methodProcessReq = m_mapService.GetType().GetMethod("ProcessRequest");

      m_processReqDelegate = (ProcessRequestDelegate)Delegate.CreateDelegate(typeof(ProcessRequestDelegate), m_mapService, methodProcessReq);

      MethodInfo methodGetMapUrls = m_mapService.GetType().GetMethod("GetMapUrls");
      m_getMapUrlsDelegate = (GetMapUrlsDelegate)Delegate.CreateDelegate(typeof(GetMapUrlsDelegate), m_mapService, methodGetMapUrls);

      AssemblyLoader.Unregister();
    }

    public void Start()
    {
      MethodInfo mi = m_mapService.GetType().GetMethod("Start");
      mi.Invoke(m_mapService, null);
    }

    public void WriteError(Exception ex)
    {
      MethodInfo mi = m_mapService.GetType().GetMethod("WriteError");
      mi.Invoke(m_mapService, new object[] { ex });
    }

    public void Dispose()
    {
      if (m_mapService != null)
      {
        MethodInfo mi = m_mapService.GetType().GetMethod("Dispose");
        mi.Invoke(m_mapService, null);
        m_mapService = null;
      }
    }

    public string[] GetMapUrls()
    {
      return m_getMapUrlsDelegate();
    }

    public IAsyncResult ProcessRequest(IHttpContext context, AsyncCallback callback, object state, byte serviceType)
    {
      return m_processReqDelegate(context, callback, state, serviceType);
    }
  }
}