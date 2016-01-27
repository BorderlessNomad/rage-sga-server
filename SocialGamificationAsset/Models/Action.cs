﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
	public class Action : DbEntity
	{
		public string Verb { get; set; }

		public Guid ActivityId { get; set; }

		[ForeignKey("ActivityId")]
		public virtual Activity Activity { get; set; }

		public virtual ICollection<ActionRelation> Relations { get; set; }
	}

	public class ActionRelation : DbEntity
	{
		public Relationships Relationship { get; set; }

		public Matrix ConcernChange { get; set; }

		public Matrix RewardResourceChange { get; set; }

		public Guid ActionId { get; set; }

		[ForeignKey("ActionId")]
		public virtual Action Action { get; set; }

		public virtual ICollection<Reward> AttributeChanges { get; set; }
	}

	public class ActionHistory : DbEntity
	{
		public Guid ActorId { get; set; }

		[ForeignKey("ActorId")]
		public virtual Actor Actor { get; set; }

		public Guid ActionId { get; set; }

		[ForeignKey("ActionId")]
		public virtual Action Action { get; set; }

		public Guid ConcernId { get; set; }

		[ForeignKey("ConcernId")]
		public virtual ConcernMatrix Concern { get; set; }

		public Guid RewardResourceId { get; set; }

		[ForeignKey("RewardResourceId")]
		public virtual RewardResourceMatrix RewardResource { get; set; }
	}

	public enum Relationships
	{
		Friend,
		Enemy,
		Neutral
	}
}
