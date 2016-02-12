using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace SocialGamificationAsset.Models
{
    public abstract class Actor : DbEntity
    {
        protected Actor()
        {
            IsEnabled = true;
        }

        public bool IsEnabled { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<Achievement> Achievements { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<CustomData> CustomData { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<File> Files { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<FileActivity> FileActivities { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<Inventory> Inventories { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<Item> Items { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<Platform> Platforms { get; set; }

        public static IQueryable<Actor> Alliances(SocialGamificationAssetContext db, Guid id)
        {
            var alliancesList = Alliance.GetAllianceIds(db, id);

            return db.Actors.Where(a => alliancesList.Contains(a.Id));
        }

        public IQueryable<Actor> Alliances(SocialGamificationAssetContext db)
        {
            return Alliances(db, Id);
        }

        public static IQueryable<Actor> Alliances(SocialGamificationAssetContext db, Guid id, AllianceState state)
        {
            var alliancesList = Alliance.GetAllianceIds(db, id, state);

            return db.Actors.Where(a => alliancesList.Contains(a.Id));
        }

        public IQueryable<Actor> Alliances(SocialGamificationAssetContext db, AllianceState state)
        {
            return Alliances(db, Id, state);
        }
    }
}