using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
	public class Group : Actor
	{
		public string Name { get; set; }

		public bool IsPublic { get; set; }

		public virtual ICollection<Actor> Actors { get; set; }
	}
}
