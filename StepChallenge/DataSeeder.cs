using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace StepChallenge
{
    public class DataSeed
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DataSeed(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        
        public async Task Run(StepContext db)
        {
            // TODO setting default settings here but if none exist, user should enter them on creating a new challenge. Only create defaults for development
            if (!db.ChallengeSettings.Any())
            {
                var settings = new ChallengeSettings
                {
                    Name = "Step Challenge 2019",
                    StartDate = new DateTime(2019, 09, 16, 0, 0, 0),
                    EndDate = new DateTime(2019, 12, 02, 0, 0, 0),
                    DurationInWeeks = 10,
                    ShowLeaderBoard = false,
                    ShowLeaderBoardStepCounts = false,
                    NumberOfParticipants = 48,
                    NumberOfParticipantsInATeam = 6,
                };

                db.ChallengeSettings.Add(settings);
                db.SaveChanges();
            }
            if (!db.Team.Any())
            {
                var teams = new List<Team>
                {
                    new Team
                    {
                        TeamId = 1,
                        TeamName = "Team_1",
                        Participants = new[]
                        {
                            
                            new Participant
                            {
                                ParticipantId = 1,
                                ParticipantName = "Alice",
                                IsAdmin = true,
                                IdentityUser = await GetIdentityUser("alice"),
                                Steps = GetSteps()
                            },
                            new Participant
                            {
                                ParticipantId = 2,
                                ParticipantName = "Bob",
                                IdentityUser = await GetIdentityUser("bob"),
                                Steps = GetSteps()
                            }
                        }
                    },
                    new Team
                    {
                        TeamId = 2,
                        TeamName = "Team_2",
                        Participants = new[]
                        {
                            new Participant
                            {
                                ParticipantId = 3,
                                ParticipantName = "Susan",
                                IdentityUser = await GetIdentityUser("susan"),
                                Steps = GetSteps()
                            },
                            new Participant
                            {
                                ParticipantId = 4,
                                ParticipantName = "Helga",
                                IdentityUser = await GetIdentityUser("helga"),
                                Steps = GetSteps()
                            }
                        }
                    }
                };
                
                teams.AddRange(GetTeams());
                db.Team.AddRange(teams);
                db.SaveChanges();
            }
        }

        public async Task SetupRoles()
        {
            // TODO - this is now duplicated in the participants servive and should use that faunction
             var user = new IdentityUser {UserName = "Admin"};
             await _userManager.CreateAsync(user, "AdminPassword1!");

             await _userManager.AddClaimAsync(user, new Claim("role", "Admin"));

             var adminRole = await _roleManager.FindByNameAsync("Admin");
             if (adminRole == null)
             {
                 adminRole = new IdentityRole("Admin");
                 await _roleManager.CreateAsync(adminRole);
                 await _roleManager.AddClaimAsync(adminRole, new Claim("Authentication", "Admin"));
             }

             if (!await _userManager.IsInRoleAsync(user, adminRole.Name))
             {
                 await _userManager.AddToRoleAsync(user, adminRole.Name);
             }
        }

        private List<Steps> GetSteps()
        {
            var week = 1;
            var monday = new DateTime(2019, 9, 16, 0, 0, 0);
            var steps = new List<Steps>
            {
                new Steps{
                    DateOfSteps = monday,
                    StepCount = GenerateRandomSteps(),
                    Week = week,
                    Day = 1,
                },
                new Steps{
                    DateOfSteps = monday.AddDays(1),
                    StepCount = GenerateRandomSteps(),
                    Week = week,
                    Day = 2,
                },
                new Steps{
                    DateOfSteps = monday.AddDays(5),
                    StepCount = GenerateRandomSteps(),
                    Week = week,
                    Day = 6,
                },
                new Steps{
                    DateOfSteps = monday.AddDays(4),
                    StepCount = GenerateRandomSteps(),
                    Week = week,
                    Day = 5,
                },
            };
            return steps;

            int GenerateRandomSteps()
            {
                Random rnd = new Random();
                return rnd.Next(0, 10); 
            }
            
        }

        private List<Team> GetTeams()
        {
            var newTeams = new List<Team>();
            var numberOfTeams = 8;
            for (int i = 2; i < numberOfTeams; i++)
            {
                newTeams.Add(new Team
                {
                    TeamName = "Team_" + (i + 1)
                });
            }
            return newTeams;
        }

        private async Task<IdentityUser> GetIdentityUser(string name)
        {
            var user = new IdentityUser {UserName = name};
            await _userManager.CreateAsync(user, $"{name}Password1!");
            return user;
        }
    }
}