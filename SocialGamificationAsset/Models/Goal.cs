using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
	public class Goal : DbEntity
	{
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
