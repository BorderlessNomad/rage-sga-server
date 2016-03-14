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
    [Table(SocialGamificationAssetContext.PlayersTableName)]
    public class Player : Actor
    {
        public Player()
        {
            Class = "Player";
            Role = AccountType.Player;
            ActivationCode = Guid.NewGuid();
        }

        [Index(IsUnique = true)]
        [StringLength(128)]
        public string Username { get; set; }

        [IgnoreDataMember]
        public string Password { get; set; }

        [Index(IsUnique = true)]
        [StringLength(128)]
        public string Email { get; set; }

        [IgnoreDataMember]
        public Guid ActivationCode { get; private set; }

        public AccountType Role { get; set; }

        [IgnoreDataMember]
        public string LastLoginAddress { get; set; }

        public virtual ICollection<Group> Groups { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<Session> Sessions { get; set; }

        public static async Task<IList<Player>> LoadRandom(
            SocialGamificationAssetContext db,
            Player player,
            IList<CustomDataBase> customData,
            bool alliancesOnly = false,
            int limit = 1)
        {
            var query = db.Players.Where(a => a.Role == AccountType.Player);

            if (alliancesOnly)
            {
                var alliancesList = Alliance.GetAllianceIds(db, player.Id, AllianceState.Accepted);

                query = query.Where(p => alliancesList.Contains(p.Id));
            }
            else
            {
                query = query.Where(p => p.Id != player.Id);
            }

            // CustomData conditions
            var customDataQuery = Models.CustomData.ConditionBuilder(db, customData, CustomDataType.Player);
            if (customDataQuery != null)
            {
                IList<Guid> similarPlayers = await customDataQuery.Select(c => c.ObjectId).Distinct().ToListAsync();

                // Check if Players satisfy CustomData constraints
                query = query.Where(p => similarPlayers.Contains(p.Id));
            }

            IList<Player> players = await query.ToListAsync();

            return Helper.Shuffle(players, limit);
        }

        /**
		 * Check if the current Username already exists
         *
         * @return boolean Returns TRUE if Username exists
         */

        public static async Task<bool> ExistsUsername(SocialGamificationAssetContext db, string username)
        {
            var player = await db.Players.Where(p => p.Username.Equals(username)).FirstOrDefaultAsync();

            return player != null;
        }

        /**
		 * Check if the current Email already exists
		 *
		 * @return boolean Returns TRUE if Username exists
		 */

        public static async Task<bool> ExistsEmail(SocialGamificationAssetContext db, string email)
        {
            var player = await db.Players.Where(p => p.Email.Equals(email)).FirstOrDefaultAsync();

            return player != null;
        }

        /**
		 * Verify if this account is online (with Last session being marked active)
		 *
		 * @return bool Returns TRUE if the account is online
		 */

        public static async Task<bool> IsOnline(SocialGamificationAssetContext db, Guid playerId)
        {
            var session = await GetSession(db, playerId);

            return session != null;
        }

        public async Task<bool> IsOnline(SocialGamificationAssetContext db)
        {
            return await IsOnline(db, Id);
        }

        /**
		 * Get the current session of Logged account
		 *
		 * @return Session
		 */

        public static async Task<Session> GetSession(SocialGamificationAssetContext db, Guid playerId)
        {
            var session =
                await
                db.Players.Where(p => p.Id.Equals(playerId))
                  .Include(p => p.Sessions.Where(s => s.IsExpired.Equals(false)).OrderByDescending(s => s.UpdatedDate))
                  .Select(p => p.Sessions)
                  .FirstOrDefaultAsync() as Session;

            return session;
        }

        public async Task<Session> GetSession(SocialGamificationAssetContext db)
        {
            return await GetSession(db, Id);
        }

        public async Task<ContentResult> AddOrUpdateCustomData(
            SocialGamificationAssetContext db,
            IList<CustomDataBase> sourceData)
        {
            return await Models.CustomData.AddOrUpdate(db, sourceData, Id, CustomDataType.Player);
        }

        /// <summary>
        ///     Assign Groups to the Player.
        /// </summary>
        public void AddGroups(SocialGamificationAssetContext db, ICollection<Player> groupsList)
        {
            var groupIds = groupsList.Select(a => a.Id).ToList();

            var groups = db.Groups.Where(g => groupIds.Contains(g.Id)).ToList();

            Groups = new List<Group>(groups);
        }
    }

    public enum AccountType
    {
        Admin,

        Player
    }
}