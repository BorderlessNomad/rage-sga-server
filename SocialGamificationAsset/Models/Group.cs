using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using Microsoft.AspNet.Mvc;

namespace SocialGamificationAsset.Models
{
    [Table(SocialGamificationAssetContext.GroupsTableName)]
    public class Group : Actor
    {
        public Group()
        {
            Class = "Group";
            Type = GroupVisibility.Public;
        }

        [Index(IsUnique = true)]
        [StringLength(128)]
        public string Username { get; set; }

        public GroupVisibility Type { get; set; }

        // [IgnoreDataMember]
        public virtual ICollection<Player> Players { get; set; }

        public Guid? OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public virtual Player Owner { get; set; }

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
        ///     Assign Players to the Group.
        /// </summary>
        public void AddPlayers(SocialGamificationAssetContext db, ICollection<Player> playersList)
        {
            if (playersList.Count < 2)
            {
                Console.WriteLine("At least two Players are required to create a Group.");
                return;
            }

            var playerIds = playersList.Select(a => a.Id).ToList();

            var players = db.Players.Where(p => playerIds.Contains(p.Id)).ToList();

            Players = new List<Player>(players);
        }

        public async Task<ContentResult> AddOrUpdateCustomData(
            SocialGamificationAssetContext db,
            IList<CustomDataBase> sourceData)
        {
            return await Models.CustomData.AddOrUpdate(db, sourceData, Id, CustomDataType.Group);
        }

        /**
		 * Check if the current Username already exists
         *
         * @return boolean Returns TRUE if Username exists
         */

        public static async Task<bool> ExistsUsername(SocialGamificationAssetContext db, string username)
        {
            var group = await db.Groups.Where(g => g.Username.Equals(username)).FirstOrDefaultAsync();

            return group != null;
        }
    }

    public enum GroupVisibility
    {
        Public,

        Invisible,

        InviteOnly
    }

    public class GroupFrom
    {
        public string Name { get; set; }

        public GroupVisibility Type { get; set; }

        public IList<Guid> Players { get; set; }
    }
}