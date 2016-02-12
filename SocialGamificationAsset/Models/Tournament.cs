using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SocialGamificationAsset.Models
{
    public class Tournament : DbEntity
    {
        public Tournament()
        {
            Title = "Test Tournament";
            IsFinished = false;
        }

        public Guid OwnerId { get; set; }

        [IgnoreDataMember]
        [ForeignKey("OwnerId")]
        public virtual Actor Owner { get; set; }

        public string Title { get; set; }

        public bool IsFinished { get; set; }

        public DateTime? DateFinished { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<CustomData> CustomData { get; set; }
    }
}