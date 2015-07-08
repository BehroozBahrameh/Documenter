using System.Collections.Generic;

namespace Rocket.Web.Documenter.Core
{
    public class ApiDocument
    {
        public string Url { get; set; }

        public string Method { get; set; }

        public IEnumerable<string[]> UrlProperties { get; set; }

        public IEnumerable<string[]> RequestProperties { get; set; }

        public string RequestBodyJson { get; set; }

        public IEnumerable<string[]> ResponseProperties { get; set; }

        public string ResponseJson { get; set; }
    }
}
