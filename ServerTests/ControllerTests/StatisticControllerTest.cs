using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TicTacToe.Server.Enums;
using TicTacToe.Server.Services;
using TicTacToe.Server.Controllers;
using TicTacToe.Server.DTO;
using Xunit;

namespace ServerTests.ControllerTests
{
    public class StatisticControllerTest
    {
        private readonly Mock<ILogger<StatisticController>> _loggerMock = new();

        private readonly Mock<IStatisticService> _serviceMock = new();

        private const string ParseFormat = "dd.MM.yyyy HH:mm";

        public static readonly object[] CorrectUserData =
        {
        new object[] { "qwerty", DateTime.MinValue, DateTime.MaxValue },
        new object[] { "qwerty123", DateTime.ParseExact("14.03.2022 12:00", ParseFormat, CultureInfo.InvariantCulture),
            DateTime.ParseExact("16.03.2022 20:00", ParseFormat, CultureInfo.InvariantCulture) },
        new object[] { "alex", DateTime.ParseExact("09.03.2022 00:00", ParseFormat, CultureInfo.InvariantCulture),
            DateTime.ParseExact("10.03.2022 20:00", ParseFormat, CultureInfo.InvariantCulture) },
    };

        public static readonly object[] InvalidUserData =
        {
        new object[] { "", DateTime.MinValue, DateTime.MaxValue },
        new object[] { null!, DateTime.ParseExact("14.03.2022 12:00", ParseFormat, CultureInfo.InvariantCulture),
            DateTime.ParseExact("16.03.2022 20:00", ParseFormat, CultureInfo.InvariantCulture) },
        new object[] { null!, DateTime.ParseExact("09.03.2022 00:00", ParseFormat, CultureInfo.InvariantCulture),
            DateTime.ParseExact("10.03.2022 20:00", ParseFormat, CultureInfo.InvariantCulture) },
    };

        private StatisticController ConfigureControllerContext(string name)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-Custom-Header"] = "statistic-test-methods";

            return new StatisticController(_serviceMock.Object, _loggerMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext
                },
                LoginUser = name
            };
        }

        [Theory]
        [InlineData("qwerty")]
        [InlineData("qwerty123")]
        [InlineData("alex")]
        public async Task GetPrivateStatisticTestShouldReturnOkAndContent(string name)
        {
            // Arrange
            _ = _serviceMock.Setup(x => x.GetPrivateStatisticAsync(name, DateTime.MinValue,
                      DateTime.MaxValue))
                .Returns(GetPrivateStatistic());

            var statisticController = ConfigureControllerContext(name);

            // Act
            var result = await statisticController.GetPrivateStatisticAsync();

            // Assert
            Assert.NotNull(result);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var content = okResult?.Value as PrivateStatisticDto;
            Assert.NotNull(content);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null!)]
        public async Task GetPrivateStatisticShouldReturnUnauthorized(string name)
        {
            // Arrange
            var statisticController = ConfigureControllerContext(name);

            //Act
            var result = await statisticController.GetPrivateStatisticAsync();

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Theory, MemberData(nameof(CorrectUserData))]
        public async Task GetPrivateStatisticInTimeIntervalShouldReturnOk(string name,
            DateTime startTime, DateTime endTime)
        {
            // Arrange
            _ = _serviceMock.Setup(x => x.GetPrivateStatisticAsync(name, startTime, endTime))
                .Returns(GetPrivateStatistic());

            var statisticController = ConfigureControllerContext(name);

            //Act
            var result = await statisticController
                .GetPrivateStatisticInTimeIntervalAsync(new TimeIntervalDto(startTime, endTime));

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<OkObjectResult>(result);
        }

        [Theory, MemberData(nameof(InvalidUserData))]
        public async Task GetPrivateStatisticInTimeIntervalShouldReturnUnauthorized(string name,
            DateTime startTime, DateTime endTime)
        {
            // Arrange
            var statisticController = ConfigureControllerContext(name);

            //Act
            var result = await statisticController
                .GetPrivateStatisticInTimeIntervalAsync(new TimeIntervalDto(startTime, endTime));

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Theory]
        [InlineData(SortingType.Losses)]
        public async Task GetLeadersStatisticShouldReturnOkAndContent(SortingType type)
        {
            // Arrange
            _ = _serviceMock.Setup(x => x.GetLeadersAsync(type))
                .Returns(GetLeadersStatistic());

            var statisticController = ConfigureControllerContext("");

            //Act
            var result = await statisticController.GetLeadersStatisticAsync(type);

            //Assert
            Assert.NotNull(result);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var content = okResult?.Value as List<LeaderStatisticDto>;
            Assert.NotNull(content);
            Assert.Equal(3, content?.Count);
        }

        private static Task<PrivateStatisticDto> GetPrivateStatistic()
        {
            return Task.FromResult(new PrivateStatisticDto(2, 8, 10, 60,
                new List<int>(), 3, new List<int>(), 5,
                new TimeSpan(7, 10, 40)));
        }

        private static Task<List<LeaderStatisticDto>> GetLeadersStatistic()
        {
            List<LeaderStatisticDto> list = new()
            {
                new LeaderStatisticDto("qwerty", 6, 3, 10, TimeSpan.Parse("6:00:00")),
                new LeaderStatisticDto("qwerty123", 14, 5, 20, TimeSpan.Parse("12:00:00")),
                new LeaderStatisticDto("alex", 20, 10, 20, TimeSpan.Parse("9:00:00"))
            };
            return Task.FromResult(list);
        }
    }
}
