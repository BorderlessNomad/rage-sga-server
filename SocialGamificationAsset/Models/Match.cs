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

		public DateTime? ExpirationDate { get; set; }

		public int TotalRounds { get; set; }

		public bool Finished { get; set; }

		public virtual ICollection<MatchActor> Actors { get; set; }

		// public virtual ICollection<MatchRound> Rounds { get; set; } // Make this Virtual method

		public Match()
		{
			Title = "Test";
			TotalRounds = 1;
			Finished = false;
		}
	}

	public enum MatchType
	{
		Player,
		Group
	}

	public class QuickMatch
	{
		public MatchType Type { get; set; }

		public Guid? ActorId { get; set; }

		public bool FriendsOnly { get; set; }

		public int Actors { get; set; }

		public int Rounds { get; set; }

		public Guid? Tournament { get; set; }

		public IList<CustomDataBase> Data { get; set; }

		public QuickMatch()
		{
			Type = MatchType.Player;

			FriendsOnly = false;

			if (Actors < 2)
			{
				Actors = 2;
			}

			if (Rounds < 1)
			{
				Rounds = 1;
			}
		}
	}
}
