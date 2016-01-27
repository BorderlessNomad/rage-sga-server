using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
	public class Reward : DbEntity
	{
		public Guid AttributeTypeId { get; set; }

		[ForeignKey("AttributeTypeId")]
		public virtual AttributeType AttributeType { get; set; }

		public float Value { get; set; }

		public RewardStatus Status { get; set; }
	}

	public enum RewardStatus
	{
		InProgress,
		Completed
	}
}
