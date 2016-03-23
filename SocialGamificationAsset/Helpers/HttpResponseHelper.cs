using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Net.Http.Headers;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using SocialGamificationAsset.Models;

namespace SocialGamificationAsset.Helpers
{
    public class HttpResponseHelper
    {
        public static JsonSerializerSettings JsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            return settings;
        }

        public static string SerializeToJson(object value)
        {
            return JsonConvert.SerializeObject(value, JsonSerializerSettings());
        }

        /**
         * TODO: 
         * Refactor ErrorContentResult to ErrorContentResult(string responseType = "json") 
         * and allow JSON & XML responses depending on request type from user.
         */

        public static ContentResult ErrorContentResult(
            object value,
            int status,
            string responseType = "json")
        {
            if (value is string)
            {
                value = new ApiError { Error = value.ToString() };
            }

            string content;
            MediaTypeHeaderValue contentType;
            if (responseType == "xml")
            {
                content = SerializeToJson(value);
                contentType = new MediaTypeHeaderValue("application/xml");
            }
            else
            {
                content = SerializeToJson(value);
                contentType = new MediaTypeHeaderValue("application/json");
            }

            return new ContentResult { StatusCode = status, Content = content, ContentType = contentType };
        }

        public static ContentResult NotFound(object value)
        {
            return ErrorContentResult(value, StatusCodes.Status404NotFound);
        }

        public static ContentResult BadRequest(object value)
        {
            return ErrorContentResult(value, StatusCodes.Status400BadRequest);
        }

        public static ContentResult Unauthorized(object value)
        {
            return ErrorContentResult(value, StatusCodes.Status401Unauthorized);
        }
    }
}