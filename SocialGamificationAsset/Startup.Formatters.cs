using Boilerplate.Web.Mvc.Formatters;

using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json.Serialization;

namespace SocialGamificationAsset
{
    public partial class Startup
    {
        /// <summary>
        ///     Configures the input and output formatters.
        /// </summary>
        private static void ConfigureFormatters(IMvcBuilder mvcBuilder)
        {
            // The JSON input and output formatters are added to MVC by default.
            // $Start-JsonSerializerSettings$

            // Configures the JSON output formatter to use camel case property names like 'propertyName' instead of 
            // pascal case 'PropertyName' as this is the more common JavaScript/JSON style.
            mvcBuilder.AddJsonOptions(
                x => x.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

            // $End-JsonSerializerSettings$
            // $Start-BsonFormatter$

            // Adds the BSON input and output formatters using the JSON.NET serializer.
            mvcBuilder.AddBsonSerializerFormatters();

            // $End-BsonFormatter$
            // $Start-XmlFormatter-DataContractSerializer$

            // Adds the XML input and output formatter using the DataContractSerializer.
            mvcBuilder.AddXmlDataContractSerializerFormatters();

            // $End-XmlFormatter-DataContractSerializer$
            // $Start-XmlFormatter-XmlSerializer$

            // Adds the XML input and output formatter using the XmlSerializer.
            mvcBuilder.AddXmlSerializerFormatters();

            // $End-XmlFormatter-XmlSerializer$
        }

        /// <summary>
        ///     Configures the input and output formatters.
        /// </summary>
        private static void ConfigureFormatters(MvcOptions mvcOptions)
        {
            // $Start-NoContentFormatter$
            // Force 204 No Content response, when returning null values.
            // Note that we are inserting this at the top of the formatters collection so we can select a formatter
            // quickly in this case.
            mvcOptions.OutputFormatters.Insert(0, new HttpNoContentOutputFormatter());

            // $End-NoContentFormatter$

            // $Start-NotAcceptableFormatter$
            // Force 406 Not Acceptable responses if the media type is not supported, instead of returning the default.
            // Note that we are inserting this near the top of the formatters collection so we can select a formatter
            // quickly in this case.
            mvcOptions.OutputFormatters.Insert(1, new HttpNotAcceptableOutputFormatter());

            // $End-NotAcceptableFormatter$
        }

        private static void ConfigureFormatters(IApplicationBuilder application)
        {
            application.UseExceptionHandler(
                appBuilder =>
                    {
                        /*
                appBuilder.Use(async (context, next) =>
                {
                    var excHandler = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;
                    if (context.Request.GetTypedHeaders().Accept.Any(header => header.MediaType == "application/json"))
                    {
                        var jsonString = string.Format("{{\"error\":\"{0}\"}}", excHandler.Error.Message);
                        context.Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
                        await context.Response.WriteAsync(jsonString, Encoding.UTF8);
                    }
                    else
                    {
                        //I haven't figured out a better way of signally ExceptionHandlerMiddleware that we can't handle the exception
                        //But this will do the trick of letting the other error handlers to intervene
                        //as the ExceptionHandlerMiddleware class will swallow this exception and rethrow the original one
                        throw excHandler.Error;
                    }
                });
                */
                    });
        }
    }
}