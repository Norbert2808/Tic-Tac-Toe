using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TicTacToe.Server.Controllers;
using TicTacToe.Server.DTO;
using TicTacToe.Server.Exceptions;
using TicTacToe.Server.Services;
using Xunit;

namespace ServerTests.ControllerTests;

public class GameControllerTest
{
    private readonly Mock<ILogger<GameController>> _loggerMock = new();

    private readonly Mock<IRoomService> _serviceMock = new();

    public static readonly object[] CorrectUserName =
    {
        new object[] {"qwerty"}, new object[] {"qwerty123"}
    };

    private GameController ConfigureControllerContext(string userLogin)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Custom-Header"] = "statistic-test-methods";

        return new GameController(_serviceMock.Object, _loggerMock.Object)
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            },
            LoginUser = userLogin
        };
    }

    [Theory, MemberData(nameof(CorrectUserName))]
    public async Task CreateRoomAsyncShouldReturnOkIfValidParameters(string login)
    {
        //Arrange
        var roomId = Guid.NewGuid().ToString();
        var settings = new RoomSettingsDto() {RoomId = roomId};
        _serviceMock.Setup(x => x.StartRoomAsync(settings.RoomId, login, settings))
            .Returns(Task.FromResult(roomId));
        var gameController = ConfigureControllerContext(login);

        //Act
        var result = await gameController.StartRoomAsync(settings);

        //Assert
        Assert.NotNull(result);

        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult);

        var content = okResult?.Value as string;
        Assert.NotNull(content);

        Assert.Equal(roomId, content);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task CreateRoomAsyncShouldReturnUnauthorized(string login)
    {
        //Arrange
        var gameController = ConfigureControllerContext(login);

        //Act
        var result = await gameController.StartRoomAsync(null);

        //Assert
        Assert.NotNull(result);
        _ = Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Theory, MemberData(nameof(CorrectUserName))]
    public async Task CreateRoomAsyncShouldReturnBadRequestIfSettingsNull(string login)
    {
        //Arrange
        var gameController = ConfigureControllerContext(login);

        //Act
        var result = await gameController.StartRoomAsync(null);

        //Assert
        Assert.NotNull(result);
        _ = Assert.IsType<BadRequestObjectResult>(result);
    }

    [Theory, MemberData(nameof(CorrectUserName))]
    public async Task CreateRoomAsyncShouldReturnBadRequestWhenRoomAlreadyTakenOrTokenNotExit(string login)
    {
        //Arrange
        var roomId = Guid.NewGuid().ToString();
        var settings = new RoomSettingsDto { RoomId = roomId };
        _serviceMock.Setup(x => x.StartRoomAsync(settings.RoomId, login, settings))
            .Throws<RoomException>();
        var gameController = ConfigureControllerContext(login);

        //Act
        var result = await gameController.StartRoomAsync(settings);

        //Assert
        Assert.NotNull(result);
        _ = Assert.IsType<BadRequestObjectResult>(result);
    }
}
