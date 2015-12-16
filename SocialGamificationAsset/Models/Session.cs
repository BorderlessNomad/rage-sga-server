using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Models
{
    public class Session
    {
        public Guid Id { get; set; }

        public Guid IdAccount { get; set; }

        public Guid GUID { get; set; }

        public DateTime LastActionDate { get; set; }

        public string LastActionIP{ get; set; }

        public DateTime SignitureTimestamp { get; set; }
    }
}
