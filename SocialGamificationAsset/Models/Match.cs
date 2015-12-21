using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
	public class Match : Model
	{
		public Guid TournamentId { get; set; }

		[ForeignKey("TournamentId")]
		public Tournament Tournament { get; set; }

		public string Title { get; set; }

		public DateTime ExpirationDate { get; set; }

		public int TotalRounds { get; set; }

		public bool Finished { get; set; }

		public ICollection<MatchRound> Rounds { get; set; }
	}
}
