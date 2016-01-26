using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Models
{
	public class Attribute : Model
	{
		public Guid AttributeTypeId { get; set; }

		[ForeignKey("AttributeTypeId")]
		public virtual AttributeType Type { get; set; }

		public float Value { get; set; }
	}

	public class AttributeType : Model
	{
		public string Name { get; set; }

		public float DefaultValue { get; set; }

		public AttributeTypeEnum Type { get; set; }
	}

	public enum AttributeTypeEnum
	{
		Skill,
		Resource,
		Achievement
	}
}
