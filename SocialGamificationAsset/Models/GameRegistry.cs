using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
	public class GameRegistry : DbEntity
	{
		[Index(IsUnique = true)]
		[StringLength(128)]
		[Required]
		public string Name { get; set; }

		[Required]
		public string DeveloperName { get; set; }

		public string Url { get; set; }
	}
}
