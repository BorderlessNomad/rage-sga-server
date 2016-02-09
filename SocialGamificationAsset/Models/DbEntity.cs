using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

using Newtonsoft.Json;

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
        public DbEntity()
        {
            Id = Guid.NewGuid();
        }

        /*
		public Guid GameId { get; set; }

		[ForeignKey("GameId")]
		public virtual GameRegistry Game { get; set; }
		*/

        public DateTime? UpdatedDate { get; set; }

        public DateTime CreatedDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
    }
}