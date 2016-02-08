using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
    public class FileActivity : DbEntity
    {
        public Guid FileId { get; set; }

        [ForeignKey("FileId")]
        public virtual File File { get; set; }

        public Guid ActorId { get; set; }

        [ForeignKey("ActorId")]
        public virtual Actor Actor { get; set; }

        public int Likes { get; set; }

        public int Views { get; set; }
    }
}