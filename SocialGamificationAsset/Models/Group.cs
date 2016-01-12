using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Models
{
	[Table(SocialGamificationAssetContext.GroupsTableName)]
	public class Group : Actor
	{
		public string Name { get; set; }

		public GroupVisibility Type { get; set; }

		public virtual ICollection<Player> Players { get; set; }

		public Group()
		{
			Type = GroupVisibility.Public;
		}

		public static async Task<IList<Actor>> LoadRandom(SocialGamificationAssetContext db, Group group, IList<CustomDataBase> customData, bool friendsOnly = false, int limit = 1)
		{
			if (friendsOnly)
			{
				return Actor.LoadRandom(db, group, customData, friendsOnly, limit);
			}

			IList<Group> groups = await db.Groups
				.Where(a => a.Id != group.Id)
				.Where(a => a.Type == GroupVisibility.Public)
				.ToListAsync()
			;

			return (IList<Actor>)Helper.Shuffle(groups, limit);
		}

		/// <summary>
		/// Create the group for the first time. Needs to populate the fake actor and get the list of player actors.
		/// </summary>
		public void AddPlayers(SocialGamificationAssetContext db, ICollection<Player> actorsList)
		{
			if (actorsList.Count < 2)
			{
				Console.WriteLine("At least two Players are required to create a Group.");
				return;
			}

			List<Guid> actorIds = actorsList.Select(a => a.Id).ToList();

			List<Player> actors = db.Players.Where(a => actorIds.Contains(a.Id)).ToList();

			this.Players = new List<Player>(actors);
		}
	}

	public enum GroupVisibility
	{
		Public,
		Invisible,
		InviteOnly
	}
}
