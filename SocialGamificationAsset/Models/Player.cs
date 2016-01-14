using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Models
{
	[Table(SocialGamificationAssetContext.PlayersTableName)]
	public class Player : Actor
	{
		public string Username { get; set; }

		public string Password { get; set; }

		public string Email { get; set; }

		public Guid ActivationCode { get; private set; }

		public AccountType Role { get; set; }

		public string LastLoginIp { get; set; }

		public virtual ICollection<Group> Groups { get; set; }

		public virtual ICollection<Session> Sessions { get; set; }

		public Player()
		{
			Role = AccountType.Player;
			ActivationCode = Guid.NewGuid();
		}

		public static async Task<IList<Player>> LoadRandom(SocialGamificationAssetContext db, Player player, IList<CustomDataBase> customData, bool alliancesOnly = false, int limit = 1)
		{
			IQueryable<Player> query = db.Players.Where(a => a.Role == AccountType.Player);

			if (alliancesOnly)
			{
				var alliancesList = Alliance.GetAllianceIds(db, player.Id, AllianceState.Accepted);

				query = query.Where(p => alliancesList.Contains(player.Id));
			}
			else
			{
				query = query.Where(p => p.Id != player.Id);
			}

			// CustomData conditions
			var cQuery = Models.CustomData.ConditionBuilder(db, customData, CustomDataType.Player);
			IList<Guid> similarPlayers = await cQuery.Select(c => c.ObjectId).Distinct().ToListAsync();

			// Check if Players satisfy CustomData constaints
			IList<Player> players = await query.Where(p => similarPlayers.Contains(p.Id)).ToListAsync();

			return Helper.Shuffle(players, limit);
		}

		/**
		 * Check if the current Username already exists
         *
         * @return boolean Returns TRUE if Username exists
         */

		public static async Task<bool> ExistsUsername(SocialGamificationAssetContext db, string username)
		{
			Player player = await db.Players.Where(p => p.Username.Equals(username)).FirstOrDefaultAsync();

			if (player != null)
			{
				return true;
			}

			return false;
		}

		/**
		 * Check if the current Email already exists
		 *
		 * @return boolean Returns TRUE if Username exists
		 */

		public static async Task<bool> ExistsEmail(SocialGamificationAssetContext db, string email)
		{
			Player player = await db.Players.Where(p => p.Email.Equals(email)).FirstOrDefaultAsync();

			if (player != null)
			{
				return true;
			}

			return false;
		}

		/**
		 * Verify if this account is online (with Last session being marked active)
		 *
		 * @return bool Returns TRUE if the account is online
		 */

		public static async Task<bool> IsOnline(SocialGamificationAssetContext db, Guid playerId)
		{
			Session session = await GetSession(db, playerId);

			if (session != null)
			{
				return true;
			}

			return false;
		}

		public async Task<bool> IsOnline(SocialGamificationAssetContext db)
		{
			return await IsOnline(db, this.Id);
		}

		/**
		 * Get the current session of Logged account
		 *
		 * @return Session
		 */

		public static async Task<Session> GetSession(SocialGamificationAssetContext db, Guid playerId)
		{
			Session session = (Session)await db.Players
				.Where(p => p.Id.Equals(playerId))
				.Include(p => p.Sessions
					.Where(s => s.IsExpired.Equals(false))
					.OrderByDescending(s => s.UpdatedDate)
				)
				.Select(p => p.Sessions)
				.FirstOrDefaultAsync();

			return session;
		}

		public async Task<Session> GetSession(SocialGamificationAssetContext db)
		{
			return await GetSession(db, this.Id);
		}
	}

	public enum AccountType
	{
		Admin,
		Player,
	}
}
