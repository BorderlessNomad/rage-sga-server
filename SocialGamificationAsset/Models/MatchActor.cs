using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Validation;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;

using SocialGamificationAsset.Helpers;

namespace SocialGamificationAsset.Models
{
    public class MatchActor : DbEntity
    {
        public Guid MatchId { get; set; }

        [IgnoreDataMember]
        [ForeignKey("MatchId")]
        public Match Match { get; set; }

        public Guid ActorId { get; set; }

        [ForeignKey("ActorId")]
        public Actor Actor { get; set; }

        public ICollection<CustomData> CustomData { get; set; }

        public static async Task<ContentResult> Add(SocialGamificationAssetContext db, Match match, Actor actor)
        {
            // Add actors to this match
            var matchActor = new MatchActor { MatchId = match.Id, ActorId = actor.Id };

            db.MatchActors.Add(matchActor);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbEntityValidationException e)
            {
                return HttpResponseHelper.ErrorContentResult(e.Message, StatusCodes.Status500InternalServerError);
            }

            for (var i = 1; i <= match.TotalRounds; ++i)
            {
                // Add round(s) entry for each Actor
                var matchRound = new MatchRound { MatchActorId = matchActor.Id, RoundNumber = i };

                db.MatchRounds.Add(matchRound);

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbEntityValidationException e)
                {
                    return HttpResponseHelper.ErrorContentResult(e.Message, StatusCodes.Status500InternalServerError);
                }
            }

            return null;
        }
    }
}