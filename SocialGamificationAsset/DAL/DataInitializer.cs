using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using SocialGamificationAsset.Tests.Controllers;

namespace SocialGamificationAsset.Models
{
    public class DataInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider, bool isAsync = false)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var _context = serviceScope.ServiceProvider.GetService<SocialGamificationAssetContext>();

                await SeedGames(_context, isAsync);

                await SeedGroups(_context, isAsync);

                await SeedPlayerSessions(_context, isAsync);

                await SeedAlliances(_context, isAsync);

                await SeedCustomData(_context, isAsync);
            }
        }

        protected static void Test()
        {
            var matchActorGet = @"
            [
	            {
		            ""$id"": ""1"",
		            ""matchId"": ""ab66cb81-7534-41ba-b104-7ce1c76f442a"",
		            ""match"": null,
		            ""actorId"": ""59aeb9ac-732a-4c03-8911-0c052b4c9d14"",
		            ""actor"": {
			            ""$id"": ""2"",
			            ""username"": ""matt"",
			            ""email"": null,
			            ""role"": 1,
			            ""groups"": null,
			            ""class"": ""Player"",
			            ""isEnabled"": true,
			            ""updatedDate"": ""2016-02-25T10:10:53"",
			            ""createdDate"": ""2016-02-25T10:10:53"",
			            ""id"": ""59aeb9ac-732a-4c03-8911-0c052b4c9d14""
		            },
		            ""customData"": null,
		            ""updatedDate"": ""2016-02-25T10:17:02"",
		            ""createdDate"": ""2016-02-25T10:17:02"",
		            ""id"": ""6acd0be2-af03-4728-95bf-dd2d573bf457""
	            },
	            {
		            ""$id"": ""3"",
		            ""matchId"": ""ab66cb81-7534-41ba-b104-7ce1c76f442a"",
		            ""match"": null,
		            ""actorId"": ""3e08fff9-346e-4d9d-84ab-e19e0f9c84ac"",
		            ""actor"": {
			            ""$id"": ""4"",
			            ""username"": ""mayur"",
			            ""email"": null,
			            ""role"": 1,
			            ""groups"": null,
			            ""class"": ""Player"",
			            ""isEnabled"": true,
			            ""updatedDate"": ""2016-02-25T10:10:53"",
			            ""createdDate"": ""2016-02-25T10:10:53"",
			            ""id"": ""3e08fff9-346e-4d9d-84ab-e19e0f9c84ac""
		            },
		            ""customData"": null,
		            ""updatedDate"": ""2016-02-25T10:17:02"",
		            ""createdDate"": ""2016-02-25T10:17:02"",
		            ""id"": ""e4384134-7e4b-47b5-90bd-e330f4500578""
	            }
            ]";

            var settings = new JsonSerializerSettings { Converters = { new ActorJsonConverter() } };

            var matchActors = JsonConvert.DeserializeObject<List<MatchActor>>(matchActorGet, settings);
        }

        protected static async Task SeedGames(SocialGamificationAssetContext _context, bool isAsync = false)
        {
            if (!_context.GameRegistry.Any())
            {
                IList<GameRegistry> games = new List<GameRegistry>
                {
                    new GameRegistry { Name = "ButtonGame", DeveloperName = "PlayGen" },
                    new GameRegistry { Name = "TankGame", DeveloperName = "PlayGen" }
                };

                _context.GameRegistry.AddRange(games);

                await SaveChanges(_context, isAsync);

                Debug.WriteLine("Games Seeded.");

                // GameRegistry Game = await _context.GameRegistry.Where(g => g.Username.Equals("ButtonGame")).FirstOrDefaultAsync();
            }
        }

        protected static async Task SeedGroups(SocialGamificationAssetContext _context, bool isAsync = false)
        {
            if (!_context.Alliances.Any())
            {
                IList<Group> groups = new List<Group>
                {
                    new Group { Username = "boardgame" },
                    new Group { Username = "gameideas" },
                    new Group { Username = "rage" }
                };

                _context.Groups.AddRange(groups);

                await SaveChanges(_context, isAsync);

                Debug.WriteLine("Groups Seeded.");
            }
        }

        protected static async Task SeedPlayerSessions(SocialGamificationAssetContext _context, bool isAsync = false)
        {
            if (!_context.Sessions.Any())
            {
                var boardgame = await _context.Groups.Where(a => a.Username.Equals("boardgame")).FirstOrDefaultAsync();
                var gameideas = await _context.Groups.Where(a => a.Username.Equals("gameideas")).FirstOrDefaultAsync();
                var rage = await _context.Groups.Where(a => a.Username.Equals("rage")).FirstOrDefaultAsync();

                IList<Session> sessions = new List<Session>
                {
                    new Session
                    {
                        Player =
                            new Player
                            {
                                Username = "admin",
                                Password = Helper.HashPassword("admin"),
                                Role = AccountType.Admin
                            }
                    },
                    new Session
                    {
                        Player =
                            new Player
                            {
                                Username = "playgen",
                                Password = Helper.HashPassword("playgen"),
                                Role = AccountType.Admin
                            }
                    },
                    new Session
                    {
                        Player =
                            new Player
                            {
                                Username = "mayur",
                                Email = "mayur@playgen.com",
                                Password = Helper.HashPassword("mayur"),
                                Groups = new List<Group> { boardgame, gameideas, rage }
                            }
                    },
                    new Session
                    {
                        Player =
                            new Player
                            {
                                Username = "jack",
                                Email = "jack@playgen.com",
                                Password = Helper.HashPassword("jack"),
                                Groups = new List<Group> { gameideas, rage }
                            }
                    },
                    new Session
                    {
                        Player =
                            new Player
                            {
                                Username = "matt",
                                Email = "matt@playgen.com",
                                Password = Helper.HashPassword("matt"),
                                Groups = new List<Group> { boardgame, rage }
                            }
                    },
                    new Session
                    {
                        Player =
                            new Player
                            {
                                Username = "ben",
                                Password = Helper.HashPassword("ben"),
                                Groups = new List<Group> { boardgame, gameideas }
                            }
                    },
                    new Session
                    {
                        Player =
                            new Player
                            {
                                Username = "kam",
                                Password = Helper.HashPassword("kam"),
                                Groups = new List<Group> { gameideas }
                            }
                    }
                };

                _context.Sessions.AddRange(sessions);

                await SaveChanges(_context, isAsync);

                Debug.WriteLine("Players & Sessions Seeded.");
            }
        }

        protected static async Task SeedAlliances(SocialGamificationAssetContext _context, bool isAsync = false)
        {
            if (!_context.Alliances.Any())
            {
                var mayur = await _context.Players.Where(a => a.Username.Equals("mayur")).FirstOrDefaultAsync();
                var matt = await _context.Players.Where(a => a.Username.Equals("matt")).FirstOrDefaultAsync();
                var jack = await _context.Players.Where(a => a.Username.Equals("jack")).FirstOrDefaultAsync();
                var kam = await _context.Players.Where(a => a.Username.Equals("kam")).FirstOrDefaultAsync();
                var ben = await _context.Players.Where(a => a.Username.Equals("ben")).FirstOrDefaultAsync();

                IList<Alliance> alliances = new List<Alliance>
                {
                    new Alliance { RequesterId = mayur.Id, RequesteeId = matt.Id, State = AllianceState.Pending },
                    new Alliance { RequesterId = mayur.Id, RequesteeId = jack.Id, State = AllianceState.Accepted },
                    new Alliance { RequesterId = jack.Id, RequesteeId = matt.Id, State = AllianceState.Declined },
                    new Alliance { RequesterId = kam.Id, RequesteeId = mayur.Id, State = AllianceState.Accepted }
                };

                _context.Alliances.AddRange(alliances);

                await SaveChanges(_context, isAsync);

                Debug.WriteLine("Alliances Seeded.");
            }
        }

        protected static async Task SeedCustomData(SocialGamificationAssetContext _context, bool isAsync = false)
        {
            if (!_context.CustomData.Any())
            {
                var mayur = await _context.Players.Where(a => a.Username.Equals("mayur")).FirstOrDefaultAsync();
                var matt = await _context.Players.Where(a => a.Username.Equals("matt")).FirstOrDefaultAsync();
                var jack = await _context.Players.Where(a => a.Username.Equals("jack")).FirstOrDefaultAsync();
                var kam = await _context.Players.Where(a => a.Username.Equals("kam")).FirstOrDefaultAsync();
                var ben = await _context.Players.Where(a => a.Username.Equals("ben")).FirstOrDefaultAsync();

                IList<CustomData> customData = new List<CustomData>
                {
                    new CustomData
                    {
                        Key = "ip",
                        Value = "127.0.0.1",
                        ObjectId = mayur.Id,
                        ObjectType = CustomDataType.Player
                    },
                    new CustomData
                    {
                        Key = "ip",
                        Value = "127.0.0.1",
                        ObjectId = matt.Id,
                        ObjectType = CustomDataType.Player
                    },
                    new CustomData
                    {
                        Key = "ip",
                        Value = "127.0.0.1",
                        ObjectId = jack.Id,
                        ObjectType = CustomDataType.Player
                    },
                    new CustomData
                    {
                        Key = "ip",
                        Value = "127.0.0.1",
                        ObjectId = ben.Id,
                        ObjectType = CustomDataType.Player
                    },
                    new CustomData
                    {
                        Key = "video_id",
                        Value = "1234",
                        ObjectId = mayur.Id,
                        ObjectType = CustomDataType.Player
                    },
                    new CustomData
                    {
                        Key = "video_id",
                        Value = "1234",
                        ObjectId = matt.Id,
                        ObjectType = CustomDataType.Player
                    },
                    new CustomData
                    {
                        Key = "video_id",
                        Value = "1234",
                        ObjectId = jack.Id,
                        ObjectType = CustomDataType.Player
                    },
                    new CustomData
                    {
                        Key = "chat_id",
                        Value = "235f73ea-e54f-4150-8dc3-3eb9995d0728",
                        ObjectId = mayur.Id,
                        ObjectType = CustomDataType.Player
                    },
                    new CustomData
                    {
                        Key = "chat_id",
                        Value = "235f73ea-e54f-4150-8dc3-3eb9995d0728",
                        ObjectId = matt.Id,
                        ObjectType = CustomDataType.Player
                    }
                };

                _context.CustomData.AddRange(customData);

                await SaveChanges(_context, isAsync);

                Debug.WriteLine("CustomData Seeded.");
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
            catch (DbUpdateException e)
            {
                throw e;
            }
        }
    }
}