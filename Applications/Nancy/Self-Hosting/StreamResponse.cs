//==========================================================================================
//
//		MapSurfer.Web.Hosting.Nancy
//		Copyright (c) 2008-2015, MapSurfer.NET
//
//    Authors: Maxim Rylov
//
//==========================================================================================
using System;
using System.Collections.Generic;
using System.IO;

using Nancy;

namespace MapSurfer.Web.Hosting.Nancy
{
  /// <summary>
  /// Response that returns the contents of a stream of a given content-type.
  /// </summary>
  public class StreamResponse : Response
  {
    private Stream source;
    /// <summary>
    /// Initializes a new instance of the <see cref="StreamResponse"/> class with the
    /// provided stream provider and content-type.
    /// </summary>
    /// <param name="source">The value producer for the response.</param>
    public StreamResponse(Func<Stream> source)
    {
      this.Contents = GetResponseBodyDelegate(source);
      this.StatusCode = HttpStatusCode.OK;
    }

    private Action<Stream> GetResponseBodyDelegate(Func<Stream> sourceDelegate)
    {
      return stream =>
      {
        using (this.source = sourceDelegate.Invoke())
        {
          this.source.CopyTo(stream);
        }
      };
    }
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public override void Dispose()
    {
      if (this.source != null)
      {
        this.source.Dispose();
      }
    }
  }
}
