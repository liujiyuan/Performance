using System.Web;

namespace HelloWorld
{
    public class HelloWorldHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
            Response.Write("Hello World ASP.NET Handler");
        }

        public bool IsReusable
        {
            // To enable pooling, return true here.
            // This keeps the handler in memory.
            get { return false; }
        }
    }
}
