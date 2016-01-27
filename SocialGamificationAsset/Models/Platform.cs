using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
	public class Platform : DbEntity
	{
		public string Key { get; set; }

		public Guid ActorId { get; set; }

		[ForeignKey("ActorId")]
		public virtual Actor Account { get; set; }
	}
}
