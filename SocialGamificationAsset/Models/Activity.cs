using System.Collections.Generic;

namespace SocialGamificationAsset.Models
{
    public class Activity : DbEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public virtual ICollection<AttributeType> Skills { get; set; }

        public virtual ICollection<Goal> Goals { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}