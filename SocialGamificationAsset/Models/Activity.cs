using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SocialGamificationAsset.Models
{
	public class Activity : DbEntity
	{
		[Required]
		public string Name { get; set; }

		public string Description { get; set; }

		public string Image { get; set; }

        // [IgnoreDataMember]
        public virtual ICollection<AttributeType> Skills { get; set; }

        // [IgnoreDataMember]
        public virtual ICollection<Goal> Goals { get; set; }

        // [IgnoreDataMember]
        public virtual ICollection<Role> Roles { get; set; }
    }

    public class ActivityGoalForm
    {
        public string Description { get; set; }
    }

	public class ActivityDropdown
	{
		public string Id { get; set; }

		public string Name { get; set; }
	}
}