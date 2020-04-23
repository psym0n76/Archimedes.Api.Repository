using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Data.SqlClient;
using System.Threading;

namespace Archimedes.Api.Repository.Tests
{
    [TestFixture]
    public class PriceRepositoryTests
    {
        private static Price _price;
        private static Price _priceTwo;
        private static List<Price> _prices;
        private const string Connection = "Server=localhost\\SQLEXPRESS;Database=Archimedes;Integrated Security=SSPI;";

        [Test]
        public async Task Should_ReturnOk_When_PostingPriceData()
        {
            try
            {
                var repo = GetRepository();
                await repo.AddPricesAsync(_prices,CancellationToken.None);
                await repo.FxDatabaseContext.SaveChangesAsync();
                DeleteTestData();
                Assert.IsTrue(true);
            }
            catch
            {
                Assert.IsTrue(false);
            }
        }

        [Test]
        public async Task Should_ReturnTwoRecords_When_PriceCalled()
        {
            var repo = GetRepository();
            AddTestData();
            var result = await repo.GetPricesAsync(x => x.Market == "TEST_GBPUSD", CancellationToken.None);

            Assert.IsInstanceOf(typeof(IEnumerable<Price>), result);
            Assert.IsTrue(result.Count() == 5);
            DeleteTestData();
        }

        [Test]
        public async Task Should_ReturnNoRecords_When_PriceCalled()
        {
            var repo = GetRepository();
            AddTestData();
            var result = await repo.GetPricesAsync(x => x.Market == "GBPUSD", CancellationToken.None);

            Assert.IsInstanceOf(typeof(IEnumerable<Price>), result);
            Assert.IsTrue(!result.Any());
            DeleteTestData();
        }

        [Test]
        public async Task Should_ReturnFiveRecords_When_PricesCalled()
        {
            var repo = GetRepository();
            AddTestData();
            var result = await repo.GetPricesAsync(1, 100, CancellationToken.None);

            Assert.IsInstanceOf(typeof(IEnumerable<Price>), result);
            Assert.IsTrue(result.Count() == 5);
            DeleteTestData();
        }

        [Test]
        public async Task Should_ReturnOneRecord_When_PricesCalledWithPaging()
        {
            var repo = GetRepository();
            AddTestData();
            var result = await repo.GetPricesAsync(1, 1, CancellationToken.None);

            Assert.IsInstanceOf(typeof(IEnumerable<Price>), result);
            Assert.IsTrue(result.Count() == 1);
            DeleteTestData();
        }

        [Test]
        public async Task Should_ReturnNilRecords_When_TruncateCalled()
        {
            var repo = GetRepository();
            AddTestData();
            repo.Truncate();

            var result = await repo.GetPricesAsync(1, 100, CancellationToken.None);

            Assert.IsInstanceOf(typeof(IEnumerable<Price>), result);
            Assert.IsTrue(!result.Any());
            DeleteTestData();
        }

        private static PriceRepository GetRepository()
        {
            var option = new DbContextOptionsBuilder<ArchimedesContext>();
            option.UseSqlServer(Connection);

            return new PriceRepository(new ArchimedesContext(option.Options));
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
                Timestamp = new DateTime(2020, 01, 01)
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
                Timestamp = new DateTime(2020, 01, 01)
            };

            _prices = new List<Price>() {_price, _priceTwo};
        }

        [OneTimeTearDown]
        public void RunAfterTests()
        {
            DeleteTestData();
        }

        private static void DeleteTestData()
        {
            using (var connection = new SqlConnection(Connection))
            {
                const string queryString = "DELETE FROM PRICES WHERE Market = 'TEST_GBPUSD'";
                var command = new SqlCommand(queryString, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private static void AddTestData()
        {
            const string queryString =
                "INSERT [dbo].[Prices] ([Market], [Granularity], [BidOpen], [BidClose], [BidHigh], [BidLow], [AskOpen], [AskClose], [AskHigh], [AskLow], [TickQty], [Timestamp]) VALUES (N'TEST_GBPUSD', NULL, 0, 0, 0, 0, 0, 0, 0, 0, 0, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2))" +
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