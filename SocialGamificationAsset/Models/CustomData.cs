using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
				// Check for allowed operators
				List<string> allowedOperators = new List<string> { "=", "!", "%", ">", ">=", "<", "<=" };

				for (int i = 0; i < sourceData.Count; i++)
				{
					CustomDataBase data = sourceData[i];

					if (!allowedOperators.Contains(data.Operator))
					{
						continue;
					}

					switch (data.Operator)
					{
						case "!": // Unequal
							data.Operator = "<>";
							break;

						case "%": // Like
							data.Operator = "REGEXP";
							break;
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
				// Check for allowed operators
				List<string> allowedOperators = new List<string> { "=", "!", "%", ">", ">=", "<", "<=" };

				for (int i = 0; i < sourceData.Count; i++)
				{
					CustomDataBase data = sourceData[i];

					if (!allowedOperators.Contains(data.Operator))
					{
						continue;
					}

					switch (data.Operator)
					{
						case "!": // Unequal
							data.Operator = "<>";
							break;

						case "%": // Like
							data.Operator = "REGEXP";
							break;
					}

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
	}

	public enum CustomDataType
	{
		Player,
		Group,
		Match,
		Tournament
	}
}
