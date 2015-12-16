using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
	public class Session : Model
	{
		public Guid ActorId { get; set; }

		[ForeignKey("ActorId")]
		public virtual Actor Actor { get; set; }

		public DateTime LastActionDate { get; set; }

		public string LastActionIP { get; set; }

		public DateTime SignitureTimestamp { get; set; }
	}
}
