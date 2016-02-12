using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SocialGamificationAsset.Models
{
    public class Activity : DbEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<AttributeType> Skills { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<Goal> Goals { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<Role> Roles { get; set; }
    }
}