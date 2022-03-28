using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using TicTacToe.Client.Services;
using TicTacToe.Client.States;
using TicTacToe.Client.States.Impl;
using Xunit;

namespace ClientTests.StateTests
{
    public class AuthorizationStateTest
    {
        private readonly Mock<ILogger<AuthorizationMenuState>> _loggerMock;
        private readonly Mock<IMainMenuState> _mainStateMock;
        private readonly Mock<ILeaderMenuState> _leaderStateMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly IAuthorizationMenuState _authorizationState;

        public AuthorizationStateTest()
        {
            _loggerMock = new Mock<ILogger<AuthorizationMenuState>>();
            _mainStateMock = new Mock<IMainMenuState>();
            _leaderStateMock = new Mock<ILeaderMenuState>();
            _userServiceMock = new Mock<IUserService>();
            _authorizationState = new AuthorizationMenuState(_userServiceMock.Object,
                _mainStateMock.Object, _leaderStateMock.Object, _loggerMock.Object);
        }

        private void ConsoleReplacement()
        {
            var output = new StringWriter();
            Console.SetOut(output);

            var input = new StringReader("");
            Console.SetIn(input);
        }

        [Fact]
        public async Task ExecuteLoginVerifyMainStateInvokeMenuAsync()
        {
            _ = _userServiceMock.Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

            ConsoleReplacement();

            await _authorizationState.ExecuteLoginAsync();

            _mainStateMock.Verify(x => x.InvokeMenuAsync(), Times.Once);
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.Conflict)]
        public async Task ExecuteLoginVerifyNeverMainStateInvokeMenuAsync(HttpStatusCode code)
        {
            _ = _userServiceMock.Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new HttpResponseMessage(code)));

            ConsoleReplacement();

            await _authorizationState.ExecuteLoginAsync();

            _mainStateMock.Verify(x => x.InvokeMenuAsync(), Times.Never);
        }

        [Fact]
        public async Task ExecuteRegistrationVerifyMainStateInvokeMenuAsync()
        {
            _ = _userServiceMock.Setup(x => x.RegistrationAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

            ConsoleReplacement();

            await _authorizationState.ExecuteRegistrationAsync();

            _mainStateMock.Verify(x => x.InvokeMenuAsync(), Times.Once);
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.Conflict)]
        public async Task ExecuteRegistrationVerifyNeverMainStateInvokeMenuAsync(HttpStatusCode code)
        {
            _ = _userServiceMock.Setup(x => x.RegistrationAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new HttpResponseMessage(code)));

            ConsoleReplacement();

            await _authorizationState.ExecuteRegistrationAsync();

            _mainStateMock.Verify(x => x.InvokeMenuAsync(), Times.Never);
        }

        [Fact]
        public async Task ExecuteLeaderMenuVerifyLeaderStateInvokeMenuAsync()
        {
            await _authorizationState.ExecuteLeaderMenuAsync();

            _leaderStateMock.Verify(x => x.InvokeMenuAsync(), Times.Once);
        }
    }
}
