using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;

namespace SocialGamificationAsset.Models
{
	public class SocialGamificationAssetInitializer
	{
		private readonly SocialGamificationAssetContext _context;

		public SocialGamificationAssetInitializer(SocialGamificationAssetContext context)
		{
			_context = context;
		}

		public void InitializeDatabase()
		{
			if (!_context.Database.Exists())
			{
				// if database did not exist before - create it
				_context.Database.Create();
			}
			else
			{
				// query to check if MigrationHistory table is present in the database
				var migrationHistoryTableExists = ((IObjectContextAdapter)_context).ObjectContext.ExecuteStoreQuery<int>(
				string.Format("SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = '{0}' AND table_name = '__MigrationHistory'", ""));

				// if MigrationHistory table is not there (which is the case first time we run) - create it
				if (migrationHistoryTableExists.FirstOrDefault() == 0)
				{
					_context.Database.Delete();
					_context.Database.Create();
				}
			}
		}

		public void Seed()
		{
			if (!_context.Tests.Any())
			{
				IList<Test> tests = new List<Test>
				{
					new Test
					{
						Username = "test1",
						Password = "test1"
					},
					new Test
					{
						Username = "test2",
						Password = "test2"
					},
					new Test
					{
						Username = "test3",
						Password = "test3"
					},
				};

				_context.Tests.AddRange(tests);

				try
				{
					_context.SaveChanges();
				}
				catch (DbEntityValidationException e)
				{
					throw e;
				}

				Debug.WriteLine("Tests Created.");
			}
		}
	}
}
