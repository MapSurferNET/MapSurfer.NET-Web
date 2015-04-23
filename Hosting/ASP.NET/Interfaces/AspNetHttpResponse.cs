using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace MapSurfer.Web.Interfaces.AspNet
{
  public class AspNetHttpResponse : IHttpResponse
  {
    private HttpResponse m_response;

    public AspNetHttpResponse(HttpResponse response)
    {
      m_response = response;
    }

    public bool IsClientConnected
    {
      get
      {
        return m_response.IsClientConnected;
      }
    }

    public string ContentType
    {
      get
      {
        return m_response.ContentType;
      }
      set
      {
        m_response.ContentType = value;
      }
    }

    public HttpStatusCode StatusCode
    {
      get
      {
        return (HttpStatusCode)m_response.StatusCode;
      }
      set
      {
        m_response.StatusCode = (int)value;
      }
    }

    public void Clear()
    {
      m_response.Clear();
    }

    public void Flush()
    {
      m_response.Flush();
    }

    public void AddHeader(string header, string value)
    {
      m_response.AddHeader(header, value);
    }

    public void SetExpirationTime(int time)
    {
      m_response.Cache.SetMaxAge(new TimeSpan(0, 0, time));
      m_response.Cache.SetExpires(DateTime.UtcNow.AddSeconds(time));
      m_response.Cache.SetValidUntilExpires(false);
    }

    public void SetCreationTime(DateTime time)
    {
      if (time != DateTime.MinValue)
        m_response.Cache.SetLastModified(time);
    }

    public void Write(string value)
    {
      m_response.Write(value);
    }

    public void Write(string value, Encoding encoding)
    {
      m_response.Write(value);
    }

    public void Write(byte[] bytes, int offset, int length)
    {
      m_response.OutputStream.Write(bytes, offset, length);
    }

    public void Write(Action<Stream> content)
    {
      content(m_response.OutputStream);
    }
  }
}