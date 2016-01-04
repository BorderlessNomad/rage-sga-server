using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
	public class Session : Model
	{
		public Guid ActorId { get; set; }

		[ForeignKey("ActorId")]
		public virtual Actor Actor { get; set; }

		public string LastActionIP { get; set; }

		public bool IsExpired { get; set; }

		public Session()
		{
			this.IsExpired = false;
		}
	}
}
