using Microsoft.Extensions.DependencyInjection;
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

				await CreateGroups(_context, isAsync);

				await CreatePlayerSessions(_context, isAsync);

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

		protected static async Task CreateGroups(SocialGamificationAssetContext _context, bool isAsync = false)
		{
			if (!_context.Friends.Any())
			{
				IList<Group> groups = new List<Group>
				{
					new Group
					{
						Name = "boardgame"
					},
					new Group
					{
						Name = "gameideas"
					},
					new Group
					{
						Name = "rage"
					}
				};

				_context.Groups.AddRange(groups);

				await SaveChanges(_context, isAsync);

				Debug.WriteLine("Groups Created.");
			}
		}

		protected static async Task CreatePlayerSessions(SocialGamificationAssetContext _context, bool isAsync = false)
		{
			if (!_context.Sessions.Any())
			{
				Group boardgame = await _context.Groups.Where(a => a.Name.Equals("boardgame")).FirstOrDefaultAsync();
				Group gameideas = await _context.Groups.Where(a => a.Name.Equals("gameideas")).FirstOrDefaultAsync();
				Group rage = await _context.Groups.Where(a => a.Name.Equals("rage")).FirstOrDefaultAsync();

				IList<Session> sessions = new List<Session>
				{
					new Session
					{
						Player = new Player
						{
							Username = "admin",
							Password = "admin",
							Role = AccountType.Admin
						}
					},
					new Session
					{
						Player = new Player
						{
							Username = "playgen",
							Password = "playgen",
							Role = AccountType.Admin
						}
					},
					new Session
					{
						Player = new Player
						{
							Username = "mayur",
							Password = "mayur",
							Groups = new List<Group> { boardgame, gameideas, rage }
						}
					},
					new Session
					{
						Player = new Player
						{
							Username = "jack",
							Password = "jack",
							Groups = new List<Group> { gameideas, rage }
						}
					},
					new Session
					{
						Player = new Player
						{
							Username = "matt",
							Password = "matt",
							Groups = new List<Group> { boardgame, rage }
						}
					},
					new Session
					{
						Player = new Player
						{
							Username = "ben",
							Password = "ben",
							Groups = new List<Group> { boardgame, gameideas }
						}
					},
					new Session
					{
						Player = new Player
						{
							Username = "kam",
							Password = "kam",
							Groups = new List<Group> { gameideas }
						}
					}
				};

				_context.Sessions.AddRange(sessions);

				await SaveChanges(_context, isAsync);

				Debug.WriteLine("Players & Sessions Created.");
			}
		}

		protected static async Task CreateFriends(SocialGamificationAssetContext _context, bool isAsync = false)
		{
			if (!_context.Friends.Any())
			{
				Player mayur = await _context.Players.Where(a => a.Username.Equals("mayur")).FirstOrDefaultAsync();
				Player matt = await _context.Players.Where(a => a.Username.Equals("matt")).FirstOrDefaultAsync();
				Player jack = await _context.Players.Where(a => a.Username.Equals("jack")).FirstOrDefaultAsync();
				Player kam = await _context.Players.Where(a => a.Username.Equals("kam")).FirstOrDefaultAsync();
				Player ben = await _context.Players.Where(a => a.Username.Equals("ben")).FirstOrDefaultAsync();

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
