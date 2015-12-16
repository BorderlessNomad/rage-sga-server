using System;

namespace SocialGamificationAsset.Models
{
	public class UserFiles
	{
		public Guid Id { get; set; }

		public Guid IdAccount { get; set; }

		public string Name { get; set; }

		public string Url { get; set; }

		public string ShareType { get; set; }

		public int Likes { get; set; }

		public int Views { get; set; }
	}
}
