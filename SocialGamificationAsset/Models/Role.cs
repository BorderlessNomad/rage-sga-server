using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
    public class Role : DbEntity
    {
        [Index(IsUnique = true)]
        [StringLength(128)]
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public Guid GoalId { get; set; }

        [ForeignKey("GoalId")]
        public Goal Goal { get; set; }

        public Guid ActivityId { get; set; }

        [ForeignKey("ActivityId")]
        public Activity Activity { get; set; }
    }
}