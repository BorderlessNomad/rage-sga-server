using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
	public class Group : Model
	{
		public string Name { get; set; }

		public Guid OwnerId { get; set; }

		[ForeignKey("OwnerId")]
		public virtual Actor Owner { get; set; }

		public bool IsPublic { get; set; }

		public virtual ICollection<Actor> Actors { get; set; }

		public virtual ICollection<CustomData> CustomData { get; set; }
	}
}
