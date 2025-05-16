using Crudify.Api.Controllers;
using Crudify.Application.Dtos.Auth;
using Crudify.Application.Dtos.Auth.Request;
using Crudify.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Crudify.Api.Tests
{
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
        public async Task Login_ValidModelState_ReturnsOkObjectResult()
        {
            // Arrange
            var loginDto = new LoginUserDTO
            {
                Email = "test@example.com",
                Password = "password123"
            };

            var expectedResult = new AuthResult
            {
                Token = "mock-token",
                Success = true
            };

            _mockAuthService
                .Setup(s => s.Login(loginDto.Email, loginDto.Password))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var authResult = okResult.Value as AuthResult;
            Assert.IsNotNull(authResult);
            Assert.IsTrue(authResult.Success);
            Assert.AreEqual("mock-token", authResult.Token);
        }

        [TestMethod]
        public async Task Login_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new LoginUserDTO() { Email = string.Empty, Password = string.Empty};
            _controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual(400, badRequest.StatusCode);

            var authResult = badRequest.Value as AuthResult;
            Assert.IsNotNull(authResult);
            Assert.IsFalse(authResult.Success);
            CollectionAssert.Contains(authResult.Errors, "Invalid payload");
        }
    }
}
