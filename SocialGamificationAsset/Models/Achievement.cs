using System;
using System.Collections.Generic;

namespace SocialGamificationAsset.Models
{
	public class Achievement : Model
	{
		public string Title { get; set; }

		public string Description { get; set; }

		public int Position { get; set; }

		public int Progress { get; set; }

		public virtual ICollection<Actor> Account { get; set; }
	}
}
