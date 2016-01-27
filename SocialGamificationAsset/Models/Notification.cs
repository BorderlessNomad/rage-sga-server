using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
	public class Notification : DbEntity
	{
		public Guid ActorId { get; set; }

		[ForeignKey("ActorId")]
		public virtual Actor Actor { get; set; }

		public Guid SenderId { get; set; }

		[ForeignKey("SenderId")]
		public virtual Actor Sender { get; set; }

		public string Message { get; set; }

		public NotificationStatus Status { get; set; }

		public DateTime? ExpiryDate { get; set; }
	}

	public enum NotificationStatus
	{
		Unsent,
		Sent,
		Delivered,
		Read,
		Expired
	}
}
