using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Validation;

namespace SocialGamificationAsset.Models
{
	public class MatchActor : Model
	{
		public Guid MatchId { get; set; }

		[ForeignKey("MatchId")]
		public Match Match { get; set; }

		public Guid ActorId { get; set; }

		[ForeignKey("ActorId")]
		public Actor Actor { get; set; }

		public ICollection<CustomData> CustomData { get; set; }

		public static async void Add(SocialGamificationAssetContext db, Match match, Actor actor)
		{
			// Add actors to this match
			MatchActor matchActor = new MatchActor()
			{
				MatchId = match.Id,
				ActorId = actor.Id
			};

			db.MatchActors.Add(matchActor);

			try
			{
				await db.SaveChangesAsync();
			}
			catch (DbEntityValidationException e)
			{
				throw e;
			}

			for (int i = 1; i <= match.TotalRounds; ++i)
			{
				// Add round(s) entry for each Actor
				MatchRound matchRound = new MatchRound()
				{
					MatchActorId = matchActor.Id
				};

				db.MatchRounds.Add(matchRound);

				try
				{
					await db.SaveChangesAsync();
				}
				catch (DbEntityValidationException e)
				{
					throw e;
				}
			}
		}
	}
}
