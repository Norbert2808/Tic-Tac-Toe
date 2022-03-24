
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using TicTacToe.Server.DTO;
using TicTacToe.Server.Exceptions;
using TicTacToe.Server.Services.Impl;
using TicTacToe.Server.Tools;
using Xunit;

namespace ServerTests.ServiceTest;

public class AccountServiceTest
{
    private readonly Mock<IBlocker> _blockerMock;

    private readonly Mock<IJsonHelper<UserAccountDto>> _jsonMock;

    public AccountServiceTest()
    {
        _blockerMock = new Mock<IBlocker>();
        _jsonMock = new Mock<IJsonHelper<UserAccountDto>>();
    }

    [Fact]
    public async Task InvokeLoginAsyncShouldThrowTimeoutExceptionWhenUserFailedLoginMoreThanThreeTimes()
    {
        //Arrange
        var userAcc = new UserAccountDto("qwerty", "111111");

        _blockerMock.Setup(x => x.IsBlocked(userAcc.Login))
            .Returns(true);
        _jsonMock.Setup(x => x.DeserializeAsync())
            .ReturnsAsync(new List<UserAccountDto> {new("qwerty", "111111")});
        var accountService = new AccountService(_blockerMock.Object, _jsonMock.Object);

        //Act
        try
        {
            await accountService.InvokeLoginAsync(userAcc);
            Assert.True(false, "Expected exception was not thrown");
        }
        catch (TimeoutException)
        {
            Assert.True(true, "Expected exception was thrown");
        }
    }

    [Fact]
    public async Task InvokeLoginAsyncShouldLoginUser()
    {
        //Arrange
        var userAcc = new UserAccountDto("qwertyuio", "111111");

        _blockerMock.Setup(x => x.IsBlocked(userAcc.Login))
            .Returns(false);
        _jsonMock.Setup(x => x.DeserializeAsync())
            .ReturnsAsync(new List<UserAccountDto> {new("qwertyuio", "111111")});
        var accountService = new AccountService(_blockerMock.Object, _jsonMock.Object);

        //Act
        await accountService.InvokeLoginAsync(userAcc);

        //Assert
        Assert.True(true);
    }

    [Fact]
    public async Task InvokeLoginAsyncShouldThrowsAccountExceptionWhenInvalidLogin()
    {
        //Arrange
        var userAcc = new UserAccountDto("vvvvvvvv", "111111");

        _blockerMock.Setup(x => x.IsBlocked(userAcc.Login))
            .Returns(false);
        _jsonMock.Setup(x => x.DeserializeAsync())
            .ReturnsAsync(new List<UserAccountDto> {new("qwerty", "111111")});
        var accountService = new AccountService(_blockerMock.Object, _jsonMock.Object);

        //Act
        try
        {
            await accountService.InvokeLoginAsync(userAcc);
            Assert.True(false, "Expected exception was not thrown");
        }
        catch (AccountException)
        {
            Assert.True(true, "Expected exception was thrown");
        }
    }

    [Fact]
    public async Task InvokeLoginAsyncShouldThrowsAccountExceptionWhenInvalidPassword()
    {
        //Arrange
        var userAcc = new UserAccountDto("qwerty", "444444");

        _blockerMock.Setup(x => x.IsBlocked(userAcc.Login))
            .Returns(false);
        _jsonMock.Setup(x => x.DeserializeAsync())
            .ReturnsAsync(new List<UserAccountDto> {new("qwerty", "111111")});
        var accountService = new AccountService(_blockerMock.Object, _jsonMock.Object);

        //Act
        try
        {
            await accountService.InvokeLoginAsync(userAcc);
            Assert.True(false, "Expected exception was not thrown");
        }
        catch (AccountException)
        {
            Assert.True(true, "Expected exception was thrown");
        }
    }

    [Fact]
    public async Task InvokeLoginAsyncShouldThrowsAccountExceptionWhenUserAlreadyLoggedIn()
    {
        //Arrange
        var userAcc = new UserAccountDto("qwerty", "111111");

        _blockerMock.Setup(x => x.IsBlocked(userAcc.Login))
            .Returns(false);
        _jsonMock.Setup(x => x.DeserializeAsync())
            .ReturnsAsync(new List<UserAccountDto> {new("qwerty", "111111")});
        var accountService = new AccountService(_blockerMock.Object, _jsonMock.Object);

        //Act
        try
        {
            // First login
            await accountService.InvokeLoginAsync(userAcc);
            //Second attempt
            await accountService.InvokeLoginAsync(userAcc);
            Assert.True(false, "Expected exception was not thrown");
        }
        catch (AccountException)
        {
            Assert.True(true, "Expected exception was thrown");
        }
    }
}
