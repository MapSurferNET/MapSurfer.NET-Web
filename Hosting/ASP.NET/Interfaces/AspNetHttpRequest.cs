using System;
using System.Collections.Specialized;
using System.Web;

namespace MapSurfer.Web.Interfaces.AspNet
{
  public class AspNetHttpRequest : IHttpRequest
  {
    private HttpRequest m_request;

    public AspNetHttpRequest(HttpRequest request)
    {
      m_request = request;
    }

    public string RawUrl
    {
      get { return m_request.RawUrl; }
    }

    public string UserHostAddress
    {
      get { return m_request.UserHostAddress; }
    }

    public NameValueCollection QueryString
    {
      get { return m_request.QueryString; }
    }

    public NameValueCollection ServerVariables
    {
      get { return m_request.ServerVariables; }
    }

    public bool IsLocal
    {
      get { return m_request.IsLocal; }
    }

    public Uri Url
    {
      get { return m_request.Url; }
    }

    public Uri UrlReferrer
    {
      get { return m_request.UrlReferrer; }
    }

    public string GetHeaderValue(string header)
    {
      NameValueCollection headers = m_request.Headers;
      for (int i = 0; i < headers.Count; i++)
      {
        string h = m_request.Headers[i];
        if (string.Equals(h, header, StringComparison.OrdinalIgnoreCase))
          return headers[h];
      }

      return null;
    }
  }
}