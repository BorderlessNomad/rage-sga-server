using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;

using Newtonsoft.Json;

using SocialGamificationAsset.Helpers;

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
        ///     Set default Created Date field.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        ///     Custom Primary Key set to <see cref="Guid" /> .
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        /// <summary>
        ///     Asynchronously save data
        /// </summary>
        /// <returns>
        ///     ErrorContentResult if <see cref="DbUpdateException" /> exception
        ///     occurs
        /// </returns>
        public static async Task<ContentResult> SaveChanges(
            SocialGamificationAssetContext _context,
            bool isAsync = false)
        {
            try
            {
                if (isAsync)
                {
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.SaveChanges();
                }
            }
            catch (DbUpdateException e)
            {
                return HttpResponseHelper.ErrorContentResult(
                    GetExceptionString(e),
                    StatusCodes.Status500InternalServerError);
            }

            return null;
        }

        public static string GetExceptionString(Exception e)
        {
            if (e.Message == "An error occurred while updating the entries. See the inner exception for details.")
            {
                return GetExceptionString(e.InnerException);
            }

            return e.Message;
        }
    }

    public struct ApiError
    {
        public string Error;
    }
}