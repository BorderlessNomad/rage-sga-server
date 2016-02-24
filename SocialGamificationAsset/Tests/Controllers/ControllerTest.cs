using System;
using System.Net.Http;
using System.Threading.Tasks;

using SocialGamificationAsset.Models;

using Xunit;

namespace SocialGamificationAsset.Tests.Controllers
{
    public class ControllerTest
    {
        public const string ServerUrl = "https://localhost:44363";

        protected readonly HttpClient _server;

        public ControllerTest()
        {
            _server = new HttpClient { BaseAddress = new Uri(ServerUrl) };

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

        protected async Task<Session> Login()
        {
            using (var client = _server.AcceptJson())
            {
                // login 
                var loginForm = new UserForm { Username = "mayur", Password = "mayur" };

                var loginResponse = await client.PostAsJsonAsync("/api/sessions", loginForm);
                Assert.True(loginResponse.IsSuccessStatusCode);

                var created = await loginResponse.Content.ReadAsJsonAsync<Session>();

                return created;
            }
        }
    }
}