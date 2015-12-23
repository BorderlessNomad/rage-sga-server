using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using SocialGamificationAsset.Models;
using SocialGamificationAsset.Policies;
using System.Diagnostics;

namespace SocialGamificationAsset
{
	public class Startup
	{
		public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(appEnv.ApplicationBasePath)
				.AddJsonFile("config.json")

				// All environment variables in the process's context flow in as configuration values.
				.AddEnvironmentVariables()
			;

			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; set; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IConfiguration>(_ => Configuration);

			// Add Application Context
			services.AddScoped<SocialGamificationAssetContext>();

			// Add DB Initiliazer Service
			services.AddTransient<SocialGamificationAssetInitializer>();

			// Add MVC services to the services container
			services.AddMvc(options =>
			{
			});

			// Add memory cache services
			services.AddCaching();

			// Add session related services.
			services.AddSession();

			// Add CORS support to the service
			services.AddCors(options =>
			{
				options.AddPolicy("AllowAll", builder =>
				{
					builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
				});
			});

			// Configure Auth
			services.AddSingleton<ISessionAuthorizeFilter, SessionAuthorizeFilter>();

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

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, SocialGamificationAssetInitializer dbInitializer)
		{
			Debug.WriteLine("Starting ", Configuration["site_name"]);

			loggerFactory.AddConsole(minLevel: LogLevel.Warning);
			loggerFactory.AddDebug();

			if (env.IsDevelopment())
			{
				app.UseBrowserLink();
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
				app.UseRuntimeInfoPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseIISPlatformHandler();

			// Configure Session.
			app.UseSession();

			// Add static files to the request pipeline
			app.UseStaticFiles();

			app.UseCors("AllowAll");

			app.UseMvcWithDefaultRoute();

			// Seed the database with Test values
			dbInitializer.Seed();

			app.UseSwaggerGen();
			app.UseSwaggerUi();
		}

		// Entry point for the application.
		public static void Main(string[] args) => WebApplication.Run<Startup>(args);
	}
}
