using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Models
{
	public enum FriendState
	{
		Pending,
		Declined,
		Accepted
	}

	public class Friend : Model
	{
		public Guid ActorId { get; set; }

		[ForeignKey("ActorId")]
		public virtual Actor Actor { get; set; }

		public FriendState State { get; set; }
	}
}
