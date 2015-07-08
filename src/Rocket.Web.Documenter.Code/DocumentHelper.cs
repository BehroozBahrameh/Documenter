using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json;
using Rocket.Web.Documenter.Core.Apenders;

namespace Rocket.Web.Documenter.Core
{
    public class DocumentHelper
    {
        public DocumentHelper()
        {
            Appenders = new List<IDocumenterAppender>
            {
                new MarkdownDocumenterAppender()
            };
        }

        public ICollection<IDocumenterAppender> Appenders;

        private IEnumerable<ApiDocument> DocumentObjGenerator(HttpConfiguration configuration)
        {
            IApiExplorer apiExplorer = configuration.Services.GetApiExplorer();
            var des = apiExplorer.ApiDescriptions;

            ///APIs
            foreach (var apiDescription in des)
            {
                var apiDoc = new ApiDocument
                {
                    Method = apiDescription.HttpMethod.ToString(),
                    Url = apiDescription.RelativePath,
                    ResponseJson = apiDescription.ResponseDescription.ResponseType == null
                        ? "EMPTY"
                        : JsonHelper.FormatJson(JsonConvert.SerializeObject(MakeInstance(apiDescription.ResponseDescription.ResponseType))),
                    ResponseProperties = MakeDocument(apiDescription.ResponseDescription.ResponseType)
                };

                var parameters = apiDescription.ParameterDescriptions.GroupBy(i => i.Source);
                var req = "EMPTY";

                //BODY OR URL
                foreach (var parameter in parameters)
                {
                    //writer.WriteLine("####" + parameter.Key);

                    foreach (var apiParameterDescription in parameter)
                    {
                        if (apiParameterDescription.ParameterDescriptor == null) continue;

                        if (parameter.Key == ApiParameterSource.FromBody)
                        {
                            var ins = MakeInstance(apiParameterDescription.ParameterDescriptor.ParameterType);
                            apiDoc.RequestBodyJson = JsonHelper.FormatJson(JsonConvert.SerializeObject(ins));
                            apiDoc.RequestProperties = MakeDocument(apiParameterDescription.ParameterDescriptor.ParameterType);
                        }
                        else
                        {
                            apiDoc.UrlProperties = MakeDocument(apiParameterDescription.ParameterDescriptor.ParameterType, apiParameterDescription.Name);
                        }
                    }
                }

                yield return apiDoc;
            }
        }

        public void Generate(HttpConfiguration configuration)
        {
            var docObj = DocumentObjGenerator(configuration).ToArray();

            foreach (var documenterAppender in Appenders)
            {
                documenterAppender.Create(docObj);
            }
        }

        /// <summary>
        /// Instantiated object and instantiate all inner properties
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object MakeInstance(Type type)
        {
            object instance = null;
            if (type.IsValueType)
            {
                instance = Activator.CreateInstance(type);
            }
            else
            {
                if (type == typeof(string))
                {
                    instance = "sample string";
                }
                else if (type.IsArray)
                {
                    var elmType = type.GetElementType();
                    var array = Array.CreateInstance(elmType, 1);
                    array.SetValue(MakeInstance(elmType), 0);
                    instance = array;
                }
                else if (type.GetInterfaces().Any(t => new[] { typeof(IEnumerable<>), typeof(IEnumerable) }.Contains(t)))
                {
                    var listType = typeof(List<>);
                    var elmType = type.GetGenericArguments()[0];
                    var constructedListType = listType.MakeGenericType(elmType);

                    instance = Activator.CreateInstance(constructedListType);
                    instance.GetType().GetMethod("Add").Invoke(instance, new[] { MakeInstance(elmType) });
                }
                else if (type.IsEnum)
                {
                    //TODO
                }
                else
                {
                    instance = Activator.CreateInstance(type);
                    var props = type.GetProperties();
                    foreach (var propertyInfo in props)
                    {
                        var innerInstanse = MakeInstance(propertyInfo.PropertyType);
                        propertyInfo.SetValue(instance, innerInstanse);
                    }
                }
            }
            return instance;
        }

        /// <summary>
        /// Create obj document table
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static IEnumerable<string[]> MakeDocument(Type type, string name = "---")
        {
            var result = new List<string[]>();

            if (type == null)
            {
                return result;
            }

            if (type.IsValueType)
            {
                if (type.IsEnum)
                {
                    var value = Enum.GetValues(type)
                        .Cast<object>()
                        .Aggregate("",
                            (current, item) =>
                                current + string.Format("{1} : {0} ,", item.ToString(), Convert.ToInt64(item)));
                    result.Add(new[] { name, value });
                }
                else
                {
                    result.Add(new[] { name, type.Name });
                }
            }
            else
            {
                if (type == typeof(string))
                {
                    result.Add(new[] { name, type.Name });
                }
                else if (type.IsArray)
                {
                    var elmType = type.GetElementType();
                    result.Add(new[] { name, elmType.Name + "[]" });


                    if (!GetEndType(elmType).IsValueType)
                    {
                        result.AddRange(MakeDocument(elmType, elmType.Name));
                    }
                }
                else if (type.GetInterfaces().Any(t => t == typeof(IEnumerable)))
                {
                    var elmType = type.GetGenericArguments()[0];
                    result.Add(new[] { name, elmType.Name + "[]" });
                    result.AddRange(MakeDocument(elmType, elmType.Name));
                }
                else
                {
                    var props = type.GetProperties();
                    foreach (var propertyInfo in props)
                    {
                        result.AddRange(MakeDocument(propertyInfo.PropertyType, propertyInfo.Name));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get array of array end type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Type GetEndType(Type type)
        {
            return type.GetElementType() != null
                ? GetEndType(type.GetElementType())
                : type;
        }
    }
}
