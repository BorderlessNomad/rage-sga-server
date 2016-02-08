using System.ComponentModel.DataAnnotations;

namespace SocialGamificationAsset.Models
{
    public class Test : DbEntity
    {
        [Required]
        public string Field1 { get; set; }

        public string Field2 { get; set; }
    }
}