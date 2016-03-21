using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Models
{
    public class Goal : DbEntity
    {
        public Goal()
        {
            Description = "None provided";
            ConcernId = Guid.Empty;
            RewardResourceId = Guid.Empty;
            FeedbackId = Guid.Empty;
            IsDeleted = false;
        }

        public string Description { get; set; }

        public bool IsDeleted { get; set; }

        public Guid? ConcernId { get; set; }

        [ForeignKey("ConcernId")]
        public virtual ConcernMatrix Concern { get; set; }

        public Guid? RewardResourceId { get; set; }

        [ForeignKey("RewardResourceId")]
        public virtual RewardResourceMatrix RewardResource { get; set; }

        public Guid? FeedbackId { get; set; }

        [ForeignKey("FeedbackId")]
        public virtual GoalFeedback Feedback { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<Activity> Activities { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<Action> Actions { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<Reward> Rewards { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<Role> Roles { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<Target> Targets { get; set; }

        public async Task<Reward> CalculateRewardFromAction(SocialGamificationAssetContext context, string actionVerb)
        {
            var actionMatch = Actions.FirstOrDefault(a => a.Verb.Equals(actionVerb));

            if (actionMatch != null)
            {
                actionMatch.Relations =
                    await
                    context.ActionRelations.Where(a => a.ActionId.Equals(actionMatch.Id))
                           .Include(ar => ar.AttributeChanges.Select(ac => ac.AttributeType))
                           .ToListAsync();

                foreach (var ar in actionMatch.Relations)
                {
                    foreach (var reward in ar.AttributeChanges)
                    {
                        var rewardMatch =
                            Rewards.Where(r => r.TypeReward.Equals(RewardType.Store))
                                   .Where(r => r.AttributeType.Name.Equals(reward.AttributeType.Name))
                                   .FirstOrDefault();

                        if (rewardMatch != null)
                        {
                            rewardMatch.Value += reward.Value;
                            return rewardMatch;
                        }
                    }
                }
            }

            return null;
        }
    }

    public class GoalFeedback : DbEntity
    {
        public float Threshold { get; set; }

        public Direction Direction { get; set; }

        public string Message { get; set; }

        public GoalFeedbackTarget Target { get; set; }
    }

    public enum Direction
    {
        Less,

        Equal,

        More
    }

    public enum GoalFeedbackTarget
    {
        Concern,

        RewardResource,

        AttributeType
    }

    public enum GoalStatus
    {
        Inactive,

        InProgress,

        Complete
    }
}