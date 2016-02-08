using System.Collections.Generic;

namespace SocialGamificationAsset.Models
{
    public enum LeaderboardUnique
    {
        None,

        Increase,

        ReplaceHigher,

        ReplaceAny
    }

    public enum LeaderboardOrder
    {
        Desc,

        Asc
    }

    public enum LeaderboardValue
    {
        Int,

        Float
    }

    public class Leaderboard : DbEntity
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public LeaderboardUnique UniqueRecords { get; set; }

        public LeaderboardOrder OrderType { get; set; }

        public LeaderboardValue ValueType { get; set; }

        public bool AllowAnonymous { get; set; }

        public virtual ICollection<LeaderboardUser> LeaderboardUsers { get; set; }
    }
}