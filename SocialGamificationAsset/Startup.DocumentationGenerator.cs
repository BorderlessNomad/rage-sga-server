using System;
using System.Collections.Generic;

using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;

using Swashbuckle.SwaggerGen;

namespace SocialGamificationAsset
{
    public partial class Startup
    {
        /// <summary>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="applicationEnvironment"></param>
        private static void ConfigureDocumentationGeneratorServices(
            IServiceCollection services,
            IApplicationEnvironment applicationEnvironment)
        {
            // SWASHBUCKLE SWAGGER API Documentation Generator
            services.AddSwaggerGen();

            services.ConfigureSwaggerDocument(
                options =>
                    {
                        options.IgnoreObsoleteActions = true;

                        options.OrderActionGroupsBy(new DescendingAlphabeticComparer());

                        options.SingleApiVersion(
                            new Info
                            {
                                Version = "v1",
                                Title = "Social Gamification API",
                                Description =
                                    "This module allows to layer game mechanics affording game-inspired social relations and interactions on top a system to support engagement, collaboration, and learning. Two main forms of social interaction are supported: player-player interactions (such as matches) and group interactions (such as shared team goals or team vs. team competitions).",
                                TermsOfService = ""
                            });

                        // options.OperationFilter(new Swashbuckle.SwaggerGen.XmlComments.ApplyXmlActionComments(pathToDoc));

                        /*
				options.SecurityDefinitions.Add("apiKey", new ApiKeyScheme()
				{
					Type = "apiKey",
					Description = "API Key Authentication",
					Name = "apiKey",
					In = "header"
				});
				*/
                    });

            services.ConfigureSwaggerSchema(
                options =>
                    {
                        options.DescribeAllEnumsAsStrings = true;

                        // options.ModelFilter(new Swashbuckle.SwaggerGen.XmlComments.ApplyXmlTypeComments(pathToDoc));
                    });
        }

        /// <summary>
        /// </summary>
        /// <param name="application"></param>
        private static void ConfigureDocumentationGenerator(IApplicationBuilder application)
        {
            application.UseSwaggerGen();
            application.UseSwaggerUi();
        }
    }

    /// <summary>
    /// </summary>
    public class DescendingAlphabeticComparer : IComparer<string>
    {
        /// <summary>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>
        /// </returns>
        public int Compare(string x, string y)
        {
            return string.Compare(y, x, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}