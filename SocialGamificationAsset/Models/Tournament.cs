using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
	public class Tournament : Model
	{
		public Guid OwnerId { get; set; }

		[ForeignKey("OwnerId")]
		public virtual Actor Owner { get; set; }

		public string Title { get; set; }

		public bool IsFinished { get; set; }

		public DateTime? DateFinished { get; set; }

		[NotMapped]
		public virtual ICollection<CustomData> CustomData { get; set; }

		public Tournament()
		{
			Title = "Test Tournament";
			IsFinished = false;
		}
	}
}
