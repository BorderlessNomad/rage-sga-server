using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using SocialGamificationAsset.Models;

namespace SocialGamificationAsset.Tests.Controllers
{
    public class ActorJsonConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType) => objectType == typeof(Actor);

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);

            if (jo["class"].Value<string>() == "Player")
            {
                return jo.ToObject<Player>(serializer);
            }

            if (jo["class"].Value<string>() == "Group")
            {
                return jo.ToObject<Group>(serializer);
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}