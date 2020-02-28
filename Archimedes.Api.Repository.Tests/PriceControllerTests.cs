using System;
using System.Collections.Generic;
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
            var  controller = PriceControllerGetId(1);
            var result = await controller.Get(1);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(ActionResult),result);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task Should_ReturnOK_When_InValidIdToGetMethod()
        {
            var  controller = PriceControllerGetId(2);
            var result = await controller.Get(1);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(ActionResult),result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Should_ReturnOK_When_NoIdToGetMethod()
        {
            var  controller = PriceControllerGet();
            var result = await controller.Get();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(ActionResult),result);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task Should_ReturnNotFound_When_NullIsReturnedFromUnitOfWork()
        {
            var  controller = PriceControllerGetNull();
            var result = await controller.Get(10);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(ActionResult),result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Ignore("Unable to test null exception from unit of work")]
        [Test]
        public async Task Should_ReturnNotFound_When_NullToGetRequest()
        {
            var  controller = PriceControllerGetMarketNull();
            var result = await controller.Get("GBPUSD");

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(ActionResult),result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Ignore("Unable to test null exception from unit of work")]
        [Test]
        public async Task Should_ReturnNotFound_When_NullIsReturnedFromGranularityMarketFromDateToDate()
        {
            var  controller = PriceControllerGetMarketDateGranularityNull();
            var result = await controller.Get("GBPUSD","15","2020-01-01T05:00:00","2020-01-01T10:00:00");

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(ActionResult),result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Should_ReturnOk_When_MarketToGetMethod()
        {
            var  controller = PriceControllerGetMarket();
            var result = await controller.Get("GBPUSD");

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(ActionResult),result);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task Should_ReturnOk_When_MarketGranularityDateToGetMethod()
        {
            var  controller = PriceControllerGetMarketDateGranularity();
            var result = await controller.Get("GBPUSD","15","2020-01-01T05:00:00","2020-01-01T10:00:00");

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(ActionResult),result);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task Should_ReturnBadRequest_When_IncorrectDateToGetMethod()
        {
            var  controller = PriceControllerGetMarketDateGranularity();
            var result = await controller.Get("GBPUSD","15","bad","2020-01-01T10:00:00");

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(ActionResult),result);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task Should_ReturnOk_When_LastUpdatedGetMethod()
        {
            var  controller = PriceControllerGetLastUpdated();
            var result = await controller.Get("GBPUSD","15");

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(ActionResult),result);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task Should_ReturnBadRequest_When_IncorrectDateFromGetMethod()
        {
            var  controller = PriceControllerGetMarketDateGranularity();
            var result = await controller.Get("GBPUSD","15","2020-01-01T05:00:00","bad");

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(ActionResult),result);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task Should_ReturnOk_When_PostMethod()
        {
            var  controller = PriceControllerPost();
            var result = await controller.Post(new List<PriceDto>());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(ActionResult),result);
            Assert.IsInstanceOf<OkResult>(result);
        }


        private  PriceController PriceControllerGet()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<PriceController>>();

            mockUnitOfWork.Setup(m => m.Price.GetPrices(1,100)).ReturnsAsync(GetListOfPrices);

            return  new PriceController(mockMapper.Object, mockUnitOfWork.Object, mockLogger.Object);
        }

        private  PriceController PriceControllerGetMarket()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<PriceController>>();

            mockUnitOfWork.Setup(m => m.Price.GetPrices(price => price.Market == "GBPUSD")).ReturnsAsync(GetListOfPrices);

            return  new PriceController(mockMapper.Object, mockUnitOfWork.Object, mockLogger.Object);
        }

        private  PriceController PriceControllerGetMarketDateGranularity()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<PriceController>>();

            mockUnitOfWork.Setup(m => m.Price.GetPrices(a =>
                a.Market == "GBPUSD" 
                && a.Timestamp > new DateTime(2020,01,20,10,00,00) 
                && a.Timestamp <= new DateTime(2020,05,20,10,00,00) &&
                a.Granularity == "15")).ReturnsAsync(GetListOfPrices);

            return  new PriceController(mockMapper.Object, mockUnitOfWork.Object, mockLogger.Object);
        }

        private  PriceController PriceControllerGetLastUpdated()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<PriceController>>();

            mockUnitOfWork.Setup(m => m.Price.GetPrices(a =>
                a.Market == "GBPUSD" && a.Granularity == "15")).ReturnsAsync(GetListOfPrices);


            //mockUnitOfWork.Setup(m => m.Price.GetPrices(a =>
            //    a.Market == "GBPUSD" && a.Granularity == "15")).ReturnsAsync(new List<Price>(){new Price(){AskClose = 1},new Price(){AskClose = 2}});

            return  new PriceController(mockMapper.Object, mockUnitOfWork.Object, mockLogger.Object);
        }

        private  PriceController PriceControllerGetMarketDateGranularityNull()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<PriceController>>();

            mockUnitOfWork.Setup(m => m.Price.GetPrices(a =>
                a.Market == "GBPUSD" 
                && a.Timestamp > new DateTime(2020,01,20,10,00,00) 
                && a.Timestamp <= new DateTime(2020,05,20,10,00,00) &&
                a.Granularity == "15")).ReturnsAsync(default(IEnumerable<Price>));

            return  new PriceController(mockMapper.Object, mockUnitOfWork.Object, mockLogger.Object);
        }

        private  PriceController PriceControllerGetMarketNull()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<PriceController>>();

            mockUnitOfWork.Setup(m => m.Price.GetPrices(price => price.Market == "GBPUSD")).ReturnsAsync(default(IEnumerable<Price>));

            return  new PriceController(mockMapper.Object, mockUnitOfWork.Object, mockLogger.Object);
        }

        private  PriceController PriceControllerGetNull()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<PriceController>>();

            mockUnitOfWork.Setup(m => m.Price.GetPrice(10)).ReturnsAsync(default(Price));

            return  new PriceController(mockMapper.Object, mockUnitOfWork.Object, mockLogger.Object);
        }

        private  PriceController PriceControllerGetId(int id)
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<PriceController>>();

            mockUnitOfWork.Setup(m => m.Price.GetPrice(id)).Returns(Task.FromResult(GetPrice()));

            return  new PriceController(mockMapper.Object, mockUnitOfWork.Object, mockLogger.Object);
        }

        private  PriceController PriceControllerPost()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<PriceController>>();

            mockUnitOfWork.Setup(m => m.Price.AddPrice(new Price()));

            return  new PriceController(mockMapper.Object, mockUnitOfWork.Object, mockLogger.Object);
        }

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
        }

        private IEnumerable<Price> GetListOfPrices()
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
                Timestamp = new DateTime(2020,01,01)
            };

            var price2  = new Price()
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

            prices.Add(price1);
            prices.Add(price2);

            return prices;
        }

        private Price GetPrice()
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
                Timestamp = new DateTime(2020,01,01)
            };

            return price1;
        }



        [OneTimeTearDown]
        public void RunAfterTests()
        {
        }
    }
}