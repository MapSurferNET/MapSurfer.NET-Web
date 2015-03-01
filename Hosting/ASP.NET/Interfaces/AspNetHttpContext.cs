using System.Threading;
using System.Web;

namespace MapSurfer.Web.Interfaces.AspNet
{
  public class AspNetHttpContext : IHttpContext
  {
    private HttpContext m_context;
    private AspNetHttpRequest m_request;
    private AspNetHttpResponse m_response;

    public AspNetHttpContext(HttpContext context)
    {
      m_context = context;
      m_request = new AspNetHttpRequest(context.Request);
      m_response = new AspNetHttpResponse(context.Response);
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
        return null;
      }
    }
  }
}