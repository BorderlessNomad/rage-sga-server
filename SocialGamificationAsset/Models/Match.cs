using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Models
{
	public class Match : Model
	{
		public Guid TournamentId { get; set; }

		[ForeignKey("TournamentId")]
		public Tournament Tournament { get; set; }

		public string Title { get; set; }

		public DateTime CreationTime { get; set; }

		public DateTime ExpirationDate { get; set; }

		public int Rounds { get; set; }

		public bool Finished { get; set; }

		public ICollection<MatchRound> MatchRounds { get; set; }
	}
}
