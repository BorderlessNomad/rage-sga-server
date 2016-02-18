using System;
using System.Net.Http;

namespace SocialGamificationAsset.Tests
{
    public class ControllerTest
    {
        protected readonly HttpClient _server;

        public ControllerTest()
        {
            _server = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44363/api"),
            };

            /**
             * http://dotnetliberty.com/index.php/2015/10/22/unit-testing-mvc6-and-entityframework-7-with-xunit/
             *
             * http://docs.asp.net/en/latest/testing/integration-testing.html 
             *
             * http://dotnetliberty.com/index.php/2015/12/17/asp-net-5-web-api-integration-testing/
             *
             * http://www.jerriepelser.com/blog/unit-testing-aspnet5-entityframework7-inmemory-database
             */
        }
    }
}