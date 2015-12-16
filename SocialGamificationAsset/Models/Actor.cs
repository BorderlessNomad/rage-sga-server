using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Models
{
	public class Actor : Model
	{
		public string Username { get; set; }

		public string Password { get; set; }

		public string Email { get; set; }

		public Guid ActivationCode { get; private set; }

		public bool IsEnabled { get; set; }

		public AccountTypeEnum Type { get; set; }

		public string LastLoginIp { get; set; }

		public DateTime CreatedDate { get; set; }

		public DateTime LastLoginDate { get; set; }

		public virtual ICollection<Achievement> Achievement { get; set; }

		public virtual ICollection<CustomData> CustomData { get; set; }

		public virtual ICollection<Platform> Platform { get; set; }

		public virtual ICollection<Session> Sessions { get; set; }

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

	public enum AccountTypeEnum
	{
		Player = 0,
		Admin = 1,
	}
}
