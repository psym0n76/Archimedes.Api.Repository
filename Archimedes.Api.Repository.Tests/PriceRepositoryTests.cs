using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Data.SqlClient;

namespace Archimedes.Api.Repository.Tests
{
    [TestFixture]
    public class PriceRepositoryTests
    {
        private static Price _price;
        private static Price _priceTwo;
        private static IEnumerable<Price> _prices;
        private const string Connection = "Server=localhost\\SQLEXPRESS;Database=Archimedes;Integrated Security=SSPI;";

        [Test]
        public async Task Should_ReturnOk_WhenPostingPriceData()
        {
            try
            {
                var repo = GetRepository();
                await repo.AddPrices(_prices);
                await repo.FxDatabaseContext.SaveChangesAsync();
                DeleteFromTable();
                Assert.IsTrue(true);
            }
            catch 
            {
                Assert.IsTrue(false);
            }
        }

        [Test]
        public async Task Should_ReturnTwoRecords_WhenGetPricesIsCalled()
        {

            var repo = GetRepository();
            AddToTable();
            var  result = await repo.GetPrices(x => x.Market == "TEST_GBPUSD");

            Assert.IsTrue(result.Count() == 5);
            DeleteFromTable();

        }

        [Test]
        public async Task Should_ReturnNoRecords_WhenGetPricesIsCalled()
        {

            var repo = GetRepository();
            AddToTable();
            var  result = await repo.GetPrices(x => x.Market == "GBPUSD");

            Assert.IsTrue(!result.Any());
            DeleteFromTable();

        }

        private static ArchimedesContext GetContext()
        {
            var option = new DbContextOptionsBuilder<ArchimedesContext>();
            option.UseSqlServer(Connection);

            return new ArchimedesContext(option.Options);
        }

        private static PriceRepository GetRepository()
        {
            return  new PriceRepository(GetContext());
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
                Market = "TEST_GBPUSD",
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
                Market = "TEST_GBPUSD",
                TickQty = 2540,
                Timestamp = new DateTime(2020,01,01)
            };

            _prices = new List<Price>(){_price,_priceTwo};
        }

        [OneTimeTearDown]
        public void RunAfterTests()
        {
            DeleteFromTable();
        }

        private static void DeleteFromTable()
        {
            using (var connection = new SqlConnection(Connection))
            {
                const string queryString = "DELETE FROM PRICES WHERE Market = 'TEST_GBPUSD'";
                var command = new SqlCommand(queryString, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }


        private static void  AddToTable()
        {
            const string queryString = "INSERT [dbo].[Prices] ([Market], [Granularity], [BidOpen], [BidClose], [BidHigh], [BidLow], [AskOpen], [AskClose], [AskHigh], [AskLow], [TickQty], [Timestamp]) VALUES (N'TEST_GBPUSD', NULL, 0, 0, 0, 0, 0, 0, 0, 0, 0, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2))" +
                                       "INSERT [dbo].[Prices] ([Market], [Granularity], [BidOpen], [BidClose], [BidHigh], [BidLow], [AskOpen], [AskClose], [AskHigh], [AskLow], [TickQty], [Timestamp]) VALUES (N'TEST_GBPUSD', NULL, 0, 0, 0, 0, 0, 0, 0, 0, 0, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2))" +
                                       "INSERT [dbo].[Prices] ([Market], [Granularity], [BidOpen], [BidClose], [BidHigh], [BidLow], [AskOpen], [AskClose], [AskHigh], [AskLow], [TickQty], [Timestamp]) VALUES (N'TEST_GBPUSD', NULL, 0, 0, 0, 0, 0, 0, 0, 0, 0, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2))" +
                                       "INSERT [dbo].[Prices] ([Market], [Granularity], [BidOpen], [BidClose], [BidHigh], [BidLow], [AskOpen], [AskClose], [AskHigh], [AskLow], [TickQty], [Timestamp]) VALUES (N'TEST_GBPUSD', N'15', 1.25, 1.24, 1.26, 1.21, 1.25, 1.24, 1.26, 1.21, 100, CAST(N'2020-02-09T00:00:00.0000000' AS DateTime2))" +
                                       "INSERT [dbo].[Prices] ([Market], [Granularity], [BidOpen], [BidClose], [BidHigh], [BidLow], [AskOpen], [AskClose], [AskHigh], [AskLow], [TickQty], [Timestamp]) VALUES (N'TEST_GBPUSD', N'15', 1.25, 1.24, 1.26, 1.21, 1.25, 1.24, 1.26, 1.21, 100, CAST(N'2020-02-10T00:00:00.0000000' AS DateTime2))";

            using (var connection = new SqlConnection(Connection))
            {
                var command = new SqlCommand(queryString, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}