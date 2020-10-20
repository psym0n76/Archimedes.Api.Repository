using System;
using System.Threading;
using System.Threading.Tasks;
using Archimedes.Api.Repository.Controllers;
using Archimedes.Library.Message.Dto;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Archimedes.Api.Repository.Tests
{
    [TestFixture]
    public class StrategyControllerTests
    {
        [Test]
        public async Task Should_ReturnOK_When_IdToGetMethod()
        {
            var controller = StrategyControllerGetId(1);
            var result = await controller.GetStrategyAsync(1, CancellationToken.None);

            Assert.That(result, Is.InstanceOf<ActionResult<StrategyDto>>());
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        private static StrategyController StrategyControllerGetId(int id)
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger<StrategyController>>();
            var mockMapper = new Mock<IMapper>();

            mockUnitOfWork.Setup(m => m.Strategy.GetStrategyAsync(id, CancellationToken.None))
                .ReturnsAsync(GetStrategy());

            return new StrategyController(mockUnitOfWork.Object, mockLogger.Object, mockMapper.Object);
        }

        private static Strategy GetStrategy()
        {
            var strategy = new Strategy()
            {
                Id = 1,
                Name = "PIVOT HIGH",

                Market = "GBP/USD",
                Granularity = "15",
                Active = true,

                StartDate = new DateTime(2020, 01, 01),
                EndDate = new DateTime(2020, 01, 01),

                Count = 1,
                LastUpdated = new DateTime(2020, 01, 01),
            };

            return strategy;
        }
    }
}