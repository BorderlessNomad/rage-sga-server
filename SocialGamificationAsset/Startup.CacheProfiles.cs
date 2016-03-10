using System.Collections.Generic;

using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;

namespace SocialGamificationAsset
{
    public partial class Startup
    {
        /// <summary>
        ///     Controls how controller actions cache content in one central
        ///     location.
        /// </summary>
        /// <param name="cacheProfiles">
        ///     The settings for the
        ///     <see cref="Microsoft.AspNet.Mvc.ResponseCacheAttribute" /> 's.
        /// </param>
        /// <param name="configuration">
        ///     Gets or sets the application configuration, where key value pair
        ///     settings are stored.
        /// </param>
        private static void ConfigureCacheProfiles(
            IDictionary<string, CacheProfile> cacheProfiles,
            IConfiguration configuration)
        {
            var configurationSection = configuration.GetSection(nameof(CacheProfileSettings));
            var cacheProfileSettings = new CacheProfileSettings();
            configurationSection.Bind(cacheProfileSettings);

            if (cacheProfileSettings.CacheProfiles == null)
            {
                return;
            }

            foreach (var keyValuePair in cacheProfileSettings.CacheProfiles)
            {
                cacheProfiles.Add(keyValuePair);
            }
        }
    }
}