using System;
using System.Collections.Generic;
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
    public class PriceControllerTests
    {

        [Test]
        public async Task Should_ReturnOK_When_IdToGetMethod()
        {
            var controller = PriceControllerGetId(1);
            var result = await controller.GetPriceAsync(1, CancellationToken.None);

            Assert.That(result, Is.InstanceOf<ActionResult<Price>>());
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task Should_ReturnNotFound_When_InValidIdToGetMethod()
        {
            var controller = PriceControllerGetId(2);
            var result = await controller.GetPriceAsync(1, CancellationToken.None);


            Assert.That(result, Is.InstanceOf<ActionResult<Price>>());
            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
            Assert.That(result.Value, Is.Null); // not populated ???

        }

        [Test]
        public async Task Should_ReturnNotFound_When_NoIdToGetMethod()
        {
            var controller = PriceControllerGet();
            var result = await controller.GetPriceAsync(4, CancellationToken.None);

            Assert.That(result, Is.InstanceOf<ActionResult<Price>>());
            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Should_ReturnNotFound_When_NullIsReturnedFromUnitOfWork()
        {
            var controller = PriceControllerGetNull();
            var result = await controller.GetPriceAsync(10, CancellationToken.None);

            Assert.That(result, Is.InstanceOf<ActionResult<Price>>());
            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
            Assert.That(result.Value, Is.Null);
        }

        [Ignore("Unable to test null exception from unit of work")]
        [Test]
        public async Task Should_ReturnNotFound_When_NullToGetRequest()
        {
            var controller = PriceControllerGetMarketNull();
            var result = await controller.GetMarketPricesAsync("GBPUSD", CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(ActionResult), result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Ignore("Unable to test null exception from unit of work")]
        [Test]
        public async Task Should_ReturnNotFound_When_NullIsReturnedFromGranularityMarketFromDateToDate()
        {
            var controller = PriceControllerGetMarketDateGranularityNull();
            var result = await controller.GetMarketGranularityPricesDate("GBPUSD", "15", "2020-01-01T05:00:00",
                "2020-01-01T10:00:00", CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(ActionResult), result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Should_ReturnOk_When_MarketToGetMethod()
        {
            var controller = PriceControllerGetMarket();
            var result = await controller.GetMarketPricesAsync("GBPUSD", CancellationToken.None);

            Assert.That(result, Is.InstanceOf<ActionResult<IEnumerable<Price>>>());
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            Assert.That(result.Value, Is.Null);
        }

        [Test]
        public async Task Should_ReturnOk_When_MarketGranularityDateToGetMethod()
        {
            var controller = PriceControllerGetMarketDateGranularity();
            var result = await controller.GetMarketGranularityPricesDate("GBPUSD", "15", "2020-01-01T05:00:00",
                "2020-01-01T10:00:00", CancellationToken.None);

            Assert.That(result, Is.InstanceOf<ActionResult<Price>>());
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            Assert.That(result.Value, Is.Null);
        }

        [Test]
        public async Task Should_ReturnBadRequest_When_IncorrectDateToGetMethod()
        {
            var controller = PriceControllerGetMarketDateGranularity();
            var result = await controller.GetMarketGranularityPricesDate("GBPUSD", "15", "bad", "2020-01-01T10:00:00",
                CancellationToken.None);

            Assert.That(result, Is.InstanceOf<ActionResult<Price>>());
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            Assert.That(result.Value, Is.Null);
        }

        [Test]
        public async Task Should_ReturnOk_When_LastUpdatedGetMethod()
        {
            var controller = PriceControllerGetLastUpdated();
            var result = await controller.GetLastUpdated("GBPUSD", "15", CancellationToken.None);

            Assert.That(result, Is.InstanceOf<ActionResult<DateTime>>());
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task Should_ReturnBadRequest_When_IncorrectDateFromGetMethod()
        {
            var controller = PriceControllerGetMarketDateGranularity();
            var result = await controller.GetMarketGranularityPricesDate("GBPUSD", "15", "2020-01-01T05:00:00", "bad",
                CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.That(result, Is.InstanceOf<ActionResult<Price>>());
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            Assert.That(result.Value, Is.Null);
        }

        [Test]
        public async Task Should_ReturnOk_When_PostMethod()
        {
            var controller = PriceControllerPost();
            var result = await controller.PostPrice(new List<Price>(), CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(ActionResult), result);
            Assert.IsInstanceOf<OkResult>(result);
        }


        private PriceController PriceControllerGet()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<PriceController>>();

            mockUnitOfWork.Setup(m => m.Price.GetPricesAsync(1, 100, CancellationToken.None))
                .ReturnsAsync(GetListOfPrices);

            return new PriceController(mockUnitOfWork.Object, mockLogger.Object);
        }

        private PriceController PriceControllerGetMarket()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<PriceController>>();

            mockUnitOfWork.Setup(m => m.Price.GetPricesAsync(price => price.Market == "GBPUSD", CancellationToken.None))
                .ReturnsAsync(GetListOfPrices);

            return new PriceController(mockUnitOfWork.Object, mockLogger.Object);
        }

        private PriceController PriceControllerGetMarketDateGranularity()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<PriceController>>();

            mockUnitOfWork.Setup(m => m.Price.GetPricesAsync(a =>
                a.Market == "GBPUSD"
                && a.Timestamp > new DateTime(2020, 01, 20, 10, 00, 00)
                && a.Timestamp <= new DateTime(2020, 05, 20, 10, 00, 00) &&
                a.Granularity == "15", CancellationToken.None)).ReturnsAsync(GetListOfPrices);

            return new PriceController(mockUnitOfWork.Object, mockLogger.Object);
        }

        private PriceController PriceControllerGetLastUpdated()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<PriceController>>();

            mockUnitOfWork.Setup(m => m.Price.GetPricesAsync(a =>
                a.Market == "GBPUSD" && a.Granularity == "15", CancellationToken.None)).ReturnsAsync(GetListOfPrices);

            mockUnitOfWork
                .Setup(a => a.Price.GetLastUpdated(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync(default(DateTime));

            return new PriceController(mockUnitOfWork.Object, mockLogger.Object);
        }

        private static PriceController PriceControllerGetMarketDateGranularityNull()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<PriceController>>();

            mockUnitOfWork.Setup(m => m.Price.GetPricesAsync(a =>
                a.Market == "GBPUSD"
                && a.Timestamp > new DateTime(2020, 01, 20, 10, 00, 00)
                && a.Timestamp <= new DateTime(2020, 05, 20, 10, 00, 00) &&
                a.Granularity == "15", CancellationToken.None)).ReturnsAsync(default(List<Price>));

            return new PriceController(mockUnitOfWork.Object, mockLogger.Object);
        }

        private static PriceController PriceControllerGetMarketNull()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<PriceController>>();

            mockUnitOfWork.Setup(m => m.Price.GetPricesAsync(price => price.Market == "GBPUSD", CancellationToken.None))
                .ReturnsAsync(default(List<Price>));

            return new PriceController(mockUnitOfWork.Object, mockLogger.Object);
        }

        private static PriceController PriceControllerGetNull()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<PriceController>>();

            mockUnitOfWork.Setup(m => m.Price.GetPriceAsync(It.IsAny<long>(), CancellationToken.None))
                .ReturnsAsync(default(Price));

            return new PriceController(mockUnitOfWork.Object, mockLogger.Object);
        }

        private static PriceController PriceControllerGetId(int id)
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<PriceController>>();

            mockUnitOfWork.Setup(m => m.Price.GetPriceAsync(id, CancellationToken.None))
                .Returns(Task.FromResult(GetPrice()));

            return new PriceController(mockUnitOfWork.Object, mockLogger.Object);
        }

        private static PriceController PriceControllerPost()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<PriceController>>();

            mockUnitOfWork.Setup(m => m.Price.AddPriceAsync(new Price(),CancellationToken.None));

            return new PriceController(mockUnitOfWork.Object, mockLogger.Object);
        }

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
        }

        private static List<Price> GetListOfPrices()
        {
            var prices = new List<Price>();

            var price1 = new Price()
            {
                AskClose = 1.25,
                AskHigh = 1.24,
                AskLow = 1.24,
                AskOpen = 1.25,
                BidClose = 1.24,
                BidHigh = 1.25,
                BidOpen = 1.27,
                BidLow = 1.24,
                Granularity = "15",
                Id = 1,
                Market = "GBPUSD",
                TickQty = 2540,
                Timestamp = new DateTime(2020, 01, 01)
            };

            var price2 = new Price()
            {
                AskClose = 1.25,
                AskHigh = 1.24,
                AskLow = 1.24,
                AskOpen = 1.25,
                BidClose = 1.24,
                BidHigh = 1.25,
                BidOpen = 1.27,
                BidLow = 1.24,
                Granularity = "15",
                Id = 1,
                Market = "GBPUSD",
                TickQty = 2540,
                Timestamp = new DateTime(2020, 01, 01)
            };

            prices.Add(price1);
            prices.Add(price2);

            return prices;
        }

        private static Price GetPrice()
        {

            var price1 = new Price()
            {
                AskClose = 1.25,
                AskHigh = 1.24,
                AskLow = 1.24,
                AskOpen = 1.25,
                BidClose = 1.24,
                BidHigh = 1.25,
                BidOpen = 1.27,
                BidLow = 1.24,
                Granularity = "15",
                Id = 1,
                Market = "GBPUSD",
                TickQty = 2540,
                Timestamp = new DateTime(2020, 01, 01)
            };

            return price1;
        }

        [OneTimeTearDown]
        public void RunAfterTests()
        {
        }
    }
}