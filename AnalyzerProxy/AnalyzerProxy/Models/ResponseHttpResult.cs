using System.Net;

namespace AnalyzerProxy.Models
{
    public class ResponseHttpResult
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string StatusCode { get; set; }
        public string JsonResponse { get; set; }
        public string Token { get; set; }
    }
}
