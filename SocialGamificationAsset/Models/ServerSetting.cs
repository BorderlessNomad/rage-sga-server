namespace SocialGamificationAsset.Models
{
	public class ServerSetting : Model
	{
		public static string ServerVersion = "1.0.0";

		public string DataKey { get; set; }

		public string DataValue { get; set; }
	}
}
