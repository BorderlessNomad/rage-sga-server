using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
    public class Match : DbEntity
    {
        // public virtual ICollection<MatchRound> Rounds { get; set; } // Make this Virtual method

        public Match()
        {
            this.Title = "Test";
            this.TotalRounds = 1;
            this.IsFinished = false;
            this.IsDeleted = false;
        }

        public Guid TournamentId { get; set; }

        [ForeignKey("TournamentId")]
        public Tournament Tournament { get; set; }

        public string Title { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public int TotalRounds { get; set; }

        public bool IsFinished { get; set; }

        public bool IsDeleted { get; set; }

        public virtual ICollection<MatchActor> Actors { get; set; }

        [NotMapped]
        public virtual ICollection<CustomData> CustomData { get; set; }
    }

    public enum MatchType
    {
        Player,

        Group
    }

    public class QuickMatch
    {
        public QuickMatch()
        {
            this.Type = MatchType.Player;

            this.AlliancesOnly = false;

            if (this.Actors < 2)
            {
                this.Actors = 2;
            }

            if (this.Rounds < 1)
            {
                this.Rounds = 1;
            }
        }

        public MatchType Type { get; set; }

        public Guid? ActorId { get; set; }

        public bool AlliancesOnly { get; set; }

        public int Actors { get; set; }

        public int Rounds { get; set; }

        public Guid? Tournament { get; set; }

        public IList<CustomDataBase> CustomData { get; set; }
    }
}