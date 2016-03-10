using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
    public class MatchRound : DbEntity
    {
        public int RoundNumber { get; set; }

        public Guid MatchActorId { get; set; }

        [ForeignKey("MatchActorId")]
        public MatchActor MatchActor { get; set; }

        public int Score { get; set; }

        public DateTime? DateScore { get; set; }
    }

    public class MatchRoundForm
    {
        [Required]
        public Guid ActorId { get; set; }

        [Required]
        public int RoundNumber { get; set; }

        [Required]
        public int Score { get; set; }
    }

    public class MatchRoundActor
    {
        public Guid ActorId { get; set; }

        public Player Actor { get; set; }

        public int Score { get; set; }

        public DateTime? DateScore { get; set; }
    }

    public class MatchRoundResponse
    {
        public int RoundNumber { get; set; }

        public IList<MatchRoundActor> Actors { get; set; }
    }

    public class MatchRoundScoreResponse
    {
        public int RoundNumber { get; set; }

        public Guid ActorId { get; set; }

        public Player Actor { get; set; }

        public int Score { get; set; }

        public DateTime? DateScore { get; set; }
    }
}