using System.IO;

using Boilerplate.Web.Mvc;

using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;

using Serilog;

namespace SocialGamificationAsset
{
    /// <summary>
    ///     The main start-up class for the application.
    /// </summary>
    public partial class Startup
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="appEnv">
        ///     The location the application is running in.
        /// </param>
        /// <param name="hostingEnv">
        ///     The environment the application is running under. This can be
        ///     Development, Staging or Production by default.
        /// </param>
        public Startup(IApplicationEnvironment appEnv, IHostingEnvironment hostingEnv)
        {
            this.appEnv = appEnv;
            this.hostingEnv = hostingEnv;
            configuration = ConfigureConfiguration(hostingEnv);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole()
                .CreateLogger();
        }

        #endregion Constructors

        /// <summary>
        ///     Entry point for the application.
        /// </summary>
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);

        #region Fields

        /// <summary>
        ///     The location the application is running in.
        /// </summary>
        private readonly IApplicationEnvironment appEnv;

        /// <summary>
        ///     Gets or sets the application configuration, where key value pair
        ///     settings are stored. See
        ///     http://docs.asp.net/en/latest/fundamentals/configuration.html
        ///     http://weblog.west-wind.com/posts/2015/Jun/03/Strongly-typed-AppSettings-Configuration-in-ASPNET-5
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        ///     The environment the application is running under. This can be
        ///     Development, Staging or Production by default. To set the hosting
        ///     environment on Windows:
        /// </summary>
        private readonly IHostingEnvironment hostingEnv;

        #endregion Fields

        #region Public Methods

        /// <summary>
        ///     Configures the <paramref name="services" /> to add to the ASP.NET
        ///     MVC 6 Injection of Control (IoC) container. This method gets called
        ///     by the ASP.NET runtime. See:
        ///     http://blogs.msdn.com/b/webdev/archive/2014/06/17/dependency-injection-in-asp-net-vnext.aspx
        /// </summary>
        /// <param name="services">
        ///     The services collection or IoC container.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureDebuggingServices(services, hostingEnv);

            ConfigureOptionsServices(services, configuration);

            ConfigureApplicationServices(services, configuration);

            ConfigureCachingServices(services);

            // Configure MVC routing. We store the route options for use by ConfigureSearchEngineOptimizationFilters.
            RouteOptions routeOptions = null;

            services.ConfigureRouting(
                x =>
                    {
                        routeOptions = x;

                        ConfigureRouting(services, x);
                    });

            // Add many MVC services to the services container.
            var mvcBuilder = services.AddMvc(
                mvcOptions =>
                    {
                        ConfigureCacheProfiles(mvcOptions.CacheProfiles, configuration);

                        ConfigureSecurityFilters(hostingEnv, mvcOptions.Filters);

                        ConfigureFormatters(mvcOptions);
                    });

            ConfigureFormatters(mvcBuilder);

            ConfigureSessionServices(services);

            ConfigureDocumentationGeneratorServices(services, appEnv);
        }

        /// <summary>
        ///     Configures the <paramref name="application" /> and HTTP request
        ///     pipeline. <see cref="Startup.Configure" /> is called after
        ///     <see cref="Startup.ConfigureServices" /> is called by the ASP.NET
        ///     runtime.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="loggerfactory">The logger factory.</param>
        public void Configure(IApplicationBuilder application, ILoggerFactory loggerfactory)
        {
            // Give the ASP.NET MVC Boilerplate NuGet package assembly access to the HttpContext, so it can generate
            // absolute URL's and get the current request path.
            application.UseBoilerplate();

            // Add the IIS platform handler to the request pipeline.
            application.UseIISPlatformHandler();

            // Add static files to the request pipeline e.g. hello.html or world.css.
            application.UseStaticFiles();

            ConfigureDebugging(application, hostingEnv);

            ConfigureLogging(application, hostingEnv, loggerfactory, configuration);

            ConfigureErrorPages(application, hostingEnv);

            ConfigureFormatters(application);

            ConfigureHeadersOverride(application);

            ConfigureSecurity(application, hostingEnv);

            ConfigureSession(application);

            ConfigureCors(application);

            // Add MVC to the request pipeline.
            application.UseMvcWithDefaultRoute();

            // Add a 404 Not Found error page for visiting /this-resource-does-not-exist.
            Configure404NotFoundErrorPage(application, hostingEnv);

            ConfigureDocumentationGenerator(application);

            // Seed the database with Test values
            ConfigureDatabaseInitialization(application);
        }

        #endregion Public Methods
    }
}