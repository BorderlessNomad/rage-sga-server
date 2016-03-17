using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SocialGamificationAsset.Models
{
    public class Reward : DbEntity
    {
        public Guid AttributeTypeId { get; set; }

        [ForeignKey("AttributeTypeId")]
        public virtual AttributeType AttributeType { get; set; }

        public float Value { get; set; }

        public RewardStatus Status { get; set; }

        public Guid GoalId { get; set; }

        [ForeignKey("GoalId")]
        public virtual Goal Goal { get; set; }

        public RewardType TypeReward { get; set; }

        [DataMember(IsRequired = false)]
        public Guid? ActionRelationId { get; set; }

        [ForeignKey("ActionRelationId")]
        public virtual ActionRelation ActionRelation { get; set; }
    }

    public enum RewardStatus
    {
        InProgress,

        Completed
    }

    public enum RewardType
    {
        Modify,

        Store
    }
}