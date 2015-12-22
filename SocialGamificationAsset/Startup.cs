using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using SocialGamificationAsset.Models;
using System.Diagnostics;

namespace SocialGamificationAsset
{
	public class Startup
	{
		public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
		{
			var builder = new ConfigurationBuilder()
				.AddJsonFile("config.json")
				.AddEnvironmentVariables() //All environment variables in the process's context flow in as configuration values.
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

			// Add framework services.
			services.AddMvc();

			// Add CORS support to the service
			services.AddCors(options =>
			{
				options.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
			});

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
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, SocialGamificationAssetInitializer seeder)
		{
			Debug.WriteLine("Starting ", Configuration["site_name"]);

			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			if (env.IsDevelopment())
			{
				app.UseBrowserLink();
				app.UseDeveloperExceptionPage();
				app.UseRuntimeInfoPage(); // default path is /runtimeinfo
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseIISPlatformHandler();

			app.UseStaticFiles();

			app.UseMvcWithDefaultRoute();

			app.UseCors("AllowAll");

			// Seed the database with Test values
			seeder.Seed();

			app.UseSwaggerGen();
			app.UseSwaggerUi();
		}

		// Entry point for the application.
		public static void Main(string[] args) => WebApplication.Run<Startup>(args);
	}
}
