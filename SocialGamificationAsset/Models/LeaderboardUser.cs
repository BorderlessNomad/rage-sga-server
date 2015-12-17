using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Models
{
	public class LeaderboardUser : Model
	{
		public Guid LeaderboardId { get; set; }

		[ForeignKey("LeaderboardId")]
		public Leaderboard Leaderboard { get; set; }

		public Guid ActorId { get; set; }

		[ForeignKey("ActorId")]
		public Actor Actor { get; set; }

		public int ValueInt { get; set; }

		public int ValueFloat { get; set; }

		public string ValueString { get; set; }

		public DateTime LastUpdated { get; set; }
	}
}
