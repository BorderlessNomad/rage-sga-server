using System;

using Microsoft.AspNet.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SocialGamificationAsset.Models;

namespace SocialGamificationAsset
{
    public partial class Startup
    {
        /// <summary>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        private static void ConfigureApplicationServices(IServiceCollection services, IConfiguration configuration)
        {
            // Add Application Context
            services.AddScoped<SocialGamificationAssetContext>();

            // Add DB Initiliazer Service
            services.AddTransient<DataInitializer>();
        }

        /// <summary>
        /// </summary>
        /// <param name="application"></param>
        /// <param name="loadAsync"></param>
        /// <exception cref="Exception" />
        private static void ConfigureDatabaseInitialization(IApplicationBuilder application, bool loadAsync = true)
        {
            try
            {
                DataInitializer.Initialize(application.ApplicationServices, loadAsync).Wait();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}