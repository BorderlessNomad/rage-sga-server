using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
	public class LeaderboardUser : DbEntity
	{
		public Guid LeaderboardId { get; set; }

		[ForeignKey("LeaderboardId")]
		public Leaderboard Leaderboard { get; set; }

		public Guid ActorId { get; set; }

		[ForeignKey("ActorId")]
		public Actor Actor { get; set; }

		public int ValueInt { get; set; }

		public double ValueFloat { get; set; }

		public string ValueString { get; set; }
	}
}
