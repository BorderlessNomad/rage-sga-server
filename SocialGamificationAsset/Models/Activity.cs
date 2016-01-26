using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Models
{
	public class Activity : Model
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public string Image { get; set; }

		public virtual ICollection<AttributeType> Skills { get; set; }

		public virtual ICollection<Goal> Goals { get; set; }

		public virtual ICollection<Role> Roles { get; set; }
	}
}
