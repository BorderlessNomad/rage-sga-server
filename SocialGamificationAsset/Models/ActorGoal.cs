using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
    public class ActorGoal : DbEntity
    {
        public ActorGoal()
        {
            ActorId = Guid.Empty;
            GoalId = Guid.Empty;
            Status = GoalStatus.InProgress;
            ConcernOutcomeId = Guid.Empty;
            RewardResourceOutcomeId = Guid.Empty;
            ActivityId = Guid.Empty;
            RoleId = Guid.Empty;
        }
        public Guid ActorId { get; set; }

        [ForeignKey("ActorId")]
        public virtual Player Actor { get; set; }

        public Guid GoalId { get; set; }

        [ForeignKey("GoalId")]
        public virtual Goal Goal { get; set; }

        public GoalStatus Status { get; set; }

        public Guid ConcernOutcomeId { get; set; }

        [ForeignKey("ConcernOutcomeId")]
        public virtual ConcernMatrix ConcernOutcome { get; set; }

        public Guid RewardResourceOutcomeId { get; set; }

        [ForeignKey("RewardResourceOutcomeId")]
        public virtual RewardResourceMatrix RewardResourceOutcome { get; set; }

        public Guid ActivityId { get; set; }

        [ForeignKey("ActivityId")]
        public virtual Activity Activity { get; set; }

        public Guid RoleId { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
    }
}