using System;

namespace SocialGamificationAsset.Models
{
	public class User
	{
		public Guid Id { get; set; }

		public string Username { get; set; }

		public string Email { get; set; }

		public string Password { get; set; }

		public DateTime DateCreated { get; set; }
	}
}
