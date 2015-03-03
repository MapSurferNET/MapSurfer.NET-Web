//==========================================================================================
//
//		MapSurfer.Web.Interfaces.Nancy
//		Copyright (c) 2008-2015, MapSurfer.NET
//
//    Authors: Maxim Rylov
//
//==========================================================================================
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

using Nancy;

using HttpStatusCode = System.Net.HttpStatusCode;
using NancyHttpStatusCode = Nancy.HttpStatusCode;

namespace MapSurfer.Web.Interfaces.Nancy
{
  public class NancyHttpResponse : IHttpResponse
  {
    private Response m_response;

    public NancyHttpResponse(Response response)
    {
      m_response = response;
    }

    public bool IsClientConnected
    {
      get
      {
        return true;
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
        return (HttpStatusCode)(int)m_response.StatusCode;
      }
      set
      {
        m_response.StatusCode = (NancyHttpStatusCode)value;
      }
    }

    public void Clear()
    {
    }

    public void Flush()
    {
     // m_response.Flush();
    }

    public void AddHeader(string header, string value)
    {
      m_response.Headers.Add(header, value);
    }

    private StreamWriter CreateStreamWriter(Stream stream, Encoding encoding)
    {
      return new StreamWriter(stream, encoding, 1024, true);
    }

    public void Write(string value)
    {
      m_response.Contents = stream =>
      {
        using (var writer = CreateStreamWriter(stream, Encoding.UTF8))
        {
          writer.Write(value);
        }
      };
    }

    public void Write(string value, Encoding encoding)
    {
      m_response.Contents = stream =>
      {
        using (var writer = CreateStreamWriter(stream, encoding))
        {
          writer.Write(value);
        }
      };
    }

    public void Write(byte[] bytes, int offset, int length)
    {
      m_response.Contents = stream =>
      {
        stream.Write(bytes, offset, length);
      };
    }

    public void Write(Action<Stream> content)
    {
      m_response.Contents = stream =>
      {
        content(stream);
      };
    }
  }
}