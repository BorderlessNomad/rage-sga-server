﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Models
{
	public class SocialGamificationAssetInitializer
	{
		public static async Task InitializeDatabase(IServiceProvider serviceProvider, bool isAsync = false)
		{
			using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				var _context = serviceScope.ServiceProvider.GetService<SocialGamificationAssetContext>();

				await CreateTests(_context, isAsync);

				await CreateActorSessions(_context, isAsync);

				await CreateFriends(_context, isAsync);
			}
		}

		protected static async Task CreateTests(SocialGamificationAssetContext _context, bool isAsync = false)
		{
			if (!_context.Tests.Any())
			{
				IList<Test> tests = new List<Test>
				{
					new Test
					{
						Field1 = "test1",
						Field2 = "test1"
					},
					new Test
					{
						Field1 = "test2",
						Field2 = "test2"
					},
					new Test
					{
						Field1 = "test3",
						Field2 = "test3"
					},
				};

				_context.Tests.AddRange(tests);

				await SaveChanges(_context, isAsync);

				Debug.WriteLine("Tests Created.");
			}
		}

		protected static async Task CreateActorSessions(SocialGamificationAssetContext _context, bool isAsync = false)
		{
			if (!_context.Sessions.Any())
			{
				IList<Session> sessions = new List<Session>
				{
					new Session
					{
						Actor = new Actor
						{
							Username = "admin",
							Password = "admin",
							Role = AccountType.Admin
						}
					},
					new Session
					{
						Actor = new Actor
						{
							Username = "playgen",
							Password = "playgen",
							Role = AccountType.Admin
						}
					},
					new Session
					{
						Actor = new Actor
						{
							Username = "mayur",
							Password = "mayur"
						}
					},
					new Session
					{
						Actor = new Actor
						{
							Username = "jack",
							Password = "jack"
						}
					},
					new Session
					{
						Actor = new Actor
						{
							Username = "matt",
							Password = "matt"
						}
					},
					new Session
					{
						Actor = new Actor
						{
							Username = "ben",
							Password = "ben"
						}
					},
					new Session
					{
						Actor = new Actor
						{
							Username = "kam",
							Password = "kam"
						}
					}
				};

				_context.Sessions.AddRange(sessions);

				await SaveChanges(_context, isAsync);

				Debug.WriteLine("Actors & Sessions Created.");
			}
		}

		protected static async Task CreateFriends(SocialGamificationAssetContext _context, bool isAsync = false)
		{
			if (!_context.Friends.Any())
			{
				Actor mayur = await _context.Actors.Where(a => a.Username.Equals("mayur")).FirstOrDefaultAsync();
				Actor matt = await _context.Actors.Where(a => a.Username.Equals("matt")).FirstOrDefaultAsync();
				Actor jack = await _context.Actors.Where(a => a.Username.Equals("jack")).FirstOrDefaultAsync();
				Actor kam = await _context.Actors.Where(a => a.Username.Equals("kam")).FirstOrDefaultAsync();
				Actor ben = await _context.Actors.Where(a => a.Username.Equals("ben")).FirstOrDefaultAsync();

				IList<Friend> friends = new List<Friend>
				{
					new Friend
					{
						RequesterId = mayur.Id,
						RequesteeId = matt.Id
					},
					new Friend
					{
						RequesterId = mayur.Id,
						RequesteeId = jack.Id
					},
					new Friend
					{
						RequesterId = jack.Id,
						RequesteeId = matt.Id
					},
					new Friend
					{
						RequesterId = jack.Id,
						RequesteeId = kam.Id
					},
					new Friend
					{
						RequesterId = matt.Id,
						RequesteeId = ben.Id
					},
					new Friend
					{
						RequesterId = kam.Id,
						RequesteeId = ben.Id
					},
					new Friend
					{
						RequesterId = kam.Id,
						RequesteeId = mayur.Id
					},
				};

				_context.Friends.AddRange(friends);

				await SaveChanges(_context, isAsync);

				Debug.WriteLine("Friends Created.");
			}
		}

		protected static async Task SaveChanges(SocialGamificationAssetContext _context, bool isAsync = false)
		{
			try
			{
				if (isAsync)
				{
					await _context.SaveChangesAsync();
				}
				else
				{
					_context.SaveChanges();
				}
			}
			catch (DbEntityValidationException e)
			{
				throw e;
			}
		}
	}
}
