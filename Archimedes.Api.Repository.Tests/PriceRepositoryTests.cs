using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Archimedes.Api.Repository.Controllers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Archimedes.Api.Repository.Tests
{
    [TestFixture]
    public class PriceRepositoryTests
    {
        private static Price _price;
        private static Price _priceTwo;
        private static IEnumerable<Price> _prices;

        [Test]
        public async Task Should_Return_OK_Response_When_Passing_Valid_Id_To_Get_Request()
        {
            var  controller = GetMockPriceControllerWithId(1);
            var result = await controller.Get(1);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(ActionResult),result);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task Should_Return_OK_Response_When_Passing_InValid_Id_To_Get_Request()
        {
            var  controller = GetMockPriceControllerWithId(2);
            var result = await controller.Get(1);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(ActionResult),result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }


        [Test]
        public async Task Should_Return_OK_Response_When_Passing_No_Id_To_Get_Request()
        {
            var  controller = GetMockPriceController();
            var result = await controller.Get();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(ActionResult),result);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task Should_Return_NotFound_When_Null_Is_Returned_from_repo()
        {
            var  controller = GetMockPriceControllerNull();
            var result = await controller.Get();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(ActionResult),result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Should_Return_Ok_When_Passing_Market_To_Get_Request()
        {
            var  controller = GetMockPriceControllerMarket();
            var result = await controller.Get("GBPUSD");

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(ActionResult),result);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task Should_Return_Ok_When_Passing_Null_To_Get_Request()
        {
            var  controller = GetMockPriceControllerMarketNull();
            var result = await controller.Get("GBPUSD");

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(ActionResult),result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        private static PriceController GetMockPriceController()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<PriceController>>();

            mockUnitOfWork.Setup(m => m.Price.GetPrices(1,100)).Returns(Task.FromResult(_prices));

            return  new PriceController(mockMapper.Object, mockUnitOfWork.Object, mockLogger.Object);
        }

        private static PriceController GetMockPriceControllerMarket()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<PriceController>>();

            mockUnitOfWork.Setup(m => m.Price.GetPrices(price => price.Market == "GBPUSD")).Returns(Task.FromResult(_prices));

            return  new PriceController(mockMapper.Object, mockUnitOfWork.Object, mockLogger.Object);
        }

        private static PriceController GetMockPriceControllerMarketNull()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<PriceController>>();

            mockUnitOfWork.Setup(m => m.Price.GetPrices(price => price.Market == "GBPUSD")).Returns(Task.FromResult((IEnumerable<Price>)null));

            return  new PriceController(mockMapper.Object, mockUnitOfWork.Object, mockLogger.Object);
        }

        private static PriceController GetMockPriceControllerNull()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<PriceController>>();

            mockUnitOfWork.Setup(m => m.Price.GetPrices(1,100)).Returns(Task.FromResult((IEnumerable<Price>)null));

            return  new PriceController(mockMapper.Object, mockUnitOfWork.Object, mockLogger.Object);
        }

        private static PriceController GetMockPriceControllerWithId(int id)
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<PriceController>>();

            mockUnitOfWork.Setup(m => m.Price.GetPrice(id)).Returns(Task.FromResult(_price));

            return  new PriceController(mockMapper.Object, mockUnitOfWork.Object, mockLogger.Object);
        }

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            _price = new Price()
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
                Timestamp = new DateTime(2020,01,01)
            };

            _priceTwo = new Price()
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
                Timestamp = new DateTime(2020,01,01)
            };

            _prices = new List<Price>(){_price,_priceTwo};

        }

        [OneTimeTearDown]
        public void RunAfterTests()
        {
        }
    }
}