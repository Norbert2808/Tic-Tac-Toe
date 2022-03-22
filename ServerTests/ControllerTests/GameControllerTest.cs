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

namespace ServerTests.ControllerTests
{
    public class GameControllerTest
    {
        private readonly Mock<ILogger<GameController>> _loggerMock = new();

        private readonly Mock<IRoomService> _serviceMock = new();

        public static readonly object[] CorrectUserName = { new object[] { "qwerty" } };

        public static readonly object[] InvalidUserName = { new object[] { "" }, new object[] { null! } };

        private GameController ConfigureControllerContext(string userLogin)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-Custom-Header"] = "game-test-methods";

            return new GameController(_serviceMock.Object, _loggerMock.Object)
            {
                ControllerContext = new ControllerContext() { HttpContext = httpContext },
                LoginUser = userLogin
            };
        }

        [Theory, MemberData(nameof(CorrectUserName))]
        public async Task CreateRoomAsyncShouldReturnOkIfValidParameters(string login)
        {
            //Arrange
            var roomId = Guid.NewGuid().ToString();
            var settings = new RoomSettingsDto() { RoomId = roomId };
            _ = _serviceMock.Setup(x => x.StartRoomAsync(settings.RoomId, login, settings))
                .ReturnsAsync(roomId);
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

        [Theory, MemberData(nameof(InvalidUserName))]
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
            var settings = new RoomSettingsDto { RoomId = roomId };
            _ = _serviceMock.Setup(x => x.StartRoomAsync(settings.RoomId, login, settings))
                .Throws<RoomException>();
            var gameController = ConfigureControllerContext(login);

            //Act
            var result = await gameController.StartRoomAsync(settings);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory, MemberData(nameof(InvalidUserName))]
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
            _ = _serviceMock.Setup(x => x.CheckRoomAsync(string.Empty))
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
            _ = _serviceMock.Setup(x => x.CheckRoomAsync(string.Empty))
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
            _ = _serviceMock.Setup(x => x.CheckRoomAsync(string.Empty))
                .ReturnsAsync((true, string.Empty));
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
            _ = _serviceMock.Setup(x => x.CheckMoveAsync(string.Empty, login))
                .ReturnsAsync(roundState);
            var gameController = ConfigureControllerContext(login);

            //Act
            var result = await gameController.CheckMoveAsync(string.Empty);

