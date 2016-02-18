using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;

using SocialGamificationAsset.Controllers;
using SocialGamificationAsset.Models;

namespace SocialGamificationAsset.Tests.Controllers
{
    public class SessionsControllerTest : ControllerTest
    {
        private SessionsController _controller;

        public SessionsControllerTest(SocialGamificationAssetContext context)
            : base(context)
        {
        }
    }
}
