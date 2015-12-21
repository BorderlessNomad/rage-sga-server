using Microsoft.Extensions.Configuration;
using MySql.Data.Entity;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Models
{
	[DbConfigurationType(typeof(MySqlEFConfiguration))]
	public class SocialGamificationAssetContext : DbContext
	{
		public SocialGamificationAssetContext(IConfiguration config)
			: base(config["Data:MySQLConnection:ConnectionString"])
		{
			Database.SetInitializer(new CreateDatabaseIfNotExists<SocialGamificationAssetContext>());
			Database.SetInitializer(new DropCreateDatabaseIfModelChanges<SocialGamificationAssetContext>());
		}

		public DbSet<Achievement> Achievements { get; set; }

		public DbSet<Actor> Actors { get; set; }

		public DbSet<CustomData> CustomData { get; set; }

		public DbSet<File> Files { get; set; }

		public DbSet<FileActivity> FileActivities { get; set; }

		public DbSet<Group> Groups { get; set; }

		public DbSet<Inventory> Inventory { get; set; }

		public DbSet<Leaderboard> Leaderboards { get; set; }

		public DbSet<LeaderboardUser> LeaderboardUsers { get; set; }

		public DbSet<Match> Matches { get; set; }

		public DbSet<MatchActor> MatchActors { get; set; }

		public DbSet<MatchRound> MatchRounds { get; set; }

		public DbSet<Platform> Platforms { get; set; }

		public DbSet<ServerSetting> ServerSettings { get; set; }

		public DbSet<Session> Sessions { get; set; }

		public DbSet<Tournament> Tournaments { get; set; }

		public DbSet<Test> Tests { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
			modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
		}

		protected void SaveChangesWithHooks()
		{
			var entries = this.ChangeTracker.Entries();
			var changes = entries.Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

			foreach (var entry in changes)
			{
				var type = entry.Entity.GetType();

				// Invoke a BeforeInsert() method on the entity if it exists
				MethodInfo saveMethod;

				if (entry.State == EntityState.Added)
				{
					saveMethod = type.GetMethod("BeforeInsert");

					if (null != saveMethod && saveMethod.GetParameters().Length == 0)
					{
						saveMethod.Invoke(entry.Entity, null);
					}
				}
				else if (entry.State == EntityState.Modified)
				{
					// Invoke a BeforeUpdate() method on the entity if it exists
					saveMethod = type.GetMethod("BeforeUpdate");

					if (null != saveMethod && saveMethod.GetParameters().Length == 0)
					{
						saveMethod.Invoke(entry.Entity, null);
					}
				}

				// Invoke a BeforeSave() method on the entity if it exists
				saveMethod = type.GetMethod("BeforeSave");

				if (null != saveMethod && saveMethod.GetParameters().Length == 0)
				{
					saveMethod.Invoke(entry.Entity, null);
				}

				// Set the created and updated date properties if they exist
				PropertyInfo property = null;
				if (entry.State == EntityState.Added)
				{
					property = type.GetProperty("CreatedDate");
					if (property != null)
					{
						property.SetValue(entry.Entity, DateTime.Now, null);
					}
				}

				property = type.GetProperty("UpdatedDate");
				if (property != null)
				{
					property.SetValue(entry.Entity, DateTime.Now, null);
				}
			}
		}

		public override int SaveChanges()
		{
			SaveChangesWithHooks();

			return base.SaveChanges();
		}

		public override async Task<int> SaveChangesAsync()
		{
			SaveChangesWithHooks();

			return await base.SaveChangesAsync();
		}
	}
}