            //Assert
            Assert.NotNull(result);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(roundState, okResult?.Value as RoundStateDto);
        }

        [Theory, MemberData(nameof(InvalidUserName))]
        public async Task CheckMoveAsyncShouldReturnUnauthorizedIfUserNonAuthorized(string login)
        {
            //Arrange
            var gameController = ConfigureControllerContext(login);

            //Act
            var result = await gameController.CheckMoveAsync(string.Empty);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Theory, MemberData(nameof(CorrectUserName))]
        public async Task CheckMoveAsyncShouldReturnNotFoundIfRoomNotFound(string login)
        {
            //Arrange
            var gameController = ConfigureControllerContext(login);
            _ = _serviceMock.Setup(x => x.CheckMoveAsync(string.Empty, login))
                .Throws<RoomException>();

            //Act
            var result = await gameController.CheckMoveAsync(string.Empty);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<NotFoundObjectResult>(result);
        }

        [Theory, MemberData(nameof(CorrectUserName))]
        public async Task CheckMoveAsyncShouldReturnConflictIfMoveTimeExpires(string login)
        {
            //Arrange
            var gameController = ConfigureControllerContext(login);
            _ = _serviceMock.Setup(x => x.CheckMoveAsync(string.Empty, login))
                .Throws<TimeoutException>();

            //Act
            var result = await gameController.CheckMoveAsync(string.Empty);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<ConflictObjectResult>(result);
        }

        [Theory, MemberData(nameof(CorrectUserName))]
        public async Task CheckMoveAsyncShouldReturnConflictIfGameOver(string login)
        {
            //Arrange
            var gameController = ConfigureControllerContext(login);
            _ = _serviceMock.Setup(x => x.CheckMoveAsync(string.Empty, login))
                .Throws<GameException>();

            //Act
            var result = await gameController.CheckMoveAsync(string.Empty);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<ConflictObjectResult>(result);
        }

        [Theory, MemberData(nameof(CorrectUserName))]
        public async Task MoveAsyncShouldReturnOk(string login)
        {
            //Arrange
            var move = new MoveDto(0, 1);
            _ = _serviceMock.Setup(x => x.DoMoveAsync(string.Empty,
                  login, move))
                .Returns(Task.CompletedTask);
            var gameController = ConfigureControllerContext(login);

            //Act
            var result = await gameController.MoveAsync(string.Empty, move);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<OkResult>(result);
        }

        [Theory, MemberData(nameof(InvalidUserName))]
        public async Task MoveAsyncShouldReturnUnauthorizedIfUserNonAuthorized(string login)
        {
            //Arrange
            var gameController = ConfigureControllerContext(login);

            //Act
            var result = await gameController.MoveAsync(string.Empty, new MoveDto(1, 1));

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Theory, MemberData(nameof(CorrectUserName))]
        public async Task MoveAsyncShouldReturnNotFoundIfOpponentLeftFromRoom(string login)
        {
            //Arrange
            var move = new MoveDto(1, 1);
            _ = _serviceMock.Setup(x => x.DoMoveAsync(string.Empty, login, move))
                .Throws<RoomException>();
            var gameController = ConfigureControllerContext(login);

            //Act
            var result = await gameController.MoveAsync(string.Empty, move);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<NotFoundObjectResult>(result);
        }

        [Theory, MemberData(nameof(CorrectUserName))]
        public async Task MoveAsyncShouldReturnBadRequestIfNotValidInputDataForMove(string login)
        {
            //Arrange
            var move = new MoveDto(1, 1);
            _ = _serviceMock.Setup(x => x.DoMoveAsync(string.Empty, login, move))
                .Throws<ArgumentException>();
            var gameController = ConfigureControllerContext(login);

            //Act
            var result = await gameController.MoveAsync(string.Empty, move);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory, MemberData(nameof(CorrectUserName))]
        public async Task MoveAsyncShouldReturnConflictIfMoveTimeIsOver(string login)
        {
            //Arrange
            var move = new MoveDto(1, 1);
            _ = _serviceMock.Setup(x => x.DoMoveAsync(string.Empty, login, move))
                .Throws<TimeoutException>();
            var gameController = ConfigureControllerContext(login);

            //Act
            var result = await gameController.MoveAsync(string.Empty, move);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<ConflictObjectResult>(result);
        }

        [Theory, MemberData(nameof(CorrectUserName))]
        public async Task SendConfirmationAsyncShouldReturnOk(string login)
        {
            //Arrange
            _ = _serviceMock.Setup(x => x.AppendConfirmationAsync(true, string.Empty))
                .Returns(Task.CompletedTask);
            var gameController = ConfigureControllerContext(login);

            //Act
            var result = await gameController.AppendConfirmationAsync(string.Empty, true);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<OkResult>(result);
        }

        [Theory, MemberData(nameof(InvalidUserName))]
        public async Task SendConfirmationAsyncShouldReturnUnauthorizedIfUserNonAuthorized(string login)
        {
            //Arrange
            var gameController = ConfigureControllerContext(login);

            //Act
            var result = await gameController.AppendConfirmationAsync(string.Empty, true);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Theory, MemberData(nameof(CorrectUserName))]
        public async Task SendConfirmationAsyncShouldReturnConflictIfTimeoutOrOpponentLeftFromRoom(string login)
        {
            var gameController = ConfigureControllerContext(login);

            _ = _serviceMock.Setup(x => x.AppendConfirmationAsync(true, string.Empty))
                .Throws<TimeoutException>();

            var resultTimeout = await gameController.AppendConfirmationAsync(string.Empty, true);

            _ = _serviceMock.Setup(x => x.AppendConfirmationAsync(true, string.Empty))
                .Throws<RoomException>();

            var resultOpponentLeft = await gameController.AppendConfirmationAsync(string.Empty, true);

            Assert.NotNull(resultTimeout);
            _ = Assert.IsType<ConflictObjectResult>(resultTimeout);

            Assert.NotNull(resultOpponentLeft);
            _ = Assert.IsType<ConflictObjectResult>(resultOpponentLeft);
        }

        [Theory, MemberData(nameof(CorrectUserName))]
        public async Task CheckConfirmationAsyncShouldReturnOk(string login)
        {
            //Arrange
            var gameController = ConfigureControllerContext(login);
            _ = _serviceMock.Setup(x => x.CheckConfirmationAsync(string.Empty, login))
                .ReturnsAsync((true, string.Empty));
            //Act
            var result = await gameController.CheckConfirmationAsync(string.Empty);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<OkResult>(result);
        }

        [Theory, MemberData(nameof(InvalidUserName))]
        public async Task CheckConfirmationAsyncShouldReturnUnauthorizedIfUserNonAuthorized(string login)
        {
            //Arrange
            var gameController = ConfigureControllerContext(login);

            //Act
            var result = await gameController.CheckConfirmationAsync(string.Empty);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Theory, MemberData(nameof(CorrectUserName))]
        public async Task CheckConfirmationAsyncShouldReturnConflictIfOpponentLeftFromRoomOrTimeout(string login)
        {
            var gameController = ConfigureControllerContext(login);

            _ = _serviceMock.Setup(x => x.CheckConfirmationAsync(string.Empty, login))
                .Throws<RoomException>();

            var resultOpponentLeft = await gameController.CheckConfirmationAsync(string.Empty);

            Assert.NotNull(resultOpponentLeft);
            _ = Assert.IsType<ConflictObjectResult>(resultOpponentLeft);

            _ = _serviceMock.Setup(x => x.CheckConfirmationAsync(string.Empty, login))
                .Throws<TimeoutException>();

            var resultTimeout = await gameController.CheckConfirmationAsync(string.Empty);

            Assert.NotNull(resultTimeout);
            _ = Assert.IsType<ConflictObjectResult>(resultTimeout);
        }

        [Theory, MemberData(nameof(CorrectUserName))]
        public async Task GetResultAsyncShouldReturnOkAndContent(string login)
        {
            //Arrange
            var gameController = ConfigureControllerContext(login);
            var roundResults = new ResultsDto();
            _ = _serviceMock.Setup(x => x.GetResultAsync(string.Empty))
                .ReturnsAsync(roundResults);

            //Act
            var result = await gameController.GetResultsAsync(string.Empty);

            //Assert
            Assert.NotNull(result);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var content = okResult?.Value as ResultsDto;
            Assert.Equal(roundResults, content);
        }

        [Theory, MemberData(nameof(InvalidUserName))]
        public async Task GetResultAsyncShouldReturnUnauthorizedIfUserNonAuthorized(string login)
        {
            //Arrange
            var gameController = ConfigureControllerContext(login);

            //Act
            var result = await gameController.GetResultsAsync(string.Empty);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Theory, MemberData(nameof(CorrectUserName))]
        public async Task GetResultAsyncShouldReturnNotFoundIfRoomNotFound(string login)
        {
            //Arrange
            var gameController = ConfigureControllerContext(login);

            _ = _serviceMock.Setup(x => x.GetResultAsync(string.Empty))
                .Throws<RoomException>();

            //Act
            var result = await gameController.GetResultsAsync(string.Empty);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<NotFoundObjectResult>(result);
        }

        [Theory, MemberData(nameof(CorrectUserName))]
        public async Task SurrenderAsyncShouldReturnOk(string login)
        {
            //Arrange
            var gameController = ConfigureControllerContext(login);
            _ = _serviceMock.Setup(x => x.SurrenderAsync(string.Empty, login))
                .Returns(Task.CompletedTask);

            //Act
            var result = await gameController.SurrenderAsync(login);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<OkResult>(result);
        }

        [Theory, MemberData(nameof(CorrectUserName))]
        public async Task SurrenderAsyncShouldReturnBadRequestIfRoomNotFound(string login)
        {
            //Arrange
            var gameController = ConfigureControllerContext(login);
            _ = _serviceMock.Setup(x => x.SurrenderAsync(string.Empty, login))
                .Throws<RoomException>();

            //Act
            var result = await gameController.SurrenderAsync(string.Empty);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory, MemberData(nameof(InvalidUserName))]
        public async Task SurrenderAsyncShouldReturnUnauthorizedIfUserNonAuthorized(string login)
        {
            //Arrange
            var gameController = ConfigureControllerContext(login);

            //Act
            var result = await gameController.SurrenderAsync(string.Empty);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Theory, MemberData(nameof(CorrectUserName))]
        public async Task ExitFromRoomAsyncShouldReturnOk(string login)
        {
            //Arrange
            var gameController = ConfigureControllerContext(login);

            _ = _serviceMock.Setup(x => x.ExitFromRoomAsync(string.Empty))
                .Returns(Task.CompletedTask);

            //Act
            var result = await gameController.ExitFromRoomAsync(string.Empty);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<OkResult>(result);
        }

        [Theory, MemberData(nameof(InvalidUserName))]
        public async Task ExitFromRoomAsyncShouldReturnUnauthorizedIfUserNonAuthorized(string login)
        {
            //Arrange
            var gameController = ConfigureControllerContext(login);

            //Act
            var result = await gameController.ExitFromRoomAsync(string.Empty);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Theory, MemberData(nameof(CorrectUserName))]
        public async Task ExitFromRoomAsyncShouldReturnNotFoundIfRoomCompleted(string login)
        {
            //Arrange
            var gameController = ConfigureControllerContext(login);

            _ = _serviceMock.Setup(x => x.ExitFromRoomAsync(string.Empty))
                .Throws<RoomException>();

            //Act
            var result = await gameController.ExitFromRoomAsync(string.Empty);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<NotFoundObjectResult>(result);
        }

        [Theory, MemberData(nameof(CorrectUserName))]
        public async Task CheckRoundStateAsyncShouldReturnOkAndContent(string login)
        {
            //Arrange
            var gameController = ConfigureControllerContext(login);
            var roundState = new RoundStateDto();
            _ = _serviceMock.Setup(x => x.CheckStateRoundAsync(string.Empty, login))
                .ReturnsAsync(roundState);

            //Act
            var result = await gameController.CheckStateRoundAsync(string.Empty);

            //Assert
            Assert.NotNull(result);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var content = okResult?.Value as RoundStateDto;
            Assert.Equal(roundState, content);
        }

        [Theory, MemberData(nameof(InvalidUserName))]
        public async Task CheckRoundStateAsyncShouldReturnUnauthorizedIfUserNonAuthorized(string login)
        {
            //Arrange
            var gameController = ConfigureControllerContext(login);

            //Act
            var result = await gameController.CheckStateRoundAsync(string.Empty);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Theory, MemberData(nameof(CorrectUserName))]
        public async Task CheckRoundStateAsyncShouldReturnNotFoundIfRoomNotFound(string login)
        {
            //Arrange
            var gameController = ConfigureControllerContext(login);

            _ = _serviceMock.Setup(x => x.CheckStateRoundAsync(string.Empty, login))
                .Throws<RoomException>();

            //Act
            var result = await gameController.CheckStateRoundAsync(string.Empty);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
