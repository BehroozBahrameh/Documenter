# Documenter
lightweight document generator for Asp.net WebApi.

By default Documenter generate markdown documentation  but you can develop your owned appender.

```csharp
	var generator = Rocket.Web.Documenter.Core.DocumentHelper();
	generator.Generate(GlobalConfiguration.Configuration);
	
```

MarkdownAppneder generate text file in root of project `in api_document_{datetime}.txt` format

## Custom Appender
To develop your owned appender, you must implement **IDocumenterAppender** interface.

```csharp
    public class MyDocumenterAppender : IDocumenterAppender
    {
		void Create(IEnumerable<ApiDocument> models){
			...
		}
    }
```


Create method get collection of ApiDocument object, each ApiDocument object is a your project api-controller action,

```csharp
	public class ApiDocument
    {
        public string Url { get; set; }	//Url of action							
        public string Method { get; set; } //GET, POST or ...
        public IEnumerable<string[]> UrlProperties { get; set; } //Query strings
        public IEnumerable<string[]> RequestProperties { get; set; } //Body properties
        public string RequestBodyJson { get; set; } //Sample JSON of body object
        public IEnumerable<string[]> ResponseProperties { get; set; } //Response properties
        public string ResponseJson { get; set; } //Sample JSON of response object
    }
```
