using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SocialGamificationAsset.Models
{
    public class Item : DbEntity
    {
        public Guid ActorId { get; set; }

        [IgnoreDataMember]
        [ForeignKey("ActorId")]
        public virtual Actor Actor { get; set; }

        public Guid ItemTypeId { get; set; }

        [IgnoreDataMember]
        [ForeignKey("ItemTypeId")]
        public virtual ItemType Type { get; set; }

        public int Quantity { get; set; }
    }

    public class ItemType : DbEntity
    {
        [Index(IsUnique = true)]
        [StringLength(128)]
        [Required]
        public string Name { get; set; }

        public string Image { get; set; }
    }

    public class ItemForm
    {
        public Guid? ActorId { get; set; }

        public Guid? ItemTypeId { get; set; }

        [Required]
        public string ItemTypeName { get; set; }

        public int? Quantity { get; set; }

        public ItemOperation Operation { get; set; }
    }

    public class ItemTypeResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public int Total { get; set; }

        public DateTime LastUpdated { get; set; }
    }

    public enum ItemOperation
    {
        Add,

        Remove
    }
}