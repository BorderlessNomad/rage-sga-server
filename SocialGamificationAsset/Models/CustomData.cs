using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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
			if (sourceData != null && sourceData.Count > 0)
			{
				for (int i = 0; i < sourceData.Count; i++)
				{
					CustomDataBase data = sourceData[i];

					if (!Helper.AllowedOperators.Contains(data.Operator))
					{
						continue;
					}

					customData.Add(data);
				}
			}

			return customData;
		}
	}

	public class CustomData : Model
	{
		public string Key { get; set; }

		public string Value { get; set; }

		public Guid ObjectId { get; set; }

		public CustomDataType ObjectType { get; set; }

		public static IList<CustomData> Parse(IList<CustomDataBase> sourceData, Guid objectId, CustomDataType objectType)
		{
			// Build the filter by CustomData
			IList<CustomData> customData = new List<CustomData>();

			if (sourceData != null && sourceData.Count > 0)
			{
				for (int i = 0; i < sourceData.Count; i++)
				{
					CustomDataBase data = sourceData[i];

					customData.Add(new CustomData()
					{
						Key = data.Key,
						Value = data.Value,
						ObjectId = objectId,
						ObjectType = objectType
					});
				}
			}

			return customData;
		}

		public static IQueryable<CustomData> ConditionBuilder(SocialGamificationAssetContext db, IList<CustomDataBase> sourceData, CustomDataType objectType)
		{
			IQueryable<CustomData> query = db.CustomData.Where(c => c.ObjectType == objectType);

			if (sourceData != null && sourceData.Count > 0)
			{
				foreach (CustomDataBase data in sourceData)
				{
					query = query.Where(c => c.Key.Equals(data.Key));

					if (!Helper.AllowedOperators.Contains(data.Operator))
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
