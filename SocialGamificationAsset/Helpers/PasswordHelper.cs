using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Net.Http.Headers;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Crypto = BCrypt.Net.BCrypt;

namespace SocialGamificationAsset.Helpers
{
    public class PasswordHelper
    {
        public static string GenerateRandomSalt()
        {
            return Crypto.GenerateSalt(12);
        }

        public static string HashPassword(string plainTextPassword)
        {
            return Crypto.HashPassword(plainTextPassword, GenerateRandomSalt());
        }

        public static bool ValidatePassword(string password, string correctHash)
        {
            return Crypto.Verify(password, correctHash);
        }
    }
}