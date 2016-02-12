using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SocialGamificationAsset.Models
{
    public class Achievement : DbEntity
    {
        public Guid TypeId { get; set; }

        [ForeignKey("TypeId")]
        public virtual AchievementType Type { get; set; }

        public Guid ActorId { get; set; }

        [IgnoreDataMember]
        [ForeignKey("ActorId")]
        public virtual Actor Actor { get; set; }

        public Guid ActivityId { get; set; }

        [IgnoreDataMember]
        [ForeignKey("ActivityId")]
        public virtual Activity Activity { get; set; }
    }

    public class AchievementType : DbEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public Guid GoalId { get; set; }

        [IgnoreDataMember]
        [ForeignKey("GoalId")]
        public virtual Goal Goal { get; set; }
    }
}