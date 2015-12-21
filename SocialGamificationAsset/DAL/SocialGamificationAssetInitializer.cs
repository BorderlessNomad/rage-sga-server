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
