//==========================================================================================
//
//		MapSurfer.Web
//		Copyright (c) 2008-2015, MapSurfer.NET
//
//    Authors: Maxim Rylov
//
//==========================================================================================
using System;

namespace MapSurfer.Web
{
  public static class HostingEnvironment
  {
    private static string m_webSiteName;

    public static string GetWebSiteName()
    { 
      return m_webSiteName;
    }

    public static void SetWebSiteName(string value)
    {
      m_webSiteName = value;
    }
  }
}
