using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
	public class File : DbEntity
	{
		public Guid ActorId { get; set; }

		[ForeignKey("ActorId")]
		public virtual Actor Actor { get; set; }

		public string Name { get; set; }

		public string Url { get; set; }

		public string ShareType { get; set; }

		public int Likes { get; set; }

		public int Views { get; set; }

		public virtual ICollection<FileActivity> Activities { get; set; }
	}
}
