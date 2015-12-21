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

			// Seed the database with Test values
			seeder.Seed();
		}

		// Entry point for the application.
		public static void Main(string[] args) => WebApplication.Run<Startup>(args);
	}
}
