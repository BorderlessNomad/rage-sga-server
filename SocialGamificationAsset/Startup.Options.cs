using System.Collections.Generic;

using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SocialGamificationAsset
{
    public partial class Startup
    {
        /// <summary>
        ///     Configures the settings by binding the contents of the config.json file
        ///     to the specified Plain Old CLR Objects (POCO) and adding
        ///     <see cref="IOptions{}" /> objects to the <paramref name="services" />
        ///     collection.
        /// </summary>
        /// <param name="services">
        ///     The services collection or IoC container.
        /// </param>
        /// <param name="configuration">
        ///     Gets or sets the application configuration, where key value pair
        ///     settings are stored.
        /// </param>
        private static void ConfigureOptionsServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(_ => configuration);

            // Adds IOptions<AppSettings> to the services container.
            services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));

            // Adds IOptions<CacheProfileSettings> to the services container.
            services.Configure<CacheProfileSettings>(configuration.GetSection(nameof(CacheProfileSettings)));
        }
    }

    /// <summary>
    ///     The settings for the current application.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        ///     Gets or sets the short name of the application, used for display
        ///     purposes where the full name will be too long.
        /// </summary>
        public string SiteShortTitle { get; set; }

        /// <summary>
        ///     Gets or sets the full name of the application.
        /// </summary>
        public string SiteTitle { get; set; }
    }

    public class CacheProfileSettings
    {
        /// <summary>
        ///     Gets or sets the cache profiles (How long to cache things for).
        /// </summary>
        public Dictionary<string, CacheProfile> CacheProfiles { get; set; }
    }
}