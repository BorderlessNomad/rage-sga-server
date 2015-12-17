using System.ComponentModel.DataAnnotations;

namespace SocialGamificationAsset.Models
{
	public class Test : Model
	{
		[Required]
		public string Username { get; set; }

		public string Password { get; set; }
	}
}
