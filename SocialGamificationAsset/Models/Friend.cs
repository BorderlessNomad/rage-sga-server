using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SocialGamificationAsset.Models
{
	public enum FriendState
	{
		Pending,
		Declined,
		Accepted
	}

	public class Friend : Model
	{
		public Guid RequesterId { get; set; }

		[ForeignKey("RequesterId")]
		public virtual Actor Requester { get; set; }

		public Guid RequesteeId { get; set; }

		[ForeignKey("RequesteeId")]
		public virtual Actor Requestee { get; set; }

		public FriendState State { get; set; }

		public Friend()
		{
			State = FriendState.Pending;
		}

		public static IQueryable<Friend> FindWithLink(SocialGamificationAssetContext db, Guid actorId, Guid friendId)
		{
			return db.Friends
				.Where(f =>
					(f.RequesterId.Equals(friendId) && f.RequesteeId.Equals(actorId)) ||
					(f.RequesteeId.Equals(friendId) && f.RequesterId.Equals(actorId))
				);
		}

		public static IList<Guid> FriendsList(IList<Friend> friends, Guid actorId)
		{
			var friendIds = new List<Guid>();
			foreach (Friend friend in friends)
			{
				friendIds.Add(friend.RequesteeId.Equals(actorId) ? friend.RequesterId : friend.RequesteeId);
			}

			friendIds.Distinct();

			return friendIds;
		}

		public static IList<Guid> GetFriendIds(SocialGamificationAssetContext db, Guid actorId)
		{
			IList<Friend> friends = db.Friends
				.Where(f => (f.RequesterId.Equals(actorId) || f.RequesteeId.Equals(actorId)))
				.ToList();

			return FriendsList(friends, actorId);
		}

		public static IList<Guid> GetFriendIds(SocialGamificationAssetContext db, Guid actorId, FriendState state = FriendState.Accepted)
		{
			IList<Friend> friends = db.Friends
				.Where(f => (f.RequesterId.Equals(actorId) || f.RequesteeId.Equals(actorId)))
				.Where(f => f.State == state)
				.ToList();

			return FriendsList(friends, actorId);
		}
	}
}
