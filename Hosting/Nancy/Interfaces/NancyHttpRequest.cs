//==========================================================================================
//
//		MapSurfer.Web.Interfaces.Nancy
//		Copyright (c) 2008-2015, MapSurfer.NET
//
//    Authors: Maxim Rylov
//
//==========================================================================================
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

using Nancy;

namespace MapSurfer.Web.Interfaces.Nancy
{
  public class NancyHttpRequest : IHttpRequest
  {
    private NameValueCollection m_query;
    private Request m_request;

    public NancyHttpRequest(Request request)
    {
      m_request = request;
    }

    public string RawUrl
    {
      get
      {
        return m_request.Url.ToString();
      }
    }

    public string UserHostAddress
    {
      get { return m_request.UserHostAddress; }
    }

    private NameValueCollection GetQueryValues()
    {
      if (m_query == null)
      {
        m_query = new NameValueCollection();

        DynamicDictionary dynDict = m_request.Query;
        foreach (string itemKey in dynDict.Keys)
        {
          dynamic newValue = null;
          if (dynDict.TryGetValue(itemKey, out newValue))
            m_query.Add(itemKey, newValue.Value.ToString());
        }
      }

      return m_query;
    }

    public NameValueCollection QueryString
    {
      get
      {
        return GetQueryValues();
      }
    }

    public NameValueCollection ServerVariables
    {
      get
      {
        return GetQueryValues(); // TODO
      }
    }

    public bool IsLocal
    {
      get
      {
        string remoteAddress = m_request.UserHostAddress;
        return !string.IsNullOrEmpty(remoteAddress) && (remoteAddress == "127.0.0.1" || remoteAddress == "::1");// || remoteAddress == this.GetLocalAddress());
      }
    }

    public Uri Url
    {
      get
      {
        return m_request.Url;
      }
    }

    public Uri UrlReferrer
    {
      get
      {
        return String.IsNullOrEmpty(m_request.Headers.Referrer) ? null : new Uri(m_request.Headers.Referrer);
      }
    }

    public string GetHeaderValue(string header)
    {
      IEnumerable<string> values = m_request.Headers[header];
      foreach (string v in values)
        return v;

      return null;
    }
  }
}