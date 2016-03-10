using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;

using Microsoft.AspNet.Mvc;

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
        }

        public virtual ICollection<Reward> Rewards { get; set; }

        public virtual ICollection<Target> Targets { get; set; }

        public Guid ConcernId { get; set; }

        [ForeignKey("ConcernId")]
        public virtual ConcernMatrix Concern { get; set; }

        public Guid RewardResourceId { get; set; }

        [ForeignKey("RewardResourceId")]
        public virtual RewardResourceMatrix RewardResource { get; set; }

        public virtual ICollection<Activity> Activities { get; set; }

        public virtual ICollection<Action> Actions { get; set; }

        public string Description { get; set; }

        public ICollection<Role> Roles { get; set; }

        public Guid FeedbackId { get; set; }

        [ForeignKey("FeedbackId")]
        public virtual GoalFeedback Feedback { get; set; }

        public bool CalculateRewardFromAction(SocialGamificationAssetContext context, string actionVerb)
        {
            Action actionMatch = this.Actions.Where(a => a.Verb.Equals(actionVerb)).FirstOrDefault();

            if (actionMatch != null)
            {
                actionMatch.Relations = context.ActionRelations.Where(a => a.ActionId.Equals(actionMatch.Id)).Include(ar => ar.AttributeChanges).ToList();

                foreach (ActionRelation ar in actionMatch.Relations)
                {
                    foreach (Reward reward in ar.AttributeChanges)
                    {
                        Reward rewardMatch = this.Rewards.Where(r => r.AttributeType.Name.Equals(reward.AttributeType.Name)).FirstOrDefault();

                        if (rewardMatch != null)
                        {
                            rewardMatch.Value += reward.Value;
                            return true;
                        }
                    }
                }
            }
            return false;
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