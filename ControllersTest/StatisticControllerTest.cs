using System;
using System.Collections.Generic;
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

namespace ControllersTest;

public class StatisticControllerTest
{
    private readonly Mock<ILogger<StatisticController>> _loggerMock = new ();

    private readonly Mock<IStatisticService> _serviceMock = new();

    public static readonly object[] CorrectUserData =
    {
        new object[] { "qwerty", DateTime.MinValue, DateTime.MaxValue },
        new object[] { "qwerty123", DateTime.Parse("14.03.2022 12:00:43"), DateTime.Parse("16.03.2022 20:00:23") },
        new object[] { "alex", DateTime.Parse("9.03.2022 00:00:00"), DateTime.Parse("10.03.2022 20:00:23") },
    };

    public static readonly object[] InvalidUserData =
    {
        new object[] { "", DateTime.MinValue, DateTime.MaxValue },
        new object[] { null!, DateTime.Parse("14.03.2022 12:00:43"), DateTime.Parse("16.03.2022 20:00:23") },
        new object[] { null!, DateTime.Parse("9.03.2022 00:00:00"), DateTime.Parse("10.03.2022 20:00:23") },
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
        _serviceMock.Setup(x => x.GetPrivateStatisticAsync(name, DateTime.MinValue,
                DateTime.MaxValue))
            .Returns(GetPrivateStatistic());

        var statisticController = ConfigureControllerContext(name);

        // Act
        var result = await statisticController.GetPrivateStatisticAsync();
        var okResult = result as OkObjectResult;
        var content = okResult?.Value as PrivateStatisticDto;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(okResult);
        Assert.NotNull(content);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null!)]
    public async Task GetPrivateStatisticShouldReturnUnauthorized(string name)
    {
        // Arrange
        _serviceMock.Setup(x => x.GetPrivateStatisticAsync(name, DateTime.MinValue,
                DateTime.MaxValue))
            .Returns((Task<PrivateStatisticDto>)null!);

        var statisticController = ConfigureControllerContext(name);

        //Act
        var result = await statisticController.GetPrivateStatisticAsync();

        //Assert
        Assert.NotNull(result);
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Theory, MemberData(nameof(CorrectUserData))]
    public async Task GetPrivateStatisticInTimeIntervalShouldReturnOk(string name,
        DateTime startTime, DateTime endTime)
    {
        // Arrange
        _serviceMock.Setup(x => x.GetPrivateStatisticAsync(name, startTime, endTime))
            .Returns(GetPrivateStatistic());

        var statisticController = ConfigureControllerContext(name);

        //Act
        var result = await statisticController
            .GetPrivateStatisticInTimeIntervalAsync(new TimeIntervalDto(startTime, endTime));

        //Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
    }

    [Theory, MemberData(nameof(InvalidUserData))]
    public async Task GetPrivateStatisticInTimeIntervalShouldReturnUnauthorized(string name,
        DateTime startTime, DateTime endTime)
    {
        // Arrange
        _serviceMock.Setup(x => x.GetPrivateStatisticAsync(name, startTime, endTime))
            .Returns((Task<PrivateStatisticDto>) null!);

        var statisticController = ConfigureControllerContext(name);

        //Act
        var result = await statisticController
            .GetPrivateStatisticInTimeIntervalAsync(new TimeIntervalDto(startTime, endTime));

        //Assert
        Assert.NotNull(result);
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Theory]
    [InlineData(SortingType.Losses)]
    public async Task GetLeadersStatisticShouldReturnOkAndContent(SortingType type)
    {
        // Arrange
        _serviceMock.Setup(x => x.GetLeadersAsync(type))
            .Returns(GetLeadersStatistic());

        var statisticController = ConfigureControllerContext("");

        //Act
        var result = await statisticController.GetLeadersStatisticAsync(type);
        var okResult = result as OkObjectResult;
        var content = okResult?.Value as List<LeaderStatisticDto>;

        //Assert
        Assert.NotNull(result);
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
