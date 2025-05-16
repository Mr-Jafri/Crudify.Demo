using Crudify.Api.Controllers;
using Crudify.Application.Dtos.Auth;
using Crudify.Application.Dtos.Auth.Request;
using Crudify.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Crudify.Api.Tests;

[TestClass]
public sealed class AuthControllerTests
{
    private Mock<IAuthService> _mockAuthService;
    private AuthController _controller;

    [TestInitialize]
    public void Setup()
    {
        _mockAuthService = new Mock<IAuthService>();
        _controller = new AuthController(_mockAuthService.Object);
    }

    [TestMethod]
    public async Task Login_InvalidPayload_ReturnsBadRequest()
    {
        // Arrange
        var invalidUser = new LoginUserDTO
        {
            Email = null,
            Password = null
        };

        // Act
        var result = await _controller.Login(invalidUser);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);

        var authResult = badRequestResult.Value as AuthResult;
        Assert.IsNotNull(authResult);
        Assert.IsFalse(authResult.Success);
        CollectionAssert.Contains(authResult.Errors, "Invalid payload");
    }

    [TestMethod]
    public async Task Login_ValidCredentials_ReturnsOkWithAuthResult()
    {
        // Arrange
        var userDto = new LoginUserDTO
        {
            Email = "user@example.com",
            Password = "Password123"
        };

        var expectedAuthResult = new AuthResult
        {
            Success = true,
            Token = "fake-jwt-token"
        };

        _mockAuthService
            .Setup(s => s.Login(userDto.Email, userDto.Password))
            .ReturnsAsync(expectedAuthResult);

        // Act
        var result = await _controller.Login(userDto);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var actualAuthResult = okResult.Value as AuthResult;
        Assert.IsNotNull(actualAuthResult);
        Assert.IsTrue(actualAuthResult.Success);
        Assert.AreEqual(expectedAuthResult.Token, actualAuthResult.Token);
    }
}
