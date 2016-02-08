using Boilerplate.Web.Mvc.Formatters;

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

            // Configures the JSON output formatter to use camel case property names like 'propertyName' instead of
            // pascal case 'PropertyName' as this is the more common JavaScript/JSON style.
            mvcBuilder.AddJsonOptions(
                x => x.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

            // Adds the BSON input and output formatters using the JSON.NET serializer.
            mvcBuilder.AddBsonSerializerFormatters();

            // Adds the XML input and output formatter using the DataContractSerializer.
            // mvcBuilder.AddXmlDataContractSerializerFormatters();

            // Adds the XML input and output formatter using the XmlSerializer.
            // mvcBuilder.AddXmlSerializerFormatters();
        }
    }
}