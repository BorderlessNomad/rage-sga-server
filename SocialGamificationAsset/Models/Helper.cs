﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Crypto = BCrypt.Net.BCrypt;

namespace SocialGamificationAsset.Models
{
    public class Helper
    {
        // public static List<string> allowedOperators = new List<string> { "=", "!", "%", ">", ">=", "<", "<=" };
        public static List<string> AllowedOperators = new List<string> { "=", "!", "%" };

        /**
		 * The modern version of the Fisher–Yates shuffle aka 'Algorithm P'
		 * -- To shuffle an array a of n elements (indices 0..n-1):
		 * for i from n−1 down to 1 do
		 *		j ← random integer such that 0 ≤ j ≤ i
		 *		exchange a[j] and a[i]
		 */

        public static IList<T> Shuffle<T>(IList<T> list, int limit = 1)
        {
            var r = new Random();
            for (var i = list.Count - 1; i >= 1; i--)
            {
                // 0 ≤ j ≤ i
                var j = r.Next(0, i + 1);
                var value = list[i];
                list[i] = list[j];
                list[j] = value;
            }

            return list.Take(limit).ToList();
        }

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

        public static JsonSerializerSettings JsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            return settings;
        }

        public static HttpResponseException ApiException(
            string message,
            HttpStatusCode StatusCode = HttpStatusCode.InternalServerError)
        {
            var response = new HttpResponseMessage(StatusCode);
            response.Content =
                new StringContent(
                    JsonConvert.SerializeObject(new { Message = message }, JsonSerializerSettings()),
                    Encoding.UTF8,
                    "application/json");

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return new HttpResponseException(response);
        }

        public static HttpResponseException ApiException(
            Exception e,
            HttpStatusCode StatusCode = HttpStatusCode.InternalServerError)
        {
            return ApiException(e.Message, StatusCode);
        }

        public static ContentResult JsonErrorContentResult(
            object value,
            int status = StatusCodes.Status500InternalServerError)
        {
            string content;

            if (value is string)
            {
                content = JsonConvert.SerializeObject(new { Error = value }, JsonSerializerSettings());
            }
            else
            {
                content = JsonConvert.SerializeObject(value, JsonSerializerSettings());
            }

            return new ContentResult
            {
                StatusCode = status,
                Content = content,
                ContentType = new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/json")
            };
        }

        public static ContentResult HttpNotFound(object value)
        {
            return JsonErrorContentResult(value, StatusCodes.Status404NotFound);
        }

        public static ContentResult HttpBadRequest(object value)
        {
            return JsonErrorContentResult(value, StatusCodes.Status400BadRequest);
        }

        public static ContentResult HttpUnauthorized(object value)
        {
            return JsonErrorContentResult(value, StatusCodes.Status401Unauthorized);
        }
    }
}