﻿using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Models
{
	public class SocialGamificationAssetInitializer
	{
		private readonly SocialGamificationAssetContext _context;

		public SocialGamificationAssetInitializer(SocialGamificationAssetContext context)
		{
			_context = context;
		}

		public async Task Seed()
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
					await _context.SaveChangesAsync();
				}
				catch (DbEntityValidationException e)
				{
					throw e;
				}

				Debug.WriteLine("Tests Created.");
			}

			if (!_context.Sessions.Any())
			{
				IList<Session> sessions = new List<Session>
				{
					new Session
					{
						Actor = new Actor
						{
							Username = "admin",
							Password = "admin"
						}
					},
					new Session
					{
						Actor = new Actor
						{
							Username = "mayur",
							Password = "123456"
						}
					}
				};

				_context.Sessions.AddRange(sessions);

				try
				{
					await _context.SaveChangesAsync();
				}
				catch (DbEntityValidationException e)
				{
					throw e;
				}

				Debug.WriteLine("Sessions Created.");
			}
		}
	}
}
