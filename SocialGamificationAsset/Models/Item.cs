using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
	public class Item : Model
	{
		public Guid ActorId { get; set; }

		[ForeignKey("ActorId")]
		public virtual Actor Actor { get; set; }

		public Guid ItemTypeId { get; set; }

		[ForeignKey("ItemTypeId")]
		public virtual ItemType Type { get; set; }

		public int Quantity;
	}

	public class ItemType : Model
	{
		public string Name { get; set; }

		public string Image { get; set; }
	}
}
