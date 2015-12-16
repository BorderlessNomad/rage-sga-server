using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Models
{
    public class UserGroup
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid IdOwner { get; set; }

        public bool Public { get; set; }

        public List<string> CustomData { get; set; }
    }
}
