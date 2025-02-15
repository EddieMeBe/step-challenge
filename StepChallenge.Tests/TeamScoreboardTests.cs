using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Model;
using Moq;
using NUnit.Framework;
using StepChallenge.Services;

namespace StepChallenge.Tests
{
    public class TeamScoreboardTests : BaseTests
    {
        /// <summary>
        /// Tests if a participant has steps saved, they have a step status of true - steps saved against this date
        /// </summary>
        [Test]
        public void Test_TeamMembersStepStatus_IsTrueIfStepsAreSaved()
        {
            var team = TestData.CreateTeamForTeamScoreboard();
            team.Participants.First().Steps = TestData.CreateSteps(10, team.Participants.First().ParticipantId, StartDate);

            var teamService = new TeamService(GetMockStepContext(team));
            var result = teamService.GetTeamScoreBoard(1);

            var resultFirstParticipantStepsStatus = result.First().ParticipantsStepsStatus.First().ParticipantAddedStepCount;
            
            Assert.IsTrue(resultFirstParticipantStepsStatus, $"Expected participant to have step status of true but got {resultFirstParticipantStepsStatus}"); 
        }

        /// <summary>
        /// Tests if a participant has not saved their steps, they have a step status of false - no steps saved against this date
        /// </summary>
        [Test]
        public void Test_TeamMembersStepStatus_IsFalseIfNoStepsAreSaved()
        {
            var team = TestData.CreateTeamForTeamScoreboard();
            team.Participants.First().Steps = new List<Steps>();

            var teamService = new TeamService(GetMockStepContext(team));
            var result = teamService.GetTeamScoreBoard(1);

            var resultFirstParticipantStepsStatus = result.First().ParticipantsStepsStatus.First().ParticipantAddedStepCount;
            
            Assert.IsFalse(resultFirstParticipantStepsStatus, $"Expected participant to have step status of False but got {resultFirstParticipantStepsStatus}"); 
        }

        /// <summary>
        /// Tests that if a participant has saved a step count of Zero, they have a step status of false. We assume no participants will have a step count of Zero and they have no steps saved against this date
        /// </summary>
        [Test]
        public void Test_TeamMembersStepStatus_IsFalseIfStepsTotalIsZero()
        {
            var team = TestData.CreateTeamForTeamScoreboard();
            team.Participants.First().Steps = TestData.CreateSteps(0, team.Participants.First().ParticipantId, StartDate);

            var teamService = new TeamService(GetMockStepContext(team));
            var result = teamService.GetTeamScoreBoard(1);

            var resultFirstParticipantStepsStatus = result.First().ParticipantsStepsStatus.First().ParticipantAddedStepCount;
            
            Assert.IsFalse(resultFirstParticipantStepsStatus, $"Expected participant to have step status of False but got {resultFirstParticipantStepsStatus}"); 
        }
        
        private StepContext GetMockStepContext(Team team)
        {
            var teamSteps = new List<Steps>();
            var participants = new List<Participant>();
            foreach (var participant in team.Participants)
            {
                participants.Add(participant);
                if (participant.Steps != null)
                {
                    teamSteps.AddRange(participant.Steps);
                }
            }

            var mockContext = new Mock<StepContext>();
            
            mockContext.Setup(x => x.Steps).Returns(GetDbSetSteps(teamSteps).Object);
            mockContext.Setup(x => x.Participants).Returns(GetDbSetParticipants(participants).Object);

            return mockContext.Object;
        }

        private Mock<DbSet<Steps>> GetDbSetSteps(List<Steps> stepsList)
        {
            var steps = stepsList.AsQueryable();

            var mockStepsDb = new Mock<DbSet<Steps>>();
            mockStepsDb.As<IQueryable<Steps>>().Setup(m => m.Expression).Returns(steps.Expression);
            mockStepsDb.As<IQueryable<Steps>>().Setup(m => m.Provider).Returns(steps.Provider);
            mockStepsDb.As<IQueryable<Steps>>().Setup(m => m.Expression).Returns(steps.Expression);
            mockStepsDb.As<IQueryable<Steps>>().Setup(m => m.ElementType).Returns(steps.ElementType);
            mockStepsDb.As<IQueryable<Steps>>().Setup(m => m.GetEnumerator()).Returns(steps.GetEnumerator());

            return mockStepsDb;
        }

        private Mock<DbSet<Participant>> GetDbSetParticipants(List<Participant> participantList)
        {
            var participants = participantList.AsQueryable();

            var mockParticipantSet = new Mock<DbSet<Participant>>();
            mockParticipantSet.As<IQueryable<Participant>>().Setup(m => m.Expression).Returns(participants.Expression);
            mockParticipantSet.As<IQueryable<Participant>>().Setup(m => m.Provider).Returns(participants.Provider);
            mockParticipantSet.As<IQueryable<Participant>>().Setup(m => m.Expression).Returns(participants.Expression);
            mockParticipantSet.As<IQueryable<Participant>>().Setup(m => m.ElementType).Returns(participants.ElementType);
            mockParticipantSet.As<IQueryable<Participant>>().Setup(m => m.GetEnumerator()).Returns(participants.GetEnumerator());

            return mockParticipantSet;
        }
    }
}