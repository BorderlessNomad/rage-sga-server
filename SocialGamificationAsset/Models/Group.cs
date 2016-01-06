using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SocialGamificationAsset.Models
{
	public class Group : Actor
	{
		public string Name { get; set; }

		public bool IsPublic { get; set; }

		public virtual ICollection<Actor> Actors { get; set; }

		/// <summary>
		/// Create the group for the first time. Needs to populate the fake actor and get the list of player actors.
		/// </summary>
		public void AddActors(SocialGamificationAssetContext db, ICollection<Actor> actorsList)
		{
			if (actorsList.Count < 2)
			{
				Console.WriteLine("At least two users are needed to create a group");
				return;
			}

			List<Guid> actorIds = actorsList.Select(a => a.Id).ToList();

			List<Actor> actors = db.Actors.Where(a => actorIds.Contains(a.Id)).ToList();

			this.Actors = new List<Actor>(actors);
		}
	}
}
