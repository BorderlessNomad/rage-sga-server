namespace SocialGamificationAsset.Models
{
	public class ServerSetting : Model
	{
		public static string ServerVersion = "0.0.1";

		public string DataKey { get; set; }

		public string DataValue { get; set; }
	}
}
