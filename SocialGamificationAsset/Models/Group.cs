using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Models
{
    [Table(SocialGamificationAssetContext.GroupsTableName)]
    public class Group : Actor
    {
        public Group()
        {
            Type = GroupVisibility.Public;
        }

        public string Name { get; set; }

        public GroupVisibility Type { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<Player> Players { get; set; }

        public static async Task<IList<Group>> LoadRandom(
            SocialGamificationAssetContext db,
            Group group,
            IList<CustomDataBase> customData,
            bool alliancesOnly = false,
            int limit = 1)
        {
            IQueryable<Group> query = db.Groups;

            if (alliancesOnly)
            {
                var alliancesList = Alliance.GetAllianceIds(db, group.Id, AllianceState.Accepted);

                query = query.Where(a => alliancesList.Contains(group.Id));
            }
            else
            {
                query = query.Where(g => g.Id != group.Id).Where(g => g.Type == GroupVisibility.Public);
            }

            // CustomData conditions
            var cQuery = Models.CustomData.ConditionBuilder(db, customData, CustomDataType.Group);
            IList<Guid> similarGroups = await cQuery.Select(c => c.ObjectId).Distinct().ToListAsync();

            // Check if Group satisfy CustomData constraints
            IList<Group> groups = await query.Where(g => similarGroups.Contains(g.Id)).ToListAsync();

            return Helper.Shuffle(groups, limit);
        }

        /// <summary>
        ///     Create the group for the first time. Needs to populate the fake
        ///     actor and get the list of player actors.
        /// </summary>
        public void AddPlayers(SocialGamificationAssetContext db, ICollection<Player> actorsList)
        {
            if (actorsList.Count < 2)
            {
                Console.WriteLine("At least two Players are required to create a Group.");
                return;
            }

            var actorIds = actorsList.Select(a => a.Id).ToList();

            var actors = db.Players.Where(a => actorIds.Contains(a.Id)).ToList();

            Players = new List<Player>(actors);
        }

        public async Task AddOrUpdateCustomData(SocialGamificationAssetContext db, IList<CustomDataBase> sourceData)
        {
            await Models.CustomData.AddOrUpdate(db, sourceData, Id, CustomDataType.Group);
        }
    }

    public enum GroupVisibility
    {
        Public,

        Invisible,

        InviteOnly
    }
}