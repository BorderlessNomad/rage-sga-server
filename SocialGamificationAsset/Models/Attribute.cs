using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
	public class Attribute : DbEntity
	{
		public Guid AttributeTypeId { get; set; }

		[ForeignKey("AttributeTypeId")]
		public virtual AttributeType Type { get; set; }

		public float Value { get; set; }
	}

	public class AttributeType : DbEntity
	{
		public string Name { get; set; }

		public float DefaultValue { get; set; }

		public AttributeTypeEnum Type { get; set; }
	}

	public class AttributeHistory : DbEntity
	{
		public Guid TypeId { get; set; }

		[ForeignKey("TypeId")]
		public virtual AttributeType Type { get; set; }

		public Guid ActivityId { get; set; }

		[ForeignKey("ActivityId")]
		public virtual Activity Activity { get; set; }

		public float Value { get; set; }
	}

	public enum AttributeTypeEnum
	{
		Skill,
		Resource,
		Achievement
	}
}
