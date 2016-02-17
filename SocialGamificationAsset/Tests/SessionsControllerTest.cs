using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;

using SocialGamificationAsset.Controllers;
using SocialGamificationAsset.Models;

namespace SocialGamificationAsset.Tests.Controllers
{
    public class SessionsControllerTest
    {
        private readonly IServiceProvider serviceProvider;

        private SocialGamificationAssetContext _context;

        private SessionsController _controller;

        public SessionsControllerTest()
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                _context = serviceScope.ServiceProvider.GetService<SocialGamificationAssetContext>();
            }

            // Initialize DbContext in memory
            var optionsBuilder = new DbContextOptionsBuilder();

            // http://dotnetliberty.com/index.php/2015/12/17/asp-net-5-web-api-integration-testing/

            // http://dotnetliberty.com/index.php/2015/10/22/unit-testing-mvc6-and-entityframework-7-with-xunit/

            // http://www.jerriepelser.com/blog/unit-testing-aspnet5-entityframework7-inmemory-database
        }
    }
}
