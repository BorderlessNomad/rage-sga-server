using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Models
{
    public class UserFilesActivity
    {
        public Guid Id { get; set; }

        public Guid IdFile { get; set; }

        public Guid IdAccount { get; set; }

        public int Likes { get; set; }

        public int Views { get; set; }

        public DateTime LastActivity { get; set; }
    }
}
