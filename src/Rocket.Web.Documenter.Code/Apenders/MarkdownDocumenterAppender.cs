using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Hosting;

namespace Rocket.Web.Documenter.Core.Apenders
{
    public class MarkdownDocumenterAppender : IDocumenterAppender
    {
        private const string MainTemplate =
            @"
##{0}
        
URL | Method        
---|---
`~/{0}` | `{1}` 

{2}


###Request :

```javascript
{3}
```

{4}

        
###Response :

```javascript
{5}
```

{6}

***
";

        private const string TableHeaderTemplate =
            @"Name | Type
---|---
";

        private const string TableRowTemplate =
            @"{0} | {1}
";

        public static string Path = System.IO.Path.Combine(HostingEnvironment.ApplicationPhysicalPath,
            string.Format("api_document_{0:yyyyMd_HHmm}.txt", DateTime.UtcNow));

        public void Create(IEnumerable<ApiDocument> models)
        {
            using (var writer = File.AppendText(Path))
            {
                writer.WriteLine("- Methods");
                var apiDocumentModels = models.ToArray();

                foreach (var apiDocumentModel in apiDocumentModels)
                {
                    writer.WriteLine("	- [{0}](#{1})", apiDocumentModel.Url,
                        Regex.Replace(apiDocumentModel.Url.ToLower(), "[\\~#%&*{}/:<>?|\"-]", string.Empty));
                }

                foreach (var apiDocumentModel in apiDocumentModels)
                {
                    var urlParams =
                        apiDocumentModel.UrlProperties == null
                            ? string.Empty
                            : TableHeaderTemplate +
                              apiDocumentModel.UrlProperties.Aggregate("",
                                  (current, item) => current + string.Format(TableRowTemplate, item[0], item[1]));

                    var requestParams =
                        apiDocumentModel.RequestProperties == null
                            ? string.Empty
                            : TableHeaderTemplate +
                              apiDocumentModel.RequestProperties.Aggregate("",
                                  (current, item) => current + string.Format(TableRowTemplate, item[0], item[1]));

                    var responseParams =
                        apiDocumentModel.ResponseProperties == null
                            ? string.Empty
                            : TableHeaderTemplate + apiDocumentModel.ResponseProperties
                                .Aggregate("",
                                    (current, item) => current + string.Format(TableRowTemplate, item[0], item[1]));

                    var tmp = string.Format(MainTemplate
                        , apiDocumentModel.Url
                        , apiDocumentModel.Method
                        , urlParams
                        , apiDocumentModel.RequestBodyJson
                        , requestParams
                        , apiDocumentModel.ResponseJson
                        , responseParams);

                    writer.Write(tmp);
                }
            }
        }
    }
}
