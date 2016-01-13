using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SocialGamificationAsset.Models
{
	public abstract class Actor : Model
	{
		public bool IsEnabled { get; set; }

		public virtual ICollection<Achievement> Achievements { get; set; }

		[NotMapped]
		public virtual ICollection<CustomData> CustomData { get; set; }

		public virtual ICollection<File> Files { get; set; }

		public virtual ICollection<FileActivity> FileActivities { get; set; }

		public virtual ICollection<Inventory> Inventories { get; set; }

		public virtual ICollection<Platform> Platforms { get; set; }

		public Actor()
		{
			IsEnabled = true;
		}

		public static IQueryable<Actor> Friends(SocialGamificationAssetContext db, Guid id)
		{
			var friendsList = Friend.GetFriendIds(db, id);

			return db.Actors.Where(a => friendsList.Contains(a.Id));
		}

		public IQueryable<Actor> Friends(SocialGamificationAssetContext db)
		{
			return Friends(db, this.Id);
		}

		public static IQueryable<Actor> Friends(SocialGamificationAssetContext db, Guid id, FriendState state = FriendState.Accepted)
		{
			var friendsList = Friend.GetFriendIds(db, id, state);

			return db.Actors.Where(a => friendsList.Contains(a.Id));
		}

		public IQueryable<Actor> Friends(SocialGamificationAssetContext db, FriendState state = FriendState.Accepted)
		{
			return Friends(db, this.Id, state);
		}
	}
}
