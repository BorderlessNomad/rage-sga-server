using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SocialGamificationAsset.Models
{
    public abstract class Actor : DbEntity
    {
        public Actor()
        {
            this.IsEnabled = true;
        }

        public bool IsEnabled { get; set; }

        public virtual ICollection<Achievement> Achievements { get; set; }

        [NotMapped]
        public virtual ICollection<CustomData> CustomData { get; set; }

        public virtual ICollection<File> Files { get; set; }

        public virtual ICollection<FileActivity> FileActivities { get; set; }

        public virtual ICollection<Inventory> Inventories { get; set; }

        public virtual ICollection<Item> Items { get; set; }

        public virtual ICollection<Platform> Platforms { get; set; }

        public static IQueryable<Actor> Alliances(SocialGamificationAssetContext db, Guid id)
        {
            var alliancesList = Alliance.GetAllianceIds(db, id);

            return db.Actors.Where(a => alliancesList.Contains(a.Id));
        }

        public IQueryable<Actor> Alliances(SocialGamificationAssetContext db)
        {
            return Alliances(db, this.Id);
        }

        public static IQueryable<Actor> Alliances(
            SocialGamificationAssetContext db,
            Guid id,
            AllianceState state = AllianceState.Accepted)
        {
            var alliancesList = Alliance.GetAllianceIds(db, id, state);

            return db.Actors.Where(a => alliancesList.Contains(a.Id));
        }

        public IQueryable<Actor> Alliances(
            SocialGamificationAssetContext db,
            AllianceState state = AllianceState.Accepted)
        {
            return Alliances(db, this.Id, state);
        }
    }
}