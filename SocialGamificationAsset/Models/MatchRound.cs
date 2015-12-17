using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Models
{
	public class MatchRound : Model
	{
		public Guid MatchActorId { get; set; }

		[ForeignKey("MatchActorId")]
		public MatchActor MatchActor { get; set; }

		public int Score { get; set; }

		public DateTime DateScore { get; set; }
	}
}
