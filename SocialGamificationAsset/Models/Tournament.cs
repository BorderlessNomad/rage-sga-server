using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Models
{
    public class Tournament
    {
        public Guid Id { get; set; }

        public Guid IdOwner { get; set; }

        public string Title { get; set; }

        public DateTime DateCreation { get; set; }

        public DateTime DateFinished { get; set; }

        public List<string> CustomData { get; set; }

        public bool Finished { get; set; }
    }
}
