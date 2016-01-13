using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
	public class Session : Model
	{
		public Guid PlayerId { get; set; }

		[ForeignKey("PlayerId")]
		public virtual Player Player { get; set; }

		public string LastActionIP { get; set; }

		public bool IsExpired { get; set; }

		public Session()
		{
			IsExpired = false;
		}
	}

	public class UserForm
	{
		public string Username { get; set; }

		public string Email { get; set; }

		[Required]
		public string Password { get; set; }

		public IList<CustomDataBase> CustomData { get; set; }
	}
}
