using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SocialGamificationAsset.Models
{
    public class Inventory : DbEntity
    {
        public string Name;

        public int Quantity;

        public Guid ActorId { get; set; }

        [IgnoreDataMember]
        [ForeignKey("ActorId")]
        public virtual Actor Actor { get; set; }

        public Guid CustomDataId { get; set; }

        [ForeignKey("CustomDataId")]
        public virtual CustomData CustomData { get; set; }
    }
}