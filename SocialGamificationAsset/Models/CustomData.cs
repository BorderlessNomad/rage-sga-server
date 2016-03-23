using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNet.Mvc;

namespace SocialGamificationAsset.Models
{
    public struct CustomDataBase
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public string Operator { get; set; }

        public static IList<CustomDataBase> Parse(IList<CustomDataBase> sourceData)
        {
            // Build the filter by CustomData
            IList<CustomDataBase> customData = new List<CustomDataBase>();
            if (sourceData == null || !sourceData.Any())
            {
                return customData;
            }

            foreach (var data in sourceData.Where(data => CustomData.AllowedOperators.Contains(data.Operator)))
            {
                customData.Add(data);
            }

            return customData;
        }
    }

    public class CustomData : DbEntity
    {
        // public static List<string> allowedOperators = new List<string> { "=", "!", "%", ">", ">=", "<", "<=" };
        public static List<string> AllowedOperators = new List<string> { "=", "!", "%" };

        public string Key { get; set; }

        public string Value { get; set; }

        public Guid ObjectId { get; set; }

        public CustomDataType ObjectType { get; set; }

        public static async Task<ContentResult> AddOrUpdate(
            SocialGamificationAssetContext db,
            IList<CustomDataBase> sourceData,
            Guid objectId,
            CustomDataType objectType)
        {
            // Build the filter by CustomData
            if (sourceData != null && sourceData.Count > 0)
            {
                foreach (var data in sourceData)
                {
                    var customData =
                        await
                        db.CustomData.Where(c => c.Key.Equals(data.Key))
                          .Where(c => c.ObjectId.Equals(objectId))
                          .Where(c => c.ObjectType == objectType)
                          .FirstOrDefaultAsync();

                    if (customData != null)
                    {
                        db.Entry(customData).State = EntityState.Modified;
                        customData.Value = data.Value;
                    }
                    else
                    {
                        customData = new CustomData
                        {
                            Key = data.Key,
                            Value = data.Value,
                            ObjectId = objectId,
                            ObjectType = objectType
                        };

                        db.CustomData.Add(customData);
                    }
                }
            }

            return await SaveChanges(db, true);
        }

        public static IQueryable<CustomData> ConditionBuilder(
            SocialGamificationAssetContext db,
            IList<CustomDataBase> sourceData,
            CustomDataType objectType)
        {
            var query = db.CustomData.Where(c => c.ObjectType == objectType);

            if (sourceData == null || sourceData.Count <= 0)
            {
                return null;
            }

            foreach (var data in sourceData)
            {
                query = query.Where(c => c.Key.Equals(data.Key));

                if (!AllowedOperators.Contains(data.Operator))
                {
                    continue;
                }

                switch (data.Operator)
                {
                    case "=":
                        query = query.Where(c => c.Value.Equals(data.Value));
                        break;
                    case "!":
                        query = query.Where(c => c.Value != data.Value);
                        break;
                    case "%":
                        query = query.Where(c => c.Value.Contains(data.Value));
                        break;
                }
            }

            return query;
        }
    }

    public enum CustomDataType
    {
        Player,

        Group,

        Match,

        Tournament
    }
}