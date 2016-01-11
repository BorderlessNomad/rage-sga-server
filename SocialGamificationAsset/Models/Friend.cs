using System;
using System.ComponentModel.DataAnnotations.Schema;

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
		public Guid RequesterId { get; set; }

		[ForeignKey("RequesterId")]
		public virtual Actor Requester { get; set; }

		public Guid RequesteeId { get; set; }

		[ForeignKey("RequesteeId")]
		public virtual Actor Requestee { get; set; }

		public FriendState State { get; set; }

		public Friend()
		{
			State = FriendState.Pending;
		}
	}
}
