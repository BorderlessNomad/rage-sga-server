using Microsoft.AspNet.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SocialGamificationAsset
{
	public partial class Startup
	{
		private static void ConfigureDocumentationGeneratorServices(IServiceCollection services)
		{
			// SWASHBUCKLE SWAGGER API Documentation Generator
			services.AddSwaggerGen();
			services.ConfigureSwaggerDocument(options =>
			{
				options.SingleApiVersion(new Swashbuckle.SwaggerGen.Info
				{
					Version = "v1",
					Title = "Social Gamification API",
					Description = "This module allows to layer game mechanics affording game-inspired social relations and interactions on top a system to support engagement, collaboration, and learning. Two main forms of social interaction are supported: player-player interactions (such as matches) and group interactions (such as shared team goals or team vs. team competitions).",
					TermsOfService = "GPLv3"
				});
			});
		}

		private static void ConfigureDocumentationGenerator(IApplicationBuilder application)
		{
			application.UseSwaggerGen();
			application.UseSwaggerUi();
		}
	}
}
