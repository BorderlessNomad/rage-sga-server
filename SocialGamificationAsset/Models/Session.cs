using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
	public class Session : Model
	{
		public Guid ActorId { get; set; }

		[ForeignKey("ActorId")]
		public virtual Actor Actor { get; set; }

		public string LastActionIP { get; set; }

		public bool IsExpired { get; set; }

		public Session()
		{
			IsExpired = false;
		}
	}

	public class LoginForm
	{
		[Required]
		public string Username { get; set; }

		// public string Email { get; set; }

		[Required]
		public string Password { get; set; }
	}
}
