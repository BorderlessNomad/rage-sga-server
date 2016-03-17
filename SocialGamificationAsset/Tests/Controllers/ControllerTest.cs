using System;
using System.Net.Http;
using System.Threading.Tasks;

using SocialGamificationAsset.Models;

using Xunit;

namespace SocialGamificationAsset.Tests.Controllers
{
    public class ControllerTest
    {
        // public const string ServerUrl = "https://localhost:44363";
        public const string ServerUrl = "http://localhost:8063";

        protected readonly HttpClient _server;

        public ControllerTest()
        {
            _server = new HttpClient { BaseAddress = new Uri(ServerUrl) };
        }

        protected async Task<Session> Login(string username = "mayur", string password = "mayur")
        {
            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson();

                // login 
                var loginForm = new UserForm { Username = username, Password = password };

                var loginResponse = await client.PostAsJsonAsync("/api/sessions", loginForm);
                Assert.True(loginResponse.IsSuccessStatusCode);

                var created = await loginResponse.Content.ReadAsJsonAsync<Session>();

                return created;
            }
        }
    }
}