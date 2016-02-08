using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;

namespace SocialGamificationAsset.Models
{
    public class Alliance : DbEntity
    {
        public Alliance()
        {
            this.State = AllianceState.Pending;
        }

        public Guid RequesterId { get; set; }

        [ForeignKey("RequesterId")]
        public virtual Actor Requester { get; set; }

        public Guid RequesteeId { get; set; }

        [ForeignKey("RequesteeId")]
        public virtual Actor Requestee { get; set; }

        public AllianceState State { get; set; }

        public static Expression<Func<Alliance, bool>> IsAlliance(Guid actorId)
        {
            return f => f.RequesterId.Equals(actorId) || f.RequesteeId.Equals(actorId);
        }

        public static Expression<Func<Alliance, bool>> IsAlliance(Guid actorId, Guid allianceId)
        {
            return
                f =>
                (f.RequesterId.Equals(allianceId) && f.RequesteeId.Equals(actorId))
                || (f.RequesteeId.Equals(allianceId) && f.RequesterId.Equals(actorId));
        }

        public static IList<Guid> AlliancesList(IList<Alliance> alliances, Guid actorId)
        {
            var allianceIds = alliances.Select(alliance => alliance.RequesteeId.Equals(actorId) ? alliance.RequesterId : alliance.RequesteeId)
                                       .ToList();

            allianceIds.Distinct(); // Test this

            return allianceIds;
        }

        public static IList<Guid> GetAllianceIds(SocialGamificationAssetContext db, Guid actorId)
        {
            IList<Alliance> alliances = db.Alliances.Where(IsAlliance(actorId))
                                          .ToList();

            return AlliancesList(alliances, actorId);
        }

        public static IList<Guid> GetAllianceIds(
            SocialGamificationAssetContext db,
            Guid actorId,
            AllianceState state = AllianceState.Accepted)
        {
            IList<Alliance> alliances = db.Alliances.Where(IsAlliance(actorId))
                                          .Where(f => f.State == state)
                                          .ToList();

            return AlliancesList(alliances, actorId);
        }
    }

    public enum AllianceState
    {
        Pending,

        Declined,

        Accepted
    }
}