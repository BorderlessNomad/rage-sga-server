using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Models
{
	public class Platform : Model
	{
		public string Key { get; set; }

		public Guid AccountId { get; set; }

		[ForeignKey("AccountId")]
		public virtual Account Account { get; set; }
	}
}
