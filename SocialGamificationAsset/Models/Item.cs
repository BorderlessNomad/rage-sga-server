using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
	public class Item : DbEntity
	{
		public Guid ActorId { get; set; }

		[ForeignKey("ActorId")]
		public virtual Actor Actor { get; set; }

		public Guid ItemTypeId { get; set; }

		[ForeignKey("ItemTypeId")]
		public virtual ItemType Type { get; set; }

		public int Quantity;
	}

	public class ItemType : DbEntity
	{
		public string Name { get; set; }

		public string Image { get; set; }
	}

	public class ItemForm
	{
		public Guid? ActorId { get; set; }

		public Guid ItemTypeId { get; set; }

		public int? Quantity;
	}
}
