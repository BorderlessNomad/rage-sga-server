using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
	public class MatchActor : Model
	{
		public Guid MatchId { get; set; }

		[ForeignKey("MatchId")]
		public Match Match { get; set; }

		public Guid ActorId { get; set; }

		[ForeignKey("ActorId")]
		public Actor Actor { get; set; }

		public ICollection<CustomData> CustomData { get; set; }
	}
}
