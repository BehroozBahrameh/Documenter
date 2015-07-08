using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Description;
using Sample.Models;

namespace Sample.Controllers
{
    public class HomeController : ApiController
    {
        [HttpGet, ResponseType(typeof(SampleResponseModel))]
        public HttpResponseMessage Get()
        {
            return new HttpResponseMessage
            {
                Content = new ObjectContent(typeof(SampleResponseModel), new SampleResponseModel(), new JsonMediaTypeFormatter()),
                StatusCode = HttpStatusCode.OK
            };
        }

        [HttpGet, ResponseType(typeof(SampleResponseModel))]
        public HttpResponseMessage Get(long id)
        {
            return new HttpResponseMessage
            {
                Content = new ObjectContent(typeof(SampleResponseModel), new SampleResponseModel(), new JsonMediaTypeFormatter()),
                StatusCode = HttpStatusCode.OK
            };
        }

        [HttpPost, ResponseType(typeof(SampleResponseModel))]
        public HttpResponseMessage Post([FromBody] SampleRequestModel model)
        {
            return new HttpResponseMessage
            {
                Content = new ObjectContent(typeof(SampleResponseModel), new SampleResponseModel(), new JsonMediaTypeFormatter()),
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}
