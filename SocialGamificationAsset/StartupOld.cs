using Boilerplate.Web.Mvc.Formatters;
using Glimpse;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json.Serialization;
using SocialGamificationAsset.Models;
using SocialGamificationAsset.Policies;
using System.Diagnostics;

namespace SocialGamificationAsset
{
	public class StartupOld
	{
		public StartupOld(IHostingEnvironment env, IApplicationEnvironment appEnv)
		{
			IConfigurationBuilder builder = new ConfigurationBuilder();

			// Set Base path same as Application's base path
			builder.SetBasePath(appEnv.ApplicationBasePath);

			// Add configuration from the config.json file.
			builder.AddJsonFile("config.json");

			// Add configuration from an optional config.development.json, config.staging.json or
			// config.production.json file, depending on the environment. These settings override the ones in the
			// config.json file.
			builder.AddJsonFile($"config.{env.EnvironmentName}.json", optional: true);

			// Add configuration specific to the Development, Staging or Production environments. This config can
			// be stored on the machine being deployed to or if you are using Azure, in the cloud. These settings
			// override the ones in all of the above config files.
			builder.AddEnvironmentVariables();

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

			// Add Glimpse to help with debugging (See http://getglimpse.com/).
			services.AddGlimpse();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMvcBuilder mvcBuilder, ILoggerFactory loggerFactory, SocialGamificationAssetInitializer dbInitializer)
		{
			Debug.WriteLine("Starting ", Configuration["site_name"]);

			// Configures the JSON output formatter to use camel case property names like 'propertyName' instead of
			// pascal case 'PropertyName' as this is the more common JavaScript/JSON style.
			mvcBuilder.AddJsonOptions(x => x.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

			loggerFactory.AddConsole(minLevel: LogLevel.Warning);
			loggerFactory.AddDebug();

			// Add the following services only in development environment.
			if (env.IsDevelopment())
			{
				// Browse to /runtimeinfo to see information about the runtime that is being used and the packages that
				// are included in the application. See http://docs.asp.net/en/latest/fundamentals/diagnostics.html
				app.UseRuntimeInfoPage();

				// Allow updates to your files in Visual Studio to be shown in the browser. You can use the Refresh
				// browser link button in the Visual Studio toolbar or Ctrl+Alt+Enter to refresh the browser.
				app.UseBrowserLink();

				// When an error occurs, displays a detailed error page with full diagnostic information. It is unsafe
				// to use this in production. See http://docs.asp.net/en/latest/fundamentals/diagnostics.html
				app.UseDeveloperExceptionPage();

				// When a database error occurs, displays a detailed error page with full diagnostic information. It is
				// unsafe to use this in production.
				app.UseDatabaseErrorPage();

				// Add Glimpse to help with debugging (See http://getglimpse.com/).
				app.UseGlimpse();
			}
			else
			{
				// app.UseExceptionHandler("/Home/Error");
				app.UseStatusCodePagesWithReExecute("/error/{0}");
			}

			app.UseIISPlatformHandler();

			// Configure Session.
			app.UseSession();

			// Add static files to the request pipeline
			app.UseStaticFiles();

			app.UseCors("AllowAll");

			app.UseMvcWithDefaultRoute();

			// Seed the database with Test values
			// TODO: https://github.com/aspnet/MusicStore/blob/master/src/MusicStore/Models/SampleData.cs
			dbInitializer.Seed().Wait();

			app.UseSwaggerGen();
			app.UseSwaggerUi();

			// TODO: HTTPS https://github.com/RehanSaeed/ASP.NET-MVC-Boilerplate/blob/master/Source/MVC6/Boilerplate.Web.Mvc6.Sample/Startup.ContentSecurityPolicy.cs
		}

		// Entry point for the application.
		public static void Main(string[] args) => WebApplication.Run<StartupOld>(args);
	}
}
