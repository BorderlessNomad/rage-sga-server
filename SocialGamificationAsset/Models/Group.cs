using System;
using System.Collections.Generic;

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
		public void AddActors(ICollection<Actor> actors)
		{
			if (actors.Count < 2)
			{
				Console.WriteLine("At least two users are needed to create a group");
				return;
			}

			this.Actors = new List<Actor>(actors);
		}
	}
}
