using System.ComponentModel.DataAnnotations.Schema;

namespace SocialGamificationAsset.Models
{
    [ComplexType]
    public class Matrix
    {
        public float X { get; set; }

        public float Y { get; set; }
    }

    public class ConcernMatrix : DbEntity
    {
        public Matrix Coordinates { get; set; }

        public ConcernCategories Category { get; set; }
    }

    public class RewardResourceMatrix : DbEntity
    {
        public Matrix Coordinates { get; set; }

        public RewardResourceCategories Category { get; set; }
    }

    public enum ConcernCategories
    {
        Defeat,

        Collaborate,

        Withdraw,

        Accomodate,

        Compromise
    }

    public enum RewardResourceCategories
    {
        HyperCooperation,

        Pdc, // TODO

        ContrariantCooperation
    }
}