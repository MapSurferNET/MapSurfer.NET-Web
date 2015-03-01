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
using System.Linq;
using System.Threading;
using System.Web;

using Nancy;

using MapSurfer.Web;

namespace MapSurfer.Web.Interfaces.Nancy
{
  public class NancyHttpContext : IHttpContext
  {
    private NancyContext m_context;
    private NancyHttpRequest m_request;
    private NancyHttpResponse m_response;
    private ManualResetEventSlim m_manualReset;

    public NancyHttpContext(NancyContext context, ManualResetEventSlim manualReset)
    {
      m_context = context;
      m_request = new NancyHttpRequest(context.Request);
      m_response = new NancyHttpResponse(context.Response);
      m_manualReset = manualReset;
    }

    public IHttpRequest Request
    {
      get
      {
        return m_request;
      }
    }

    public IHttpResponse Response
    {
      get
      {
        return m_response;
      }
    }

    public ManualResetEventSlim ManualResetEvent
    {
      get
      {
        return m_manualReset;
      }
    }
  }
}