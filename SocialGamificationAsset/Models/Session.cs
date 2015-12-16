using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Models
{
    public class Session
    {
        public Guid Id { get; set; }

        public Guid IdAccount { get; set; }

		[ForeignKey("IdAccount")]
		public virtual Account Account { get; set; }

        public Guid GUID { get; set; }

        public DateTime LastActionDate { get; set; }

        public string LastActionIP{ get; set; }

        public DateTime SignitureTimestamp { get; set; }
    }
}
