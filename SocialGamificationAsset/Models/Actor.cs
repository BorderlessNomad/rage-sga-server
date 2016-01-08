using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialGamificationAsset.Models
{
	public class Actor : Model
	{
		public string Username { get; set; }

		public string Password { get; set; }

		public string Email { get; set; }

		public Guid ActivationCode { get; private set; }

		public bool IsEnabled { get; set; }

		public AccountType Role { get; set; }

		public string LastLoginIp { get; set; }

		public virtual ICollection<Friend> Friends { get; set; }

		public virtual ICollection<Achievement> Achievements { get; set; }

		public virtual ICollection<CustomData> CustomData { get; set; }

		public virtual ICollection<Group> Groups { get; set; }

		public virtual ICollection<Inventory> Inventories { get; set; }

		public virtual ICollection<File> Files { get; set; }

		public virtual ICollection<FileActivity> FileActivities { get; set; }

		public virtual ICollection<Platform> Platforms { get; set; }

		public virtual ICollection<Session> Sessions { get; set; }

		public Actor()
		{
			IsEnabled = true;
			Role = AccountType.Player;
		}

		public IList<Actor> LoadRandom(SocialGamificationAssetContext db, IList<CustomDataBase> customData, bool friendsOnly = false, int limit = 1)
		{
			IQueryable<Actor> results;
			if (friendsOnly)
			{
				results = (IQueryable<Actor>)Friends
					.Where(f => f.State.Equals(FriendState.Accepted))
					.Where(f => f.RequesteeId.Equals(this.Id))
					.Select(f => f.Requestee)
					.Union((IQueryable<Actor>)Friends
						.Where(f => f.State.Equals(FriendState.Accepted))
						.Where(f => f.RequesterId.Equals(this.Id))
						.Select(f => f.Requester)
					)
				;
			}
			else
			{
				results = db.Actors
					.Where(a => a.Id != this.Id)
					.Where(a => a.Role == AccountType.Player)
				;
			}

			return Helper.Shuffle(results.ToList(), limit);
		}

		/**
		 * Check if the current Username already exists
         *
         * @return boolean Returns TRUE if Username exists
         */

		public bool ExistsUsername()
		{
			return false;
		}

		/**
		 * Check if the current Email already exists
		 *
		 * @return boolean Returns TRUE if Username exists
		 */

		public bool ExistsEmail()
		{
			return false;
		}

		/**
		 * Verify if this account is logged
		 *
		 * @return bool Returns TRUE if the account is logged
		 */

		public bool IsLogged()
		{
			return false;
		}

		/**
		 * Verify if this account is online (with Last session being marked active)
		 *
		 * @return bool Returns TRUE if the account is online
		 */

		public bool IsOnline()
		{
			return false;
		}

		/**
		 * Get the last session
		 *
		 * @return Session
		 */

		public Session GetLastActionDate()
		{
			return new Session();
		}

		/**
		 * Get the current session of Logged account
		 *
		 * @return Session
		 */

		public Session GetSession()
		{
			return new Session();
		}

		/**
		 * Get the registered accounts
		 *
		 * @return IList<Account> the array of accounts
		 */

		public IList<Actor> Load()
		{
			IList<Actor> accounts = new List<Actor>();
			return accounts;
		}
	}

	public enum AccountType
	{
		Admin,
		Player,
		Group
	}
}
