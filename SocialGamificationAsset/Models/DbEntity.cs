using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace SocialGamificationAsset.Models
{
    /// <summary>
    ///     DBEntity Interface
    /// </summary>
    public interface IDbEntity
    {
        Guid Id { get; set; }
    }

    /// <summary>
    ///     DbEntity class handling Primary Key creation & other default parameters.
    /// </summary>
    [JsonObject(IsReference = true)]
    [DataContract]
    public class DbEntity : IDbEntity
    {
        /// <summary>
        ///     Default constructor initialising
        ///     <see cref="SocialGamificationAsset.Models.DbEntity.Id" /> .
        /// </summary>
        public DbEntity()
        {
            Id = Guid.NewGuid();
        }

        /*
		public Guid GameId { get; set; }

		[ForeignKey("GameId")]
		public virtual GameRegistry Game { get; set; }
		*/

        /// <summary>
        ///     Set default Updated Date field.
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        ///     Set defauly Created Date field.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        ///     Custom Primary Key set to <see cref="Guid" /> .
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
    }

    public struct ApiError
    {
        public string Error;
    }
}