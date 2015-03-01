//==========================================================================================
//
//		MapSurfer.Web
//		Copyright (c) 2008-2015, MapSurfer.NET
//
//    Authors: Maxim Rylov
//
//==========================================================================================
using System;
using System.IO;
using System.Net;
using System.Text;

namespace MapSurfer.Web
{
  public interface IHttpResponse
  {
    bool IsClientConnected { get; }

    string ContentType { get; set; }

    HttpStatusCode StatusCode { get; set; }

    void Clear();

    void Flush();

    void AddHeader(string header, string value);

    void Write(string value);

    void Write(string value, Encoding encoding);

    void Write(byte[] bytes, int offset, int length);

    void Write(Action<Stream> content);
  }
}
