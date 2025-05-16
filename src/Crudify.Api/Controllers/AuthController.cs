
namespace Crudify.Api.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDTO user)
    {

        if (user is { Email: null, Password: null } 
            && string.IsNullOrWhiteSpace(user.Email)
            && string.IsNullOrWhiteSpace(user.Password))
        {
            return BadRequest(new AuthResult
            {
                Errors = ["Invalid payload"],
                Success = false
            });
        }

        var result = await authService.Login(user.Email, user.Password);
        return new OkObjectResult(result);

    }
}
