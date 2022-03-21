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

    public static readonly object[] CorrectUserName = {new object[] {"qwerty"}, new object[] {"qwerty123"}};

    private GameController ConfigureControllerContext(string userLogin)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Custom-Header"] = "game-test-methods";

        return new GameController(_serviceMock.Object, _loggerMock.Object)
        {
            ControllerContext = new ControllerContext() {HttpContext = httpContext}, LoginUser = userLogin
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
    public async Task CreateRoomAsyncShouldReturnUnauthorizedIfUserNonAuthorized(string login)
    {
        //Arrange
        var gameController = ConfigureControllerContext(login);

        //Act
        var result = await gameController.StartRoomAsync(new RoomSettingsDto());

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
        var settings = new RoomSettingsDto {RoomId = roomId};
        _serviceMock.Setup(x => x.StartRoomAsync(settings.RoomId, login, settings))
            .Throws<RoomException>();
        var gameController = ConfigureControllerContext(login);

        //Act
        var result = await gameController.StartRoomAsync(settings);

        //Assert
        Assert.NotNull(result);
        _ = Assert.IsType<BadRequestObjectResult>(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task CheckRoomAsyncShouldReturnUnauthorizedIfUserNonAuthorized(string login)
    {
        //Arrange
        var gameController = ConfigureControllerContext(login);

        //Act
        var result = await gameController.CheckRoomAsync(string.Empty);

        //Assert
        Assert.NotNull(result);
        _ = Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Theory, MemberData(nameof(CorrectUserName))]
    public async Task CheckRoomAsyncShouldReturnBadRequestIfOpponentLeftFromRoom(string login)
    {
        //Arrange
        _serviceMock.Setup(x => x.CheckRoomAsync(string.Empty))
            .Throws<RoomException>();
        var gameController = ConfigureControllerContext(login);

        //Act
        var result = await gameController.CheckRoomAsync(string.Empty);

        //Assert
        Assert.NotNull(result);
        _ = Assert.IsType<BadRequestObjectResult>(result);
    }

    [Theory, MemberData(nameof(CorrectUserName))]
    public async Task CheckRoomAsyncShouldReturnConflictIfOpponentLeftFromRoom(string login)
    {
        //Arrange
        _serviceMock.Setup(x => x.CheckRoomAsync(string.Empty))
            .Throws<TimeoutException>();
        var gameController = ConfigureControllerContext(login);

        //Act
        var result = await gameController.CheckRoomAsync(string.Empty);

        //Assert
        Assert.NotNull(result);
        _ = Assert.IsType<ConflictObjectResult>(result);
    }

    [Theory, MemberData(nameof(CorrectUserName))]
    public async Task CheckRoomAsyncShouldReturnOk(string login)
    {
        //Arrange
        _serviceMock.Setup(x => x.CheckRoomAsync(string.Empty))
            .Returns(Task.FromResult((true, string.Empty)));
        var gameController = ConfigureControllerContext(login);

        //Act
        var result = await gameController.CheckRoomAsync(string.Empty);

        //Assert
        Assert.NotNull(result);

        var okResult = result as OkResult;
        Assert.NotNull(okResult);
    }

    [Theory, MemberData(nameof(CorrectUserName))]
    public async Task CheckMoveAsyncShouldReturnOk(string login)
    {
        //Arrange
        var roundState = new RoundStateDto();
        _serviceMock.Setup(x => x.CheckMoveAsync(string.Empty, login))
            .Returns(Task.FromResult(roundState)!);
        var gameController = ConfigureControllerContext(login);

        //Act
        var result = await gameController.CheckMoveAsync(string.Empty);

        //Assert
        Assert.NotNull(result);

        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(roundState, okResult?.Value as RoundStateDto);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task CheckMoveAsyncShouldReturnUnauthorizedIfUserNonAuthorized(string login)
    {
        //Arrange
        var gameController = ConfigureControllerContext(login);

        //Act
        var result = await gameController.CheckMoveAsync(string.Empty);

        //Assert
        Assert.NotNull(result);
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Theory, MemberData(nameof(CorrectUserName))]
    public async Task CheckMoveAsyncShouldReturnNotFoundIfRoomNotFound(string login)
    {
        //Arrange
        var gameController = ConfigureControllerContext(login);
        _serviceMock.Setup(x => x.CheckMoveAsync(string.Empty, login))
            .Throws<RoomException>();

        //Act
        var result = await gameController.CheckMoveAsync(string.Empty);

        //Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Theory, MemberData(nameof(CorrectUserName))]
    public async Task CheckMoveAsyncShouldReturnConflictIfMoveTimeExpires(string login)
    {
        //Arrange
        var gameController = ConfigureControllerContext(login);
        _serviceMock.Setup(x => x.CheckMoveAsync(string.Empty, login))
            .Throws<TimeoutException>();

        //Act
        var result = await gameController.CheckMoveAsync(string.Empty);

        //Assert
        Assert.NotNull(result);
        Assert.IsType<ConflictObjectResult>(result);
    }

    [Theory, MemberData(nameof(CorrectUserName))]
    public async Task CheckMoveAsyncShouldReturnConflictIfGameOver(string login)
    {
        //Arrange
        var gameController = ConfigureControllerContext(login);
        _serviceMock.Setup(x => x.CheckMoveAsync(string.Empty, login))
            .Throws<GameException>();

        //Act
        var result = await gameController.CheckMoveAsync(string.Empty);

        //Assert
        Assert.NotNull(result);
        Assert.IsType<ConflictObjectResult>(result);
    }

    [Theory, MemberData(nameof(CorrectUserName))]
    public async Task MoveAsyncShouldReturnOk(string login)
    {
        //Arrange
        var move = new MoveDto(0, 1);
        _serviceMock.Setup(x => x.DoMoveAsync(string.Empty,
            login, move))
            .Returns(Task.CompletedTask);
        var gameController = ConfigureControllerContext(login);

        //Act
        var result = await gameController.MoveAsync(string.Empty, move);

        //Assert
        Assert.NotNull(result);
        Assert.IsType<OkResult>(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task MoveAsyncShouldReturnUnauthorizedIfUserNonAuthorized(string login)
    {
        //Arrange
        var gameController = ConfigureControllerContext(login);

        //Act
        var result = await gameController.MoveAsync(string.Empty, new MoveDto(1, 1));

        //Assert
        Assert.NotNull(result);
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Theory, MemberData(nameof(CorrectUserName))]
    public async Task MoveAsyncShouldReturnNotFoundIfOpponentLeftFromRoom(string login)
    {
        //Arrange
        var move = new MoveDto(1, 1);
        _serviceMock.Setup(x => x.DoMoveAsync(string.Empty, login, move))
            .Throws<RoomException>();
        var gameController = ConfigureControllerContext(login);

        //Act
        var result = await gameController.MoveAsync(string.Empty, move);

        //Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Theory, MemberData(nameof(CorrectUserName))]
    public async Task MoveAsyncShouldReturnBadRequestIfNotValidInputDataForMove(string login)
    {
        //Arrange
        var move = new MoveDto(1, 1);
        _serviceMock.Setup(x => x.DoMoveAsync(string.Empty, login, move))
            .Throws<ArgumentException>();
        var gameController = ConfigureControllerContext(login);

        //Act
        var result = await gameController.MoveAsync(string.Empty, move);

        //Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Theory, MemberData(nameof(CorrectUserName))]
    public async Task MoveAsyncShouldReturnConflictIfMoveTimeIsOver(string login)
    {
        //Arrange
        var move = new MoveDto(1, 1);
        _serviceMock.Setup(x => x.DoMoveAsync(string.Empty, login, move))
            .Throws<TimeoutException>();
        var gameController = ConfigureControllerContext(login);

        //Act
        var result = await gameController.MoveAsync(string.Empty, move);

        //Assert
        Assert.NotNull(result);
        Assert.IsType<ConflictObjectResult>(result);
    }
}
