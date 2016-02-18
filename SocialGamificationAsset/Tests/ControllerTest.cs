using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNet.TestHost;

using SocialGamificationAsset.Models;

namespace SocialGamificationAsset.Tests
{
    public class ControllerTest
    {
        protected SocialGamificationAssetContext _context;

        protected readonly TestServer server;

        public ControllerTest(SocialGamificationAssetContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;

            server = new TestServer(TestServer.CreateBuilder().UseStartup<Startup>());

            /**
             * http://dotnetliberty.com/index.php/2015/10/22/unit-testing-mvc6-and-entityframework-7-with-xunit/
             *
             * http://dotnetliberty.com/index.php/2015/12/17/asp-net-5-web-api-integration-testing/
             *
             * http://www.jerriepelser.com/blog/unit-testing-aspnet5-entityframework7-inmemory-database
             */
        }
    }
}
