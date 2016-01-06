using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
	public struct CustomDataBase
	{
		public string Key { get; set; }

		public string Value { get; set; }

		public string Operator { get; set; }
	}

	public class CustomData : Model
	{
		public string Key { get; set; }

		public string Value { get; set; }

		public Guid ActorId { get; set; }

		[ForeignKey("ActorId")]
		public virtual Actor Actor { get; set; }
	}
}
