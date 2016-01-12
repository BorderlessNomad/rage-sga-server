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

		public static async Task<IList<Player>> LoadRandom(SocialGamificationAssetContext db, Player player, IList<CustomDataBase> customData, bool friendsOnly = false, int limit = 1)
		{
			if (friendsOnly)
			{
				return (IList<Player>)Actor.LoadRandom(db, player, customData, friendsOnly, limit);
			}

			IList<Player> players = await db.Players
				.Where(a => a.Id != player.Id)
				.Where(a => a.Role == AccountType.Player)
				.ToListAsync()
			;

			return Helper.Shuffle(players, limit);
		}

		/**
		 * Check if the current Username already exists
         *
         * @return boolean Returns TRUE if Username exists
         */

		public async Task<bool> ExistsUsername(SocialGamificationAssetContext db, string username)
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

		public async Task<bool> ExistsEmail(SocialGamificationAssetContext db, string email)
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

		public async Task<bool> IsOnline(SocialGamificationAssetContext db, Guid playerId)
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

		public async Task<Session> GetSession(SocialGamificationAssetContext db, Guid playerId)
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
