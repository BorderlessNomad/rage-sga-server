using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
	public class Model
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public Guid Id { get; set; }

		public DateTime? UpdatedDate { get; set; }

		public DateTime CreatedDate { get; set; }

		public Model()
		{
			this.Id = Guid.NewGuid();
		}
	}
}
