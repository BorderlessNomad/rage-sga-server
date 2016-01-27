using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SocialGamificationAsset.Models
{
	public interface IDbEntity
	{
		Guid Id { get; set; }
	}

	[JsonObject(IsReference = true)]
	[DataContract]
	public class DbEntity : IDbEntity
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public Guid Id { get; set; }

		public DateTime? UpdatedDate { get; set; }

		public DateTime CreatedDate { get; set; }

		public DbEntity()
		{
			Id = Guid.NewGuid();
		}
	}
}
