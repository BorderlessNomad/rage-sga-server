using Microsoft.AspNet.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocialGamificationAsset.Models;

namespace SocialGamificationAsset
{
	public partial class Startup
	{
		private static void ConfigureApplicationServices(IServiceCollection services, IConfiguration configuration)
		{
			// Add Application Context
			services.AddScoped<SocialGamificationAssetContext>();

			// Add DB Initiliazer Service
			services.AddTransient<SocialGamificationAssetInitializer>();
		}

		private static void ConfigureDatabaseInitialization(IApplicationBuilder application)
		{
			SocialGamificationAssetInitializer.InitializeDatabaseAsync(application.ApplicationServices).Wait();
		}
	}
}
