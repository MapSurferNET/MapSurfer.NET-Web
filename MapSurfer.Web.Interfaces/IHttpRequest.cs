//==========================================================================================
//
//		MapSurfer.Web
//		Copyright (c) 2008-2015, MapSurfer.NET
//
//    Authors: Maxim Rylov
//
//==========================================================================================
using System;
using System.Collections.Specialized;
using System.Net;

namespace MapSurfer.Web
{
  public interface IHttpRequest
  {
    string RawUrl { get; }
    string UserHostAddress { get; }

    NameValueCollection QueryString { get; }

    NameValueCollection ServerVariables { get; }

    bool IsLocal { get; }

    Uri Url { get; }

    Uri UrlReferrer { get; }

    string GetHeaderValue(string header);
  }
}
