using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TicTacToe.Server.Services;
using TicTacToe.Server.Controllers;
using TicTacToe.Server.DTO;
using TicTacToe.Server.Exceptions;
using Xunit;

namespace ServerTests.ControllerTests
{
    public class AccountControllerTest
    {
        private readonly Mock<ILogger<AccountController>> _loggerMock = new();

        private readonly Mock<IAccountService> _serviceMock = new();

        private AccountController ConfigureControllerContext()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-Custom-Header"] = "statistic-test-methods";

            return new AccountController(_loggerMock.Object, _serviceMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext
                },
            };
        }

        public static readonly object[] ValidUsersData =
        {
        new object[] {"qwerty", "111111" },
        new object[] {"qwerty123", "string" },
    };

        [Theory, MemberData(nameof(ValidUsersData))]
        public async Task LoginPressValidDataShouldReturnOk(string login, string password)
        {
            //Arrange
            var account = new UserAccountDto(login, password);
            _ = _serviceMock.Setup(x => x.InvokeLoginAsync(account));
            var accountController = ConfigureControllerContext();

            //Act
            var result = await accountController.LoginAsync(account);
            var okResult = result as OkObjectResult;
            var content = okResult?.Value as string;

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(okResult);
            Assert.NotNull(content);
            Assert.Equal(login, content);
        }

        [Theory, MemberData(nameof(ValidUsersData))]
        public async Task LoginAccountShouldReturnNotFoundIfInputLoginNotExist(string login, string password)
        {
            //Arrange
            var account = new UserAccountDto(login, password);
            _ = _serviceMock.Setup(x => x.InvokeLoginAsync(account))
                .Throws<AccountException>();
            var accountController = ConfigureControllerContext();

            //Act
            var result = await accountController.LoginAsync(account);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<NotFoundObjectResult>(result);
        }

        [Theory, MemberData(nameof(ValidUsersData))]
        public async Task LoginAccountShouldReturnConflictIfUserTryLoginThreeTimes(string login,
            string password)
        {
            //Arrange
            var account = new UserAccountDto(login, password);
            _ = _serviceMock.Setup(x => x.InvokeLoginAsync(account))
                .Throws<TimeoutException>();
            var accountController = ConfigureControllerContext();

            //Act
            var result = await accountController.LoginAsync(account);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<ConflictObjectResult>(result);
        }

        [Theory, MemberData(nameof(ValidUsersData))]
        public async Task RegistrationAsyncShouldReturnOkIfInputValidData(string login, string password)
        {
            //Arrange
            var account = new UserAccountDto(login, password);
            _ = _serviceMock.Setup(x => x.InvokeRegistrationAsync(account));
            var accountController = ConfigureControllerContext();

            //Act
            var result = await accountController.RegistrationAsync(account);
            var okResult = result as OkObjectResult;
            var content = okResult?.Value as string;

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(okResult);
            Assert.NotNull(content);
            Assert.Equal(login, content);
        }

        [Theory, MemberData(nameof(ValidUsersData))]
        public async Task RegistrationAsyncShouldReturnConflictIfUserAlreadyExit(string login, string password)
        {
            //Arrange
            var account = new UserAccountDto(login, password);
            _ = _serviceMock.Setup(x => x.InvokeRegistrationAsync(account))
                .Throws<AccountException>();
            var accountController = ConfigureControllerContext();

            //Act
            var result = await accountController.RegistrationAsync(account);

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<ConflictObjectResult>(result);
        }

        [Theory]
        [InlineData("qwerty")]
        [InlineData("qwerty123")]
        public async Task LogoutAsyncShouldReturnOk(string login)
        {
            //Arrange
            _ = _serviceMock.Setup(x => x.RemoveActiveAccountByLogin(login));

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Login"] = login;
            var accountController = new AccountController(_loggerMock.Object, _serviceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            //Act
            var result = await accountController.LogoutAsync();

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task LogoutAsyncShouldReturnUnAuthorized(string login)
        {
            //Arrange
            _ = _serviceMock.Setup(x => x.RemoveActiveAccountByLogin(login))
                .Throws<AccountException>();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Login"] = login;
            var accountController = new AccountController(_loggerMock.Object, _serviceMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext
                }
            };

            //Act
            var result = await accountController.LogoutAsync();

            //Assert
            Assert.NotNull(result);
            _ = Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
